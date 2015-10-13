using System.Collections.Generic;
using DelftTools.Shell.Core;
using DelftTools.Utils;

namespace Wti.Data
{
    public class WtiProject : INameable, IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();
        private PipingFailureMechanism pipingFailureMechanism;

        public WtiProject()
        {
            Name = "WTI project";
        }

        public string Name { get; set; }

        public PipingFailureMechanism PipingFailureMechanism
        {
            get
            {
                return pipingFailureMechanism;
            }
            set
            {
                pipingFailureMechanism = value;
            }
        }

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