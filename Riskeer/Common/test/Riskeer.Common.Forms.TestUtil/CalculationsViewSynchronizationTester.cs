// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.DataGrid;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class for testing data and selection synchronization in <see cref="CalculationsView{T}"/> derivatives.
    /// </summary>
    /// <typeparam name="T">The type of the calculations contained by the view.</typeparam>
    [TestFixture]
    public abstract class CalculationsViewSynchronizationTester<T> where T : class
    {
        private Form testForm;

        /// <summary>
        /// Gets the index of the column containing the calculation output.
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
        /// Method for obtaining the view selection object related to the selected calculation row.
        /// </summary>
        /// <param name="view">The calculations view involved.</param>
        /// <param name="selectedRowObject">The selected calculation row object.</param>
        /// <returns>The view selection object.</returns>
        protected abstract object GetCalculationSelection(CalculationsView<T> view, object selectedRowObject);

        /// <summary>
        /// Method for showing a fully configured calculations view.
        /// </summary>
        /// <param name="form">The form to use for showing the view.</param>
        /// <remarks>
        /// The view should contain the following calculation row data:
        /// <list type="bullet">
        /// <item>Row 1: calculation without output</item>
        /// <item>Row 2: calculation with output not containing a general result</item>
        /// <item>Row 3: calculation with the flag for reading the general result set to true</item>
        /// <item>Row 4: calculation with output containing a general result with two top level illustration points</item>
        /// </list>
        /// </remarks>
        /// <returns>The fully configured calculations view.</returns>
        protected abstract CalculationsView<T> ShowFullyConfiguredCalculationsView(Form form);

        /// <summary>
        /// Method for getting the calculations in <paramref name="view"/>.
        /// </summary>
        /// <param name="view">The view to get the calculations from.</param>
        /// <returns>An <see cref="ObservableList{T}"/> of calculations.</returns>
        protected abstract ObservableList<HydraulicBoundaryLocationCalculation> GetCalculationsInView(CalculationsView<T> view);

        private void ReplaceCalculationsAndNotifyObservers(CalculationsView<T> view)
        {
            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetCalculationsInView(view);

            calculations.Clear();
            calculations.Add(new HydraulicBoundaryLocationCalculation(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)));
            calculations.NotifyObservers();
        }

        private void ClearCalculationOutputAndNotifyObservers(CalculationsView<T> view)
        {
            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetCalculationsInView(view);

            calculations.ForEach(calculation =>
            {
                calculation.Output = null;
                calculation.NotifyObservers();
            });
        }

        private void SetCalculationOutputAndNotifyObservers(CalculationsView<T> view)
        {
            ObservableList<HydraulicBoundaryLocationCalculation> calculations = GetCalculationsInView(view);

            HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation = calculations.First();
            hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationCalculationOutput(new TestGeneralResultSubMechanismIllustrationPoint());
            hydraulicBoundaryLocationCalculation.NotifyObservers();
        }

        private DataGridView GetCalculationsDataGridView()
        {
            return ControlTestHelper.GetDataGridView(testForm, "DataGridView");
        }

        private DataGridViewControl GetCalculationsDataGridViewControl()
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
        public void GivenFullyConfiguredView_WhenSelectingCalculationWithoutOutput_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredCalculationsView(testForm);

            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();

            // When
            calculationsDataGridViewControl.SetCurrentCell(calculationsDataGridViewControl.GetCell(0, 1));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCalculationWithoutGeneralResult_ThenIllustrationPointsControlDataSetToEmptyEnumeration()
        {
            // Given
            ShowFullyConfiguredCalculationsView(testForm);

            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();

            // When
            calculationsDataGridViewControl.SetCurrentCell(calculationsDataGridViewControl.GetCell(1, 0));

            // Then
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCalculationWithGeneralResult_ThenGeneralResultSetOnIllustrationPointsControlData()
        {
            // Given
            ShowFullyConfiguredCalculationsView(testForm);

            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl calculationsDataGridViewControl = GetCalculationsDataGridViewControl();

            // When
            calculationsDataGridViewControl.SetCurrentCell(calculationsDataGridViewControl.GetCell(3, 0));

            // Then
            Assert.AreEqual(2, illustrationPointsControl.Data.Count());
        }

        [Test]
        public void GivenFullyConfiguredViewWithFilledIllustrationPointsControl_WhenOutputCleared_ThenDataGridViewsUpdated()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            DataGridViewRowCollection dataGridViewRows = calculationsDataGridView.Rows;
            calculationsDataGridView.CurrentCell = dataGridViewRows[3].Cells[0];

            // Precondition
            Assert.AreEqual(4, dataGridViewRows.Count);
            Assert.AreEqual("-", dataGridViewRows[0].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreNotEqual("-", dataGridViewRows[1].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", dataGridViewRows[2].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreNotEqual("-", dataGridViewRows[3].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual(2, GetIllustrationPointsControl().Data.Count());

            var refreshed = false;
            calculationsDataGridView.Invalidated += (sender, args) => refreshed = true;

            // When
            ClearCalculationOutputAndNotifyObservers(view);

            // Then
            Assert.IsTrue(refreshed);
            Assert.AreEqual(4, dataGridViewRows.Count);
            Assert.AreEqual("-", dataGridViewRows[0].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", dataGridViewRows[1].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", dataGridViewRows[2].Cells[OutputColumnIndex].FormattedValue);
            Assert.AreEqual("-", dataGridViewRows[3].Cells[OutputColumnIndex].FormattedValue);
            CollectionAssert.IsEmpty(GetIllustrationPointsControl().Data);
        }

        #endregion

        #region Selection synchronization

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCalculation_ThenSelectionUpdated()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();

            // When
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];

            // Then
            DataGridViewRow currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(3, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithCalculationSelection_WhenCalculationsReplaced_ThenSelectionUpdated()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];

            // Precondition
            DataGridViewRow currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(3, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);

            // When
            ReplaceCalculationsAndNotifyObservers(view);

            // Then
            currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(0, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithCalculationSelection_WhenOutputCleared_ThenSelectionPreserved()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];

            // Precondition
            DataGridViewRow currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(3, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);

            // When
            ClearCalculationOutputAndNotifyObservers(view);

            // Then
            currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(3, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithCalculationSelection_WhenOutputUpdated_ThenSelectionPreserved()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];

            // Precondition
            DataGridViewRow currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(3, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);

            // When
            SetCalculationOutputAndNotifyObservers(view);

            // Then
            currentCalculationRow = GetCalculationsDataGridViewControl().CurrentRow;
            Assert.AreEqual(3, currentCalculationRow.Index);
            Assert.AreEqual(GetCalculationSelection(view, currentCalculationRow.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingIllustrationPoint_ThenSelectionUpdated()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();

            // When
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Then
            AssertIllustrationPointControlSelection(view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPointSelection_WhenCalculationsReplaced_ThenSelectionSetToCalculation()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Precondition
            Assert.AreEqual(3, calculationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            AssertIllustrationPointControlSelection(view.Selection);

            // When
            ReplaceCalculationsAndNotifyObservers(view);

            // Then
            Assert.AreEqual(0, calculationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(GetCalculationSelection(view, calculationsDataGridView.CurrentRow?.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPointSelection_WhenOutputCleared_ThenSelectionSetToCalculation()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Precondition
            Assert.AreEqual(3, calculationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            AssertIllustrationPointControlSelection(view.Selection);

            // When
            ClearCalculationOutputAndNotifyObservers(view);

            // Then
            Assert.AreEqual(3, calculationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(GetCalculationSelection(view, calculationsDataGridView.CurrentRow?.DataBoundItem), view.Selection);
        }

        [Test]
        public void GivenFullyConfiguredViewWithIllustrationPointSelection_WhenOutputUpdated_ThenSelectionPreserved()
        {
            // Given
            CalculationsView<T> view = ShowFullyConfiguredCalculationsView(testForm);

            DataGridView calculationsDataGridView = GetCalculationsDataGridView();
            calculationsDataGridView.CurrentCell = calculationsDataGridView.Rows[3].Cells[0];
            DataGridView illustrationPointsDataGridView = GetIllustrationPointsDataGridView();
            illustrationPointsDataGridView.CurrentCell = illustrationPointsDataGridView.Rows[1].Cells[0];

            // Precondition
            Assert.AreEqual(3, calculationsDataGridView.CurrentRow?.Index);
            Assert.AreEqual(1, illustrationPointsDataGridView.CurrentRow?.Index);
            AssertIllustrationPointControlSelection(view.Selection);

            // When
            SetCalculationOutputAndNotifyObservers(view);

            // Then
            Assert.AreEqual(3, calculationsDataGridView.CurrentRow?.Index);
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