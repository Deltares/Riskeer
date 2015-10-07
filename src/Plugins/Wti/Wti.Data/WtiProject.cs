using System.Collections.Generic;

using DelftTools.Shell.Core;
using DelftTools.Utils;

namespace Wti.Data
{
    public class WtiProject : INameable, IObservable
    {
        private IList<IObserver> observers = new List<IObserver>();

        public WtiProject()
        {
            Name = "WTI project";
        }

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