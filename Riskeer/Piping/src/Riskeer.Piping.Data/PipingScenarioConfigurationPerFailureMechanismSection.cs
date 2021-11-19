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

using System;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// This class holds the information of the scenario configuration of the <see cref="FailureMechanismSection"/>.
    /// </summary>
    public class PipingScenarioConfigurationPerFailureMechanismSection
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/>.
        /// </summary>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to get the scenario configuration from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="section"/> is <c>null</c>.</exception>
        public PipingScenarioConfigurationPerFailureMechanismSection(FailureMechanismSection section)
        {
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Section = section;
            ScenarioConfigurationType = PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
        }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the scenario configuration.
        /// </summary>
        public PipingScenarioConfigurationPerFailureMechanismSectionType ScenarioConfigurationType { get; set; }
    }
}