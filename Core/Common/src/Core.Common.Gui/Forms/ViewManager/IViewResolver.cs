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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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
    /// <summary>
    /// Interface for an object capable of finding a view for a given data object.
    /// </summary>
    public interface IViewResolver
    {
        /// <summary>
        /// Default view types registered for data object types, that can be used to automatically
        /// resolve to a particular view when multiple candidates are available.
        /// </summary>
        /// <remarks>The keys in this dictionary are the object types and the values the 
        /// corresponding view types.</remarks>
        IDictionary<Type, Type> DefaultViewTypes { get; }

        /// <summary>
        /// Opens a view for specified data.
        /// </summary>
        /// <param name="data">Data to open a view for.</param>
        /// <param name="alwaysShowDialog">Always present the user with a dialog to choose
        /// the view that has to be opened.</param>
        bool OpenViewForData(object data, bool alwaysShowDialog = false);

        /// <summary>
        /// Closes all views for <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data object to close all views for.</param>
        void CloseAllViewsFor(object data);

        /// <summary>
        /// Gets the view info objects for the <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Data used for finding matches.</param>
        /// <returns>The matching view info object.</returns>
        IEnumerable<ViewInfo> GetViewInfosFor(object data);

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The name of the view.</returns>
        string GetViewName(IView view);
    }
}