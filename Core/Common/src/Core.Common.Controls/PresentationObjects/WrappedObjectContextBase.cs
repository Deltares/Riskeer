using System;

using Core.Common.Base;

namespace Core.Common.Controls.PresentationObjects
{
    /// <summary>
    /// This abstract class provides common boilerplate implementations for presentation
    /// objects based on a single object that needs additional dependencies or behavior
    /// for the UI layer of the application.
    /// </summary>
    /// <typeparam name="T">The object type of the wrapped instance.</typeparam>
    public abstract class WrappedObjectContextBase<T> : IObservable, IEquatable<WrappedObjectContextBase<T>> where T : IObservable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedObjectContextBase{T}"/> class.
        /// </summary>
        /// <param name="wrappedData">The wrapped data.</param>
        /// <exception cref="System.ArgumentNullException">When <paramref name="wrappedData"/> is <c>null</c>.</exception>
        protected WrappedObjectContextBase(T wrappedData)
        {
            if (wrappedData == null)
            {
                throw new ArgumentNullException("wrappedData", "Wrapped data of context cannot be null.");
            }
            WrappedData = wrappedData;
        }

        /// <summary>
        /// Gets the data wrapped in this presentation object.
        /// </summary>
        public T WrappedData { get; private set; }

        #region Equality members

        public bool Equals(WrappedObjectContextBase<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return WrappedData.Equals(other.WrappedData);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            return Equals(obj as WrappedObjectContextBase<T>);
        }

        public override int GetHashCode()
        {
            return WrappedData.GetHashCode();
        }

        #endregion

        #region IObservable implementation

        public void Attach(IObserver observer)
        {
            WrappedData.Attach(observer);
        }

        public void Detach(IObserver observer)
        {
            WrappedData.Detach(observer);
        }

        public void NotifyObservers()
        {
            WrappedData.NotifyObservers();
        }

        #endregion
    }
}