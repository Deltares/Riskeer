using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Utils.Tests.Aop.TestClasses
{
    [Entity(FireOnPropertyChange = false)]
    public class CollectionChangedAspectTestClass
    {
        public CollectionChangedAspectTestClass()
        {
            ListContainers = new EventedList<CollectionChangedAspectTestClass>(); 
            Integers = new EventedList<int>();
            NoBubblingIntegers = new EventedList<int>();
            Lists = new EventedList<IEventedList<int>>();
            PrivateList = new EventedList<int>();
        }

        public CollectionChangedAspectTestClass Nested { get; set; }

        public string Name { get; set; }

        public IEventedList<int> Integers { get; set; }

        public IEventedList<CollectionChangedAspectTestClass> ListContainers { get; private set; }

        public IEventedList<IEventedList<int>> Lists { get; private set; }

        [NoNotifyPropertyChange]
        public IEventedList<int> NoBubblingIntegers { get; private set; }

        IEventedList<int> PrivateList { get; set; }

        public void AddToPrivateList(int value)
        {
            PrivateList.Add(value);
        }
    }
}