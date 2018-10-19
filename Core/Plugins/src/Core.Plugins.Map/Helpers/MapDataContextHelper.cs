// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.Gis.Data;
using Core.Plugins.Map.PresentationObjects;

namespace Core.Plugins.Map.Helpers
{
    /// <summary>
    /// Helper class for <see cref="MapDataContext"/>.
    /// </summary>
    public static class MapDataContextHelper
    {
        /// <summary>
        /// Gets all the parents of the given <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to get the parents for.</param>
        /// <returns>A collection of <see cref="MapDataCollection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MapDataCollection> GetParentsFromContext(MapDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var parents = new List<MapDataCollection>();

            if (context.ParentMapData != null)
            {
                parents.Add((MapDataCollection)context.ParentMapData.WrappedData);
                parents.AddRange(GetParentsFromContext(context.ParentMapData));
            }

            return parents;
        }
    }
}
