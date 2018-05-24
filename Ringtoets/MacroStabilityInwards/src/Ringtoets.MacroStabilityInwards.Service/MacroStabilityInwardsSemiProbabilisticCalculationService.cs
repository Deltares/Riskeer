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
using Ringtoets.MacroStabilityInwards.Data;

namespace Ringtoets.MacroStabilityInwards.Service
{
    /// <summary>
    /// This class is responsible for calculating a factor of safety for macro stability inwards.
    /// </summary>
    public class MacroStabilityInwardsSemiProbabilisticCalculationService
    {
        private readonly double norm;
        private readonly double constantA;
        private readonly double constantB;
        private readonly double assessmentSectionLength;
        private readonly double contribution;
        private readonly double stabilityFactor;
        private readonly double modelFactor;

        private double macroStabilityInwardsProbability;
        private double macroStabilityInwardsReliability;

        private double requiredProbability;
        private double requiredReliability;

        private double macroStabilityInwardsFactorOfSafety;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSemiProbabilisticCalculationService"/>.
        /// </summary>
        /// <param name="norm">The norm.</param>
        /// <param name="constantA">The constant a.</param>
        /// <param name="constantB">The constant b.</param>
        /// <param name="assessmentSectionLength">The length of the assessment section.</param>
        /// <param name="contribution">The contribution of macro stability inwards to the total failure.</param>
        /// <param name="stabilityFactor">The stability factor.</param>
        /// <param name="modelFactor">The model factor.</param>
        private MacroStabilityInwardsSemiProbabilisticCalculationService(double norm,
                                                                         double constantA,
                                                                         double constantB,
                                                                         double assessmentSectionLength,
                                                                         double contribution,
                                                                         double stabilityFactor,
                                                                         double modelFactor)
        {
            this.norm = norm;
            this.constantA = constantA;
            this.constantB = constantB;
            this.assessmentSectionLength = assessmentSectionLength;
            this.contribution = contribution;
            this.stabilityFactor = stabilityFactor;
            this.modelFactor = modelFactor;
        }

        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="MacroStabilityInwardsCalculation"/> with <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used as input for the semi-probabilistic assessment. If the semi-
        /// probabilistic calculation is successful, <see cref="MacroStabilityInwardsCalculation.SemiProbabilisticOutput"/> is set.</param>
        /// <param name="probabilityAssessmentInput">General input that influences the probability estimate for a
        /// macro stability inwards assessment.</param>
        /// <param name="generalInput">General input that influences the probability estimate for a
        /// macro stability inwards assessment.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <param name="contribution">The contribution of macro stability inwards as a percentage (0-100) to the total of the failure probability
        /// of the assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>, <paramref name="probabilityAssessmentInput"/>
        /// or <paramref name="generalInput"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when calculation has no output from a macro stability inwards calculation.</exception>
        public static void Calculate(MacroStabilityInwardsCalculation calculation,
                                     MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput,
                                     GeneralMacroStabilityInwardsInput generalInput,
                                     double norm, double contribution)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentInput));
            }
            if (generalInput == null)
            {
                throw new ArgumentNullException(nameof(generalInput));
            }

            var calculator = new MacroStabilityInwardsSemiProbabilisticCalculationService(
                norm,
                probabilityAssessmentInput.A,
                probabilityAssessmentInput.B,
                probabilityAssessmentInput.SectionLength,
                contribution / 100,
                calculation.Output.FactorOfStability,
                generalInput.ModelFactor);

            calculator.Calculate();

            calculation.SemiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
                calculation.Output.FactorOfStability,
                calculator.requiredProbability,
                calculator.requiredReliability,
                calculator.macroStabilityInwardsProbability,
                calculator.macroStabilityInwardsReliability,
                calculator.macroStabilityInwardsFactorOfSafety
            );
        }

        /// <summary>
        /// Performs the full semi-probabilistic calculation while setting intermediate results.
        /// </summary>
        private void Calculate()
        {
            requiredProbability = CalculateRequiredProbability();
            requiredReliability = StatisticsConverter.ProbabilityToReliability(requiredProbability);

            macroStabilityInwardsReliability = CalculateEstimatedReliability();
            macroStabilityInwardsProbability = StatisticsConverter.ReliabilityToProbability(macroStabilityInwardsReliability);

            macroStabilityInwardsFactorOfSafety = macroStabilityInwardsReliability / requiredReliability;
        }

        /// <summary>
        /// Calculates the required probability of the macro stability inwards failure mechanism for the complete assessment section.
        /// </summary>
        /// <returns>A value representing the required probability.</returns>
        private double CalculateRequiredProbability()
        {
            return (norm * contribution) / (1 + (constantA * assessmentSectionLength) / constantB);
        }

        /// <summary>
        /// Calculates the estimated reliability of the macro stability inwards failure mechanism 
        /// based on the stability factor.
        /// </summary>
        /// <returns>The estimated reliability based on the stability factor.</returns>
        private double CalculateEstimatedReliability()
        {
            return ((stabilityFactor / modelFactor) - 0.41) / 0.15;
        }
    }
}