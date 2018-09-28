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
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Factory class to create <see cref="DerivedMacroStabilityInwardsOutput"/>.
    /// </summary>
    public static class DerivedMacroStabilityInwardsOutputFactory
    {
        /// <summary>
        /// Calculates the semi-probabilistic results given a <see cref="MacroStabilityInwardsCalculation"/> with <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="output">The output of a calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the output belongs to.</param>
        /// <param name="assessmentSection">The assessment section the output belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static DerivedMacroStabilityInwardsOutput Create(MacroStabilityInwardsOutput output,
                                                                MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                IAssessmentSection assessmentSection)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput = failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput;
            double contribution = failureMechanism.Contribution / 100;
            double norm = assessmentSection.FailureMechanismContribution.Norm;

            double factorOfStability = output.FactorOfStability;
            double requiredProbability = CalculateRequiredProbability(probabilityAssessmentInput.A,
                                                                      probabilityAssessmentInput.B,
                                                                      probabilityAssessmentInput.SectionLength,
                                                                      norm,
                                                                      contribution);
            double requiredReliability = StatisticsConverter.ProbabilityToReliability(requiredProbability);

            double macroStabilityInwardsReliability = CalculateEstimatedReliability(factorOfStability, failureMechanism.GeneralInput.ModelFactor);
            double macroStabilityInwardsProbability = StatisticsConverter.ReliabilityToProbability(macroStabilityInwardsReliability);

            double macroStabilityInwardsFactorOfSafety = macroStabilityInwardsReliability / requiredReliability;

            return new DerivedMacroStabilityInwardsOutput(factorOfStability,
                                                          requiredProbability,
                                                          requiredReliability,
                                                          macroStabilityInwardsProbability,
                                                          macroStabilityInwardsReliability,
                                                          macroStabilityInwardsFactorOfSafety);
        }

        /// <summary>
        /// Calculates the required probability of the macro stability inwards failure mechanism for the complete assessment section.
        /// </summary>
        /// <param name="constantA">The constant a.</param>
        /// <param name="constantB">The constant b.</param>
        /// <param name="sectionLength">The length of the assessment section.</param>
        /// <param name="norm">The norm.</param>
        /// <param name="contribution">The contribution of macro stability inwards to the total failure.</param>
        /// <returns>A value representing the required probability.</returns>
        private static double CalculateRequiredProbability(double constantA, double constantB, double sectionLength, double norm, double contribution)
        {
            return (norm * contribution) / (1 + (constantA * sectionLength) / constantB);
        }

        /// <summary>
        /// Calculates the estimated reliability of the macro stability inwards failure mechanism 
        /// based on the stability factor and model factor.
        /// </summary>
        /// <param name="factorOfStability">The factory of stability to calculate the reliability for.</param>
        /// <param name="modelFactor">The model factor of the calculation result.</param>
        /// <returns>The estimated reliability based on the stability and model factor.</returns>
        private static double CalculateEstimatedReliability(double factorOfStability, double modelFactor)
        {
            return ((factorOfStability / modelFactor) - 0.41) / 0.15;
        }
    }
}