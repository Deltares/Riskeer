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

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output
{
    /// <summary>
    /// The sliding curve result of an Uplift Van calculation.
    /// </summary>
    public class UpliftVanSlidingCurveResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSlidingCurveResult"/>.
        /// </summary>
        /// <param name="leftCircle">The left circle of the curve.</param>
        /// <param name="rightCircle">The right circle of the curve.</param>
        /// <param name="slices">The slices of the curve.</param>
        /// <param name="nonIteratedHorizontalForce">The non iterated horizontal
        /// force of the curve.</param>
        /// <param name="iteratedHorizontalForce">The iterated horizontal force
        /// of the curve.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="leftCircle"/>,
        /// <paramref name="rightCircle" /> or <paramref name="slices"/> is <c>null</c>.</exception>
        internal UpliftVanSlidingCurveResult(UpliftVanSlidingCircleResult leftCircle, UpliftVanSlidingCircleResult rightCircle,
                                             IEnumerable<UpliftVanSliceResult> slices, double nonIteratedHorizontalForce, double iteratedHorizontalForce)
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
        public UpliftVanSlidingCircleResult LeftCircle { get; }

        /// <summary>
        /// Gets the right circle.
        /// </summary>
        public UpliftVanSlidingCircleResult RightCircle { get; }

        /// <summary>
        /// Gets the slices.
        /// </summary>
        public IEnumerable<UpliftVanSliceResult> Slices { get; }

        /// <summary>
        /// Gets the non iterated horizontal force.
        /// </summary>
        public double NonIteratedHorizontalForce { get; }

        /// <summary>
        /// Gets the iterated horizontal force.
        /// </summary>
        public double IteratedHorizontalForce { get; }
    }
}