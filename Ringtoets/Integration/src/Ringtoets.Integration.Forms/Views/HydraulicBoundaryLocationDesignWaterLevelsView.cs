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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
    /// </summary>
    public partial class HydraulicBoundaryLocationDesignWaterLevelsView : UserControl, ISelectionProvider
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private IAssessmentSection assessmentSection;
        private HydraulicBoundaryDatabase hydraulicBoundaryDatabase;
        private bool updatingDataSource;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationDesignWaterLevelsView"/>.
        /// </summary>
        public HydraulicBoundaryLocationDesignWaterLevelsView()
        {
            InitializeComponent();
            InitializeDataGridView();

            assessmentSectionObserver = new Observer(UpdateDataGridViewDataSource);
            hydraulicBoundaryDatabaseObserver = new Observer(UpdateDataGridViewDataSource);
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

        public object Data
        {
            get
            {
                return hydraulicBoundaryDatabase;
            }
            set
            {
                hydraulicBoundaryDatabase = value as HydraulicBoundaryDatabase;

                UpdateDataGridViewDataSource();

                hydraulicBoundaryDatabaseObserver.Observable = hydraulicBoundaryDatabase;
            }
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryDatabaseObserver.Dispose();

            dataGridViewControl.RemoveCellClickHandler(DataGridViewOnCellClick);

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCellClickHandler(DataGridViewOnCellClick);
            
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationDesignWaterLevelRow>(row => row.Name),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationDesignWaterLevelRow>(row => row.Id),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationDesignWaterLevelRow>(row => row.Location),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Coordinates_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationDesignWaterLevelRow>(row => row.DesignWaterLevel),
                                                 Resources.HydraulicBoundaryDatabase_Locations_DesignWaterLevel_DisplayName);
        }

        private void UpdateDataGridViewDataSource()
        {
            updatingDataSource = true;
            dataGridViewControl.SetDataSource(hydraulicBoundaryDatabase != null
                                                  ? hydraulicBoundaryDatabase.Locations.Select(hl => new HydraulicBoundaryLocationDesignWaterLevelRow(hl)).ToArray()
                                                  : null);
            dataGridViewControl.RefreshDataGridView();
            updatingDataSource = false;
        }

        /// <summary>
        /// Gets or sets the <see cref="IApplicationSelection"/>.
        /// </summary>
        public IApplicationSelection ApplicationSelection { get; set; }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }
        
        #region Event handling
        
        private void DataGridViewOnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (updatingDataSource)
            {
                return;
            }

            UpdateApplicationSelection();
        }

        private void UpdateApplicationSelection()
        {
            if (ApplicationSelection == null)
            {
                return;
            }

            DesignWaterLevelLocationContext selection = CreateSelectedItemFromCurrentRow();
            if ((ApplicationSelection.Selection == null && selection != null) ||
                (ApplicationSelection.Selection != null && !ApplicationSelection.Selection.Equals(selection)))
            {
                ApplicationSelection.Selection = selection;
            }
        }

        private DesignWaterLevelLocationContext CreateSelectedItemFromCurrentRow()
        {
            var currentRow = dataGridViewControl.GetCurrentRow();

            var designWaterLevelRow = currentRow != null
                                           ? (HydraulicBoundaryLocationDesignWaterLevelRow)currentRow.DataBoundItem
                                           : null;

            return designWaterLevelRow != null 
                ? new DesignWaterLevelLocationContext(designWaterLevelRow.HydraulicBoundaryLocation) 
                : null;
        }

        #endregion
    }
}