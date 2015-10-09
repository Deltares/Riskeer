using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Table
{
    public class NonMandatoryDataGridViewComboBoxCell : DataGridViewComboBoxCell
    {
        public bool AllowUserToEnterText { get; set; }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

            var comboBox = DataGridView.EditingControl as ComboBox;
            if (comboBox == null)
            {
                return;
            }

            comboBox.DropDownStyle = AllowUserToEnterText ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
            comboBox.Text = initialFormattedValue as string;
            comboBox.TextChanged += (s, e) => DataGridView.NotifyCurrentCellDirty(true);
        }

        // Copy of the DataGridViewComboBoxCell function "ParseFormattedValue" only without throwing an error
        // when the value is not in the items list
        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle,
                                                   TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            if (valueTypeConverter == null)
            {
                if (ValueMember != null)
                {
                    var valueMemberProperty = GetPrivateBaseProperty<PropertyDescriptor>("ValueMemberProperty");
                    valueTypeConverter = valueMemberProperty.Converter;
                }
                else if (DisplayMember != null)
                {
                    var displayMemberProperty = GetPrivateBaseProperty<PropertyDescriptor>("DisplayMemberProperty");
                    valueTypeConverter = displayMemberProperty.Converter;
                }
            }

            if (DataSource != null && (!string.IsNullOrEmpty(DisplayMember) || !string.IsNullOrEmpty(ValueMember)))
            {
                var value = BaseParseFormattedValueInternal(ValueType, formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);

                var originalValue = value;
                if (LookupValue(originalValue, out value))
                {
                    return value;
                }

                return originalValue == DBNull.Value ? DBNull.Value : originalValue;
            }

            return BaseParseFormattedValueInternal(ValueType, formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }

        protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle,
                                                    TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            return value == null || Items.Contains(value)
                       ? base.GetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context)
                       : BaseGetFormattedValue(value, rowIndex, ref cellStyle, valueTypeConverter, formattedValueTypeConverter, context);
        }

        // Copy of the DataGridViewCell function "GetFormattedValue" to bypass the DataGridViewComboBoxCell function "BaseGetFormattedValue"
        // because it throws an error when value is not in the Items list
        private object BaseGetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle,
                                             TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
        {
            if (DataGridView == null)
            {
                return null;
            }

            var methodInfo = typeof(DataGridView).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(m => m.Name == "OnCellFormatting" && m.GetParameters().Count() == 5);

            var gdvcfe = (DataGridViewCellFormattingEventArgs) methodInfo.Invoke(DataGridView, new[]
            {
                ColumnIndex,
                rowIndex,
                value,
                FormattedValueType,
                cellStyle
            });

            cellStyle = gdvcfe.CellStyle;
            var formattingApplied = gdvcfe.FormattingApplied;
            var formattedValue = gdvcfe.Value;
            var checkFormattedValType = true;

            if (!formattingApplied && FormattedValueType != null && (formattedValue == null || !FormattedValueType.IsInstanceOfType(formattedValue)))
            {
                try
                {
                    formattedValue = FormatObject(formattedValue, FormattedValueType, valueTypeConverter, formattedValueTypeConverter, cellStyle.Format,
                                                  cellStyle.FormatProvider, cellStyle.NullValue, cellStyle.DataSourceNullValue);
                }
                catch (Exception exception)
                {
                    ThrowException(rowIndex, context, exception);
                    checkFormattedValType = false;
                }
            }

            if (checkFormattedValType && (formattedValue == null || FormattedValueType == null || !FormattedValueType.IsInstanceOfType(formattedValue)))
            {
                if (formattedValue == null && cellStyle.NullValue == null && FormattedValueType != null && !typeof(ValueType).IsAssignableFrom(FormattedValueType))
                {
                    // null is an acceptable formatted value
                    return null;
                }

                var exception = FormattedValueType == null
                                    ? new FormatException("Formatted value type null")
                                    : new FormatException("Formatted value has wrong type");

                ThrowException(rowIndex, context, exception);
            }
            return formattedValue;
        }

        private T GetPrivateBaseProperty<T>(string propertyName)
        {
            var propertyInfo = typeof(DataGridViewComboBoxCell).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T) propertyInfo.GetValue(this, new object[]
            {});
        }

        private bool LookupValue(object originalValue, out object value)
        {
            var methodInfo = typeof(DataGridViewComboBoxCell).GetMethod("LookupValue", BindingFlags.NonPublic | BindingFlags.Instance);

            var parameters = new[]
            {
                originalValue,
                Value
            };
            var result = methodInfo.Invoke(this, parameters);

            value = parameters[1];
            return (bool) result;
        }

        private object BaseParseFormattedValueInternal(Type valueType, object formattedValue, DataGridViewCellStyle cellStyle, TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException("cellStyle");
            }
            if (FormattedValueType == null)
            {
                throw new FormatException("FormattedValueType is null");
            }
            if (valueType == null)
            {
                throw new FormatException("FormattedValueType is null");
            }
            if (formattedValue == null || !FormattedValueType.IsInstanceOfType(formattedValue))
            {
                throw new ArgumentException("formattedValue is of the wrong type");
            }

            return ParseObject(formattedValue, valueType, FormattedValueType, valueTypeConverter, formattedValueTypeConverter,
                               cellStyle.FormatProvider, cellStyle.NullValue, cellStyle.DataSourceNullValue);
        }

        private static object ParseObject(object value, Type targetType, Type sourceType, TypeConverter targetConverter, TypeConverter sourceConverter,
                                          IFormatProvider formatInfo, object formattedNullValue, object dataSourceNullValue)
        {
            var type = typeof(DataGridView).Assembly.GetType("System.Windows.Forms.Formatter");
            var methodInfo = type.GetMethod("ParseObject");

            return methodInfo.Invoke(null, new[]
            {
                value,
                targetType,
                sourceType,
                targetConverter,
                sourceConverter,
                formatInfo,
                formattedNullValue,
                dataSourceNullValue
            });
        }

        private static object FormatObject(object formattedValue, Type formattedValueType, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter
                                           , string format, IFormatProvider formatProvider, object nullValue, object dataSourceNullValue)
        {
            // call FormatObject method of internal class System.Windows.Forms.Formatter
            var type = typeof(DataGridView).Assembly.GetType("System.Windows.Forms.Formatter");
            var methodInfo = type.GetMethod("FormatObject");

            return methodInfo.Invoke(null, new[]
            {
                formattedValue,
                formattedValueType,
                valueTypeConverter,
                formattedValueTypeConverter,
                format,
                formatProvider,
                nullValue,
                dataSourceNullValue
            });
        }

        private void ThrowException(int rowIndex, DataGridViewDataErrorContexts context, Exception exception)
        {
            // throw DataGridView exception
            var dgvdee = new DataGridViewDataErrorEventArgs(exception, ColumnIndex, rowIndex, context);

            RaiseDataError(dgvdee);

            if (dgvdee.ThrowException)
            {
                throw dgvdee.Exception;
            }
        }
    }
}