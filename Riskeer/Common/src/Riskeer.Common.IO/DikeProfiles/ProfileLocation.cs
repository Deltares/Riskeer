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
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.DikeProfiles
{
    /// <summary>
    /// Representation of a profile location as read from a shapefile.
    /// </summary>
    public class ProfileLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProfileLocation"/>.
        /// </summary>
        /// <param name="id">The identifier for this <see cref="ProfileLocation"/>.</param>
        /// <param name="name">The name of this <see cref="ProfileLocation"/>.</param>
        /// <param name="offset">The coordinate offset in the local coordinate system for this <see cref="ProfileLocation"/>.</param>
        /// <param name="point">The coordinates of the location as a <see cref="Point2D"/>.</param>
        /// <exception cref="ArgumentException">Thrown when: <list type="bullet">
        /// <item>The Id parameter is null.</item>
        /// <item>The Id parameter contains illegal characters.</item>
        /// <item>The Point parameter is null.</item>
        /// </list></exception>
        public ProfileLocation(string id, string name, double offset, Point2D point)
        {
            if (id == null)
            {
                throw new ArgumentException(Resources.ProfileLocation_ProfileLocation_Id_is_null);
            }

            if (!id.All(char.IsLetterOrDigit))
            {
                throw new ArgumentException(Resources.ProfileLocation_ProfileLocation_Id_is_invalid);
            }

            if (double.IsNaN(offset) || double.IsInfinity(offset))
            {
                throw new ArgumentException(Resources.ProfileLocation_ProfileLocation_X0_is_invalid);
            }

            if (point == null)
            {
                throw new ArgumentException(Resources.ProfileLocation_ProfileLocation_Point_is_null);
            }

            Id = id;
            Name = name;
            Offset = offset;
            Point = point;
        }

        /// <summary>
        /// Gets the identifier for this <see cref="ProfileLocation"/>.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of this <see cref="ProfileLocation"/>.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the coordinate offset in the local coordinate system for this <see cref="ProfileLocation"/>.
        /// </summary>
        public double Offset { get; }

        /// <summary>
        /// Gets the actual location of this <see cref="ProfileLocation"/>.
        /// </summary>
        public Point2D Point { get; }
    }
}