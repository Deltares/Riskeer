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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Class that holds all hydraulic boundary calculation specific output parameters.
    /// </summary>
    public class HydraulicBoundaryLocationCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationOutput"/>.
        /// </summary>
        /// <param name="result">The calculation result.</param>
        /// <param name="targetProbability">The norm used during the calculation.</param>
        /// <param name="targetReliability">The reliability index used during the calculation.</param>
        /// <param name="calculatedProbability">The calculated probability.</param>
        /// <param name="calculatedReliability">The calculated reliability.</param>
        /// <param name="calculationConvergence">The convergence status of the calculation.</param>
        /// <param name="generalResult">The general result of this output with sub mechanism illustration points.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or <paramref name="calculatedProbability"/> falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        public HydraulicBoundaryLocationCalculationOutput(double result,
                                                          double targetProbability,
                                                          double targetReliability,
                                                          double calculatedProbability,
                                                          double calculatedReliability,
                                                          CalculationConvergence calculationConvergence,
                                                          GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            ProbabilityHelper.ValidateProbability(targetProbability, nameof(targetProbability), true);
            ProbabilityHelper.ValidateProbability(calculatedProbability, nameof(calculatedProbability), true);

            Result = new RoundedDouble(2, result);

            TargetProbability = targetProbability;
            TargetReliability = new RoundedDouble(5, targetReliability);
            CalculatedProbability = calculatedProbability;
            CalculatedReliability = new RoundedDouble(5, calculatedReliability);
            CalculationConvergence = calculationConvergence;
            GeneralResult = generalResult;
        }

        /// <summary>
        /// Gets the result of the calculation.
        /// </summary>
        public RoundedDouble Result { get; }

        /// <summary>
        /// Gets the target probability.
        /// [1/year]
        /// </summary>
        public double TargetProbability { get; }

        /// <summary>
        /// Gets the target beta.
        /// [-]
        /// </summary>
        public RoundedDouble TargetReliability { get; }

        /// <summary>
        /// Gets the calculated probability.
        /// [1/year]
        /// </summary>
        public double CalculatedProbability { get; }

        /// <summary>
        /// Gets the calculated reliability.
        /// [-]
        /// </summary>
        public RoundedDouble CalculatedReliability { get; }

        /// <summary>
        /// Gets the convergence status of the calculation.
        /// [-]
        /// </summary>
        public CalculationConvergence CalculationConvergence { get; }

        /// <summary>
        /// Gets the general results with the sub mechanism illustration points.
        /// </summary>
        public GeneralResult<TopLevelSubMechanismIllustrationPoint> GeneralResult { get; }

        /// <summary>
        /// Gets the value indicating whether the output contains a general result with illustration points.
        /// </summary>
        public bool HasGeneralResult
        {
            get
            {
                return GeneralResult != null;
            }
        }
    }
}