namespace Core.Common.Utils.Events
{
    /// <summary>
    /// Interface for defining collections that fire an event when a collection has changed.
    /// </summary>
    public interface INotifyCollectionChanged
    {
        /// <summary>
        /// Occurs when the collection has been changed (such as an element has been added, 
        /// removed, inserted, replaced or the collection has completely been changed).
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}