using Core.Common.Gui.Forms.MainWindow;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface that declare member that allow for the controlling the main window of
    /// the application.
    /// </summary>
    public interface IMainWindowController
    {
        /// <summary>
        /// Gets main window of the graphical user interface.
        /// </summary>
        IMainWindow MainWindow { get; }

        /// <summary>
        /// Fully refreshes the user interface.
        /// </summary>
        void RefreshGui();

        /// <summary>
        /// Updates the title of the main window.
        /// </summary>
        void UpdateTitle();
    }
}