using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.DataEditorGenerator.Binding.ControlBindings
{
    internal class TextBoxBinding : Binding<TextBox>
    {
        private object validatedValue;

        public ErrorProvider ErrorProvider { get; set; }

        void ControlKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) 
                return;

            if (ValidateValue())
                CommitValue(validatedValue);
        }

        void ControlValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !ValidateValue();
        }
        
        void ControlLostFocus(object sender, EventArgs e)
        {
            if (ValidateValue())
                CommitValue(validatedValue);
        }

        void ControlValidated(object sender, EventArgs e)
        {
            CommitValue(validatedValue);
        }
        
        private bool ValidateValue()
        {
            //grab value from control
            var txtValue = Control.Text;

            //parse value to target type (eg, int, double)
            try
            {
                validatedValue = Convert.ChangeType(txtValue, FieldDescription.ValueType);
            }
            catch (Exception)
            {
                if (ErrorProvider != null)
                {
                    ErrorProvider.SetIconAlignment(Control, ErrorIconAlignment.MiddleLeft);
                    ErrorProvider.SetError(Control, String.Format("Value not a valid {0}.", GetValueTypeDescription(FieldDescription.ValueType)));
                }
                return false;
            }

            //do inverse unit conversion

            //do any additional validation

            if (ErrorProvider != null) ErrorProvider.SetError(Control, "");
            return true;
        }

        private static string GetValueTypeDescription(Type valueType)
        {
            if (valueType == typeof (double) || valueType == typeof(float))
            {
                return "number";
            }
            if (valueType == typeof (int) || valueType == typeof(long))
            {
                return "whole number";
            }
            if (valueType == typeof (string))
            {
                return "piece of text";
            }
            return valueType.ToString();
        }

        protected override object GetControlValue()
        {
            if (ValidateValue())
                return validatedValue;
            return DataValue;
        }

        protected override void Initialize()
        {
            Control.KeyUp += ControlKeyUp;
            Control.Validating += ControlValidating;
            Control.Validated += ControlValidated;
            Control.LostFocus += ControlLostFocus;
            Control.CausesValidation = true;

            FillControl();
        }

        private void FillControl()
        {
            var value = DataValue;
            Control.Text = value != null ? value.ToString() : "(null)";
        }

        protected override void Deinitialize()
        {
            if (ErrorProvider != null) ErrorProvider.SetError(Control, "");
            Control.KeyUp -= ControlKeyUp;
            Control.Validating -= ControlValidating;
            Control.Validated -= ControlValidated;
            Control.LostFocus -= ControlLostFocus;
        }

        protected override void DataSourceChanged()
        {
            FillControl();
        }
    }
}