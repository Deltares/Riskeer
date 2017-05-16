// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Utils;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service.Properties;

namespace Ringtoets.Piping.Service
{
    /// <summary>
    /// This class is responsible for calculating a factor of safety for piping based on the sub calculations.
    /// </summary>
    public class PipingSemiProbabilisticCalculationService
    {
        // Inputs
        private readonly double upliftFactorOfSafety;
        private readonly double heaveFactorOfSafety;
        private readonly double sellmeijerFactorOfSafety;

        private readonly double norm;
        private readonly double constantA;
        private readonly double constantB;
        private readonly double assessmentSectionLength;
        private readonly double contribution;

        // Intermediate results
        private double heaveReliability;
        private double upliftReliability;
        private double sellmeijerReliability;

        private double heaveProbability;
        private double upliftProbability;
        private double sellmeijerProbability;

        private double pipingProbability;
        private double pipingReliability;

        private double requiredProbability;
        private double requiredReliability;

        private double pipingFactorOfSafety;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSemiProbabilisticCalculationService"/>.
        /// </summary>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub calculation.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub calculation.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub calculation.</param>
        /// <param name="norm">The norm.</param>
        /// <param name="constantA">The constant a.</param>
        /// <param name="constantB">The constant b.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <param name="contribution">The contribution of piping to the total failure.</param>
        private PipingSemiProbabilisticCalculationService(double upliftFactorOfSafety, double heaveFactorOfSafety,
                                                          double sellmeijerFactorOfSafety, double norm, double constantA,
                                                          double constantB, double assessmentSectionLength, double contribution)
        {
            this.heaveFactorOfSafety = heaveFactorOfSafety;
            this.upliftFactorOfSafety = upliftFactorOfSafety;
            this.sellmeijerFactorOfSafety = sellmeijerFactorOfSafety;
            this.norm = norm;
            this.constantA = constantA;
            this.constantB = constantB;
            this.assessmentSectionLength = assessmentSectionLength;
            this.contribution = contribution;
        }

        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="PipingCalculation"/> with <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used as input for the semi-probabilistic assessment. If the semi-
        /// probabilistic calculation is successful, <see cref="PipingCalculation.SemiProbabilisticOutput"/> is set.</param>
        /// <param name="pipingProbabilityAssessmentInput">General input that influences the probability estimate for a piping
        /// assessment.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <param name="contribution">The contribution of piping as a percentage (0-100) to the total of the failure probability
        /// of the assessment section.</param>
        /// <exception cref="ArgumentException">Thrown when calculation has no output from a piping calculation.</exception>
        public static void Calculate(PipingCalculation calculation, PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput,
                                     double norm, double contribution)
        {
            ValidateOutputOnCalculation(calculation);

            PipingOutput pipingOutput = calculation.Output;

            var calculator = new PipingSemiProbabilisticCalculationService(
                pipingOutput.UpliftFactorOfSafety,
                pipingOutput.HeaveFactorOfSafety,
                pipingOutput.SellmeijerFactorOfSafety,
                norm,
                pipingProbabilityAssessmentInput.A,
                pipingProbabilityAssessmentInput.B,
                pipingProbabilityAssessmentInput.SectionLength,
                contribution / 100);

            calculator.Calculate();

            calculation.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(
                calculator.upliftFactorOfSafety,
                calculator.upliftReliability,
                calculator.upliftProbability,
                calculator.heaveFactorOfSafety,
                calculator.heaveReliability,
                calculator.heaveProbability,
                calculator.sellmeijerFactorOfSafety,
                calculator.sellmeijerReliability,
                calculator.sellmeijerProbability,
                calculator.requiredProbability,
                calculator.requiredReliability,
                calculator.pipingProbability,
                calculator.pipingReliability,
                calculator.pipingFactorOfSafety
            );
        }

        /// <summary>
        /// Performs the full semi-probabilistic calculation while setting intermediate results.
        /// </summary>
        private void Calculate()
        {
            CalculatePipingReliability();

            CalculateRequiredReliability();

            pipingFactorOfSafety = pipingReliability / requiredReliability;
        }

        /// <summary>
        /// Calculates the required reliability based on the norm and length of the assessment section and the contribution of piping.
        /// </summary>
        private void CalculateRequiredReliability()
        {
            requiredProbability = RequiredProbability();
            requiredReliability = StatisticsConverter.ProbabilityToReliability(requiredProbability);
        }

        /// <summary>
        /// Calculates the reliability of piping based on the factors of safety from the sub-mechanisms.
        /// </summary>
        private void CalculatePipingReliability()
        {
            upliftReliability = SubMechanismReliability(upliftFactorOfSafety, upliftFactors);
            upliftProbability = StatisticsConverter.ReliabilityToProbability(upliftReliability);

            heaveReliability = SubMechanismReliability(heaveFactorOfSafety, heaveFactors);
            heaveProbability = StatisticsConverter.ReliabilityToProbability(heaveReliability);

            sellmeijerReliability = SubMechanismReliability(sellmeijerFactorOfSafety, sellmeijerFactors);
            sellmeijerProbability = StatisticsConverter.ReliabilityToProbability(sellmeijerReliability);

            pipingProbability = PipingProbability(upliftProbability, heaveProbability, sellmeijerProbability);
            pipingReliability = StatisticsConverter.ProbabilityToReliability(pipingProbability);
        }

        /// <summary>
        /// Calculates the probability of occurrence of the piping failure mechanism.
        /// </summary>
        /// <param name="probabilityOfHeave">The calculated probability of the heave sub-mechanism.</param>
        /// <param name="probabilityOfUplift">The calculated probability of the uplift sub-mechanism.</param>
        /// <param name="probabilityOfSellmeijer">The calculated probability of the Sellmeijer sub-mechanism.</param>
        /// <returns>A value representing the probability of occurrence of piping.</returns>
        private static double PipingProbability(double probabilityOfHeave, double probabilityOfUplift, double probabilityOfSellmeijer)
        {
            return Math.Min(Math.Min(probabilityOfHeave, probabilityOfUplift), probabilityOfSellmeijer);
        }

        /// <summary>
        /// Calculates the required probability of the piping failure mechanism for the complete assessment section.
        /// </summary>
        /// <returns>A value representing the required probability.</returns>
        private double RequiredProbability()
        {
            return (norm * contribution) / (1 + (constantA * assessmentSectionLength) / constantB);
        }

        private double SubMechanismReliability(double factorOfSafety, SubCalculationFactors factors)
        {
            double bNorm = StatisticsConverter.ProbabilityToReliability(norm);

            return (1 / factors.A) * (Math.Log(factorOfSafety / factors.B) + (factors.C * bNorm));
        }

        private static void ValidateOutputOnCalculation(PipingCalculation calculation)
        {
            if (calculation.Output == null)
            {
                throw new ArgumentException(Resources.PipingSemiProbabilisticCalculationService_ValidateOutputOnCalculation_Factor_of_safety_cannot_be_calculated);
            }
        }

        #region sub-calculation constants

        private struct SubCalculationFactors
        {
            public double A;
            public double B;
            public double C;
        }

        private readonly SubCalculationFactors upliftFactors = new SubCalculationFactors
        {
            A = 0.46,
            B = 0.48,
            C = 0.27
        };

        private readonly SubCalculationFactors heaveFactors = new SubCalculationFactors
        {
            A = 0.48,
            B = 0.37,
            C = 0.30
        };

        private readonly SubCalculationFactors sellmeijerFactors = new SubCalculationFactors
        {
            A = 0.37,
            B = 1.04,
            C = 0.43
        };

        #endregion
    }
}