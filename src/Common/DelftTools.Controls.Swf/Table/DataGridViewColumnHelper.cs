using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Editors;

namespace DelftTools.Controls.Swf.Table
{
    public static class DataGridViewColumnHelper
    {
        public static DataGridViewColumn CreateGridViewColumn(DataGridViewColumn column, ITypeEditor typeEditor)
        {
            var boxTypeEditor = typeEditor as ComboBoxTypeEditor;
            if (boxTypeEditor != null)
            {
                return CreateComboBoxColumn(column, boxTypeEditor);
            }

            var buttonTypeEditor = typeEditor as ButtonTypeEditor;
            if (buttonTypeEditor != null)
            {
                return CreateButtonColumn(column);
            }

            if (typeEditor == null && column.ValueType == typeof(Image))
            {
                return CreateImageColumn(column);
            }

            if (typeEditor == null && column.ValueType == typeof(bool))
            {
                return CreateCheckBoxColumn(column);
            }

            return CreateTextBoxColumn(column);
        }

        private static DataGridViewColumn CreateCheckBoxColumn(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn is DataGridViewCheckBoxColumn)
            {
                return dataGridViewColumn;
            }

            var checkboxColumn = new DataGridViewCheckBoxColumn(false);
            CopyDataGridViewColumnValues(dataGridViewColumn, checkboxColumn);

            return checkboxColumn;
        }

        private static DataGridViewColumn CreateImageColumn(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn is DataGridViewImageColumn)
            {
                return dataGridViewColumn;
            }

            var imageColumn = new DataGridViewImageColumn();
            CopyDataGridViewColumnValues(dataGridViewColumn, imageColumn);

            imageColumn.ImageLayout = DataGridViewImageCellLayout.Normal;

            return imageColumn;
        }

        private static DataGridViewColumn CreateTextBoxColumn(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn is DataGridViewTextBoxColumn)
            {
                return dataGridViewColumn;
            }

            var textBoxColumn = new DataGridViewTextBoxColumn();
            CopyDataGridViewColumnValues(dataGridViewColumn, textBoxColumn);

            return textBoxColumn;
        }

        private static DataGridViewColumn CreateButtonColumn(DataGridViewColumn dataGridViewColumn)
        {
            if (dataGridViewColumn is DataGridViewButtonColumn)
            {
                return dataGridViewColumn;
            }

            var buttonColumn = new DataGridViewButtonColumn();
            CopyDataGridViewColumnValues(dataGridViewColumn, buttonColumn);

            return buttonColumn;
        }

        private static DataGridViewColumn CreateComboBoxColumn(DataGridViewColumn dataGridViewColumn, ComboBoxTypeEditor boxTypeEditor)
        {
            var comboBoxColumn = dataGridViewColumn as DataGridViewComboBoxColumn;
            if (comboBoxColumn == null)
            {
                comboBoxColumn = new DataGridViewComboBoxColumn();
                CopyDataGridViewColumnValues(dataGridViewColumn, comboBoxColumn);
            }

            if (!boxTypeEditor.ItemsMandatory)
            {
                comboBoxColumn.CellTemplate = new NonMandatoryDataGridViewComboBoxCell();
            }

            comboBoxColumn.DataSource = boxTypeEditor.Items.OfType<object>().ToDictionary(o => GetComboBoxItemText(o, boxTypeEditor), GetComboBoxItemValue).ToList();
            comboBoxColumn.DisplayMember = "key";
            comboBoxColumn.ValueMember = "value";

            comboBoxColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            comboBoxColumn.FlatStyle = FlatStyle.Flat;

            return comboBoxColumn;
        }

        private static object GetComboBoxItemValue(object o)
        {
            return o.GetType().IsEnum ? (int) o : o;
        }

        private static string GetComboBoxItemText(object o, ComboBoxTypeEditor boxTypeEditor)
        {
            if (boxTypeEditor.CustomFormatter != null)
            {
                return boxTypeEditor.CustomFormatter.Format(null, o, null);
            }

            if (boxTypeEditor.DisplayFormat != null)
            {
                return string.Format(boxTypeEditor.DisplayFormat, o);
            }

            return o.ToString();
        }

        private static void CopyDataGridViewColumnValues(DataGridViewColumn dataGridViewColumnSource, DataGridViewColumn dataGridViewColumnTarget)
        {
            dataGridViewColumnTarget.AutoSizeMode = dataGridViewColumnSource.AutoSizeMode;
            dataGridViewColumnTarget.DataPropertyName = dataGridViewColumnSource.DataPropertyName;
            dataGridViewColumnTarget.DefaultCellStyle = dataGridViewColumnSource.DefaultCellStyle.Clone();
            dataGridViewColumnTarget.DefaultHeaderCellType = dataGridViewColumnSource.DefaultHeaderCellType;
            dataGridViewColumnTarget.DisplayIndex = dataGridViewColumnSource.DisplayIndex;
            dataGridViewColumnTarget.DividerWidth = dataGridViewColumnSource.DividerWidth;
            dataGridViewColumnTarget.FillWeight = dataGridViewColumnSource.FillWeight;
            dataGridViewColumnTarget.Frozen = dataGridViewColumnSource.Frozen;
            dataGridViewColumnTarget.HeaderCell = (DataGridViewColumnHeaderCell) dataGridViewColumnSource.HeaderCell.Clone();
            dataGridViewColumnTarget.HeaderText = dataGridViewColumnSource.HeaderText;
            dataGridViewColumnTarget.MinimumWidth = dataGridViewColumnSource.MinimumWidth;
            dataGridViewColumnTarget.Name = dataGridViewColumnSource.Name;
            dataGridViewColumnTarget.ReadOnly = dataGridViewColumnSource.ReadOnly;
            dataGridViewColumnTarget.Resizable = dataGridViewColumnSource.Resizable;
            dataGridViewColumnTarget.Selected = dataGridViewColumnSource.Selected;
            dataGridViewColumnTarget.SortMode = dataGridViewColumnSource.SortMode;
            dataGridViewColumnTarget.ToolTipText = dataGridViewColumnSource.ToolTipText;
            dataGridViewColumnTarget.ValueType = dataGridViewColumnSource.ValueType;
            dataGridViewColumnTarget.Visible = dataGridViewColumnSource.Visible;
            dataGridViewColumnTarget.Width = dataGridViewColumnSource.Width;

            dataGridViewColumnTarget.ContextMenuStrip = dataGridViewColumnSource.ContextMenuStrip;
            dataGridViewColumnTarget.Tag = dataGridViewColumnSource.Tag;

            dataGridViewColumnSource.ContextMenuStrip = null;
            dataGridViewColumnSource.Tag = null;
        }
    }
}