using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Core.Common.TestUtils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Aop.Markers;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using NUnit.Framework;

namespace Core.Common.Utils.Tests.Aop
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

        private void CreateManyObjects<T>() where T : new()
        {
            for (int i = 0; i < 10000; i++)
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

            public Person Parent { get; set; }

            public bool Prop1 { get; set; }

            [NoNotifyPropertyChange]
            public bool Prop2 { get; set; }

            public int Prop3 { get; set; }

            public IEventedList<Person> AllPersons { get; set; }
        }
    }
}