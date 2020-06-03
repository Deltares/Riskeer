// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for configuration calculation scenarios.
    /// </summary>
    /// <typeparam name="TCalculationScenario">The type of calculation scenario.</typeparam>
    /// <typeparam name="TCalculationInput">The type of calculation input.</typeparam>
    /// <typeparam name="TScenarioRow">The type of the scenario row.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
    public abstract partial class ScenariosView<TCalculationScenario, TCalculationInput, TScenarioRow, TFailureMechanism> : UserControl, IView
        where TCalculationScenario : class, ICalculationScenario
        where TCalculationInput : class, ICalculationInput
        where TScenarioRow : ScenarioRow<TCalculationScenario>
        where TFailureMechanism : IFailureMechanism
    {
        private Observer failureMechanismObserver;
        private RecursiveObserver<CalculationGroup, CalculationGroup> calculationGroupObserver;
        private RecursiveObserver<CalculationGroup, TCalculationScenario> calculationObserver;
        private RecursiveObserver<CalculationGroup, TCalculationInput> calculationInputObserver;

        private IEnumerable<TScenarioRow> scenarioRows;

        /// <summary>
        /// Creates a new instance of <see cref="ScenariosView{TCalculationScenario, TCalculationInput, TScenarioRow, TFailureMechanism}"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="TFailureMechanism"/>
        /// to get the sections from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        protected ScenariosView(CalculationGroup calculationGroup, TFailureMechanism failureMechanism)
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
            FailureMechanism = failureMechanism;

            InitializeObservers();

            InitializeComponent();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Initialization is performed in <see cref="OnLoad"/> in order to
        /// prevent errors due to virtual member calls in constructor.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeListBox();
            InitializeDataGridView();

            UpdateSectionsListBox();
            UpdateDataGridViewDataSource();
        }

        public object Data
        {
            get => CalculationGroup;
            set => CalculationGroup = (CalculationGroup) value;
        }

        /// <summary>
        /// Gets the <see cref="TFailureMechanism"/>.
        /// </summary>
        protected TFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Gets the <see cref="CalculationGroup"/>.
        /// </summary>
        public CalculationGroup CalculationGroup { get; private set; }

        /// <summary>
        /// Gets the input of a <see cref="TCalculationScenario"/>.
        /// </summary>
        /// <param name="calculationScenario">The <see cref="TCalculationScenario"/> to get the input from.</param>
        /// <returns>The <see cref="TCalculationInput"/>.</returns>
        protected abstract TCalculationInput GetCalculationInput(TCalculationScenario calculationScenario);

        /// <summary>
        /// Gets a collection of <see cref="TScenarioRow"/>.
        /// </summary>
        /// <returns>The collection of <see cref="TScenarioRow"/>.</returns>
        protected abstract IEnumerable<TScenarioRow> GetScenarioRows(FailureMechanismSection failureMechanismSection);

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            calculationGroupObserver.Dispose();
            calculationObserver.Dispose();
            calculationInputObserver.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected virtual void InitializeDataGridView()
        {
            DataGridViewControl.AddCheckBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.IsRelevant),
                Resources.ScenarioView_InitializeDataGridView_In_final_rating
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.Contribution),
                Resources.ScenarioView_InitializeDataGridView_Contribution
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.Name),
                Resources.ScenarioView_Name_DisplayName
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(ScenarioRow<TCalculationScenario>.FailureProbability),
                Resources.ScenarioView_FailureProbability_DisplayName
            );
        }

        private void InitializeObservers()
        {
            failureMechanismObserver = new Observer(UpdateSectionsListBox)
            {
                Observable = FailureMechanism
            };

            calculationGroupObserver = new RecursiveObserver<CalculationGroup, CalculationGroup>(UpdateDataGridViewDataSource, pcg => pcg.Children)
            {
                Observable = CalculationGroup
            };

            calculationObserver = new RecursiveObserver<CalculationGroup, TCalculationScenario>(UpdateScenarioRows, pcg => pcg.Children)
            {
                Observable = CalculationGroup
            };

            // The concat is needed to observe the input of calculations in child groups.
            calculationInputObserver = new RecursiveObserver<CalculationGroup, TCalculationInput>(UpdateDataGridViewDataSource, pcg => pcg.Children.Concat<object>(
                                                                                                      pcg.Children.OfType<TCalculationScenario>()
                                                                                                         .Select(GetCalculationInput)))
            {
                Observable = CalculationGroup
            };
        }

        private void UpdateDataGridViewDataSource()
        {
            if (!(listBox.SelectedItem is FailureMechanismSection failureMechanismSection))
            {
                DataGridViewControl.SetDataSource(null);
                return;
            }

            scenarioRows = GetScenarioRows(failureMechanismSection);
            DataGridViewControl.SetDataSource(scenarioRows);
        }

        private void InitializeListBox()
        {
            listBox.DisplayMember = nameof(FailureMechanismSection.Name);
            listBox.SelectedValueChanged += ListBoxOnSelectedValueChanged;
        }

        private void UpdateSectionsListBox()
        {
            listBox.Items.Clear();

            if (FailureMechanism.Sections.Any())
            {
                listBox.Items.AddRange(FailureMechanism.Sections.Cast<object>().ToArray());
                listBox.SelectedItem = FailureMechanism.Sections.First();
            }
        }

        private void ListBoxOnSelectedValueChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewDataSource();
        }

        private void UpdateScenarioRows()
        {
            scenarioRows.ForEachElementDo(row => row.Update());
            DataGridViewControl.RefreshDataGridView();
        }
    }
}