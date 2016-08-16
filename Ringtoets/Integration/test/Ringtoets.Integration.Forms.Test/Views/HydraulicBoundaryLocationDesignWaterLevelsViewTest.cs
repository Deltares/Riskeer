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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.Commands;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationDesignWaterLevelsViewTest
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
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.IsNull(view.AssessmentSection);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowHydraulicBoundaryLocationDesignWaterLevelsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(5, dataGridView.ColumnCount);

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

            var locationDesignWaterlevelColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationDesignWaterlevelColumnIndex];
            const string expectedLocationDesignWaterHeaderText = "Toetspeil [m+NAP]";
            Assert.AreEqual(expectedLocationDesignWaterHeaderText, locationDesignWaterlevelColumn.HeaderText);
        }

        [Test]
        public void Data_HydraulicBoundaryDatabase_DataSet()
        {
            // Setup
            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView())
            {
                var hydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase();

                // Call
                view.Data = hydraulicBoundaryDatabase;

                // Assert
                Assert.AreSame(hydraulicBoundaryDatabase, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanHydraulicBoundaryDatabase_DataNull()
        {
            // Setup
            using (var view = new HydraulicBoundaryLocationDesignWaterLevelsView())
            {
                var data = new object();

                // Call
                view.Data = data;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void HydraulicBoundaryLocationDesignWaterLevelsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            var cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("NaN", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("NaN", cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void HydraulicBoundaryLocationDesignWaterLevelsView_HydraulicBoundaryDatabaseUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            HydraulicBoundaryLocationDesignWaterLevelsView view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();
            HydraulicBoundaryDatabase newHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            newHydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevel = 10.23
            });

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            // Call
            view.Data = newHydraulicBoundaryDatabase;

            // Assert
            Assert.AreEqual(1, rows.Count);
            var cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(10.23.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void HydraulicBoundaryLocationDesignWaterLevelsView_AssessmentSectionUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            HydraulicBoundaryLocationDesignWaterLevelsView view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();
            IAssessmentSection section = view.AssessmentSection;

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("NaN", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("NaN", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            // Call
            section.HydraulicBoundaryDatabase.Locations.ForEach(loc => loc.DesignWaterLevel = double.NaN);
            section.NotifyObservers();

            // Assert
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("NaN", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("NaN", rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("NaN", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void HydraulicBoundaryLocationDesignWaterLevelsView_SelectingCellInRow_ApplicationSelectionCorrectlySynced()
        {
            // Setup
            var view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();
            var secondHydraulicBoundaryLocation = ((HydraulicBoundaryDatabase) view.Data).Locations.Skip(1).First();

            var mocks = new MockRepository();
            var applicationSelectionMock = mocks.StrictMock<IApplicationSelection>();
            applicationSelectionMock.Stub(asm => asm.Selection).Return(null);
            applicationSelectionMock.Expect(asm => asm.Selection = new DesignWaterLevelLocationContext(secondHydraulicBoundaryLocation));
            mocks.ReplayAll();

            view.ApplicationSelection = applicationSelectionMock;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[locationNameColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(1, 0));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void HydraulicBoundaryLocationDesignWaterLevelsView_SelectingCellInAlreadySelectedRow_ApplicationSelectionNotSyncedRedundantly()
        {
            // Setup
            var view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();
            var secondHydraulicBoundaryLocation = ((HydraulicBoundaryDatabase) view.Data).Locations.Skip(1).First();

            var mocks = new MockRepository();
            var applicationSelectionMock = mocks.StrictMock<IApplicationSelection>();
            applicationSelectionMock.Stub(asm => asm.Selection).Return(new DesignWaterLevelLocationContext(secondHydraulicBoundaryLocation));
            mocks.ReplayAll();

            view.ApplicationSelection = applicationSelectionMock;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[locationNameColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(locationNameColumnIndex, 0));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Selection_Always_ReturnsTheSelectedRowObject(int selectedRow)
        {
            // Setup
            var view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[locationNameColumnIndex];

            // Call
            var selection = view.Selection;

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelLocationContext>(selection);

            var dataRow = (HydraulicBoundaryLocationDesignWaterLevelRow) dataGridView.Rows[selectedRow].DataBoundItem;
            Assert.AreSame(dataRow.HydraulicBoundaryLocation, ((DesignWaterLevelLocationContext) selection).WrappedData);
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllLocationsSelected()
        {
            // Setup
            ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();

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
            ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();

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
        public void CalculateForSelectedButton_NoneSelected_CallsCalculateDesignWaterLevels()
        {
            // Setup
            HydraulicBoundaryLocationDesignWaterLevelsView view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();

            var mockRepository = new MockRepository();
            var commandHandlerMock = mockRepository.StrictMock<ICalculateDesignWaterLevelCommandHandler>();

            IEnumerable<HydraulicBoundaryLocation> locations = null;
            commandHandlerMock.Expect(ch => ch.CalculateDesignWaterLevels(null)).IgnoreArguments().WhenCalled(
                invocation => { locations = (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[0]; });
            mockRepository.ReplayAll();

            view.CalculationCommandHandler = commandHandlerMock;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            Assert.AreEqual(0, locations.Count());
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevels()
        {
            // Setup
            HydraulicBoundaryLocationDesignWaterLevelsView view = ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var mockRepository = new MockRepository();
            var commandHandlerMock = mockRepository.StrictMock<ICalculateDesignWaterLevelCommandHandler>();

            IEnumerable<HydraulicBoundaryLocation> locations = null;
            commandHandlerMock.Expect(ch => ch.CalculateDesignWaterLevels(null)).IgnoreArguments().WhenCalled(
                invocation => { locations = (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[0]; });
            mockRepository.ReplayAll();

            view.CalculationCommandHandler = commandHandlerMock;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            var hydraulicBoundaryLocations = locations.ToArray();
            Assert.AreEqual(1, hydraulicBoundaryLocations.Length);
            HydraulicBoundaryLocation expectedLocation = view.AssessmentSection.HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, hydraulicBoundaryLocations.First());
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_CalculationCommandHandlerNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView();
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        private const int locationCalculateColumnIndex = 0;
        private const int locationNameColumnIndex = 1;
        private const int locationIdColumnIndex = 2;
        private const int locationColumnIndex = 3;
        private const int locationDesignWaterlevelColumnIndex = 4;

        private HydraulicBoundaryLocationDesignWaterLevelsView ShowHydraulicBoundaryLocationDesignWaterLevelsView()
        {
            var view = new HydraulicBoundaryLocationDesignWaterLevelsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private HydraulicBoundaryLocationDesignWaterLevelsView ShowFullyConfiguredHydraulicBoundaryLocationDesignWaterLevelsView()
        {
            var view = ShowHydraulicBoundaryLocationDesignWaterLevelsView();

            var assessmentSection = new TestAssessmentSection()
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            view.Data = assessmentSection.HydraulicBoundaryDatabase;
            view.AssessmentSection = assessmentSection;
            return view;
        }

        private class TestHydraulicBoundaryDatabase : HydraulicBoundaryDatabase
        {
            public TestHydraulicBoundaryDatabase()
            {
                Locations.Add(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0));
                Locations.Add(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevel = 1.23
                });
                Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeight = 2.45
                });
            }
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public string Comments { get; set; }
            public long StorageId { get; set; }
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
    }
}