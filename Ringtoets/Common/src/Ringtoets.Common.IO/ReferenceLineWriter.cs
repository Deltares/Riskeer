// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Writers;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Shapefile writer that writes a <see cref="ReferenceLine"/> as a line feature.
    /// </summary>
    public class ReferenceLineWriter
    {
        /// <summary>
        /// Writes a <see cref="ReferenceLine"/> as a line feature in a shapefile.
        /// </summary>
        /// <param name="referenceLine">The reference line which is to be written to file.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public static void WriteReferenceLine(ReferenceLine referenceLine, string filePath)
        {
            var polyLineShapeFileWriter = new PolylineShapeFileWriter();

            MapLineData mapLineData = CreateMapLineData(referenceLine);

            polyLineShapeFileWriter.AddFeature(mapLineData);

            polyLineShapeFileWriter.SaveAs(filePath);
        }

        private static MapLineData CreateMapLineData(ReferenceLine referenceLine)
        {
            MapGeometry referenceGeometry = new MapGeometry(
                new List<IEnumerable<Point2D>>
                {
                    referenceLine.Points
                });

            MapFeature mapFeature = new MapFeature(new []{referenceGeometry});

            return new MapLineData("referentielijn")
            {
                Features = new[] { mapFeature }
            };
        }
    }
}