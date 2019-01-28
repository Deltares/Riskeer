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
using System.Linq;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Riskeer.Integration.Data.Assembly
{
    /// <summary>
    /// Class containing helper methods for <see cref="AssessmentSection"/>.
    /// </summary>
    public static class AssessmentSectionHelper
    {
        /// <summary>
        /// Determines whether the assessment section has section assembly results that are manually overwritten.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="AssessmentSection"/>.</param>
        /// <returns><c>true</c> if the assessment section contains section assembly results that are manually overwritten,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="assessmentSection"/> contains failure mechanisms
        /// that do not implement <see cref="IHasSectionResults{T}"/>.</exception>
        public static bool HasManualAssemblyResults(AssessmentSection assessmentSection)
        {
            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            return assessmentSection.GetFailureMechanisms()
                                    .Cast<IHasSectionResults<FailureMechanismSectionResult>>()
                                    .Any(fm => fm.IsRelevant && HasSectionResultsHelper.HasManualAssemblyResults(fm));
        }
    }
}