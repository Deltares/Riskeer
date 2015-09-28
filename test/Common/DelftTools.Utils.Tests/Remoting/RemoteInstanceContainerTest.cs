using System;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using DelftTools.TestUtils;
using DelftTools.Utils.Remoting;
using log4net;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Remoting
{
    [TestFixture]
    [Ignore("Tests not stable enough")]
    public class RemoteInstanceContainerTest
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (RemoteInstanceContainerTest));

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void GetInstance()
        {
            // starts a new server and returns remote instance
            IRemoteClass remoteInstance = null;
            try
            {
                remoteInstance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();

                log.DebugFormat("Remote process Id: {0}", remoteInstance.ProcessId);
                Assert.AreNotEqual(Process.GetCurrentProcess().Id, remoteInstance.ProcessId);

                Assert.AreEqual(1, RemoteInstanceContainer.NumInstances);
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(remoteInstance);
            }
            Assert.AreEqual(0, RemoteInstanceContainer.NumInstances);
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void ServerShouldStopAfterInstanceIsRemovedFromContainer()
        {
            // starts a new server and returns remote instance
            int instanceCount = 0;
            IRemoteClass remoteInstance = null;
            try
            {
                remoteInstance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();

                log.DebugFormat("Remote process Id: {0}", remoteInstance.ProcessId);

                var currentProcessId = Process.GetCurrentProcess().Id;
                Assert.AreNotEqual(currentProcessId, remoteInstance.ProcessId);

                instanceCount = RemoteInstanceContainer.NumInstances;
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(remoteInstance);
            }

            Assert.AreEqual(instanceCount - 1, RemoteInstanceContainer.NumInstances);
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void AfterInstanceIsCreatedClassCanBeInstantiatedLocally()
        {
            // starts a new server and returns remote instance
            IRemoteClass remoteInstance = null;
            try
            {
                remoteInstance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();

                log.DebugFormat("Remote process Id: {0}", remoteInstance.ProcessId);

                var localInstance = new RemotingClass();

                Assert.AreNotEqual(Process.GetCurrentProcess().Id, remoteInstance.ProcessId);
                Assert.AreEqual(Process.GetCurrentProcess().Id, localInstance.ProcessId);
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(remoteInstance);
            }
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void GettingTwoInstancesShouldStartTwoProcesses()
        {
            IRemoteClass remoteInstance1 = null;
            IRemoteClass remoteInstance2 = null;
            try
            {
                remoteInstance1 = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();
                remoteInstance2 = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();

                log.DebugFormat("Remote process 1 Id: {0}", remoteInstance1.ProcessId);
                log.DebugFormat("Remote process 2 Id: {0}", remoteInstance2.ProcessId);

                Assert.AreNotEqual(remoteInstance2.ProcessId, remoteInstance1.ProcessId);

                var currentProcessId = Process.GetCurrentProcess().Id;

                Assert.AreNotEqual(currentProcessId, remoteInstance1.ProcessId);
                Assert.AreNotEqual(currentProcessId, remoteInstance2.ProcessId);

            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(remoteInstance1);
                RemoteInstanceContainer.RemoveInstance(remoteInstance2);
            }
        }

        [Test]
        [Category(TestCategory.Integration)]
        [Category(TestCategory.WorkInProgress)]
        public void StartInAnyCurrentDirectory()
        {
            // TODO: when this test is fixed, check also if the RTC-memory problem
            // is fixed (TOOLS-7224). If so, remove the RunEngineRemote initialization
            // from the RealTimeControlModel constructors. If not, remove the
            // 'set run engine remote' part from FewsAdapter.OpenProjectAndModel()

            var currentDirectoryPath = Environment.CurrentDirectory;
            var workingDirectoryPath = Path.Combine(currentDirectoryPath, "dummyStartupDir");
            Directory.CreateDirectory(workingDirectoryPath);
            Assert.IsTrue(Directory.Exists(workingDirectoryPath));

            Environment.CurrentDirectory = workingDirectoryPath;

            IRemoteClass remoteInstance = null;
            try
            {
                remoteInstance =
                    RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>(workingDirectoryPath);
                Assert.AreEqual(1, RemoteInstanceContainer.NumInstances);
            }
            finally
            {
                Environment.CurrentDirectory = currentDirectoryPath;
                Directory.Delete(workingDirectoryPath);
                RemoteInstanceContainer.RemoveInstance(remoteInstance);
            }
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void CallRemoteMethodThatGivesException()
        {
            int called = 0;
            var remoteInstance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();
            try
            {
                remoteInstance.MethodThatThrowsException();
            }
            catch (FaultException fe)
            {
                called++;
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(remoteInstance);
            }
            Assert.AreEqual(1, called);
        }

        [Test]
        [Category(TestCategory.Integration)]
        public void CopyHugeArrayUsingSharedMemory()
        {
            var remoteInstance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();

            var stopwatch = new Stopwatch();

            Console.WriteLine("Before:");
            Console.WriteLine(remoteInstance.GetMemoryInfo());

            var floatValue = 99.123f;

            var floatArray = new float[100000000]; //*4 = mb => +/- 400mb
            floatArray[floatArray.Length - 1] = floatValue;

            stopwatch.Start();

            var returnedValue = remoteInstance.CopyArrayAndReturnLastValue(floatArray);

            stopwatch.Stop();

            Console.WriteLine("After:");
            Console.WriteLine(remoteInstance.GetMemoryInfo());
                //nothing to see here because the array is not allocated on the heap

            Console.WriteLine("Time to transfer: {0}ms", stopwatch.ElapsedMilliseconds);

            RemoteInstanceContainer.RemoveInstance(remoteInstance);

            Assert.AreEqual(floatValue, returnedValue);
        }

        [Test]
        [Category(TestCategory.Integration)]
        [Ignore("Memory intensive")]
        public void CopyLargeDoubleArrayToServerHeap()
        {
            var localInstance = new RemotingClass();

            log.Debug("Memory info before remote instance is created");

            log.Debug("Local:");
            log.Debug(localInstance.GetMemoryInfo());

            {
                // starts a new server and returns remote instance
                var remoteInstance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>();
                try
                {
                    log.Debug("Memory info before allocation =======================");

                    log.Debug("Remote:");
                    log.Debug(remoteInstance.GetMemoryInfo());
                    Console.WriteLine(remoteInstance.GetMemoryInfo());

                    log.Debug("Local:");
                    log.Debug(localInstance.GetMemoryInfo());
                    {
                        var values = new byte[80000000];
                        for (var i = 0; i < 3; i++)
                        {
                            var grid = new Grid { gridValues = values };
                            remoteInstance.CopyToHeap(grid);

                            Console.WriteLine(remoteInstance.GetMemoryInfo());
                        }
                    }

                    log.Debug("Memory info after allocation =======================");

                    log.Debug("Remote:");
                    log.Debug(remoteInstance.GetMemoryInfo());
                }
                finally
                {
                    RemoteInstanceContainer.RemoveInstance(remoteInstance);
                }
            }

            GC.Collect();
            GC.Collect();
            GC.Collect();

            log.Debug("Local:");
            log.Debug(localInstance.GetMemoryInfo());

        }

        [Test]
        [Category(TestCategory.Performance)]
        [Category(TestCategory.WorkInProgress)]
        public void StartRemoteInstanceShouldBeFast()
        {
            IRemoteClass instance = null;
            try
            {
                TestHelper.AssertIsFasterThan(50,
                                              () =>
                                              instance = RemoteInstanceContainer.CreateInstance<IRemoteClass, RemotingClass>());
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(instance);
            }
        }
    }
}