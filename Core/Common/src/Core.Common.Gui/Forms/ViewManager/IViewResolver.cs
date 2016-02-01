// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Collections.Generic;

using Core.Common.Controls.Views;
using Core.Common.Gui.Plugin;

namespace Core.Common.Gui.Forms.ViewManager
{
    public interface IViewResolver
    {
        /// <summary>
        /// Default view types registered for data object types.
        /// </summary>
        /// <remarks>The keys in this dictionary are the object types and the values the 
        /// corresponding view object types.</remarks>
        IDictionary<Type, Type> DefaultViewTypes { get; }

        /// <summary>
        /// List of view info objects used for resolving views
        /// </summary>
        IList<ViewInfo> ViewInfos { get; }

        /// <summary>
        /// Opens a view for specified data. Using viewprovider to resolve the correct view.
        /// </summary>
        /// <param name="data">Data to open a view for</param>
        /// <param name="alwaysShowDialog">Always present the user with a dialog to choose from</param>
        bool OpenViewForData(object data, bool alwaysShowDialog = false);

        /// <summary>
        /// Creates a view for the <paramref name="data"/> 
        /// </summary>
        /// <param name="data">The data to create a view for</param>
        /// <param name="selectViewInfo">Function to filter the view infos to use</param>
        /// <returns>A view for data</returns>
        IView CreateViewForData(object data, Func<ViewInfo, bool> selectViewInfo = null);

        /// <summary>
        /// Check if a view can be created for the <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data to check for</param>
        /// <returns></returns>
        bool CanOpenViewFor(object data);

        /// <summary>
        /// Returns all currently opened views for the same data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        IList<IView> GetViewsForData(object data);

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

        string GetViewName(IView view);
    }
}