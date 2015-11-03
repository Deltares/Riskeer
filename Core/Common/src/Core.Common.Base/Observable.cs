using System.Collections.Generic;

namespace Core.Common.Base
{
    public class Observable : IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

        /// <summary>
        /// Attach an <see cref="IObserver"/> to this <see cref="IObservable"/>. Changes in the <see cref="IObservable"/> will notify the
        /// <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to notify on changes.</param>
        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Detach an <see cref="IObserver"/> from this <see cref="IObservable"/>. Changes in the <see cref="IObservable"/> will no longer
        /// notify the <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to no longer notify on changes.</param>
        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        /// <summary>
        /// Notifies all the observers that have been currently attached to this <see cref="IObservable"/>.
        /// </summary>
        public void NotifyObservers()
        {
            foreach (var observer in observers)
            {
                observer.UpdateObserver();
            }
        }
    }
}