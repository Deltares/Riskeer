﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Views
{
    /// <summary>
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.WaveHeight"/>
    /// </summary>
    public class GrassCoverErosionOutwardsWaveHeightLocationsView : HydraulicBoundaryLocationsView<WaveHeightLocationRow>
    {
        private readonly Observer hydraulicBoundaryLocationObserver;

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveHeightLocationsView"/>
        /// </summary>
        public GrassCoverErosionOutwardsWaveHeightLocationsView()
        {
            hydraulicBoundaryLocationObserver = new Observer(UpdateDataGridViewDataSource);
        }

        public override object Data
        {
            get
            {
                return base.Data;
            }
            set
            {
                base.Data = value;
                hydraulicBoundaryLocationObserver.Observable = value as IObservable;
            }
        }

        public IAssessmentSection AssessmentSection { get; set; }

        protected override WaveHeightLocationRow CreateNewRow(HydraulicBoundaryLocation location)
        {
            return new WaveHeightLocationRow(location);
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            var currentRow = dataGridViewControl.CurrentRow;
            
            return currentRow != null ?
                       new GrassCoverErosionOutwardsWaveHeightLocationContext((ObservableList<HydraulicBoundaryLocation>)Data,
                                                                              ((WaveHeightLocationRow) currentRow.DataBoundItem).HydraulicBoundaryLocation) :
                       null;
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            if (AssessmentSection == null || AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                return;
            }

            bool successFullCalculation = CalculationGuiService.CalculateWaveHeights(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                     locations,
                                                                                     AssessmentSection.Id,
                                                                                     AssessmentSection.FailureMechanismContribution.Norm,
                                                                                     new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider());

            if (successFullCalculation)
            {
                ((IObservable)Data).NotifyObservers();
            }
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationRow>(row => row.WaveHeight),
                                                 Resources.GrassCoverErosionOutwardsHydraulicBoundaryLocation_DesignWaterLevel_DisplayName);
        }

        protected override void Dispose(bool disposing)
        {
            hydraulicBoundaryLocationObserver.Dispose();
            base.Dispose(disposing);
        }
    }
}