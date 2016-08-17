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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;
using Core.Common.Utils.Extensions;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Commands;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Properties;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
    /// </summary>
    public partial class DesignWaterLevelLocationsView : UserControl, ISelectionProvider
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private IAssessmentSection assessmentSection;
        private bool updatingDataSource;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelLocationsView"/>.
        /// </summary>
        public DesignWaterLevelLocationsView()
        {
            InitializeComponent();
            InitializeDataGridView();

            assessmentSectionObserver = new Observer(UpdateDataGridViewDataSource);
            hydraulicBoundaryDatabaseObserver = new Observer(RefreshDataGridView);
        }

        /// <summary>
        /// Gets or sets the <see cref="IApplicationSelection"/>.
        /// </summary>
        public IApplicationSelection ApplicationSelection { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICalculateDesignWaterLevelCommandHandler"/>.
        /// </summary>
        public ICalculateDesignWaterLevelCommandHandler CalculationCommandHandler { private get; set; }

        public object Data
        {
            get
            {
                return assessmentSection;
            }
            set
            {
                assessmentSection = value as IAssessmentSection;

                UpdateDataGridViewDataSource();
                assessmentSectionObserver.Observable = assessmentSection;
                SetHydraulicBoundaryDatabaseObserver();
            }
        }

        private void SetHydraulicBoundaryDatabaseObserver()
        {
            hydraulicBoundaryDatabaseObserver.Observable = assessmentSection != null ? assessmentSection.HydraulicBoundaryDatabase : null;
        }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }

        protected override void Dispose(bool disposing)
        {
            assessmentSectionObserver.Dispose();
            hydraulicBoundaryDatabaseObserver.Dispose();

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void RefreshDataGridView()
        {
            dataGridViewControl.RefreshDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCellClickHandler(DataGridViewOnCellClick);

            dataGridViewControl.AddCheckBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationContextRow>(row => row.ToCalculate),
                                                  Resources.DesignWaterLevelLocationContextRow_Calculate);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationContextRow>(row => row.Name),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationContextRow>(row => row.Id),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationContextRow>(row => row.Location),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Coordinates_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<DesignWaterLevelLocationContextRow>(row => row.DesignWaterLevel),
                                                 Resources.HydraulicBoundaryDatabase_Locations_DesignWaterLevel_DisplayName);
        }

        private void UpdateDataGridViewDataSource()
        {
            SetHydraulicBoundaryDatabaseObserver();

            updatingDataSource = true;
            dataGridViewControl.SetDataSource(assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null
                                                  ? assessmentSection.HydraulicBoundaryDatabase.Locations.Select(
                                                  hl => new DesignWaterLevelLocationContextRow(
                                                      new DesignWaterLevelLocationContext(assessmentSection.HydraulicBoundaryDatabase, hl))).ToArray()
                                                  : null);
            RefreshDataGridView();
            updatingDataSource = false;
        }

        private IEnumerable<DesignWaterLevelLocationContextRow> GetDesignWaterLevelLocationContextRows()
        {
            return from DataGridViewRow row in dataGridViewControl.Rows select (DesignWaterLevelLocationContextRow) row.DataBoundItem;
        }

        private IEnumerable<HydraulicBoundaryLocation> GetSelectedDesignWaterLevelLocationContext()
        {
            return GetDesignWaterLevelLocationContextRows().Where(r => r.ToCalculate).Select(r => r.DesignWaterLevelLocationContext.HydraulicBoundaryLocation);
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

            HydraulicBoundaryLocationContext selection = CreateSelectedItemFromCurrentRow();
            if ((ApplicationSelection.Selection == null && selection != null) ||
                (ApplicationSelection.Selection != null && !ReferenceEquals(selection, ApplicationSelection.Selection)))
            {
                ApplicationSelection.Selection = selection;
            }
        }

        private HydraulicBoundaryLocationContext CreateSelectedItemFromCurrentRow()
        {
            var currentRow = dataGridViewControl.CurrentRow;

            var designWaterLevelRow = currentRow != null
                                          ? (DesignWaterLevelLocationContextRow) currentRow.DataBoundItem
                                          : null;

            return designWaterLevelRow != null
                       ? designWaterLevelRow.DesignWaterLevelLocationContext
                       : null;
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            GetDesignWaterLevelLocationContextRows().ForEachElementDo(row => row.ToCalculate = true);
            dataGridViewControl.RefreshDataGridView();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            GetDesignWaterLevelLocationContextRows().ForEachElementDo(row => row.ToCalculate = false);
            dataGridViewControl.RefreshDataGridView();
        }

        private void CalculateForSelectedButton_Click(object sender, EventArgs e)
        {
            if (CalculationCommandHandler == null)
            {
                return;
            }
            var locations = GetSelectedDesignWaterLevelLocationContext();
            CalculationCommandHandler.CalculateDesignWaterLevels(locations);
        }

        #endregion
    }
}