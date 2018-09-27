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
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// Enumeration that defines the possible failure mechanism categories.
    /// </summary>
    public enum FailureMechanismCategoryType
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName))]
        MechanismSpecificFactorizedSignalingNorm = 1,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName))]
        MechanismSpecificSignalingNorm = 2,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName))]
        MechanismSpecificLowerLimitNorm = 3,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName))]
        LowerLimitNorm = 4,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName))]
        FactorizedLowerLimitNorm = 5
    }
}