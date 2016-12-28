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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;

namespace Ringtoets.DuneErosion.Data
{
    /// <summary>
    /// Location of a dune.
    /// </summary>
    public class DuneLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocation"/>.
        /// </summary>
        /// <param name="id">Id of the <see cref="DuneLocation"/>.</param>
        /// <param name="name">Name of the <see cref="DuneLocation"/>.</param>
        /// <param name="coordinateX">X-coordinate of the <see cref="DuneLocation"/>.</param>
        /// <param name="coordinateY">Y-coordinate of the <see cref="DuneLocation"/>.</param>
        /// <param name="coastalAreaId">Coastal area id of the <see cref="DuneLocation"/>.</param>
        /// <param name="offset">Offset of the <see cref="DuneLocation"/>.</param>
        /// <param name="orientation">Orientation of the <see cref="DuneLocation"/>.</param>
        /// <param name="d50">D50 of the <see cref="DuneLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public DuneLocation(long id, string name, double coordinateX, double coordinateY, int coastalAreaId, double offset, double orientation, double d50)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Id = id;
            Name = name;
            Location = new Point2D(coordinateX, coordinateY);
            CoastalAreaId = coastalAreaId;
            Offset = new RoundedDouble(2, offset);
            Orientation = new RoundedDouble(1, orientation);
            D50 = new RoundedDouble(6, d50);
        }

        /// <summary>
        /// Gets the database id of the dune location.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets the name of the dune location.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the location of the dune location.
        /// </summary>
        public Point2D Location { get; private set; }

        /// <summary>
        /// Gets the coastal area id of the dune location.
        /// </summary>
        public int CoastalAreaId { get; private set; }

        /// <summary>
        /// Gets the offset of the dune location.
        /// </summary>
        public RoundedDouble Offset { get; private set; }

        /// <summary>
        /// Gets the orientation of the dune location.
        /// </summary>
        public RoundedDouble Orientation { get; private set; }

        /// <summary>
        /// Gets the D50 of the dune location.
        /// </summary>
        public RoundedDouble D50 { get; private set; }
    }
}