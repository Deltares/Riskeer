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
using System.ComponentModel;
using Core.Common.Util.Reflection;
using Core.Components.BruTile.Configurations;
using Core.Components.BruTile.Forms.Properties;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Forms
{
    /// <summary>
    /// Class responsible for creating <see cref="IConfiguration"/> instances for a given
    /// map data.
    /// </summary>
    public static class BrutileConfigurationFactory
    {
        /// <summary>
        /// Creates a new initialized <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="mapData">The <see cref="ImageBasedMapData"/> to create an
        /// <see cref="IConfiguration"/> for.</param>
        /// <returns>A new initialized configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when a configuration can't
        /// be created for the type of <paramref name="mapData"/>.</exception>
        /// <exception cref="ConfigurationInitializationException">Thrown when the configuration
        /// can't connect with the tile service or creating the file cache failed.</exception>
        public static IConfiguration CreateInitializedConfiguration(ImageBasedMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            var wmtsBackgroundMapData = mapData as WmtsMapData;
            var wellKnownBackgroundMapData = mapData as WellKnownTileSourceMapData;
            if (wmtsBackgroundMapData != null)
            {
                return CreateInitializedConfiguration(wmtsBackgroundMapData);
            }

            if (wellKnownBackgroundMapData != null)
            {
                return CreateInitializedConfiguration(wellKnownBackgroundMapData);
            }

            throw new NotSupportedException($"Cannot create a configuration for type {mapData.GetType()}.");
        }

        /// <summary>
        /// Creates a new initialized <see cref="WellKnownTileSourceLayerConfiguration"/>.
        /// </summary>
        /// <param name="wellKnownBackgroundMapData">The <see cref="WellKnownTileSourceMapData"/>
        /// to create the configuration for.</param>
        /// <returns>A new initialized <see cref="WellKnownTileSourceLayerConfiguration"/>.</returns>
        /// <exception cref="ConfigurationInitializationException">Thrown when the configuration
        /// can't connect with the tile service or creating the file cache failed.</exception>
        private static WellKnownTileSourceLayerConfiguration CreateInitializedConfiguration(WellKnownTileSourceMapData wellKnownBackgroundMapData)
        {
            try
            {
                return WellKnownTileSourceLayerConfiguration.CreateInitializedConfiguration(wellKnownBackgroundMapData.TileSource);
            }
            catch (NotSupportedException e)
            {
                string tileDisplayName;
                try
                {
                    tileDisplayName = TypeUtils.GetDisplayName(wellKnownBackgroundMapData.TileSource);
                }
                catch (InvalidEnumArgumentException)
                {
                    tileDisplayName = wellKnownBackgroundMapData.TileSource.ToString();
                }

                throw new ConfigurationInitializationException(
                    string.Format(Resources.TryCreateInitializedConfiguration_InitializeBackgroundLayer_Connect_to_TileSourceName_0_failed,
                                  tileDisplayName),
                    e);
            }
            catch (CannotCreateTileCacheException e)
            {
                throw new ConfigurationInitializationException(
                    Resources.TryCreateInitializedConfiguration_InitializeBackgroundLayer_Persistent_cache_creation_failed,
                    e);
            }
        }

        /// <summary>
        /// Creates a new initialized <see cref="PersistentCacheConfiguration"/>.
        /// </summary>
        /// <param name="wmtsBackgroundMapData">The <see cref="WmtsMapData"/>
        /// to create the configuration for.</param>
        /// <returns>A new initialized <see cref="PersistentCacheConfiguration"/>.</returns>
        /// <exception cref="ConfigurationInitializationException">Thrown when the configuration
        /// can't connect with the tile service or creating the file cache failed.</exception>
        private static PersistentCacheConfiguration CreateInitializedConfiguration(WmtsMapData wmtsBackgroundMapData)
        {
            try
            {
                return WmtsLayerConfiguration.CreateInitializedConfiguration(wmtsBackgroundMapData.SourceCapabilitiesUrl,
                                                                             wmtsBackgroundMapData.SelectedCapabilityIdentifier,
                                                                             wmtsBackgroundMapData.PreferredFormat);
            }
            catch (Exception e) when (e is CannotFindTileSourceException || e is CannotReceiveTilesException)
            {
                throw new ConfigurationInitializationException(
                    Resources.TryCreateInitializedConfiguration_InitializeBackgroundLayer_Wmts_connection_failed,
                    e);
            }
            catch (CannotCreateTileCacheException e)
            {
                throw new ConfigurationInitializationException(
                    Resources.TryCreateInitializedConfiguration_InitializeBackgroundLayer_Persistent_cache_creation_failed,
                    e);
            }
        }
    }
}