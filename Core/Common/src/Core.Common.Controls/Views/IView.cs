using System;

namespace Core.Common.Controls.Views
{
    /// <summary>
    /// General interface for graphical user interface views used in applications
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Gets or sets data shown by this view. Usually it is any object in the system which can be shown by some IView derived class.
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Gets or sets the *caption/title* for the view
        /// </summary>
        string Text { get; set; }

        ViewInfo ViewInfo { get; set; }
    }
}