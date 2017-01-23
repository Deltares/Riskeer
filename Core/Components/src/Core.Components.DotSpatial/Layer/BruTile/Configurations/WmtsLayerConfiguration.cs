// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Net;
using BruTile;
using BruTile.Cache;
using BruTile.Wmts;
using Core.Components.DotSpatial.Layer.BruTile.TileFetching;

namespace Core.Components.DotSpatial.Layer.BruTile.Configurations
{
    /// <summary>
    /// A configuration for a connection to a Web Map Tile Service (WMTS).
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/WmtsLayerConfiguration.cs
    /// </remarks>
    public class WmtsLayerConfiguration : PersistentCacheConfiguration, IConfiguration
    {
        private readonly string capabilitiesUri;
        private readonly string capabilityIdentifier;
        private readonly string preferredFormat;

        /// <summary>
        /// Creates an instance of <see cref="WmtsLayerConfiguration"/>.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities url of the WMTS.</param>
        /// <param name="capabilityIdentifier">The capability name to get tiles from.</param>
        /// <param name="preferredFormat">The preferred tile image format, as MIME-type.</param>
        /// <param name="persistentCacheDirectoryPath">The directory path to the persistent tile cache.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="persistentCacheDirectoryPath"/>
        /// is an invalid folder path</exception>
        public WmtsLayerConfiguration(string wmtsCapabilitiesUrl, string capabilityIdentifier, string preferredFormat,
                                      string persistentCacheDirectoryPath) : base(persistentCacheDirectoryPath)
        {
            ValidateConfigurationParameters(wmtsCapabilitiesUrl, capabilityIdentifier, preferredFormat);

            capabilitiesUri = wmtsCapabilitiesUrl;
            this.capabilityIdentifier = capabilityIdentifier;
            this.preferredFormat = preferredFormat;
            LegendText = capabilityIdentifier;

            Initialized = false;
        }

        /// <summary>
        /// Creates a new initialized instance of <see cref="WmtsLayerConfiguration"/>.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities url of the WMTS.</param>
        /// <param name="tileSource">The tile source.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tileSource"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache failed.</exception>
        private WmtsLayerConfiguration(string wmtsCapabilitiesUrl, ITileSource tileSource)
            : base(SuggestTileCachePath(tileSource))
        {
            if (tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            }

            capabilitiesUri = wmtsCapabilitiesUrl;
            capabilityIdentifier = ((WmtsTileSchema) tileSource.Schema).Identifier;
            preferredFormat = tileSource.Schema.Format;

            InitializeFromTileSource(tileSource);
        }

        public ITileCache<byte[]> TileCache { get; private set; }

        public bool Initialized { get; private set; }

        public string LegendText { get; }

        public ITileSource TileSource { get; private set; }

        public TileFetcher TileFetcher { get; private set; }

        public static WmtsLayerConfiguration CreateInitializedConfiguration(string capabilitiesUrl, string capabilityIdentifier, string preferredFormat)
        {
            ITileSource tileSource = GetConfiguredTileSource(capabilitiesUrl, capabilityIdentifier, preferredFormat);
            return new WmtsLayerConfiguration(capabilitiesUrl, tileSource);
        }

        public IConfiguration Clone()
        {
            return new WmtsLayerConfiguration(capabilitiesUri, capabilityIdentifier, persistentCacheDirectoryPath, preferredFormat);
        }

        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            ITileSource tileSource = GetConfiguredTileSource(capabilitiesUri, capabilityIdentifier, preferredFormat);

            InitializeFromTileSource(tileSource);
        }

        /// <summary>
        /// Validate the configuration parameters.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities url of the WMTS.</param>
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
                throw new ArgumentException("Specified image format is not a MIME type.", nameof(preferredFormat));
            }
        }

        /// <summary>
        /// Connects to the WMTS to retrieve the configured tile source.
        /// </summary>
        /// <param name="capabilitiesUri">The URL of the tile source server.</param>
        /// <param name="capabilityIdentifier">The identifier of the tile source.</param>
        /// <param name="preferredFormat">The preferred tile image format, as MIME-type.</param>
        /// <returns>The tile source, or null if no matching tile source could be found.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when unable to retrieve
        /// the configured tile source.</exception>
        private static ITileSource GetConfiguredTileSource(string capabilitiesUri, string capabilityIdentifier, string preferredFormat)
        {
            IEnumerable<ITileSource> tileSources = TileSourceFactory.Instance.GetWmtsTileSources(capabilitiesUri);
            ITileSource tileSource = tileSources.FirstOrDefault(ts => IsMatch(ts, capabilityIdentifier, preferredFormat));
            if (tileSource == null)
            {
                string message = string.Format("Niet in staat om de databron met naam '{0}' te kunnen vinden bij de WTMS url '{1}'.",
                                               capabilitiesUri, capabilityIdentifier);
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

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                host = host.Replace(c, '$');
            }
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                layerStyle = layerStyle.Replace(c, '$');
            }

            return Path.Combine(BruTileSettings.PersistentCacheDirectoryRoot, "Wmts", host, layerStyle, format);
        }

        /// <summary>
        /// Initialized the configuration based on the given <see cref="ITileSource"/>.
        /// </summary>
        /// <param name="tileSource">The tile source to initialize for.</param>
        /// <exception cref="CannotCreateTileCacheException">Thrown when a critical error
        /// occurs when creating the tile cache.</exception>
        private void InitializeFromTileSource(ITileSource tileSource)
        {
            TileSource = tileSource;
            TileCache = CreateTileCache();
            ITileProvider provider = BruTileReflectionHelper.GetProviderFromTileSource(tileSource);
            TileFetcher = new TileFetcher(provider,
                                          BruTileSettings.MemoryCacheMinimum,
                                          BruTileSettings.MemoryCacheMaximum,
                                          TileCache);
            Initialized = true;
        }
    }
}