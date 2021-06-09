// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using Core.Components.Gis.Data;
using Core.Gui.Helpers;
using Core.Gui.PresentationObjects.Map;
using Core.Gui.PropertyClasses.Map;

namespace Core.Gui.Plugin.Map
{
    /// <summary>
    /// Factory for creating <see cref="PropertyInfo"/> objects for <see cref="MapData"/>.
    /// </summary>
    public static class MapPropertyInfoFactory
    {
        /// <summary>
        /// Creates the <see cref="PropertyInfo"/> objects.
        /// </summary>
        /// <returns>The created <see cref="PropertyInfo"/> objects.</returns>
        public static IEnumerable<PropertyInfo> Create()
        {
            yield return new PropertyInfo<MapDataCollectionContext, MapDataCollectionProperties>
            {
                CreateInstance = context => new MapDataCollectionProperties(
                    (MapDataCollection) context.WrappedData)
            };
            yield return new PropertyInfo<MapPointDataContext, MapPointDataProperties>
            {
                CreateInstance = context => new MapPointDataProperties(
                    (MapPointData) context.WrappedData,
                    MapDataContextHelper.GetParentsFromContext(context))
            };
            yield return new PropertyInfo<MapLineDataContext, MapLineDataProperties>
            {
                CreateInstance = context => new MapLineDataProperties(
                    (MapLineData) context.WrappedData,
                    MapDataContextHelper.GetParentsFromContext(context))
            };
            yield return new PropertyInfo<MapPolygonDataContext, MapPolygonDataProperties>
            {
                CreateInstance = context => new MapPolygonDataProperties(
                    (MapPolygonData) context.WrappedData,
                    MapDataContextHelper.GetParentsFromContext(context))
            };
        }
    }
}