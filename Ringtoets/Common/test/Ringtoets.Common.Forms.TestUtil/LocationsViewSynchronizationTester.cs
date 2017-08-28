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
using Rhino.Mocks;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for testing data and selection synchronization in <see cref="LocationsView{T}"/> derivatives.
    /// </summary>
    /// <typeparam name="T">The type of the locations contained by the view.</typeparam>
    public abstract class LocationsViewSynchronizationTester<T> where T : class
    {
        protected Form testForm;
        protected MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
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
        /// <item>Row 5: location with output containing a general result with two top level illustration points</item>
        /// </list>
        /// </remarks>
        /// <returns>The fully configured locations view.</returns>
        protected abstract LocationsView<T> ShowFullyConfiguredLocationsView(Form form);

        /// <summary>
        /// Method for replacing the hydraulic boundary database as well as notifying the observers.
        /// </summary>
        /// <param name="view">The locations view involved.</param>
        protected abstract void ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(LocationsView<T> view);

        /// <summary>
        /// Method for clearing all location output as well as notifying the observers.
        /// </summary>
        /// <param name="view">The locations view involved.</param>
        protected abstract void ClearLocationOutputAndNotifyObservers(LocationsView<T> view);

        /// <summary>
        /// Method for adding some location output as well as notifying the observers.
        /// </summary>
        /// <param name="view">The locations view involved.</param>
        protected abstract void AddLocationOutputAndNotifyObservers(LocationsView<T> view);

        private DataGridView GetLocationsDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "DataGridView");
        }

        private DataGridViewControl GetLocationsDataGridViewControl()
        {
            return ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
        }

        private IllustrationPointsControl GetIllustrationPointsControl()
        {
            return ControlTestHelper.GetControls<IllustrationPointsControl>(testForm, "IllustrationPointsControl").Single();
        }

        private DataGridView GetIllustrationPointsDataGridView()
        {
            return ControlTestHelper.GetDataGridView(GetIllustrationPointsControl(), "DataGridView");
        }

        #region Data synchronization

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutOutput_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();

            // When
            locationsDataGridViewControl.SetCurrentCell(locationsDataGridViewControl.GetCell(0, 1));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithoutGeneralResult_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();

            // When
            locationsDataGridViewControl.SetCurrentCell(locationsDataGridViewControl.GetCell(1, 0));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocationWithGeneralResult_ThenGeneralResultSetOnIllustrationPointsControlData()
        {
            // Given
            ShowFullyConfiguredLocationsView(testForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();

            // When
            locationsDataGridViewControl.SetCurrentCell(locationsDataGridViewControl.GetCell(4, 0));

            // Then
            Assert.AreEqual(2, illustrationPointsControl.Data.Count());
        }

        [Test]
        public void GivenFullyConfiguredViewWithFilledIllustrationPointsControl_WhenOutputCleared_ThenDataGridViewsUpdated()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            DataGridViewRowCollection locationsDataGridViewRows = locationsDataGridView.Rows;
            locationsDataGridView.CurrentCell = locationsDataGridViewRows[4].Cells[0];

            // Precondition
            Assert.AreEqual(5, locationsDataGridViewRows.Count);
            Assert.AreEqual("-", locationsDataGridViewRows[0].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreNotEqual("-", locationsDataGridViewRows[1].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", locationsDataGridViewRows[2].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", locationsDataGridViewRows[3].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreNotEqual("-", locationsDataGridViewRows[4].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual(2, GetIllustrationPointsControl().Data.Count());

            var refreshed = false;
            locationsDataGridView.Invalidated += (sender, args) => refreshed = true;

            // When
            ClearLocationOutputAndNotifyObservers(view);

            // Then
            Assert.IsTrue(refreshed);
            Assert.AreEqual(5, locationsDataGridViewRows.Count);
            Assert.AreEqual("-", locationsDataGridViewRows[0].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", locationsDataGridViewRows[1].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", locationsDataGridViewRows[2].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", locationsDataGridViewRows[3].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", locationsDataGridViewRows[4].Cells[OutputColumnIndex].FormattedValue);
            CollectionAssert.IsEmpty(GetIllustrationPointsControl().Data);
        }

        #endregion

        #region Selection synchronization

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingLocation_ThenSelectionUpdated()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();

            // When
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];

            // Then
            DataGridViewRow currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithLocationSelection_WhenDatabaseReplaced_ThenSelectionUpdated()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];

            // Precondition
            DataGridViewRow currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);

            // When
            ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(view);

            // Then
            currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(0, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithLocationSelection_WhenOutputCleared_ThenSelectionPreserved()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];

            // Precondition
            DataGridViewRow currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);

            // When
            ClearLocationOutputAndNotifyObservers(view);

            // Then
            currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithLocationSelection_WhenOutputUpdated_ThenSelectionPreserved()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];

            // Precondition
            DataGridViewRow currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);

            // When
            AddLocationOutputAndNotifyObservers(view);

            // Then
            currentLocationRow = GetLocationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(4, currentLocationRow.Index);
            Assert.AreEqual(GetLocationSelection(view, currentLocationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingIllustrationPoint_ThenSelectionUpdated()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();

            // When
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Then
            var selection = view.Selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(selection);
            Assert.AreSame(GetIllustrationPointsControl().Data.ElementAt(1).Source, selection.TopLevelSubMechanismIllustrationPoint);
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPointSelection_WhenDatabaseReplaced_ThenSelectionSetToLocation()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Precondition
            Assert.AreEqual(4, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            var selection = view.Selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(selection);
            Assert.AreSame(GetIllustrationPointsControl().Data.ElementAt(1).Source, selection.TopLevelSubMechanismIllustrationPoint);

            // When
            ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(view);

            // Then
            Assert.AreEqual(0, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(GetLocationSelection(view, locationsDataGridView.CurrentRow?.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPointSelection_WhenOutputCleared_ThenSelectionSetToLocation()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Precondition
            Assert.AreEqual(4, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            var selection = view.Selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(selection);
            Assert.AreSame(GetIllustrationPointsControl().Data.ElementAt(1).Source, selection.TopLevelSubMechanismIllustrationPoint);

            // When
            ClearLocationOutputAndNotifyObservers(view);

            // Then
            Assert.AreEqual(4, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(GetLocationSelection(view, locationsDataGridView.CurrentRow?.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPointSelection_WhenOutputUpdated_ThenSelectionPreserved()
        {
            // Given
            LocationsView<T> view = ShowFullyConfiguredLocationsView(testForm);

            DataGridView locationsDataGridView = GetLocationsDataGridView();
            locationsDataGridView.CurrentCell = locationsDataGridView.Rows[4].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Precondition
            Assert.AreEqual(4, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            var selection = view.Selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(selection);
            Assert.AreSame(GetIllustrationPointsControl().Data.ElementAt(1).Source, selection.TopLevelSubMechanismIllustrationPoint);

            // When
            AddLocationOutputAndNotifyObservers(view);

            // Then
            Assert.AreEqual(4, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            selection = view.Selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(selection);
            Assert.AreSame(GetIllustrationPointsControl().Data.ElementAt(1).Source, selection.TopLevelSubMechanismIllustrationPoint);
        }

        #endregion
    }
}