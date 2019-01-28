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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// The waternet line created by the Waternet calculator in the derived
    /// macro stability inwards calculation input.
    /// </summary>
    public class MacroStabilityInwardsWaternetLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsWaternetLine"/>.
        /// </summary>
        /// <param name="name">The name of the waternet line.</param>
        /// <param name="geometry">The geometry points of the waternet line.</param>
        /// <param name="phreaticLine">The associated phreatic line.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsWaternetLine(string name, IEnumerable<Point2D> geometry, MacroStabilityInwardsPhreaticLine phreaticLine)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (phreaticLine == null)
            {
                throw new ArgumentNullException(nameof(phreaticLine));
            }

            Name = name;
            Geometry = geometry;
            PhreaticLine = phreaticLine;
        }

        /// <summary>
        /// Gets the name of the waternet line.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the geometry points of the waternet line.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the associated phreatic line.
        /// </summary>
        public MacroStabilityInwardsPhreaticLine PhreaticLine { get; }

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

            return Equals((MacroStabilityInwardsWaternetLine) obj);
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

                hashCode = (hashCode * 397) ^ PhreaticLine.GetHashCode();

                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsWaternetLine other)
        {
            return Name.Equals(other.Name)
                   && PhreaticLine.Equals(other.PhreaticLine)
                   && Geometry.SequenceEqual(other.Geometry);
        }
    }
}