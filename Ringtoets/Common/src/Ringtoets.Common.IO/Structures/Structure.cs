﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// Representation of a structure as read from a shapefile.
    /// </summary>
    public class Structure
    {
        /// <summary>
        /// Creates a new instance of <see cref="Structure"/>.
        /// </summary>
        /// <param name="id">The identifier for this <see cref="Structure"/>.</param>
        /// <param name="name">The name of this <see cref="Structure"/>.</param>
        /// <param name="point">The coordinates of this <see cref="Structure"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public Structure(string id, string name, Point2D point)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }
            Id = id;
            Name = name;
            Point = point;
        }

        /// <summary>
        /// Gets the identifier for this <see cref="Structure"/>.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="Structure"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the actual location of this <see cref="Structure"/>.
        /// </summary>
        public Point2D Point { get; private set; }
    }
}