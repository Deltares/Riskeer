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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BruTile;
using BruTile.Wmts;
using Core.Components.BruTile.Data;
using Core.Components.BruTile.Properties;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// A configuration for a connection to a Web Map Tile Service (WMTS).
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/WmtsLayerConfiguration.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public class WmtsLayerConfiguration : PersistentCacheConfiguration
    {
        private readonly string capabilitiesUri;
        private readonly string capabilityIdentifier;
        private readonly string preferredFormat;

        /// <summary>
        /// Creates an instance of <see cref="WmtsLayerConfiguration"/>.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities URL of the WMTS.</param>
        /// <param name="capabilityIdentifier">The capability name to get tiles from.</param>
        /// <param name="preferredFormat">The preferred tile image format, as MIME-type.</param>
        /// <param name="persistentCacheDirectoryPath">The directory path to the persistent tile cache.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="persistentCacheDirectoryPath"/>
        /// is an invalid folder path.</exception>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        private WmtsLayerConfiguration(string wmtsCapabilitiesUrl, string capabilityIdentifier, string preferredFormat,
                                       string persistentCacheDirectoryPath) : base(persistentCacheDirectoryPath)
        {
            ValidateConfigurationParameters(wmtsCapabilitiesUrl, capabilityIdentifier, preferredFormat);

            capabilitiesUri = wmtsCapabilitiesUrl;
            this.capabilityIdentifier = capabilityIdentifier;
            this.preferredFormat = preferredFormat;

            Initialized = false;
        }

        /// <summary>
        /// Creates a new initialized instance of <see cref="WmtsLayerConfiguration"/>.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities URL of the WMTS.</param>
        /// <param name="tileSource">The tile source.</param>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache failed.</exception>
        /// <exception cref="CannotReceiveTilesException">Thrown when <paramref name="tileSource"/>
        /// does not allow for tiles to be retrieved.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// occurs when creating the tile cache.</exception>
        private WmtsLayerConfiguration(string wmtsCapabilitiesUrl, ITileSource tileSource)
            : base(SuggestTileCachePath(ValidateTileSource(tileSource)))
        {
            capabilitiesUri = wmtsCapabilitiesUrl;
            capabilityIdentifier = ((WmtsTileSchema) tileSource.Schema).Identifier;
            preferredFormat = tileSource.Schema.Format;

            InitializeFromTileSource(tileSource);
        }

        /// <summary>
        /// Creates a fully initialized instance of <see cref="WmtsLayerConfiguration"/>.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities URL of the WMTS.</param>
        /// <param name="capabilityIdentifier">The capability name to get tiles from.</param>
        /// <param name="preferredFormat">The preferred tile image format, as MIME-type.</param>
        /// <returns>The new <see cref="WmtsLayerConfiguration"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="preferredFormat"/>
        /// is not an image MIME-type.</exception>
        /// <exception cref="CannotFindTileSourceException">Thrown when it has become impossible
        /// to create an <see cref="ITileSource"/> based on the given information (for example:
        /// unable to connect to server).</exception>
        /// <exception cref="CannotReceiveTilesException">Thrown when the configured <see cref="ITileSource"/>
        /// does not allow for tiles to be retrieved.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// occurs when creating the tile cache.</exception>
        public static WmtsLayerConfiguration CreateInitializedConfiguration(string wmtsCapabilitiesUrl, string capabilityIdentifier, string preferredFormat)
        {
            ValidateConfigurationParameters(wmtsCapabilitiesUrl, capabilityIdentifier, preferredFormat);

            ITileSource tileSource = GetConfiguredTileSource(wmtsCapabilitiesUrl, capabilityIdentifier, preferredFormat);
            return new WmtsLayerConfiguration(wmtsCapabilitiesUrl, tileSource);
        }

        protected override IConfiguration OnClone()
        {
            return new WmtsLayerConfiguration(capabilitiesUri, capabilityIdentifier, preferredFormat, PersistentCacheDirectoryPath);
        }

        protected override void OnInitialize()
        {
            if (Initialized)
            {
                return;
            }

            ITileSource tileSource = GetConfiguredTileSource(capabilitiesUri, capabilityIdentifier, preferredFormat);

            InitializeFromTileSource(tileSource);
        }

        /// <summary>
        /// Validates an <see cref="ITileSource"/>.
        /// </summary>
        /// <param name="tileSource">The source to be validated.</param>
        /// <returns>Returns <paramref name="tileSource"/>.</returns>
        /// <exception cref="CannotCreateTileCacheException">Thrown when <paramref name="tileSource"/>
        /// doesn't contain a <see cref="WmtsTileSchema"/>.</exception>
        private static ITileSource ValidateTileSource(ITileSource tileSource)
        {
            if (!(tileSource.Schema is WmtsTileSchema))
            {
                throw new CannotCreateTileCacheException(Resources.WmtsLayerConfiguration_ValidateTileSource_TileSource_must_have_WmtsTileSchema);
            }

            return tileSource;
        }

        /// <summary>
        /// Validate the configuration parameters.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities URL of the WMTS.</param>
        /// <param name="capabilityIdentifier">The capability name to get tiles from.</param>
        /// <param name="preferredFormat">The preferred tile image format, as MIME-type.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="preferredFormat"/>
        /// is not specified as a MIME-type.</exception>
        private static void ValidateConfigurationParameters(string wmtsCapabilitiesUrl, string capabilityIdentifier, string preferredFormat)
        {
            if (wmtsCapabilitiesUrl == null)
            {
                throw new ArgumentNullException(nameof(wmtsCapabilitiesUrl));
            }

            if (capabilityIdentifier == null)
            {
                throw new ArgumentNullException(nameof(capabilityIdentifier));
            }

            if (preferredFormat == null)
            {
                throw new ArgumentNullException(nameof(preferredFormat));
            }

            if (!preferredFormat.StartsWith("image/"))
            {
                throw new ArgumentException(Resources.WmtsLayerConfiguration_ValidateConfigurationParameters_PreferredFormat_must_be_mimetype,
                                            nameof(preferredFormat));
            }
        }

        /// <summary>
        /// Connects to the WMTS to retrieve the configured tile source.
        /// </summary>
        /// <param name="capabilitiesUri">The URL of the tile source server.</param>
        /// <param name="capabilityIdentifier">The identifier of the tile source.</param>
        /// <param name="preferredFormat">The preferred tile image format, as MIME-type.</param>
        /// <returns>The tile source with <see cref="WmtsTileSchema"/>.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when unable to retrieve
        /// the configured tile source.</exception>
        private static ITileSource GetConfiguredTileSource(string capabilitiesUri, string capabilityIdentifier, string preferredFormat)
        {
            IEnumerable<ITileSource> tileSources = TileSourceFactory.Instance.GetWmtsTileSources(capabilitiesUri);
            ITileSource tileSource = tileSources.FirstOrDefault(ts => IsMatch(ts, capabilityIdentifier, preferredFormat));
            if (tileSource == null)
            {
                string message = string.Format(Resources.WmtsLayerConfiguration_GetConfiguredTileSource_Cannot_find_LayerId_0_at_WmtsUrl_1_,
                                               capabilityIdentifier, capabilitiesUri);
                throw new CannotFindTileSourceException(message);
            }

            return tileSource;
        }

        private static bool IsMatch(ITileSource wmtsTileSource, string capabilityIdentifier, string preferredFormat)
        {
            var schema = (WmtsTileSchema) wmtsTileSource.Schema;
            return schema.Identifier.Equals(capabilityIdentifier)
                   && schema.Format.Equals(preferredFormat);
        }

        private static string SuggestTileCachePath(ITileSource tileSource)
        {
            var tileSchema = (WmtsTileSchema) tileSource.Schema;
            string host = tileSchema.Title;
            string format = tileSchema.Format.Split('/')[1];
            string layerStyle = tileSchema.Identifier;
            if (!string.IsNullOrEmpty(tileSchema.Style))
            {
                layerStyle += "_" + tileSchema.Style;
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                host = host.Replace(c, '$');
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                layerStyle = layerStyle.Replace(c, '$');
            }

            return Path.Combine(BruTileSettings.PersistentCacheDirectoryRoot, "Wmts", host, layerStyle, format);
        }
    }
}