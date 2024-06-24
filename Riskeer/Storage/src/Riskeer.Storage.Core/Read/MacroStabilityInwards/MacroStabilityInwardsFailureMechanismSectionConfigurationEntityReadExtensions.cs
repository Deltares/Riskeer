﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="FailureMechanismSectionConfiguration"/> based on the
    /// <see cref="MacroStabilityInwardsFailureMechanismSectionConfigurationEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsFailureMechanismSectionConfigurationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsFailureMechanismSectionConfigurationEntity"/> and use the information to construct a 
        /// <see cref="FailureMechanismSectionConfiguration"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsFailureMechanismSectionConfigurationEntity"/> used to update 
        /// the <paramref name="failureMechanismSectionConfiguration"/>.</param>
        /// <param name="failureMechanismSectionConfiguration">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this MacroStabilityInwardsFailureMechanismSectionConfigurationEntity entity,
                                  FailureMechanismSectionConfiguration failureMechanismSectionConfiguration)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanismSectionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionConfiguration));
            }

            failureMechanismSectionConfiguration.A = (RoundedDouble) entity.A;
        }
    }
}