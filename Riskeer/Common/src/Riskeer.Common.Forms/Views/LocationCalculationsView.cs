// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// Base view for selecting and performing hydraulic boundary location calculations.
    /// </summary>
    /// <typeparam name="T">The type of the calculation objects.</typeparam>
    public abstract partial class LocationCalculationsView<T> : UserControl, ISelectionProvider, IView where T : class
    {
        private const int calculateColumnIndex = 0;

        private readonly Observer calculationsObserver;
        private readonly RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation> calculationObserver;

        private readonly IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations;
        private bool suspendAllEvents;
        private bool suspendIllustrationPointsControlSelectionChanges;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="LocationCalculationsView{T}"/>.
        /// </summary>
        protected LocationCalculationsView(IObservableEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                           IAssessmentSection assessmentSection)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            AssessmentSection = assessmentSection;

            calculationsObserver = new Observer(UpdateDataGridViewDataSource);
            calculationObserver = new RecursiveObserver<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, HydraulicBoundaryLocationCalculation>(
                HandleHydraulicBoundaryLocationCalculationUpdate, hblc => hblc);

            this.calculations = calculations;

            calculationsObserver.Observable = calculations;
            calculationObserver.Observable = calculations;

            InitializeComponent();
            LocalizeControls();
            InitializeEventHandlers();

            UpdateDataGridViewDataSource();
        }

        /// <summary>
        /// Gets or sets the <see cref="IAssessmentSection"/>.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="IHydraulicBoundaryLocationCalculationGuiService"/>.
        /// </summary>
        public IHydraulicBoundaryLocationCalculationGuiService CalculationGuiService { get; set; }

        public object Selection { get; private set; }

        public object Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeDataGridView();
        }

        /// <summary>
        /// Performs the selected <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The calculations to perform.</param>
        protected abstract void PerformSelectedCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations);

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
            dataGridViewControl.AddCheckBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.IncludeIllustrationPoints),
                                                  Resources.HydraulicBoundaryLocationCalculationInput_IncludeIllustrationPoints_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Name),
                                                 Resources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Id),
                                                 Resources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(HydraulicBoundaryLocationCalculationRow.Location),
                                                 Resources.HydraulicBoundaryDatabase_Location_Coordinates_DisplayName);
        }

        /// <summary>
        /// Creates a new object that is used as the object for <see cref="Selection"/> from
        /// the currently selected row in the data table.
        /// </summary>
        /// <returns>The newly created object.</returns>
        protected abstract object CreateSelectedItemFromCurrentRow();

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            calculationsObserver.Dispose();
            calculationObserver.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets all the row items from the <see cref="DataGridView"/>.
        /// </summary>
        protected IEnumerable<CalculatableRow<HydraulicBoundaryLocationCalculation>> GetCalculatableRows()
        {
            return dataGridViewControl.Rows
                                      .Cast<DataGridViewRow>()
                                      .Select(row => (CalculatableRow<HydraulicBoundaryLocationCalculation>) row.DataBoundItem);
        }

        /// <summary>
        /// Gets all the selected calculatable objects.
        /// </summary>
        private IEnumerable<HydraulicBoundaryLocationCalculation> GetSelectedCalculatableObjects()
        {
            return GetCalculatableRows().Where(r => r.ShouldCalculate)
                                        .Select(r => r.CalculatableObject);
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

        private void SetDataSource()
        {
            Dictionary<HydraulicBoundaryLocation, string> lookup = GetHydraulicBoundaryLocationLookup();
            dataGridViewControl.SetDataSource(calculations?.Select(c => CreateNewRow(c, lookup)).ToArray());
        }

        private static HydraulicBoundaryLocationCalculationRow CreateNewRow(HydraulicBoundaryLocationCalculation calculation,
                                                                            IReadOnlyDictionary<HydraulicBoundaryLocation, string> lookup)
        {
            return new HydraulicBoundaryLocationCalculationRow(calculation, lookup[calculation.HydraulicBoundaryLocation]);
        }

        private Dictionary<HydraulicBoundaryLocation, string> GetHydraulicBoundaryLocationLookup()
        {
            var lookup = new Dictionary<HydraulicBoundaryLocation, string>();
            foreach (HydraulicBoundaryDatabase database in AssessmentSection.HydraulicBoundaryData.HydraulicBoundaryDatabases)
            {
                foreach (HydraulicBoundaryLocation location in database.Locations)
                {
                    lookup[location] = Path.GetFileName(database.FilePath);
                }
            }

            return lookup;
        }

        private void CalculateForSelectedRows()
        {
            if (CalculationGuiService == null)
            {
                return;
            }

            PerformSelectedCalculations(GetSelectedCalculatableObjects());
        }

        private string ValidateCalculatableObjects()
        {
            if (!GetCalculatableRows().Any(r => r.ShouldCalculate))
            {
                return Resources.CalculatableViews_No_calculations_selected;
            }

            return null;
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
            HideHydraulicBoundaryDatabaseColumnCheckBox.Text = Resources.LocationCalculationsView_HideHydraulicBoundaryDatabaseColumnCheckBox_Text;
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

        private IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;
            if (currentRow == null)
            {
                return Enumerable.Empty<IllustrationPointControlItem>();
            }

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = ((HydraulicBoundaryLocationCalculationRow) currentRow.DataBoundItem).CalculatableObject;

            HydraulicBoundaryLocationCalculationOutput hydraulicBoundaryLocationCalculationOutput = hydraulicBoundaryLocationCalculation.Output;
            if (hydraulicBoundaryLocationCalculation.HasOutput
                && hydraulicBoundaryLocationCalculationOutput.HasGeneralResult)
            {
                return hydraulicBoundaryLocationCalculationOutput.GeneralResult.TopLevelIllustrationPoints.Select(
                    topLevelSubMechanismIllustrationPoint =>
                    {
                        SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                            topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint;
                        return new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint,
                                                                topLevelSubMechanismIllustrationPoint.WindDirection.Name,
                                                                topLevelSubMechanismIllustrationPoint.ClosingSituation,
                                                                subMechanismIllustrationPoint.Stochasts,
                                                                subMechanismIllustrationPoint.Beta);
                    }).ToArray();
            }

            return Enumerable.Empty<IllustrationPointControlItem>();
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