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

using System;
using Core.Common.Base.Data;
using Riskeer.Piping.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingFailureMechanismSectionConfiguration"/> based on the
    /// <see cref="PipingFailureMechanismSectionConfigurationEntity"/>.
    /// </summary>
    internal static class PipingFailureMechanismSectionConfigurationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="PipingFailureMechanismSectionConfigurationEntity"/> and use the information to construct a 
        /// <see cref="PipingFailureMechanismSectionConfiguration"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismSectionConfigurationEntity"/> used to update 
        /// the <paramref name="failureMechanismSectionConfiguration"/>.</param>
        /// <param name="failureMechanismSectionConfiguration">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this PipingFailureMechanismSectionConfigurationEntity entity,
                                  PipingFailureMechanismSectionConfiguration failureMechanismSectionConfiguration)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanismSectionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionConfiguration));
            }

            failureMechanismSectionConfiguration.ScenarioConfigurationType =
                (PipingFailureMechanismSectionScenarioConfigurationType) entity.PipingScenarioConfigurationPerFailureMechanismSectionType;
            failureMechanismSectionConfiguration.A = (RoundedDouble) entity.A;
        }
    }
}