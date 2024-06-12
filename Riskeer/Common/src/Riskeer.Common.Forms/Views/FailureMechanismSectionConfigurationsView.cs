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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// View for a collection of <see cref="FailureMechanismSectionConfiguration"/>.
    /// </summary>
    /// <typeparam name="TFailureMechanismSectionConfiguration">The type of failure mechanism section configuration.</typeparam>
    /// <typeparam name="TFailureMechanismSectionConfigurationRow">The type of failure mechanism section configuration row.</typeparam>
    public class FailureMechanismSectionConfigurationsView<TFailureMechanismSectionConfiguration, TFailureMechanismSectionConfigurationRow> : FailureMechanismSectionsView
        where TFailureMechanismSectionConfiguration : FailureMechanismSectionConfiguration
        where TFailureMechanismSectionConfigurationRow : FailureMechanismSectionConfigurationRow
    {
        private readonly IEnumerable<TFailureMechanismSectionConfiguration> sectionConfigurations;
        private readonly Func<TFailureMechanismSectionConfiguration, double, double, TFailureMechanismSectionConfigurationRow> createRowFunc;

        private readonly RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration> sectionConfigurationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionsView"/>.
        /// </summary>
        /// <param name="sectionConfigurations">The collection of section configurations to be displayed in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the view belongs to.</param>
        /// <param name="createRowFunc">The function to create the rows with.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionConfigurationsView(IObservableEnumerable<TFailureMechanismSectionConfiguration> sectionConfigurations,
                                                         IFailureMechanism failureMechanism,
                                                         Func<TFailureMechanismSectionConfiguration, double, double, TFailureMechanismSectionConfigurationRow> createRowFunc)
            : base(sectionConfigurations?.Select(sc => sc.Section), failureMechanism)
        {
            if (createRowFunc == null)
            {
                throw new ArgumentNullException(nameof(createRowFunc));
            }

            this.sectionConfigurations = sectionConfigurations;
            this.createRowFunc = createRowFunc;

            sectionConfigurationsObserver = new RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration>(
                UpdateDataGridViewControl, c => c)
            {
                Observable = sectionConfigurations
            };

            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionConfigurationRow.A),
                                                                         Resources.FailureMechanismSectionConfigurationsView_Parameter_A_DisplayName);

            failureMechanismSectionsDataGridViewControl.AddTextBoxColumn(nameof(FailureMechanismSectionConfigurationRow.N),
                                                                         Resources.FailureMechanismSectionConfigurationsView_NRoundedSection_DisplayName,
                                                                         true);
        }

        protected override void SetDataGridViewControlData()
        {
            failureMechanismSectionsDataGridViewControl.SetDataSource(FailureMechanismSectionPresentationHelper.CreatePresentableFailureMechanismSectionConfigurations(
                                                                          sectionConfigurations,
                                                                          createRowFunc));
        }

        protected override void Dispose(bool disposing)
        {
            sectionConfigurationsObserver?.Dispose();
            base.Dispose(disposing);
        }

        private void UpdateDataGridViewControl()
        {
            failureMechanismSectionsDataGridViewControl.Refresh();
        }
    }
}