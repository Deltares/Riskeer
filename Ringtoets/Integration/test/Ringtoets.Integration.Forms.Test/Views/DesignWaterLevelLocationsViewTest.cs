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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class DesignWaterLevelLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int locationNameColumnIndex = 1;
        private const int locationIdColumnIndex = 2;
        private const int locationColumnIndex = 3;
        private const int locationDesignWaterlevelColumnIndex = 4;
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
            using (var view = new DesignWaterLevelLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView<DesignWaterLevelLocationRow>>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Call
            using (var view = new DesignWaterLevelLocationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithLocations_ReturnsSelectedLocationWrappedInContext()
        {
            // Call
            using (var view = ShowFullyConfiguredDesignWaterLevelLocationsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var selectedLocationRow = dataGridView.Rows[0];
                selectedLocationRow.Cells[0].Value = true;

                // Assert
                var selection = view.Selection as DesignWaterLevelLocationContext;
                var dataBoundItem = selectedLocationRow.DataBoundItem as DesignWaterLevelLocationRow;

                Assert.NotNull(selection);
                Assert.NotNull(dataBoundItem);
                Assert.AreSame(dataBoundItem.HydraulicBoundaryLocation, selection.HydraulicBoundaryLocation);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowDesignWaterLevelLocationsView();

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

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button) buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void DesignWaterLevelLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredDesignWaterLevelLocationsView();

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
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

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
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryDatabaseUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            IAssessmentSection assessmentSection = view.AssessmentSection;
            HydraulicBoundaryDatabase newHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevel = (RoundedDouble) 10.23
            };
            newHydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            assessmentSection.HydraulicBoundaryDatabase = newHydraulicBoundaryDatabase;

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            var cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(hydraulicBoundaryLocation.DesignWaterLevel, cells[locationDesignWaterlevelColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_AssessmentSectionUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            IAssessmentSection assessmentSection = view.AssessmentSection;

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            // Call
            assessmentSection.HydraulicBoundaryDatabase.Locations.ForEach(loc => loc.DesignWaterLevel = RoundedDouble.NaN);
            assessmentSection.NotifyObservers();

            // Assert
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevels(bool isSuccessful)
        {
            // Setup
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            IAssessmentSection assessmentSection = view.AssessmentSection;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var mockRepository = new MockRepository();
            var guiServiceMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = mockRepository.StrictMock<IObserver>();
            assessmentSection.HydraulicBoundaryDatabase.Attach(observer);

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            ICalculationMessageProvider messageProvider = null;
            IEnumerable<HydraulicBoundaryLocation> locations = null;
            guiServiceMock.Expect(ch => ch.CalculateDesignWaterLevels(null, null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    locations = (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[1];
                    messageProvider = (ICalculationMessageProvider) invocation.Arguments[4];
                }).Return(isSuccessful);
            mockRepository.ReplayAll();

            view.CalculationGuiService = guiServiceMock;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationMessageProvider>(messageProvider);
            var hydraulicBoundaryLocations = locations.ToArray();
            Assert.AreEqual(1, hydraulicBoundaryLocations.Length);
            HydraulicBoundaryLocation expectedLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, hydraulicBoundaryLocations.First());

            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredDesignWaterLevelLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        private DesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView()
        {
            var view = new DesignWaterLevelLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private DesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView()
        {
            var view = ShowDesignWaterLevelLocationsView();

            var assessmentSection = new TestAssessmentSection()
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            view.Data = assessmentSection.HydraulicBoundaryDatabase.Locations;
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
                    DesignWaterLevel = (RoundedDouble) 1.23
                });
                Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeight = (RoundedDouble) 2.45
                });
            }
        }

        private class TestAssessmentSection : Observable, IAssessmentSection
        {
            public TestAssessmentSection()
            {
                FailureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 0, 300000);
            }

            public string Comments { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public AssessmentSectionComposition Composition { get; private set; }
            public ReferenceLine ReferenceLine { get; set; }
            public FailureMechanismContribution FailureMechanismContribution { get; private set; }
            public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

            public IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }

            public void ChangeComposition(AssessmentSectionComposition newComposition) {}
        }
    }
}