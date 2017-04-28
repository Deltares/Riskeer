// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.Plugin;

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Interface for an object capable of:
    /// <list type="bullet">
    /// <item>
    /// <description>adding document views to a <see cref="IViewHost"/>;</description>
    /// </item>
    /// <item>
    /// <description>finding document views in a <see cref="IViewHost"/>;</description>
    /// </item>
    /// <item>
    /// <description>removing document views from a <see cref="IViewHost"/>.</description>
    /// </item>
    /// </list>
    /// </summary>
    public interface IDocumentViewController
    {
        /// <summary>
        /// Gets the default view types registered for data object types, that can be used to
        /// automatically resolve a particular view when multiple candidates are available.
        /// </summary>
        /// <remarks>The keys in this dictionary are the object types and the values the 
        /// corresponding view types.</remarks>
        IDictionary<Type, Type> DefaultViewTypes { get; }

        /// <summary>
        /// Opens a view for <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data to open a view for.</param>
        /// <param name="alwaysShowDialog">Always present the user with a dialog to choose
        /// the view that has to be opened.</param>
        bool OpenViewForData(object data, bool alwaysShowDialog = false);

        /// <summary>
        /// Closes all views for <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data object to close all views for.</param>
        void CloseAllViewsFor(object data);

        /// <summary>
        /// Gets the view info objects for <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data to obtain the view info objects for.</param>
        /// <returns>The matching view info objects.</returns>
        IEnumerable<ViewInfo> GetViewInfosFor(object data);
    }
}