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
        /// <param name="sectionConfiguration">The wrapped section configuration.</param>
        /// <param name="failureMechanism">The failure mechanism the section configuration belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingScenariosViewFailureMechanismSectionViewModel(PipingFailureMechanismSectionConfiguration sectionConfiguration,
                                                                   PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (sectionConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sectionConfiguration));
            }

            this.failureMechanism = failureMechanism;
            SectionConfiguration = sectionConfiguration;
        }

        /// <summary>
        /// Gets the wrapped <see cref="PipingFailureMechanismSectionConfiguration"/>.
        /// </summary>
        public PipingFailureMechanismSectionConfiguration SectionConfiguration { get; }

        public override string ToString()
        {
            string name = SectionConfiguration.Section.Name;

            if (failureMechanism.ScenarioConfigurationType == PipingFailureMechanismScenarioConfigurationType.PerFailureMechanismSection)
            {
                name += $" ({GetScenarioConfigurationTypeDisplayName()})";
            }

            return name;
        }

        private string GetScenarioConfigurationTypeDisplayName()
        {
            return SectionConfiguration.ScenarioConfigurationType == PipingFailureMechanismSectionScenarioConfigurationType.SemiProbabilistic
                       ? Resources.PipingScenariosViewFailureMechanismSectionViewModel_GetScenarioConfigurationTypeDisplayName_SemiProbabilistic
                       : Resources.PipingScenariosViewFailureMechanismSectionViewModel_GetScenarioConfigurationTypeDisplayName_Probabilistic;
        }
    }
}