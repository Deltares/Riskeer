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

using Core.Common.Base.Geometry;

namespace Ringtoets.HydraRing.Data
{
    public class HydraulicBoundaryLocation
    {
        public HydraulicBoundaryLocation(long id, string name, double x, double y)
        {
            Id = id;
            Name = name;
            Location = new Point2D(x, y);
        }

        /// <summary>
        /// Gets the database id of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets the name of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the X-coordinate of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        public Point2D Location { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}