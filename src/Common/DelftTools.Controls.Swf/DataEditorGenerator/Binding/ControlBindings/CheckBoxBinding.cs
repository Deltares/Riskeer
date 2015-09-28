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

        void ControlCheckedChanged(object sender, System.EventArgs e)
        {
            DataValue = GetControlValue();
        }

        private void FillControl()
        {
            Control.Checked = (bool) (DataValue ?? false);
        }

        protected override void Deinitialize()
        {
        }

        protected override void DataSourceChanged()
        {
            FillControl();
        }
    }
}