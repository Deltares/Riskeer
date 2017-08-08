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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.Controls.DataGrid;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class WaveHeightLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationWaveHeightColumnIndex = 5;
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
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new WaveHeightLocationsView(new ObservableTestAssessmentSectionStub()))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingRowInLocationsTable_ThenReturnSelectedLocation()
        {
            // Given
            WaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
            DataGridViewRow currentRow = dataGridView.Rows[1];

            HydraulicBoundaryLocation location = ((HydraulicBoundaryLocationRow) currentRow.DataBoundItem).CalculatableObject;

            // When
            dataGridView.CurrentCell = currentRow.Cells[0];
            EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));
            var selection = view.Selection as WaveHeightLocationContext;

            // Then
            Assert.IsNotNull(selection);
            Assert.AreSame(location, selection.HydraulicBoundaryLocation);
        }

        [Test]
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Call
            using (var view = new WaveHeightLocationsView(new ObservableTestAssessmentSectionStub()))
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_LocationWithoutOutput_IllustrationPointsControlDataSetToNull()
        {
            // Setup
            ShowFullyConfiguredWaveHeightLocationsView();
            IllustrationPointsControl illustrationPointsControl = ControlTestHelper.GetControls<IllustrationPointsControl>(testForm,
                                                                                                                           "IllustrationPointsControl").First();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");

            // Call
            dataGridView.SetCurrentCell(dataGridView.GetCell(0, 1));

            // Assert
            Assert.IsNull(illustrationPointsControl.Data);
        }

        [Test]
        public void Selection_LocationWithoutGeneralResult_IllustrationPointsControlDataSetToNull()
        {
            // Setup
            ShowFullyConfiguredWaveHeightLocationsView();
            IllustrationPointsControl illustrationPointsControl = ControlTestHelper.GetControls<IllustrationPointsControl>(testForm,
                                                                                                                           "IllustrationPointsControl").First();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");

            // Call
            dataGridView.SetCurrentCell(dataGridView.GetCell(1, 0));

            // Assert
            Assert.IsNull(illustrationPointsControl.Data);
        }

        [Test]
        public void Selection_LocationWithGeneralResult_GeneralResultSetOnIllustrationPointsControlData()
        {
            // Setup
            ShowFullyConfiguredWaveHeightLocationsView();
            IllustrationPointsControl illustrationPointsControl = ControlTestHelper.GetControls<IllustrationPointsControl>(testForm,
                                                                                                                           "IllustrationPointsControl").First();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");

            // Call
            dataGridView.SetCurrentCell(dataGridView.GetCell(4, 0));

            // Assert
            Assert.IsNotNull(illustrationPointsControl.Data);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowWaveHeightLocationsView(new ObservableTestAssessmentSectionStub());

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
            Assert.AreEqual(6, dataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var locationWaveHeightColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationWaveHeightColumnIndex];
            Assert.AreEqual("Hs [m]", locationWaveHeightColumn.HeaderText);

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void WaveHeightLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredWaveHeightLocationsView();

            // Assert
            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(5, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationWaveHeightColumnIndex].FormattedValue);

            cells = rows[4].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(5, 5).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[locationWaveHeightColumnIndex].FormattedValue);
        }

        [Test]
        public void WaveHeightLocationsView_HydraulicBoundaryDatabaseUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            WaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            IAssessmentSection assessmentSection = view.AssessmentSection;
            var newHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(10.23)
                }
            };
            newHydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            // Precondition
            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(5, rows.Count);

            // Call
            assessmentSection.HydraulicBoundaryDatabase = newHydraulicBoundaryDatabase;
            assessmentSection.NotifyObservers();

            // Assert
            Assert.AreEqual(1, rows.Count);
            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(hydraulicBoundaryLocation.WaveHeight, cells[locationWaveHeightColumnIndex].Value);
        }

        [Test]
        public void WaveHeightLocationsView_AssessmentSectionUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            WaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            IAssessmentSection assessmentSection = view.AssessmentSection;

            // Precondition
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "DataGridView");
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(5, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), rows[1].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), rows[4].Cells[locationWaveHeightColumnIndex].FormattedValue);

            assessmentSection.HydraulicBoundaryDatabase.Locations.ForEach(loc => loc.WaveHeightCalculation.Output = null);

            var refreshed = false;
            dataGridView.Invalidated += (sender, args) => refreshed = true;

            // Call
            assessmentSection.NotifyObservers();

            // Assert
            Assert.IsTrue(refreshed);
            Assert.AreEqual(5, rows.Count);
            Assert.AreEqual("-", rows[0].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[1].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[2].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[3].Cells[locationWaveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", rows[4].Cells[locationWaveHeightColumnIndex].FormattedValue);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateWaveHeights(bool isSuccessful)
        {
            // Setup
            WaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            IAssessmentSection assessmentSection = view.AssessmentSection;
            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var mockRepository = new MockRepository();
            var guiService = mockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = mockRepository.StrictMock<IObserver>();
            assessmentSection.HydraulicBoundaryDatabase.Attach(observer);

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            IEnumerable<HydraulicBoundaryLocation> locations = null;
            guiService.Expect(ch => ch.CalculateWaveHeights(null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation => { locations = (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[1]; }).Return(isSuccessful);
            mockRepository.ReplayAll();

            view.CalculationGuiService = guiService;
            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            button.Click();

            // Assert
            HydraulicBoundaryLocation[] hydraulicBoundaryLocations = locations.ToArray();
            Assert.AreEqual(1, hydraulicBoundaryLocations.Length);
            HydraulicBoundaryLocation expectedLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, hydraulicBoundaryLocations.First());
            mockRepository.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredWaveHeightLocationsView();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void WaveHeightLocationsView_HydraulicBoundaryDatabaseNotifyObservers_UpdateIllustrationPointsControlData()
        {
            // Setup
            WaveHeightLocationsView view = ShowFullyConfiguredWaveHeightLocationsView();
            IllustrationPointsControl illustrationPointsControl = ControlTestHelper.GetControls<IllustrationPointsControl>(testForm,
                                                                                                                           "IllustrationPointsControl").First();

            DataGridViewControl dataGridView = ControlTestHelper.GetDataGridViewControl(testForm, "DataGridViewControl");

            dataGridView.SetCurrentCell(dataGridView.GetCell(3, 0));

            // Precondition
            Assert.IsNull(illustrationPointsControl.Data);

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationOutput(generalResult);

            // Call
            view.AssessmentSection.HydraulicBoundaryDatabase.Locations[3].WaveHeightCalculation.Output = output;
            view.AssessmentSection.HydraulicBoundaryDatabase.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        private static IEnumerable<IllustrationPointControlItem> CreateControlItems(
            GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            return generalResult.TopLevelIllustrationPoints
                                .Select(topLevelIllustrationPoint =>
                                {
                                    SubMechanismIllustrationPoint illustrationPoint = topLevelIllustrationPoint.SubMechanismIllustrationPoint;
                                    return new IllustrationPointControlItem(topLevelIllustrationPoint,
                                                                            topLevelIllustrationPoint.WindDirection.Name,
                                                                            topLevelIllustrationPoint.ClosingSituation,
                                                                            illustrationPoint.Stochasts,
                                                                            illustrationPoint.Beta);
                                });
        }

        private WaveHeightLocationsView ShowWaveHeightLocationsView(IAssessmentSection assessmentSection)
        {
            var view = new WaveHeightLocationsView(assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private WaveHeightLocationsView ShowFullyConfiguredWaveHeightLocationsView()
        {
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            WaveHeightLocationsView view = ShowWaveHeightLocationsView(assessmentSection);

            view.Data = assessmentSection.HydraulicBoundaryDatabase.Locations;
            return view;
        }

        private class TestHydraulicBoundaryDatabase : HydraulicBoundaryDatabase
        {
            public TestHydraulicBoundaryDatabase()
            {
                Locations.Add(new HydraulicBoundaryLocation(1, "1", 1.0, 1.0));
                Locations.Add(new HydraulicBoundaryLocation(2, "2", 2.0, 2.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                });
                Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                });
                Locations.Add(new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                });
                var output = new TestHydraulicBoundaryLocationOutput(1.01, new TestGeneralResultSubMechanismIllustrationPoint());
                Locations.Add(new HydraulicBoundaryLocation(5, "5", 5.0, 5.0)
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = output
                    }
                });
            }
        }
    }
}