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
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Probability
{
    /// <summary>
    /// Extension methods for the <see cref="ScenarioConfigurationPerFailureMechanismSection"/>.
    /// </summary>
    public static class ScenarioConfigurationPerFailureMechanismSectionExtensions
    {
        /// <summary>
        /// Calculates the 'N' based on the <see cref="ScenarioConfigurationPerFailureMechanismSection"/>
        /// and the 'b' parameter to factor in the 'length effect' when determining
        /// the maximum tolerated probability of failure.
        /// </summary>
        /// <param name="configuration">The scenario configuration for the failure mechanism section.</param>
        /// <param name="b">The 'b' parameter used to factor in the 'length effect' when determining
        /// the maximum tolerated probability of failure.</param>
        /// <returns>The 'N' parameter used to factor in the 'length effect'.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="configuration"/>
        /// is <c>null</c>.</exception>
        public static double GetN(this ScenarioConfigurationPerFailureMechanismSection configuration, double b)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            return 1 + ((double) configuration.A * configuration.Section.Length) / b;
        }
    }
}