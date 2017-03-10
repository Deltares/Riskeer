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

namespace Ringtoets.Common.Forms.TypeConverters
{
    /// <summary>
    /// Converter to convert <see cref="BackgroundData"/> to <see cref="ImageBasedMapData"/>
    /// and back.
    /// </summary>
    public static class BackgroundDataConverter
    {
        /// <summary>
        /// Converts <see cref="ImageBasedMapData"/> to <see cref="BackgroundData"/>.
        /// </summary>
        /// <param name="mapData">The <see cref="ImageBasedMapData"/> to convert.</param>
        /// <returns>The converted <see cref="BackgroundData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/>
        /// is <c>null</c>.</exception>
        public static BackgroundData ConvertTo(ImageBasedMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            var backgroundData = new BackgroundData
            {
                Name = mapData.Name,
                IsConfigured = mapData.IsConfigured,
                IsVisible = mapData.IsVisible,
                Transparency = mapData.Transparency
            };

            var wmtsMapData = mapData as WmtsMapData;
            if (wmtsMapData != null)
            {
                ConvertWmtsMapDataParameters(wmtsMapData, backgroundData);
            }
            var wellKnownMapData = mapData as WellKnownTileSourceMapData;
            if (wellKnownMapData != null)
            {
                ConvertWellKnownMapDataParameters(wellKnownMapData, backgroundData);
            }

            return backgroundData;
        }

        /// <summary>
        /// Converts <see cref="BackgroundData"/> to <see cref="ImageBasedMapData"/>.
        /// </summary>
        /// <param name="backgroundData">The <see cref="BackgroundData"/> to convert.</param>
        /// <returns>The converted <see cref="ImageBasedMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundData"/>
        /// is <c>null</c>.</exception>
        public static ImageBasedMapData ConvertFrom(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            ImageBasedMapData mapData;

            switch (backgroundData.BackgroundMapDataType)
            {
                case BackgroundMapDataType.Wmts:
                    mapData = CreateWmtsMapData(backgroundData);
                    break;
                case BackgroundMapDataType.WellKnown:
                    mapData = CreateWellKnownMapdata(backgroundData);
                    break;
                default:
                    throw new NotSupportedException();
            }

            mapData.Name = backgroundData.Name;
            mapData.IsVisible = backgroundData.IsVisible;
            mapData.Transparency = backgroundData.Transparency;

            return mapData;
        }

        private static void ConvertWellKnownMapDataParameters(WellKnownTileSourceMapData mapData, BackgroundData backgroundData)
        {
            backgroundData.BackgroundMapDataType = BackgroundMapDataType.WellKnown;
            backgroundData.Parameters[BackgroundDataIdentifiers.WellKnownTileSource] = ((int) mapData.TileSource).ToString();
        }

        private static void ConvertWmtsMapDataParameters(WmtsMapData mapData, BackgroundData backgroundData)
        {
            backgroundData.BackgroundMapDataType = BackgroundMapDataType.Wmts;

            if (backgroundData.IsConfigured)
            {
                backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl] = mapData.SourceCapabilitiesUrl;
                backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier] = mapData.SelectedCapabilityIdentifier;
                backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat] = mapData.PreferredFormat;
            }
        }

        private static WmtsMapData CreateWmtsMapData(BackgroundData backgroundData)
        {
            WmtsMapData wmtsMapData = WmtsMapData.CreateUnconnectedMapData();

            if (backgroundData.IsConfigured)
            {
                wmtsMapData.Configure(backgroundData.Parameters[BackgroundDataIdentifiers.SourceCapabilitiesUrl],
                                      backgroundData.Parameters[BackgroundDataIdentifiers.SelectedCapabilityIdentifier],
                                      backgroundData.Parameters[BackgroundDataIdentifiers.PreferredFormat]);
            }
            return wmtsMapData;
        }

        private static WellKnownTileSourceMapData CreateWellKnownMapdata(BackgroundData backgroundData)
        {
            var tileSource = (WellKnownTileSource)Convert.ToInt32(backgroundData.Parameters[BackgroundDataIdentifiers.WellKnownTileSource]);
            return new WellKnownTileSourceMapData(tileSource);
        }
    }
}