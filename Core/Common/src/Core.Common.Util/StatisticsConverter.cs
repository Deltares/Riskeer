// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using MathNet.Numerics.Distributions;

namespace Core.Common.Util
{
    /// <summary>
    /// This class contains a converter related to the statistics domain.
    /// </summary>
    public static class StatisticsConverter
    {
        /// <summary>
        /// Calculates the probability from a reliability.
        /// </summary>
        /// <param name="reliability">The reliability to convert.</param>
        /// <returns>The probability.</returns>
        public static double ReliabilityToProbability(double reliability)
        {
            return Normal.CDF(0, 1, -reliability);
        }

        /// <summary>
        /// Calculates the reliability from a probability.
        /// </summary>
        /// <param name="probability">The probability to convert.</param>
        /// <returns>The reliability.</returns>
        public static double ProbabilityToReliability(double probability)
        {
            return Normal.InvCDF(0, 1, 1 - probability);
        }
    }
}