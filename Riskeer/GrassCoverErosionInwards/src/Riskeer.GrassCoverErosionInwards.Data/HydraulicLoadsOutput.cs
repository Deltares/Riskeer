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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probability;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Base class for the result of a hydraulic loads calculation.
    /// </summary>
    public abstract class HydraulicLoadsOutput : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLoadsOutput"/>.
        /// </summary>
        /// <param name="targetProbability">The norm used during the calculation.</param>
        /// <param name="targetReliability">The reliability index used during the calculation.</param>
        /// <param name="calculatedProbability">The calculated probability.</param>
        /// <param name="calculatedReliability">The calculated reliability index.</param>
        /// <param name="calculationConvergence">The convergence status of the calculation.</param>
        /// <param name="generalResult">The general result with the fault tree illustration points.</param> 
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetProbability"/> 
        /// or <paramref name="calculatedProbability"/> falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        protected HydraulicLoadsOutput(double targetProbability, double targetReliability,
                                       double calculatedProbability, double calculatedReliability,
                                       CalculationConvergence calculationConvergence,
                                       GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            ProbabilityHelper.ValidateProbability(targetProbability, nameof(targetProbability), true);
            ProbabilityHelper.ValidateProbability(calculatedProbability, nameof(calculatedProbability), true);

            TargetProbability = targetProbability;
            TargetReliability = new RoundedDouble(5, targetReliability);
            CalculatedProbability = calculatedProbability;
            CalculatedReliability = new RoundedDouble(5, calculatedReliability);
            CalculationConvergence = calculationConvergence;
            GeneralResult = generalResult;
        }

        /// <summary>
        /// Gets the norm used during the calculation.
        /// [1/year]
        /// </summary>
        public double TargetProbability { get; }

        /// <summary>
        /// Gets the reliability index used during the calculation.
        /// [-]
        /// </summary>
        public RoundedDouble TargetReliability { get; }

        /// <summary>
        /// Gets the calculated probability.
        /// [1/year]
        /// </summary>
        public double CalculatedProbability { get; }

        /// <summary>
        /// Gets the calculated reliability index.
        /// [-]
        /// </summary>
        public RoundedDouble CalculatedReliability { get; }

        /// <summary>
        /// Gets the convergence status of the calculation.
        /// [-]
        /// </summary>
        public CalculationConvergence CalculationConvergence { get; }

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

        /// <summary>
        /// Gets the general result with the fault tree illustration points.
        /// </summary>
        public GeneralResult<TopLevelFaultTreeIllustrationPoint> GeneralResult { get; private set; }

        public object Clone()
        {
            var clone = (HydraulicLoadsOutput) MemberwiseClone();

            if (GeneralResult != null)
            {
                clone.GeneralResult = (GeneralResult<TopLevelFaultTreeIllustrationPoint>) GeneralResult.Clone();
            }

            return clone;
        }

        /// <summary>
        /// Clears the illustration points of the output
        /// </summary>
        public void ClearIllustrationPoints()
        {
            GeneralResult = null;
        }
    }
}