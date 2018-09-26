// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// The macro stability inwards sliding curve.
    /// </summary>
    public class MacroStabilityInwardsSlidingCurve : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSlidingCurve"/>.
        /// </summary>
        /// <param name="leftCircle">The left circle of the curve.</param>
        /// <param name="rightCircle">The right circle of the curve.</param>
        /// <param name="slices">The slices of the curve.</param>
        /// <param name="nonIteratedHorizontalForce">The non iterated horizontal
        /// force of the curve.</param>
        /// <param name="iteratedHorizontalForce">The iterated horizontal force
        /// of the curve.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSlidingCurve(MacroStabilityInwardsSlidingCircle leftCircle, MacroStabilityInwardsSlidingCircle rightCircle,
                                                 IEnumerable<MacroStabilityInwardsSlice> slices, double nonIteratedHorizontalForce, double iteratedHorizontalForce)
        {
            if (leftCircle == null)
            {
                throw new ArgumentNullException(nameof(leftCircle));
            }

            if (rightCircle == null)
            {
                throw new ArgumentNullException(nameof(rightCircle));
            }

            if (slices == null)
            {
                throw new ArgumentNullException(nameof(slices));
            }

            LeftCircle = leftCircle;
            RightCircle = rightCircle;
            Slices = slices;
            NonIteratedHorizontalForce = nonIteratedHorizontalForce;
            IteratedHorizontalForce = iteratedHorizontalForce;
        }

        /// <summary>
        /// Gets the left circle.
        /// </summary>
        public MacroStabilityInwardsSlidingCircle LeftCircle { get; private set; }

        /// <summary>
        /// Gets the right circle.
        /// </summary>
        public MacroStabilityInwardsSlidingCircle RightCircle { get; private set; }

        /// <summary>
        /// Gets the slices.
        /// </summary>
        public IEnumerable<MacroStabilityInwardsSlice> Slices { get; private set; }

        /// <summary>
        /// Gets the non iterated horizontal force.
        /// </summary>
        public double NonIteratedHorizontalForce { get; }

        /// <summary>
        /// Gets the iterated horizontal force.
        /// </summary>
        public double IteratedHorizontalForce { get; }

        public object Clone()
        {
            var clone = (MacroStabilityInwardsSlidingCurve) MemberwiseClone();
            clone.LeftCircle = (MacroStabilityInwardsSlidingCircle) LeftCircle.Clone();
            clone.RightCircle = (MacroStabilityInwardsSlidingCircle) RightCircle.Clone();
            clone.Slices = Slices.Select(s => (MacroStabilityInwardsSlice) s.Clone()).ToArray();

            return clone;
        }
    }
}