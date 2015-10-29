namespace Core.Common.Controls
{
    public class TableViewCell
    {
        public TableViewCell(int rowIndex, ITableViewColumn column)
        {
            RowIndex = rowIndex;
            Column = column;
        }

        /// <summary>
        /// Row index of the cell
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Column of the cell
        /// </summary>
        public ITableViewColumn Column { get; set; }
    }
}