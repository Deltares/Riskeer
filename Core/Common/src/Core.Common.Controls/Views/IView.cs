using System;

namespace Core.Common.Controls.Views
{
    /// <summary>
    /// Interface for graphical user interface views.
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Gets or sets the data shown by the <see cref="IView"/>.
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// Gets or sets the caption/title of the <see cref="IView"/>.
        /// </summary>
        string Text { get; set; }
    }
}