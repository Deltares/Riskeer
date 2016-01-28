using System.Windows.Forms;
using Core.Common.Gui.Forms.MessageWindow;

namespace Core.Common.Gui.Forms.MainWindow
{
    /// <summary>
    /// Main window of a shell application
    /// </summary>
    public interface IMainWindow : IWin32Window
    {
        /// <summary>
        /// Property grid tool window. See also <seealso cref="IGui.ToolWindowViews"/>.
        /// </summary>
        IPropertyGrid PropertyGrid { get; }

        /// <summary>
        /// Tool window containing log messages. See also <seealso cref="IGui.ToolWindowViews"/>.
        /// </summary>
        IMessageWindow MessageWindow { get; }

        //TODO: This is  inconsistent with the form title which is called .Text
        /// <summary>
        /// The window title
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Is the window visible?
        /// </summary>
        bool Visible { get; }

        string StatusBarMessage { get; set; }

        void Show();

        /// <summary>
        /// Closes main window
        /// </summary>
        void Close();

        void ValidateItems();

        void SetWaitCursorOn();

        void SetWaitCursorOff();
    }
}