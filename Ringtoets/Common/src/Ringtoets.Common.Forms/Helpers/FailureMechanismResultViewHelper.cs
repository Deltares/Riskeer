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

using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;

namespace Ringtoets.Common.Forms.Helpers
{
    /// <summary>
    /// Helper class for determining read-only states in <see cref="FailureMechanismResultView{TSectionResult,TFailureMechanism}"/>.
    /// </summary>
    public static class FailureMechanismResultViewHelper
    {
        /// <summary>
        /// Helper method that determines whether the simple assessment has passed.
        /// </summary>
        /// <param name="simpleAssessmentResult">The simple assessment result to check.</param>
        /// <returns><c>true</c> when the simple assessment is <see cref="SimpleAssessmentResultType.ProbabilityNegligible"/>
        /// or <see cref="SimpleAssessmentResultType.NotApplicable"/>, <c>false</c> otherwise.</returns>
        public static bool HasPassedSimpleAssessment(SimpleAssessmentResultType simpleAssessmentResult)
        {
            return simpleAssessmentResult == SimpleAssessmentResultType.ProbabilityNegligible
                   || simpleAssessmentResult == SimpleAssessmentResultType.NotApplicable;
        }

        /// <summary>
        /// Helper method that determines whether the simple assessment has passed.
        /// </summary>
        /// <param name="simpleAssessmentResult">The simple assessment result to check.</param>
        /// <returns><c>true</c> when the simple assessment is <see cref="SimpleAssessmentResultType.NotApplicable"/>, 
        /// <c>false</c> otherwise.</returns>
        public static bool HasPassedSimpleAssessment(SimpleAssessmentResultValidityOnlyType simpleAssessmentResult)
        {
            return simpleAssessmentResult == SimpleAssessmentResultValidityOnlyType.NotApplicable;
        }
    }
}