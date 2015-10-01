using System.Collections.Generic;
using DelftTools.Shell.Core;

namespace Wti.Data
{
    public class PipingData : IObservable
    {
        private IList<IObserver> observers = new List<IObserver>();

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

        public double AssessmentLevel { get; set; } 
    }
}