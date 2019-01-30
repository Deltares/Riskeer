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
using Core.Common.Base.Data;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// The Uplift Van calculation grid of a macro stability calculation.
    /// </summary>
    public class MacroStabilityInwardsSlipPlaneUpliftVan : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSlipPlaneUpliftVan"/>.
        /// </summary>
        /// <param name="leftGrid">The left grid of the result.</param>
        /// <param name="rightGrid">The right grid of the result.</param>
        /// <param name="tangentLines">The tangent lines of the result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSlipPlaneUpliftVan(MacroStabilityInwardsGrid leftGrid,
                                                       MacroStabilityInwardsGrid rightGrid,
                                                       IEnumerable<RoundedDouble> tangentLines)
        {
            if (leftGrid == null)
            {
                throw new ArgumentNullException(nameof(leftGrid));
            }

            if (rightGrid == null)
            {
                throw new ArgumentNullException(nameof(rightGrid));
            }

            if (tangentLines == null)
            {
                throw new ArgumentNullException(nameof(tangentLines));
            }

            LeftGrid = leftGrid;
            RightGrid = rightGrid;
            TangentLines = tangentLines.Select(tangentLine => new RoundedDouble(2, tangentLine)).ToArray();
        }

        /// <summary>
        /// Gets the left grid result.
        /// </summary>
        public MacroStabilityInwardsGrid LeftGrid { get; private set; }

        /// <summary>
        /// Gets the right grid result.
        /// </summary>
        public MacroStabilityInwardsGrid RightGrid { get; private set; }

        /// <summary>
        /// Gets the tangent lines result.
        /// </summary>
        public IEnumerable<RoundedDouble> TangentLines { get; private set; }

        public object Clone()
        {
            var clone = (MacroStabilityInwardsSlipPlaneUpliftVan) MemberwiseClone();
            clone.LeftGrid = (MacroStabilityInwardsGrid) LeftGrid.Clone();
            clone.RightGrid = (MacroStabilityInwardsGrid) RightGrid.Clone();
            clone.TangentLines = TangentLines.ToArray();

            return clone;
        }
    }
}