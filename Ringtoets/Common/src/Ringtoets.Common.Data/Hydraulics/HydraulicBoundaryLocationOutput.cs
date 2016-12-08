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

using System;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// This class contains the result a calculation for a <see cref="HydraulicBoundaryLocation"/>.
    /// </summary>
    public class HydraulicBoundaryLocationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationOutput"/>.
        /// </summary>
        /// <param name="result">The calculation result.</param>
        /// <param name="targetProbability">The norm used during the calculation.</param>
        /// <param name="targetReliability">The reliability index used during the calculation.</param>
        /// <param name="calculatedProbability">the calculated probability.</param>
        /// <param name="calculatedReliability">The calculated reliability.</param>
        /// <param name="calculationConvergence">The convergence status of the calculation.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or <paramref name="calculatedProbability"/> falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        public HydraulicBoundaryLocationOutput(double result, double targetProbability, double targetReliability,
                                               double calculatedProbability, double calculatedReliability,
                                               CalculationConvergence calculationConvergence)
        {
            if (!IsValidProbability(targetProbability))
            {
                throw new ArgumentOutOfRangeException("targetProbability", targetProbability,
                                                      Resources.Probability_Must_be_in_range_zero_to_one);
            }
            if (!IsValidProbability(calculatedProbability))
            {
                throw new ArgumentOutOfRangeException("calculatedProbability", calculatedProbability,
                                                      Resources.Probability_Must_be_in_range_zero_to_one);
            }

            Result = new RoundedDouble(2, result);

            TargetProbability = targetProbability;
            TargetReliability = new RoundedDouble(5, targetReliability);
            CalculatedProbability = calculatedProbability;
            CalculatedReliability = new RoundedDouble(5, calculatedReliability);
            CalculationConvergence = calculationConvergence;
        }

        /// <summary>
        /// Gets or sets the result of the calculation.
        /// </summary>
        public RoundedDouble Result { get; private set; }

        /// <summary>
        /// Gets the target probability.
        /// [1/year]
        /// </summary>
        public double TargetProbability { get; private set; }

        /// <summary>
        /// Gets the target beta.
        /// [-]
        /// </summary>
        public RoundedDouble TargetReliability { get; private set; }

        /// <summary>
        /// Gets the calculated probability.
        /// [1/year]
        /// </summary>
        public double CalculatedProbability { get; private set; }

        /// <summary>
        /// Gets the calculated reliability.
        /// [-]
        /// </summary>
        public RoundedDouble CalculatedReliability { get; private set; }

        /// <summary>
        /// Gets or sets the convergence status of the calculation.
        /// [-]
        /// </summary>
        public CalculationConvergence CalculationConvergence { get; set; }

        private static bool IsValidProbability(double probability)
        {
            return double.IsNaN(probability) || (0.0 <= probability && probability <= 1.0);
        }
    }
}