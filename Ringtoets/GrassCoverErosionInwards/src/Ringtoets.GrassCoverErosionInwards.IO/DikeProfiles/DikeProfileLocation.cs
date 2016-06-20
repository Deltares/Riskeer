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

using Core.Common.Base.Geometry;

namespace Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles
{
    /// <summary>
    /// Representation of a dike profile location as read from a shapefile.
    /// </summary>
    public class DikeProfileLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="DikeProfileLocation"/>.
        /// </summary>
        /// <param name="id">The identifier for this <see cref="DikeProfileLocation"/></param>
        /// <param name="name">The name of this <see cref="DikeProfileLocation"/></param>
        /// <param name="offset">The coordinate offset in the local coordinate system for this <see cref="DikeProfileLocation"/></param>
        /// <param name="point">The coordinates of the location as a <see cref="Point2D"/>.</param>
        public DikeProfileLocation(string id, string name, double offset, Point2D point)
        {
            Id = id;
            Name = name;
            Offset = offset;
            Point = point;
        }

        /// <summary>
        /// Gets the identifier for this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the coordinate offset in the local coordinate system for this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public double Offset { get; private set; }

        /// <summary>
        /// Gets the actual location of this <see cref="DikeProfileLocation"/>.
        /// </summary>
        public Point2D Point { get; private set; }
    }
}