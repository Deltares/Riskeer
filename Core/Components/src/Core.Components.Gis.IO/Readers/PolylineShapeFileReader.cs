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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using DotSpatial.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GisIOResources = Core.Components.Gis.IO.Properties.Resources;

namespace Core.Components.Gis.IO.Readers
{
    /// <summary>
    /// Class to be used to read polylines from a shapefile.
    /// </summary>
    public class PolylineShapeFileReader : ShapeFileReaderBase
    {
        private int readIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineShapeFileReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shapefile path.</param>
        /// <exception cref="ArgumentException">When <paramref name="shapeFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">When either:
        /// <list type="bullet">
        /// <item><paramref name="shapeFilePath"/> points to a file that doesn't exist.</item>
        /// <item>The shapefile has non-line geometries in it.</item>
        /// </list>
        /// </exception>
        public PolylineShapeFileReader(string shapeFilePath) : base(shapeFilePath)
        {
            try
            {
                ShapeFile = new LineShapefile(shapeFilePath);
            }
            catch (ArgumentException e)
            {
                string message = new FileReaderErrorMessageBuilder(shapeFilePath)
                    .Build(GisIOResources.LineShapeFileReader_File_contains_geometries_not_line);
                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Reads a line shape from the file.
        /// </summary>
        /// <returns>The <see cref="PointBasedMapData"/> representing the read line shape, or 
        /// <c>null</c> when at the end of the shapefile.</returns>
        /// <exception cref="ElementReadException">When reading a multi-line feature.</exception>
        public override PointBasedMapData ReadLine(string name = null)
        {
            if (readIndex == GetNumberOfLines())
            {
                return null;
            }

            try
            {
                IFeature lineFeature = GetFeature(readIndex);
                return ConvertSingleLineFeatureToMapLineData(lineFeature, name ?? GisIOResources.PolylineShapeFileReader_ReadLine_Line);
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
        public override IFeature GetFeature(int index)
        {
            IFeature lineFeature = ShapeFile.Features[index];
            if (lineFeature.NumGeometries > 1)
            {
                string message = new FileReaderErrorMessageBuilder(FilePath)
                    .WithLocation(string.Format(GisIOResources.ShapeFileReaderBase_GetFeature_At_shapefile_index_0_, index))
                    .Build(GisIOResources.PolylineShapeFileReader_ReadLine_Read_unsupported_multipolyline);
                throw new ElementReadException(message);
            }
            return lineFeature;
        }

        private MapLineData ConvertSingleLineFeatureToMapLineData(IFeature lineFeature, string name)
        {
            var lineData = new MapLineData(lineFeature.Coordinates.Select(c => new Point2D(c.X, c.Y)), name);
            DataTable table = ShapeFile.GetAttributes(readIndex, 1);
            DataRow dataRow = table.Rows[0];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                lineData.MetaData[table.Columns[i].ColumnName] = dataRow[i];
            }
            return lineData;
        }
    }
}