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
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// This class is responsible for calculating the parameters required for <see cref="GrassCoverErosionInwardsOutput"/>.
    /// </summary>
    public class GrassCoverErosionInwardsOutputCalculationService
    {
        // Inputs
        private readonly double norm;
        private readonly int lengthEffectN;
        private readonly double probability;

        // Results
        private double requiredProbability;
        private double pTCrossAllowed;
        private double pTCrossGrassCoverErosionInwards;
        private double requiredReliability;
        private double reliability;
        private double factorOfSafety;

        private GrassCoverErosionInwardsOutputCalculationService(double norm, int lengthEffectN, double probability)
        {
            this.norm = norm;
            this.lengthEffectN = lengthEffectN;
            this.probability = probability;
        }

        /// <summary>
        /// Calculates the <see cref="GrassCoverErosionInwardsOutput"/> given the <paramref name="calculation"/>, <paramref name="norm"/>, and <paramref name="probability"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used.</param>
        /// <param name="norm">The norm which has been defined on the assessment section.</param>
        /// <param name="probability">The probability result.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/> is <c>null</c>.</exception>
        public static void Calculate(GrassCoverErosionInwardsCalculation calculation, double norm, double probability)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException("calculation");
            }
            var calculator = new GrassCoverErosionInwardsOutputCalculationService(norm, calculation.NormProbabilityInput.N, probability);

            calculator.Calculate();

            calculation.Output = new GrassCoverErosionInwardsOutput(calculator.requiredProbability,
                                                                    calculator.requiredReliability,
                                                                    calculator.probability,
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
            requiredProbability = RequiredProbability(norm);
            pTCrossAllowed = PtCrossAllowed(probability, norm, lengthEffectN);
            pTCrossGrassCoverErosionInwards = PtCrossGrassCoverErosionInwards();
        }

        private void CalculateRequiredReliability()
        {
            requiredReliability = RequiredReliability(pTCrossAllowed);
            reliability = Reliability(pTCrossGrassCoverErosionInwards);
        }

        #region Sub calculations

        private static double RequiredProbability(double contribution)
        {
            return new Normal().InverseCumulativeDistribution(1 - 1.0/contribution);
        }

        private static double PtCrossAllowed(double probability, double contribution, double n)
        {
            return probability*(1.0/contribution)/n;
        }

        private static double PtCrossGrassCoverErosionInwards()
        {
            return 1.0/50000;
        }

        private static double RequiredReliability(double pTCrossAllowed)
        {
            return new Normal().InverseCumulativeDistribution(1 - pTCrossAllowed);
        }

        private static double Reliability(double pTCross)
        {
            return new Normal().InverseCumulativeDistribution(1 - pTCross);
        }

        private static double FactorOfSafety(double betaCrossGekb, double betaCrossAllowed)
        {
            return betaCrossGekb/betaCrossAllowed;
        }

        #endregion
    }
}