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
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for calculating <see cref="ProbabilityAssessmentOutput"/>.
    /// </summary>
    public static class ProbabilityAssessmentService
    {
        /// <summary>
        /// Calculates the <see cref="ProbabilityAssessmentOutput"/> given <paramref name="probabilityAssessmentInput"/> and <paramref name="reliability"/>.
        /// </summary>
        /// <param name="probabilityAssessmentInput">The probability assesment input to use for the calculation.</param>
        /// <param name="reliability">The reliability to use for the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="probabilityAssessmentInput"/> is <c>null</c>.</exception>
        public static ProbabilityAssessmentOutput Calculate(ProbabilityAssessmentInput probabilityAssessmentInput, double reliability)
        {
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException("probabilityAssessmentInput");
            }

            var requiredProbability = RequiredProbability(probabilityAssessmentInput.Contribution / 100.0, probabilityAssessmentInput.Norm, probabilityAssessmentInput.N);
            var probability = ReliabilityToProbability(reliability);
            var requiredReliability = ProbabilityToReliability(requiredProbability);
            var factorOfSafety = FactorOfSafety(reliability, requiredReliability);

            return new ProbabilityAssessmentOutput(1/requiredProbability,
                                                   requiredReliability,
                                                   1/probability,
                                                   reliability,
                                                   factorOfSafety);
        }

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
    }
}