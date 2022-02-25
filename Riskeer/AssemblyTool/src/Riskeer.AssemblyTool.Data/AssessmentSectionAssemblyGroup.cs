﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.Data.Properties;

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Enum defining the assembly groups for an assessment section.
    /// </summary>
    public enum AssessmentSectionAssemblyGroup
    {
        /// <summary>
        /// Represents group GR (No result).
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        /// <summary>
        /// Represents group NVT (Not applicable).
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_Empty_DisplayName))]
        NotApplicable = 2,

        /// <summary>
        /// Represents group NGO (Not assessed).
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_NotAssessed_DisplayName))]
        NotAssessed = 3,

        /// <summary>
        /// Represents group A+.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_APlus_DisplayName))]
        APlus = 4,

        /// <summary>
        /// Represents group A.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_A_DisplayName))]
        A = 5,

        /// <summary>
        /// Represents group B.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_B_DisplayName))]
        B = 6,

        /// <summary>
        /// Represents group C.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_C_DisplayName))]
        C = 7,

        /// <summary>
        /// Represents group D.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.AssessmentSectionAssemblyCategoryGroup_D_DisplayName))]
        D = 8
    }
}