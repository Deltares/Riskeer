using System.Windows.Forms;

namespace Core.Common.Controls.Dialogs
{
    /// <summary>
    /// Base class for dialogs which should be derived in order to get a consistent look and feel.
    /// The base class ensures:
    /// <list type="bullet">
    /// <item>
    /// <description>the dialog is shown in the center of the parent form (<see cref="Form.StartPosition"/> is set to <see cref="FormStartPosition.CenterParent"/>);</description>
    /// </item>
    /// <item>
    /// <description>no task bar icon is shown (<see cref="Form.ShowInTaskbar"/> is set to <c>false</c>);</description>
    /// </item>
    /// <item>
    /// <description>no minimize control box item is shown (<see cref="Form.MinimizeBox"/> is set to <c>false</c>);</description>
    /// </item>
    /// <item>
    /// <description>no maximize control box item is shown (<see cref="Form.MaximizeBox"/> is set to <c>false</c>);</description>
    /// </item>
    /// <item>
    /// <description>the parent form is automatically obtained using <see cref="ModalHelper"/>.</description>
    /// </item>
    /// </list>
    /// </summary>
    public partial class DialogBase : Form
    {
        /// <summary>
        /// Constructs a new <see cref="DialogBase"/>.
        /// </summary>
        protected DialogBase()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This method provides a new implementation of <see cref="Form.ShowDialog()"/>.
        /// In this new implementation the dialog is shown with an <see cref="IWin32Window"/> owner,
        /// which is automatically derived via <see cref="ModalHelper"/>.
        /// </summary>
        /// <returns>A <see cref="DialogResult"/>.</returns>
        public new DialogResult ShowDialog()
        {
            return base.ShowDialog(ModalHelper.MainWindow);
        }
    }
}
