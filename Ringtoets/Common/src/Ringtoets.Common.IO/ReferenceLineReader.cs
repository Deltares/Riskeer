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
using System.IO;
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO;

using Ringtoets.Common.Data;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Common.IO
{
    /// <summary>
    /// Shapefile reader that reads a <see cref="ReferenceLine"/> based on the line feature in the file.
    /// </summary>
    public class ReferenceLineReader
    {
        /// <summary>
        /// Reads an instance of <see cref="ReferenceLine"/> from a shape file that should
        /// contain one polyline.
        /// </summary>
        /// <param name="shapeFilePath">The filepath to the shape file.</param>
        /// <returns>The reference line created from the data in the shape file.</returns>
        /// <exception cref="ArgumentException">When <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">When either:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that does not exist.</item>
        /// <item>There isn't exactly 1 polyline in the shapefile.</item>
        /// <item>Shapefile contains a multi-polyline.</item>
        /// </list>
        /// </exception>
        public ReferenceLine ReadReferenceLine(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            using (PolylineShapeFileReader lineShapeReader = OpenPolyLineShapeFile(shapeFilePath))
            {
                var lineMapData = GetReferenceLineMapData(lineShapeReader, shapeFilePath);
                return CreateReferenceLine(lineMapData);
            }
        }

        /// <summary>
        /// Opens the poly line shape file.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <returns>The reader that can be used to read the shape file.</returns>
        /// <exception cref="CriticalFileReadException">When shape file does not have line geometries.</exception>
        private static PolylineShapeFileReader OpenPolyLineShapeFile(string shapeFilePath)
        {
            try
            {
                return new PolylineShapeFileReader(shapeFilePath);
            }
            catch (CriticalFileReadException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineReader_File_must_contain_1_polyline);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Gets the reference line map data.
        /// </summary>
        /// <param name="lineShapeReader">The line shape reader.</param>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <returns></returns>
        /// <exception cref="CriticalFileReadException">
        /// When either:
        /// <list type="bullet">
        /// <item>There isn't exactly 1 polyline in the shapefile.</item>
        /// <item>Shapefile contains a multi-polyline.</item>
        /// </list>
        /// </exception>
        private static MapLineData GetReferenceLineMapData(PolylineShapeFileReader lineShapeReader, string shapeFilePath)
        {
            if (lineShapeReader.GetNumberOfLines() != 1)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineReader_File_must_contain_1_polyline);
                throw new CriticalFileReadException(message);
            }

            try
            {
                return lineShapeReader.ReadLine();
            }
            catch (ElementReadException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(RingtoetsCommonIOResources.ReferenceLineReader_File_contains_unsupported_multi_polyline);
                throw new CriticalFileReadException(message, e);
            }
        }

        private static ReferenceLine CreateReferenceLine(MapLineData lineMapData)
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(lineMapData.Points.Select(t => new Point2D(t.Item1, t.Item2)));
            return referenceLine;
        }
    }
}