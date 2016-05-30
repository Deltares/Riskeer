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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// This class is responsible for calculating the parameters required for <see cref="ProbabilisticOutput"/>.
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
        /// Calculates the <see cref="ProbabilisticOutput"/> given the <paramref name="calculation"/> and <paramref name="reliability"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used.</param>
        /// <param name="reliability">The reliability result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static void Calculate(GrassCoverErosionInwardsCalculation calculation, double reliability)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
            var calculator = new GrassCoverErosionInwardsOutputCalculationService(
                calculation.ProbabilityAssessmentInput.Norm,
                calculation.ProbabilityAssessmentInput.Contribution,
                calculation.ProbabilityAssessmentInput.N,
                reliability);

            calculator.Calculate();

            calculation.Output = new ProbabilisticOutput(1/calculator.requiredProbability,
                                                         calculator.requiredReliability,
                                                         1/calculator.probability,
                                                         calculator.reliability,
                                                         calculator.factorOfSafety);
        }

        private void Calculate()
        {
            CalculateProbability();

            CalculateRequiredReliability();

            factorOfSafety = FactorOfSafety(reliability, requiredReliability);
        }

        private void CalculateProbability()
        {
            requiredProbability = RequiredProbability(contribution, norm, lengthEffectN);
            probability = ReliabilityToProbability(reliability);
        }

        private void CalculateRequiredReliability()
        {
            requiredReliability = ProbabilityToReliability(requiredProbability);
        }

        #region Sub calculations

        private static double RequiredProbability(double contribution, double norm, double lengthEffectN)
        {
            return contribution*(1/norm)/lengthEffectN;
        }

        private static double ReliabilityToProbability(double reliability)
        {
            return Normal.CDF(0, 1, -reliability);
        }

        private static double ProbabilityToReliability(double probability)
        {
            return Normal.InvCDF(0, 1, 1 - probability);
        }

        private static double FactorOfSafety(double reliability, double requiredReliability)
        {
            return reliability/requiredReliability;
        }

        #endregion
    }
}