﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for calculating <see cref="ProbabilityAssessmentOutput"/>.
    /// </summary>
    public static class ProbabilityAssessmentService
    {
        /// <summary>
        /// Calculates <see cref="ProbabilityAssessmentOutput"/> based on the provided parameters.
        /// </summary>
        /// <param name="norm">The return period to assess for.</param>
        /// <param name="contribution">The contribution of the failure mechanism as a percentage (0-100)
        /// to the total of the failure probability of the assessment section.</param>
        /// <param name="lengthEffectN">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <param name="reliability">The reliability to use for the calculation.</param>
        /// <returns>The calculated <see cref="ProbabilityAssessmentOutput"/>.</returns>
        public static ProbabilityAssessmentOutput Calculate(int norm, double contribution, double lengthEffectN, double reliability)
        {
            var requiredProbability = RequiredProbability(contribution/100.0, norm, lengthEffectN);
            var probability = ReliabilityToProbability(reliability);
            var requiredReliability = ProbabilityToReliability(requiredProbability);
            var factorOfSafety = FactorOfSafety(reliability, requiredReliability);

            return new ProbabilityAssessmentOutput(requiredProbability,
                                                   requiredReliability,
                                                   probability,
                                                   reliability,
                                                   factorOfSafety);
        }

        private static double RequiredProbability(double contribution, int norm, double lengthEffectN)
        {
            return contribution*(1.0/norm)/lengthEffectN;
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