using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using ProtoBufRemote;

namespace DelftTools.Utils.RemoteInstanceServer
{
    class Program
    {
        private static string logLocation;
       
        private static Mutex weAreAliveMutex;
        private static bool streamsCreated;
        private static NamedPipeServerStream pipeServerStreamIn;
        private static NamedPipeServerStream pipeServerStreamOut;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern void SetDllDirectory(string lpPathName);
        
        static void Main(string[] args)
        {
            logLocation = Path.Combine(Path.GetTempPath(),
                                       string.Format("delta_shell_worker_{0}.log",
                                                     Process.GetCurrentProcess().Id));

            InitializeErrorHandling();

            // Debugger.Launch();

            if (args.Length != 6)
                throw new ArgumentException("Invalid number of arguments, quitting");

            Log("Number of arguments correct.");
            
            var pipeName = args[0];
            var useSharedMemoryArg = args[1];
            var assembliesToLoad = args[2];
            var interfaceName = args[3];
            var implName = args[4];
            var typeConverters = args[5];

            var useSharedMemory = useSharedMemoryArg == "sharedMemory=true";

            foreach (var assemblyPath in assembliesToLoad.Split(';'))
            {
                Assembly.LoadFrom(assemblyPath);
				// effectively only sets the last directory as search directory:
                SetDllDirectory(Path.GetDirectoryName(assemblyPath)); 
            }

            var interfaceType = LoadType(interfaceName);
            var implType = LoadType(implName);
            
            ProcessTypeConverters(typeConverters);

            Log("All types loaded.");

            Log("Setting up mutexes, pipename: " + pipeName);
            SetupMutexes(pipeName);
            Log("Setting up mutexes -- Done.");

            StartServer(interfaceType, implType, pipeName, useSharedMemory);
        }

        private static Type LoadType(string assemblyQualifiedName)
        {
            return Type.GetType(assemblyQualifiedName,
                                n => AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(z => z.FullName == n.FullName),
                                null, true);
        }

        private static void ProcessTypeConverters(string typeConverters)
        {
            var types = typeConverters.Split('|');
            foreach (var typeStr in types)
            {
                var inst = (ITypeToProtoConverter) Activator.CreateInstance(LoadType(typeStr));
                RemotingTypeConverters.RegisterTypeConverter(inst);
            }
        }

        private static void StartServer(Type interfaceType, Type implType, string pipeName, bool useSharedMemory)
        {
            //build the connection using named pipes
            try
            {
                pipeServerStreamIn = CreateNamedPipe(pipeName + "ctos", PipeDirection.In);
                pipeServerStreamOut = CreateNamedPipe(pipeName + "stoc", PipeDirection.Out);
                streamsCreated = true;
                pipeServerStreamIn.WaitForConnection();
                pipeServerStreamOut.WaitForConnection();
                
                //create and start the channel which will receive requests
                var channel = new StreamRpcChannel(pipeServerStreamIn, pipeServerStreamOut, useSharedMemory);

                //create the server
                var server = new RpcServer(channel);
                
                //register the service with the server. We must specify the interface explicitly since we did not use attributes
                server.GetType()
                      .GetMethod("RegisterService")
                      .MakeGenericMethod(interfaceType)
                      .Invoke(server, new[] { Activator.CreateInstance(implType) });

                server.Start();
            }
            catch (IOException e)
            {
                //swallow and exit
                Console.WriteLine("Something went wrong (pipes busy?), quitting: " + e);
                throw;
            }
        }

        private static NamedPipeServerStream CreateNamedPipe(string pipeName, PipeDirection pipeDirection, int retry=10)
        {
            try
            {
                if (retry <= 0)
                    throw new InvalidOperationException("Unable to establish named pipe server " + pipeName);

                return new NamedPipeServerStream(pipeName, pipeDirection, 1);
            }
            catch (IOException)
            {
                Thread.Sleep(50); // wait for pipe to become available..
                return CreateNamedPipe(pipeName, pipeDirection, retry - 1);
            }
        }

        #region Error handling

        [DllImport("kernel32.dll")]
        static extern ErrorModes SetErrorMode(ErrorModes uMode);

        [Flags]
        public enum ErrorModes : uint
        {
            SYSTEM_DEFAULT = 0x0,
            SEM_FAILCRITICALERRORS = 0x0001,
            SEM_NOALIGNMENTFAULTEXCEPT = 0x0004,
            SEM_NOGPFAULTERRORBOX = 0x0002,
            SEM_NOOPENFILEERRORBOX = 0x8000
        }

        private static void InitializeErrorHandling()
        {
            Log(string.Format("Instance started (pid: {0}).", Process.GetCurrentProcess().Id), clean: true); 
         
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            const ErrorModes desiredErrorModes = ErrorModes.SEM_FAILCRITICALERRORS | ErrorModes.SEM_NOGPFAULTERRORBOX;
            var dwMode = SetErrorMode(desiredErrorModes);
            SetErrorMode(dwMode | desiredErrorModes);
        }

        private static void Log(string contents, bool clean=false)
        {
            // overwrite the last-error log file
            try
            {
                // see if the problems occur if we don't log at all:
                WriteToFileExclusive(logLocation, contents, clean ? FileMode.Create : FileMode.Append);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void WriteToFileExclusive(string path, string contents, FileMode mode = FileMode.Append)
        {
            // don't allow write sharing: could block
            using (var fs = File.Open(path, mode, FileAccess.Write, FileShare.Read))
            {
                var writer = new StreamWriter(fs);
                writer.WriteLine(contents);
                writer.Flush(); //no need to dispose: we already dispose the stream
            }
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // write error logging:
            var errorMessage = string.Format("{0}: {1}", DateTime.Now, e.ExceptionObject);
            Log(errorMessage); //last error only

            AttemptToCloseStreams();

            Environment.Exit(-1); //prevent popup dialogs
        }

        private static void AttemptToCloseStreams()
        {
            if (streamsCreated)
            {
                try
                {
                    if (pipeServerStreamIn != null)
                        pipeServerStreamIn.Close();
                    pipeServerStreamIn = null;
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
                try
                {
                    if (pipeServerStreamOut != null)
                        pipeServerStreamOut.Close();
                    pipeServerStreamOut = null;
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            }
        }

        #endregion

        #region Mutexes

        private static void SetupMutexes(string mutexName)
        {
            bool created;
            weAreAliveMutex = new Mutex(true, mutexName + "_srv", out created);
            try
            {
                Log("Grabbing server mutex...");
                if (!weAreAliveMutex.WaitOne(10000)) //grab this mutex always 
                    throw new InvalidOperationException("Server mutex couldn't be obtained, conflict with other instance: fatal error!");
                Log("Server mutex grabbed");
            }
            catch (AbandonedMutexException)
            {
                Log("Server mutex grabbed (abondoned)");
                //previous instance was forcefully exited..ok..good to know
                //gulp
            }

            var t = new Thread(() => MutexWatcher(mutexName));
            t.Start();
        }

        private static void MutexWatcher(string mutexName)
        {
            bool created;
            var mutex = new Mutex(false, mutexName, out created);
            try
            {
                mutex.WaitOne();
            }
            catch (AbandonedMutexException)
            {
                //parent process was abruptly killed!..ok
            }
            mutex.ReleaseMutex(); //we don't actually want the mutex! release it

            //if we get here, the parent process was killed, so we should exit too
            Environment.Exit(11);
        }

        #endregion
    }

    public class FakeThingToKeepProjectReferenceAlive { }
}