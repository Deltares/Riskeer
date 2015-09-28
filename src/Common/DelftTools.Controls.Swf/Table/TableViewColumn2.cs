using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Table
{
    public class TableViewColumn2 : ITableViewColumn
    {
        internal DataGridViewColumn Column;
        private readonly TableView2 tableView2;
        private string filterString;
        private ITypeEditor editor;
        private string toolTip;
        private SortOrder sortOrder;
        private bool sortingAllowed;

        public TableViewColumn2(DataGridViewColumn dataGridViewColumn, TableView2 tableView2)
        {
            Column = dataGridViewColumn;
            this.tableView2 = tableView2;
            sortingAllowed = true;
        }

        public string Name
        {
            get { return Column.Name; }
        }

        public bool Visible
        {
            get { return Column.Visible; }
            set { Column.Visible = value; }
        }

        public ITypeEditor Editor
        {
            get { return editor; }
            set
            {
                editor = value;
                var newColumn = DataGridViewColumnHelper.CreateGridViewColumn(Column, editor);
                if (DataGridViewColumnHelper.ReplaceColumn(tableView2, Column, newColumn))
                {
                    Column = newColumn;
                }
            }
        }

        public string Caption
        {
            get { return Column.HeaderText; }
            set { Column.HeaderText = value; }
        }

        public int DisplayIndex
        {
            get { return Column.DisplayIndex; }
            set { Column.DisplayIndex = value; }
        }

        public bool ReadOnly
        {
            get { return Column.ReadOnly; }
            set
            {
                Column.ReadOnly = value;
                Column.DefaultCellStyle.BackColor = value ? tableView2.ReadOnlyCellBackColor : Color.Empty;
                Column.DefaultCellStyle.ForeColor = value ? tableView2.ReadOnlyCellForeColor : Color.Empty;
            }
        }

        public bool SortingAllowed
        {
            get { return tableView2.AllowColumnSorting && sortingAllowed; }
            set { sortingAllowed = value; }
        }

        public SortOrder SortOrder
        {
            get { return sortOrder; }
            set
            {
                sortOrder = value;

                if (!tableView2.bindingSource.SupportsSorting) return;

                switch (sortOrder)
                {
                    case SortOrder.None:
                        tableView2.bindingSource.RemoveSort();
                        break;
                    case SortOrder.Ascending:
                        tableView2.DataGridView.Sort(Column, ListSortDirection.Ascending);
                        break;
                    case SortOrder.Descending:
                        tableView2.DataGridView.Sort(Column, ListSortDirection.Descending);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public string FilterString
        {
            get { return filterString; }
            set
            {
                filterString = value;
                SetToolTipText();
                tableView2.UpdateFilter();
                tableView2.RaiseColumnFilterChanged(this);
            }
        }

        public int AbsoluteIndex
        {
            get { return Column.Index; }
        }

        public Type ColumnType
        {
            get { return Column.ValueType; }
        }
        
        public string ToolTip 
        {
            get { return toolTip; }
            set
            {
                toolTip = value;
                SetToolTipText();
            }
        }

        public int Width
        {
            get { return Column.Width; }
            set { Column.Width = value; }
        }

        public ICustomFormatter CustomFormatter { get; set; }

        public string DisplayFormat
        {
            get { return Column.DefaultCellStyle.Format; }
            set
            {
                Column.DefaultCellStyle.Format = value;
            }
        }

        public bool Pinned
        {
            get { return Column.Frozen; }
            set
            {
                if (Pinned == value) return;

                if (!value)
                {
                    //move column to the end of the pinned columns )
                    var numberOfPinndedColumns = tableView2.Columns.Count(c => c.Pinned) -1;//subtract this column
                    Column.DisplayIndex = numberOfPinndedColumns; 

                    Column.Frozen = false;

                    // Restore original display index
                    var unPinnedColumns = tableView2.Columns.Where(c => !c.Pinned).ToList();
                    var columnToTheLeft = unPinnedColumns.LastOrDefault(c => c.AbsoluteIndex < AbsoluteIndex);
                    DisplayIndex = columnToTheLeft != null
                                           ? columnToTheLeft.DisplayIndex
                                           : tableView2.Columns.Count - unPinnedColumns.Count;
                }
                else
                {
                    var pinnedColumns = tableView2.Columns.Where(c => c.Pinned).ToList();
                    var columnToTheLeft = pinnedColumns.LastOrDefault(c => c.AbsoluteIndex < AbsoluteIndex);
                    DisplayIndex = columnToTheLeft != null
                                       ? columnToTheLeft.DisplayIndex + 1
                                       : 0;
                    Column.Frozen = true;
                }
            }
        }
        
        public bool IsUnbound { get; set; }

        public object DefaultValue { get; set; }

        public void Dispose()
        {
        }

        private void SetToolTipText()
        {
            Column.ToolTipText = (!string.IsNullOrEmpty(filterString))
                ? string.Format("{0} (Filter : {1})", toolTip, filterString)
                : toolTip;
        }
    }
}