// original code taken from: http://stackoverflow.com/questions/1089309/weak-events-in-net

using System;
using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class WeakEventTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            testScenarios.Add(SetupTestGeneric);
            testScenarios.Add(SetupTestPropChange);
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
        }

        private readonly List<Action<bool>> testScenarios = new List<Action<bool>>();

        private IEventSource source;
        private WeakReference sourceRef;

        private IEventConsumer consumer;
        private WeakReference consumerRef;

        private IEventConsumer consumer2;
        private WeakReference consumerRef2;

        private void ConsumerSourceTestMethod()
        {
            Assert.IsFalse(consumer.eventSet);
            source.Fire();
            Assert.IsTrue(consumer.eventSet);
        }

        private void ConsumerLinkTestMethod()
        {
            consumer = null;
            GC.Collect();
            Assert.IsFalse(consumerRef.IsAlive);
            Assert.IsTrue(source.InvocationCount == 1);
            source.Fire();
            Assert.IsTrue(source.InvocationCount == 0);
        }

        private void ConsumerLinkTestDoubleMethod()
        {
            consumer = null;
            GC.Collect();
            Assert.IsFalse(consumerRef.IsAlive);
            Assert.IsTrue(source.InvocationCount == 2);
            source.Fire();
            Assert.IsTrue(source.InvocationCount == 1);
            consumer2 = null;
            GC.Collect();
            Assert.IsFalse(consumerRef2.IsAlive);
            Assert.IsTrue(source.InvocationCount == 1);
            source.Fire();
            Assert.IsTrue(source.InvocationCount == 0);
        }

        private void ConsumerLinkTestMultipleMethod()
        {
            consumer = null;
            consumer2 = null;
            GC.Collect();
            Assert.IsFalse(consumerRef.IsAlive);
            Assert.IsFalse(consumerRef2.IsAlive);
            Assert.IsTrue(source.InvocationCount == 2);
            source.Fire();
            Assert.IsTrue(source.InvocationCount == 0);
        }

        private void SourceLinkTestMethod()
        {
            source = null;
            GC.Collect();
            Assert.IsFalse(sourceRef.IsAlive);
        }

        private void SourceLinkTestMultipleMethod()
        {
            source = null;
            GC.Collect();
            Assert.IsFalse(sourceRef.IsAlive);
        }

        public void SetupTestGeneric(bool both)
        {
            source = new EventSourceGeneric();
            sourceRef = new WeakReference(source);

            consumer = new EventConsumerGeneric((EventSourceGeneric) source);
            consumerRef = new WeakReference(consumer);

            if (both)
            {
                consumer2 = new EventConsumerGeneric((EventSourceGeneric) source);
                consumerRef2 = new WeakReference(consumer2);
            }
        }

        public void SetupTestPropChange(bool both)
        {
            source = new EventSourcePropChange();
            sourceRef = new WeakReference(source);

            consumer = new EventConsumerPropChange((EventSourcePropChange) source);
            consumerRef = new WeakReference(consumer);

            if (both)
            {
                consumer2 = new EventConsumerPropChange((EventSourcePropChange) source);
                consumerRef2 = new WeakReference(consumer2);
            }
        }

        public interface IEventSource
        {
            int InvocationCount { get; }
            void Fire();
        }

        public class EventSourceGeneric : IEventSource
        {
            #region IEventSource Members

            public int InvocationCount
            {
                get { return (theEvent != null) ? theEvent.GetInvocationList().Length : 0; }
            }

            public void Fire()
            {
                if (theEvent != null) theEvent(this, EventArgs.Empty);
            }

            #endregion

            public event EventHandler<EventArgs> theEvent;
        }

        public class EventSourcePropChange : IEventSource
        {
            #region IEventSource Members

            public int InvocationCount
            {
                get { return (theEvent != null) ? theEvent.GetInvocationList().Length : 0; }
            }

            public void Fire()
            {
                if (theEvent != null) theEvent(this, new PropertyChangedEventArgs(""));
            }

            #endregion

            public event PropertyChangedEventHandler theEvent;
        }

        public interface IEventConsumer
        {
            bool eventSet { get; }
        }

        public class EventConsumerGeneric : IEventConsumer
        {
            public EventConsumerGeneric(EventSourceGeneric sourceGeneric)
            {
                sourceGeneric.theEvent +=
                    new EventHandler<EventArgs>(source_theEvent).MakeWeak((e) => sourceGeneric.theEvent -= e);
            }

            #region IEventConsumer Members

            public bool eventSet { get; private set; }

            #endregion

            public void source_theEvent(object sender, EventArgs e)
            {
                eventSet = true;
            }
        }

        public class EventConsumerPropChange : IEventConsumer
        {
            public EventConsumerPropChange(EventSourcePropChange sourcePropChange)
            {
                sourcePropChange.theEvent +=
                    new PropertyChangedEventHandler(source_theEvent).MakeWeak((e) => sourcePropChange.theEvent -= e);
            }

            #region IEventConsumer Members

            public bool eventSet { get; private set; }

            #endregion

            public void source_theEvent(object sender, PropertyChangedEventArgs e)
            {
                eventSet = true;
            }
        }

        [Test]
        public void ConsumerLinkTest()
        {
            foreach (var a in testScenarios)
            {
                a(false);
                ConsumerLinkTestMethod();
            }
        }

        [Test]
        public void ConsumerLinkTestDouble()
        {
            foreach (var a in testScenarios)
            {
                a(true);
                ConsumerLinkTestDoubleMethod();
            }
        }

        [Test]
        public void ConsumerLinkTestMultiple()
        {
            foreach (var a in testScenarios)
            {
                a(true);
                ConsumerLinkTestMultipleMethod();
            }
        }

        [Test]
        public void ConsumerSourceTest()
        {
            foreach (var a in testScenarios)
            {
                a(false);
                ConsumerSourceTestMethod();
            }
        }

        [Test]
        public void SourceLinkTest()
        {
            foreach (var a in testScenarios)
            {
                a(false);
                SourceLinkTestMethod();
            }
        }

        [Test]
        public void SourceLinkTestMultiple()
        {
            SetupTestGeneric(true);
            foreach (var a in testScenarios)
            {
                a(true);
                SourceLinkTestMultipleMethod();
            }
        }
    }
}