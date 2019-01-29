// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.ComponentModel;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="MapDataCollection"/> for properties panel.
    /// </summary>
    public class MapDataCollectionProperties : ObjectProperties<MapDataCollection>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapDataCollectionProperties"/>.
        /// </summary>
        /// <param name="mapDataCollection">The <see cref="MapDataCollection"/> to show the properties for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapDataCollection"/>
        /// is <c>null</c>.</exception>
        public MapDataCollectionProperties(MapDataCollection mapDataCollection)
        {
            if (mapDataCollection == null)
            {
                throw new ArgumentNullException(nameof(mapDataCollection));
            }

            Data = mapDataCollection;
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MapDataCollection))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapDataCollection_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_MapDataCollection))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.MapData_IsVisible_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.MapDataCollection_IsVisible_Description))]
        [TypeConverter(typeof(EnumTypeConverter))]
        public MapDataCollectionVisibility Visibility
        {
            get
            {
                return data.GetVisibility();
            }
        }
    }
}