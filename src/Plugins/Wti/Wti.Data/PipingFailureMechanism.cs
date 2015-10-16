using System.Collections.Generic;
using System.Linq;
using DelftTools.Shell.Core;

namespace Wti.Data
{
    /// <summary>
    /// Model for performing piping calculations.
    /// </summary>
    public class PipingFailureMechanism : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingFailureMechanism"/> class.
        /// </summary>
        public PipingFailureMechanism()
        {
            SurfaceLines = Enumerable.Empty<PipingSurfaceLine>();
            PipingData = new PipingData();
        }

        /// <summary>
        /// Gets the available surface lines within the scope of the piping failure mechanism.
        /// </summary>
        public IEnumerable<PipingSurfaceLine> SurfaceLines { get; private set; }

        /// <summary>
        /// Gets the input data which contains input and output of a piping calculation
        /// </summary>
        public PipingData PipingData { get; set; }

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
    }
}