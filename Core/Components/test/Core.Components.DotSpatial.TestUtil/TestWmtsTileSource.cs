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
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using BruTile;
using BruTile.Web;
using BruTile.Wmts;
using Core.Components.DotSpatial.TestUtil.Properties;
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.TestUtil
{
    /// <summary>
    /// Defines a <see cref="ITileSource"/> suitable to most unit test cases related to
    /// dealing with <see cref="WmtsMapData"/>.
    /// </summary>
    public class TestWmtsTileSource : HttpTileSource
    {
        private static byte[] pngTileDataStub;

        /// <summary>
        /// Create a new instance of <see cref="TestWmtsTileSource"/> suitable to work for
        /// a given <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="backgroundMapData">The map data to work with.</param>
        public TestWmtsTileSource(WmtsMapData backgroundMapData)
            : base(CreateWmtsTileSchema(backgroundMapData), null, "Stub schema", null, GetStubTile)
        {
            var imageFormatExtension = backgroundMapData.PreferredFormat.Split('/')[1];
            if (imageFormatExtension != "png")
            {
                throw new NotImplementedException($"Please extend this class to support the '*.{imageFormatExtension}' extension.");
            }
        }

        private static WmtsTileSchema CreateWmtsTileSchema(WmtsMapData backgroundMapData)
        {
            WmtsTileSchema schema = ConstructWmtsTileSchema();
            schema.Title = backgroundMapData.Name;
            schema.Format = backgroundMapData.PreferredFormat;
            
            schema.Resolutions["1"] = new Resolution("1", 1);

            var regex1 = new Regex(@"(?<Layer>.+)\((?<TileMatrixSet>.+)\)");
            Match match = regex1.Match(backgroundMapData.SelectedCapabilityIdentifier);
            schema.Layer = match.Groups["Layer"].Value;
            schema.TileMatrixSet = match.Groups["TileMatrixSet"].Value;

            var regex2 = new Regex(@"EPSG:(?<SrsNumber>\d+)");
            Match potentialMatch = regex2.Match(schema.TileMatrixSet);
            schema.Srs = potentialMatch.Success ?
                             $"EPSG:{potentialMatch.Groups["SrsNumber"]}" :
                             "EPSG:3857";

            return schema;
        }

        private static WmtsTileSchema ConstructWmtsTileSchema()
        {
            return (WmtsTileSchema) typeof(WmtsTileSchema)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
                .Invoke(null);
        }

        private static byte[] GetStubTile(Uri arg)
        {
            if (pngTileDataStub == null)
            {
                using (var stream = new MemoryStream())
                {
                    Resources.stubTile.Save(stream, ImageFormat.Png);
                    pngTileDataStub = stream.ToArray();
                }
            }
            return pngTileDataStub;
        }
    }
}