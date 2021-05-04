﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Components.Gis.Data;

namespace Core.Gui.PresentationObjects.Map
{
    /// <summary>
    /// Presentation object for <see cref="MapPolygonData"/>.
    /// </summary>
    public class MapPolygonDataContext : FeatureBasedMapDataContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapPolygonDataContext"/>.
        /// </summary>
        /// <param name="wrappedData">The <see cref="MapPolygonData"/> to wrap.</param>
        /// <param name="parentMapData">The parent <see cref="MapDataCollectionContext"/> 
        /// the <paramref name="wrappedData"/> belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MapPolygonDataContext(MapPolygonData wrappedData, MapDataCollectionContext parentMapData)
            : base(wrappedData, parentMapData) {}
    }
}