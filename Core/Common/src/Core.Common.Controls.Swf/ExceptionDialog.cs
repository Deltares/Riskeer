using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf
{
    public partial class ExceptionDialog : Form
    {
        public event EventHandler RestartClicked;

        public event EventHandler ExitClicked;

        public event EventHandler ContinueClicked;

        public event EventHandler OpenLogClicked;
        private Exception exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionDialog"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="text">The text to be added after exception.</param>
        public ExceptionDialog(Exception exception, string text)
        {
            InitializeComponent();
            Exception = exception;

            exceptionTextBox.Text += text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionDialog"/> class.
        /// 
        /// Default constructor is required by designer
        /// </summary>
        internal ExceptionDialog()
        {
            InitializeComponent();
        }

        public Exception Exception
        {
            get
            {
                return exception;
            }
            private set
            {
                exception = value;

                exceptionTextBox.Text = GetExceptionText(exception);
            }
        }

        public string ExceptionText
        {
            get
            {
                return GetExceptionText(exception);
            }
        }

        public Button ContinueButton { get; private set; }

        private string GetExceptionText(Exception e)
        {
            if (e == null)
            {
                return "";
            }

            var str = exception.ToString();

            if (exception.InnerException != null)
            {
                str += "Inner Exception:\n";
                str += exception.InnerException.ToString();
            }

            if (exception is ReflectionTypeLoadException)
            {
                var reflException = exception as ReflectionTypeLoadException;

                str += "Loader Exceptions:\n";
                str = reflException.LoaderExceptions.Aggregate(str, (current, ex) => current + (ex + "\n"));
            }

            return str;
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            buttonRestart.Enabled = false;
            buttonExit.Enabled = false;

            if (RestartClicked != null)
            {
                RestartClicked(this, null);
            }

            Close();
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            buttonRestart.Enabled = false;
            buttonExit.Enabled = false;

            if (ExitClicked != null)
            {
                ExitClicked(this, null);
            }

            Close();
        }

        private void buttonCopyTextToClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(exceptionTextBox.Text, true);
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if (ContinueClicked != null)
            {
                ContinueClicked(this, null);
            }
        }

        private void buttonOpenLog_Click(object sender, EventArgs e)
        {
            if (OpenLogClicked != null)
            {
                OpenLogClicked(this, null);
            }
        }
    }
}