// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Common.Util.TypeConverters
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
        /// <exception cref="NotSupportedException">Thrown when the <see cref="ImageBasedMapData"/>
        /// is not supported for conversion.</exception>
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
        /// <exception cref="NotSupportedException">Thrown when <see cref="BackgroundData.Configuration"/>
        /// doesn't is of a type that can be converted.</exception>
        public static ImageBasedMapData ConvertFrom(BackgroundData backgroundData)
        {
            if (backgroundData == null)
            {
                throw new ArgumentNullException(nameof(backgroundData));
            }

            ImageBasedMapData mapData = CreateMapData(backgroundData);

            mapData.IsVisible = backgroundData.IsVisible;
            mapData.Transparency = backgroundData.Transparency;

            return mapData;
        }

        /// <summary>
        /// Creates the map data with data of the <see cref="IBackgroundDataConfiguration"/>.
        /// </summary>
        /// <param name="backgroundData">The background data to get the configuration from
        /// and create the data for.</param>
        /// <returns>The created <see cref="ImageBasedMapData"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <see cref="BackgroundData.Configuration"/>
        /// doesn't is of a type that can be converted.</exception>
        private static ImageBasedMapData CreateMapData(BackgroundData backgroundData)
        {
            var wmtsBackgroundDataConfiguration = backgroundData.Configuration as WmtsBackgroundDataConfiguration;
            if (wmtsBackgroundDataConfiguration != null)
            {
                return CreateWmtsMapData(backgroundData.Name, wmtsBackgroundDataConfiguration);
            }

            var wellKnownBackgroundDataConfiguration = backgroundData.Configuration as WellKnownBackgroundDataConfiguration;
            if (wellKnownBackgroundDataConfiguration != null)
            {
                return CreateWellKnownMapdata(wellKnownBackgroundDataConfiguration);
            }

            throw new NotSupportedException($"Can't create a image based map data for {backgroundData.Configuration.GetType()}.");
        }

        /// <summary>
        /// Creates a <see cref="IBackgroundDataConfiguration"/> based on the <see cref="ImageBasedMapData"/>.
        /// </summary>
        /// <param name="mapData">The map <see cref="ImageBasedMapData"/> the configuration is based upon.</param>
        /// <returns>A <see cref="IBackgroundDataConfiguration"/> based on the <see cref="ImageBasedMapData"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <see cref="ImageBasedMapData"/> is not supported
        /// for creating the configuration.</exception>
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

            throw new NotSupportedException($"Can't create a background data configuration for {mapData.GetType()}.");
        }

        private static WellKnownBackgroundDataConfiguration CreateWellKnownBackgroundDataConfiguration(WellKnownTileSourceMapData mapData)
        {
            return new WellKnownBackgroundDataConfiguration((RiskeerWellKnownTileSource) mapData.TileSource);
        }

        private static WmtsBackgroundDataConfiguration CreateWmtsBackgroundDataConfiguration(WmtsMapData mapData)
        {
            return new WmtsBackgroundDataConfiguration(mapData.IsConfigured,
                                                       mapData.SourceCapabilitiesUrl,
                                                       mapData.SelectedCapabilityIdentifier,
                                                       mapData.PreferredFormat);
        }

        private static WmtsMapData CreateWmtsMapData(string name, WmtsBackgroundDataConfiguration backgroundDataConfiguration)
        {
            if (backgroundDataConfiguration.IsConfigured)
            {
                return new WmtsMapData(name,
                                       backgroundDataConfiguration.SourceCapabilitiesUrl,
                                       backgroundDataConfiguration.SelectedCapabilityIdentifier,
                                       backgroundDataConfiguration.PreferredFormat);
            }

            return new WmtsMapData(name);
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
            return new WellKnownTileSourceMapData((WellKnownTileSource) backgroundDataConfiguration.WellKnownTileSource);
        }
    }
}