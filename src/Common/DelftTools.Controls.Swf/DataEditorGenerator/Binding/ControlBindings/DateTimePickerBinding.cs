using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class DateTimePickerBinding : Binding<DateTimePicker>
    {
        protected override object GetControlValue()
        {
            return Control.Value;
        }

        protected override void Initialize()
        {
            Control.ValueChanged += ControlValueChanged;
            FillControl();
        }

        void ControlValueChanged(object sender, EventArgs e)
        {
            DataValue = GetControlValue();
        }

        private void FillControl()
        {
            var value = (DateTime) DataValue;

            if (value > DateTime.MinValue)
                Control.Value = value;
        }

        protected override void Deinitialize()
        {
            Control.ValueChanged -= ControlValueChanged;
        }

        protected override void DataSourceChanged()
        {
            FillControl();
        }
    }
}