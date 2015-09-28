using System.Collections.Generic;

namespace DelftTools.Utils.Collections.Generic
{
    /// <summary>
    /// This interface defines a list which observes all changes in the underlying items and
    /// subscribes to their INotifyPropertyChanged and INotifyCollectionChange if they implement them.
    /// </summary>
    /// <typeparam name="T">The datatype that is being added/removed</typeparam>
    public interface IEventedList<T> : IList<T>, INotifyCollectionChange
    {
        void AddRange(IEnumerable<T> enumerable);
    }
}