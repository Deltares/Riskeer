using System.ComponentModel;

namespace Core.Common.Utils.Events
{
    /// <summary>
    /// <see cref="CancelEventArgs"/> for collection changes.
    /// </summary>
    public class NotifyCollectionChangeEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangeEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that has been reformed on the collection.</param>
        /// <param name="item">The item affected by the action.</param>
        /// <param name="index">The current index of <paramref name="item"/>.</param>
        /// <param name="oldIndex">The previous index of <paramref name="item"/>.</param>
        public NotifyCollectionChangeEventArgs(NotifyCollectionChangeAction action, object item, int index, int oldIndex)
        {
            Action = action;
            Item = item;
            OldItem = null;
            Index = index;
            OldIndex = oldIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangeEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that has been reformed on the collection.</param>
        /// <param name="item">The item affected by the action.</param>
        /// <param name="index">The current index of <paramref name="item"/>.</param>
        /// <param name="oldIndex">The previous index of <paramref name="item"/>.</param>
        /// <param name="oldItem">The previous value of the effected element that is now replaced with <paramref name="item"/>.</param>
        public NotifyCollectionChangeEventArgs(NotifyCollectionChangeAction action, object item, int index, int oldIndex, object oldItem)
        {
            Action = action;
            Item = item;
            OldItem = oldItem;
            Index = index;
            OldIndex = oldIndex;
        }

        /// <summary>
        /// The operation took place.
        /// </summary>
        public NotifyCollectionChangeAction Action { get; private set; }

        /// <summary>
        /// The location where the element now resides.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The location where the element used to reside.
        /// </summary>
        public int OldIndex { get; private set; }

        /// <summary>
        /// The value of the element for which the event was generated.
        /// </summary>
        public object Item { get; private set; }

        /// <summary>
        /// The value of the element before it was replaced.
        /// </summary>
        public object OldItem { get; private set; }

        #region Factory methods

        /// <summary>
        /// Creates and initializes an new instance of the <see cref="NotifyCollectionChangeEventArgs"/>
        /// class for when the collection has completely been reset.
        /// </summary>
        public static NotifyCollectionChangeEventArgs CreateCollectionResetArgs()
        {
            return new NotifyCollectionChangeEventArgs(NotifyCollectionChangeAction.Reset, null, -1, -1);
        }

        /// <summary>
        /// Creates and initializes an new instance of the <see cref="NotifyCollectionChangeEventArgs"/>
        /// class for when an element has been added to the collection.
        /// </summary>
        /// <param name="addedElement">The element that has been added to the collection.</param>
        /// <param name="currentIndex">The index where <paramref name="addedElement"/> has been added.</param>
        public static NotifyCollectionChangeEventArgs CreateCollectionAddArgs(object addedElement, int currentIndex)
        {
            return new NotifyCollectionChangeEventArgs(NotifyCollectionChangeAction.Add, addedElement, currentIndex, -1);
        }

        /// <summary>
        /// Creates and initializes an new instance of the <see cref="NotifyCollectionChangeEventArgs"/>
        /// class for when an element has been removed from the collection.
        /// </summary>
        /// <param name="removedElement">The element that has been removed from the collection.</param>
        /// <param name="originalIndex">The index where <paramref name="removedElement"/> was removed.</param>
        public static NotifyCollectionChangeEventArgs CreateCollectionRemoveArgs(object removedElement, int originalIndex)
        {
            return new NotifyCollectionChangeEventArgs(NotifyCollectionChangeAction.Remove, removedElement, originalIndex, -1);
        }

        #endregion
    }
}