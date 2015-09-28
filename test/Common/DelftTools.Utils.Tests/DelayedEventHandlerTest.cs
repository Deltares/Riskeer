using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DelftTools.TestUtils;
using DelftTools.Utils.Threading;
using log4net;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class DelayedEventHandlerTest: Control
    {
        private event PropertyChangedEventHandler PropertyChanged;

        [SetUp]
        public void SetUp()
        {
            LogHelper.ConfigureLogging();
            var log = LogManager.GetLogger(typeof(DelayedEventHandlerTest)); // required in order to get messages from another thread
            log.DebugFormat("Initializing logging");
        }

        [Test]
        [Ignore("Example")]
        public void WithAndWithoutDelayedEventHandler()
        {
            var stopwatch = new Stopwatch();
            Action uiMethod = () => Thread.Sleep(200);

            // 1. ========================================= normal way to subscribe to events
            PropertyChangedEventHandler eventHandler1 = delegate { uiMethod(); };
            PropertyChanged += eventHandler1;

            stopwatch.Start();
            var result = 0;
            for (var i = 0; i < 10; i++)
            {
                // processing logic
                result += i;

                // fire event (usually happens automatically on object model change)
                PropertyChanged(this, null);
            }
            stopwatch.Stop();

            PropertyChanged -= eventHandler1;
            
            Console.WriteLine("Elapsed time (without delayed event handler): {0} ms", stopwatch.ElapsedMilliseconds);

            // 2. ========================================= subscribe with delayed event handler
            using (var eventHandler2 = new DelayedEventHandler<PropertyChangedEventArgs>(delegate { uiMethod(); }))
            {
                PropertyChanged += eventHandler2;

                stopwatch.Reset();
                stopwatch.Start();
                result = 0;
                for (var i = 0; i < 10; i++)
                {
                    // processing logic
                    result += i;

                    // fire event (usually happens automatically on object model change)
                    PropertyChanged(this, null);
                }
                stopwatch.Stop();

                PropertyChanged -= eventHandler2;

                Console.WriteLine("Elapsed time (with delayed event handler): {0} ms", stopwatch.ElapsedMilliseconds);
            }
        }
        
        [Test]
        public void ComplexScenarioWhereEventsAreFiredWhileActionIsPerformed()
        {
            var callCount = 0;

            using (var eventHandler = new DelayedEventHandler<PropertyChangedEventArgs>(delegate
                {
                    // e.g. UI handler
                    Thread.Sleep(10);
                    callCount++;
                }))
            {
                PropertyChanged += eventHandler;

                for (var i = 0; i < 10; i++)
                {
                    // processing logic
                    PropertyChanged(this, null);
                }

                while (eventHandler.IsRunning || eventHandler.HasEventsToProcess)
                {
                    Thread.Sleep(10);
                }

                PropertyChanged -= eventHandler;

                Assert.Greater(callCount, 0);
                Assert.LessOrEqual(callCount, 2);
            }
        }

        [Test]
        public void FullRefreshWithSingleEventRefreshLimit()
        {
            var times = new List<DateTime>();

            var fullRefreshTimes = new List<DateTime>();

            using (var eventHandler =
                    new DelayedEventHandler<PropertyChangedEventArgs>(delegate { times.Add(DateTime.Now); })
                        {
                            Delay = 1,
                            FireLastEventOnly = false,
                            FullRefreshEventHandler = delegate { fullRefreshTimes.Add(DateTime.Now); },
                            FullRefreshEventsCount = 2

                        })
            {

                PropertyChanged += eventHandler;

                for (var i = 0; i < 100; i++)
                {
                    Thread.Sleep(10);
                    PropertyChanged(this, null);
                }

                PropertyChanged -= eventHandler;

                while (eventHandler.IsRunning || eventHandler.HasEventsToProcess)
                {
                    Thread.Sleep(10);
                }

                Console.WriteLine("refresh times: " + times.Aggregate("", (current, t) => current + t + ", "));

                Console.WriteLine("full refresh times: " +
                                  fullRefreshTimes.Aggregate("", (current, t) => current + t + ", "));

            }
        }

        [Test]
        public void DelayedEventHandlerDoesNotFireWhenFireEventsIsFalse()
        {
            try
            {
                var callCount = 0;
                using (var eventHandler = new DelayedEventHandler<PropertyChangedEventArgs>((s, e) => callCount++))
                {
                    DelayedEventHandlerController.FireEvents = false;

                    PropertyChanged += eventHandler;
                    PropertyChanged(this, null);

                    //wait max 1 sec.
                    var maxWaitTime = new TimeSpan(0, 0, 0, 1);
                    var startTime = DateTime.Now;
                    while (((DateTime.Now - startTime) < maxWaitTime) &&
                           ((eventHandler.IsRunning || eventHandler.HasEventsToProcess)))
                    {
                        Thread.Sleep(10);
                    }

                    Assert.AreEqual(0, callCount);
                }
            }
            finally
            {
                DelayedEventHandlerController.FireEvents = true; // always reset
            }
        }

        [Test]
        public void NoEventsFiredAfterDispose()
        {
            var callCount = 0;
            using (var eventHandler = new DelayedEventHandler<PropertyChangedEventArgs>((s, e) => callCount++)
                {Delay = 50, SynchronizingObject = this})
            {
                PropertyChanged += eventHandler;
                PropertyChanged(this, null);

                Thread.Sleep(10);
            }

            Thread.Sleep(2000);

            Assert.AreEqual(0, callCount);
        }

        internal class FireEventClass
        {
            private DelayedEventHandlerTest delayedEventHandlerTest;

            public FireEventClass(DelayedEventHandlerTest delayedEventHandlerTest)
            {
                this.delayedEventHandlerTest = delayedEventHandlerTest;
            }

            public void FireTwentyTimesInTwoSeconds()
            {
                for(int i = 0; i < 20 ;i++)
                {
                    Thread.Sleep(100);
                    delayedEventHandlerTest.PropertyChanged(this,  new PropertyChangedEventArgs("FireEventClass "+ i));

                }
            }
        }
    }
}