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
using Riskeer.Common.Forms.Properties;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.GuiServices;

namespace Riskeer.DuneErosion.Forms.Views
{
    /// <summary>
    /// Base view for selecting dune location calculations and starting a calculation for said objects.
    /// </summary>
    public abstract partial class DuneLocationCalculationsViewBase : UserControl, ISelectionProvider, IView
    {
        private readonly Observer failureMechanismObserver;
        private readonly Observer duneLocationCalculationsObserver;
        private readonly IObservableEnumerable<DuneLocationCalculation> calculations;
        private readonly Func<double> getTargetProbabilityFunc;
        private readonly Func<string> getCalculationIdentifierFunc;
        private readonly RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation> duneLocationCalculationObserver;
        
        private const int calculateColumnIndex = 0;
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
        protected DuneLocationCalculationsViewBase(IObservableEnumerable<DuneLocationCalculation> calculations,
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

            duneLocationCalculationObserver = new RecursiveObserver<IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculation>(() => dataGridViewControl.RefreshDataGridView(), list => list)
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
        }

        public object Selection
        {
            get
            {
                return CreateSelectedItemFromCurrentRow();
            }
        }

        public abstract object Data { get; set; }
        
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
            
            duneLocationCalculationsObserver.Dispose();
            duneLocationCalculationObserver.Dispose();
            failureMechanismObserver.Dispose();

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