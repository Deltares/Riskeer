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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Base view for calculatable objects such as <see cref="HydraulicBoundaryLocation"/> views 
    /// which should be derived in order to get a consistent look and feel.
    /// </summary>
    public abstract partial class CalculatableView : UserControl, ISelectionProvider
    {
        private const int calculateColumnIndex = 0;
        private bool updatingDataSource;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="CalculatableView"/>.
        /// </summary>
        protected CalculatableView()
        {
            InitializeComponent();
            LocalizeControls();
            InitializeEventHandlers();
        }

        public abstract object Data { get; set; }

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
            if (disposing)
            {
                components?.Dispose();
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
            dataGridViewControl.AddCheckBoxColumn(TypeUtils.GetMemberName<CalculatableRow>(row => row.ToCalculate),
                                                  Resources.HydraulicBoundaryLocationsView_Calculate);
        }

        /// <summary>
        /// Creates a new object that is used as the object for <see cref="Selection"/> from
        /// the currently selected row in the data table.
        /// </summary>
        /// <returns>The newly created object.</returns>
        protected abstract object CreateSelectedItemFromCurrentRow();

        /// <summary>
        /// Sets the datasource on the <see cref="DataGridView"/>.
        /// </summary>
        protected abstract void SetDataSource();

        /// <summary>
        /// Handles the calculation routine for the currently selected rows.
        /// </summary>
        protected abstract void CalculateForSelectedRows();

        protected IEnumerable<CalculatableRow> GetCalculatableRows()
        {
            return dataGridViewControl.Rows.Cast<DataGridViewRow>().Select(row => (CalculatableRow) row.DataBoundItem);
        }

        private void LocalizeControls()
        {
            CalculateForSelectedButton.Text = Resources.HydraulicBoundaryLocationsView_CalculateForSelectedButton_Text;
            DeselectAllButton.Text = Resources.HydraulicBoundaryLocationsView_DeselectAllButton_Text;
            SelectAllButton.Text = Resources.HydraulicBoundaryLocationsView_SelectAllButton_Text;
            ButtonGroupBox.Text = Resources.HydraulicBoundaryLocationsView_ButtonGroupBox_Text;
        }

        private void UpdateCalculateForSelectedButton()
        {
            CalculateForSelectedButton.Enabled = GetCalculatableRows().Any(r => r.ToCalculate);
        }

        private void InitializeEventHandlers()
        {
            dataGridViewControl.AddCurrentCellChangedHandler(DataGridViewOnCurrentCellChangedHandler);
            dataGridViewControl.AddCellValueChangedHandler(DataGridViewCellValueChanged);
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        #region Event handling

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (updatingDataSource || e.ColumnIndex != calculateColumnIndex)
            {
                return;
            }
            UpdateCalculateForSelectedButton();
        }

        private void DataGridViewOnCurrentCellChangedHandler(object sender, EventArgs e)
        {
            if (updatingDataSource)
            {
                return;
            }

            OnSelectionChanged();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            GetCalculatableRows().ForEachElementDo(row => row.ToCalculate = true);
            dataGridViewControl.RefreshDataGridView();
            UpdateCalculateForSelectedButton();
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            GetCalculatableRows().ForEachElementDo(row => row.ToCalculate = false);
            dataGridViewControl.RefreshDataGridView();
            UpdateCalculateForSelectedButton();
        }

        private void CalculateForSelectedButton_Click(object sender, EventArgs e)
        {
            CalculateForSelectedRows();
        }

        #endregion
    }
}