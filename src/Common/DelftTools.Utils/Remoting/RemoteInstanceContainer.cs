using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading;
using DelftTools.Utils.Remoting.TypeConverters;
using ProtoBufRemote;

namespace DelftTools.Utils.Remoting
{
    public static class RemoteInstanceContainer
    {
        private static readonly IDictionary<object, RemoteProcessInfo> RunningInstances = new Dictionary<object, RemoteProcessInfo>();

        public static int NumInstances { get { return RunningInstances.Count; } }

        public static bool IsRemoteInstance(object o)
        {
            return RunningInstances.ContainsKey(o);
        }

        public static TInterface CreateInstance<TInterface, TImpl>(string workingDirectory = null,
                                                                   bool showConsole = false, params Assembly[] additionalAssemblies)
            where TInterface : class
            where TImpl : TInterface, new()
        {
            return CreateInstance<TInterface, TImpl>(Environment.Is64BitProcess, workingDirectory, showConsole, additionalAssemblies);
        }

        public static TInterface CreateInstance<TInterface, TImpl>(bool start64bitWorker, string workingDirectory = null, bool showConsole=false, params Assembly[] additionalAssemblies)
            where TInterface : class
            where TImpl : TInterface, new()
        {
            workingDirectory = workingDirectory ?? "";

            lock (RunningInstances)
            {
                var commName = string.Format("ds-worker-{0}-{1}-{2}",
                                             Process.GetCurrentProcess().Id,
                                             Thread.CurrentThread.ManagedThreadId,
                                             Guid.NewGuid().ToString());

                var assembliesToLoad = RemotingTypeConverters.RegisteredConverters.Select(t => t.GetType().Assembly)
                                                             .Concat(new[] {typeof (TInterface).Assembly})
                                                             .Concat(new[] {typeof (TImpl).Assembly})
                                                             .Concat(additionalAssemblies)
                                                             .Select(a => a.Location).Distinct();

                var typeConverterTypes =
                    RemotingTypeConverters.RegisteredConverters.Select(c => c.GetType().AssemblyQualifiedName).ToArray();

                bool created;
                var weAreAliveMutex = new Mutex(true, commName, out created);
                if (!weAreAliveMutex.WaitOne(1000)) //grab the mutex always
                {
                    throw new InvalidOperationException(
                        string.Format("Could not start remote instance, pipe {0} taken, existing instances: {1}",
                                      commName, RunningInstances.Count));
                }


                //use shared memory only if same bitness (both 32bit or both 64bit)
                var useSharedMemory = start64bitWorker == Environment.Is64BitProcess;
                
                //create the server process
                try
                {
                    var remoteInstanceLocation = Path.GetDirectoryName(typeof (RemoteInstanceContainer).Assembly.Location);
                    var exeName = string.Format("DelftTools.Utils.RemoteInstanceServer.{0}.exe", start64bitWorker ? "x64" : "x86");
                    var remoteInstanceExePath = Path.Combine(remoteInstanceLocation, exeName);
                    var p = Process.Start(new ProcessStartInfo(remoteInstanceExePath,
                                                               string.Format(
                                                                   @"""{0}"" {1} ""{2}"" ""{3}"" ""{4}"" ""{5}""",
                                                                   commName,
                                                                   "sharedMemory=" + (useSharedMemory ? "true" : "false"),
                                                                   string.Join(";", assembliesToLoad),
                                                                   typeof (TInterface).AssemblyQualifiedName,
                                                                   typeof (TImpl).AssemblyQualifiedName,
                                                                   string.Join("|", typeConverterTypes)))
                        {
                            WindowStyle = showConsole ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden,
                            WorkingDirectory = workingDirectory,
                        });

                    Console.WriteLine("Remote: Creating worker, remote pid: {0}", p.Id);
                    
                    //create the client
                    var channel = SetupConnection(commName, p.Id, useSharedMemory);
                    var client = new RpcClient(channel, new Mutex(false, commName + "_srv", out created));
                    
                    var service = client.GetProxy<TInterface>();

                    RunningInstances.Add(service, new RemoteProcessInfo(p, weAreAliveMutex, channel));

                    return service;
                }
                catch (Exception)
                {
                    Console.WriteLine("Remote: Failed creation: {0}", commName);
                    weAreAliveMutex.ReleaseMutex();
                    throw;
                }
            }
        }

        private static StreamRpcChannel SetupConnection(string pipeName, int pid, bool useSharedMemory)
        {
            // create stream (IPC only)
            var pipeClientStreamIn = new NamedPipeClientStream(".", pipeName + "stoc", PipeDirection.In);
            var pipeClientStreamOut = new NamedPipeClientStream(".", pipeName + "ctos", PipeDirection.Out);

            try
            {
                pipeClientStreamIn.Connect(20000); //20secs to connect, otherwise fail
                pipeClientStreamOut.Connect(1000);
            }
            catch (Exception)
            {
                // get error log:
                var errorMsg = "<no error log found>";
                var logPath = GetLogFilePath(pid);
                if (File.Exists(logPath))
                {
                    using (var fs = File.Open(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using(var reader = new StreamReader(fs))
                    {
                        errorMsg = reader.ReadToEnd();
                    }
                }
                // If you encounter this on build agents: previously this problem occurred due to the Symantec heuristic scanner
                // occassionally freezing the remote instance process on startup. This was solved by disabling the BHDrvx86 / 
                // BHDrvx64 drivers on those agents (part of the BASH technology of Symantec): 
                // 'sc config BhDrvX64 start= disabled' (note the spaces!!)
                throw new InvalidOperationException(string.Format(
                    "Unable to connect to remote instance: instance crashed during startup (virus scan heuristics?), or existing instance still running?{0}" +
                    "Last log message: {1}", Environment.NewLine, errorMsg));
            }

            //create and start the channel which will receive requests
            var channel = new StreamRpcChannel(pipeClientStreamIn, pipeClientStreamOut, useSharedMemory);
            return channel;
        }

        private static string GetLogFilePath(int pid)
        {
            return Path.Combine(Path.GetTempPath(), string.Format("delta_shell_worker_{0}.log", pid));
        }

        //private static FakeThingToKeepProjectReferenceAlive ts;

        public static void RemoveInstance(object service)
        {
            lock (RunningInstances)
            {
                RemoteProcessInfo processInfo;
                if (RunningInstances.TryGetValue(service, out processInfo))
                {
                    processInfo.EndProcess();
                    RunningInstances.Remove(service);
                }
            }
        }

        static RemoteInstanceContainer()
        {
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainUnload;
            RemotingTypeConverters.RegisterTypeConverter(new TypeToProtoConverter());
            RemotingTypeConverters.RegisterTypeConverter(new DateTimeToProtoConverter());
            RemotingTypeConverters.RegisterTypeConverter(new DateTimeArrayToProtoConverter());
            RemotingTypeConverters.RegisterTypeConverter(new TimeSpanToProtoConverter());
            //RemotingTypeConverters.RegisterTypeConverter(new ArrayToProtoConverter());
            RemotingTypeConverters.RegisterTypeConverter(new Array2DToProtoConverter<int>());
            RemotingTypeConverters.RegisterTypeConverter(new Array2DToProtoConverter<double>());
        }

        static void CurrentDomainUnload(object sender, EventArgs e)
        {
            // cleanup lingering instances
            lock (RunningInstances)
            {
                foreach (var runningInstance in RunningInstances.Values)
                {
                    runningInstance.EndProcess(false); //this thread does not own it
                }
                RunningInstances.Clear();
            }
        }

        private class RemoteProcessInfo
        {
            private readonly int owningThread;
            private readonly StreamRpcChannel channel;
            private Mutex ourMutex;
            private Process process;
            
            public RemoteProcessInfo(Process process, Mutex ourMutex, StreamRpcChannel channel)
            {
                owningThread = Thread.CurrentThread.ManagedThreadId;
                this.process = process;
                this.ourMutex = ourMutex;
                this.channel = channel;
            }

            public void EndProcess(bool releaseMutex=true)
            {
                lock (this)
                {
                    var pid = process != null ? process.Id : -1;
                    Console.WriteLine("Remote: Ending process, remote pid: {0} (release:{1})", pid, releaseMutex);

                    if (channel != null)
                        channel.Close();

                    // delete worker logfile
                    try
                    {
                        File.Delete(GetLogFilePath(pid));
                    }
                    catch (Exception e)
                    {
                        // gulp
                    }

                    if (releaseMutex)
                    {
                        if (owningThread != Thread.CurrentThread.ManagedThreadId)
                        {
                            TryKillProcess();
                            throw new InvalidOperationException(
                                "You must remove the remote instance from the same thread you created it on");
                        }

                        if (ourMutex != null)
                        {
                            ourMutex.ReleaseMutex();
                            ourMutex = null;
                        }
                    }

                    TryKillProcess();
                }
            }

            private void TryKillProcess()
            {
                if (process == null) 
                    return;

                try
                {
                    if (!process.HasExited)
                        process.Kill();
                }
                catch (Exception)
                {
                }
                process = null;
            }
        }
    }
}