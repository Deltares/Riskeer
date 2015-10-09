using System;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class ComboBoxBinding : Binding<ComboBox>
    {
        protected override object GetControlValue()
        {
            return Control.SelectedItem;
        }

        protected override void Initialize()
        {
            Control.Format += ControlFormat;
            Control.DropDown += ControlFirstDropDown;
            Control.SelectionChangeCommitted += ControlSelectionChangeCommitted;

            Control.FormattingEnabled = true;
            Enum[] enumValues =
                Enum.GetValues(FieldDescription.ValueType)
                    .OfType<Enum>()
                    .ToArray();
            Control.Items.AddRange(enumValues);

            FillControl();
        }

        protected override void DataSourceChanged()
        {
            FillControl();
        }

        protected override void Deinitialize()
        {
            Control.Format -= ControlFormat;
            Control.SelectionChangeCommitted -= ControlSelectionChangeCommitted;
        }

        private void ControlSelectionChangeCommitted(object sender, EventArgs e)
        {
            CommitValue(GetControlValue());
        }

        private void ControlFirstDropDown(object sender, EventArgs e)
        {
            // measure the required width
            var width = Control.DropDownWidth;

            using (var g = Control.CreateGraphics())
            {
                var font = Control.Font;
                var vertScrollBarWidth = (Control.Items.Count > Control.MaxDropDownItems)
                                             ? SystemInformation.VerticalScrollBarWidth
                                             : 0;

                foreach (var s in Control.Items.Cast<object>().Select(Control.GetItemText))
                {
                    var newWidth = TextRenderer.MeasureText(g, s, font).Width + vertScrollBarWidth + 10; // +10 for a little bit of padding to the end.
                    if (width < newWidth)
                    {
                        width = newWidth;
                    }
                }
                Control.DropDownWidth = width;
            }

            // unsubscribe, so we do this only once
            Control.DropDown -= ControlFirstDropDown;
        }

        private static void ControlFormat(object sender, ListControlConvertEventArgs e)
        {
            e.Value = FormatValue(e.Value);
        }

        private static string FormatValue(object value)
        {
            return EnumDescriptionAttributeTypeConverter.GetEnumDescription((Enum) value);
        }

        private void FillControl()
        {
            Control.SelectedItem = DataValue;
        }
    }
}