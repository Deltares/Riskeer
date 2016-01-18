using Core.Common.Controls.Views;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// This interface describes interaction with the document <see cref="IView"/>s.
    /// </summary>
    public interface IDocumentViewController {

        /// <summary>
        /// Retrieves the currently active document view.
        /// </summary>
        IView ActiveView { get; }
    }
}