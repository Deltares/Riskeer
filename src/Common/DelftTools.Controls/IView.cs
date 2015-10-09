using System;
using System.Collections.Generic;
using System.Drawing;
using DelftTools.Utils.Collections.Generic;

namespace DelftTools.Controls
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

    public interface ICompositeView : IView
    {
        IEventedList<IView> ChildViews { get; }

        bool HandlesChildViews { get; }

        void ActivateChildView(IView childView);
    }

    /// <summary>
    /// View that can be searched by using the SearchDialog
    /// TODO : change to ISearchable and move to a different place
    /// </summary>
    public interface ISearchableView : IView
    {
        /// <summary>
        /// Returns the objects that where found using the text.
        /// This will be called from a separate thread.
        /// </summary>
        IEnumerable<Tuple<string, object>> SearchItemsByText(string text, bool caseSensitive, Func<bool> isSearchCancelled, Action<int> setProgressPercentage);
    }

    /// <summary>
    /// Marker interface to indicate this view is not a principal view of its current data object and therefore 
    /// should not be returned when asking for existing views for a data object. It will however be closed when 
    /// the data is removed.
    /// </summary>
    public interface IAdditionalView : IView {}
}