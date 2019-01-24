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
using Riskeer.AssemblyTool.Data.Properties;

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Enum defining the assembly categories for an assessment section.
    /// </summary>
    public enum AssessmentSectionAssemblyCategoryGroup
    {
        /// <summary>
        /// Represents the category group GR (No result) for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        /// <summary>
        /// Represents the category group NVT (Not applicable) for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_Empty_DisplayName))]
        NotApplicable = 2,

        /// <summary>
        /// Represents the category group NGO (Not assessed) for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_NotAssessed_DisplayName))]
        NotAssessed = 3,

        /// <summary>
        /// Represents the category group A+ for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_APlus_DisplayName))]
        APlus = 4,

        /// <summary>
        /// Represents the category group A+ for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_A_DisplayName))]
        A = 5,

        /// <summary>
        /// Represents the category group B for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_B_DisplayName))]
        B = 6,

        /// <summary>
        /// Represents the category group C for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_C_DisplayName))]
        C = 7,

        /// <summary>
        /// Represents the category group D for an assessment section.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_D_DisplayName))]
        D = 8
    }
}