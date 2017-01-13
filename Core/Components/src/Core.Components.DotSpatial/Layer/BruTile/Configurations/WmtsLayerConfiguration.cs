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
        private readonly string persistentCacheDirectoryPath;

        private readonly Uri capabilitiesUri;
        private readonly string capabilityName;

        /// <summary>
        /// Creates an instance of <see cref="WmtsLayerConfiguration"/>.
        /// </summary>
        /// <param name="wmtsCapabilitiesUrl">The capabilities url of the WMTS.</param>
        /// <param name="capabilityName">The capability name to get tiles from.</param>
        /// <param name="persistentCacheDirectoryPath">The directory path to the persistent tile cache.</param>
        public WmtsLayerConfiguration(string wmtsCapabilitiesUrl, string capabilityName, string persistentCacheDirectoryPath)
            : base(persistentCacheDirectoryPath)
        {
            capabilitiesUri = new Uri(wmtsCapabilitiesUrl);
            this.capabilityName = capabilityName;
            LegendText = capabilityName;

            this.persistentCacheDirectoryPath = persistentCacheDirectoryPath;

            Initialized = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="persistentCacheDirectoryPath"></param>
        /// <param name="name"></param>
        /// <param name="tileSource"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="tileSource"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache at <paramref name="persistentCacheDirectoryPath"/> failed.</exception>
        [Obsolete("Do not use due to being unable to retrieve the 'Capabilities' URL. This means instances created with this constructor do not support Clone().")]
        public WmtsLayerConfiguration(string persistentCacheDirectoryPath, string name, ITileSource tileSource)
            : base(persistentCacheDirectoryPath)
        {
            this.persistentCacheDirectoryPath = persistentCacheDirectoryPath;

            LegendText = name;
            capabilityName = name;

            if (tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            }

            InitializeFromTileSource(tileSource);
        }

        public ITileCache<byte[]> TileCache { get; private set; }

        public bool Initialized { get; private set; }

        public string LegendText { get; }

        public ITileSource TileSource { get; private set; }

        public TileFetcher TileFetcher { get; private set; }

        public IConfiguration Clone()
        {
            return new WmtsLayerConfiguration(capabilitiesUri.AbsolutePath, capabilityName, persistentCacheDirectoryPath);
        }

        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            ITileSource tileSource = GetConfiguredTileSource();

            InitializeFromTileSource(tileSource);
        }

        /// <summary>
        /// Connects to the WMTS to retrieve the configured tile source.
        /// </summary>
        /// <returns>The tile source, or null if no matching tile source could be found.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when unable to retrieve
        /// the configured tile source.</exception>
        private ITileSource GetConfiguredTileSource()
        {
            IEnumerable<ITileSource> tileSources = GetTileSources(capabilitiesUri);
            ITileSource tileSource = tileSources.FirstOrDefault(ts => ts.Name.Equals(capabilityName, StringComparison.InvariantCulture));
            if (tileSource == null)
            {
                string message = string.Format("Niet in staat om de databron met naam '{0}' te kunnen vinden bij de WTMS url '{1}'.",
                                               capabilitiesUri, capabilityName);
                throw new CannotFindTileSourceException(message);
            }
            return tileSource;
        }

        /// <summary>
        /// Connects to the WMTS and retrieves all available tile sources.
        /// </summary>
        /// <param name="capabilitiesUrl">The capabilities URL.</param>
        /// <returns>All available tile sources.</returns>
        /// <exception cref="CannotFindTileSourceException">Thrown when an exception occurs
        /// while retrieving the available tile sources.</exception>
        private static IEnumerable<ITileSource> GetTileSources(Uri capabilitiesUrl)
        {
            try
            {
                var req = WebRequest.Create(capabilitiesUrl);
                using (var resp = req.GetResponse())
                {
                    using (var s = resp.GetResponseStream())
                    {
                        return WmtsParser.Parse(s);
                    }
                }
            }
            catch (Exception e)
            {
                string message = string.Format("Niet in staat om de databronnen op te halen bij de WTMS url '{0}'.", capabilitiesUrl);
                throw new CannotFindTileSourceException(message, e);
            }
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