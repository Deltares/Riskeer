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
using System.Collections;
using System.Collections.Generic;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface describing the object that hosts all the leaded graphical user interface
    /// plugins of the application.
    /// </summary>
    public interface IGuiPluginsHost
    {
        /// <summary>
        /// List of plugins.
        /// </summary>
        IList<GuiPlugin> Plugins { get; }

        /// <summary>
        /// Returns GuiPlugin for a given type.
        /// TODO: a bit too implicit method, to be removed.
        /// </summary>
        /// <param name="type">Any type loaded from plugin.</param>
        /// <returns>Plugin gui associated with a given type</returns>
        GuiPlugin GetPluginGuiForType(Type type);

        /// <summary>
        /// Queries the plugins to get all data with view definitions recursively given a
        /// piece of hierarchical data.
        /// </summary>
        /// <param name="rootDataObject">The root data object.</param>
        /// <returns>An enumeration of all (child)data that have view definitions declared.</returns>
        IEnumerable GetAllDataWithViewDefinitionsRecursively(object rootDataObject);
    }
}