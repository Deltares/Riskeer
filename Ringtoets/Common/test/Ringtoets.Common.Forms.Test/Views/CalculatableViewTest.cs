// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class CalculatableViewTest
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
            using (var view = new TestCalculatableView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_CalculateAllButtonCorrectlyInitialized()
        {
            // Setup & Call
            ShowTestCalculatableView();

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button)buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowTestCalculatableView();

            // Assert
            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(1, dataGridView.ColumnCount);

            var calculateColumn = (DataGridViewCheckBoxColumn)dataGridView.Columns[calculateColumnIndex];
            const string expectedLocationCalculateHeaderText = "Berekenen";
            Assert.AreEqual(expectedLocationCalculateHeaderText, calculateColumn.HeaderText);

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button)buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void CalculatableView_SelectingCellInRow_SelectionChangedFired()
        {
            // Setup
            var view = ShowFullyConfiguredTestCalculatableView();
            var createdSelection = new object();
            view.CreateForSelection = createdSelection;

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Assert
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void Selection_Always_ReturnsCreatedSelectionObject()
        {
            // Setup
            var view = ShowFullyConfiguredTestCalculatableView();
            var createdSelection = new object();
            view.CreateForSelection = createdSelection;

            // Call
            var selection = view.Selection;

            // Assert
            Assert.AreSame(createdSelection, selection);
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllLocationsSelected()
        {
            // Setup
            ShowFullyConfiguredTestCalculatableView();

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            var button = new ButtonTester("SelectAllButton", testForm);

            // Precondition
            Assert.IsFalse((bool)rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool)rows[1].Cells[calculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsTrue((bool)rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsTrue((bool)rows[1].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void DeselectAllButton_AllLocationsSelectedDeselectAllButtonClicked_AllLocationsNotSelected()
        {
            // Setup
            ShowFullyConfiguredTestCalculatableView();

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            var button = new ButtonTester("DeselectAllButton", testForm);

            foreach (DataGridViewRow row in rows)
            {
                row.Cells[calculateColumnIndex].Value = true;
            }

            // Precondition
            Assert.IsTrue((bool)rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsTrue((bool)rows[1].Cells[calculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsFalse((bool)rows[0].Cells[calculateColumnIndex].Value);
            Assert.IsFalse((bool)rows[1].Cells[calculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_NoneSelected_CalculateForSelectedButtonDisabled()
        {
            // Setup
            TestCalculatableView view = ShowFullyConfiguredTestCalculatableView();
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            var button = (Button)buttonTester.TheObject;

            // Assert
            Assert.IsFalse(button.Enabled);
            Assert.IsEmpty(view.LocationsToCalculate);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateHandleCalculateSelectedLocations()
        {
            // Setup
            TestCalculatableView view = ShowFullyConfiguredTestCalculatableView();

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[calculateColumnIndex].Value = true;

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, view.LocationsToCalculate.Count());
            TestCalculatableObject expectedObject = ((IEnumerable<TestCalculatableObject>)view.Data).First();
            Assert.AreEqual(expectedObject, view.LocationsToCalculate.First());
        }

        private TestCalculatableView ShowTestCalculatableView()
        {
            var view = new TestCalculatableView();

            testForm.Controls.Add(view);
            testForm.Show();
            return view;
        }

        private TestCalculatableView ShowFullyConfiguredTestCalculatableView()
        {
            var view = ShowTestCalculatableView();
            view.Data = new[]
            {
                new TestCalculatableObject(),
                new TestCalculatableObject()
            };
            return view;
        }

        private class TestCalculatableRow : CalculatableRow<TestCalculatableObject>
        {
            public TestCalculatableRow(TestCalculatableObject calculatableObject) : base (calculatableObject)
            {
                ToCalculate = calculatableObject.IsChecked;
            }
        }

        private class TestCalculatableObject
        {
            public bool IsChecked { get; }
        }

        private class TestCalculatableView : CalculatableView<TestCalculatableObject>
        {
            private IEnumerable<TestCalculatableObject> data;

            public TestCalculatableView()
            {
                LocationsToCalculate = new List<TestCalculatableObject>();
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

            public IEnumerable<TestCalculatableObject> LocationsToCalculate { get; private set; }

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
                LocationsToCalculate = GetCalculatableRows()
                    .Where(r => r.ToCalculate)
                    .Cast<TestCalculatableRow>()
                    .Select(row => row.CalculatableObject);
            }
        }
    }
}