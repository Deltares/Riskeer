using System.Collections.Generic;

namespace DelftTools.Shell.Core
{
    /// <summary>
    /// Extends the <see cref="List{T}"/> class with implementation for <see cref="IObservable"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class ObservableList<T> : List<T>, IObservable
    {
        private readonly IList<IObserver> observers = new List<IObserver>();

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