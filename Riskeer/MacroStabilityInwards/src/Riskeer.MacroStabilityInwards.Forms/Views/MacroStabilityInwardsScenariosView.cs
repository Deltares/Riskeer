﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Views;
using Riskeer.MacroStabilityInwards.Data;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    /// <summary>
    /// View for configuring macrostability inwards calculation scenarios.
    /// </summary>
    public class MacroStabilityInwardsScenariosView : ScenariosView<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInput, MacroStabilityInwardsScenarioRow, MacroStabilityInwardsFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsScenariosView"/>.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/>
        /// to get the calculations from.</param>
        /// <param name="failureMechanism">The <see cref="MacroStabilityInwardsFailureMechanism"/>
        /// to get the sections from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsScenariosView(CalculationGroup calculationGroup, MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(calculationGroup, failureMechanism) {}

        protected override MacroStabilityInwardsInput GetCalculationInput(MacroStabilityInwardsCalculationScenario calculationScenario)
        {
            return calculationScenario.InputParameters;
        }

        protected override IEnumerable<MacroStabilityInwardsScenarioRow> GetScenarioRows(FailureMechanismSection failureMechanismSection)
        {
            IEnumerable<Segment2D> lineSegments = Math2D.ConvertPointsToLineSegments(failureMechanismSection.Points);
            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = CalculationGroup
                                                                                 .GetCalculations()
                                                                                 .OfType<MacroStabilityInwardsCalculationScenario>()
                                                                                 .Where(pc => pc.IsSurfaceLineIntersectionWithReferenceLineInSection(lineSegments));

            return calculations.Select(pc => new MacroStabilityInwardsScenarioRow(pc, FailureMechanism, failureMechanismSection)).ToList();
        }

        protected override void InitializeDataGridView()
        {
            DataGridViewControl.AddCheckBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.IsRelevant),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_In_final_rating
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.Contribution),
                RiskeerCommonFormsResources.ScenarioView_InitializeDataGridView_Contribution
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.Name),
                RiskeerCommonFormsResources.ScenarioView_Name_DisplayName
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.FailureProbability),
                RiskeerCommonFormsResources.ScenarioView_ProfileFailureProbability_DisplayName
            );
            DataGridViewControl.AddTextBoxColumn(
                nameof(MacroStabilityInwardsScenarioRow.SectionFailureProbability),
                RiskeerCommonFormsResources.ScenarioView_SectionFailureProbability_DisplayName
            );
        }
    }
}