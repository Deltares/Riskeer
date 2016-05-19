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
        private double betaTrajectNorm;
        private double pTCrossAllowed;
        private double pTCrossGEKB;
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
        /// Calculates 
        /// </summary>
        /// <param name="calculation"></param>
        /// <param name="norm"></param>
        /// <param name="probability"></param>
        public static void Calculate(GrassCoverErosionInwardsCalculation calculation, double norm, double probability)
        {
            var calculator = new GrassCoverErosionInwardsOutputCalculationService(norm, calculation.NormProbabilityInput.N, probability);

            calculator.Calculate();

            calculation.Output = new GrassCoverErosionInwardsOutput(calculator.betaTrajectNorm,
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
            betaTrajectNorm = BetaTrajectNorm(norm);
            pTCrossAllowed = PTCrossAllowed(probability, norm, lengthEffectN);
            pTCrossGEKB = PTCrossGEKB();
        }

        private void CalculateRequiredReliability()
        {
            requiredReliability = BetaCrossAllowed(pTCrossAllowed);
            reliability = BetaCrossGEKB(pTCrossGEKB);
        }

        #region Sub calculations

        private static double BetaTrajectNorm(double contribution)
        {
            return new Normal().InverseCumulativeDistribution(1 - 1.0/contribution);
        }

        private static double PTCrossAllowed(double probability, double contribution, double n)
        {
            return probability*(1.0/contribution)/n;
        }

        private static double PTCrossGEKB()
        {
            return 1.0/50000;
        }

        private static double BetaCrossAllowed(double pTCrossAllowed)
        {
            return new Normal().InverseCumulativeDistribution(1 - pTCrossAllowed);
        }

        private static double BetaCrossGEKB(double pTCrossGEKB)
        {
            return new Normal().InverseCumulativeDistribution(1 - pTCrossGEKB);
        }

        private static double FactorOfSafety(double betaCrossGekb, double betaCrossAllowed)
        {
            return betaCrossGekb/betaCrossAllowed;
        }

        #endregion
    }
}