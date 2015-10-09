using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils.Reflection;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class DataGridBinding : Binding<DataGridView>
    {
        private object previousData; // Reference to check if FillControl is required
        private bool oldRowHeadersVisible; // Remember original value when showing row error where row headers are normally hidden

        protected override object GetControlValue()
        {
            return DataValue;
        }

        protected override void Initialize()
        {
            oldRowHeadersVisible = Control.RowHeadersVisible;

            Control.KeyUp += ControlOnKeyUp;
            Control.CellValidating += OnCellValidating;
            Control.VisibleChanged += ControlVisibleChanged;
            Control.CausesValidation = true;

            FillControl();
        }

        protected override void Deinitialize()
        {
            // Value is reverted:
            for (var i = 0; i < Control.Rows.Count; i++)
            {
                Control.Rows[i].ErrorText = "";
            }
            Control.RowHeadersVisible = oldRowHeadersVisible;

            Control.CellValidating -= OnCellValidating;
            Control.KeyUp -= ControlOnKeyUp;
            Control.VisibleChanged -= ControlVisibleChanged;

            Control.SuspendDrawing(); // Required to prevent exception when Disposing WindowsFormsHost, see TOOLS-19890
            Control.DataSource = null;
            Control.ResumeDrawing();
        }

        protected override void DataSourceChanged()
        {
            FillControl();
        }

        private void ControlVisibleChanged(object sender, EventArgs e)
        {
            Control.ClearSelection();
            Control.CurrentCell = null;
        }

        private void FillControl()
        {
            FillDataSource();
            Control.ColumnHeadersVisible = Control.ColumnCount > 1;
            Control.ClearSelection();
            Control.CurrentCell = null;
        }

        private void FillDataSource()
        {
            // Collection object hasn't changed, so nothing to refresh:
            if (ReferenceEquals(previousData, DataValue))
            {
                return;
            }
            previousData = DataValue;

            var intList = DataValue as IList<int>;
            if (intList != null)
            {
                Control.DataSource =
                    new BindingList<ValueWrapper<int>>(
                        Enumerable.Range(0, intList.Count)
                                  .Select(i => new ValueWrapper<int>(intList, i))
                                  .ToList());
                return;
            }
            var doubleList = DataValue as IList<double>;
            if (doubleList != null)
            {
                Control.DataSource =
                    new BindingList<ValueWrapper<double>>(
                        Enumerable.Range(0, doubleList.Count)
                                  .Select(i => new ValueWrapper<double>(doubleList, i))
                                  .ToList());
                return;
            }
            var dateTimeList = DataValue as IList<DateTime>;
            if (dateTimeList != null)
            {
                Control.DataSource =
                    new BindingList<ValueWrapper<DateTime>>(
                        Enumerable.Range(0, dateTimeList.Count)
                                  .Select(i => new ValueWrapper<DateTime>(dateTimeList, i))
                                  .ToList());
                return;
            }
            var dataType = FieldDescription.ValueType.GetGenericArguments().ToList().FirstOrDefault();
            Control.DataSource = TypeUtils.CreateGeneric(typeof(BindingList<>), dataType, DataValue);
        }

        private static string GetExpectedTypeString(Type type)
        {
            if (type == typeof(double))
            {
                return "number";
            }
            if (type == typeof(int))
            {
                return "whole number";
            }
            if (type == typeof(DateTime))
            {
                return "date and time";
            }

            return type.ToString();
        }

        private void ControlOnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                // Value is reverted:
                for (var i = 0; i < Control.Rows.Count; i++)
                {
                    Control.Rows[i].ErrorText = "";
                }
                Control.RowHeadersVisible = oldRowHeadersVisible;
            }
        }

        private void OnCellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var genericArgument = FieldDescription.ValueType.GetGenericArguments().FirstOrDefault();
            if (genericArgument != null)
            {
                var row = Control.Rows[e.RowIndex];
                try
                {
                    var testValue = Convert.ChangeType(
                        e.FormattedValue.ToString(), // String in cell
                        genericArgument // Type of item in the collection
                        );
                    row.ErrorText = "";
                    Control.RowHeadersVisible = oldRowHeadersVisible;
                }
                catch (Exception)
                {
                    // Not a valid value:
                    Control.RowHeadersVisible = true;
                    row.ErrorText = String.Format("Value on cell (Col: {0}) not a valid {1}.",
                                                  Control.Columns[0].Name, GetExpectedTypeString(genericArgument));
                    e.Cancel = true;
                }
            }
        }

        #region Nested Type: ValueWrapper

        public class ValueWrapper<T>
        {
            private readonly IList<T> valueList;
            private readonly int index;

            public ValueWrapper(IList<T> valueList, int index)
            {
                this.valueList = valueList;
                this.index = index;
            }

            public T Value
            {
                get
                {
                    return valueList[index];
                }
                set
                {
                    valueList[index] = value;
                }
            }
        }

        #endregion
    }
}