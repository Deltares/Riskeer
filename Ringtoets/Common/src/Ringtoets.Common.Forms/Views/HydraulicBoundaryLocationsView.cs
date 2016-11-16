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
using Core.Common.Controls.Views;
using Core.Common.Utils.Extensions;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.HydraRing.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Base view for <see cref="HydraulicBoundaryLocation"/> views which should be derived in order to get a consistent look and feel.
    /// </summary>
    /// <typeparam name="T">The type of the row objects which are shown in the data table.</typeparam>
    public abstract partial class HydraulicBoundaryLocationsView<T> : UserControl, ISelectionProvider where T : HydraulicBoundaryLocationRow
    {
        private const int locationCalculateColumnIndex = 0;
        private bool updatingDataSource;
        private IEnumerable<HydraulicBoundaryLocation> locations;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsView{T}"/>.
        /// </summary>
        protected HydraulicBoundaryLocationsView()
        {
            InitializeComponent();
            LocalizeControls();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets or sets the <see cref="IHydraulicBoundaryLocationCalculationGuiService"/>.
        /// </summary>
        public IHydraulicBoundaryLocationCalculationGuiService CalculationGuiService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAssessmentSection"/>.
        /// </summary>
        public abstract IAssessmentSection AssessmentSection { get; set; }

        public virtual object Data
        {
            get
            {
                return locations;
            }
            set
            {
                locations = value as IEnumerable<HydraulicBoundaryLocation>;
                UpdateDataGridViewDataSource();
            }
        }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeDataGridView();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the data source of the data table based on the <see cref="Data"/>.
        /// </summary>
        protected void UpdateDataGridViewDataSource()
        {
            updatingDataSource = true;
            SetDataSource();
            updatingDataSource = false;
            UpdateCalculateForSelectedButton();
        }

        /// <summary>
        /// Initializes the <see cref="DataGridView"/>.
        /// </summary>
        protected virtual void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationRow>(row => row.ToCalculate),
                                                  RingtoetsCommonFormsResources.HydraulicBoundaryLocationsView_Calculate);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationRow>(row => row.Name),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationRow>(row => row.Id),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationRow>(row => row.Location),
                                                 RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName);
        }

        /// <summary>
        /// Creates a new row that is added to the data table.
        /// </summary>
        /// <param name="location">The location for which to create a new row.</param>
        /// <returns>The newly created row.</returns>
        protected abstract T CreateNewRow(HydraulicBoundaryLocation location);

        /// <summary>
        /// Creates a new object that is used as the object for <see cref="Selection"/> from
        /// the currently selected row in the data table.
        /// </summary>
        /// <returns>The newly created object.</returns>
        protected abstract object CreateSelectedItemFromCurrentRow();

        /// <summary>
        /// Handles the calculation of the <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The enumeration of <see cref="HydraulicBoundaryLocation"/> to use in the calculation.</param>
        protected abstract void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations);

        private void LocalizeControls()
        {
            CalculateForSelectedButton.Text = RingtoetsCommonFormsResources.HydraulicBoundaryLocationsView_CalculateForSelectedButton_Text;
            DeselectAllButton.Text = RingtoetsCommonFormsResources.HydraulicBoundaryLocationsView_DeselectAllButton_Text;
            SelectAllButton.Text = RingtoetsCommonFormsResources.HydraulicBoundaryLocationsView_SelectAllButton_Text;
            ButtonGroupBox.Text = RingtoetsCommonFormsResources.HydraulicBoundaryLocationsView_ButtonGroupBox_Text;
        }

        private IEnumerable<T> GetHydraulicBoundaryLocationContextRows()
        {
            return dataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => (T) row.DataBoundItem);
        }

        /// <summary>
        /// Sets the datasource on the <see cref="DataGridView"/>.
        /// </summary>
        private void SetDataSource()
        {
            dataGridViewControl.SetDataSource(locations != null ? locations.Select(CreateNewRow).ToArray()
                                                  : null);
        }

        private void UpdateCalculateForSelectedButton()
        {
            CalculateForSelectedButton.Enabled = GetHydraulicBoundaryLocationContextRows().Any(r => r.ToCalculate);
        }

        private void InitializeEventHandlers()
        {
            dataGridViewControl.AddCellClickHandler(DataGridViewOnCellClick);
            dataGridViewControl.AddCellValueChangedHandler(DataGridViewCellValueChanged);
        }

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (updatingDataSource || e.ColumnIndex != locationCalculateColumnIndex)
            {
                return;
            }
            UpdateCalculateForSelectedButton();
        }

        private void DataGridViewOnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (updatingDataSource)
            {
                return;
            }

            OnSelectionChanged();
        }

        private void OnSelectionChanged()
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new EventArgs());
            }
        }

        private IEnumerable<HydraulicBoundaryLocation> GetSelectedHydraulicBoundaryLocationContext()
        {
            return GetHydraulicBoundaryLocationContextRows().Where(r => r.ToCalculate).Select(r => r.HydraulicBoundaryLocation);
        }

        #region Event handling

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            GetHydraulicBoundaryLocationContextRows().ForEachElementDo(row => row.ToCalculate = true);
            dataGridViewControl.RefreshDataGridView();
            UpdateCalculateForSelectedButton();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            GetHydraulicBoundaryLocationContextRows().ForEachElementDo(row => row.ToCalculate = false);
            dataGridViewControl.RefreshDataGridView();
            UpdateCalculateForSelectedButton();
        }

        private void CalculateForSelectedButton_Click(object sender, EventArgs e)
        {
            if (CalculationGuiService == null)
            {
                return;
            }
            var selectedLocations = GetSelectedHydraulicBoundaryLocationContext();
            HandleCalculateSelectedLocations(selectedLocations);
        }

        #endregion
    }
}