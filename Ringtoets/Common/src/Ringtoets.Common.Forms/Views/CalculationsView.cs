// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Base view for selecting and performing hydraulic boundary location calculations.
    /// </summary>
    /// <typeparam name="T">The type of the calculation objects.</typeparam>
    public abstract partial class CalculationsView<T> : UserControl, ISelectionProvider, IView where T : class
    {
        private const int calculateColumnIndex = 0;
        private bool suspendAllEvents;
        private bool suspendIllustrationPointsControlSelectionChanges;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationsView{T}"/>.
        /// </summary>
        protected CalculationsView()
        {
            InitializeComponent();
            LocalizeControls();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets or sets the <see cref="IAssessmentSection"/>.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; protected set; }

        public object Selection { get; private set; }

        public abstract object Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeDataGridView();
        }

        /// <summary>
        /// Updates the data source of the data table based on the <see cref="Data"/>.
        /// </summary>
        protected void UpdateDataGridViewDataSource()
        {
            suspendAllEvents = true;
            SetDataSource();
            illustrationPointsControl.Data = GetIllustrationPointControlItems();
            suspendAllEvents = false;

            UpdateCalculateForSelectedButton();
            ProvideCalculationSelection();
        }

        /// <summary>
        /// Initializes the <see cref="DataGridView"/>.
        /// </summary>
        protected virtual void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(nameof(CalculatableRow<T>.ShouldCalculate),
                                                  Resources.CalculatableView_Calculate);
        }

        /// <summary>
        /// Creates a new object that is used as the object for <see cref="Selection"/> from
        /// the currently selected row in the data table.
        /// </summary>
        /// <returns>The newly created object.</returns>
        protected abstract object CreateSelectedItemFromCurrentRow();

        /// <summary>
        /// Sets the data source on the <see cref="DataGridView"/>.
        /// </summary>
        protected abstract void SetDataSource();

        /// <summary>
        /// Handles the calculation routine for the currently selected rows.
        /// </summary>
        protected abstract void CalculateForSelectedRows();

        /// <summary>
        /// Gets all the row items from the <see cref="DataGridView"/>.
        /// </summary>
        protected IEnumerable<CalculatableRow<T>> GetCalculatableRows()
        {
            return dataGridViewControl.Rows
                                      .Cast<DataGridViewRow>()
                                      .Select(row => (CalculatableRow<T>) row.DataBoundItem);
        }

        /// <summary>
        /// Gets all the selected calculatable objects.
        /// </summary>
        protected IEnumerable<T> GetSelectedCalculatableObjects()
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
        /// Handles the update of a hydraulic boundary location calculation by refreshing the data grid view
        /// and updating the data of the illustration points control.
        /// </summary>
        protected void HandleHydraulicBoundaryLocationCalculationUpdate()
        {
            suspendAllEvents = true;
            dataGridViewControl.RefreshDataGridView();
            HandlePossibleOutdatedIllustrationPointsControl();
            suspendAllEvents = false;

            HandlePossibleOutdatedIllustrationPointsSelection();
        }

        private void UpdateCalculateForSelectedButton()
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

        private void HandlePossibleOutdatedIllustrationPointsControl()
        {
            IEnumerable<IllustrationPointControlItem> illustrationPointControlItems = GetIllustrationPointControlItems();

            if (illustrationPointsControl.Data.Count() != illustrationPointControlItems.Count())
            {
                illustrationPointsControl.Data = illustrationPointControlItems;
            }
        }

        private void HandlePossibleOutdatedIllustrationPointsSelection()
        {
            if (illustrationPointsControl.Selection == null && Selection is SelectedTopLevelSubMechanismIllustrationPoint)
            {
                ProvideCalculationSelection();
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
            dataGridViewControl.CurrentRowChanged += DataGridViewControlOnCurrentRowChanged;
            dataGridViewControl.CellValueChanged += DataGridViewControlOnCellValueChanged;
            illustrationPointsControl.SelectionChanged += IllustrationPointsControlOnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void ProvideCalculationSelection()
        {
            Selection = CreateSelectedItemFromCurrentRow();
            OnSelectionChanged();
        }

        #region Event handling

        private void DataGridViewControlOnCurrentRowChanged(object sender, EventArgs e)
        {
            if (suspendAllEvents)
            {
                return;
            }

            suspendIllustrationPointsControlSelectionChanges = true;
            illustrationPointsControl.Data = GetIllustrationPointControlItems();
            suspendIllustrationPointsControlSelectionChanges = false;

            ProvideCalculationSelection();
        }

        private void DataGridViewControlOnCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (suspendAllEvents || e.ColumnIndex != calculateColumnIndex)
            {
                return;
            }

            UpdateCalculateForSelectedButton();
        }

        private void IllustrationPointsControlOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            if (suspendAllEvents || suspendIllustrationPointsControlSelectionChanges)
            {
                return;
            }

            var selection = illustrationPointsControl.Selection as IllustrationPointControlItem;
            Selection = selection != null
                            ? new SelectedTopLevelSubMechanismIllustrationPoint((TopLevelSubMechanismIllustrationPoint) selection.Source,
                                                                                GetIllustrationPointControlItems().Select(ipci => ipci.ClosingSituation))
                            : null;

            OnSelectionChanged();
        }

        /// <summary>
        /// Gets the illustration point control items based on the data of the illustration points.
        /// </summary>
        /// <returns>The illustration point control items if it has obtained as part of the calculation, <c>null</c>
        /// otherwise.</returns>
        protected abstract IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems();

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