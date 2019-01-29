// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Util.IO;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.IO.Readers;

namespace Riskeer.DuneErosion.IO
{
    /// <summary>
    /// Shapefile reader that reads features and converts it into a collection of <see cref="ReadDuneLocation"/>.
    /// </summary>
    public class DuneLocationsReader
    {
        private const string nameKey = "Naam";
        private const string coastalAreaIdKey = "KV";
        private const string offsetKey = "RSP";
        private const string orientationKey = "Richting";
        private const string d50Key = "Dreken";

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of <see cref="ReadDuneLocation"/> based on the line features within the embedded shape file.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ReadDuneLocation"/>.</returns>
        public IEnumerable<ReadDuneLocation> ReadDuneLocations()
        {
            using (var embeddedResourceFileWriter = new EmbeddedResourceFileWriter(typeof(DuneLocationsReader).Assembly,
                                                                                   true,
                                                                                   "RSPstelsel.shp",
                                                                                   "RSPstelsel.dbf",
                                                                                   "RSPstelsel.cpg",
                                                                                   "RSPstelsel.sbn",
                                                                                   "RSPstelsel.sbx",
                                                                                   "RSPstelsel.shx"))
            {
                string filePath = Path.Combine(embeddedResourceFileWriter.TargetFolderPath, "RSPstelsel.shp");

                var readDuneLocations = new List<ReadDuneLocation>();
                using (var pointShapeReader = new PointShapeFileReader(filePath))
                {
                    FeatureBasedMapData locationsData = pointShapeReader.ReadShapeFile();
                    readDuneLocations.AddRange(CreateDuneLocations(locationsData));
                }

                return readDuneLocations;
            }
        }

        private static IEnumerable<ReadDuneLocation> CreateDuneLocations(FeatureBasedMapData locationsData)
        {
            foreach (MapFeature locationData in locationsData.Features)
            {
                Point2D location = locationData.MapGeometries.First().PointCollections.First().First();

                object nameValue = locationData.MetaData[nameKey];
                string name = nameValue?.ToString() ?? string.Empty;

                int coastalAreaId = Convert.ToInt32(locationData.MetaData[coastalAreaIdKey]);
                double offset = Convert.ToDouble(locationData.MetaData[offsetKey]);
                double orientation = Convert.ToDouble(locationData.MetaData[orientationKey]);
                double d50 = Convert.ToDouble(locationData.MetaData[d50Key]);

                yield return new ReadDuneLocation(name, location, coastalAreaId, offset, orientation, d50);
            }
        }
    }
}