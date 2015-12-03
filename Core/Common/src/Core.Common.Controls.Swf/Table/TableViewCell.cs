namespace Core.Common.Controls.Swf.Table
{
    public class TableViewCell
    {
        public TableViewCell(int rowIndex, TableViewColumn column)
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
        public TableViewColumn Column { get; set; }
    }
}