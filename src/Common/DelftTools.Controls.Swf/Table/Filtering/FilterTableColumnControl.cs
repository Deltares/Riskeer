using System;
using System.Windows.Forms;
using DelftTools.Utils.Reflection;

namespace DelftTools.Controls.Swf.Table.Filtering
{
    public partial class FilterTableColumnControl : UserControl
    {
        private ITableViewColumn tableViewColumn;
        private IFilterControl columnFilterEditor;

        public FilterTableColumnControl()
        {
            InitializeComponent();
        }

        public ITableViewColumn TableViewColumn
        {
            get { return tableViewColumn; }
            set
            {
                tableViewColumn = value;
                labelColumnName.Text = TableViewColumn.Caption;
                
                columnFilterEditor = GetColumnFilterEditor();
                columnFilterEditor.Filter = tableViewColumn.FilterString != null
                    ? tableViewColumn.FilterString.Replace(tableViewColumn.Name + " ", "")
                    : null;

                panelEditor.Controls.Clear();
                panelEditor.Controls.Add((Control) columnFilterEditor);
            }
        }

        private IFilterControl GetColumnFilterEditor()
        {
            var columnType = tableViewColumn.ColumnType;

            if (columnType.IsNumericalType())
            {
                return new FilterNumericControl();
            }

            if (columnType == typeof (string))
            {
                return new FilterTextControl();
            }

            if (columnType == typeof(DateTime))
            {
                return new FilterDateTimeControl();
            }

            if (columnType == typeof(bool))
            {
                return new FilterBooleanControl();
            }

            throw new Exception("Can't create filter editor for type : " + columnType);
        }

        private void toolStripButtonOk_Click(object sender, EventArgs e)
        {
            TableViewColumn.FilterString = tableViewColumn.Name + " " + columnFilterEditor.Filter;
            Visible = false;
        }

        private void toolStripButtonCancel_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void toolStripButtonClearFilter_Click(object sender, EventArgs e)
        {
            TableViewColumn.FilterString = null;
            Visible = false;
        }
    }
}
