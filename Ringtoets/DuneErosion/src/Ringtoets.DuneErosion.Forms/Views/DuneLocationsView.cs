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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.DuneErosion.Forms.Views
{
    /// <summary>
    /// View for the <see cref="DuneLocation"/>.
    /// </summary>
    public partial class DuneLocationsView : CalculatableView<DuneLocation>
    {
        private readonly Observer duneLocationsObserver;
        private readonly Observer assessmentSectionObserver;

        private IAssessmentSection assessmentSection;
        private DuneErosionFailureMechanism failureMechanism;
        private ObservableList<DuneLocation> locations;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationsView"/>.
        /// </summary>
        public DuneLocationsView()
        {
            InitializeComponent();

            duneLocationsObserver = new Observer(UpdateDuneLocations);
            assessmentSectionObserver = new Observer(UpdateCalculateForSelectedButton);
        }

        public override object Data
        {
            get
            {
                return locations;
            }
            set
            {
                var data = (ObservableList<DuneLocation>) value;
                locations = data;
                UpdateDataGridViewDataSource();
                duneLocationsObserver.Observable = data;
            }
        }

        /// <summary>
        /// Gets or sets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                assessmentSection = value;
                assessmentSectionObserver.Observable = assessmentSection;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DuneErosionFailureMechanism"/> for which the
        /// locations are shown.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism
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

        /// <summary>
        /// Gets or sets the <see cref="DuneLocationCalculationGuiService"/> 
        /// to perform calculations with.
        /// </summary>
        public DuneLocationCalculationGuiService CalculationGuiService { get; set; }

        protected override void Dispose(bool disposing)
        {
            duneLocationsObserver.Dispose();
            assessmentSectionObserver.Dispose();
            base.Dispose(disposing);
        }

        protected override void InitializeDataGridView()
        {
            base.InitializeDataGridView();
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.Name),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.Id),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.Location),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.CoastalAreaId),
                                                 Resources.DuneLocation_CoastalAreaId_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.Offset),
                                                 Resources.DuneLocation_Offset_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.WaterLevel),
                                                 Resources.DuneLocation_WaterLevel_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.WaveHeight),
                                                 Resources.DuneLocation_WaveHeight_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.WavePeriod),
                                                 Resources.DuneLocation_WavePeriod_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationRow.D50),
                                                 Resources.DuneLocation_D50_DisplayName);
        }

        protected override object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
            return currentRow != null
                       ? new DuneLocationContext((ObservableList<DuneLocation>) Data, ((DuneLocationRow) currentRow.DataBoundItem).CalculatableObject)
                       : null;
        }

        protected override void SetDataSource()
        {
            dataGridViewControl.SetDataSource(locations?.Select(l => new DuneLocationRow(l)).ToArray());
        }

        protected override void CalculateForSelectedRows()
        {
            if (CalculationGuiService != null)
            {
                IEnumerable<DuneLocation> selectedLocations = GetSelectedCalculatableObjects();
                HandleCalculateSelectedLocations(selectedLocations);
            }
        }

        protected override string ValidateCalculatableObjects()
        {
            if (FailureMechanism != null && FailureMechanism.Contribution <= 0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return base.ValidateCalculatableObjects();
        }

        private void HandleCalculateSelectedLocations(IEnumerable<DuneLocation> locationsToCalculate)
        {
            CalculationGuiService.Calculate(locationsToCalculate,
                                            FailureMechanism,
                                            AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                            FailureMechanism.GetMechanismSpecificNorm(AssessmentSection.FailureMechanismContribution.Norm));

            ((IObservable) Data).NotifyObservers();
        }

        private void UpdateDuneLocations()
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
            DataGridViewRowCollection rows = dataGridViewControl.Rows;
            int rowCount = rows.Count;
            if (rowCount != locations.Count)
            {
                return true;
            }
            for (var i = 0; i < rowCount; i++)
            {
                DuneLocation locationFromGrid = ((DuneLocationRow) rows[i].DataBoundItem).CalculatableObject;
                if (!ReferenceEquals(locationFromGrid, locations[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}