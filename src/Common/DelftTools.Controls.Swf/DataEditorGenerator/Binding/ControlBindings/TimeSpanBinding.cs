using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
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

        void ControlValueChanged(object sender, EventArgs e)
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