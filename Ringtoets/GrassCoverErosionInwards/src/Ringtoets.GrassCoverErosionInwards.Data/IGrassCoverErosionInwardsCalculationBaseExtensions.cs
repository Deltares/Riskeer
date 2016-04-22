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

using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Defines extension methods dealing with <see cref="ICalculationBase"/> instances for grass cover erosion inwards.
    /// </summary>
    public static class IGrassCoverErosionInwardsCalculationBaseExtensions
    {
        /// <summary>
        /// Recursively enumerates across over the contents of the calculation groups, 
        /// yielding the grass cover erosion inwards calculations found within the calculation group.
        /// </summary>
        /// <param name="calculationBase">The calculation item to be evaluated.</param>
        /// <returns>Returns all contained grass cover erosion inwards calculations as an enumerable result.</returns>
        public static IEnumerable<GrassCoverErosionInwardsCalculation> GetGrassCoverErosionInwardsCalculations(this ICalculationBase calculationBase)
        {
            var calculation = calculationBase as GrassCoverErosionInwardsCalculation;
            if (calculation != null)
            {
                yield return calculation;
            }
            var group = calculationBase as CalculationGroup;
            if (group != null)
            {
                foreach (GrassCoverErosionInwardsCalculation calculationInGroup in group.Children.GetGrassCoverErosionInwardsCalculations())
                {
                    yield return calculationInGroup;
                }
            }
        }

        /// <summary>
        /// Recursively enumerates across over the contents of all the calculation groups, 
        /// yielding the grass cover erosion inwards calculations found within those calculation group.
        /// </summary>
        /// <param name="grassCoverErosionInwardsCalculationBaseItems">The calculation items to be evaluated.</param>
        /// <returns>Returns all contained grass cover erosion inwards calculations as an enumerable result.</returns>
        private static IEnumerable<GrassCoverErosionInwardsCalculation> GetGrassCoverErosionInwardsCalculations(this IEnumerable<ICalculationBase> grassCoverErosionInwardsCalculationBaseItems)
        {
            return grassCoverErosionInwardsCalculationBaseItems.SelectMany(GetGrassCoverErosionInwardsCalculations);
        }
    }
}