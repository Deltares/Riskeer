using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    /// <summary>
    /// Simple input box dialog 
    /// </summary>
    public partial class InputTextDialog : Form
    {
        private bool escapeButtonPressed;

        public InputTextDialog()
        {
            InitializeComponent();
            InitialText = "";
            textBox1.Validating += textBox1_Validating;
            errorProvider.SetIconAlignment(ButtonOk, ErrorIconAlignment.MiddleLeft);
            ValidationErrorMsg = "Please verify the input is valid";
        }

        public bool Multiline
        {
            get
            {
                return textBox1.Multiline;
            }
            set
            {
                textBox1.Multiline = value;
                AcceptButton = value ? null : ButtonOk;
            }
        }

        ///<summary>
        /// Text the user entered in the dialog
        ///</summary>
        public string EnteredText
        {
            get
            {
                return textBox1.Text;
            }
            set
            {
                textBox1.Text = value;
            }
        }

        public string InitialText { get; set; }

        public Predicate<string> ValidationMethod { get; set; }

        public string ValidationErrorMsg { get; set; }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                escapeButtonPressed = true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (escapeButtonPressed)
            {
                // no validation on cancel
                escapeButtonPressed = false;
                e.Cancel = false;
                return;
            }

            if (!DoValidate())
            {
                e.Cancel = true;
                return;
            }
            errorProvider.SetError(ButtonOk, string.Empty);
        }

        private bool DoValidate()
        {
            if (ValidationMethod != null)
            {
                if (!ValidationMethod(textBox1.Text))
                {
                    errorProvider.SetError(ButtonOk, ValidationErrorMsg);
                    return false;
                }
            }
            return true;
        }

        private void ButtonOkClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ButtonCancelClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void InputTextDialogActivated(object sender, EventArgs e)
        {
            //set focus to the textbox when the dialog is shown
            textBox1.Text = InitialText;
            textBox1.Focus();
        }
    }
}