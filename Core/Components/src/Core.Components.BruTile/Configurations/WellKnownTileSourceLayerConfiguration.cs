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
using System.IO;
using BruTile;
using BruTile.Predefined;
using Core.Components.BruTile.Data;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;

namespace Core.Components.BruTile.Configurations
{
    /// <summary>
    /// A configuration for a connection to built-in web-based tile services.
    /// </summary>
    /// <remarks>
    /// Original source: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Configuration/KnownTileLayerConfiguration.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </remarks>
    public class WellKnownTileSourceLayerConfiguration : PersistentCacheConfiguration
    {
        private readonly KnownTileSource knownTileSource;

        /// <summary>
        /// Creates an instance of <see cref="WellKnownTileSourceLayerConfiguration"/>.
        /// </summary>
        /// <param name="knownTileSource">The built-in tile provider to be used.</param>
        /// <param name="persistentCacheDirectoryPath">The directory path to the persistent tile cache.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="persistentCacheDirectoryPath"/>
        /// is an invalid folder path.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="knownTileSource"/>
        /// isn't a supported member.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache failed.</exception>
        private WellKnownTileSourceLayerConfiguration(string persistentCacheDirectoryPath, KnownTileSource knownTileSource)
            : base(persistentCacheDirectoryPath)
        {
            this.knownTileSource = knownTileSource;

            ITileSource tileSource = TileSourceFactory.Instance.GetKnownTileSource(knownTileSource);
            InitializeFromTileSource(tileSource);
        }

        /// <summary>
        /// Creates a new initialized instance of <see cref="WellKnownTileSourceLayerConfiguration"/>.
        /// </summary>
        /// <param name="knownTileSource">The built-in tile provider to be used.</param>
        /// <param name="tileSource">The tile source corresponding to <paramref name="knownTileSource"/>.</param>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache failed.</exception>
        private WellKnownTileSourceLayerConfiguration(KnownTileSource knownTileSource, ITileSource tileSource)
            : base(SuggestTileCachePath(tileSource, knownTileSource))
        {
            this.knownTileSource = knownTileSource;

            InitializeFromTileSource(tileSource);
        }

        /// <summary>
        /// Creates a fully initialized instance of <see cref="WellKnownTileSourceLayerConfiguration"/>.
        /// </summary>
        /// <param name="wellKnownTileSource">The tile provider to be used.</param>
        /// <returns>The new <see cref="WellKnownTileSourceLayerConfiguration"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="wellKnownTileSource"/>
        /// isn't a supported member.</exception>
        /// <exception cref="CannotCreateTileCacheException">Thrown when creating the file
        /// cache failed.</exception>
        public static WellKnownTileSourceLayerConfiguration CreateInitializedConfiguration(WellKnownTileSource wellKnownTileSource)
        {
            KnownTileSource knownTileSourceEquivalent = WellKnownTileSourceToKnownTileSource(wellKnownTileSource);

            ITileSource tileSource = TileSourceFactory.Instance.GetKnownTileSource(knownTileSourceEquivalent);
            return new WellKnownTileSourceLayerConfiguration(knownTileSourceEquivalent, tileSource);
        }

        protected override IConfiguration OnClone()
        {
            return new WellKnownTileSourceLayerConfiguration(PersistentCacheDirectoryPath, knownTileSource);
        }

        protected override void OnInitialize()
        {
            Initialized = true;
        }

        /// <summary>
        /// Returns the <see cref="KnownTileSource"/> equivalent of <see cref="WellKnownTileSource"/>.
        /// </summary>
        /// <param name="wellKnownTileSource">The tile provider to be used.</param>
        /// <returns>The <see cref="KnownTileSource"/> equivalent of the <paramref name="wellKnownTileSource"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="wellKnownTileSource"/>
        /// is not a valid enum value of <see cref="WellKnownTileSource"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="wellKnownTileSource"/>
        /// is not a supported member.</exception>
        private static KnownTileSource WellKnownTileSourceToKnownTileSource(WellKnownTileSource wellKnownTileSource)
        {
            if (!Enum.IsDefined(typeof(WellKnownTileSource), wellKnownTileSource))
            {
                throw new InvalidEnumArgumentException(nameof(wellKnownTileSource),
                                                       (int) wellKnownTileSource,
                                                       typeof(WellKnownTileSource));
            }

            switch (wellKnownTileSource)
            {
                case WellKnownTileSource.BingAerial:
                    return KnownTileSource.BingAerial;
                case WellKnownTileSource.BingHybrid:
                    return KnownTileSource.BingHybrid;
                case WellKnownTileSource.BingRoads:
                    return KnownTileSource.BingRoads;
                case WellKnownTileSource.EsriWorldTopo:
                    return KnownTileSource.EsriWorldTopo;
                case WellKnownTileSource.EsriWorldShadedRelief:
                    return KnownTileSource.EsriWorldShadedRelief;
                case WellKnownTileSource.OpenStreetMap:
                    return KnownTileSource.OpenStreetMap;
                default:
                    throw new NotSupportedException();
            }
        }

        private static string SuggestTileCachePath(ITileSource tileSource, KnownTileSource knownTileSource)
        {
            ITileSchema tileSchema = tileSource.Schema;
            string host = knownTileSource.ToString();
            string format = tileSchema.Format;

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                host = host.Replace(c, '$');
            }

            return Path.Combine(BruTileSettings.PersistentCacheDirectoryRoot, "WellKnown", host, format);
        }
    }
}