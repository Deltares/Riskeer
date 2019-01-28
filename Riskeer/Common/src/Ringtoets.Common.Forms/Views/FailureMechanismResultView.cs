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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Controls;
using Ringtoets.Common.Forms.Properties;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="TSectionResult">The type of results which are presented by the 
    /// <see cref="FailureMechanismResultView{TSectionResult, TSectionResultRow, TFailureMechanism, TAssemblyResultControl}"/>.</typeparam>
    /// <typeparam name="TSectionResultRow">The type of the row that is used to show the data.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism this view belongs to.</typeparam>
    /// <typeparam name="TAssemblyResultControl">The type of the assembly result control in this view.</typeparam>
    public abstract partial class FailureMechanismResultView<TSectionResult, TSectionResultRow, TFailureMechanism, TAssemblyResultControl> : UserControl, IView
        where TSectionResult : FailureMechanismSectionResult
        where TSectionResultRow : FailureMechanismSectionResultRow<TSectionResult>
        where TFailureMechanism : IHasSectionResults<TSectionResult>
        where TAssemblyResultControl : AssemblyResultControl, new()
    {
        private readonly IObservableEnumerable<TSectionResult> failureMechanismSectionResults;
        private readonly Observer failureMechanismObserver;
        private readonly Observer failureMechanismSectionResultObserver;
        private readonly RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult> failureMechanismSectionResultsObserver;
        protected TAssemblyResultControl FailureMechanismAssemblyResultControl;

        private IEnumerable<TSectionResultRow> sectionResultRows;
        private bool rowUpdating;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView{TSectionResult, TSectionResultRow, TFailureMechanism, TAssemblyResultControl}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <typeparamref name="TSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism the results belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected FailureMechanismResultView(IObservableEnumerable<TSectionResult> failureMechanismSectionResults, TFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            InitializeComponent();
            InitializeFailureMechanismAssemblyResultPanel();

            FailureMechanism = failureMechanism;
            this.failureMechanismSectionResults = failureMechanismSectionResults;

            failureMechanismObserver = new Observer(UpdateSectionResultRows)
            {
                Observable = failureMechanism
            };

            failureMechanismSectionResultObserver = new Observer(UpdateView)
            {
                Observable = failureMechanismSectionResults
            };

            failureMechanismSectionResultsObserver = new RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult>(
                UpdateSectionResultRows,
                sr => sr)
            {
                Observable = failureMechanismSectionResults
            };
        }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public TFailureMechanism FailureMechanism { get; }

        public object Data { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddDataGridColumns();

            DataGridViewControl.CellFormatting += HandleCellStyling;

            UpdateView();
        }

        /// <summary>
        /// Creates a display object for <paramref name="sectionResult"/> which is added to the
        /// <see cref="DataGridView"/> on the <see cref="FailureMechanismResultView{TSectionResult, TSectionResultRow, TFailureMechanism, TAssemblyResultControl}"/>.
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
        /// Updates the content of the <see cref="FailureMechanismAssemblyResultControl"/>.
        /// </summary>
        /// <exception cref="AssemblyException">Thrown when the assembly result
        /// could not be created.</exception>
        protected abstract void UpdateAssemblyResultControl();

        /// <summary>
        /// Updates all controls in the view.
        /// </summary>
        protected void UpdateView()
        {
            UpdateDataGridViewDataSource();
            UpdateFailureMechanismAssemblyResultControl();
        }

        /// <summary>
        /// Refreshes the data grid in the view.
        /// </summary>
        protected virtual void RefreshDataGrid()
        {
            DataGridViewControl.RefreshDataGridView(false);
        }

        private bool HasManualAssemblyResults()
        {
            return HasSectionResultsHelper.HasManualAssemblyResults(FailureMechanism);
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

            sectionResultRows?.ForEachElementDo(row =>
            {
                row.RowUpdated += RowUpdated;
                row.RowUpdateDone += RowUpdateDone;
            });
        }

        private void UpdateFailureMechanismAssemblyResultControl()
        {
            FailureMechanismAssemblyResultControl.ClearAssemblyResult();
            FailureMechanismAssemblyResultControl.ClearMessages();

            try
            {
                UpdateAssemblyResultControl();
            }
            catch (AssemblyException e)
            {
                FailureMechanismAssemblyResultControl.SetError(e.Message);
            }

            if (HasManualAssemblyResults())
            {
                FailureMechanismAssemblyResultControl.SetManualAssemblyWarning();
            }
        }

        private void InitializeFailureMechanismAssemblyResultPanel()
        {
            infoIcon.BackgroundImage = CoreCommonGuiResources.information;
            toolTip.SetToolTip(infoIcon, Resources.FailureMechanismResultView_InfoToolTip);

            FailureMechanismAssemblyResultControl = new TAssemblyResultControl();
            TableLayoutPanel.Controls.Add(FailureMechanismAssemblyResultControl, 1, 0);
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
            UpdateFailureMechanismAssemblyResultControl();
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

            UpdateFailureMechanismAssemblyResultControl();
        }
    }
}