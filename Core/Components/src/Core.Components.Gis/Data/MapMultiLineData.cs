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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// This class represents data in 2D space which is visible as a line.
    /// </summary>
    public class MapMultiLineData : MapData
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapLineData"/>.
        /// </summary>
        /// <param name="lines">A <see cref="IEnumerable{T}"/> of <see cref="Tuple{T1,T2}"/> as (X,Y) lines.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lines"/> is <c>null</c>.</exception>
        public MapMultiLineData(IEnumerable<IEnumerable<Point2D>> lines)
        {
            if (lines == null)
            {
                var message = String.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedMapData));
                throw new ArgumentNullException("lines", message);
            }
            Lines = lines.ToArray();
        }

        /// <summary>
        /// Gets all the lines in the multi line data
        /// </summary>
        public IEnumerable<Point2D>[] Lines { get; private set; }
    }
}