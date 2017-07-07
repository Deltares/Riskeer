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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class LocationsViewTest
    {
        private const int calculateColumnIndex = 0;

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
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new TestLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);

                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Control.ControlCollection verticalSplitContainerPanel1Controls = splitContainer.Panel1.Controls;
                Assert.AreEqual(2, verticalSplitContainerPanel1Controls.Count);
                Assert.IsInstanceOf<DataGridViewControl>(verticalSplitContainerPanel1Controls[0]);
                Assert.IsInstanceOf<GroupBox>(verticalSplitContainerPanel1Controls[1]);

                Control.ControlCollection verticalSplitContainerPanel2Controls = splitContainer.Panel2.Controls;
                Assert.AreEqual(1, verticalSplitContainerPanel2Controls.Count);
                Assert.IsInstanceOf<IllustrationPointsControl>(verticalSplitContainerPanel2Controls[0]);
            }
        }

        [Test]
        public void Constructor_CalculateAllButtonCorrectlyInitialized()
        {
            // Setup & Call
            TestLocationsView view = ShowTestCalculatableView();

            // Assert
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            TestLocationsView view = ShowTestCalculatableView();

            // Assert
            DataGridView dataGridView = GetDataGridView();
            Assert.AreEqual(1, dataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[calculateColumnIndex];
            const string expectedCalculateHeaderText = "Berekenen";
            Assert.AreEqual(expectedCalculateHeaderText, calculateColumn.HeaderText);

            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedFired()
        {
            // Given
            TestLocationsView view = ShowFullyConfiguredTestCalculatableView();

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = GetDataGridView();

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenIllustrationPointsUpdated()
        {
            // Given
            ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = GetDataGridView();
            DataGridView illustrationPointDataGridView = GetIllustrationPointDataGridView();

            // Precondition
            Assert.AreEqual(2, illustrationPointDataGridView.Rows.Count);

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(3, illustrationPointDataGridView.Rows.Count);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingRowInLocationsTable_ThenReturnSelectedLocation()
        {
            // Given
            TestLocationsView view = ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = GetDataGridView();
            DataGridViewRow currentRow = dataGridView.Rows[1];

            TestCalculatableObject calculatableObject = ((TestCalculatableRow) currentRow.DataBoundItem).CalculatableObject;

            // When
            dataGridView.CurrentCell = currentRow.Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));
            object selection = view.Selection;

            // Then
            Assert.AreSame(calculatableObject, selection);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingRowInIllustrationPointsTable_ThenReturnSelectedIllustrationPoint()
        {
            // Given
            TestLocationsView view = ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = GetDataGridView();
            DataGridView illustrationPointDataGridView = GetIllustrationPointDataGridView();

            DataGridViewRow currentRow = illustrationPointDataGridView.Rows[1];

            TestCalculatableObject calculatableObject = ((TestCalculatableRow) dataGridView.Rows[0].DataBoundItem).CalculatableObject;

            // When
            illustrationPointDataGridView.CurrentCell = currentRow.Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(illustrationPointDataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));
            object selection = view.Selection;

            // Then
            Assert.AreSame(calculatableObject.GeneralResult.TopLevelSubMechanismIllustrationPoints.ElementAt(1), selection);
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllCalculatableItemsSelected()
        {
            // Setup
            ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = GetDataGridView();
            DataGridViewRowCollection rows = dataGridView.Rows;
            var button = new ButtonTester("SelectAllButton", testForm);

            // Precondition
            Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[1].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void DeselectAllButton_AllCalculatableItemsSelectedDeselectAllButtonClicked_AllCalculatableItemsNotSelected()
        {
            // Setup
            ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = GetDataGridView();
            var button = new ButtonTester("DeselectAllButton", testForm);

            DataGridViewRowCollection rows = dataGridView.Rows;
            foreach (DataGridViewRow row in rows)
            {
                row.Cells[calculateColumnIndex].Value = true;
            }

            // Precondition
            Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[1].Cells[calculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void GivenFullyConfiguredView_WhenNoRowsSelected_ThenCalculateForSelectedButtonDisabledAndErrorMessageProvided()
        {
            // Given & When
            TestLocationsView view = ShowFullyConfiguredTestCalculatableView();

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual("Er zijn geen berekeningen geselecteerd.", errorProvider.GetError(button));
        }

        [Test]
        public void GivenFullyConfiguredView_WhenRowsSelected_ThenCalculateForSelectedButtonEnabledAndNoErrorMessageProvided()
        {
            // Given
            TestLocationsView view = ShowFullyConfiguredTestCalculatableView();
            DataGridView dataGridView = GetDataGridView();

            // When
            dataGridView.Rows[0].Cells[calculateColumnIndex].Value = true;

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsTrue(button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual("", errorProvider.GetError(button));
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateHandleCalculateSelectedObjects()
        {
            // Setup
            TestLocationsView view = ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = GetDataGridView();

            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, view.ObjectsToCalculate.Count());
            TestCalculatableObject expectedObject = ((IEnumerable<TestCalculatableObject>) view.Data).First();
            Assert.AreEqual(expectedObject, view.ObjectsToCalculate.First());
        }

        private DataGridView GetDataGridView()
        {
            return GetControls<DataGridView>("DataGridView").First();
        }

        private DataGridView GetIllustrationPointDataGridView()
        {
            return GetControls<DataGridView>("DataGridView").Last();
        }

        /// <summary>
        /// Gets the controls by name.
        /// </summary>
        /// <param name="controlName">The name of the controls.</param>
        /// <returns>The found control.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="controlName"/> is <c>null</c> or empty.</exception>
        private IEnumerable<TView> GetControls<TView>(string controlName) where TView : Control
        {
            return testForm.Controls.Find(controlName, true).Cast<TView>();
        }

        private TestLocationsView ShowTestCalculatableView()
        {
            var view = new TestLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();
            return view;
        }

        private TestLocationsView ShowFullyConfiguredTestCalculatableView()
        {
            TestLocationsView view = ShowTestCalculatableView();
            view.Data = new[]
            {
                new TestCalculatableObject
                {
                    GeneralResult = new GeneralResultSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(),
                        Enumerable.Empty<Stochast>(),
                        new[]
                        {
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                                new SubMechanismIllustrationPoint("Point 1", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>(), 0.9)),
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Open",
                                new SubMechanismIllustrationPoint("Point 2", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>(), 0.7))
                        })
                },
                new TestCalculatableObject
                {
                    GeneralResult = new GeneralResultSubMechanismIllustrationPoint(
                        WindDirectionTestFactory.CreateTestWindDirection(),
                        Enumerable.Empty<Stochast>(),
                        new[]
                        {
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                                new SubMechanismIllustrationPoint("Point 1", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>(), 0.9)),
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Open",
                                new SubMechanismIllustrationPoint("Point 2", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>(), 0.7)),
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Closed",
                                new SubMechanismIllustrationPoint("Point 3", Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>(), 0.8))
                        })
                }
            };
            return view;
        }

        private class TestCalculatableRow : CalculatableRow<TestCalculatableObject>
        {
            public TestCalculatableRow(TestCalculatableObject calculatableObject) : base(calculatableObject)
            {
                ShouldCalculate = calculatableObject.IsChecked;
            }
        }

        private class TestCalculatableObject
        {
            public bool IsChecked { get; }

            public GeneralResultSubMechanismIllustrationPoint GeneralResult { get; set; }
        }

        private class TestLocationsView : LocationsView<TestCalculatableObject>
        {
            private IEnumerable<TestCalculatableObject> data;

            public TestLocationsView()
            {
                ObjectsToCalculate = new List<TestCalculatableObject>();
            }

            public override object Data
            {
                get
                {
                    return data;
                }
                set
                {
                    data = value as IEnumerable<TestCalculatableObject>;
                    UpdateDataGridViewDataSource();
                }
            }

            public IEnumerable<TestCalculatableObject> ObjectsToCalculate { get; private set; }

            protected override object CreateSelectedItemFromCurrentRow()
            {
                return ((TestCalculatableRow) dataGridViewControl.CurrentRow?.DataBoundItem)?.CalculatableObject;
            }

            protected override void SetDataSource()
            {
                dataGridViewControl.SetDataSource(data.Select(d => new TestCalculatableRow(d)).ToArray());
            }

            protected override void CalculateForSelectedRows()
            {
                ObjectsToCalculate = GetCalculatableRows()
                    .Where(r => r.ShouldCalculate)
                    .Cast<TestCalculatableRow>()
                    .Select(row => row.CalculatableObject);
            }

            protected override GeneralResultSubMechanismIllustrationPoint GetGeneralResultSubMechanismIllustrationPoints()
            {
                TestCalculatableObject calculatableObject = ((TestCalculatableRow) dataGridViewControl.CurrentRow?.DataBoundItem)?.CalculatableObject;

                return calculatableObject?.GeneralResult;
            }
        }
    }
}