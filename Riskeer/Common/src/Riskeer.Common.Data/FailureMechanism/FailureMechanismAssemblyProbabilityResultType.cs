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
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.FailureMechanism
{
    /// <summary>
    /// Specifies the type of probability for a failure mechanism assembly result.
    /// </summary>
    public enum FailureMechanismAssemblyProbabilityResultType
    {
        /// <summary>
        /// No Probability type
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyProbabilityResultTypeNone_DisplayName))]
        None = 1,

        /// <summary>
        /// The automatically calculated probability type based on independent sections.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyProbabilityResultTypeAutomaticP1_DisplayName))]
        AutomaticP1 = 2,

        /// <summary>
        /// The automatically calculated probability type based on the worst section or profile.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyProbabilityResultTypeAutomaticP2_DisplayName))]
        AutomaticP2 = 3,

        /// <summary>
        /// The manual probability type.
        /// </summary>
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismAssemblyProbabilityResultTypeManual_DisplayName))]
        Manual = 4
    }
}