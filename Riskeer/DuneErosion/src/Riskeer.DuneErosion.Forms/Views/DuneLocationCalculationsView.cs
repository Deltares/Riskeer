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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.DuneErosion.Forms.PresentationObjects.HydraulicLoadsState;
using Riskeer.DuneErosion.Forms.Properties;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// View for selecting dune location calculations and starting a calculation for said objects.
    /// </summary>
    public partial class DuneLocationCalculationsView : UserControl, ISelectionProvider, IView
    {
        private const int calculateColumnIndex = 0;
        private const int hydraulicBoundaryDatabaseFileNameColumnIndex = 3;
        private readonly Observer failureMechanismObserver;
        private readonly Observer duneLocationCalculationsObserver;
        private readonly IObservableEnumerable<DuneLocationCalculation> calculations;
        private readonly Func<double> getTargetProbabilityFunc;
        private readonly Func<string> getCalculationIdentifierFunc;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> duneLocationCalculationObserver;

        private bool updatingDataSource;
        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationsView"/>.
        /// </summary>
        /// <param name="calculations">The calculations to show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism which the calculations belong to.</param>
        /// <param name="assessmentSection">The assessment section which the calculations belong to.</param>
        /// <param name="getTargetProbabilityFunc"><see cref="Func{TResult}"/> for getting the target probability to use during calculations.</param>
        /// <param name="getCalculationIdentifierFunc"><see cref="Func{TResult}"/> for getting the calculation identifier to use in all messages.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DuneLocationCalculationsView(IObservableEnumerable<DuneLocationCalculation> calculations,
                                            DuneErosionFailureMechanism failureMechanism,
                                            IAssessmentSection assessmentSection,
                                            Func<double> getTargetProbabilityFunc,
                                            Func<string> getCalculationIdentifierFunc)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (getTargetProbabilityFunc == null)
            {
                throw new ArgumentNullException(nameof(getTargetProbabilityFunc));
            }

            if (getCalculationIdentifierFunc == null)
            {
                throw new ArgumentNullException(nameof(getCalculationIdentifierFunc));
            }

            this.calculations = calculations;
            this.getTargetProbabilityFunc = getTargetProbabilityFunc;
            this.getCalculationIdentifierFunc = getCalculationIdentifierFunc;
            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;

            duneLocationCalculationsObserver = new Observer(UpdateDataGridViewDataSource)
            {
                Observable = calculations
            };

            duneLocationCalculationObserver = new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(
                () => dataGridViewControl.RefreshDataGridView(), list => list)
            {
                Observable = calculations
            };

            failureMechanismObserver = new Observer(UpdateCalculateForSelectedButton)
            {
                Observable = failureMechanism
            };

            InitializeComponent();
            LocalizeControls();
            InitializeEventHandlers();

            UpdateDataGridViewDataSource();
        }

        /// <summary>
        /// Gets or sets the <see cref="DuneLocationCalculationGuiService"/> 
        /// to perform calculations with.
        /// </summary>
        public DuneLocationCalculationGuiService CalculationGuiService { get; set; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        public IAssessmentSection AssessmentSection { get; }

        /// <summary>
        /// Gets the <see cref="DuneErosionFailureMechanism"/> for which the
        /// calculations are shown.
        /// </summary>
        public DuneErosionFailureMechanism FailureMechanism { get; }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }

        public object Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitializeDataGridView();
            UpdateDataGridViewColumnVisibility();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            duneLocationCalculationsObserver.Dispose();
            duneLocationCalculationObserver.Dispose();
            failureMechanismObserver.Dispose();

            base.Dispose(disposing);
        }

        private void UpdateDataGridViewDataSource()
        {
            updatingDataSource = true;
            SetDataSource();
            updatingDataSource = false;
            UpdateCalculateForSelectedButton();
        }

        private void UpdateDataGridViewColumnVisibility()
        {
            dataGridViewControl.GetColumnFromIndex(hydraulicBoundaryDatabaseFileNameColumnIndex).Visible =
                showHydraulicBoundaryDatabaseFileNameColumnCheckBox.Checked;
        }

        private void InitializeDataGridView()
        {
            dataGridViewControl.AddCheckBoxColumn(nameof(DuneLocationCalculationRow.ShouldCalculate),
                                                  RiskeerCommonFormsResources.LocationCalculationsView_Calculate);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Name),
                                                 RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Name_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Id),
                                                 RiskeerCommonFormsResources.HydraulicBoundaryDatabase_Location_Id_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.HydraulicBoundaryDatabaseFileName),
                                                 RiskeerCommonFormsResources.HydraulicBoundaryDatabase_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.CoastalAreaId),
                                                 Resources.DuneLocation_CoastalAreaId_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.Offset),
                                                 Resources.DuneLocation_Offset_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WaterLevel),
                                                 Resources.DuneLocationCalculationOutput_WaterLevel_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WaveHeight),
                                                 Resources.DuneLocationCalculationOutput_WaveHeight_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WavePeriod),
                                                 Resources.DuneLocationCalculationOutput_WavePeriod_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.MeanTidalAmplitude),
                                                 Resources.DuneLocationCalculationOutput_MeanTidalAmplitude_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.WaveDirectionalSpread),
                                                 Resources.DuneLocationCalculationOutput_WaveDirectionalSpread_DisplayName);
            dataGridViewControl.AddTextBoxColumn(nameof(DuneLocationCalculationRow.TideSurgePhaseDifference),
                                                 Resources.DuneLocationCalculationOutput_TideSurgePhaseDifference_DisplayName);
        }

        private IEnumerable<DuneLocationCalculationRow> GetCalculatableRows()
        {
            return dataGridViewControl.Rows
                                      .Cast<DataGridViewRow>()
                                      .Select(row => (DuneLocationCalculationRow) row.DataBoundItem);
        }

        private IEnumerable<DuneLocationCalculation> GetSelectedCalculatableObjects()
        {
            return GetCalculatableRows().Where(r => r.ShouldCalculate)
                                        .Select(r => r.CalculatableObject);
        }

        private string ValidateCalculatableObjects()
        {
            if (!GetCalculatableRows().Any(r => r.ShouldCalculate))
            {
                return RiskeerCommonFormsResources.LocationCalculationsView_No_calculations_selected;
            }

            return null;
        }

        private void UpdateCalculateForSelectedButton()
        {
            string validationText = ValidateCalculatableObjects();
            if (!string.IsNullOrEmpty(validationText))
            {
                calculateForSelectedButton.Enabled = false;
                calculateForSelectedButtonErrorProvider.SetError(calculateForSelectedButton, validationText);
            }
            else
            {
                calculateForSelectedButton.Enabled = true;
                calculateForSelectedButtonErrorProvider.SetError(calculateForSelectedButton, "");
            }
        }

        private void LocalizeControls()
        {
            calculateForSelectedButton.Text = RiskeerCommonFormsResources.LocationCalculationsView_CalculateForSelectedButton_Text;
            deselectAllButton.Text = RiskeerCommonFormsResources.LocationCalculationsView_DeselectAllButton_Text;
            selectAllButton.Text = RiskeerCommonFormsResources.LocationCalculationsView_SelectAllButton_Text;
            buttonGroupBox.Text = RiskeerCommonFormsResources.LocationCalculationsView_ButtonGroupBox_Text;
            showHydraulicBoundaryDatabaseFileNameColumnCheckBox.Text = RiskeerCommonFormsResources.LocationCalculationsView_ShowHydraulicBoundaryDatabaseColumnCheckBox_Text;
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

        private object CreateSelectedItemFromCurrentRow()
        {
            DataGridViewRow currentRow = dataGridViewControl.CurrentRow;

            if (currentRow != null)
            {
                DuneLocationCalculation calculation = ((DuneLocationCalculationRow) currentRow.DataBoundItem).CalculatableObject;
                return new DuneLocationCalculationContext(calculation, AssessmentSection);
            }

            return null;
        }

        private void SetDataSource()
        {
            IReadOnlyDictionary<HydraulicBoundaryLocation, string> lookup = AssessmentSection.GetHydraulicBoundaryLocationLookup();
            dataGridViewControl.SetDataSource(
                calculations?.Select(calc => new DuneLocationCalculationRow(calc, lookup[calc.DuneLocation.HydraulicBoundaryLocation]))
                            .ToArray());
        }

        private void CalculateForSelectedRows()
        {
            CalculationGuiService?.Calculate(GetSelectedCalculatableObjects(),
                                             AssessmentSection,
                                             getTargetProbabilityFunc(),
                                             getCalculationIdentifierFunc());
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

        private void ShowHydraulicBoundaryDatabaseFileNameColumnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDataGridViewColumnVisibility();
        }

        #endregion
    }
}