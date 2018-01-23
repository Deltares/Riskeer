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
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for testing data and selection synchronization in <see cref="LocationsView{T}"/> derivatives.
    /// </summary>
    /// <typeparam name="T">The type of the locations contained by the view.</typeparam>
    [TestFixture]
    public abstract class LocationsViewSynchronizationTester<T> where T : class
    {
        private Form testForm;

        /// <summary>
        /// Gets the index of the column containing the locations output.
        /// </summary>
        protected abstract int OutputColumnIndex { get; }

        [SetUp]
        public virtual void Setup()
        {
            testForm = new Form();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
        }

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
        /// <item>Row 2: location with output not containing a general result</item>
        /// <item>Row 3: location with the flag for parsing the general result set to true</item>
        /// <item>Row 4: location with output containing a general result with two top level illustration points</item>
        /// </list>
        /// </remarks>
        /// <returns>The fully configured locations view.</returns>
        protected abstract LocationsView<T> ShowFullyConfiguredLocationsView(Form form);

        /// <summary>
        /// Method for getting the locations in <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view to get the locations from.</param>
        /// <returns>An <see cref="ObservableList{T}"/> of locations.</returns>
        protected abstract ObservableList<HydraulicBoundaryLocation> GetLocationsInView(LocationsView<T> view);

        /// <summary>
        /// Method for getting the <see cref="HydraulicBoundaryLocationCalculation"/> corresponding to
        /// the provided <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to get the calculation for.</param>
        /// <returns>The calculation at stake.</returns>
        protected abstract HydraulicBoundaryLocationCalculation GetCalculationForLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation);

        private void ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(LocationsView<T> view)
        {
            ObservableList<HydraulicBoundaryLocation> locations = GetLocationsInView(view);

            locations.Clear();
            locations.Add(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0));
            locations.NotifyObservers();
        }

        private void ClearLocationOutputAndNotifyObservers(LocationsView<T> view)
        {
            ObservableList<HydraulicBoundaryLocation> locations = GetLocationsInView(view);

            locations.ForEach(loc =>
            {
                GetCalculationForLocation(loc).Output = null;
                loc.NotifyObservers();
            });
        }

        private void AddLocationOutputAndNotifyObservers(LocationsView<T> view)
        {
            ObservableList<HydraulicBoundaryLocation> locations = GetLocationsInView(view);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = locations.First();
            GetCalculationForLocation(hydraulicBoundaryLocation).Output = new TestHydraulicBoundaryLocationOutput(new TestGeneralResultSubMechanismIllustrationPoint());
            hydraulicBoundaryLocation.NotifyObservers();
        }

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
            AssertIllustrationPointControlSelection(view.Selection);
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
            AssertIllustrationPointControlSelection(view.Selection);

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
            AssertIllustrationPointControlSelection(view.Selection);

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
            AssertIllustrationPointControlSelection(view.Selection);

            // When
            AddLocationOutputAndNotifyObservers(view);

            // Then
            Assert.AreEqual(4, locationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            AssertIllustrationPointControlSelection(view.Selection);
        }

        private void AssertIllustrationPointControlSelection(object selection)
        {
            var illustrationPointSelection = selection as SelectedTopLevelSubMechanismIllustrationPoint;
            Assert.IsNotNull(illustrationPointSelection);
            Assert.AreSame(GetIllustrationPointsControl().Data.ElementAt(1).Source, illustrationPointSelection.TopLevelSubMechanismIllustrationPoint);
            CollectionAssert.AreEqual(GetIllustrationPointsControl().Data.Select(data => data.ClosingSituation), illustrationPointSelection.ClosingSituations);
        }

        #endregion
    }
}