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
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Readers;
using Riskeer.Common.Data.AssessmentSection;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.Common.IO.ReferenceLines
{
    /// <summary>
    /// Shapefile reader that reads a <see cref="ReferenceLine"/> based on the line feature in the file.
    /// </summary>
    public class ReferenceLineReader
    {
        /// <summary>
        /// Reads an instance of <see cref="ReferenceLine"/> from a shapefile containing one polyline.
        /// </summary>
        /// <param name="shapeFilePath">The file path to the shapefile.</param>
        /// <returns>The reference line created from the data in the shapefile.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item>There isn't exactly 1 polyline in the shapefile.</item>
        /// <item>Shapefile contains a multi-polyline.</item>
        /// <item>An unexpected error occurred when reading the shapefile.</item>
        /// </list>
        /// </exception>
        public ReferenceLine ReadReferenceLine(string shapeFilePath)
        {
            IOUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            using (var lineShapeReader = new PolylineShapeFileReader(shapeFilePath))
            {
                MapLineData lineMapData = GetReferenceLineMapData(lineShapeReader, shapeFilePath);
                return CreateReferenceLine(lineMapData, shapeFilePath);
            }
        }

        /// <summary>
        /// Gets the reference line map data.
        /// </summary>
        /// <param name="lineShapeReader">The line shape reader.</param>
        /// <param name="shapeFilePath">The shapefile path.</param>
        /// <returns>The map data representing the reference line.</returns>
        /// <exception cref="CriticalFileReadException">
        /// When either:
        /// <list type="bullet">
        /// <item>There isn't exactly 1 polyline in the shapefile.</item>
        /// <item>The shapefile doesn't contains lines.</item>
        /// </list>
        /// </exception>
        private static MapLineData GetReferenceLineMapData(PolylineShapeFileReader lineShapeReader, string shapeFilePath)
        {
            if (lineShapeReader.GetNumberOfFeatures() != 1)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineReader_File_must_contain_1_polyline);
                throw new CriticalFileReadException(message);
            }

            return (MapLineData) lineShapeReader.ReadFeature(RingtoetsCommonDataResources.ReferenceLine_DisplayName);
        }

        private static ReferenceLine CreateReferenceLine(MapLineData lineMapData, string shapeFilePath)
        {
            MapFeature[] lineFeatures = lineMapData.Features.ToArray();

            if (lineFeatures.Length > 1)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineReader_File_contains_unsupported_multi_polyline);
                throw new CriticalFileReadException(message);
            }

            var referenceLine = new ReferenceLine();
            MapFeature referenceLineFeature = lineMapData.Features.First();
            MapGeometry[] referenceGeometries = referenceLineFeature.MapGeometries.ToArray();

            if (referenceGeometries.Length > 1)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineReader_File_contains_unsupported_multi_polyline);
                throw new CriticalFileReadException(message);
            }

            MapGeometry referenceGeometry = referenceGeometries[0];
            referenceLine.SetGeometry(referenceGeometry.PointCollections.First().Select(t => new Point2D(t)).ToArray());
            return referenceLine;
        }
    }
}