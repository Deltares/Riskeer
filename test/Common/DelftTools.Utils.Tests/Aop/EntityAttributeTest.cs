using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using DelftTools.TestUtils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Aop
{
    [TestFixture]
    public class EntityAttributeTest
    {
        [Test]
        public void TestEvent()
        {
            var person = new Person();

            var calledChanging = 0;
            var calledChanged = 0;

            ((INotifyPropertyChanging) person).PropertyChanging += (s, e) => calledChanging++;
            ((INotifyPropertyChanged) person).PropertyChanged += (s, e) => calledChanged++;

            person.Prop1 = true;

            Assert.AreEqual(1, calledChanging);
            Assert.AreEqual(1, calledChanged);
        }

        [Test]
        public void TestAggregationList()
        {
            var person = new Person
            {
                Child = new Person
                {
                    Prop3 = 15
                },
                AllPersons = new EventedList<Person>()
            };

            var propChanged = 0;
            var collChanged = 0;

            ((INotifyPropertyChanged) person).PropertyChanged += (s, e) => propChanged++;
            ((INotifyCollectionChange) person).CollectionChanged += (s, e) => collChanged++;

            person.AllPersons.Add(person.Child);

            Assert.AreEqual(1, collChanged, "coll");

            person.Child.Prop3 = 16;

            Assert.AreEqual(1, propChanged, "prop");
        }

        [Test]
        public void TestNoNotify()
        {
            var person = new Person();

            var calledChanging = 0;
            var calledChanged = 0;

            ((INotifyPropertyChanging) person).PropertyChanging += (s, e) => calledChanging++;
            ((INotifyPropertyChanged) person).PropertyChanged += (s, e) => calledChanged++;

            person.Prop2 = true;

            Assert.AreEqual(0, calledChanging);
            Assert.AreEqual(0, calledChanged);
        }

        [Test]
        public void TestChildEvent()
        {
            var person = new Person
            {
                Child = new Person()
            };

            var calledChanging = 0;
            var calledChanged = 0;

            ((INotifyPropertyChanging) person).PropertyChanging += (s, e) =>
            {
                Assert.AreEqual(s, person.Child);
                calledChanging++;
            };

            ((INotifyPropertyChanged) person).PropertyChanged += (s, e) =>
            {
                Assert.AreEqual(s, person.Child);
                calledChanged++;
            };

            person.Child.Prop1 = true;

            Assert.AreEqual(1, calledChanging);
            Assert.AreEqual(1, calledChanged);
        }

        [Test]
        public void TestAggregationEvent()
        {
            var person = new Person();

            var calledChanging = 0;
            var calledChanged = 0;

            ((INotifyPropertyChanging) person).PropertyChanging += (s, e) =>
            {
                Assert.AreEqual(s, person);
                calledChanging++;
            };

            ((INotifyPropertyChanged) person).PropertyChanged += (s, e) =>
            {
                Assert.AreEqual(s, person);
                calledChanged++;
            };

            person.Parent = new Person();

            Assert.AreEqual(1, calledChanging);
            Assert.AreEqual(1, calledChanged);

            person.Parent.Prop1 = true;

            Assert.AreEqual(1, calledChanging);
            Assert.AreEqual(1, calledChanged);
        }

        [Test]
        public void AspectInitializationShouldWorkInCaseOfVirtualCallsInConstructor()
        {
            var superSuper = new SuperSuper();

            var elements = superSuper.AllElements;
            superSuper.AllElements = null;

            var called = 0;
            ((INotifyCollectionChanged) superSuper).CollectionChanged += (s, e) => called++;

            elements.Add(new Person());

            Assert.AreEqual(0, called);
        }

        [Test]
        public void ConstructingManyObjectsShouldBeFast()
        {
            //450-500 on my pc (TS)
            TestHelper.AssertIsFasterThan(1400, () =>
            {
                for (int i = 0; i < 10000; i++)
                {
                    new SuperSuper();
                }
            });
        }

        [Test]
        public void MemoryUsageOfEntityAttributeShouldNotGrow()
        {
            var numToCreate = 100000;
            var list = new List<EmptyObject>(numToCreate);

            var sizeBefore = GC.GetTotalMemory(true);

            for (int i = 0; i < numToCreate; i++)
            {
                list.Add(new EmptyObject());
            }

            var sizeAfter = GC.GetTotalMemory(true);

            var bytesForEntityAttribute = (sizeAfter - sizeBefore)/numToCreate;

            Assert.Less(bytesForEntityAttribute, 100, "entity attribute memory footprint is growing!"); // was 96 when writing test
        }

        [Test]
        public void ConstructingObjectsInSeveralThreadsShouldBeSafe()
        {
            Task.WaitAll(Task.Factory.StartNew(CreateManyObjects<Super>), Task.Factory.StartNew(CreateManyObjects<Person>), Task.Factory.StartNew(CreateManyObjects<Super>));
        }

        [Test]
        [Ignore("Known issue")]
        public void WIPUnsubscriptionShouldWorkInCaseOfVirtualCallsNotCallingBase()
        {
            var super = new Super();

            var elements = super.OtherElements;
            super.OtherElements = null;

            var called = 0;
            ((INotifyCollectionChanged) super).CollectionChanged += (s, e) => called++;

            elements.Add(new Person());

            Assert.AreEqual(0, called);
        }

        private void CreateManyObjects<T>() where T : new()
        {
            for (int i = 0; i < 1000000; i++)
            {
                new T();
            }
        }

        [Entity]
        public class EmptyObject {}

        [Entity]
        public class Base
        {
            public Base()
            {
                AllElements = new EventedList<Person>();
                OtherElements = new EventedList<Person>();
            }

            public IEventedList<Person> AllElements { get; set; }

            public virtual IEventedList<Person> OtherElements { get; set; }
        }

        [Entity]
        public class Super : Base
        {
            /// <summary>
            /// override, but not calling base!
            /// </summary>
            public override IEventedList<Person> OtherElements { get; set; }
        }

        [Entity]
        public class SuperSuper : Super
        {
        }

        [Entity]
        public class Person
        {
            public Person Child { get; set; }

            [Aggregation]
            public Person Parent { get; set; }

            public bool Prop1 { get; set; }

            [NoNotifyPropertyChange]
            public bool Prop2 { get; set; }

            public int Prop3 { get; set; }

            [Aggregation]
            public IEventedList<Person> AllPersons { get; set; }
        }
    }
}