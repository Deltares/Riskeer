namespace Core.Common.BaseDelftTools
{
    /// <summary>
    /// This interface describes the methods that need to be implemented on a class of objects that are supposed to be observable.
    /// Being observable let classes notify observers that they have performed an action (for example changed data).
    /// </summary>
    public interface IObservable
    {
        /// <summary>
        /// Attach an <see cref="IObserver"/> to this <see cref="IObservable"/>. Changes in the <see cref="IObservable"/> will notify the
        /// <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to notify on changes.</param>
        void Attach(IObserver observer);

        /// <summary>
        /// Detach an <see cref="IObserver"/> from this <see cref="IObservable"/>. Changes in the <see cref="IObservable"/> will no longer
        /// notify the <paramref name="observer"/>.
        /// </summary>
        /// <param name="observer">The <see cref="IObserver"/> to no longer notify on changes.</param>
        void Detach(IObserver observer);

        /// <summary>
        /// Notifies all the observers that have been currently attached to this <see cref="IObservable"/>.
        /// </summary>
        void NotifyObservers();
    }
}