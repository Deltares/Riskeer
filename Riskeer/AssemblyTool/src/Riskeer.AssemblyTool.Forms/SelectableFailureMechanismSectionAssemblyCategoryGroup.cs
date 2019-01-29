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
using RiskeerCommonPrimitivesResources = Riskeer.Common.Primitives.Properties.Resources;

namespace Riskeer.AssemblyTool.Forms
{
    /// <summary>
    /// Enum defining the assembly categories that can be selected for a failure mechanism section.
    /// </summary>
    public enum SelectableFailureMechanismSectionAssemblyCategoryGroup
    {
        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_NotApplicable_DisplayName))]
        NotApplicable = 2,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_Iv_DisplayName))]
        Iv = 3,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_IIv_DisplayName))]
        IIv = 4,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_IIIv_DisplayName))]
        IIIv = 5,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_IVv_DisplayName))]
        IVv = 6,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_Vv_DisplayName))]
        Vv = 7,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_VIv_DisplayName))]
        VIv = 8,

        [ResourcesDisplayName(typeof(RiskeerCommonPrimitivesResources), nameof(RiskeerCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_VIIv_DisplayName))]
        VIIv = 9
    }
}