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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// The waternet created by the Waternet calculator in the derived
    /// macro stability inwards calculation input.
    /// </summary>
    public class MacroStabilityInwardsWaternet
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsWaternet"/>.
        /// </summary>
        /// <param name="phreaticLines">The collection of <see cref="MacroStabilityInwardsPhreaticLine"/></param>
        /// <param name="waternetLines">The collection of <see cref="MacroStabilityInwardsWaternetLine"/></param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsWaternet(IEnumerable<MacroStabilityInwardsPhreaticLine> phreaticLines,
                                             IEnumerable<MacroStabilityInwardsWaternetLine> waternetLines)
        {
            if (phreaticLines == null)
            {
                throw new ArgumentNullException(nameof(phreaticLines));
            }

            if (waternetLines == null)
            {
                throw new ArgumentNullException(nameof(waternetLines));
            }

            PhreaticLines = phreaticLines;
            WaternetLines = waternetLines;
        }

        /// <summary>
        /// Gets the collection of phreatic lines.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsPhreaticLine> PhreaticLines { get; }

        /// <summary>
        /// Gets the collection of waternet lines.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsWaternetLine> WaternetLines { get; }

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

            return Equals((MacroStabilityInwardsWaternet) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397;

                foreach (MacroStabilityInwardsPhreaticLine phreaticLine in PhreaticLines)
                {
                    hashCode = (hashCode * 397) ^ phreaticLine.GetHashCode();
                }

                foreach (MacroStabilityInwardsWaternetLine waternetLine in WaternetLines)
                {
                    hashCode = (hashCode * 397) ^ waternetLine.GetHashCode();
                }

                return hashCode;
            }
        }

        private bool Equals(MacroStabilityInwardsWaternet other)
        {
            return PhreaticLines.SequenceEqual(other.PhreaticLines)
                   && WaternetLines.SequenceEqual(other.WaternetLines);
        }
    }
}