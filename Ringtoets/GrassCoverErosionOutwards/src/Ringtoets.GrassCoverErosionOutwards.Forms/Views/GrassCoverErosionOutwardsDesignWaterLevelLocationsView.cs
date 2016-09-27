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

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Properties;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Views
{
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsView : HydraulicBoundaryLocationsView<DesignWaterLevelLocationRow>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionOutwardsDesignWaterLevelLocationsView));
        private readonly Observer hydraulicBoundaryLocationsObserver;

        public GrassCoverErosionOutwardsDesignWaterLevelLocationsView()
        {
            hydraulicBoundaryLocationsObserver = new Observer(UpdateHydraulicBoundaryLocations);
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

        protected override DesignWaterLevelLocationRow CreateNewRow(HydraulicBoundaryLocation location)
        {
            return new DesignWaterLevelLocationRow(location);
        }

        /// <summary>
        /// Gets or sets the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for which the
        /// hydraulic boundary locations are shown.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism FailureMechanism { get; set; }

        public override IAssessmentSection AssessmentSection { get; set; }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationRow>(row => row.DesignWaterLevel),
                                                 Resources.GrassCoverErosionOutwardsHydraulicBoundaryLocation_DesignWaterLevel_DisplayName);
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            var currentRow = dataGridViewControl.CurrentRow;

            return currentRow != null ?
                       new GrassCoverErosionOutwardsDesignWaterLevelLocationContext((ObservableList<HydraulicBoundaryLocation>) Data, ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).HydraulicBoundaryLocation) :
                       null;
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            if (AssessmentSection == null || AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                return;
            }

            var modifiedBeta = GetModifiedBeta();

            if (!double.IsNaN(modifiedBeta))
            {
                bool successfulCalculation = CalculationGuiService.CalculateDesignWaterLevels(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                              locations,
                                                                                              AssessmentSection.Id,
                                                                                              modifiedBeta,
                                                                                              new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider());
                if (successfulCalculation)
                {
                    ((IObservable) Data).NotifyObservers();
                }
            }
        }

        private void UpdateHydraulicBoundaryLocations()
        {
            if (IsDataGridDataSourceChanged())
            {
                UpdateDataGridViewDataSource();
            }
            else
            {
                dataGridViewControl.RefreshDataGridView();
            }
        }

        private bool IsDataGridDataSourceChanged()
        {
            var locations = (ObservableList<HydraulicBoundaryLocation>) Data;
            var count = dataGridViewControl.Rows.Count;
            if (count != locations.Count)
            {
                return true;
            }
            for (int i = 0; i < count; i++)
            {
                var locationFromGrid = ((DesignWaterLevelLocationRow) dataGridViewControl.Rows[i].DataBoundItem).HydraulicBoundaryLocation;
                if (!ReferenceEquals(locationFromGrid, locations[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private double GetModifiedBeta()
        {
            var calculationBeta = double.NaN;
            try
            {
                calculationBeta = FailureMechanism.CalculationBeta(AssessmentSection);
            }
            catch (ArgumentException e)
            {
                log.ErrorFormat(e.Message);
            }
            return calculationBeta;
        }
    }
}