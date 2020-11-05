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
using Core.Common.Base.Data;
using Riskeer.Piping.InputParameterCalculation;

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// Class responsible for calculating the derived semi-probabilistic piping input.
    /// </summary>
    public static class DerivedSemiProbabilisticPipingInput
    {
        /// <summary>
        /// Gets the piezometric head at the exit point.
        /// [m]
        /// </summary>
        /// <param name="input">The input to calculate the derived piping input for.</param>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is <c>null</c>.</exception>
        /// <returns>Returns the corresponding derived input value.</returns>
        public static RoundedDouble GetPiezometricHeadExit(PipingInput input, RoundedDouble assessmentLevel)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            RoundedDouble dampingFactorExit = SemiProbabilisticPipingDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue();
            RoundedDouble phreaticLevelExit = PipingDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue();

            return new RoundedDouble(2, InputParameterCalculationService.CalculatePiezometricHeadAtExit(assessmentLevel,
                                                                                                        dampingFactorExit,
                                                                                                        phreaticLevelExit));
        }
    }
}