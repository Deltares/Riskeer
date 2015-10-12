using System;
using System.Collections.Generic;
using System.Linq;
using DelftTools.TestUtils.TestReferenceHelper;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class TestReferenceHelperTest
    {
        [Test]
        public void GetObjects()
        {
            var testObject = new TestClass();
            var parent = new TestClass();

            testObject.Parent = parent;

            var referenceNodes = TestReferenceHelper.GetReferenceNodesInTree(testObject).ToList();
            Assert.AreEqual(testObject, referenceNodes[0].Object);
            Assert.AreEqual(parent, referenceNodes[1].Object);
        }

        [Test]
        public void GetPostSharpEvents()
        {
            var testObject = new TestClass();

            var subscriptionsBefore = new List<string>();
            var numEventsBefore = TestReferenceHelper.FindEventSubscriptionsAdvanced(testObject, subscriptionsBefore, 2);

            Assert.AreEqual(0, numEventsBefore);

            //After
            ((INotifyPropertyChange) testObject).PropertyChanged += (s, e) => { };
            ((INotifyPropertyChange) testObject).PropertyChanging += (s, e) => { };

            var evnt = new NotifyCollectionChangedEventHandler((s, e) => { });
            ((INotifyCollectionChange) testObject).CollectionChanged += evnt;

            var subscriptionsAfter = new List<string>();
            var numEventsAfter = TestReferenceHelper.FindEventSubscriptionsAdvanced(testObject, subscriptionsAfter, 2);

            Console.WriteLine(String.Join("\n", subscriptionsAfter.ToArray()));
            Assert.AreEqual(3, numEventsAfter);

            //Final
            ((INotifyCollectionChange) testObject).CollectionChanged -= evnt;

            var subscriptionsFinal = new List<string>();
            var numEventsFinal = TestReferenceHelper.FindEventSubscriptionsAdvanced(testObject, subscriptionsFinal, 2);

            Console.WriteLine("===\n" + String.Join("\n", subscriptionsFinal.ToArray()));
            Assert.AreEqual(2, numEventsFinal);
        }

        [Test]
        public void GetPostSharpEventsBaseClass()
        {
            var testObject = new SuperTestClass();

            var subscriptionsBefore = new List<string>();
            var numEventsBefore = TestReferenceHelper.FindEventSubscriptionsAdvanced(testObject, subscriptionsBefore, 2);

            Assert.AreEqual(0, numEventsBefore);

            //After
            ((INotifyPropertyChange) testObject).PropertyChanged += (s, e) => { };
            ((INotifyPropertyChange) testObject).PropertyChanging += (s, e) => { };

            var evnt = new NotifyCollectionChangedEventHandler((s, e) => { });
            ((INotifyCollectionChange) testObject).CollectionChanged += evnt;

            var subscriptionsAfter = new List<string>();
            var numEventsAfter = TestReferenceHelper.FindEventSubscriptionsAdvanced(testObject, subscriptionsAfter, 2);

            Console.WriteLine(String.Join("\n", subscriptionsAfter.ToArray()));
            Assert.AreEqual(3, numEventsAfter);

            //Final
            ((INotifyCollectionChange) testObject).CollectionChanged -= evnt;

            var subscriptionsFinal = new List<string>();
            var numEventsFinal = TestReferenceHelper.FindEventSubscriptionsAdvanced(testObject, subscriptionsFinal, 2);

            Console.WriteLine("===\n" + String.Join("\n", subscriptionsFinal.ToArray()));
            Assert.AreEqual(2, numEventsFinal);
        }

        [Entity]
        private class TestClass
        {
            public TestClass Parent { get; set; }
        }

        [Entity]
        private class SuperTestClass : TestClass
        {

        }
    }
}