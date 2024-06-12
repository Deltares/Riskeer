// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
// 
// This file is part of DiKErnel.
// 
// DiKErnel is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with this
// program. If not, see <http://www.gnu.org/licenses/>.
// 
// All names, logos, and references to "Deltares" are registered trademarks of Stichting
// Deltares and remain full property of Stichting Deltares at all times. All rights reserved.

using System;
using Core.Common.Base;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Forms.Properties;

namespace Riskeer.Piping.Forms.Views
{
    /// <summary>
    /// View for a collection of <see cref="PipingFailureMechanismSectionConfiguration"/>.
    /// </summary>
    public class PipingFailureMechanismSectionConfigurationsView : FailureMechanismSectionConfigurationsView
        <PipingFailureMechanismSectionConfiguration, PipingFailureMechanismSectionConfigurationRow>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingFailureMechanismSectionConfigurationsView"/>.
        /// </summary>
        /// <param name="sectionConfigurations">The collection of section configurations to be displayed in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the view belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public PipingFailureMechanismSectionConfigurationsView(
            IObservableEnumerable<PipingFailureMechanismSectionConfiguration> sectionConfigurations,
            PipingFailureMechanism failureMechanism)
            : base(sectionConfigurations, failureMechanism,
                   (configuration, start, end) => new PipingFailureMechanismSectionConfigurationRow(configuration, start, end, failureMechanism.GeneralInput.B))
        {
            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(PipingFailureMechanismSectionConfigurationRow.FailureMechanismSensitiveSectionLength),
                                                                         Resources.PipingFailureMechanismSectionConfigurationsView_FailureMechanismSensitiveSectionLength_DisplayName,
                                                                         true);
        }
    }
}