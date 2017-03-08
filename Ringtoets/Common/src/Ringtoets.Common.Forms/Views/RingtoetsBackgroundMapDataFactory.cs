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
        /// Creates <see cref="WmtsMapData"/> from a <see cref="BackgroundMapData"/>.
        /// </summary>
        /// <param name="backgroundMapData">The <see cref="BackgroundMapData"/> to create
        /// the <see cref="WmtsMapData"/> for.</param>
        /// <returns>The created <see cref="WmtsMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundMapData"/>
        /// is <c>null</c>.</exception>
        public static WmtsMapData CreateBackgroundMapData(BackgroundMapData backgroundMapData)
        {
            if (backgroundMapData == null)
            {
                throw new ArgumentNullException(nameof(backgroundMapData));
            }

            if (backgroundMapData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                return CreateWmtsBackgroundMapData(backgroundMapData);
            }

            return null;
        }

        /// <summary>
        /// Updates an existing <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="mapData">The <see cref="WmtsMapData"/> to update.</param>
        /// <param name="backgroundMapData">The <see cref="BackgroundMapData"/> used
        /// to update the map data.</param>
        /// <returns>The updated <see cref="WmtsMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static void UpdateBackgroundMapData(WmtsMapData mapData, BackgroundMapData backgroundMapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }
            if (backgroundMapData == null)
            {
                throw new ArgumentNullException(nameof(backgroundMapData));
            }

            if (backgroundMapData.BackgroundMapDataType == BackgroundMapDataType.Wmts)
            {
                WmtsMapData newMapData = CreateWmtsBackgroundMapData(backgroundMapData);

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

        private static WmtsMapData CreateWmtsBackgroundMapData(BackgroundMapData backgroundData)
        {
            WmtsMapData mapData = WmtsMapData.CreateUnconnectedMapData();
            mapData.Name = backgroundData.Name;

            if (backgroundData.IsConfigured)
            {
                mapData.Configure(backgroundData.Parameters["SourceCapabilitiesUrl"],
                                  backgroundData.Parameters["SelectedCapabilityIdentifier"],
                                  backgroundData.Parameters["PreferredFormat"]);
            }

            mapData.IsVisible = backgroundData.IsVisible;
            mapData.Transparency = backgroundData.Transparency;

            return mapData;
        }
    }
}