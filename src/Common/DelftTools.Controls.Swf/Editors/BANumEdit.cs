using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Editors
{
    /// <summary>
    /// Numeric data entry control
    /// </summary>
    public class NumEdit : TextBox
    {
        #region NumEditType enum

        public enum NumEditType
        {
            Currency,
            Decimal,
            Single,
            Double,
            SmallInteger,
            Integer,
            LargeInteger
        }

        #endregion

        private NumEditType m_inpType;

        public NumEdit()
        {
            // set default input type
            InputType = NumEditType.Integer;

            // NOTE: Existing context menu allows Paste command, with no known
            //	method to intercept. Only option is to reset to empty.
            //	(setting to Null doesn't work)
            ContextMenu = new ContextMenu();
        }

        [Description("Sets the numeric type allowed"), Category("Behavior")]
        public NumEditType InputType
        {
            get { return m_inpType; }
            set
            {
                m_inpType = value;
            }
        }

        public override string Text
        {
            get { return base.Text; }
            set
            {
                if (IsValid(value, true))
                    base.Text = value;
            }
        }

        private bool IsValid(string val, bool user)
        {
            // this method validates the ENTIRE string
            //	not each character
            // Rev 1: Added bool user param. This bypasses preliminary checks
            //	that allow -, this is used by the OnLeave event
            //	to prevent
            bool ret = true;

            if (val.Equals("")
                || val.Equals(String.Empty))
                return ret;

            if (user)
            {
                // allow first char == '-'
                if (val.Equals("-"))
                    return ret;
            }

            // parse into dataType, errors indicate invalid value
            // NOTE: parsing also validates data type min/max
            try
            {
                switch (m_inpType)
                {
                    case NumEditType.Currency:
                        decimal dec = decimal.Parse(val);
                        int pos = val.IndexOf(".");
                        if (pos != -1)
                            ret = val.Substring(pos).Length <= 3; // 2 decimals + "."
                        //ret &= Min <= (double)dec && (double)dec <= Max;
                        break;
                    case NumEditType.Single:
                        float flt = float.Parse(val);
                        //ret &= Min <= flt && flt <= Max;
                        break;
                    case NumEditType.Double:
                        double dbl = double.Parse(val);
                        //ret &= Min <= dbl && dbl <= Max;
                        break;
                    case NumEditType.Decimal:
                        decimal dec2 = decimal.Parse(val);
                        //ret &= Min <= (double)dec2 && (double)dec2 <= Max;
                        break;
                    case NumEditType.SmallInteger:
                        short s = short.Parse(val);
                        //ret &= Min <= s && s <= Max;
                        break;
                    case NumEditType.Integer:
                        int i = int.Parse(val);
                        //ret &= Min <= i && i <= Max;
                        break;
                    case NumEditType.LargeInteger:
                        long l = long.Parse(val);
                        //ret &= Min <= l && l <= Max;
                        break;
                    default:
                        throw new ApplicationException();
                }
            }
            catch(Exception)
            {
                ret = false;
            }
            return ret;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // trap Ctrl-V paste and prevent invalid values
            //	return false to allow further processing
            if (keyData == (Keys) Shortcut.CtrlV || keyData == (Keys) Shortcut.ShiftIns)
            {
                IDataObject iData = Clipboard.GetDataObject();

                // assemble new string and check IsValid
                string newText;
                newText = base.Text.Substring(0, base.SelectionStart)
                          + (string) iData.GetData(DataFormats.Text)
                          + base.Text.Substring(base.SelectionStart + base.SelectionLength);

                // check if data to be pasted is convertable to inputType
                if (!IsValid(newText, true))
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnLeave(EventArgs e)
        {
            // handle - and leading zeros input since KeyPress handler must allow this
            if (base.Text != "")
            {
                if (!IsValid(base.Text, false))
                    base.Text = "";
                else if (Double.Parse(base.Text) == 0) // this used for -0, 000 and other strings
                    base.Text = "0";
            }
            base.OnLeave(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // assemble new text with new KeyStroke
            //	and pass to validation routine.

            // NOTES;
            //	1) Delete key is NOT passed here
            //	2) control passed here after ProcessCmdKey() is run

            char c = e.KeyChar;
            if (!Char.IsControl(c)) // not sure about this?? nothing in docs about what is Control char??
            {
                // prevent spaces
                if (c.ToString() == " ")
                {
                    e.Handled = true;
                    return;
                }

                string newText = base.Text.Substring(0, base.SelectionStart)
                                 + c.ToString() + base.Text.Substring(base.SelectionStart + base.SelectionLength);

                if (!IsValid(newText, true))
                    e.Handled = true;
            }
            base.OnKeyPress(e);
        }
    }
}