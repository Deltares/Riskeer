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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// The view for the <see cref="FailureMechanismSectionResult"/>.
    /// </summary>
    /// <typeparam name="T">The type of results which are presented by the <see cref="FailureMechanismResultView{T}"/>.</typeparam>
    public abstract partial class FailureMechanismResultView<T> : UserControl, IView where T : FailureMechanismSectionResult
    {
        protected const int AssessmentLayerOneColumnIndex = 1;
        private readonly Observer failureMechanismSectionResultObserver;
        private readonly IObservableEnumerable<T> failureMechanismSectionResults;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismResultView{T}"/>.
        /// </summary>
        /// <param name="failureMechanismSectionResults">The collection of <typeparamref name="T"/> to
        /// show in the view.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResults"/>
        /// is <c>null</c>.</exception>
        protected FailureMechanismResultView(IObservableEnumerable<T> failureMechanismSectionResults)
        {
            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            InitializeComponent();

            this.failureMechanismSectionResults = failureMechanismSectionResults;
            failureMechanismSectionResultObserver = new Observer(UpdateDataGridViewDataSource)
            {
                Observable = failureMechanismSectionResults
            };
        }

        /// <summary>
        /// Sets the failure mechanism.
        /// </summary>
        public virtual IFailureMechanism FailureMechanism { protected get; set; }

        public object Data { get; set; }

        protected DataGridViewControl DataGridViewControl { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AddDataGridColumns();
        }

        /// <summary>
        /// Creates a display object for <paramref name="sectionResult"/> which is added to the
        /// <see cref="DataGridView"/> on the <see cref="FailureMechanismResultView{T}"/>.
        /// </summary>
        /// <param name="sectionResult">The <typeparamref name="T"/> for which to create a
        /// display object.</param>
        /// <returns>A display object which can be added as a row to the <see cref="DataGridView"/>.</returns>
        protected abstract object CreateFailureMechanismSectionResultRow(T sectionResult);

        protected override void Dispose(bool disposing)
        {
            failureMechanismSectionResultObserver?.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds out whether the assessment section which is represented by the row at index 
        /// <paramref name="rowIndex"/> has passed the level 1 assessment.
        /// </summary>
        /// <param name="rowIndex">The index of the row which has a section attached.</param>
        /// <returns><c>false</c> if assessment level 1 has passed, <c>true</c> otherwise.</returns>
        protected bool HasPassedLevelOne(int rowIndex)
        {
            return (AssessmentLayerOneState) DataGridViewControl.GetCell(rowIndex, AssessmentLayerOneColumnIndex).Value
                   == AssessmentLayerOneState.Sufficient;
        }

        /// <summary>
        /// Updates the data source of the data grid view with the current known failure mechanism section results.
        /// </summary>
        protected void UpdateDataGridViewDataSource()
        {
            DataGridViewControl.EndEdit();
            DataGridViewControl.SetDataSource(
                failureMechanismSectionResults
                    .Select(CreateFailureMechanismSectionResultRow)
                    .Where(sr => sr != null)
                    .ToList()
            );
        }

        /// <summary>
        /// Gets data that is visualized on the row a the given <paramref name="rowIndex"/>.
        /// </summary>
        /// <param name="rowIndex">The position of the row in the data source.</param>
        /// <returns>The data bound to the row at index <paramref name="rowIndex"/>.</returns>
        protected object GetDataAtRow(int rowIndex)
        {
            return DataGridViewControl.GetRowFromIndex(rowIndex).DataBoundItem;
        }

        /// <summary>
        /// Adds the columns section name and the assessment layer one in the view.
        /// </summary>
        protected virtual void AddDataGridColumns()
        {
            DataGridViewControl.AddTextBoxColumn(
                nameof(FailureMechanismSectionResultRow<T>.Name),
                Resources.FailureMechanismResultView_InitializeDataGridView_Section_name,
                true);

            EnumDisplayWrapper<AssessmentLayerOneState>[] oneStateDataSource =
                Enum.GetValues(typeof(AssessmentLayerOneState))
                    .OfType<AssessmentLayerOneState>()
                    .Select(el => new EnumDisplayWrapper<AssessmentLayerOneState>(el))
                    .ToArray();

            DataGridViewControl.AddComboBoxColumn(
                nameof(FailureMechanismSectionResultRow<T>.AssessmentLayerOne),
                Resources.FailureMechanismResultView_InitializeDataGridView_Assessment_layer_one,
                oneStateDataSource,
                nameof(EnumDisplayWrapper<AssessmentLayerOneState>.Value),
                nameof(EnumDisplayWrapper<AssessmentLayerOneState>.DisplayName));
        }
    }
}