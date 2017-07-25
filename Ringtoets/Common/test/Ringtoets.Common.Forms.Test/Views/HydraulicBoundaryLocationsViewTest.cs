﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var view = new TestHydraulicBoundaryLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<LocationsView<HydraulicBoundaryLocation>>(view);
                Assert.IsNull(view.Data);
            }
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
        public void Data_HydraulicBoundaryLocations_DataSet()
        {
            // Setup
            using (var view = new TestHydraulicBoundaryLocationsView())
            {
                IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = Enumerable.Empty<HydraulicBoundaryLocation>();

                // Call
                view.Data = hydraulicBoundaryLocations;

                // Assert
                Assert.AreSame(hydraulicBoundaryLocations, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanHydraulicBoundaryLocations_DataNull()
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
            DataGridView dataGridView = ControlTestHelper.GetDataGridView(testForm, "dataGridView");
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(4, rows.Count);

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
            Assert.AreEqual(false, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(3, 3).ToString(), cells[locationColumnIndex].FormattedValue);

            cells = rows[3].Cells;
            Assert.AreEqual(5, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual(true, cells[includeIllustrationPointsColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("4", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(4, 4).ToString(), cells[locationColumnIndex].FormattedValue);
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
            TestHydraulicBoundaryLocationsView view = ShowTestHydraulicBoundaryLocationsView();

            var output = new TestHydraulicBoundaryLocationOutput(1);
            var topLevelIllustrationPoints = new[]
            {
                new TopLevelSubMechanismIllustrationPoint(WindDirectionTestFactory.CreateTestWindDirection(),
                                                          "Regular",
                                                          new TestSubMechanismIllustrationPoint())
            };
            var generalResult = new TestGeneralResultSubMechanismIllustrationPoint(topLevelIllustrationPoints);
            output.SetGeneralResult(generalResult);

            var calculation = new HydraulicBoundaryLocationCalculation
            {
                Output = output
            };
            view.ItemToCreate = calculation;

            var hydraulicBoundaryLocations = new ObservableList<TestHydraulicBoundaryLocation>();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            hydraulicBoundaryLocations.Add(hydraulicBoundaryLocation);

            view.Data = hydraulicBoundaryLocations;

            // Call
            IEnumerable<IllustrationPointControlItem> actualControlItems =
                view.PublicGetIllustrationPointControlItems();

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, view.GetCalculationsCallArgument);

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

        private TestHydraulicBoundaryLocationsView ShowTestHydraulicBoundaryLocationsView()
        {
            var view = new TestHydraulicBoundaryLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private void ShowFullyConfiguredTestHydraulicBoundaryLocationsView()
        {
            TestHydraulicBoundaryLocationsView view = ShowTestHydraulicBoundaryLocationsView();

            var assessmentSection = new ObservableTestAssessmentSectionStub
            {
                HydraulicBoundaryDatabase = new TestHydraulicBoundaryDatabase()
            };

            view.Data = assessmentSection.HydraulicBoundaryDatabase.Locations;
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
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        }
                    }
                });
            }
        }

        private class TestHydraulicBoundaryLocationRow : HydraulicBoundaryLocationRow
        {
            public TestHydraulicBoundaryLocationRow(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                    HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
                : base(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation) {}
        }

        private sealed class TestHydraulicBoundaryLocationsView : HydraulicBoundaryLocationsView
        {
            public TestHydraulicBoundaryLocationsView() : base(new ObservableTestAssessmentSectionStub()) {}

            public HydraulicBoundaryLocation GetCalculationsCallArgument { get; private set; }

            public HydraulicBoundaryLocationCalculation ItemToCreate { private get; set; }

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

            protected override HydraulicBoundaryLocationCalculation GetCalculation(HydraulicBoundaryLocation location)
            {
                GetCalculationsCallArgument = location;
                return ItemToCreate ?? location.WaveHeightCalculation;
            }
        }
    }
}