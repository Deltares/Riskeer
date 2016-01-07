using Core.Common.Controls.Views;
using Core.Common.Gui;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// Interface for the <see cref="OxyPlotGuiPlugin"/>. Describes methods which require knowledge of
    /// the <see cref="RingtoetsGui"/>.
    /// </summary>
    public interface IOxyPlotGuiPlugin {
        
        /// <summary>
        /// Checks whether a tool window of type <typeparamref name="T"/> is open.
        /// </summary>
        /// <typeparam name="T">The type of tool window to check for.</typeparam>
        /// <returns><c>true</c> if a tool window of type <typeparamref name="T"/> is open, <c>false</c> otherwise.</returns>
        bool IsToolWindowOpen<T>();

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