// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Riskeer.Integration.Data.Assembly
{
    /// <summary>
    /// Class containing helper methods for assembling <see cref="AssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionAssemblyHelper
    {
        /// <summary>
        /// Checks whether all correlated failure mechanisms are part of the assembly.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/> to check for.</param>
        /// <returns><c>true</c> when all correlated failure mechanisms are part of the assembly; <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        public static bool AllCorrelatedFailureMechanismsInAssembly(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return assessmentSection.GrassCoverErosionInwards.InAssembly && assessmentSection.HeightStructures.InAssembly;
        }
    }
}