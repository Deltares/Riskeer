using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.Controls.Properties;

namespace Core.Common.Controls.Dialogs
{
    public partial class ExceptionDialog : DialogBase
    {
        public event EventHandler RestartClicked;

        public event EventHandler ExitClicked;

        public event EventHandler OpenLogClicked;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionDialog"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public ExceptionDialog(Exception exception)
        {
            InitializeComponent();

            Icon = Resources.bug__exclamation;

            exceptionTextBox.Text = GetExceptionText(exception);
        }

        private void ButtonRestartClick(object sender, EventArgs e)
        {
            buttonRestart.Enabled = false;
            buttonExit.Enabled = false;

            if (RestartClicked != null)
            {
                RestartClicked(this, null);
            }

            Close();
        }

        private void ButtonExitClick(object sender, EventArgs e)
        {
            buttonRestart.Enabled = false;
            buttonExit.Enabled = false;

            if (ExitClicked != null)
            {
                ExitClicked(this, null);
            }

            Close();
        }

        private void ButtonCopyTextToClipboardClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(exceptionTextBox.Text, true);
        }

        private void ButtonOpenLogClick(object sender, EventArgs e)
        {
            if (OpenLogClicked != null)
            {
                OpenLogClicked(this, null);
            }
        }

        private string GetExceptionText(Exception exception)
        {
            if (exception == null)
            {
                return "";
            }

            var str = exception.ToString();

            if (exception.InnerException != null)
            {
                str += string.Format(Resources.ExceptionDialog_GetExceptionText_Inner_exceptions_0_,
                                     exception.InnerException);
            }

            var reflectionTypeLoadException = exception as ReflectionTypeLoadException;
            if (reflectionTypeLoadException != null)
            {
                str += Resources.ExceptionDialog_GetExceptionText_Loader_exceptions;
                str = reflectionTypeLoadException.LoaderExceptions.Aggregate(str, (current, ex) => current + (ex + Environment.NewLine));
            }

            return str;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ExitClicked != null)
            {
                ExitClicked(this, null);
            }

            Close();

            base.OnClosing(e);
        }
    }
}