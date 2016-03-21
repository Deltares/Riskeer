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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Base.Storage;

namespace Ringtoets.HydraRing.Data
{
    /// <summary>
    /// Location of an hydraulic boundary.
    /// </summary>
    public class HydraulicBoundaryLocation : Observable, IStorable
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public HydraulicBoundaryLocation()
        {
            DesignWaterLevel = Double.NaN;
        }

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="id">Id of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="name">Name of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="coordinateX">X coordinate of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="coordinateY">Y coordinate of the <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocation(long id, string name, double coordinateX, double coordinateY)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            Id = id;
            Name = name;
            Location = new Point2D(coordinateX, coordinateY);
            DesignWaterLevel = Double.NaN;
        }

        /// <summary>
        /// Gets the database id of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets the name of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the coordinates of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public Point2D Location { get; set; }

        /// <summary>
        /// Gets the design water level of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public double DesignWaterLevel { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the storage of the class.
        /// </summary>
        public long StorageId { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}