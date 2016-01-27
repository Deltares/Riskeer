using System;

using Core.Common.Controls.Views;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for controller that controls Document Views in the application.
    /// </summary>
    public interface IDocumentViewController
    {
        /// <summary>
        /// Fired when the active view in the document pane changes.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        /// <summary>
        /// Gets the currently active document <see cref="IView"/>.
        /// </summary>
        IView ActiveView { get; }

        /// <summary>
        ///  Gets all document views currently opened in the gui.
        /// </summary>
        IViewList DocumentViews { get; }

        /// <summary>
        /// Resolves document views
        /// </summary>
        IViewResolver DocumentViewsResolver { get; }

        /// <summary>
        /// Suspends view removal on item delete. Useful to avoid unnecessary checks (faster item removal).
        /// </summary>
        bool IsViewRemoveOnItemDeleteSuspended { get; set; }

        /// <summary>
        /// Update the tool tip for every view currently open. Reasons for doing so 
        /// include the modification of the tree structure which is reflected in a tool tip.
        /// </summary>
        void UpdateToolTips();
    }
}