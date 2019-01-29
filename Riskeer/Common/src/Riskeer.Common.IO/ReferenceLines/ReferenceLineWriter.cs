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
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.IO.Writers;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.IO.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.Common.IO.ReferenceLines
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
        /// <param name="id">The id of the assessment section to which this reference line is associated.</param>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/>, <paramref name="id"/> 
        /// or <paramref name="filePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <list type="bullet">
        /// <item><paramref name="filePath"/> is invalid, or</item>
        /// <item><paramref name="id"/> is empty or consists of whitespace.</item>
        /// </list></exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public void WriteReferenceLine(ReferenceLine referenceLine, string id, string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var polyLineShapeFileWriter = new PolylineShapeFileWriter();

            MapLineData mapLineData = CreateMapLineData(referenceLine, id);

            polyLineShapeFileWriter.CopyToFeature(mapLineData);

            polyLineShapeFileWriter.SaveAs(filePath);
        }

        /// <summary>
        /// Create a new instance of <see cref="MapLineData"/> from a reference line and a traject id.
        /// </summary>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> supplying the geometry information 
        /// for the new <see cref="MapLineData"/> object.</param>
        /// <param name="id">The id of the assessment section to which the reference line is associated.</param>
        /// <returns>A new instance of <see cref="MapLineData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/> or <paramref name="id"/> 
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty or consists only of whitespace.</exception>
        private static MapLineData CreateMapLineData(ReferenceLine referenceLine, string id)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(Resources.ReferenceLineWriter_CreateMapLineData_Traject_cannot_be_empty, nameof(id));
            }

            var referenceLineGeometry = new MapGeometry(
                new List<IEnumerable<Point2D>>
                {
                    referenceLine.Points
                });

            var mapFeature = new MapFeature(new[]
            {
                referenceLineGeometry
            });

            mapFeature.MetaData.Add(Resources.ReferenceLineWriter_CreateMapLineData_Traject_id, id);

            return new MapLineData(RingtoetsCommonDataResources.ReferenceLine_DisplayName)
            {
                Features = new[]
                {
                    mapFeature
                }
            };
        }
    }
}