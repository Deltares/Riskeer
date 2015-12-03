using System.Drawing;

namespace Core.Common.Controls.Swf.Table
{
    public class TableViewCellStyle : TableViewCell
    {
        public TableViewCellStyle(int rowIndex, TableViewColumn column, bool selected) : base(rowIndex, column)
        {
            Selected = selected;
        }

        public Color ForeColor { get; set; }

        public Color BackColor { get; set; }

        public bool Selected { get; private set; }
    }
}