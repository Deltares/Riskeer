using System.ComponentModel;

namespace DelftTools.Utils.Collections
{
    /// <summary>
    /// EventArgs for a collection that include the item and the action.
    /// 
    /// Note: for performance reasons we use fields here instead of properties.
    /// </summary>
    public class NotifyCollectionChangingEventArgs : CancelEventArgs
    {
        public NotifyCollectionChangingEventArgs(NotifyCollectionChangeAction action, object item, int index, int oldIndex) 
        {
            Action = action;
            Item = item;
            Index = index;
            OldIndex = oldIndex;
        }

        public NotifyCollectionChangingEventArgs()
        {
        }

        /// <summary>
        /// Indicate what operation took place such as add, remove etc...
        /// </summary>
        public NotifyCollectionChangeAction Action;

        /// <summary>
        /// The item added, removed or replaced. On replace the new item is given
        /// </summary>
        public virtual object Item { get; set; }

        /// <summary>
        /// Item that is replaced. (only filled out on replace)
        /// </summary>
        public virtual object OldItem { get; set; }

        /// <summary>
        /// When inserting, this is the position where the item is inserted.
        /// </summary>
        public int Index;

        /// <summary>
        /// Previous index (if changed). TODO: remove it. This is the index at which the insert was intent
        /// </summary>
        public int OldIndex;
    }
}