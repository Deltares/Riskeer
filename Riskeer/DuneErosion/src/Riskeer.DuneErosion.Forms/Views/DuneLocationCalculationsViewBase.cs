// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Forms.Properties;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// Base view for selecting dune location calculations and starting a calculation for said objects.
    /// </summary>
    public abstract partial class DuneLocationCalculationsViewBase : UserControl, ISelectionProvider, IView
    {
        private const int calculateColumnIndex = 0;
        private bool updatingDataSource;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsViewBase"/>.
        /// </summary>
        protected DuneLocationCalculationsViewBase()
        {
            InitializeComponent();
            LocalizeControls();
            InitializeEventHandlers();
        }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }

        public abstract object Data { get; set; }

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
            dataGridViewControl.AddCheckBoxColumn(nameof(DuneLocationCalculationRow.ShouldCalculate),
                                                  Resources.CalculatableView_Calculate);
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

        /// <summary>
        /// Gets all the row items from the <see cref="DataGridView"/>.
        /// </summary>
        protected IEnumerable<DuneLocationCalculationRow> GetCalculatableRows()
        {
            return dataGridViewControl.Rows
                                      .Cast<DataGridViewRow>()
                                      .Select(row => (DuneLocationCalculationRow) row.DataBoundItem);
        }

        /// <summary>
        /// Gets all the selected calculatable objects.
        /// </summary>
        protected IEnumerable<DuneLocationCalculation> GetSelectedCalculatableObjects()
        {
            return GetCalculatableRows().Where(r => r.ShouldCalculate)
                                        .Select(r => r.CalculatableObject);
        }

        /// <summary>
        /// Validates the calculatable objects.
        /// </summary>
        /// <returns>A validation message in case no calculations can be performed, <c>null</c> otherwise.</returns>
        protected virtual string ValidateCalculatableObjects()
        {
            if (!GetCalculatableRows().Any(r => r.ShouldCalculate))
            {
                return Resources.CalculatableViews_No_calculations_selected;
            }

            return null;
        }

        /// <summary>
        /// Updates the state of the calculation button and the corresponding error provider.
        /// </summary>
        protected void UpdateCalculateForSelectedButton()
        {
            string validationText = ValidateCalculatableObjects();
            if (!string.IsNullOrEmpty(validationText))
            {
                CalculateForSelectedButton.Enabled = false;
                CalculateForSelectedButtonErrorProvider.SetError(CalculateForSelectedButton, validationText);
            }
            else
            {
                CalculateForSelectedButton.Enabled = true;
                CalculateForSelectedButtonErrorProvider.SetError(CalculateForSelectedButton, "");
            }
        }

        private void LocalizeControls()
        {
            CalculateForSelectedButton.Text = Resources.CalculatableView_CalculateForSelectedButton_Text;
            DeselectAllButton.Text = Resources.CalculatableView_DeselectAllButton_Text;
            SelectAllButton.Text = Resources.CalculatableView_SelectAllButton_Text;
            ButtonGroupBox.Text = Resources.CalculatableView_ButtonGroupBox_Text;
        }

        private void InitializeEventHandlers()
        {
            dataGridViewControl.CurrentRowChanged += DataGridViewOnCurrentRowChangedHandler;
            dataGridViewControl.CellValueChanged += DataGridViewCellValueChanged;
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        #region Event handling

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!updatingDataSource && e.ColumnIndex == calculateColumnIndex)
            {
                UpdateCalculateForSelectedButton();
            }
        }

        private void DataGridViewOnCurrentRowChangedHandler(object sender, EventArgs e)
        {
            OnSelectionChanged();
        }

        private void SelectAllButton_Click(object sender, EventArgs e)
        {
            SetShouldCalculateForAllRowsAndRefresh(true);
        }

        private void DeselectAllButton_Click(object sender, EventArgs e)
        {
            SetShouldCalculateForAllRowsAndRefresh(false);
        }

        private void SetShouldCalculateForAllRowsAndRefresh(bool newShouldCalculateValue)
        {
            GetCalculatableRows().ForEachElementDo(row => row.ShouldCalculate = newShouldCalculateValue);
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