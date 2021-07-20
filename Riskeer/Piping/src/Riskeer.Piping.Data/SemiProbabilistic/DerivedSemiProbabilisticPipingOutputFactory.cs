// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// Factory class to create <see cref="DerivedSemiProbabilisticPipingOutput"/>.
    /// </summary>
    public static class DerivedSemiProbabilisticPipingOutputFactory
    {
        /// <summary>
        /// Creates a new <see cref="DerivedSemiProbabilisticPipingOutput"/> based on the given parameters.
        /// </summary>
        /// <param name="output">The output of a calculation.</param>
        /// <param name="failureMechanism">The failure mechanism the calculation belongs to.</param>
        /// <param name="assessmentSection">The assessment section the calculation belongs to.</param>
        /// <returns>The created <see cref="DerivedSemiProbabilisticPipingOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static DerivedSemiProbabilisticPipingOutput Create(SemiProbabilisticPipingOutput output,
                                                                  PipingFailureMechanism failureMechanism,
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

            double norm = assessmentSection.FailureMechanismContribution.Norm;

            double upliftFactorOfSafety = output.UpliftFactorOfSafety;
            double heaveFactorOfSafety = output.HeaveFactorOfSafety;
            double sellmeijerFactorOfSafety = output.SellmeijerFactorOfSafety;

            double upliftReliability = SubMechanismReliability(upliftFactorOfSafety, upliftFactors, norm);
            double upliftProbability = StatisticsConverter.ReliabilityToProbability(upliftReliability);

            double heaveReliability = SubMechanismReliability(heaveFactorOfSafety, heaveFactors, norm);
            double heaveProbability = StatisticsConverter.ReliabilityToProbability(heaveReliability);

            double sellmeijerReliability = SubMechanismReliability(sellmeijerFactorOfSafety, sellmeijerFactors, norm);
            double sellmeijerProbability = StatisticsConverter.ReliabilityToProbability(sellmeijerReliability);

            double pipingProbability = PipingProbability(upliftProbability, heaveProbability, sellmeijerProbability);
            double pipingReliability = StatisticsConverter.ProbabilityToReliability(pipingProbability);

            return new DerivedSemiProbabilisticPipingOutput(upliftFactorOfSafety, upliftReliability,
                                                            upliftProbability, heaveFactorOfSafety,
                                                            heaveReliability, heaveProbability,
                                                            sellmeijerFactorOfSafety, sellmeijerReliability,
                                                            sellmeijerProbability, pipingProbability,
                                                            pipingReliability);
        }

        /// <summary>
        /// Calculates the probability of occurrence of the piping failure mechanism.
        /// </summary>
        /// <param name="probabilityOfUplift">The calculated probability of the uplift sub mechanism.</param>
        /// <param name="probabilityOfHeave">The calculated probability of the heave sub mechanism.</param>        
        /// <param name="probabilityOfSellmeijer">The calculated probability of the Sellmeijer sub mechanism.</param>
        /// <returns>A value representing the probability of occurrence of piping.</returns>
        private static double PipingProbability(double probabilityOfUplift, double probabilityOfHeave, double probabilityOfSellmeijer)
        {
            return Math.Min(Math.Min(probabilityOfUplift, probabilityOfHeave), probabilityOfSellmeijer);
        }

        private static double SubMechanismReliability(double factorOfSafety, SubCalculationFactors factors, double norm)
        {
            double bNorm = StatisticsConverter.ProbabilityToReliability(norm);

            return (1 / factors.A) * (Math.Log(factorOfSafety / factors.B) + (factors.C * bNorm));
        }

        #region Sub calculation constants

        private struct SubCalculationFactors
        {
            public double A;
            public double B;
            public double C;
        }

        private static readonly SubCalculationFactors upliftFactors = new SubCalculationFactors
        {
            A = 0.46,
            B = 0.48,
            C = 0.27
        };

        private static readonly SubCalculationFactors heaveFactors = new SubCalculationFactors
        {
            A = 0.48,
            B = 0.37,
            C = 0.30
        };

        private static readonly SubCalculationFactors sellmeijerFactors = new SubCalculationFactors
        {
            A = 0.37,
            B = 1.04,
            C = 0.43
        };

        #endregion
    }
}