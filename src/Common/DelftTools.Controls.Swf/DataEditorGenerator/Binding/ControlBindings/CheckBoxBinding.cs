using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class CheckBoxBinding : Binding<CheckBox>
    {
        protected override object GetControlValue()
        {
            return Control.Checked;
        }

        protected override void Initialize()
        {
            Control.CheckedChanged += ControlCheckedChanged;
            FillControl();
        }

        protected override void Deinitialize() {}

        protected override void DataSourceChanged()
        {
            FillControl();
        }

        private void ControlCheckedChanged(object sender, EventArgs e)
        {
            DataValue = GetControlValue();
        }

        private void FillControl()
        {
            Control.Checked = (bool) (DataValue ?? false);
        }
    }
}