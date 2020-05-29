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

using System.Windows.Forms;
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
    public abstract partial class ScenariosView<TCalculationScenario> : UserControl, IView
        where TCalculationScenario : class, ICalculationScenario
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScenariosView{TCalculationScenario}"/>.
        /// </summary>
        protected ScenariosView()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeListBox();
        }

        public object Data { get; set; }

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
    }
}