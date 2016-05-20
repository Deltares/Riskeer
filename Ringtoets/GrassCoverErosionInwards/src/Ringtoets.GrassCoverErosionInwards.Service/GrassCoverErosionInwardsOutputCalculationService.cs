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
using MathNet.Numerics.Distributions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Integration.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// This class is responsible for calculating the parameters required for <see cref="GrassCoverErosionInwardsOutput"/>.
    /// </summary>
    public class GrassCoverErosionInwardsOutputCalculationService
    {
        // Inputs
        private readonly double norm;
        private readonly double contribution;
        private readonly int lengthEffectN;
        private readonly double reliability;

        // Results
        private double requiredProbability;
        private double probability;
        private double requiredReliability;
        private double factorOfSafety;

        private GrassCoverErosionInwardsOutputCalculationService(double norm, double contribution, int lengthEffectN, double reliability)
        {
            this.norm = norm;
            this.contribution = contribution/100;
            this.lengthEffectN = lengthEffectN;
            this.reliability = reliability;
        }

        /// <summary>
        /// Calculates the <see cref="GrassCoverErosionInwardsOutput"/> given the <paramref name="calculation"/>, <paramref name="norm"/>, and <paramref name="reliability"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used.</param>
        /// <param name="contribution">The amount of contribution as a percentage [0-100] for the <see cref="IFailureMechanism"/> as part of the overall verdict. </param>
        /// <param name="norm">The norm which has been defined on the <see cref="AssessmentSection"/>.</param>
        /// <param name="reliability">The reliability result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static void Calculate(GrassCoverErosionInwardsCalculation calculation, double norm, double contribution, double reliability)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
            var calculator = new GrassCoverErosionInwardsOutputCalculationService(norm, contribution, calculation.NormProbabilityInput.N, reliability);

            calculator.Calculate();

            calculation.Output = new GrassCoverErosionInwardsOutput(1/calculator.requiredProbability,
                                                                    calculator.requiredReliability,
                                                                    1/calculator.probability,
                                                                    calculator.reliability,
                                                                    calculator.factorOfSafety);
        }

        private void Calculate()
        {
            CalculateReliability();

            CalculateRequiredReliability();

            factorOfSafety = FactorOfSafety(reliability, requiredReliability);
        }

        private void CalculateReliability()
        {
            requiredProbability = AllowedCrossSectionProbability(contribution, norm, lengthEffectN);
            probability = AllowedCrossSectionProbability(reliability);
        }

        private void CalculateRequiredReliability()
        {
            requiredReliability = RequiredReliability(requiredProbability);
        }

        #region Sub calculations

        private static double AllowedCrossSectionProbability(double probability, double contribution, double factorN)
        {
            return probability*(1/contribution)/factorN;
        }

        private static double AllowedCrossSectionProbability(double grassCoverErosionInwardsCrossSectionProbability)
        {
            return new Normal().Density(grassCoverErosionInwardsCrossSectionProbability);
        }

        private static double RequiredReliability(double allowedCrossSectionProbability)
        {
            return new Normal().InverseCumulativeDistribution(1 - allowedCrossSectionProbability);
        }

        private static double FactorOfSafety(double reliability, double requiredReliability)
        {
            return reliability/requiredReliability;
        }

        #endregion
    }
}