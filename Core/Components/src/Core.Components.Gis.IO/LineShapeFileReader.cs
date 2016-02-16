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

using Core.Common.Utils;
using Core.Components.Gis.Data;

using DotSpatial.Data;

namespace Core.Components.Gis.IO
{
    /// <summary>
    /// Class to be used to read lines from a shape file.
    /// </summary>
    public class LineShapeFileReader : IDisposable
    {
        private readonly LineShapefile lineShapeFile;
        private int readIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="LineShapeFileReader"/> class.
        /// </summary>
        /// <param name="shapeFilePath">The shape file path.</param>
        /// <exception cref="ArgumentException">When <paramref name="shapeFilePath"/> is invalid.</exception>
        public LineShapeFileReader(string shapeFilePath)
        {
            FileUtils.ValidateFilePath(shapeFilePath);

            lineShapeFile = new LineShapefile(shapeFilePath);
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
        /// <returns>The <see cref="MapLineData"/> representing the read line shape.</returns>
        public MapLineData ReadLine()
        {
            IFeature lineFeature = lineShapeFile.Features[readIndex];

            var lineData = new MapLineData(lineFeature.Coordinates.Select(c => new Tuple<double, double>(c.X, c.Y)));
            DataTable table = lineShapeFile.GetAttributes(readIndex,1);

            var dataRow = table.Rows[0];
            for (int i = 0; i < table.Columns.Count; i++)
            {
                lineData.MetaData[table.Columns[i].ColumnName] = dataRow[i];
            }

            readIndex++;

            return lineData;
        }
    }
}