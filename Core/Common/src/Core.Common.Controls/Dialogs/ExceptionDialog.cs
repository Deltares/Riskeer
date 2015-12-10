using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Core.Common.Controls.Properties;

namespace Core.Common.Controls.Dialogs
{
    /// <summary>
    /// Class for showing an exception dialog.
    /// The exception dialog can return the following results:
    /// <list type="bullet">
    /// <item>
    /// <description><see cref="DialogResult.OK"/>: this result represents a request for restarting the application.</description>
    /// </item>
    /// <item>
    /// <description><see cref="DialogResult.Cancel"/>: this result represents a request for closing the application.</description>
    /// </item>
    /// </list>
    /// </summary>
    public partial class ExceptionDialog : DialogBase
    {
        private Action openLogClicked;

        /// <summary>
        /// Constructs a new <see cref="ExceptionDialog"/>.
        /// </summary>
        /// <param name="owner">The owner of the dialog.</param>
        /// <param name="exception">The exception to show in the dialog.</param>
        public ExceptionDialog(IWin32Window owner, Exception exception) : base(owner, Resources.bug__exclamation)
        {
            InitializeComponent();

            buttonOpenLog.Visible = false;
            exceptionTextBox.Text = GetExceptionText(exception);
        }

        /// <summary>
        /// Gets or sets the action that should be performed after clicking the log button.
        /// </summary>
        /// <remarks>The log button is only visible when this action is set.</remarks>
        public Action OpenLogClicked
        {
            private get
            {
                return openLogClicked;
            }
            set
            {
                openLogClicked = value;

                buttonOpenLog.Visible = openLogClicked != null;
            }
        }

        protected override Button GetCancelButton()
        {
            return buttonExit;
        }

        private void ButtonRestartClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }

        private void ButtonExitClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;

            Close();
        }

        private void ButtonCopyTextToClipboardClick(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(exceptionTextBox.Text, true);
        }

        private void ButtonOpenLogClick(object sender, EventArgs e)
        {
            OpenLogClicked();
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
    }
}