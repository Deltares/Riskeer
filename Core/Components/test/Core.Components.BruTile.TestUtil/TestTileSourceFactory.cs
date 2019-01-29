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
using BruTile;
using BruTile.Predefined;
using Core.Components.BruTile.Configurations;
using Core.Components.Gis.Data;

namespace Core.Components.BruTile.TestUtil
{
    /// <summary>
    /// A <see cref="ITileSourceFactory"/> implementation suitable for unit testing purposes.
    /// </summary>
    public class TestTileSourceFactory : ITileSourceFactory
    {
        private TestWmtsTileSource wmtsTileSource;
        private TestWellKnownTileSource wellKnownTileSource;

        /// <summary>
        /// Initializes a new instance of <see cref="TestTileSourceFactory"/> for a given
        /// <see cref="ImageBasedMapData"/>.
        /// </summary>
        /// <param name="backgroundMapData">The map data to work for.</param>
        /// <remarks>If <paramref name="backgroundMapData"/> isn't initialized at construction
        /// time, then <see cref="GetWmtsTileSources"/> returns no tile sources.</remarks>
        public TestTileSourceFactory(ImageBasedMapData backgroundMapData)
        {
            var wellKnownMapData = backgroundMapData as WellKnownTileSourceMapData;
            if (wellKnownMapData != null)
            {
                SetWellKnownTileSource(wellKnownMapData);
            }

            var wmtsMapData = backgroundMapData as WmtsMapData;
            if (wmtsMapData != null && wmtsMapData.IsConfigured)
            {
                SetWmtsTileSource(wmtsMapData);
            }
        }

        public IEnumerable<ITileSource> GetWmtsTileSources(string capabilitiesUrl)
        {
            if (wmtsTileSource != null)
            {
                yield return wmtsTileSource;
            }
        }

        public ITileSource GetKnownTileSource(KnownTileSource knownTileSource)
        {
            if (wellKnownTileSource == null)
            {
                throw new NotSupportedException("use TestTileSourceFactory() to set the expected well known tile source");
            }

            return wellKnownTileSource;
        }

        private void SetWmtsTileSource(WmtsMapData backgroundMapData)
        {
            if (backgroundMapData.IsConfigured)
            {
                wmtsTileSource = new TestWmtsTileSource(backgroundMapData);
            }
        }

        private void SetWellKnownTileSource(WellKnownTileSourceMapData wellKnownMapData)
        {
            wellKnownTileSource = new TestWellKnownTileSource(wellKnownMapData);
        }
    }
}