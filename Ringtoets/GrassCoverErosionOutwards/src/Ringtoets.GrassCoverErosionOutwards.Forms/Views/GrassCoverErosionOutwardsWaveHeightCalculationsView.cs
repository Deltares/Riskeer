// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// View for presenting and performing wave height calculations for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightCalculationsView : HydraulicBoundaryCalculationsView
    {
        private readonly GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider messageProvider;
        private readonly Observer failureMechanismObserver;
        private readonly Func<double> getNormFunc;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism that the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section that the calculations belong to.</param>
        /// <param name="getNormFunc"><see cref="Func{TResult}"/> for getting the norm to derive a mechanism specific norm from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveHeightCalculationsView(ObservableList<HydraulicBoundaryLocationCalculation> calculations,
                                                                   GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                   IAssessmentSection assessmentSection,
                                                                   Func<double> getNormFunc)
            : base(calculations, assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (getNormFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormFunc));
            }

            FailureMechanism = failureMechanism;
            messageProvider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            failureMechanismObserver = new Observer(UpdateCalculateForSelectedButton)
            {
                Observable = failureMechanism
            };

            this.getNormFunc = getNormFunc;
        }

        public override object Data { get; set; }

        /// <summary>
        /// Gets the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for which the
        /// hydraulic boundary location calculations are shown.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; }

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();

            base.Dispose(disposing);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Result),
                                                 Resources.GrassCoverErosionOutwardsHydraulicBoundaryLocation_WaveHeight_DisplayName);
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            return currentRow != null
                       ? new GrassCoverErosionOutwardsWaveHeightCalculationContext(((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject)
                       : null;
        }

        protected override void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations)
        {
            double mechanismSpecificNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                getNormFunc(),
                FailureMechanism.Contribution,
                FailureMechanism.GeneralInput.N);

            CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                       AssessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                       calculations,
                                                       mechanismSpecificNorm,
                                                       messageProvider);
        }

        protected override string ValidateCalculatableObjects()
        {
            return FailureMechanism != null && FailureMechanism.Contribution <= 0
                       ? RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero
                       : base.ValidateCalculatableObjects();
        }
    }
}