﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Core.Common.Util.Attributes;
using Ringtoets.Common.Primitives.Properties;

namespace Ringtoets.Common.Primitives
{
    /// <summary>
    /// This enum defines the possible result types for a tailor made assessment 
    /// on a failure mechanism section with either a probability or detailed
    /// calculation result.
    /// </summary>
    public enum TailorMadeAssessmentProbabilityAndDetailedCalculationResultType
    {
        /// <summary>
        /// No option has been selected for this failure
        /// mechanism section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_None))]
        None = 1,

        /// <summary>
        /// The probability of failure for the failure mechanism
        /// section is negligible.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_ProbabilityNegligible))]
        ProbabilityNegligible = 2,

        /// <summary>
        /// The calculated probability for this failure
        /// mechanism section has to be entered.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_Probability))]
        Probability = 3,

        /// <summary>
        /// The assessment for this failure mechanism
        /// section was sufficient.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_Sufficient))]
        Sufficient = 4,

        /// <summary>
        /// The assessment for this failure mechanism
        /// section was insufficient.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_Insufficient))]
        Insufficient = 5,

        /// <summary>
        /// No assessment has been performed for this failure
        /// mechanism section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_NotAssessed))]
        NotAssessed = 6
    }
}