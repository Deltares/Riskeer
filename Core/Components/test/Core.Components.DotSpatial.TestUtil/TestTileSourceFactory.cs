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

using System.Collections.Generic;
using BruTile;
using Core.Components.DotSpatial.Layer.BruTile.Configurations;
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.TestUtil
{
    /// <summary>
    /// A <see cref="ITileSourceFactory"/> implementation suitable for unit testing purposes.
    /// </summary>
    public class TestTileSourceFactory : ITileSourceFactory
    {
        private readonly TestWmtsTileSource wmtsTileSource;

        /// <summary>
        /// Initializes a new instance of <see cref="TestTileSourceFactory"/> for a given
        /// <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="backgroundMapData">The map data to work for.</param>
        /// <remarks>If <paramref name="backgroundMapData"/> isn't initialized at construction
        /// time, then <see cref="GetWmtsTileSources"/> returns no tile sources.</remarks>
        public TestTileSourceFactory(WmtsMapData backgroundMapData)
        {
            if (backgroundMapData.IsConfigured)
            {
                wmtsTileSource = new TestWmtsTileSource(backgroundMapData);
            }
        }

        public IEnumerable<ITileSource> GetWmtsTileSources(string capabilitiesUrl)
        {
            if (wmtsTileSource != null)
            {
                yield return wmtsTileSource;
            }
        }
    }
}