// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismSectionConfiguration"/> related to creating a 
    /// <see cref="MacroStabilityInwardsFailureMechanismSectionConfigurationEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsFailureMechanismSectionConfigurationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsFailureMechanismSectionConfigurationEntity"/> based on the information of the
        /// <see cref="FailureMechanismSectionConfiguration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to create a database entity for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsFailureMechanismSectionConfigurationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/> is <c>null</c>.</exception>
        internal static MacroStabilityInwardsFailureMechanismSectionConfigurationEntity Create(this FailureMechanismSectionConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return new MacroStabilityInwardsFailureMechanismSectionConfigurationEntity
            {
                A = configuration.A
            };
        }
    }
}