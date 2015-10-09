using System;

namespace DelftTools.Controls
{
    /// <summary>
    /// IReusable views are reused. When a view should be opened for an object supported by the view and the view
    /// is not locked the view will be reused. 
    /// When implementing this interface the view must expect data will be set after construction to render new objects.
    /// </summary>
    public interface IReusableView : IView
    {
        /// <summary>
        /// Required. Event should be fired when lock changes. Allows to update the UI (image for the tab)
        /// </summary>
        event EventHandler LockedChanged;

        /// <summary>
        /// Required. Determines whether the view is tight to the data it renders.
        /// </summary>
        bool Locked { get; set; }
    }
}