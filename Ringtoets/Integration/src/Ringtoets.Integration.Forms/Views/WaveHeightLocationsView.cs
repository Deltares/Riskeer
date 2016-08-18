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
    /// View for the <see cref="HydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.WaveHeight"/>.
    /// </summary>
    public partial class WaveHeightLocationsView : UserControl, ISelectionProvider
    {
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        private IAssessmentSection assessmentSection;
        private bool updatingDataSource;

        /// <summary>
        /// Creates a new instance of <see cref="WaveHeightLocationsView"/>.
        /// </summary>
        public WaveHeightLocationsView()
        {
            InitializeComponent();
            InitializeDataGridView();

            assessmentSectionObserver = new Observer(UpdateDataGridViewDataSource);
            hydraulicBoundaryDatabaseObserver = new Observer(() => dataGridViewControl.RefreshDataGridView());
        }

        /// <summary>
        /// Gets or sets the <see cref="IApplicationSelection"/>.
        /// </summary>
        public IApplicationSelection ApplicationSelection { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IHydraulicBoundaryLocationCalculationCommandHandler"/>.
        /// </summary>
        public IHydraulicBoundaryLocationCalculationCommandHandler CalculationCommandHandler { get; set; }

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
            }
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

        private void SetHydraulicBoundaryDatabaseObserver()
        {
            hydraulicBoundaryDatabaseObserver.Observable = assessmentSection != null ? assessmentSection.HydraulicBoundaryDatabase : null;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCellClickHandler(DataGridViewOnCellClick);

            dataGridViewControl.AddCheckBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationContextRow>(row => row.ToCalculate),
                                                  Resources.HydraulicBoundaryLocationsView_Calculate);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationContextRow>(row => row.Name),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationContextRow>(row => row.Id),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationContextRow>(row => row.Location),
                                                 Resources.HydraulicBoundaryDatabase_Locations_Coordinates_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<WaveHeightLocationContextRow>(row => row.WaveHeight),
                                                 Resources.HydraulicBoundaryDatabase_Locations_WaveHeight_DisplayName);
        }

        private void UpdateDataGridViewDataSource()
        {
            SetHydraulicBoundaryDatabaseObserver();

            updatingDataSource = true;
            dataGridViewControl.SetDataSource(assessmentSection != null && assessmentSection.HydraulicBoundaryDatabase != null
                                                  ? assessmentSection.HydraulicBoundaryDatabase.Locations.Select(
                                                      hl => new WaveHeightLocationContextRow(
                                                                new WaveHeightLocationContext(assessmentSection.HydraulicBoundaryDatabase, hl))).ToArray()
                                                  : null);
            updatingDataSource = false;
        }

        private IEnumerable<WaveHeightLocationContextRow> GetWaveHeightLocationContextRows()
        {
            return dataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => (WaveHeightLocationContextRow) row.DataBoundItem);
        }

        private IEnumerable<HydraulicBoundaryLocation> GetSelectedHydraulicBoundaryLocationContext()
        {
            return GetWaveHeightLocationContextRows().Where(r => r.ToCalculate).Select(r => r.HydraulicBoundaryLocationContext.HydraulicBoundaryLocation);
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

            var waterLevelRow = currentRow != null
                                    ? (WaveHeightLocationContextRow) currentRow.DataBoundItem
                                    : null;

            return waterLevelRow != null
                       ? waterLevelRow.HydraulicBoundaryLocationContext
                       : null;
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            GetWaveHeightLocationContextRows().ForEachElementDo(row => row.ToCalculate = true);
            dataGridViewControl.RefreshDataGridView();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            GetWaveHeightLocationContextRows().ForEachElementDo(row => row.ToCalculate = false);
            dataGridViewControl.RefreshDataGridView();
        }

        private void CalculateForSelectedButton_Click(object sender, EventArgs e)
        {
            if (CalculationCommandHandler == null)
            {
                return;
            }
            var locations = GetSelectedHydraulicBoundaryLocationContext();
            CalculationCommandHandler.CalculateWaveHeights(locations);
        }

        #endregion
    }
}