using System.Collections.Generic;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Base.Tests.TestObjects
{
    [Entity]
    public class Child : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        public Child()
        {
            Children = new EventedList<Child>();
        }

        public string Name { get; set; }
        public IList<Child> Children { get; set; }

        #region IObservable

        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.UpdateObserver();
            }
        }

        #endregion
    }
}