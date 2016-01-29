using System;

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Event arguments to be used in the event that node data has been deleted from a <see cref="TreeView"/>.
    /// </summary>
    public class TreeNodeDataDeletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeDataDeletedEventArgs"/> class.
        /// </summary>
        /// <param name="deletedDataInstance">The deleted data instance.</param>
        public TreeNodeDataDeletedEventArgs(object deletedDataInstance)
        {
            DeletedDataInstance = deletedDataInstance;
        }

        /// <summary>
        /// Gets the data instance deleted from the <see cref="TreeView"/>.
        /// </summary>
        public object DeletedDataInstance { get; private set; }
    }
}