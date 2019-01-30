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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Riskeer.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// The phreatic line created by the Waternet calculator in the derived
    /// macro stability inwards calculation input.
    /// </summary>
    public class MacroStabilityInwardsPhreaticLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsPhreaticLine"/>.
        /// </summary>
        /// <param name="name">The name of the phreatic line.</param>
        /// <param name="geometry">The geometry points of the phreatic line.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsPhreaticLine(string name, IEnumerable<Point2D> geometry)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Name = name;
            Geometry = geometry;
        }

        /// <summary>
        /// Gets the name of the phreatic line.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the geometry points of the phreatic line.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((MacroStabilityInwardsPhreaticLine) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();

                foreach (Point2D point in Geometry)
                {
                    hashCode = (hashCode * 397) ^ point.GetHashCode();
                }

                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsPhreaticLine other)
        {
            return Name.Equals(other.Name)
                   && Geometry.SequenceEqual(other.Geometry);
        }
    }
}