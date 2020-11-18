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
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Util
{
    /// <summary>
    /// Class that contains helper methods for determining whether grass cover erosion inwards objects have illustration point results.
    /// </summary>
    public static class ProbabilisticPipingIllustrationPointsHelper
    {
        /// <summary>
        /// Determines whether a <see cref="ProbabilisticPipingCalculationScenario"/> has illustration point results.
        /// </summary>
        /// <param name="calculation">The calculation to check.</param>
        /// <returns><c>true</c> when <paramref name="calculation"/> has illustration point results, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool HasIllustrationPoints(ProbabilisticPipingCalculationScenario calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            return calculation.HasOutput && HasIllustrationPoints(calculation.Output);
        }

        /// <summary>
        /// Determines whether a collection of <see cref="ProbabilisticPipingCalculationScenario"/> contain
        /// calculations with illustration point results.
        /// </summary>
        /// <param name="calculations">The calculations to check.</param>
        /// <returns><c>true</c> when <paramref name="calculations"/> contain calculations with
        /// illustration point results, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public static bool HasIllustrationPoints(IEnumerable<ProbabilisticPipingCalculationScenario> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            return calculations.Any(HasIllustrationPoints);
        }

        private static bool HasIllustrationPoints(ProbabilisticPipingOutput output)
        {
            return output.ProfileSpecificOutput.HasGeneralResult
                   || output.SectionSpecificOutput.HasGeneralResult;
        }
    }
}