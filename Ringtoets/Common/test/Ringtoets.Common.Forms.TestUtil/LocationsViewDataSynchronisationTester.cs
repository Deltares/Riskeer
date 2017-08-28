// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for testing data synchronization in <see cref="LocationsView{T}"/> derivatives.
    /// </summary>
    /// <typeparam name="T">The type of the locations contained by the view.</typeparam>
    public abstract class LocationsViewDataSynchronisationTester<T> where T : class
    {
        private Form testForm;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutOutput_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridViewControl = GetDataGridViewControl();

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(0, 1));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutGeneralResult_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridViewControl = GetDataGridViewControl();

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(1, 0));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithGeneralResult_ThenGeneralResultSetOnIllustrationPointsControlData()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl dataGridViewControl = GetDataGridViewControl();

            // When
            dataGridViewControl.SetCurrentCell(dataGridViewControl.GetCell(4, 0));

            // Then
            CollectionAssert.IsNotEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredViewWithLocationSelection_WhenOutputCleared_ThenDataGridViewUpdatedAndSelectionPreserved()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView dataGridView = GetDataGridView();
            DataGridViewRowCollection rows = dataGridView.Rows;
            dataGridView.CurrentCell = rows[4].Cells[0];

            // Precondition
            Assert.AreEqual(5, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreNotEqual("-", rows[1].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreNotEqual("-", rows[4].Cells[OutputColumnIndex].FormattedValue);
            DataGridViewRow dataGridViewRow = GetDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, dataGridViewRow.Index);
            Assert.AreEqual(GetLocationSelection(view, dataGridViewRow.DataBoundItem), view.Selection);

            var refreshed = false;
            dataGridView.Invalidated += (sender, args) => refreshed = true;

            // When
            ClearLocationOutputAndNotifyObservers(view);

            // Then
            Assert.IsTrue(refreshed);
            Assert.AreEqual(5, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[1].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[4].Cells[OutputColumnIndex].FormattedValue);
            dataGridViewRow = GetDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, dataGridViewRow.Index);
            Assert.AreEqual(GetLocationSelection(view, dataGridViewRow.DataBoundItem), view.Selection);
        }

        /// <summary>
        /// Gets the index of the column containing the locations output.
        /// </summary>
        protected abstract int OutputColumnIndex { get; }

        /// <summary>
        /// Method for obtaining the view selection object related to the selected location row.
        /// </summary>
        /// <param name="view">The locations view involved.</param>
        /// <param name="selectedRowObject">The selected location row object.</param>
        /// <returns>The view selection object.</returns>
        protected abstract object GetLocationSelection(LocationsView<T> view, object selectedRowObject);

        /// <summary>
        /// Method for showing a fully configured locations view.
        /// </summary>
        /// <param name="form">The form to use for showing the view.</param>
        /// <remarks>
        /// The view should contain the following location row data:
        /// <list type="bullet">
        /// <item>Row 1: location without output</item>
        /// <item>Row 2: location with design water level output (but without general result)</item>
        /// <item>Row 3: location with wave height output (but without general result)</item>
        /// <item>Row 4: location with flag for parsing the general result set to true</item>
        /// <item>Row 5: location with output containing a general result</item>
        /// </list>
        /// </remarks>
        /// <returns>The fully configured locations view.</returns>
        protected abstract LocationsView<T> ShowFullyConfiguredLocationsView(Form form);

        /// <summary>
        /// Method for clearing all location output as well as notifying the observers.
        /// </summary>
        /// <param name="view">The locations view involved.</param>
        protected abstract void ClearLocationOutputAndNotifyObservers(LocationsView<T> view);

        private DataGridView GetDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "DataGridView");
        }

        private DataGridViewControl GetDataGridViewControl()
        {
            return ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return ControlTestHelper.GetControls<IllustrationPointsControl>(testForm, "IllustrationPointsControl").Single();
        }
    }
}