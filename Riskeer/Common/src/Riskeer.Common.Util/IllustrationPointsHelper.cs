﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Class that contains helper methods for determining whether items have illustration point results.
    /// </summary>
    public static class IllustrationPointsHelper
    {
        /// <summary>
        /// Determines whether a  <see cref="HydraulicBoundaryLocationCalculation"/> in <paramref name="calculations"/>
        /// has illustration point results.
        /// </summary>
        /// <param name="calculations">The calculations to check.</param>
        /// <returns><c>true</c> when one or more calculations have illustration point results, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public static bool HasIllustrationPoints(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            return calculations.Any(calc => calc.HasOutput && calc.Output.HasGeneralResult);
        }

        /// <summary>
        /// Determines whether an <see cref="IStructuresCalculation"/> in <paramref name="calculations"/>
        /// has illustration point results.
        /// </summary>
        /// <param name="calculations">The calculations to check.</param>
        /// <returns><c>true</c> when one or more calculations have illustration point results, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        public static bool HasIllustrationPoints(IEnumerable<IStructuresCalculation> calculations)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            return calculations.Any(HasIllustrationPoints);
        }

        /// <summary>
        /// Determines whether a <see cref="IStructuresCalculation"/> has illustration point results.
        /// </summary>
        /// <param name="calculation">The calculation to check.</param>
        /// <returns><c>true</c> when <paramref name="calculation"/> has illustration point results, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static bool HasIllustrationPoints(IStructuresCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            return calculation.HasOutput && calculation.Output.HasGeneralResult;
        }
    }
}