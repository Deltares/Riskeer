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

using Core.Common.Util.Attributes;
using Riskeer.Common.Primitives.Properties;

namespace Riskeer.Common.Primitives
{
    /// <summary>
    /// This enum defines the possible result types for a simple assessment 
    /// on a failure mechanism section in case there is only a relevance check.
    /// </summary>
    public enum SimpleAssessmentValidityOnlyResultType
    {
        /// <summary>
        /// No option has been selected for this failure
        /// mechanism section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_None))]
        None = 1,

        /// <summary>
        /// The failure mechanism section is not applicable.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SimpleAssessmentResultType_NotApplicable))]
        NotApplicable = 2,

        /// <summary>
        /// The failure mechanism section is applicable.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SimpleAssessmentValidityOnlyResultType_Applicable))]
        Applicable = 3
    }
}