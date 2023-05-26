// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    /// Enum defining the assembly groups for a failure mechanism section.
    /// </summary>
    public enum FailureMechanismSectionAssemblyGroup
    {
        /// <summary>
        /// Represents group Not Dominant.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_NotDominant_DisplayName))]
        NotDominant = 1,

        /// <summary>
        /// Represents group +III.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_III_DisplayName))]
        III = 2,

        /// <summary>
        /// Represents group +II.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_II_DisplayName))]
        II = 3,

        /// <summary>
        /// Represents group +I.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_I_DisplayName))]
        I = 4,

        /// <summary>
        /// Represents group 0.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_Zero_DisplayName))]
        Zero = 5,

        /// <summary>
        /// Represents group -I.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_IMin_DisplayName))]
        IMin = 6,

        /// <summary>
        /// Represents group -II.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_IIMin_DisplayName))]
        IIMin = 7,

        /// <summary>
        /// Represents group -III.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_IIIMin_DisplayName))]
        IIIMin = 8,

        /// <summary>
        /// Represents group Dominant.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_Dominant_DisplayName))]
        Dominant = 9,

        /// <summary>
        /// Represents group No Result.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_NoResult_DisplayName))]
        NoResult = 10,

        /// <summary>
        /// Represents group Not Relevant.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismSectionAssemblyGroup_NotRelevant_DisplayName))]
        NotRelevant = 11
    }
}