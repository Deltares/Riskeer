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
using Core.Common.Base.Geometry;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Location of a hydraulic boundary.
    /// </summary>
    public class HydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="id">The id of the hydraulic boundary location.</param>
        /// <param name="name">The name of the hydraulic boundary location.</param>
        /// <param name="coordinateX">The x-coordinate of the hydraulic boundary location.</param>
        /// <param name="coordinateY">The y-coordinate of the hydraulic boundary location.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public HydraulicBoundaryLocation(long id, string name, double coordinateX, double coordinateY)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Id = id;
            Name = name;
            Location = new Point2D(coordinateX, coordinateY);
        }

        /// <summary>
        /// Gets the id of the hydraulic boundary location.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the hydraulic boundary location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the location of the hydraulic boundary.
        /// </summary>
        public Point2D Location { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}