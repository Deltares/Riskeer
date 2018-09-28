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
using Core.Components.Gis.Geometries;

namespace Core.Components.Gis.Features
{
    /// <summary>
    /// Features containing an <see cref="IEnumerable{T}"/> of <see cref="MapGeometry"/>
    /// defined in the RD-new coordinate system.
    /// </summary>
    public class MapFeature
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapFeature"/>.
        /// </summary>
        /// <param name="mapGeometries">An <see cref="IEnumerable{T}"/> of <see cref="MapGeometry"/>
        /// defined in the RD-new coordinate system.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapGeometries"/> is <c>null</c>.</exception>
        public MapFeature(IEnumerable<MapGeometry> mapGeometries)
        {
            if (mapGeometries == null)
            {
                throw new ArgumentNullException(nameof(mapGeometries), @"MapFeature cannot be created without map geometries.");
            }

            MapGeometries = mapGeometries;
            MetaData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the geometries defined in the RD-new coordinate system associated with this feature.
        /// </summary>
        public IEnumerable<MapGeometry> MapGeometries { get; }

        /// <summary>
        /// Gets the meta data associated with the feature.
        /// </summary>
        public IDictionary<string, object> MetaData { get; }
    }
}