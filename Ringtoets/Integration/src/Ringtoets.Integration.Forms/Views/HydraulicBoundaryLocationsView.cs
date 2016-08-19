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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Commands;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// Base view for <see cref="HydraulicBoundaryLocation"/> views which should be derived in order to get a consistent look and feel.
    /// </summary>
    public abstract partial class HydraulicBoundaryLocationsView : UserControl, ISelectionProvider
    {
        private const int locationCalculateColumnIndex = 0;
        private readonly Observer assessmentSectionObserver;
        private readonly Observer hydraulicBoundaryDatabaseObserver;
        protected IAssessmentSection AssessmentSection;
        private bool updatingDataSource;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationsView"/>.
        /// </summary>
        protected HydraulicBoundaryLocationsView()
        {
            InitializeComponent();
            InitializeEventHandlers();

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
                return AssessmentSection;
            }
            set
            {
                AssessmentSection = value as IAssessmentSection;

                UpdateDataGridViewDataSource();
                assessmentSectionObserver.Observable = AssessmentSection;
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

        /// <summary>
        /// Sets the datasource on the <see cref="DataGridView"/>.
        /// </summary>
        protected abstract void SetDataSource();

        private IEnumerable<HydraulicBoundaryLocationContextRow> GetHydraulicBoundaryLocationContextRows()
        {
            return dataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => (HydraulicBoundaryLocationContextRow) row.DataBoundItem);
        }

        private void SetHydraulicBoundaryDatabaseObserver()
        {
            hydraulicBoundaryDatabaseObserver.Observable = AssessmentSection != null ? AssessmentSection.HydraulicBoundaryDatabase : null;
        }

        private void UpdateDataGridViewDataSource()
        {
            SetHydraulicBoundaryDatabaseObserver();

            updatingDataSource = true;
            SetDataSource();
            updatingDataSource = false;
            UpdateCalculateForSelectedButton();
        }

        private void UpdateCalculateForSelectedButton()
        {
            CalculateForSelectedButton.Enabled = GetHydraulicBoundaryLocationContextRows().Any(r => r.ToCalculate);
        }

        #region Event handling

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

            var locationContextRow = currentRow != null
                                         ? (HydraulicBoundaryLocationContextRow) currentRow.DataBoundItem
                                         : null;

            return locationContextRow != null
                       ? locationContextRow.HydraulicBoundaryLocationContext
                       : null;
        }

        private IEnumerable<HydraulicBoundaryLocation> GetSelectedHydraulicBoundaryLocationContext()
        {
            return GetHydraulicBoundaryLocationContextRows().Where(r => r.ToCalculate).Select(r => r.HydraulicBoundaryLocationContext.HydraulicBoundaryLocation);
        }

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
            if (CalculationCommandHandler == null)
            {
                return;
            }
            var locations = GetSelectedHydraulicBoundaryLocationContext();
            HandleCalculateSelectedLocations(locations);
        }

        /// <summary>
        /// Handles the calculation of the <paramref name="locations"/>.
        /// </summary>
        /// <param name="locations">The enumeration of <see cref="HydraulicBoundaryLocation"/> to use in the calculation.</param>
        protected abstract void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations);

        #endregion
    }
}