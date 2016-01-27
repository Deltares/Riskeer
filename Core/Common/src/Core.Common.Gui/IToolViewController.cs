using Core.Common.Controls.Views;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for controller that controls Tool Views in the application.
    /// </summary>
    public interface IToolViewController
    {
        /// <summary>
        /// Gets view manager used to handle tool windows.
        /// </summary>
        IViewList ToolWindowViews { get; }

        /// <summary>
        /// Open the tool view and make it visible in the interface.
        /// </summary>
        /// <param name="toolView">The tool view to open.</param>
        void OpenToolView(IView toolView);

        /// <summary>
        /// Close the tool view removing it from the interface.
        /// </summary>
        /// <param name="toolView">The tool view to close.</param>
        void CloseToolView(IView toolView);
    }
}