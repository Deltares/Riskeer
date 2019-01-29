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
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.Forms;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;

namespace Core.Components.DotSpatial.Layer
{
    /// <summary>
    /// Class responsible for creating <see cref="BruTileLayer"/> instances for a given
    /// map data.
    /// </summary>
    public static class ImageBasedMapDataLayerFactory
    {
        /// <summary>
        /// Creates a new <see cref="BruTileLayer"/> based on the <paramref name="backgroundMapData"/>.
        /// </summary>
        /// <param name="backgroundMapData">The <see cref="ImageBasedMapData"/> to create a 
        /// <see cref="BruTileLayer"/> for.</param>
        /// <returns>A new <see cref="BruTileLayer"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundMapData"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when a configuration can't
        /// be created for the type of <paramref name="backgroundMapData"/>.</exception>
        /// <exception cref="ConfigurationInitializationException">Thrown when the configuration
        /// can't connect with the tile service or creating the file cache failed.</exception>
        public static BruTileLayer Create(ImageBasedMapData backgroundMapData)
        {
            if (backgroundMapData == null)
            {
                throw new ArgumentNullException(nameof(backgroundMapData));
            }

            IConfiguration configuration = BrutileConfigurationFactory.CreateInitializedConfiguration(backgroundMapData);

            return new BruTileLayer(configuration)
            {
                IsVisible = backgroundMapData.IsVisible,
                Transparency = Convert.ToSingle(backgroundMapData.Transparency)
            };
        }
    }
}