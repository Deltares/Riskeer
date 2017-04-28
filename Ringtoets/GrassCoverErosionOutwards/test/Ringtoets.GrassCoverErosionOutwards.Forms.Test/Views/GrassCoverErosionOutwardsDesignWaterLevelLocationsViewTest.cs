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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionOutwardsDesignWaterLevelLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int locationNameColumnIndex = 1;
        private const int locationIdColumnIndex = 2;
        private const int locationColumnIndex = 3;
        private const int locationDesignWaterlevelColumnIndex = 4;

        private Form testForm;
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mockRepository = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mockRepository.VerifyAll();
        }

        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView())
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
            using (var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithLocations_ReturnsSelectedLocationWrappedInContext()
        {
            // Call
            using (GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRow selectedLocationRow = dataGridView.Rows[0];
                selectedLocationRow.Cells[0].Value = true;

                // Assert
                var selection = view.Selection as GrassCoverErosionOutwardsDesignWaterLevelLocationContext;
                var dataBoundItem = selectedLocationRow.DataBoundItem as DesignWaterLevelLocationRow;

                Assert.NotNull(selection);
                Assert.NotNull(dataBoundItem);
                Assert.AreSame(dataBoundItem.CalculatableObject, selection.HydraulicBoundaryLocation);
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
            const string expectedLocationDesignWaterHeaderText = "Waterstand bij doorsnede-eis [m+NAP]";
            Assert.AreEqual(expectedLocationDesignWaterHeaderText, locationDesignWaterlevelColumn.HeaderText);

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button) buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void GivenDesignWaterLevelLocationsView_WhenSettingAssessmentSectionAndDisposingView_ThenCorrectlyAttachAndDetachAssessmentSection()
        {
            // Setup
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            var assessmentSection = mockRepository.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.Attach(Arg<IObserver>.Is.NotNull)).IgnoreArguments();
            assessmentSection.Expect(a => a.Detach(Arg<IObserver>.Is.NotNull)).IgnoreArguments();
            mockRepository.ReplayAll();

            // Call
            view.AssessmentSection = assessmentSection;

            // Assert
            // Assertions based on mock expectancies
        }

        [Test]
        public void DesignWaterLevelLocationsView_WithNonIObservableList_ThrowsInvalidCastException()
        {
            // Setup
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView();

            var locations = new List<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(1.23)
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(2.45)
                }
            };

            // Call
            TestDelegate action = () => view.Data = locations;

            // Assert
            Assert.Throws<InvalidCastException>(action);
        }

        [Test]
        public void DesignWaterLevelLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredDesignWaterLevelLocationsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
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
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            object dataGridViewSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(3, rows.Count);

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(10.23)
            };

            locations.Clear();
            locations.Add(hydraulicBoundaryLocation);

            // Call
            locations.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(hydraulicBoundaryLocation.DesignWaterLevel, cells[locationDesignWaterlevelColumnIndex].Value);
            Assert.AreNotSame(dataGridViewSource, dataGridView.DataSource);
            Assert.IsFalse((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_EachHydraulicBoundaryLocationUpdated_DataGridViewRefreshedWithNewValues()
        {
            // Setup
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            locations.ForEach(loc => loc.DesignWaterLevelOutput = null);

            var refreshed = false;
            dataGridView.Invalidated += (sender, args) => refreshed = true;

            // Call
            locations.NotifyObservers();

            // Assert
            Assert.IsTrue(refreshed);
            Assert.AreEqual(3, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[1].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevelsSelectionNotChanged(bool isSuccessful)
        {
            // Setup
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();
            var locations = (ObservableList<HydraulicBoundaryLocation>) view.Data;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            object dataGridViewSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var guiServiceMock = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = mockRepository.StrictMock<IObserver>();
            locations.Attach(observer);

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            ICalculationMessageProvider messageProvider = null;
            HydraulicBoundaryLocation[] calculatedLocations = null;
            guiServiceMock.Expect(ch => ch.CalculateDesignWaterLevels(null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    calculatedLocations = ((IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[1]).ToArray();
                    messageProvider = (ICalculationMessageProvider) invocation.Arguments[3];
                }).Return(isSuccessful);

            var assessmentSectionStub = mockRepository.Stub<IAssessmentSection>();
            assessmentSectionStub.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            assessmentSectionStub.Stub(ass => ass.Id).Return(string.Empty);
            assessmentSectionStub.Stub(ass => ass.FailureMechanismContribution)
                                 .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));
            assessmentSectionStub.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSectionStub.Stub(a => a.Detach(null)).IgnoreArguments();
            mockRepository.ReplayAll();

            view.AssessmentSection = assessmentSectionStub;
            view.FailureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            view.CalculationGuiService = guiServiceMock;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider>(messageProvider);
            Assert.AreEqual(1, calculatedLocations.Length);
            HydraulicBoundaryLocation expectedLocation = locations.First();
            Assert.AreEqual(expectedLocation, calculatedLocations.First());
            Assert.AreSame(dataGridViewSource, dataGridView.DataSource);
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[2].Cells[locationCalculateColumnIndex].Value);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredDesignWaterLevelLocationsView();

            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        [TestCase(false, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, false, message)")]
        [TestCase(true, false, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, false, message)")]
        [TestCase(false, true, "Er zijn geen berekeningen geselecteerd.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, true, message)")]
        [TestCase(true, true, "", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, true, message)")]
        public void GivenDesignWaterLevelLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                                          bool contributionNotZero,
                                                                                                                                                                          string expectedErrorMessage)
        {
            // Given
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView();

            // When
            if (rowSelected)
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            if (contributionNotZero)
            {
                failureMechanism.Contribution = 5;
            }
            view.FailureMechanism = failureMechanism;

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && contributionNotZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView()
        {
            var view = new GrassCoverErosionOutwardsDesignWaterLevelLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private GrassCoverErosionOutwardsDesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView()
        {
            GrassCoverErosionOutwardsDesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView();
            view.Data = new ObservableList<HydraulicBoundaryLocation>
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    DesignWaterLevelOutput = new TestHydraulicBoundaryLocationOutput(1.23)
                },
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeightOutput = new TestHydraulicBoundaryLocationOutput(2.45)
                }
            };
            return view;
        }
    }
}