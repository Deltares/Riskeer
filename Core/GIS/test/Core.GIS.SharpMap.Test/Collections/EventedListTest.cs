using System.ComponentModel;
using System.Linq;

using Core.Common.TestUtil;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.GIS.SharpMap.Api.Collections;

using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Collections
{
    [TestFixture]
    public class EventedListTest
    {
        [SetUp]
        public void SetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [Test]
        public void CollectionChangedWhenValueIsAdded()
        {
            var eventedList = new EventedList<object>();
            var callCount = 0;
            var item = new object();
            eventedList.CollectionChanged += delegate(object sender, NotifyCollectionChangeEventArgs e)
            {
                Assert.AreEqual(eventedList, sender);
                Assert.AreEqual(item, e.Item);
                callCount++;
            };
            eventedList.Add(item);
            Assert.AreEqual(1, callCount);
        }

        [Test]
        public void CollectionChangedWhenValueIsRemoved()
        {
            var eventedList = new EventedList<int>();

            var called = false;
            eventedList.Add(1);
            eventedList.CollectionChanged += delegate { called = true; };
            eventedList.Remove(1);
            Assert.IsTrue(called);
        }

        [Test]
        public void ItemReplaced()
        {
            var eventedList = new EventedList<int>();

            var called = false;
            eventedList.Add(1);
            eventedList.CollectionChanged += delegate { called = true; };
            eventedList[0] = 2;
            Assert.IsTrue(called);
        }

        [Test]
        public void UnsubscribeFromOldItemOnReplace()
        {
            var eventedList = new EventedList<MockClassWithTwoProperties>();

            var aPropertyChangeCount = 0;
            var listPropertyChangeCount = 0;

            var a = new MockClassWithTwoProperties
            {
                StringProperty = "a"
            };

            eventedList.Add(a);

            eventedList.PropertyChanged += delegate { listPropertyChangeCount++; };
            a.PropertyChanged += delegate { aPropertyChangeCount++; };

            // replace item
            eventedList[0] = new MockClassWithTwoProperties
            {
                StringProperty = "second a"
            };

            a.StringProperty = "a2";

            Assert.AreEqual(0, listPropertyChangeCount);
            Assert.AreEqual(1, aPropertyChangeCount);
        }

        [Test]
        public void UnsubscribeFromRemovedItems()
        {
            var eventedList = new EventedList<MockClassWithTwoProperties>();

            var aPropertyChangeCount = 0;
            var listPropertyChangeCount = 0;

            var a = new MockClassWithTwoProperties
            {
                StringProperty = "a"
            };

            eventedList.Add(a);

            eventedList.PropertyChanged += delegate { listPropertyChangeCount++; };
            a.PropertyChanged += delegate { aPropertyChangeCount++; };

            // replace item
            eventedList.Remove(a);

            a.StringProperty = "a2";

            Assert.AreEqual(0, listPropertyChangeCount);
            Assert.AreEqual(1, aPropertyChangeCount);
        }

        [Test]
        public void AddRangeTest()
        {
            var eventedList = new EventedList<int>();

            //keep record of number of collectionchanges.
            var i = 0;
            eventedList.CollectionChanged += delegate { i++; };

            //add three integers to the list.
            eventedList.AddRange(new[]
            {
                1,
                2,
                3
            });

            //three collectionchanged events will be generated.
            Assert.AreEqual(3, i);

            //check if items where added to the list.
            Assert.IsTrue(eventedList.SequenceEqual(new[]
            {
                1,
                2,
                3
            }));
        }

        [Test]
        public void ContainsShouldWorkForObjectsOfADifferentType()
        {
            var doubleList = new EventedList<double>
            {
                1.0, 2.0
            };
            Assert.IsFalse(doubleList.Contains("This must return false"));
        }

        [Test]
        public void ListShouldSubscribeToPropertyChangesInChildObjectAfterAddRange()
        {
            var eventedList = new EventedList<MockClassWithTwoProperties>();

            //add three integers to the list.
            var properties = new MockClassWithTwoProperties();
            eventedList.AddRange(new[]
            {
                properties
            });

            object theSender = null;
            PropertyChangedEventArgs theEventArgs = null;
            eventedList.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                theSender = sender;
                theEventArgs = e;
            };
            properties.StringProperty = "iets";
            Assert.AreEqual(properties, theSender);
            Assert.AreEqual("StringProperty", theEventArgs.PropertyName);
        }

        [Test]
        public void ListShouldSubscribeToPropertyChangesInChildObjectAfterAdd()
        {
            var eventedList = new EventedList<MockClassWithTwoProperties>();

            //add three integers to the list.
            var properties = new MockClassWithTwoProperties();
            eventedList.Add(properties);

            object theSender = null;
            PropertyChangedEventArgs theEventArgs = null;
            eventedList.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                theSender = sender;
                theEventArgs = e;
            };
            properties.StringProperty = "iets";
            Assert.AreEqual(properties, theSender);
            Assert.AreEqual("StringProperty", theEventArgs.PropertyName);
        }

        [Test]
        public void EventSubscriptionShouldBeFast()
        {
            var eventedList = new EventedList<MockWithPropertyAndCollectionChange>();

            // warm-up
            for (var i = 0; i < 50; i++)
            {
                eventedList.Add(new MockWithPropertyAndCollectionChange());
            }

            TestHelper.AssertIsFasterThan(250, () =>
            {
                for (var i = 0; i < 20000; i++)
                {
                    eventedList.Add(new MockWithPropertyAndCollectionChange());
                }
            });
        }

        private class MockWithPropertyAndCollectionChange : INotifyPropertyChange, INotifyCollectionChanged
        {
            // Required by interface, but not used (yet)
#pragma warning disable 67
            public event PropertyChangingEventHandler PropertyChanging;

            public event PropertyChangedEventHandler PropertyChanged;

            public event NotifyCollectionChangedEventHandler CollectionChanged;

            public event NotifyCollectionChangingEventHandler CollectionChanging;
#pragma warning restore 67
        }

        private class MockClassWithTwoProperties : INotifyPropertyChange
        {
            private string stringField;
            public event PropertyChangedEventHandler PropertyChanged;
            public event PropertyChangingEventHandler PropertyChanging;

            public string StringProperty
            {
                get
                {
                    return stringField;
                }
                set
                {
                    if (PropertyChanging != null)
                    {
                        PropertyChanging(this, new PropertyChangingEventArgs("StringProperty"));
                    }
                    stringField = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("StringProperty"));
                    }
                }
            }
        }
    }
}