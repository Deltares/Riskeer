using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using DelftTools.Utils.Remoting;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Remoting
{
    [TestFixture]
    public class RemoteInstanceContainer2Test
    {
        [Test]
        public void SimpleRemoteClass()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var square = service.GetSquare(3);
            Assert.AreEqual(9.0, square);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void CallsPerSec()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            service.GetSquare(3);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var numCalls = 15000;
            for (int i = 0; i < numCalls; i++)
                service.GetSquare(3);

            stopwatch.Stop();

            Console.WriteLine("{0}ms", stopwatch.ElapsedMilliseconds);
            Console.WriteLine("Calls per sec: {0}", (1000*numCalls)/stopwatch.ElapsedMilliseconds);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void CreateManyCheckIssues()
        {
            for (int i = 0; i < 5; i++)
            {
                var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
                var square = service.GetSquare(3);
                Assert.AreEqual(9.0, square);

                var service2 = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
                var square2 = service2.GetSquare(3);
                Assert.AreEqual(9.0, square2);
                RemoteInstanceContainer.RemoveInstance(service2);

                RemoteInstanceContainer.RemoveInstance(service);
            }
        }

        [Test]
        public void AdditionalLoggingInCaseBuildServerFails()
        {
            try
            {
                var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
                RemoteInstanceContainer.RemoveInstance(service);
            }
            catch (Exception)
            {
                var allProcesses = Process.GetProcesses().ToList();
                Console.WriteLine("There are {0} remote instances running",
                                  allProcesses.Count(p => p.ProcessName == "DelftTools.Utils.RemoteInstanceServer"));
                Thread.Sleep(10000);
                Console.WriteLine("(10 secs later) There are now {0} remote instances running",
                                  allProcesses.Count(p => p.ProcessName == "DelftTools.Utils.RemoteInstanceServer"));
            }
        }

        [Test]
        [Ignore("Manual only: quite intense")]
        public void CreateRemoteInstancesFromMultipleDomainsCheckIssues()
        {
            var localInstance = new StartManyInstancesClass();
            var anotherDomain = AppDomain.CreateDomain("AltDomain", null, AppDomain.CurrentDomain.SetupInformation);
            var type = typeof(StartManyInstancesClass);
            var otherDomainInstance = (StartManyInstancesClass)anotherDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            otherDomainInstance.StartInstances();
            localInstance.StartInstances();
        }

        private class StartManyInstancesClass : MarshalByRefObject
        {
            public void StartInstances()
            {
                for (int i = 0; i < 50; i++)
                {
                    var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
                    //var square = service.GetSquare(3);
                    //RemoteInstanceContainer.RemoveInstance(service);
                }
            }
        }

        [Test]
        [Ignore("Manual only: quite intense")]
        public void CheckRecoverAfterCrash()
        {
            int exceptions = 0;
            AppDomain.CurrentDomain.UnhandledException += (s, e) => exceptions++;

            var threadsBefore = Process.GetCurrentProcess().Threads.Count;
            Console.WriteLine("threads:" + threadsBefore);

            const int numItems = 100;
            for (var i = 0; i < numItems; i++)
            {
                var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>(Path.GetTempPath());
                try
                {
                    service.MethodThatTerminates();
                }
                catch (Exception e)
                {
                    //gulp
                    Console.WriteLine(e.Message);
                }
                RemoteInstanceContainer.RemoveInstance(service);
                service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>(Path.GetTempPath());
                service.GetNameOfType(typeof (DateTime));
                RemoteInstanceContainer.RemoveInstance(service);
            }

            var threadsAfter = Process.GetCurrentProcess().Threads.Count;
            Console.WriteLine("threads:" + threadsAfter);
            Assert.Less(threadsAfter, threadsBefore + 20);
            Assert.AreEqual(0, exceptions);
        }

        [Test]
        [Ignore("Manual only: quite intense")]
        public void SequentialSafetyShotgunTest()
        {
            int exceptions = 0;
            AppDomain.CurrentDomain.UnhandledException += (s, e) => exceptions++;

            var threadsBefore = Process.GetCurrentProcess().Threads.Count;
            Console.WriteLine("threads:" + threadsBefore);

            const int numItems = 100;
            for (var i = 0; i < numItems; i++)
            {
                var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>(Path.GetTempPath());
                service.GetNameOfType(typeof(DateTime));
                RemoteInstanceContainer.RemoveInstance(service);
            }

            var threadsAfter = Process.GetCurrentProcess().Threads.Count;
            Console.WriteLine("threads:" + threadsAfter);
            Assert.Less(threadsAfter, threadsBefore + 20);
            Assert.AreEqual(0, exceptions);
        }

        [Test]
        [Ignore("Manual only: quite intense")]
        public void ThreadSafetyShotgunTest()
        {
            int exceptions = 0;
            AppDomain.CurrentDomain.UnhandledException += (s, e) => exceptions++;

            var doneEvent = new ManualResetEvent(false);
            const int numItems = 50;
            int done = numItems;
            for (var i = 0; i < numItems; i++)
            {
                ThreadPool.QueueUserWorkItem(t =>
                    {
                        try
                        {
                            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>(Path.GetTempPath());
                            for (int j = 0; j < numItems; j++)
                            {
                                service.GetNameOfType(typeof (DateTime));
                                service.DoNothingWithBigArray(new double[6000]);
                                service.GetNameOfType(typeof (DateTime));
                            }
                            RemoteInstanceContainer.RemoveInstance(service);
                        }
                        finally
                        {
                            if (Interlocked.Decrement(ref done) == 0)
                            {
                                doneEvent.Set();
                            }
                        }
                    });
            }
            doneEvent.WaitOne(); 
            
            var service2 = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>(Path.GetTempPath());
            RemoteInstanceContainer.RemoveInstance(service2);

            Assert.AreEqual(0, exceptions);
        }

        [Test]
        public void SimpleRemoteClassWithWorkingDir()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>(Path.GetTempPath());

            var square = service.GetSquare(3);
            Assert.AreEqual(9.0, square);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void CheckStructPropertySet()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            //service.Type = typeof (RemoteInstanceContainer);
            //var oldSpan = service.TimeSpan;
            service.TimeSpan = new TimeSpan(15, 0, 0, 0);
            service.DateTime = new DateTime(2000, 1, 1);

            Assert.AreEqual(15.0, service.TimeSpan.TotalDays);
            Assert.AreEqual(2000.0, service.DateTime.Year);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void ByRefMethod()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var values = new double[5];
            var count = 0;
            service.FixValues(ref values, 22, ref count);
            Assert.AreEqual(42.0, values[0]);
            Assert.AreEqual(22.0, values[1]);
            Assert.AreEqual(5, count);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        [Ignore("Manual interaction only")]
        public void KillingTestKillsRemoteInstance()
        {
            RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            Thread.Sleep(20000);
        }

        [Test]
        public void CheckDoubleArray()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var sumOfFirstTwoElements = service.DoNothingWithBigArray(new[] { 1.0, 2.0 });
            Assert.AreEqual(3.0, sumOfFirstTwoElements);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void WorkWithConvertedTypes()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            try
            {
                var name = service.GetNameOfType(typeof (string));
                Assert.AreEqual(name, "String");

                var intptr = service.GetIntPtr();
                Assert.AreEqual(42, intptr.ToInt32());
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(service);
            }
        }
        
        [Test]
        public void CheckDoubleArrayAsReturnArgument()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var reversedArray = service.Reverse(new[] { 1.0, 2.0 });
            Assert.AreEqual(new[] { 2.0, 1.0 }, reversedArray);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void CheckManyArrayTypes()
        {
            var service = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var arraySums = service.DoComplexArrayStuff(new[] { 1.0, 2.0, 3.0, 4.5 },
                                                        new float[] { 45.0f, 498203.3f, 198320.0f },
                                                        new int[] { 34, 5, 3, 3, 2, 2 });
            Assert.AreEqual(new[] { 10, 696568, 49 }, arraySums);

            RemoteInstanceContainer.RemoveInstance(service);
        }

        [Test]
        public void CheckWarmup()
        {
            // do a warmup
            var stopwatch = new Stopwatch();

            for (int i = 0; i < 5; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                var coldInstance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
                coldInstance.GetSquare(1);
                RemoteInstanceContainer.RemoveInstance(coldInstance);
                stopwatch.Stop();
                var time = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Startup time: {0:0.00}ms", time);
            }
        }

        [Test]
        [Ignore]
        public void DoManyCallsAndCheckOverhead()
        {
            var repeats = 500;
            var waitTime = 30.0;

            // local
            var localInstance = new SquareService();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < repeats; i++)
                localInstance.MethodThatWaits30millis();

            stopwatch.Stop();
            
            // remote
            var remote = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var stopwatchRemote = new Stopwatch();
            stopwatchRemote.Start();

            for (int i = 0; i < repeats; i++)
                remote.MethodThatWaits30millis();

            stopwatchRemote.Stop();
            RemoteInstanceContainer.RemoveInstance(remote);

            // summary
            Console.WriteLine("---");
            
            var totalOverheadLocal = stopwatch.ElapsedMilliseconds - (repeats*waitTime);
            Console.WriteLine("Local:: Total time: {0}ms, overhead per call: {1}ms", stopwatch.ElapsedMilliseconds,
                  totalOverheadLocal / repeats);
            
            var totalOverheadRemote = stopwatchRemote.ElapsedMilliseconds - (repeats*waitTime);
            Console.WriteLine("Remote:: Total time: {0}ms, overhead per call: {1}ms", stopwatchRemote.ElapsedMilliseconds,
                              totalOverheadRemote / repeats);
        }

        [Test]
        [Ignore]
        public void DoManyCallsWithConsolePrintsAndCheckOverhead()
        {
            var repeats = 500;
            var waitTime = 20.0;

            // local
            var localInstance = new SquareService();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            for (int i = 0; i < repeats; i++)
                localInstance.MethodThatWaits20millisAndWritesMuchToConsole();

            stopwatch.Stop();

            // remote
            var remote = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            var stopwatchRemote = new Stopwatch();
            stopwatchRemote.Start();

            for (int i = 0; i < repeats; i++)
                remote.MethodThatWaits20millisAndWritesMuchToConsole();

            stopwatchRemote.Stop();
            RemoteInstanceContainer.RemoveInstance(remote);

            // summary
            Console.WriteLine("---");

            var totalOverheadLocal = stopwatch.ElapsedMilliseconds - (repeats * waitTime);
            Console.WriteLine("Local:: Total time: {0}ms, overhead per call: {1}ms", stopwatch.ElapsedMilliseconds,
                  totalOverheadLocal / repeats);

            var totalOverheadRemote = stopwatchRemote.ElapsedMilliseconds - (repeats * waitTime);
            Console.WriteLine("Remote:: Total time: {0}ms, overhead per call: {1}ms", stopwatchRemote.ElapsedMilliseconds,
                              totalOverheadRemote / repeats);
        }

        [Test]
        [Ignore]
        public void CheckScalability()
        {
            var repeats = 500;
            var amounts = new[] { 1, 4, 16, 128, 512, 1024, 2048, 4096, 8196, 128000 };

            var localInstance = new SquareService();
            var reUsedInstance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            Console.WriteLine("Repeats: {0}", repeats);

            // check scalabity
            foreach (var amount in amounts)
            {
                var values = new double[amount];

                var stopwatch = new Stopwatch();
                stopwatch.Reset();
                stopwatch.Start();

                for (int i = 0; i < repeats; i++)
                    localInstance.DoNothingWithBigArray(values);

                stopwatch.Stop();
                var localTime = stopwatch.ElapsedMilliseconds;

                stopwatch.Reset();
                stopwatch.Start();
                for (int i = 0; i < repeats; i++)
                    reUsedInstance.DoNothingWithBigArray(values);
                stopwatch.Stop();
                var remoteTime = stopwatch.ElapsedMilliseconds;

                Console.WriteLine("For {0} values, local: {1}ms, remote: {2}ms",
                                  amount,
                                  localTime,
                                  remoteTime);
            }

            RemoteInstanceContainer.RemoveInstance(reUsedInstance);
        }

        [Test]
        public void StartMultipleInstancesConcurrent()
        {
            var instanceOne = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            var instanceTwo = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();

            instanceOne.DoNothingWithBigArray(new double[5]);

            instanceTwo.DoNothingWithBigArray(new double[6]);

            RemoteInstanceContainer.RemoveInstance(instanceOne);

            instanceTwo.DoNothingWithBigArray(new double[6]);

            RemoteInstanceContainer.RemoveInstance(instanceTwo);
        }

        [Test]
        public void CallAfterRemovingInstance()
        {
            var instance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            RemoteInstanceContainer.RemoveInstance(instance);

            try
            {
                instance.DoNothingWithBigArray(new double[4]);
            }
            catch (Exception e)
            {
                string expected = "Remote process not available / terminated during call DoNothingWithBigArray";
                Console.WriteLine(e.Message);
                Assert.IsTrue(e.Message.StartsWith(expected));
                return;
            }
            Assert.Fail("Expected exception!");
        }

        [Test]
        public void ExceptionOnTheOtherEndIsIsMarshalledIntoSameTypeAndRethrownLocally()
        {
            var instance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            try
            {
                instance.MethodThatThrowsException();
                Assert.Fail("Should not get here");
            }
            catch (InvalidTimeZoneException ex)
            {
                var exToString = ex.ToString();
                Assert.IsTrue(exToString.Contains("MethodThatThrowsException"));
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(instance);
            }
        }

        [Test]
        public void CrashOnOtherEndIsReceived()
        {
            var instance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            try
            {
                instance.MethodThatTerminates();
                Assert.Fail("Should not get here");
            }
            catch (Exception)
            {
                
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(instance);
            }
        }

        [Test]
        public void VoidMethodThatTakesLongTimeIsNoProblemAndIsBlocking()
        {
            var instance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                instance.MethodThatWaits2secs();
                stopWatch.Stop();
                Assert.Greater(stopWatch.ElapsedMilliseconds, 2000);
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(instance);
            }
        }

        [Test]
        [ExpectedException(typeof(InvalidTimeZoneException), ExpectedMessage = "Yes")]
        public void ExceptionOnTheOtherEndInVoidMessageIsMarshalledIntoSameType()
        {
            var instance = RemoteInstanceContainer.CreateInstance<ISquareService, SquareService>();
            try
            {
                instance.VoidMethodThatThrowsException();

                //we don't want void messages to block (right?), but exception will arrive delayed..so we cannot 
                //expect to recieve the exception when we instantly close the remote instance. Therefore, we must 
                //wait a while here.
                Thread.Sleep(500);
            }
            finally
            {
                RemoteInstanceContainer.RemoveInstance(instance);
            }
        }

        public interface ISquareService
        {
            double GetSquare(double x);
            
            double DoNothingWithBigArray(double[] values);

            double[] Reverse(double[] values);

            int[] DoComplexArrayStuff(double[] values, float[] values2, int[] values3);
            
            int MethodThatThrowsException();
            
            void VoidMethodThatThrowsException();

            int MethodThatTerminates();

            void MethodThatWaits2secs();

            void MethodThatWaits30millis();
            
            void FixValues(ref double[] values, int something, ref int count);

            string GetNameOfType(Type type);
            IntPtr GetIntPtr();

            Type Type { get; set; }

            TimeSpan TimeSpan { get; set; }

            DateTime DateTime { get; set; }

            void MethodThatWaits20millisAndWritesMuchToConsole();
        }

        public class SquareService : ISquareService
        {
            private TimeSpan timeSpan;

            public double GetSquare(double x)
            {
                return x * x;
            }

            public double DoNothingWithBigArray(double[] values)
            {
                if (values.Length >= 2)
                {
                    return values[0] + values[1];
                }
                return -1;
            }

            public double[] Reverse(double[] values)
            {
                return values.Reverse().ToArray();
            }

            public int[] DoComplexArrayStuff(double[] values, float[] values2, int[] values3)
            {
                return new[] { (int)values.Sum(), (int)values2.Sum(), values3.Sum() };
            }

            public int MethodThatThrowsException()
            {
                throw new InvalidTimeZoneException("Yes");
            }

            public void VoidMethodThatThrowsException()
            {
                throw new InvalidTimeZoneException("Yes");
            }

            public int MethodThatTerminates()
            {
                Environment.Exit(-1);
                return -1;
            }

            public void MethodThatWaits2secs()
            {
                Thread.Sleep(2000);
            }

            private readonly Stopwatch stopwatch = new Stopwatch();

            public void MethodThatWaits30millis()
            {
                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < 30)
                {
                }
            }

            public void FixValues(ref double[] values, int something, ref int count)
            {
                values[0] = 42.0;
                values[1] = something;
                count = values.Length;
            }

            public string GetNameOfType(Type type)
            {
                return type.Name;
            }

            public IntPtr GetIntPtr()
            {
                return new IntPtr(42);
            }

            public Type Type { get; set; }

            public TimeSpan TimeSpan
            {
                get { return timeSpan; }
                set { timeSpan = value; }
            }

            public DateTime DateTime { get; set; }

            public void MethodThatWaits20millisAndWritesMuchToConsole()
            {
                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < 20)
                {
                }

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine("A lot of data for the console");
                }
            }
        }
    }
}