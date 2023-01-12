// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.Model.Enums
{
    /// <summary>
    /// Enum defining the exportable assembly groups for an assessment section.
    /// </summary>
    public enum ExportableAssessmentSectionAssemblyGroup
    {
        /// <summary>
        /// Represents group A+.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssessmentSectionAssemblyGroup_APlus_DisplayName))]
        APlus = 1,

        /// <summary>
        /// Represents group A.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssessmentSectionAssemblyGroup_A_DisplayName))]
        A = 2,

        /// <summary>
        /// Represents group B.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssessmentSectionAssemblyGroup_B_DisplayName))]
        B = 3,

        /// <summary>
        /// Represents group C.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssessmentSectionAssemblyGroup_C_DisplayName))]
        C = 4,

        /// <summary>
        /// Represents group D.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ExportableAssessmentSectionAssemblyGroup_D_DisplayName))]
        D = 5
    }
}