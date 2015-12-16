using System;
using System.Drawing;

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
        /// TODO: change it to Name
        /// 
        /// IGui implementation sets this.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Sets or gets image set on the title of the view.
        /// </summary>
        Image Image { get; set; }

        /// <summary>
        /// True when view is visible.
        /// </summary>
        bool Visible { get; }

        ViewInfo ViewInfo { get; set; }

        /// <summary>
        /// Makes object visible in the view if possible
        /// </summary>
        /// <param name="item"></param>
        void EnsureVisible(object item);
    }
}