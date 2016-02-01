// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

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