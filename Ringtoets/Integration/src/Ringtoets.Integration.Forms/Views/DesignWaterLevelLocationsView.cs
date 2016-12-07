// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
    /// </summary>
    public partial class DesignWaterLevelLocationsView : HydraulicBoundaryLocationsView<DesignWaterLevelLocationRow>
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private IAssessmentSection assessmentSection;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelLocationsView"/>.
        /// </summary>
        public DesignWaterLevelLocationsView()
        {
            InitializeComponent();

            assessmentSectionObserver = new Observer(UpdateHydraulicBoundaryDatabase);
            hydraulicBoundaryDatabaseObserver = new Observer(() => dataGridViewControl.RefreshDataGridView());
        }

        public override IAssessmentSection AssessmentSection
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                assessmentSection = value;

                if (assessmentSection != null)
                {
                    assessmentSectionObserver.Observable = assessmentSection;
                    hydraulicBoundaryDatabaseObserver.Observable = assessmentSection.HydraulicBoundaryDatabase;
                }
            }
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            var currentRow = dataGridViewControl.CurrentRow;

            return currentRow != null ?
                       new DesignWaterLevelLocationContext(AssessmentSection.HydraulicBoundaryDatabase, ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).HydraulicBoundaryLocation) :
                       null;
        }

        protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
        {
            if (AssessmentSection == null || AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                return;
            }
            bool successfulCalculation = CalculationGuiService.CalculateDesignWaterLevels(AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                          locations,
                                                                                          AssessmentSection.Id,
                                                                                          AssessmentSection.FailureMechanismContribution.Norm, new DesignWaterLevelCalculationMessageProvider());
            if (successfulCalculation)
            {
                AssessmentSection.HydraulicBoundaryDatabase.NotifyObservers();
            }
        }

        protected override DesignWaterLevelLocationRow CreateNewRow(HydraulicBoundaryLocation location)
        {
            return new DesignWaterLevelLocationRow(location);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationRow>(row => row.DesignWaterLevel),
                                                 Resources.HydraulicBoundaryDatabase_Location_DesignWaterLevel_DisplayName);
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryDatabaseObserver.Dispose();

            base.Dispose(disposing);
        }

        private void UpdateHydraulicBoundaryDatabase()
        {
            if (AssessmentSection == null || AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                hydraulicBoundaryDatabaseObserver.Observable = null;
                Data = null;
            }
            else
            {
                var hydraulicBoundaryDatabase = AssessmentSection.HydraulicBoundaryDatabase;
                hydraulicBoundaryDatabaseObserver.Observable = hydraulicBoundaryDatabase;
                Data = hydraulicBoundaryDatabase.Locations;
            }
            UpdateDataGridViewDataSource();
        }
    }
}