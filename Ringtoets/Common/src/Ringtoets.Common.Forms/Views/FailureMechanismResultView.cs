﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="TSectionResult">The type of results which are presented by the 
    /// <see cref="FailureMechanismResultView{TSectionResult, TSectionResultRow, TFailureMechanism}"/>.</typeparam>
    /// <typeparam name="TSectionResultRow">The type of the row that is used to show the data.</typeparam>
    /// <typeparam name="TFailureMechanism">The type of the failure mechanism this view belongs to.</typeparam>
    public abstract partial class FailureMechanismResultView<TSectionResult, TSectionResultRow, TFailureMechanism> : UserControl, IView
        where TSectionResult : FailureMechanismSectionResult
        where TSectionResultRow : FailureMechanismSectionResultRow<TSectionResult>
        where TFailureMechanism : IFailureMechanism
    {
        protected const int SimpleAssessmentColumnIndex = 1;
        private readonly Observer failureMechanismSectionResultObserver;
        private readonly IObservableEnumerable<TSectionResult> failureMechanismSectionResults;
        private readonly RecursiveObserver<IObservableEnumerable<TSectionResult>, TSectionResult> failureMechanismSectionResultsObserver;
        protected IEnumerable<TSectionResultRow> SectionResultRows;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView{TSectionResult, TSectionResultRow, TFailureMechanism}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <typeparamref name="TSectionResult"/> to
        /// show in the view.</param>
        /// <param name="failureMechanism">The failure mechanism this view belongs to.</param>
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

            FailureMechanism = failureMechanism;
            this.failureMechanismSectionResults = failureMechanismSectionResults;
            failureMechanismSectionResultObserver = new Observer(UpdateDataGridViewDataSource)
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

        protected DataGridViewControl DataGridViewControl { get; private set; }

        /// <summary>
        /// Updates the section result rows whenever a section result is notified.
        /// </summary>
        protected virtual void UpdateSectionResultRows()
        {
            DataGridViewControl.RefreshDataGridView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddDataGridColumns();
            BindEvents();

            UpdateDataGridViewDataSource();
        }

        /// <summary>
        /// Creates a display object for <paramref name="sectionResult"/> which is added to the
        /// <see cref="DataGridView"/> on the <see cref="FailureMechanismResultView{TSectionResult, TSectionResultRow, TFailureMechanism}"/>.
        /// </summary>
        /// <param name="sectionResult">The <typeparamref name="TSectionResult"/> for which to create a
        /// display object.</param>
        /// <returns>A display object which can be added as a row to the <see cref="DataGridView"/>.</returns>
        protected abstract TSectionResultRow CreateFailureMechanismSectionResultRow(TSectionResult sectionResult);

        protected override void Dispose(bool disposing)
        {
            failureMechanismSectionResultObserver.Dispose();
            failureMechanismSectionResultsObserver.Dispose();

            DataGridViewControl.CellFormatting -= HandleCellStyling;

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the data source of the data grid view with the current known failure mechanism section results.
        /// </summary>
        protected void UpdateDataGridViewDataSource()
        {
            DataGridViewControl.EndEdit();
            SectionResultRows = failureMechanismSectionResults
                                .Select(CreateFailureMechanismSectionResultRow)
                                .Where(sr => sr != null)
                                .ToList();
            DataGridViewControl.SetDataSource(SectionResultRows);
            UpdateSectionResultRows();
        }

        /// <summary>
        /// Gets data that is visualized on the row a the given <paramref name="rowIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The position of the row in the data source.</param>
        /// <returns>The data bound to the row at index <paramref name="rowIndex"/>.</returns>
        protected TSectionResultRow GetDataAtRow(int rowIndex)
        {
            return (TSectionResultRow) DataGridViewControl.GetRowFromIndex(rowIndex).DataBoundItem;
        }

        /// <summary>
        /// Adds the columns to the view.
        /// </summary>
        protected abstract void AddDataGridColumns();

        /// <summary>
        /// Binds the events to the data grid view.
        /// </summary>
        protected virtual void BindEvents()
        {
            DataGridViewControl.CellFormatting += HandleCellStyling;
        }

        private void HandleCellStyling(object sender, DataGridViewCellFormattingEventArgs e)
        {
            TSectionResultRow row = GetDataAtRow(e.RowIndex);

            if (row.ColumnStateDefinitions.ContainsKey(e.ColumnIndex))
            {
                DataGridViewColumnStateDefinition columnStateDefinition = row.ColumnStateDefinitions[e.ColumnIndex];
                DataGridViewCell cell = DataGridViewControl.GetCell(e.RowIndex, e.ColumnIndex);

                cell.ReadOnly = columnStateDefinition.ReadOnly;
                cell.ErrorText = columnStateDefinition.ErrorText;
                cell.Style.BackColor = columnStateDefinition.Style.BackgroundColor;
                cell.Style.ForeColor = columnStateDefinition.Style.TextColor;
            }
        }
    }
}