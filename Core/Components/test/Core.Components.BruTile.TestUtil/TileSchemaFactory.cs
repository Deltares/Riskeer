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
using System.Text.RegularExpressions;
using BruTile;
using BruTile.Wmts;
using Core.Components.Gis.Data;

namespace Core.Components.BruTile.TestUtil
{
    /// <summary>
    /// Factory class to create instances of <see cref="ITileSchema"/>.
    /// </summary>
    public static class TileSchemaFactory
    {
        /// <summary>
        /// Create a <see cref="WmtsTileSchema"/> based on a <see cref="WmtsMapData"/> instance.
        /// </summary>
        /// <param name="mapData">The data for which the schema will be based upon.</param>
        /// <returns>The newly created tile schema.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mapData"/> isn't
        /// configured.</exception>
        public static WmtsTileSchema CreateWmtsTileSchema(WmtsMapData mapData)
        {
            if (!mapData.IsConfigured)
            {
                throw new ArgumentException($@"Only configured {typeof(WmtsMapData).Name} instances can be used to create a schema for.",
                                            nameof(mapData));
            }

            WmtsTileSchema schema = ConstructWmtsTileSchema();
            schema.Title = mapData.Name;
            schema.Format = mapData.PreferredFormat;

            schema.Resolutions["1"] = new Resolution("1", 1);

            var capabilityIdRegex = new Regex(@"(?<Layer>.+)\((?<TileMatrixSet>.+)\)");
            Match match = capabilityIdRegex.Match(mapData.SelectedCapabilityIdentifier);
            schema.Layer = match.Groups["Layer"].Value;
            schema.TileMatrixSet = match.Groups["TileMatrixSet"].Value;

            var coordinateSystemRegex = new Regex(@"EPSG:(?<SrsNumber>\d+)");
            Match potentialMatch = coordinateSystemRegex.Match(schema.TileMatrixSet);
            schema.Srs = potentialMatch.Success ? $"EPSG:{potentialMatch.Groups["SrsNumber"]}" : "EPSG:3857";

            return schema;
        }

        private static WmtsTileSchema ConstructWmtsTileSchema()
        {
            return (WmtsTileSchema) Activator.CreateInstance(typeof(WmtsTileSchema), true);
        }
    }
}