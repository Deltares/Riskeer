using System.Windows.Forms;
using Core.Common.Forms.Dialogs;
using Core.Common.Gui.Properties;

namespace Core.Common.Gui.Forms.MessageWindow
{
    /// <summary>
    /// Class for showing a message dialog.
    /// </summary>
    public partial class MessageWindowDialog : DialogBase
    {
        /// <summary>
        /// Constructs a new <see cref="MessageWindowDialog"/>.
        /// </summary>
        /// <param name="owner">The owner of the dialog.</param>
        /// <param name="text">The text to show in the dialog.</param>
        public MessageWindowDialog(IWin32Window owner, string text) : base(owner, Resources.application_import_blue1, 200, 150)
        {
            InitializeComponent();

            textBox.Text = text;

            Select(); // Select the form; otherwise the text in the textbox is selected by default
        }

        protected override Button GetCancelButton()
        {
            return buttonHidden;
        }
    }
}