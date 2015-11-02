using System.Collections.Generic;
using Core.Common.BaseDelftTools;

namespace Core.Common.DelftTools.Tests.TestObjects
{
    public class Parent : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();
        public readonly IList<Child> Children = new List<Child>();
        public string Name { get; set; }

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