﻿using System.Drawing;

namespace DelftTools.Controls.Swf.Table
{
    public class TableViewCellStyle : TableViewCell
    {
        public TableViewCellStyle(int rowIndex, ITableViewColumn column, bool selected) : base (rowIndex, column)
        {
            Selected = selected;
        }
        
        public Color ForeColor { get; set; }

        public Color BackColor { get; set; }

        public bool Selected { get; private set; }
    }
}