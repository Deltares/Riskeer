// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Components.Gis.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="WmtsMapData"/> for data used in the map.
    /// </summary>
    public static class RingtoetsBackgroundMapDataFactory
    {
        /// <summary>
        /// Creates <see cref="WmtsMapData"/> from a <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="backgroundData">The <see cref="BackgroundData"/> to create
        /// the <see cref="WmtsMapData"/> for.</param>
        /// <returns>The created <see cref="WmtsMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundData"/>
        /// is <c>null</c>.</exception>
        public static WmtsMapData CreateBackgroundMapData(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            if (backgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                return CreateWmtsBackgroundMapData(backgroundData);
            }

            return null;
        }

        public static ImageBasedMapData CreateImageBasedBackgroundMapData(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            if (backgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                return CreateWmtsBackgroundMapData(backgroundData);
            }

            return new WellKnownTileSourceMapData((WellKnownTileSource)(Convert.ToInt32(backgroundData.Parameters[BackgroundDataIdentifiers.WellKnownTileSource])));
        }

        /// <summary>
        /// Updates an existing <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="mapData">The <see cref="WmtsMapData"/> to update.</param>
        /// <param name="backgroundData">The <see cref="BackgroundData"/> used
        /// to update the map data.</param>
        /// <returns>The updated <see cref="WmtsMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void UpdateBackgroundMapData(WmtsMapData mapData, BackgroundData backgroundData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            if (backgroundData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                WmtsMapData newMapData = CreateWmtsBackgroundMapData(backgroundData);

                if (newMapData.IsConfigured)
                {
                    mapData.Configure(newMapData.SourceCapabilitiesUrl,
                                      newMapData.SelectedCapabilityIdentifier,
                                      newMapData.PreferredFormat);
                }
                else
                {
                    mapData.RemoveConfiguration();
                }

                mapData.Name = newMapData.Name;
                mapData.Transparency = newMapData.Transparency;
                mapData.IsVisible = newMapData.IsVisible;
            }
        }

        private static WmtsMapData CreateWmtsBackgroundMapData(BackgroundData backgroundData)
        {
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            mapData.Name = backgroundData.Name;

            if (backgroundData.IsConfigured)
            {
                mapData.Configure(backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl],
                                  backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier],
                                  backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat]);
            }

            mapData.IsVisible = backgroundData.IsVisible;
            mapData.Transparency = backgroundData.Transparency;

            return mapData;
        }
    }
}