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
using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;
using Core.Common.Utils.Reflection;
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
    public class HydraulicBoundaryLocationsViewTest
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
            using (var view = new TestHydraulicBoundaryLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
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
        public void Data_IAssessmentSection_DataSet()
        {
            // Setup
            using (var view = new TestHydraulicBoundaryLocationsView())
            {
                var assessmentSection = new TestAssessmentSection();

                // Call
                view.Data = assessmentSection;

                // Assert
                Assert.AreSame(assessmentSection, view.Data);
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
        public void TestHydraulicBoundaryLocationsView_SelectingCellInRow_ApplicationSelectionCorrectlySynced()
        {
            // Setup
            var view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();
            IAssessmentSection assessmentSection = (IAssessmentSection) view.Data;
            var secondHydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.Skip(1).First();

            var mocks = new MockRepository();
            var applicationSelectionMock = mocks.StrictMock<IApplicationSelection>();
            applicationSelectionMock.Stub(asm => asm.Selection).Return(null);
            applicationSelectionMock.Expect(asm => asm.Selection = new TestHydraulicBoundaryLocationContext(
                                                                       assessmentSection.HydraulicBoundaryDatabase,
                                                                       secondHydraulicBoundaryLocation));
            mocks.ReplayAll();

            view.ApplicationSelection = applicationSelectionMock;

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[locationCalculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void TestHydraulicBoundaryLocationsView_SelectingCellInAlreadySelectedRow_ApplicationSelectionNotSyncedRedundantly()
        {
            // Setup
            var view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[locationCalculateColumnIndex];

            var mocks = new MockRepository();
            var applicationSelectionMock = mocks.StrictMock<IApplicationSelection>();
            applicationSelectionMock.Stub(asm => asm.Selection).Return(view.Selection);
            mocks.ReplayAll();

            view.ApplicationSelection = applicationSelectionMock;

            // Call
            dataGridView.CurrentCell = dataGridView.Rows[1].Cells[locationCalculateColumnIndex];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(locationCalculateColumnIndex, 0));

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
            var view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            dataGridView.CurrentCell = dataGridView.Rows[selectedRow].Cells[locationCalculateColumnIndex];

            // Call
            var selection = view.Selection;

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationContext>(selection);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = ((IAssessmentSection) view.Data).HydraulicBoundaryDatabase;
            Assert.AreSame(hydraulicBoundaryDatabase, ((HydraulicBoundaryLocationContext) selection).WrappedData);
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
            var commandHandlerMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationCommandHandler>();
            mockRepository.ReplayAll();

            TestHydraulicBoundaryLocationsView view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();
            view.CalculationCommandHandler = commandHandlerMock;
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
            var commandHandlerMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationCommandHandler>();
            mockRepository.ReplayAll();

            view.CalculationCommandHandler = commandHandlerMock;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.AreEqual(1, view.LocationsToCalculate.Count());
            HydraulicBoundaryLocation expectedLocation = ((IAssessmentSection) view.Data).HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, view.LocationsToCalculate.First());
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationCommandHandlerNotSet_DoesNotThrowException()
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

        private const int locationCalculateColumnIndex = 0;

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

            var assessmentSection = new TestAssessmentSection()
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            view.Data = assessmentSection;
            return view;
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

        private class TestHydraulicBoundaryLocationContext : HydraulicBoundaryLocationContext
        {
            public TestHydraulicBoundaryLocationContext(HydraulicBoundaryDatabase wrappedData, HydraulicBoundaryLocation hydraulicBoundaryLocation)
                : base(wrappedData, hydraulicBoundaryLocation) {}
        }

        private class TestHydraulicBoundaryLocationContextRow : HydraulicBoundaryLocationContextRow
        {
            public TestHydraulicBoundaryLocationContextRow(HydraulicBoundaryLocationContext hydraulicBoundaryLocationContext)
                : base(hydraulicBoundaryLocationContext) {}
        }

        private class TestHydraulicBoundaryLocationsView : HydraulicBoundaryLocationsView
        {
            public TestHydraulicBoundaryLocationsView()
            {
                dataGridViewControl.AddCheckBoxColumn(TypeUtils.GetMemberName<HydraulicBoundaryLocationContextRow>(row => row.ToCalculate), "");
                LocationsToCalculate = new List<HydraulicBoundaryLocation>();
            }

            public IEnumerable<HydraulicBoundaryLocation> LocationsToCalculate { get; private set; }

            protected override void SetDataSource()
            {
                dataGridViewControl.SetDataSource(AssessmentSection != null && AssessmentSection.HydraulicBoundaryDatabase != null
                                                      ? AssessmentSection.HydraulicBoundaryDatabase.Locations.Select(
                                                          hl => new TestHydraulicBoundaryLocationContextRow(
                                                                    new TestHydraulicBoundaryLocationContext(AssessmentSection.HydraulicBoundaryDatabase, hl))).ToArray()
                                                      : null);
            }

            protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
            {
                LocationsToCalculate = locations;
            }
        }
    }
}