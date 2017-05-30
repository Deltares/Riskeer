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
    /// This class is responsible for calculating a factor of safety for macro stability inwards based on the sub calculations.
    /// </summary>
    public class MacroStabilityInwardsSemiProbabilisticCalculationService
    {
        private readonly double norm;
        private readonly double constantA;
        private readonly double constantB;
        private readonly double assessmentSectionLength;
        private readonly double contribution;

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
        private MacroStabilityInwardsSemiProbabilisticCalculationService(double norm, double constantA,
                                                                         double constantB, double assessmentSectionLength, double contribution)
        {
            this.norm = norm;
            this.constantA = constantA;
            this.constantB = constantB;
            this.assessmentSectionLength = assessmentSectionLength;
            this.contribution = contribution;
        }

        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="MacroStabilityInwardsCalculation"/> with <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="calculation">The calculation which is used as input for the semi-probabilistic assessment. If the semi-
        /// probabilistic calculation is successful, <see cref="MacroStabilityInwardsCalculation.SemiProbabilisticOutput"/> is set.</param>
        /// <param name="macroStabilityInwardsProbabilityAssessmentInput">General input that influences the probability estimate for a
        /// macro stability inwards assessment.</param>
        /// <param name="norm">The norm to assess for.</param>
        /// <param name="contribution">The contribution of macro stability inwards as a percentage (0-100) to the total of the failure probability
        /// of the assessment section.</param>
        /// <exception cref="ArgumentException">Thrown when calculation has no output from a macro stability inwards calculation.</exception>
        public static void Calculate(MacroStabilityInwardsCalculation calculation,
                                     MacroStabilityInwardsProbabilityAssessmentInput macroStabilityInwardsProbabilityAssessmentInput,
                                     double norm, double contribution)
        {
            var calculator = new MacroStabilityInwardsSemiProbabilisticCalculationService(
                norm,
                macroStabilityInwardsProbabilityAssessmentInput.A,
                macroStabilityInwardsProbabilityAssessmentInput.B,
                macroStabilityInwardsProbabilityAssessmentInput.SectionLength,
                contribution / 100);

            calculator.Calculate();

            calculation.SemiProbabilisticOutput = new MacroStabilityInwardsSemiProbabilisticOutput(
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
            CalculateMacroStabilityInwardsReliability();

            CalculateRequiredReliability();

            macroStabilityInwardsFactorOfSafety = macroStabilityInwardsReliability / requiredReliability;
        }

        /// <summary>
        /// Calculates the required reliability based on the norm and length of the assessment section and the contribution of
        /// macro stability inwards.
        /// </summary>
        private void CalculateRequiredReliability()
        {
            requiredProbability = RequiredProbability();
            requiredReliability = StatisticsConverter.ProbabilityToReliability(requiredProbability);
        }

        /// <summary>
        /// Calculates the reliability of macro stability inwards based on the factors of safety from the sub-mechanisms.
        /// </summary>
        private void CalculateMacroStabilityInwardsReliability()
        {
            macroStabilityInwardsProbability = RequiredProbability();
            macroStabilityInwardsReliability = StatisticsConverter.ProbabilityToReliability(macroStabilityInwardsProbability);
        }

        /// <summary>
        /// Calculates the required probability of the macro stability inwards failure mechanism for the complete assessment section.
        /// </summary>
        /// <returns>A value representing the required probability.</returns>
        private double RequiredProbability()
        {
            return (norm * contribution) / (1 + (constantA * assessmentSectionLength) / constantB);
        }
    }
}