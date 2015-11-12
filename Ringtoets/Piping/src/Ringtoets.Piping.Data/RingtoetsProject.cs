using System.Collections.Generic;
using Core.Common.Base;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Container for all the data that has been imported and calculated by the user when performing an assessment.
    /// </summary>
    public class RingtoetsProject : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsProject"/> with a default name set.
        /// </summary>
        public RingtoetsProject()
        {
            Name = "Ringtoets project";
        }

        /// <summary>
        /// Gets or sets the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; private set; }

        /// <summary>
        /// The name of the <see cref="RingtoetsProject"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Removes the <see cref="PipingFailureMechanism"/> assigned to the <see cref="RingtoetsProject"/>.
        /// </summary>
        public void ClearPipingFailureMechanism()
        {
            PipingFailureMechanism = null;
        }

        /// <summary>
        /// Creates a new <see cref="PipingFailureMechanism"/> and assign it to the <see cref="RingtoetsProject"/>.
        /// </summary>
        public void InitializePipingFailureMechanism()
        {
            PipingFailureMechanism = new PipingFailureMechanism();
        }

        /// <summary>
        /// Determines whether a new <see cref="PipingFailureMechanism"/> can be added to the <see cref="RingtoetsProject"/>.
        /// </summary>
        /// <returns>True if a new <see cref="PipingFailureMechanism"/> can be assigned. False otherwise.</returns>
        public bool CanAddPipingFailureMechanism()
        {
            return PipingFailureMechanism == null;
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