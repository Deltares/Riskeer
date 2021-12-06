// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.AssemblyTool.Forms.Properties;

namespace Riskeer.AssemblyTool.Forms
{
    /// <summary>
    /// Enum defining the assembly groups to display for a failure mechanism section.
    /// </summary>
    public enum DisplayFailureMechanismSectionAssemblyGroup
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_ND_DisplayName))]
        ND = 1,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_III_DisplayName))]
        III = 2,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_II_DisplayName))]
        II = 3,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_I_DisplayName))]
        I = 4,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_ZeroPlus_DisplayName))]
        ZeroPlus = 5,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_Zero_DisplayName))]
        Zero = 6,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_IMin_DisplayName))]
        IMin = 7,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_IIMin_DisplayName))]
        IIMin = 8,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_IIIMin_DisplayName))]
        IIIMin = 9,
        
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_D_DisplayName))]
        D = 10,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.DisplayFailureMechanismSectionAssemblyGroup_GR_DisplayName))]
        GR = 11
    }
}