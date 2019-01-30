// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Riskeer.DuneErosion.IO
{
    /// <summary>
    /// Location of a dune that is read from the shape file.
    /// </summary>
    public class ReadDuneLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadDuneLocation"/>.
        /// </summary>
        /// <param name="name">Name of the <see cref="ReadDuneLocation"/>.</param>
        /// <param name="location">The coordinate of the <see cref="ReadDuneLocation"/>.</param>
        /// <param name="coastalAreaId">Coastal area id of the <see cref="ReadDuneLocation"/>.</param>
        /// <param name="offset">Offset of the <see cref="ReadDuneLocation"/>.</param>
        /// <param name="orientation">Orientation of the <see cref="ReadDuneLocation"/>.</param>
        /// <param name="d50">D50 of the <see cref="ReadDuneLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public ReadDuneLocation(string name, Point2D location, int coastalAreaId, double offset, double orientation, double d50)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Location = location;
            CoastalAreaId = coastalAreaId;
            Offset = new RoundedDouble(2, offset);
            Orientation = new RoundedDouble(1, orientation);
            D50 = new RoundedDouble(6, d50);
        }

        /// <summary>
        /// Gets the name of the dune location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the coordinate of the dune location.
        /// </summary>
        public Point2D Location { get; }

        /// <summary>
        /// Gets the coastal area id of the dune location.
        /// </summary>
        public int CoastalAreaId { get; }

        /// <summary>
        /// Gets the offset of the dune location.
        /// </summary>
        public RoundedDouble Offset { get; }

        /// <summary>
        /// Gets the orientation of the dune location.
        /// </summary>
        public RoundedDouble Orientation { get; }

        /// <summary>
        /// Gets the D50 of the dune location.
        /// </summary>
        public RoundedDouble D50 { get; }
    }
}