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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int locationNameColumnIndex = 1;
        private const int locationIdColumnIndex = 2;
        private const int locationColumnIndex = 3;
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
            using (var view = new TestHydraulicBoundaryLocationsView())
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
            ShowTestHydraulicBoundaryLocationsView();

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button) buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowTestHydraulicBoundaryLocationsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(4, dataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationCalculateColumnIndex];
            const string expectedLocationCalculateHeaderText = "Berekenen";
            Assert.AreEqual(expectedLocationCalculateHeaderText, locationCalculateColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationNameColumnIndex];
            const string expectedLocationNameHeaderText = "Naam";
            Assert.AreEqual(expectedLocationNameHeaderText, locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationIdColumnIndex];
            const string expectedLocationIdHeaderText = "ID";
            Assert.AreEqual(expectedLocationIdHeaderText, locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
            const string expectedLocationHeaderText = "Coördinaten [m]";
            Assert.AreEqual(expectedLocationHeaderText, locationColumn.HeaderText);

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button) buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Data_IAssessmentSection_DataSet()
        {
            // Setup
            using (var view = new TestHydraulicBoundaryLocationsView())
            {
                var hydraulicBoundaryLocations = Enumerable.Empty<HydraulicBoundaryLocation>();

                // Call
                view.Data = hydraulicBoundaryLocations;

                // Assert
                Assert.AreSame(hydraulicBoundaryLocations, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanIAssessmentSection_DataNull()
        {
            // Setup
            using (var view = new TestHydraulicBoundaryLocationsView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void HydraulicBoundaryLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            var cells = rows[0].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(4, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
        }

        [Test]
        public void HydraulicBoundaryLocationsView_SelectingCellInRow_SelectionChangedFired()
        {
            // Setup
            var view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();
            var createdSelection = new object();
            view.CreateForSelection = createdSelection;

            var selectionChangedCount = 0;
            view.SelectionChanged += (sender, args) => selectionChangedCount++;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[locationCalculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Assert
            Assert.AreEqual(1, selectionChangedCount);
        }

        [Test]
        public void Selection_Always_ReturnsCreatedSelectionObject()
        {
            // Setup
            var view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();
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
            ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            var button = new ButtonTester("SelectAllButton", testForm);

            // Precondition
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[locationCalculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[2].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void DeselectAllButton_AllLocationsSelectedDeselectAllButtonClicked_AllLocationsNotSelected()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            var button = new ButtonTester("DeselectAllButton", testForm);

            foreach (DataGridViewRow row in rows)
            {
                row.Cells[locationCalculateColumnIndex].Value = true;
            }

            // Precondition
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsTrue((bool) rows[2].Cells[locationCalculateColumnIndex].Value);

            // Call
            button.Click();

            // Assert
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_NoneSelected_CalculateForSelectedButtonDisabled()
        {
            // Setup
            var mockRepository = new MockRepository();
            var guiServiceMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();
            mockRepository.ReplayAll();

            TestHydraulicBoundaryLocationsView view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();
            view.CalculationGuiService = guiServiceMock;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            var button = (Button) buttonTester.TheObject;

            // Assert
            Assert.IsFalse(button.Enabled);
            Assert.IsEmpty(view.LocationsToCalculate);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateHandleCalculateSelectedLocations()
        {
            // Setup
            TestHydraulicBoundaryLocationsView view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var mockRepository = new MockRepository();
            var guiServiceMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();
            mockRepository.ReplayAll();

            view.CalculationGuiService = guiServiceMock;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, view.LocationsToCalculate.Count());
            HydraulicBoundaryLocation expectedLocation = ((IEnumerable<HydraulicBoundaryLocation>) view.Data).First();
            Assert.AreEqual(expectedLocation, view.LocationsToCalculate.First());
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        private TestHydraulicBoundaryLocationsView ShowTestHydraulicBoundaryLocationsView()
        {
            var view = new TestHydraulicBoundaryLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private TestHydraulicBoundaryLocationsView ShowFullyConfiguredTestHydraulicBoundaryLocationsView()
        {
            var view = ShowTestHydraulicBoundaryLocationsView();

            var assessmentSection = new TestAssessmentSection
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            view.Data = assessmentSection.HydraulicBoundaryDatabase.Locations;
            return view;
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Comments { get; set; }

            public string Id { get; set; }

            public string Name { get; set; }

            public AssessmentSectionComposition Composition { get; private set; }

            public ReferenceLine ReferenceLine { get; set; }

            public FailureMechanismContribution FailureMechanismContribution { get; private set; }

            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

            public IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                throw new NotImplementedException();
            }

            public void ChangeComposition(AssessmentSectionComposition newComposition)
            {
                throw new NotImplementedException();
            }
        }

        private class TestHydraulicBoundaryDatabase : HydraulicBoundaryDatabase
        {
            public TestHydraulicBoundaryDatabase()
            {
                Locations.Add(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0));
                Locations.Add(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevel = (RoundedDouble) 1.23
                });
                Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeight = (RoundedDouble) 2.45
                });
            }
        }

        private class TestHydraulicBoundaryLocationRow : HydraulicBoundaryLocationRow
        {
            public TestHydraulicBoundaryLocationRow(HydraulicBoundaryLocation hydraulicBoundaryLocation)
                : base(hydraulicBoundaryLocation) {}
        }

        private sealed class TestHydraulicBoundaryLocationsView : HydraulicBoundaryLocationsView<TestHydraulicBoundaryLocationRow>
        {
            public TestHydraulicBoundaryLocationsView()
            {
                LocationsToCalculate = new List<HydraulicBoundaryLocation>();
            }

            public override IAssessmentSection AssessmentSection { get; set; }

            public IEnumerable<HydraulicBoundaryLocation> LocationsToCalculate { get; private set; }

            public object CreateForSelection { get; set; }

            protected override TestHydraulicBoundaryLocationRow CreateNewRow(HydraulicBoundaryLocation location)
            {
                return new TestHydraulicBoundaryLocationRow(location);
            }

            protected override object CreateSelectedItemFromCurrentRow()
            {
                return CreateForSelection;
            }

            protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
            {
                LocationsToCalculate = locations;
            }
        }
    }
}