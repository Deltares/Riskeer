using System;
using System.Collections.Generic;
using Core.Common.Controls;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;

namespace Core.Common.Gui
{
    [Flags]
    public enum ViewLocation
    {
        Document = 0x0,
        Left = 0x1,
        Right = 0x2,
        Top = 0x4,
        Bottom = 0x8,
        Floating = 0x16
    };

    /// <summary>
    /// Manages currently displayed views
    /// </summary>
    public interface IViewList : IEventedList<IView>, IDisposable
    {
        /// <summary>
        /// Fired before active view has been changed.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        /// <summary>
        /// Fired after active view has been changed.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        /// <summary>
        /// Fired when a childview is added to a view
        /// </summary>
        event NotifyCollectionChangedEventHandler ChildViewChanged;

        /// <summary>
        /// HACK: Hack to disable activation temporarily
        /// </summary>
        bool IgnoreActivation { get; set; }

        /// <summary>
        /// Gets or sets active view, when view is active - its window is activated.
        /// </summary>
        IView ActiveView { get; set; }

        /// <summary>
        /// Returns all views. Including views inside composite views
        /// </summary>
        IEnumerable<IView> AllViews { get; }

        /// <summary>
        /// Adds a view to the UI. 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewLocation"></param>
        void Add(IView view, ViewLocation viewLocation);

        /// <summary>
        /// Sets the tooltip of the view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tooltip"></param>
        void SetTooltip(IView view, string tooltip);

        /// <summary>
        /// Updates the name of the view
        /// </summary>
        /// <param name="view"></param>
        void UpdateViewName(IView view);

        /// <summary>
        /// Returns views of type T that are (part of) the active view.
        /// </summary>
        /// <typeparam name="T">Type of view to look for</typeparam>
        /// <returns>Views of type <typeparamref name="T"/></returns>
        IEnumerable<T> GetActiveViews<T>() where T : class, IView;

        /// <summary>
        /// Returns views of type T that are (part of) the supplied <paramref name="views"/>
        /// </summary>
        /// <typeparam name="T">Type of view to look for</typeparam>
        /// <param name="views">Views to search</param>
        /// <returns>Views of type <typeparamref name="T"/></returns>
        IEnumerable<T> FindViewsRecursive<T>(IEnumerable<IView> views) where T : class, IView;

        /// <summary>
        /// Overloaded Clear, removes all views except <paramref name="viewToKeep"/>
        /// </summary>
        /// <param name="viewToKeep"></param>
        void Clear(IView viewToKeep);
    }
}