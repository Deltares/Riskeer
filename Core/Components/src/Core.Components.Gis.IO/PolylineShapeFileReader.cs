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
using System.Data;
using System.IO;
using System.Linq;

using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;

using DotSpatial.Data;

using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GisIOResources = Core.Components.Gis.IO.Properties.Resources;

namespace Core.Components.Gis.IO
{
    /// <summary>
    /// Class to be used to read polylines from a shape file.
    /// </summary>
    public class PolylineShapeFileReader : IDisposable
    {
        private readonly string filePath;
        private readonly LineShapefile lineShapeFile;
        private int readIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineShapeFileReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException">When <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">When either:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-line geometries in it.</item>
        /// </list></exception>
        public PolylineShapeFileReader(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);
            if (!File.Exists(shapeFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            filePath = shapeFilePath;

            try
            {
                lineShapeFile = new LineShapefile(shapeFilePath);
            }
            catch (ArgumentException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(GisIOResources.LineShapeFileReader_File_contains_geometries_not_line);
                throw new CriticalFileReadException(message, e);
            }
        }

        public void Dispose()
        {
            lineShapeFile.Close();
        }

        /// <summary>
        /// Gets the number of lines in the shape file.
        /// </summary>
        public int GetNumberOfLines()
        {
            return lineShapeFile.Features.Count;
        }

        /// <summary>
        /// Reads a line shape from the file.
        /// </summary>
        /// <returns>The <see cref="MapLineData"/> representing the read line shape, or 
        /// <c>null</c> when at the end of the shape file.</returns>
        /// <exception cref="ElementReadException">When reading a multi-line feature.</exception>
        public MapLineData ReadLine()
        {
            if (readIndex == lineShapeFile.Features.Count)
            {
                return null;
            }

            try
            {
                var lineFeature = GetSingleLineFeature(readIndex);
                return ConvertSingleLineFeatureToMapLineData(lineFeature);
            }
            finally
            {
                readIndex++;
            }
        }

        /// <summary>
        /// Gets the single line feature at the given index.
        /// </summary>
        /// <param name="index">The index of which feature to retrieve.</param>
        /// <returns>The feature that consists out of 1 whole polyline.</returns>
        /// <exception cref="ElementReadException">When reading a multi-line feature.</exception>
        private IFeature GetSingleLineFeature(int index)
        {
            IFeature lineFeature = lineShapeFile.Features[index];
            if (lineFeature.NumGeometries > 1)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .WithLocation(string.Format(Properties.Resources.PolylineShapeFileReader_GetSingleLineFeature_At_shapefile_index_0_, index))
                    .Build(Properties.Resources.PolylineShapeFileReader_ReadLine_Read_unsupported_multipolyline);
                throw new ElementReadException(message);
            }
            return lineFeature;
        }

        private MapLineData ConvertSingleLineFeatureToMapLineData(IFeature lineFeature)
        {
            var lineData = new MapLineData(lineFeature.Coordinates.Select(c => new Tuple<double, double>(c.X, c.Y)));
            DataTable table = lineShapeFile.GetAttributes(readIndex, 1);
            DataRow dataRow = table.Rows[0];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                lineData.MetaData[table.Columns[i].ColumnName] = dataRow[i];
            }
            return lineData;
        }
    }
}