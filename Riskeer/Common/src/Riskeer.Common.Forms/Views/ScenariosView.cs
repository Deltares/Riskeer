﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for configuration calculation scenarios.
    /// </summary>
    /// <typeparam name="TCalculationScenario">The type of calculation scenario.</typeparam>
    /// <typeparam name="TScenarioRow">The type of the scenario row.</typeparam>
    public abstract partial class ScenariosView<TCalculationScenario, TScenarioRow> : UserControl, IView
        where TCalculationScenario : class, ICalculationScenario
        where TScenarioRow : ScenarioRow<TCalculationScenario>
    {
        private readonly IFailureMechanism failureMechanism;

        private readonly Observer failureMechanismObserver;

        /// <summary>
        /// Creates a new instance of <see cref="ScenariosView{TCalculationScenario, TScenarioRow}"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/>
        /// to get the sections from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        protected ScenariosView(CalculationGroup calculationGroup, IFailureMechanism failureMechanism)
        {
            if (calculationGroup == null)
            {
                throw new ArgumentNullException(nameof(calculationGroup));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            CalculationGroup = calculationGroup;

            this.failureMechanism = failureMechanism;

            failureMechanismObserver = new Observer(UpdateSectionsListBox)
            {
                Observable = failureMechanism
            };

            InitializeComponent();

            InitializeListBox();
            InitializeDataGridView();
            UpdateSectionsListBox();
            UpdateDataGridViewDataSource();
        }

        public object Data { get; set; }

        /// <summary>
        /// Gets the <see cref="CalculationGroup"/>.
        /// </summary>
        protected CalculationGroup CalculationGroup { get; }

        /// <summary>
        /// Gets a collection of <see cref="TScenarioRow"/>.
        /// </summary>
        /// <returns>The collection of <see cref="TScenarioRow"/>.</returns>
        protected abstract IEnumerable<TScenarioRow> GetScenarioRows();

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void UpdateDataGridViewDataSource()
        {
            if (!(listBox.SelectedItem is FailureMechanismSection))
            {
                dataGridViewControl.SetDataSource(null);
                return;
            }

            IEnumerable<TScenarioRow> scenarioRows = GetScenarioRows();
            dataGridViewControl.SetDataSource(scenarioRows);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.IsRelevant),
                Resources.ScenarioView_InitializeDataGridView_In_final_rating
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.Contribution),
                Resources.ScenarioView_InitializeDataGridView_Contribution
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.Name),
                Resources.ScenarioView_Name_DisplayName
            );
            dataGridViewControl.AddTextBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.FailureProbability),
                Resources.ScenarioView_FailureProbability_DisplayName
            );
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (failureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(failureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = failureMechanism.Sections.First();
            }
        }
    }
}