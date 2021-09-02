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
using System.Collections.Generic;
using System.Windows.Forms;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// View for presenting and performing design water level calculations.
    /// </summary>
    public partial class DesignWaterLevelCalculationsView : HydraulicBoundaryCalculationsView
    {
        private readonly HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability;
        private readonly Func<string> getCalculationIdentifierFunc;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsView"/>.
        /// </summary>
        /// <param name="calculationsForTargetProbability">The calculations to show in the view.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <param name="getCalculationIdentifierFunc"><see cref="Func{TResult}"/> for getting the calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DesignWaterLevelCalculationsView(HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                IAssessmentSection assessmentSection,
                                                Func<string> getCalculationIdentifierFunc)
            : base(calculationsForTargetProbability?.HydraulicBoundaryLocationCalculations ?? throw new ArgumentNullException(nameof(calculationsForTargetProbability)), assessmentSection)
        {
            if (getCalculationIdentifierFunc == null)
            {
                throw new ArgumentNullException(nameof(getCalculationIdentifierFunc));
            }

            InitializeComponent();

            this.calculationsForTargetProbability = calculationsForTargetProbability;
            this.getCalculationIdentifierFunc = getCalculationIdentifierFunc;
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            if (currentRow != null)
            {
                return new DesignWaterLevelCalculationContext(((HydraulicBoundaryLocationCalculationRow) currentRow.DataBoundItem).CalculatableObject);
            }

            return null;
        }

        protected override void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            CalculationGuiService.CalculateDesignWaterLevels(calculations,
                                                             AssessmentSection,
                                                             calculationsForTargetProbability.TargetProbability,
                                                             getCalculationIdentifierFunc());
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Result),
                                                 RiskeerCommonFormsResources.WaterLevel_DisplayName);
        }
    }
}