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
using System.ComponentModel;
using Core.Components.Gis.Data;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.Utils.TypeConverters
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
            
            var backgroundData = new BackgroundData(CreateBackgroundDataConfiguration(mapData))
            {
                Name = mapData.Name,
                IsVisible = mapData.IsVisible,
                Transparency = mapData.Transparency
            };

            return backgroundData;
        }

        private static IBackgroundDataConfiguration CreateBackgroundDataConfiguration(ImageBasedMapData mapData)
        {
            var wmtsMapData = mapData as WmtsMapData;
            if (wmtsMapData != null)
            {
                return CreateWmtsBackgroundDataConfiguration(wmtsMapData);
            }
            var wellKnownMapData = mapData as WellKnownTileSourceMapData;
            if (wellKnownMapData != null)
            {
                return CreateWellKnownBackgroundDataConfiguration(wellKnownMapData);
            }
            throw new InvalidOperationException($"Can't create a background data configuration for {mapData.GetType()}.");
        }

        /// <summary>
        /// Converts <see cref="BackgroundData"/> to <see cref="ImageBasedMapData"/>.
        /// </summary>
        /// <param name="backgroundData">The <see cref="BackgroundData"/> to convert.</param>
        /// <returns>The converted <see cref="ImageBasedMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="backgroundData"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when
        /// <see cref="WellKnownBackgroundDataConfiguration.WellKnownTileSource"/> contains an 
        /// invalid value for <see cref="WellKnownTileSource"/>.</exception>
        public static ImageBasedMapData ConvertFrom(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            ImageBasedMapData mapData = null;

            var wmtsBackgroundDataConfiguration = backgroundData.Configuration as WmtsBackgroundDataConfiguration;
            if (wmtsBackgroundDataConfiguration != null)
            {
                mapData = CreateWmtsMapData(wmtsBackgroundDataConfiguration);
            }

            var wellKnownBackgroundDataConfiguration = backgroundData.Configuration as WellKnownBackgroundDataConfiguration;
            if (wellKnownBackgroundDataConfiguration != null)
            {
                mapData = CreateWellKnownMapdata(wellKnownBackgroundDataConfiguration);
            }

            mapData.Name = backgroundData.Name;
            mapData.IsVisible = backgroundData.IsVisible;
            mapData.Transparency = backgroundData.Transparency;

            return mapData;
        }

        private static WellKnownBackgroundDataConfiguration CreateWellKnownBackgroundDataConfiguration(WellKnownTileSourceMapData mapData)
        {
            return new WellKnownBackgroundDataConfiguration(mapData.TileSource);
        }

        private static WmtsBackgroundDataConfiguration CreateWmtsBackgroundDataConfiguration(WmtsMapData mapData)
        {
            return new WmtsBackgroundDataConfiguration(mapData.IsConfigured,
                                                       mapData.SourceCapabilitiesUrl,
                                                       mapData.SelectedCapabilityIdentifier,
                                                       mapData.PreferredFormat);
        }

        private static WmtsMapData CreateWmtsMapData(WmtsBackgroundDataConfiguration backgroundDataConfiguration)
        {
            WmtsMapData wmtsMapData = WmtsMapData.CreateUnconnectedMapData();

            if (backgroundDataConfiguration.IsConfigured)
            {
                wmtsMapData.Configure(backgroundDataConfiguration.SourceCapabilitiesUrl,
                                      backgroundDataConfiguration.SelectedCapabilityIdentifier,
                                      backgroundDataConfiguration.PreferredFormat);
            }
            return wmtsMapData;
        }

        /// <summary>
        /// Creates a <see cref="WellKnownTileSourceMapData"/>, based upon the 
        /// properties of <paramref name="backgroundDataConfiguration"/>.
        /// </summary>
        /// <param name="backgroundDataConfiguration">The <see cref="BackgroundData"/> 
        /// to base the <see cref="WellKnownTileSourceMapData"/> upon.</param>
        /// <returns>The newly created <see cref="WellKnownTileSourceMapData"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when 
        /// <see cref="WellKnownBackgroundDataConfiguration.WellKnownTileSource"/>
        /// contains an invalid value for <see cref="WellKnownTileSource"/>.</exception>
        private static WellKnownTileSourceMapData CreateWellKnownMapdata(WellKnownBackgroundDataConfiguration backgroundDataConfiguration)
        {
            return new WellKnownTileSourceMapData(backgroundDataConfiguration.WellKnownTileSource);
        }
    }
}