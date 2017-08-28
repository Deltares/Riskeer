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
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Service.MessageProviders;

namespace Ringtoets.Integration.Forms.Test.Views
{
    [TestFixture]
    public class DesignWaterLevelLocationsViewTest : LocationsViewSynchronizationTester<HydraulicBoundaryLocation>
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
        private const int locationDesignWaterlevelColumnIndex = 5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = MockRepository.Stub<IAssessmentSection>();
            MockRepository.ReplayAll();

            // Call
            using (var view = new DesignWaterLevelLocationsView(assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<HydraulicBoundaryLocationsView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var assessmentSection = MockRepository.Stub<IAssessmentSection>();
            MockRepository.ReplayAll();

            // Call
            ShowDesignWaterLevelLocationsView(assessmentSection, TestForm);

            // Assert
            DataGridView locationsDataGridView = GetLocationsDataGridView();
            Assert.AreEqual(6, locationsDataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) locationsDataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var includeIllustrationPointsColumn = (DataGridViewCheckBoxColumn) locationsDataGridView.Columns[includeIllustrationPointsColumnIndex];
            Assert.AreEqual("Illustratiepunten inlezen", includeIllustrationPointsColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var locationDesignWaterlevelColumn = (DataGridViewTextBoxColumn) locationsDataGridView.Columns[locationDesignWaterlevelColumnIndex];
            Assert.AreEqual("Toetspeil [m+NAP]", locationDesignWaterlevelColumn.HeaderText);

            var button = (Button) TestForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void DesignWaterLevelLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredDesignWaterLevelLocationsView(TestForm);

            // Assert
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            Assert.AreEqual(5, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[locationDesignWaterlevelColumnIndex].FormattedValue);

            cells = rows[4].Cells;
            Assert.AreEqual(6, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("5", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(5, 5).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual(1.01.ToString(CultureInfo.CurrentCulture), cells[locationDesignWaterlevelColumnIndex].FormattedValue);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryDatabaseUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(TestForm);
            IAssessmentSection assessmentSection = view.AssessmentSection;
            var newHydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(10, "10", 10.0, 10.0)
            {
                DesignWaterLevelCalculation =
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
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            Assert.AreEqual(5, rows.Count);

            assessmentSection.HydraulicBoundaryDatabase = newHydraulicBoundaryDatabase;

            // Call
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
            Assert.AreEqual(hydraulicBoundaryLocation.DesignWaterLevel, cells[locationDesignWaterlevelColumnIndex].Value);
        }

        [Test]
        public void DesignWaterLevelLocationsView_HydraulicBoundaryDatabaseUpdated_IllustrationPointsControlCorrectlyUpdated()
        {
            // Setup
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(TestForm);
            IllustrationPointsControl illustrationPointsControl = GetIllustrationPointsControl();
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();

            locationsDataGridViewControl.SetCurrentCell(locationsDataGridViewControl.GetCell(3, 0));

            // Precondition
            CollectionAssert.IsEmpty(illustrationPointsControl.Data);

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationOutput(generalResult);

            // Call
            view.AssessmentSection.HydraulicBoundaryDatabase.Locations[3].DesignWaterLevelCalculation.Output = output;
            view.AssessmentSection.HydraulicBoundaryDatabase.NotifyObservers();

            // Assert
            IEnumerable<IllustrationPointControlItem> expectedControlItems = CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, illustrationPointsControl.Data, new IllustrationPointControlItemComparer());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateDesignWaterLevels(bool isSuccessful)
        {
            // Setup
            DesignWaterLevelLocationsView view = ShowFullyConfiguredDesignWaterLevelLocationsView(TestForm);
            IAssessmentSection assessmentSection = view.AssessmentSection;
            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var guiService = MockRepository.StrictMock<IHydraulicBoundaryLocationCalculationGuiService>();

            var observer = MockRepository.StrictMock<IObserver>();
            assessmentSection.HydraulicBoundaryDatabase.Attach(observer);

            if (isSuccessful)
            {
                observer.Expect(o => o.UpdateObserver());
            }

            ICalculationMessageProvider messageProvider = null;
            IEnumerable<HydraulicBoundaryLocation> locations = null;
            guiService.Expect(ch => ch.CalculateDesignWaterLevels(null, null, 1, null)).IgnoreArguments().WhenCalled(
                invocation =>
                {
                    locations = (IEnumerable<HydraulicBoundaryLocation>) invocation.Arguments[1];
                    messageProvider = (ICalculationMessageProvider) invocation.Arguments[3];
                }).Return(isSuccessful);
            MockRepository.ReplayAll();

            view.CalculationGuiService = guiService;
            var buttonTester = new ButtonTester("CalculateForSelectedButton", TestForm);

            // Call
            buttonTester.Click();

            // Assert
            Assert.IsInstanceOf<DesignWaterLevelCalculationMessageProvider>(messageProvider);
            HydraulicBoundaryLocation[] hydraulicBoundaryLocations = locations.ToArray();
            Assert.AreEqual(1, hydraulicBoundaryLocations.Length);
            HydraulicBoundaryLocation expectedLocation = assessmentSection.HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(expectedLocation, hydraulicBoundaryLocations.First());
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredDesignWaterLevelLocationsView(TestForm);

            DataGridViewControl locationsDataGridViewControl = GetLocationsDataGridViewControl();
            DataGridViewRowCollection rows = locationsDataGridViewControl.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", TestForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
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

        private static DesignWaterLevelLocationsView ShowDesignWaterLevelLocationsView(IAssessmentSection assessmentSection, Form form)
        {
            var view = new DesignWaterLevelLocationsView(assessmentSection);

            form.Controls.Add(view);
            form.Show();

            return view;
        }

        private static DesignWaterLevelLocationsView ShowFullyConfiguredDesignWaterLevelLocationsView(Form form)
        {
            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            DesignWaterLevelLocationsView view = ShowDesignWaterLevelLocationsView(assessmentSection, form);

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
                    DesignWaterLevelCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(1.23)
                    }
                });
                Locations.Add(new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
                {
                    WaveHeightCalculation =
                    {
                        Output = new TestHydraulicBoundaryLocationOutput(2.45)
                    }
                });
                Locations.Add(new HydraulicBoundaryLocation(4, "4", 4.0, 4.0)
                {
                    DesignWaterLevelCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                });

                var topLevelIllustrationPoints = new[]
                {
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "Regular",
                                                              new TestSubMechanismIllustrationPoint()),
                    new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                              "Regular",
                                                              new TestSubMechanismIllustrationPoint())
                };

                var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
                var output = new TestHydraulicBoundaryLocationOutput(1.01, generalResult);

                Locations.Add(new HydraulicBoundaryLocation(5, "5", 5.0, 5.0)
                {
                    DesignWaterLevelCalculation =
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

        #region LocationsViewSynchronizationTester implementation

        protected override int OutputColumnIndex
        {
            get
            {
                return locationDesignWaterlevelColumnIndex;
            }
        }

        protected override object GetLocationSelection(LocationsView<HydraulicBoundaryLocation> view, object selectedRowObject)
        {
            IAssessmentSection assessmentSection = view.AssessmentSection;

            return new DesignWaterLevelLocationContext(assessmentSection.HydraulicBoundaryDatabase,
                                                       ((HydraulicBoundaryLocationRow) selectedRowObject).CalculatableObject);
        }

        protected override LocationsView<HydraulicBoundaryLocation> ShowFullyConfiguredLocationsView(Form form)
        {
            return ShowFullyConfiguredDesignWaterLevelLocationsView(form);
        }

        protected override void ReplaceHydraulicBoundaryDatabaseAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
        {
            IAssessmentSection assessmentSection = view.AssessmentSection;
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

            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            assessmentSection.NotifyObservers();
        }

        protected override void ClearLocationOutputAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
        {
            IAssessmentSection assessmentSection = view.AssessmentSection;

            assessmentSection.HydraulicBoundaryDatabase.Locations.ForEach(loc => loc.DesignWaterLevelCalculation.Output = null);
            assessmentSection.NotifyObservers();
        }

        protected override void AddLocationOutputAndNotifyObservers(LocationsView<HydraulicBoundaryLocation> view)
        {
            IAssessmentSection assessmentSection = view.AssessmentSection;

            assessmentSection.HydraulicBoundaryDatabase.Locations.First().DesignWaterLevelCalculation.Output = new TestHydraulicBoundaryLocationOutput(new TestGeneralResultSubMechanismIllustrationPoint());
            assessmentSection.NotifyObservers();
        }

        #endregion
    }
}