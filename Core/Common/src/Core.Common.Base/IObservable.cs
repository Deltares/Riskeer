namespace Core.Common.Base
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that are supposed to be observable.
    /// Observables should notify their observers of the fact that their internal state has changed.
    /// </summary>
    /// <seealso cref="IObserver"/>
    public interface IObservable
    {
        /// <summary>
        /// This method attaches <paramref name="observer"/>. As a result, changes in the <see cref="IObservable"/> will now be notified to <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to notify on changes.</param>
        void Attach(IObserver observer);

        /// <summary>
        /// This method dettaches <paramref name="observer"/>. As a result, changes in the <see cref="IObservable"/> will no longer be notified to <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to no longer notify on changes.</param>
        void Detach(IObserver observer);

        /// <summary>
        /// This method notifies all observers that have been currently attached to the <see cref="IObservable"/>.
        /// </summary>
        void NotifyObservers();
    }
}