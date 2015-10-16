using System.Collections.Generic;
using DelftTools.Shell.Core;
using DelftTools.Utils;

namespace Wti.Data
{
    /// <summary>
    /// Container for all the data that has been imported and calculated by the user when performing an assessment.
    /// </summary>
    public class WtiProject : INameable, IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Creates a new instance of <see cref="WtiProject"/> with a default name set.
        /// </summary>
        public WtiProject()
        {
            Name = "WTI project";
        }

        /// <summary>
        /// The name of the <see cref="WtiProject"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; set; }

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