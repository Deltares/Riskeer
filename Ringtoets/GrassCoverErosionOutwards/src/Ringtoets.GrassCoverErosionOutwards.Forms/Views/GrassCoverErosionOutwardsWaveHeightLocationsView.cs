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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
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
    /// for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/></summary>
    public class GrassCoverErosionOutwardsWaveHeightLocationsView : HydraulicBoundaryLocationsView
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryLocationsObserver;

        private GrassCoverErosionOutwardsFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightLocationsView"/>
        /// </summary>
        /// <param name="assessmentSection">The assessment section which the locations belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assessmentSection"/>
        /// is <c>null</c>.</exception>
        public GrassCoverErosionOutwardsWaveHeightLocationsView(IAssessmentSection assessmentSection)
            : base(assessmentSection)
        {
            assessmentSectionObserver = new Observer(UpdateCalculateForSelectedButton);
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocations);

            assessmentSectionObserver.Observable = AssessmentSection;
        }

        public override object Data
        {
            get
            {
                return base.Data;
            }
            set
            {
                var data = (ObservableList<HydraulicBoundaryLocation>) value;
                base.Data = data;
                hydraulicBoundaryLocationsObserver.Observable = data;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for which the
        /// hydraulic boundary locations are shown.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism
        {
            get
            {
                return failureMechanism;
            }
            set
            {
                failureMechanism = value;
                UpdateCalculateForSelectedButton();
            }
        }

        protected override HydraulicBoundaryLocationRow CreateNewRow(HydraulicBoundaryLocation location)
        {
            return new HydraulicBoundaryLocationRow(location, location.WaveHeightCalculation);
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            return currentRow != null
                       ? new GrassCoverErosionOutwardsWaveHeightLocationContext((ObservableList<HydraulicBoundaryLocation>) Data,
                                                                                ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject)
                       : null;
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            if (AssessmentSection?.HydraulicBoundaryDatabase == null)
            {
                return;
            }

            double mechanismSpecificNorm = RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                AssessmentSection.FailureMechanismContribution.Norm,
                FailureMechanism.Contribution,
                FailureMechanism.GeneralInput.N);

            bool successFullCalculation = CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                     locations,
                                                                                     mechanismSpecificNorm,
                                                                                     new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider());

            if (successFullCalculation)
            {
                ((IObservable) Data).NotifyObservers();
            }
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationRow.Result),
                                                 Resources.GrassCoverErosionOutwardsHydraulicBoundaryLocation_WaveHeight_DisplayName);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryLocationsObserver.Dispose();
            base.Dispose(disposing);
        }

        protected override string ValidateCalculatableObjects()
        {
            if (FailureMechanism != null && FailureMechanism.Contribution <= 0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return base.ValidateCalculatableObjects();
        }

        protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
            if (currentRow == null)
            {
                return null;
            }

            HydraulicBoundaryLocation location = ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject;

            HydraulicBoundaryLocationCalculation waveHeightCalculation = location.WaveHeightCalculation;
            HydraulicBoundaryLocationOutput waveHeightOutput = waveHeightCalculation.Output;
            if (waveHeightCalculation.HasOutput
                && waveHeightOutput.HasIllustrationPoints)
            {
                return waveHeightOutput.GeneralResultSubMechanismIllustrationPoint.TopLevelIllustrationPoints.Select(
                    topLevelSubMechanismIllustrationPoint =>
                    {
                        SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                            topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;
                        return new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint,
                                                                topLevelSubMechanismIllustrationPoint.WindDirection.Name,
                                                                topLevelSubMechanismIllustrationPoint.ClosingSituation,
                                                                subMechanismIllustrationPoint.Stochasts,
                                                                subMechanismIllustrationPoint.Beta);
                    });
            }

            return null;
        }

        private void UpdateHydraulicBoundaryLocations()
        {
            if (IsDataGridDataSourceChanged())
            {
                UpdateDataGridViewDataSource();
            }
            else
            {
                HandleHydraulicBoundaryDatabaseUpdate();
            }
        }

        private bool IsDataGridDataSourceChanged()
        {
            var locations = (ObservableList<HydraulicBoundaryLocation>) Data;
            int count = dataGridViewControl.Rows.Count;
            if (count != locations.Count)
            {
                return true;
            }
            for (var i = 0; i < count; i++)
            {
                HydraulicBoundaryLocation locationFromGrid = ((HydraulicBoundaryLocationRow) dataGridViewControl.Rows[i].DataBoundItem).CalculatableObject;
                if (!ReferenceEquals(locationFromGrid, locations[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}