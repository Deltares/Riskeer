// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Util.Attributes;
using Riskeer.Common.Primitives.Properties;

namespace Riskeer.Common.Primitives
{
    /// <summary>
    /// This enum defines the possible result types for a tailor made assessment 
    /// on a failure mechanism section in case only a category group is relevant.
    /// </summary>
    public enum TailorMadeAssessmentCategoryGroupResultType
    {
        /// <summary>
        /// No option has been selected for this failure
        /// mechanism section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentResultType_None))]
        None = 1,

        /// <summary>
        /// Represents the assembly category Iv (FV).
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TailorMadeAssessmentCategoryGroupResultType_Iv_FV_DisplayName))]
        Iv = 2,

        /// <summary>
        /// Represents the assembly category IIv.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyCategoryGroup_IIv_DisplayName))]
        IIv = 3,

        /// <summary>
        /// Represents the assembly category IIIv.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyCategoryGroup_IIIv_DisplayName))]
        IIIv = 4,

        /// <summary>
        /// Represents the assembly category IVv.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyCategoryGroup_IVv_DisplayName))]
        IVv = 5,

        /// <summary>
        /// Represents the assembly category Vv.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyCategoryGroup_Vv_DisplayName))]
        Vv = 6,

        /// <summary>
        /// Represents the assembly category VIv.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyCategoryGroup_VIv_DisplayName))]
        VIv = 7,

        /// <summary>
        /// Represents the assembly category VIIv (NGO).
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.TailorMadeAssessmentCategoryGroupResultType_VIIv_NGO_DisplayName))]
        VIIv = 8
    }
}