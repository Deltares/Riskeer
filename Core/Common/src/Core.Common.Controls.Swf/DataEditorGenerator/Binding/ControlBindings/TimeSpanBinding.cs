using System;

namespace Core.Common.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class TimeSpanBinding : Binding<TimeSpanEditor>
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

        protected override void Deinitialize()
        {
            Control.ValueChanged -= ControlValueChanged;
        }

        protected override void DataSourceChanged()
        {
            FillControl();
        }

        private void ControlValueChanged(object sender, EventArgs e)
        {
            DataValue = GetControlValue();
        }

        private void FillControl()
        {
            if (DataValue != null)
            {
                var value = (TimeSpan) DataValue;
                Control.Value = value;
            }
        }
    }
}