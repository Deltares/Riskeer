using System;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings;
using DelftTools.Controls.Swf.DataEditorGenerator.Metadata;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding
{
    internal static class DataBinder
    {
        public static IBinding Bind(FieldUIDescription fieldDescription, Control editControl, Control parentControl, ErrorProvider errorProvider)
        {
            var binding = GetBindingForControl(editControl, fieldDescription, errorProvider);
            binding.InitializeControl(fieldDescription, editControl, parentControl);
            return binding;
        }

        private static IBinding GetBindingForControl(Control editControl, FieldUIDescription fieldDescription, ErrorProvider errorProvider)
        {
            if (fieldDescription.CustomControlHelper != null)
            {
                return new CustomControlBinding(fieldDescription.CustomControlHelper);
            }
            if (editControl is TextBox)
            {
                return new TextBoxBinding
                {
                    ErrorProvider = errorProvider
                };
            }
            if (editControl is ComboBox)
            {
                return new ComboBoxBinding();
            }
            if (editControl is TimeSpanEditor)
            {
                return new TimeSpanBinding();
            }
            if (editControl is DateTimePicker)
            {
                return new DateTimePickerBinding();
            }
            if (editControl is CheckBox)
            {
                return new CheckBoxBinding();
            }
            if (editControl is DataGridView)
            {
                return new DataGridBinding();
            }
            throw new NotImplementedException(string.Format("Unknown control type: {0}", editControl.GetType()));
        }
    }
}