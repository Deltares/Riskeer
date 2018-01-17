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

namespace Ringtoets.Common.Data.Probability
{
    /// <summary>
    /// Extension methods for <see cref="IProbabilityAssessmentInput"/> objects.
    /// </summary>
    public static class IProbabilityAssessmentInputExtensions
    {
        /// <summary>
        /// Calculates the section specific N based on the general probability assessment input
        /// and the length of the section.
        /// </summary>
        /// <param name="probabilityAssessmentInput">The probability assessment input parameters.</param>
        /// <param name="length">The length of the section in meters.</param>
        /// <returns>The section specific N.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="probabilityAssessmentInput"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the value of <see cref="IProbabilityAssessmentInput.B"/>
        /// is equal to 0.</exception>
        public static double GetSectionSpecificN(this IProbabilityAssessmentInput probabilityAssessmentInput, double length)
        {
            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentInput));
            }

            return 1 + (probabilityAssessmentInput.A * length) / probabilityAssessmentInput.B;
        }
    }
}