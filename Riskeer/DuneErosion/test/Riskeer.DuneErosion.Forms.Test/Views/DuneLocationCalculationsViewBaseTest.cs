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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationCalculationsViewBaseTest
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
            using (var view = new TestDuneLocationCalculationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);

                Assert.AreEqual(new Size(526, 85), view.AutoScrollMinSize);
            }
        }

        [Test]
        public void Constructor_CalculateAllButtonCorrectlyInitialized()
        {
            // Setup & Call
            TestDuneLocationCalculationsView view = ShowTestCalculatableView();

            // Assert
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            TestDuneLocationCalculationsView view = ShowTestCalculatableView();

            // Assert
            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
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
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            var createdSelection = new object();
            view.CreateForSelection = createdSelection;

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];

            // When
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Then
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void Selection_Always_ReturnsCreatedSelectionObject()
        {
            // Setup
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            var createdSelection = new object();
            view.CreateForSelection = createdSelection;

            // Call
            object selection = view.Selection;

            // Assert
            Assert.AreSame(createdSelection, selection);
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllCalculatableItemsSelected()
        {
            // Setup
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
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
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
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
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();

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
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();
            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];

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
            TestDuneLocationCalculationsView view = ShowFullyConfiguredTestCalculatableView();

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];

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

        private TestDuneLocationCalculationsView ShowTestCalculatableView()
        {
            var view = new TestDuneLocationCalculationsView();

            testForm.Controls.Add(view);
            testForm.Show();
            return view;
        }

        private TestDuneLocationCalculationsView ShowFullyConfiguredTestCalculatableView()
        {
            TestDuneLocationCalculationsView view = ShowTestCalculatableView();
            view.Data = new[]
            {
                new TestCalculatableObject(),
                new TestCalculatableObject()
            };
            return view;
        }

        private class TestCalculatableRow : DuneLocationCalculationRow
        {
            public TestCalculatableRow(TestCalculatableObject calculatableObject) : base(calculatableObject)
            {
                ShouldCalculate = calculatableObject.IsChecked;
            }
        }

        private class TestCalculatableObject : DuneLocationCalculation
        {
            public TestCalculatableObject() : base(new TestDuneLocation()) {}
            public bool IsChecked { get; }
        }

        private class TestDuneLocationCalculationsView : DuneLocationCalculationsViewBase
        {
            private IEnumerable<TestCalculatableObject> data;

            public TestDuneLocationCalculationsView()
            {
                ObjectsToCalculate = new List<DuneLocationCalculation>();
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

            public object CreateForSelection { private get; set; }

            public IEnumerable<DuneLocationCalculation> ObjectsToCalculate { get; private set; }

            protected override object CreateSelectedItemFromCurrentRow()
            {
                return CreateForSelection;
            }

            protected override void SetDataSource()
            {
                dataGridViewControl.SetDataSource(data.Select(d => new TestCalculatableRow(d)).ToArray());
            }

            protected override void CalculateForSelectedRows()
            {
                ObjectsToCalculate = GetCalculatableRows()
                                     .Where(r => r.ShouldCalculate)
                                     .Select(row => row.CalculatableObject);
            }
        }
    }
}