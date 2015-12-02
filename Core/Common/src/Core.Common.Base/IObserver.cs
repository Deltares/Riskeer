namespace Core.Common.Base
{
    /// <summary>
    /// Interface that describes the methods that need to be implemented on classes that are supposed to be observer.
    /// <seealso cref="IObservable"/>
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// This method performs an update of the <seealso cref="IObserver"/>, triggered by a notification of an <seealso cref="IObservable"/>.
        /// </summary>
        void UpdateObserver();
    }
}