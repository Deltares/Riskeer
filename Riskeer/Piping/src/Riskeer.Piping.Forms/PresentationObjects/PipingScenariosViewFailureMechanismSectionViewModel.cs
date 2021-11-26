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
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Properties;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// ViewModel for a <see cref="FailureMechanismSection"/> as shown in the <see cref="PipingScenariosView"/>.
    /// </summary>
    public class PipingScenariosViewFailureMechanismSectionViewModel
    {
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="PipingScenariosViewFailureMechanismSectionViewModel"/>.
        /// </summary>
        /// <param name="failureMechanismSection">The wrapped <see cref="FailureMechanismSection"/>.</param>
        /// <param name="failureMechanism">The failure mechanism the section belongs to.</param>
        /// <param name="scenarioConfigurationPerSection">The scenario configuration that belongs to the section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingScenariosViewFailureMechanismSectionViewModel(FailureMechanismSection failureMechanismSection,
                                                                   PipingFailureMechanism failureMechanism,
                                                                   PipingScenarioConfigurationPerFailureMechanismSection scenarioConfigurationPerSection)
        {
            if (failureMechanismSection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSection));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (scenarioConfigurationPerSection == null)
            {
                throw new ArgumentNullException(nameof(scenarioConfigurationPerSection));
            }

            Section = failureMechanismSection;
            this.failureMechanism = failureMechanism;
            ScenarioConfigurationPerSection = scenarioConfigurationPerSection;
        }

        /// <summary>
        /// Gets the wrapped <see cref="FailureMechanismSection"/>.
        /// </summary>
        public FailureMechanismSection Section { get; }

        /// <summary>
        /// Gets the <see cref="PipingScenarioConfigurationPerFailureMechanismSection"/> that belongs to the section.
        /// </summary>
        public PipingScenarioConfigurationPerFailureMechanismSection ScenarioConfigurationPerSection { get; }

        public override string ToString()
        {
            string name = Section.Name;
                
            if (failureMechanism.ScenarioConfigurationType == PipingScenarioConfigurationType.PerFailureMechanismSection)
            {
                name += $" ({GetScenarioConfigurationTypeDisplayName()})";
            }

            return name;
        }

        private string GetScenarioConfigurationTypeDisplayName()
        {
            return ScenarioConfigurationPerSection.ScenarioConfigurationType == PipingScenarioConfigurationPerFailureMechanismSectionType.SemiProbabilistic
                       ? Resources.PipingScenariosViewFailureMechanismSectionViewModel_GetScenarioConfigurationTypeDisplayName_SemiProbabilistic
                       : Resources.PipingScenariosViewFailureMechanismSectionViewModel_GetScenarioConfigurationTypeDisplayName_Probabilistic;
        }
    }
}