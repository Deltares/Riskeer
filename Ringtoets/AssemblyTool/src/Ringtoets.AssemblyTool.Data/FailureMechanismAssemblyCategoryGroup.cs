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
    /// Enum defining the assembly categories for a failure mechanism.
    /// </summary>
    public enum FailureMechanismAssemblyCategoryGroup
    {
        /// <summary>
        /// Represents the assembly category GR (No result) for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_None_DisplayName))]
        None = 1,

        /// <summary>
        /// Represents the assembly category NVT (Not applicable) for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_NotApplicable_DisplayName))]
        NotApplicable = 2,

        /// <summary>
        /// Represents the assembly category It for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_It_DisplayName))]
        It = 3,

        /// <summary>
        /// Represents the assembly category IIt for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_IIt_DisplayName))]
        IIt = 4,

        /// <summary>
        /// Represents the assembly category IIIt for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_IIIt_DisplayName))]
        IIIt = 5,

        /// <summary>
        /// Represents the assembly category IVt for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_IVt_DisplayName))]
        IVt = 6,

        /// <summary>
        /// Represents the assembly category Vt for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_Vt_DisplayName))]
        Vt = 7,

        /// <summary>
        /// Represents the assembly category VIt for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_VIt_DisplayName))]
        VIt = 8,

        /// <summary>
        /// Represents the assembly category VIIt for a failure mechanism.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyCategoryGroup_VIIt_DisplayName))]
        VIIt = 9
    }
}