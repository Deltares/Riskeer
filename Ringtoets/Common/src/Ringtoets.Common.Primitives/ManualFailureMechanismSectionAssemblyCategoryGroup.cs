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

using Core.Common.Util.Attributes;
using RingtoetsCommonPrimitivesResources = Ringtoets.Common.Primitives.Properties.Resources;

namespace Ringtoets.Common.Primitives
{
    public enum ManualFailureMechanismSectionAssemblyCategoryGroup
    {
        /// <summary>
        /// No option has been selected for this failure
        /// mechanism section.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        /// <summary>
        /// The failure mechanism section is not applicable.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_NotApplicable_DisplayName))]
        NotApplicable = 2,

        /// <summary>
        /// Represents the assembly category Iv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_Iv_DisplayName))]
        Iv = 3,

        /// <summary>
        /// Represents the assembly category IIv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_IIv_DisplayName))]
        IIv = 4,

        /// <summary>
        /// Represents the assembly category Vv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_Vv_DisplayName))]
        Vv = 5,

        /// <summary>
        /// Represents the assembly category VIIv.
        /// </summary>
        [ResourcesDisplayName(typeof(RingtoetsCommonPrimitivesResources), nameof(RingtoetsCommonPrimitivesResources.FailureMechanismSectionAssemblyCategoryGroup_VIIv_DisplayName))]
        VIIv = 6
    }
}