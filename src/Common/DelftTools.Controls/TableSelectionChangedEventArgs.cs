using System;
using System.Collections.Generic;

namespace DelftTools.Controls
{
    public class TableSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Selection Changed EventArgs
        /// </summary>
        public TableSelectionChangedEventArgs(IList<TableViewCell> cells)
        {
            Cells = cells;
        }

        public IList<TableViewCell> Cells { get; private set; }
    }
}