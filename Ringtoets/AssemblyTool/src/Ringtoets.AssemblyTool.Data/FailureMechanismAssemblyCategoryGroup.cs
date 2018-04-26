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
using Ringtoets.AssemblyTool.Data.Properties;

namespace Ringtoets.AssemblyTool.Data
{
    /// <summary>
    /// Enum defining the assembly categories for a failure mechanism.
    /// </summary>
    public enum FailureMechanismAssemblyCategoryGroup
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_NotApplicable_DisplayName))]
        NotApplicable = 2,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_It_DisplayName))]
        It = 3,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_IIt_DisplayName))]
        IIt = 4,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_IIIt_DisplayName))]
        IIIt = 5,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_IVt_DisplayName))]
        IVt = 6,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_Vt_DisplayName))]
        Vt = 7,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_VIt_DisplayName))]
        VIt = 8,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_VIIt_DisplayName))]
        VIIt = 9
    }
}