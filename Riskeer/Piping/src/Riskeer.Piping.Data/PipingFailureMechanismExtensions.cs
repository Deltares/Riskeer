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
using System.Linq;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Extension methods for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public static class PipingFailureMechanismExtensions
    {
        /// <summary>
        /// Determines whether the scenario configuration is semi-probabilistic based on the input arguments.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> to determine the scenario configuration for.</param>
        /// <param name="scenarioConfigurationForSection">The <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/>
        /// to determine the scenario configuration with.</param>
        /// <returns><c>true</c> if the scenario configuration is semi-probabilistic, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        public static bool ScenarioConfigurationTypeIsSemiProbabilistic(
            this PipingFailureMechanism failureMechanism, PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationForSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (scenarioConfigurationForSection == null)
            {
                throw new ArgumentNullException(nameof(scenarioConfigurationForSection));
            }

            return failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.SemiProbabilistic
                   || failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.PerFailureMechanismSection
                   && scenarioConfigurationForSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic;
        }

        /// <summary>
        /// Gets the <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/> for a given <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="PipingFailureMechanism"/> that contains
        /// the <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/>.</param>
        /// <param name="sectionResult">The <see cref="FailureMechanismSectionResult"/> to retrieve the
        /// <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/> for.</param>
        /// <returns>The <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/> belonging to the <paramref name="sectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        public static PipingScenarioConfigurationPerFailureMechanismSection GetScenarioConfigurationForSection(
            this PipingFailureMechanism failureMechanism,
            FailureMechanismSectionResult sectionResult)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            return failureMechanism.ScenarioConfigurationsPerFailureMechanismSection
                                   .Single(sc => sc.Section.StartPoint.Equals(sectionResult.Section.StartPoint)
                                                 && sc.Section.EndPoint.Equals(sectionResult.Section.EndPoint));
        }
    }
}