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

namespace Riskeer.Common.Data.Probability
{
    /// <summary>
    /// Extension methods for <see cref="ProbabilityAssessmentInput"/> objects.
    /// </summary>
    public static class ProbabilityAssessmentInputExtensions
    {
        /// <summary>
        /// Calculates the N based on the general probability assessment input
        /// and the length of the section or segment.
        /// </summary>
        /// <param name="probabilityAssessmentInput">The probability assessment input parameters.</param>
        /// <param name="length">The length in meters.</param>
        /// <returns>The 'N' parameter used to factor in the 'length effect'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="probabilityAssessmentInput"/>
        /// is <c>null</c>.</exception>
        public static double GetN(this ProbabilityAssessmentInput probabilityAssessmentInput, double length)
        {
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentInput));
            }

            return 1 + (probabilityAssessmentInput.A * length) / probabilityAssessmentInput.B;
        }
    }
}