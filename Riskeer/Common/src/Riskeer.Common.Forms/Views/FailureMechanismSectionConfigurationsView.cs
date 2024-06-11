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
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// View for a collection of <see cref="FailureMechanismSection"/> that have a section specific N.
    /// </summary>
    public class FailureMechanismSectionConfigurationsView : FailureMechanismSectionsView
    {
        private readonly IEnumerable<FailureMechanismSectionConfiguration> sectionConfigurations;
        private readonly double b;

        private readonly RecursiveObserver<IObservableEnumerable<FailureMechanismSectionConfiguration>, FailureMechanismSectionConfiguration> sectionConfigurationsObserver;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionConfigurationsView"/>.
        /// </summary>
        /// <param name="sectionConfigurations">The collection of section configurations to be displayed in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the view belongs to.</param>
        /// <param name="b">The 'b' parameter representing the equivalent independent length to factor in the
        /// 'length effect'.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionConfigurations"/> or
        /// <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismSectionConfigurationsView(IObservableEnumerable<FailureMechanismSectionConfiguration> sectionConfigurations,
                                                         IFailureMechanism failureMechanism,
                                                         double b)
            : base(sectionConfigurations?.Select(sc => sc.Section), failureMechanism)
        {
            this.sectionConfigurations = sectionConfigurations;
            this.b = b;

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
            failureMechanismSectionsDataGridViewControl.SetDataSource(CreateRows());
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

        private IEnumerable<FailureMechanismSectionConfigurationRow> CreateRows()
        {
            double start = 0;

            var presentableFailureMechanismSections = new List<FailureMechanismSectionConfigurationRow>();

            foreach (FailureMechanismSectionConfiguration sectionConfiguration in sectionConfigurations)
            {
                double end = start + sectionConfiguration.Section.Length;

                presentableFailureMechanismSections.Add(new FailureMechanismSectionConfigurationRow(sectionConfiguration, start, end, b));

                start = end;
            }

            return presentableFailureMechanismSections.ToArray();
        }
    }
}