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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int includeIllustrationPointsColumnIndex = 1;
        private const int locationNameColumnIndex = 2;
        private const int locationIdColumnIndex = 3;
        private const int locationColumnIndex = 4;
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
        public void Constructor_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationsView(null,
                                                                             hbl => new HydraulicBoundaryLocationCalculation(),
                                                                             new ObservableTestAssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void Constructor_GetCalculationFuncNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                                             null,
                                                                             new ObservableTestAssessmentSectionStub());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getCalculationFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                                             hbl => new HydraulicBoundaryLocationCalculation(),
                                                                             null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup & Call
            TestHydraulicBoundaryLocationsView view = ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            // Assert
            Assert.IsInstanceOf<LocationsView<HydraulicBoundaryLocation>>(view);
            Assert.IsNull(view.Data);
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowTestHydraulicBoundaryLocationsView();

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "dataGridView");
            Assert.AreEqual(5, dataGridView.ColumnCount);

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

            var button = (Button) testForm.Controls.Find("CalculateForSelectedButton", true).First();
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void HydraulicBoundaryLocationsView_AssessmentSectionWithData_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            // Assert
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "dataGridView");
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(3, rows.Count);

            DataGridViewCellCollection cells = rows[0].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);

            cells = rows[2].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredTestHydraulicBoundaryLocationsView();

            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "dataGridView");
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void GetIllustrationPointControlItems_ViewWithData_ReturnsExpectedControlItems()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var locations = new ObservableList<HydraulicBoundaryLocation>
            {
                hydraulicBoundaryLocation
            };

            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };

            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            var output = new TestHydraulicBoundaryLocationOutput(generalResult);

            var calculation = new HydraulicBoundaryLocationCalculation
            {
                Output = output
            };

            HydraulicBoundaryLocation getCalculationsCallArgument = null;

            TestHydraulicBoundaryLocationsView view = ShowTestHydraulicBoundaryLocationsView(locations, hbl =>
            {
                getCalculationsCallArgument = hbl;

                return calculation;
            });

            // Call
            IEnumerable<IllustrationPointControlItem> actualControlItems =
                view.PublicGetIllustrationPointControlItems();

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, getCalculationsCallArgument);

            IEnumerable<IllustrationPointControlItem> expectedControlItems =
                CreateControlItems(generalResult);
            CollectionAssert.AreEqual(expectedControlItems, actualControlItems,
                                      new IllustrationPointControlItemComparer());
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

        private void ShowTestHydraulicBoundaryLocationsView()
        {
            ShowTestHydraulicBoundaryLocationsView(new ObservableList<HydraulicBoundaryLocation>(),
                                                   hbl => new HydraulicBoundaryLocationCalculation());
        }

        private TestHydraulicBoundaryLocationsView ShowTestHydraulicBoundaryLocationsView(ObservableList<HydraulicBoundaryLocation> locations,
                                                                                          Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc)
        {
            var view = new TestHydraulicBoundaryLocationsView(locations,
                                                              getCalculationFunc,
                                                              new ObservableTestAssessmentSectionStub());

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private TestHydraulicBoundaryLocationsView ShowFullyConfiguredTestHydraulicBoundaryLocationsView()
        {
            var locations = new ObservableList<HydraulicBoundaryLocation>();

            locations.AddRange(new[]
            {
                new HydraulicBoundaryLocation(1, "1", 1.0, 1.0),
                new HydraulicBoundaryLocation(2, "2", 2.0, 2.0),
                new HydraulicBoundaryLocation(3, "3", 3.0, 3.0)
            });

            var calculations = new[]
            {
                new HydraulicBoundaryLocationCalculation(),
                new HydraulicBoundaryLocationCalculation
                {
                    Output = new TestHydraulicBoundaryLocationOutput(1.23)
                },
                new HydraulicBoundaryLocationCalculation
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    }
                }
            };

            return ShowTestHydraulicBoundaryLocationsView(locations,
                                                          hbl => calculations[locations.IndexOf(hbl)]);
        }

        private sealed class TestHydraulicBoundaryLocationsView : HydraulicBoundaryLocationsView
        {
            public TestHydraulicBoundaryLocationsView(ObservableList<HydraulicBoundaryLocation> locations,
                                                      Func<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> getCalculationFunc,
                                                      IAssessmentSection assessmentSection)
                : base(locations,
                       getCalculationFunc,
                       assessmentSection) {}

            public IEnumerable<IllustrationPointControlItem> PublicGetIllustrationPointControlItems()
            {
                return GetIllustrationPointControlItems();
            }

            protected override void HandleCalculateSelectedLocations(IEnumerable<HydraulicBoundaryLocation> locations)
            {
                throw new NotImplementedException();
            }

            protected override object CreateSelectedItemFromCurrentRow()
            {
                return null;
            }
        }
    }
}