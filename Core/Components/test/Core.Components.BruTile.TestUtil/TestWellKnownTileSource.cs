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
using System.Drawing.Imaging;
using System.IO;
using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using Core.Components.BruTile.TestUtil.Properties;
using Core.Components.Gis.Data;

namespace Core.Components.BruTile.TestUtil
{
    /// <summary>
    /// Defines an <see cref="ITileSource"/> suitable to most unit test cases related to
    /// dealing with <see cref="WellKnownTileSourceMapData"/>.
    /// </summary>
    public class TestWellKnownTileSource : HttpTileSource
    {
        private static byte[] pngTileData;

        /// <summary>
        /// Create a new instance of <see cref="TestWellKnownTileSource"/> suitable to work for
        /// a given <see cref="WellKnownTileSourceMapData"/>.
        /// </summary>
        /// <param name="mapData">The map data to work with.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/> is <c>null</c>.</exception>
        public TestWellKnownTileSource(WellKnownTileSourceMapData mapData)
            : base(CreateTileSchema(mapData), new WellKnownRequestStub(), "Stub schema", null, GetStubTile) {}

        /// <summary>
        /// Create a new tile schema.
        /// </summary>
        /// <param name="mapData">The map data to work with.</param>
        /// <returns>The new tile schema.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/> is <c>null</c>.</exception>
        private static GlobalSphericalMercator CreateTileSchema(WellKnownTileSourceMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            return new GlobalSphericalMercator(YAxis.TMS, 0, 19, mapData.Name);
        }

        private static byte[] GetStubTile(Uri url)
        {
            if (pngTileData == null)
            {
                using (var stream = new MemoryStream())
                {
                    Resources.stubTile.Save(stream, ImageFormat.Png);
                    pngTileData = stream.ToArray();
                }
            }

            return pngTileData;
        }

        private class WellKnownRequestStub : IRequest
        {
            public Uri GetUri(TileInfo info)
            {
                return new Uri(@"https://www.stub.org/tiles/someTile.png");
            }
        }
    }
}