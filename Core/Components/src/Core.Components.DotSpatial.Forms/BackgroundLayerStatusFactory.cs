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
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// Factory for creating <see cref="BackgroundLayerStatus"/>.
    /// </summary>
    public static class BackgroundLayerStatusFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="BackgroundLayerStatus"/> corresponding to 
        /// the type of <paramref name="mapData"/>.
        /// </summary>
        /// <param name="mapData">The type of <see cref="ImageBasedMapData"/> to create a 
        /// <see cref="BackgroundLayerStatus"/> for.</param>
        /// <returns>A new instance of <see cref="BackgroundLayerStatus"/>.</returns>
        /// <exception cref="ArgumentNullException ">Thrown when <paramref name="mapData"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the type of <see cref="ImageBasedMapData"/>
        /// is not supported.</exception>
        internal static BackgroundLayerStatus CreateBackgroundLayerStatus(ImageBasedMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            if (mapData is WmtsMapData)
            {
                return new WmtsBackgroundLayerStatus();
            }

            if (mapData is WellKnownTileSourceMapData)
            {
                return new WellKnownBackgroundLayerStatus();
            }

            throw new NotSupportedException("Unsupported type of mapData");
        }
    }
}