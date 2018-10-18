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
using System.Linq;
using Core.Common.Gui.PropertyBag;
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
        private readonly IEnumerable<MapDataCollection> parents;

        /// <summary>
        /// Creates a new instance of <see cref="MapDataCollectionProperties"/>.
        /// </summary>
        /// <param name="mapDataCollection">The <see cref="MapDataCollection"/> to show the properties for.</param>
        /// <param name="parents">A collection containing all parent <see cref="MapDataCollection"/> of
        /// the <paramref name="mapDataCollection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MapDataCollectionProperties(MapDataCollection mapDataCollection, IEnumerable<MapDataCollection> parents)
        {
            if (mapDataCollection == null)
            {
                throw new ArgumentNullException(nameof(mapDataCollection));
            }

            if (parents == null)
            {
                throw new ArgumentNullException(nameof(parents));
            }

            this.parents = parents;

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
        public bool IsVisible
        {
            get
            {
                return data.IsVisible;
            }
            set
            {
                Dictionary<MapData, bool> childStates = GetChildStates(data);

                data.IsVisible = value;
                data.NotifyObservers();

                NotifyChildren(childStates);

                NotifyParents();
            }
        }

        private void NotifyParents()
        {
            foreach (MapDataCollection parent in parents)
            {
                parent.NotifyObservers();
            }
        }

        private static void NotifyChildren(Dictionary<MapData, bool> childStates)
        {
            foreach (KeyValuePair<MapData, bool> child in childStates)
            {
                if (child.Key.IsVisible != child.Value)
                {
                    child.Key.NotifyObservers();
                }
            }
        }
        private static Dictionary<MapData, bool> GetChildStates(MapDataCollection collection)
        {
            return GetChildren(collection).ToDictionary(child => child, child => child.IsVisible);
        }

        private static IEnumerable<MapData> GetChildren(MapDataCollection collection)
        {
            var children = new List<MapData>();

            foreach (MapData child in collection.Collection)
            {
                var childCollection = child as MapDataCollection;
                if (childCollection != null)
                {
                    children.AddRange(GetChildren(childCollection));
                }

                children.Add(child);
            }

            return children;
        }
    }
}