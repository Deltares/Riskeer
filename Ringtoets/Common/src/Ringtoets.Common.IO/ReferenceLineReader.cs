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
using System.Linq;

using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO;

using Ringtoets.Common.Data;

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
        public ReferenceLine ReadReferenceLine(string shapeFilePath)
        {
            using (var lineShapeReader = new LineShapeFileReader(shapeFilePath))
            {
                MapLineData lineMapData = lineShapeReader.ReadLine();

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(lineMapData.Points.Select(t => new Point2D(t.Item1, t.Item2)));

                return referenceLine;
            }
        }
    }
}