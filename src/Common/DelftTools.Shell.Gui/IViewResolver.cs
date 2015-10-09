using System;
using System.Collections.Generic;
using DelftTools.Controls;

namespace DelftTools.Shell.Gui
{
    public interface IViewResolver
    {
        /// <summary>
        /// Default view types registered for data object types.
        /// 
        /// <example>DefaultViewTypes[objectType] = viewType;</example>
        /// </summary>
        IDictionary<Type, Type> DefaultViewTypes { get; }

        /// <summary>
        /// List of view info objects used for resolving views
        /// </summary>
        IList<ViewInfo> ViewInfos { get; }

        /// <summary>
        /// Opens a view for specified data. Using viewprovider to resolve the correct view.
        /// </summary>
        /// <param name="data">Data to open a view for</param>
        /// <param name="viewType">ViewType to use (if null then default will be used)</param>
        /// <param name="alwaysShowDialog">Always present the user with a dialog to choose from</param>
        bool OpenViewForData(object data, Type viewType = null, bool alwaysShowDialog = false);

        /// <summary>
        /// Creates a view for the <paramref name="data"/> 
        /// </summary>
        /// <param name="data">The data to create a view for</param>
        /// <param name="selectViewInfo">Function to filter the view infos to use</param>
        /// <returns>A view for data</returns>
        IView CreateViewForData(object data, Func<ViewInfo, bool> selectViewInfo = null);

        /// <summary>
        /// Check if a view can be created for the <paramref name="data"/> and <paramref name="viewType"/>
        /// </summary>
        /// <param name="data">The data to check for</param>
        /// <param name="viewType">ViewType to use (if null then default will be used)</param>
        /// <returns></returns>
        bool CanOpenViewFor(object data, Type viewType = null);

        /// <summary>
        /// Returns all currently opened views for the same data.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="includeChildViews">Include child views of a composite view in the results</param>
        /// <returns></returns>
        IList<IView> GetViewsForData(object data, bool includeChildViews = false);

        /// <summary>
        /// Closes all views for <paramref name="data"/>.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        void CloseAllViewsFor(object data);

        /// <summary>
        /// Gives the default viewtype for the given data object.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        Type GetDefaultViewType(object dataObject);

        /// <summary>
        /// Gets the view info objects for the <paramref name="data"/>
        /// </summary>
        /// <param name="data">Data used for searching the view infos</param>
        /// <param name="viewType">The viewType of the view info</param>
        /// <returns>The matching view infos for data and view type</returns>
        IEnumerable<ViewInfo> GetViewInfosFor(object data, Type viewType = null);
    }
}