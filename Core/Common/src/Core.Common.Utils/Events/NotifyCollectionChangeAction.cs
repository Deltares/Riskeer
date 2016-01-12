namespace Core.Common.Utils.Events
{
    /// <summary>
    /// Marks the type of action that has been performed on a collection.
    /// </summary>
    public enum NotifyCollectionChangeAction
    {
        /// <summary>
        /// An element has been added (or inserted at a specific index).
        /// </summary>
        Add,
        /// <summary>
        /// An element has been removed.
        /// </summary>
        Remove,
        /// <summary>
        /// An element has been replaced by another.
        /// </summary>
        Replace,
        /// <summary>
        /// The collection as a whole as changed.
        /// </summary>
        Reset
    }
}