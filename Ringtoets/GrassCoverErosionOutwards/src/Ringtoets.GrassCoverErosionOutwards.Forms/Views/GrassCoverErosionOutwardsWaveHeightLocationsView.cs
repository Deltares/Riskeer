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
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.WaveHeight"/>
    /// for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightLocationsView : HydraulicBoundaryLocationsView
    {
        private readonly GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider messageProvider;
        private readonly Observer failureMechanismObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightLocationsView"/>.
        /// </summary>
        /// <param name="locations">The locations to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism that the locations belong to.</param>
        /// <param name="assessmentSection">The assessment section that the locations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveHeightLocationsView(ObservableList<HydraulicBoundaryLocation> locations,
                                                                GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                IAssessmentSection assessmentSection)
            : base(locations, assessmentSection)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanism = failureMechanism;
            messageProvider = new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider();

            failureMechanismObserver = new Observer(UpdateCalculateForSelectedButton)
            {
                Observable = failureMechanism
            };
        }

        public override object Data { get; set; }

        /// <summary>
        /// Gets the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for which the
        /// hydraulic boundary locations are shown.
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
                       ? new GrassCoverErosionOutwardsWaveHeightLocationContext(((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject,
                                                                                FailureMechanism.HydraulicBoundaryLocations)
                       : null;
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            double mechanismSpecificNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                AssessmentSection.FailureMechanismContribution.Norm,
                FailureMechanism.Contribution,
                FailureMechanism.GeneralInput.N);

            CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                       AssessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                       locations,
                                                       mechanismSpecificNorm,
                                                       messageProvider);
        }

        protected override string ValidateCalculatableObjects()
        {
            return FailureMechanism == null || FailureMechanism.Contribution <= 0
                       ? RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero
                       : base.ValidateCalculatableObjects();
        }

        protected override HydraulicBoundaryLocationCalculation GetCalculation(HydraulicBoundaryLocation location)
        {
            return location.WaveHeightCalculation;
        }
    }
}