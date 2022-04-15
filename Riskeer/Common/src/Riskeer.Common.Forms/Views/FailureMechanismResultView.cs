// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Forms.Exceptions;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="TSectionResult">The type of results which are presented by the 
    /// <see cref="FailureMechanismResultView{TSectionResult,TSectionResultRow,TFailureMechanism}"/>.</typeparam>
    /// <typeparam name="TSectionResultRow">The type of the row that is used to show the data.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism this view belongs to.</typeparam>
    public abstract partial class FailureMechanismResultView<TSectionResult, TSectionResultRow, TFailureMechanism> : UserControl, IView
        where TSectionResult : FailureMechanismSectionResult
        where TSectionResultRow : FailureMechanismSectionResultRow<TSectionResult>
        where TFailureMechanism : IFailureMechanism<TSectionResult>
    {
        private readonly IObservableEnumerable<TSectionResult> failureMechanismSectionResults;
        private readonly Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc;
        private readonly Observer failureMechanismObserver;
        private readonly Observer failureMechanismSectionResultObserver;
        private readonly RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult> failureMechanismSectionResultsObserver;

        private IEnumerable<TSectionResultRow> sectionResultRows;
        private bool rowUpdating;

        private bool probabilityResultTypeComboBoxUpdating;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView{TSectionResult,TSectionResultRow,TFailureMechanism}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <typeparamref name="TSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belong to.</param>
        /// <param name="assessmentSection">The assessment section the results belong to.</param>
        /// <param name="performFailureMechanismAssemblyFunc">The function to perform an assembly on the failure mechanism.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected FailureMechanismResultView(IObservableEnumerable<TSectionResult> failureMechanismSectionResults,
                                             TFailureMechanism failureMechanism,
                                             IAssessmentSection assessmentSection,
                                             Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> performFailureMechanismAssemblyFunc)
        {
            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (performFailureMechanismAssemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(performFailureMechanismAssemblyFunc));
            }

            InitializeComponent();

            FailureMechanism = failureMechanism;
            AssessmentSection = assessmentSection;
            this.failureMechanismSectionResults = failureMechanismSectionResults;
            this.performFailureMechanismAssemblyFunc = performFailureMechanismAssemblyFunc;

            failureMechanismObserver = new Observer(UpdateInternalViewData)
            {
                Observable = failureMechanism
            };

            failureMechanismSectionResultObserver = new Observer(UpdateInternalViewData)
            {
                Observable = failureMechanismSectionResults
            };

            failureMechanismSectionResultsObserver = new RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult>(
                UpdateSectionResultRows,
                sr => sr)
            {
                Observable = failureMechanismSectionResults
            };

            InitializeComboBox();
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public TFailureMechanism FailureMechanism { get; }

        public object Data { get; set; }

        /// <summary>
        /// Gets the assessment section.
        /// </summary>
        protected IAssessmentSection AssessmentSection { get; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddDataGridColumns();

            DataGridViewControl.CellFormatting += HandleCellStyling;

            UpdateInternalViewData();
        }

        /// <summary>
        /// Creates a display object for <paramref name="sectionResult"/> which is added to the
        /// <see cref="DataGridView"/> on the <see cref="FailureMechanismResultView{TSectionResult,TSectionResultRow,TFailureMechanism}"/>.
        /// </summary>
        /// <param name="sectionResult">The <typeparamref name="TSectionResult"/> for which to create a
        /// display object.</param>
        /// <returns>A display object which can be added as a row to the <see cref="DataGridView"/>.</returns>
        protected abstract TSectionResultRow CreateFailureMechanismSectionResultRow(TSectionResult sectionResult);

        protected override void Dispose(bool disposing)
        {
            failureMechanismObserver.Dispose();
            failureMechanismSectionResultObserver.Dispose();
            failureMechanismSectionResultsObserver.Dispose();

            DataGridViewControl.CellFormatting -= HandleCellStyling;

            RemoveSectionResultRowEvents();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Adds the columns to the view.
        /// </summary>
        protected abstract void AddDataGridColumns();

        /// <summary>
        /// Updates the internal data in the view.
        /// </summary>
        protected virtual void UpdateInternalViewData()
        {
            UpdateDataGridViewDataSource();
            UpdateAssemblyData();
            UpdateFailureMechanismAssemblyResultControls();
        }

        private void RefreshDataGrid()
        {
            DataGridViewControl.RefreshDataGridView(false);
        }

        private void InitializeComboBox()
        {
            IEnumerable<EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>> dataSource =
                Enum.GetValues(typeof(FailureMechanismAssemblyProbabilityResultType))
                    .Cast<FailureMechanismAssemblyProbabilityResultType>()
                    .Select(e => new EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>(e))
                    .ToArray();

            probabilityResultTypeComboBox.BeginUpdate();

            probabilityResultTypeComboBoxUpdating = true;
            probabilityResultTypeComboBox.DataSource = dataSource;
            probabilityResultTypeComboBox.ValueMember = nameof(EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>.Value);
            probabilityResultTypeComboBox.DisplayMember = nameof(EnumDisplayWrapper<FailureMechanismAssemblyProbabilityResultType>.DisplayName);
            probabilityResultTypeComboBox.SelectedValue = FailureMechanism.AssemblyResult.ProbabilityResultType;
            probabilityResultTypeComboBoxUpdating = false;

            probabilityResultTypeComboBox.EndUpdate();
        }

        private void UpdateFailureMechanismAssemblyResultControls()
        {
            probabilityResultTypeComboBox.Enabled = HasSections();

            bool isManualAssembly = FailureMechanism.AssemblyResult.IsManualProbability();
            failureMechanismAssemblyProbabilityTextBox.Enabled = isManualAssembly && HasSections();
            failureMechanismAssemblyProbabilityTextBox.ReadOnly = !isManualAssembly || !HasSections();
            failureMechanismAssemblyProbabilityTextBox.Refresh();
        }

        private bool HasSections()
        {
            return FailureMechanism.Sections.Any();
        }

        private void UpdateAssemblyData()
        {
            ClearErrorMessage();

            FailureMechanismAssemblyResult assemblyResult = FailureMechanism.AssemblyResult;
            double failureMechanismAssemblyProbability = assemblyResult.IsManualProbability()
                                                             ? assemblyResult.ManualFailureMechanismAssemblyProbability
                                                             : TryGetFailureMechanismAssemblyProbability();
            SetTextBoxValue(failureMechanismAssemblyProbability);
        }

        /// <summary>
        /// Updates the data source of the data grid view with the current known failure mechanism section results.
        /// </summary>
        private void UpdateDataGridViewDataSource()
        {
            DataGridViewControl.EndEdit();

            RemoveSectionResultRowEvents();

            sectionResultRows = failureMechanismSectionResults
                                .Select(CreateFailureMechanismSectionResultRow)
                                .Where(sr => sr != null)
                                .ToList();
            DataGridViewControl.SetDataSource(sectionResultRows);

            sectionResultRows.ForEachElementDo(row =>
            {
                row.RowUpdated += RowUpdated;
                row.RowUpdateDone += RowUpdateDone;
            });
        }

        private void RemoveSectionResultRowEvents()
        {
            sectionResultRows?.ForEachElementDo(row =>
            {
                row.RowUpdated -= RowUpdated;
                row.RowUpdateDone -= RowUpdateDone;
            });
        }

        private void RowUpdateDone(object sender, EventArgs eventArgs)
        {
            rowUpdating = false;
        }

        private void RowUpdated(object sender, EventArgs eventArgs)
        {
            rowUpdating = true;
            RefreshDataGrid();
            UpdateAssemblyData();
        }

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewControl.FormatCellWithColumnStateDefinition(e.RowIndex, e.ColumnIndex);
        }

        private void UpdateSectionResultRows()
        {
            if (rowUpdating)
            {
                return;
            }

            sectionResultRows.ForEachElementDo(row => row.Update());
            DataGridViewControl.RefreshDataGridView();

            UpdateAssemblyData();
        }

        private double TryGetFailureMechanismAssemblyProbability()
        {
            try
            {
                return performFailureMechanismAssemblyFunc(FailureMechanism, AssessmentSection).AssemblyResult;
            }
            catch (AssemblyException e)
            {
                SetErrorMessage(e.Message);
                return double.NaN;
            }
        }

        private void ProbabilityResultTypeComboBoxSelectedIndexChanged(object sender, EventArgs e)
        {
            if (probabilityResultTypeComboBoxUpdating || probabilityResultTypeComboBox.SelectedIndex == -1)
            {
                return;
            }

            FailureMechanismAssemblyResult assemblyResult = FailureMechanism.AssemblyResult;
            assemblyResult.ProbabilityResultType = (FailureMechanismAssemblyProbabilityResultType) probabilityResultTypeComboBox.SelectedValue;
            assemblyResult.NotifyObservers();

            UpdateAssemblyData();
            UpdateFailureMechanismAssemblyResultControls();
        }

        private void FailureMechanismAssemblyProbabilityTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                failureMechanismAssemblyLabel.Focus(); // Focus on different component to raise a leave event on the text box
                e.Handled = true;
            }

            if (e.KeyCode == Keys.Escape)
            {
                ClearErrorMessage();
                SetTextBoxValue(FailureMechanism.AssemblyResult.ManualFailureMechanismAssemblyProbability);
                e.Handled = true;
            }
        }

        private void FailureMechanismAssemblyProbabilityTextBoxLeave(object sender, EventArgs e)
        {
            ClearErrorMessage();
            ProcessFailureMechanismAssemblyProbabilityTextBox();
        }

        private void ProcessFailureMechanismAssemblyProbabilityTextBox()
        {
            FailureMechanismAssemblyResult assemblyResult = FailureMechanism.AssemblyResult;
            if (!assemblyResult.IsManualProbability())
            {
                return;
            }

            try
            {
                double probability = ProbabilityParsingHelper.Parse(failureMechanismAssemblyProbabilityTextBox.Text);
                assemblyResult.ManualFailureMechanismAssemblyProbability = probability;
                assemblyResult.NotifyObservers();

                SetTextBoxValue(probability);
            }
            catch (Exception exception) when (exception is ArgumentOutOfRangeException
                                              || exception is ProbabilityParsingException)
            {
                SetErrorMessage(exception.Message);
                failureMechanismAssemblyProbabilityTextBox.Focus();
            }
        }

        private void SetTextBoxValue(double probability)
        {
            failureMechanismAssemblyProbabilityTextBox.Text = ProbabilityFormattingHelper.FormatWithDiscreteNumbers(probability);

            FailureMechanismAssemblyResult assemblyResult = FailureMechanism.AssemblyResult;
            bool hasManualProbability = assemblyResult.IsManualProbability();
            if (hasManualProbability && !HasSections())
            {
                SetErrorMessage(Resources.FailureMechanismResultView_To_Enter_An_AssemblyProbability_Failure_Mechanism_Sections_Must_Be_Imported);
            }
            else if (hasManualProbability)
            {
                SetErrorMessage(FailureMechanismAssemblyResultValidationHelper.GetValidationError(assemblyResult));
            }
        }

        private void SetErrorMessage(string errorMessage)
        {
            errorProvider.SetIconPadding(failureMechanismAssemblyProbabilityTextBox, 5);
            errorProvider.SetError(failureMechanismAssemblyProbabilityTextBox, errorMessage);
        }

        private void ClearErrorMessage()
        {
            errorProvider.SetError(failureMechanismAssemblyProbabilityTextBox, string.Empty);
        }
    }
}