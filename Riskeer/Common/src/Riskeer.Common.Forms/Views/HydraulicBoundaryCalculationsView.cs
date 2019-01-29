// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.GuiServices;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for <see cref="HydraulicBoundaryLocationCalculation"/> views which should be derived in
    /// order to get a consistent look and feel.
    /// </summary>
    public abstract partial class HydraulicBoundaryCalculationsView : CalculationsView<HydraulicBoundaryLocationCalculation>
    {
        private readonly Observer calculationsObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> calculationObserver;

        private readonly IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        protected HydraulicBoundaryCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                    IAssessmentSection assessmentSection)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;

            calculationsObserver = new Observer(UpdateDataGridViewDataSource);
            calculationObserver = new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(HandleHydraulicBoundaryLocationCalculationUpdate, hblc => hblc);

            this.calculations = calculations;

            calculationsObserver.Observable = calculations;
            calculationObserver.Observable = calculations;

            UpdateDataGridViewDataSource();
        }

        public override object Data { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHydraulicBoundaryLocationCalculationGuiService"/>.
        /// </summary>
        public IHydraulicBoundaryLocationCalculationGuiService CalculationGuiService { get; set; }

        protected override void Dispose(bool disposing)
        {
            calculationsObserver.Dispose();
            calculationObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();

            dataGridViewControl.AddCheckBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.IncludeIllustrationPoints),
                                                  RingtoetsCommonFormsResources.HydraulicBoundaryLocationCalculationInput_IncludeIllustrationPoints_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Name),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Id),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Location),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName);
        }

        /// <summary>
        /// Performs the selected <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The calculations to perform.</param>
        protected abstract void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations);

        protected override void CalculateForSelectedRows()
        {
            if (CalculationGuiService == null)
            {
                return;
            }

            PerformSelectedCalculations(GetSelectedCalculatableObjects());
        }

        protected override void SetDataSource()
        {
            dataGridViewControl.SetDataSource(calculations?.Select(CreateNewRow).ToArray());
        }

        protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
            if (currentRow == null)
            {
                return Enumerable.Empty<IllustrationPointControlItem>();
            }

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = ((HydraulicBoundaryLocationCalculationRow) currentRow.DataBoundItem).CalculatableObject;

            HydraulicBoundaryLocationCalculationOutput hydraulicBoundaryLocationCalculationOutput = hydraulicBoundaryLocationCalculation.Output;
            if (hydraulicBoundaryLocationCalculation.HasOutput
                && hydraulicBoundaryLocationCalculationOutput.HasGeneralResult)
            {
                return hydraulicBoundaryLocationCalculationOutput.GeneralResult.TopLevelIllustrationPoints.Select(
                    topLevelSubMechanismIllustrationPoint =>
                    {
                        SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                            topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;
                        return new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint,
                                                                topLevelSubMechanismIllustrationPoint.WindDirection.Name,
                                                                topLevelSubMechanismIllustrationPoint.ClosingSituation,
                                                                subMechanismIllustrationPoint.Stochasts,
                                                                subMechanismIllustrationPoint.Beta);
                    }).ToArray();
            }

            return Enumerable.Empty<IllustrationPointControlItem>();
        }

        private static HydraulicBoundaryLocationCalculationRow CreateNewRow(HydraulicBoundaryLocationCalculation calculation)
        {
            return new HydraulicBoundaryLocationCalculationRow(calculation);
        }
    }
}