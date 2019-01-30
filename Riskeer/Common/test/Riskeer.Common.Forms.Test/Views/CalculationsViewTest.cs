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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.DataGrid;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.Views;

namespace Riskeer.Common.Forms.Test.Views
{
    [TestFixture]
    public class CalculationsViewTest
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
            using (var view = new TestCalculationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);

                Assert.AreEqual(1, view.Controls.Count);

                var splitContainer = view.Controls[0] as SplitContainer;
                Assert.IsNotNull(splitContainer);
                Control.ControlCollection verticalSplitContainerPanel1Controls = splitContainer.Panel1.Controls;
                Assert.AreEqual(new Size(535, 0), splitContainer.Panel1.AutoScrollMinSize);
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
            TestCalculationsView view = ShowTestCalculatableView();

            // Assert
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            TestCalculationsView view = ShowTestCalculatableView();

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
            Assert.AreEqual(1, dataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[calculateColumnIndex];
            const string expectedCalculateHeaderText = "Berekenen";
            Assert.AreEqual(expectedCalculateHeaderText, calculateColumn.HeaderText);

            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Selection_WithoutCalculations_ReturnsNull()
        {
            // Call
            using (var view = new TestCalculationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedFired()
        {
            // Given
            TestCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllCalculatableItemsSelected()
        {
            // Setup
            ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
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

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
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
            TestCalculationsView view = ShowFullyConfiguredTestCalculatableView();

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
            TestCalculationsView view = ShowFullyConfiguredTestCalculatableView();
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

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
            TestCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");

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

        private TestCalculationsView ShowTestCalculatableView()
        {
            var view = new TestCalculationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private TestCalculationsView ShowFullyConfiguredTestCalculatableView()
        {
            TestCalculationsView view = ShowTestCalculatableView();
            view.Data = new[]
            {
                new TestCalculatableObject
                {
                    GeneralResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                        WindDirectionTestFactory.CreateTestWindDirection(),
                        Enumerable.Empty<Stochast>(),
                        new[]
                        {
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                                new SubMechanismIllustrationPoint("Point 1", 0.9,
                                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>())),
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Open",
                                new SubMechanismIllustrationPoint("Point 2", 0.7,
                                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>()))
                        })
                },
                new TestCalculatableObject
                {
                    GeneralResult = new GeneralResult<TopLevelSubMechanismIllustrationPoint>(
                        WindDirectionTestFactory.CreateTestWindDirection(),
                        Enumerable.Empty<Stochast>(),
                        new[]
                        {
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Regular",
                                new SubMechanismIllustrationPoint("Point 1", 0.9,
                                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>())),
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Open",
                                new SubMechanismIllustrationPoint("Point 2", 0.7,
                                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>())),
                            new TopLevelSubMechanismIllustrationPoint(
                                WindDirectionTestFactory.CreateTestWindDirection(), "Closed",
                                new SubMechanismIllustrationPoint("Point 3", 0.8,
                                                                  Enumerable.Empty<SubMechanismIllustrationPointStochast>(),
                                                                  Enumerable.Empty<IllustrationPointResult>()))
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

            public GeneralResult<TopLevelSubMechanismIllustrationPoint> GeneralResult { get; set; }
        }

        private class TestCalculationsView : CalculationsView<TestCalculatableObject>
        {
            private IEnumerable<TestCalculatableObject> data;

            public TestCalculationsView()
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

            protected override IEnumerable<IllustrationPointControlItem> GetIllustrationPointControlItems()
            {
                TestCalculatableObject calculatableObject = ((TestCalculatableRow) dataGridViewControl.CurrentRow?.DataBoundItem)?.CalculatableObject;

                return calculatableObject?.GeneralResult?.TopLevelIllustrationPoints
                                         .Select(topLevelSubMechanismIllustrationPoint =>
                                                     new IllustrationPointControlItem(topLevelSubMechanismIllustrationPoint,
                                                                                      topLevelSubMechanismIllustrationPoint.WindDirection.Name,
                                                                                      topLevelSubMechanismIllustrationPoint.ClosingSituation,
                                                                                      topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint.Stochasts,
                                                                                      topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint.Beta))
                                         .ToArray();
            }
        }
    }
}