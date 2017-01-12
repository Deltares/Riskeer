﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Views;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int locationNameColumnIndex = 1;
        private const int locationIdColumnIndex = 2;
        private const int locationColumnIndex = 3;
        private const int coastalAreaIdColumnIndex = 4;
        private const int offsetColumnIndex = 5;
        private const int waterLevelColumnIndex = 6;
        private const int waveHeightColumnIndex = 7;
        private const int wavePeriodColumnIndex = 8;
        private const int d50ColumnIndex = 9;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

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
            using (var view = new DuneLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<CalculatableView>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowDuneLocationsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            Assert.AreEqual(10, dataGridView.ColumnCount);

            var locationCalculateColumn = (DataGridViewCheckBoxColumn) dataGridView.Columns[locationCalculateColumnIndex];
            Assert.AreEqual("Berekenen", locationCalculateColumn.HeaderText);

            var locationNameColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationNameColumnIndex];
            Assert.AreEqual("Naam", locationNameColumn.HeaderText);

            var locationIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationIdColumnIndex];
            Assert.AreEqual("ID", locationIdColumn.HeaderText);

            var locationColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[locationColumnIndex];
            Assert.AreEqual("Coördinaten [m]", locationColumn.HeaderText);

            var coastalAreaIdColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[coastalAreaIdColumnIndex];
            Assert.AreEqual("Kustvaknummer", coastalAreaIdColumn.HeaderText);

            var offssetColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[offsetColumnIndex];
            Assert.AreEqual("Metrering [dam]", offssetColumn.HeaderText);

            var waterLevelColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[waterLevelColumnIndex];
            Assert.AreEqual("Rekenwaarde waterstand [m+NAP]", waterLevelColumn.HeaderText);

            var waveHeightColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[waveHeightColumnIndex];
            Assert.AreEqual("Rekenwaarde Hs [m]", waveHeightColumn.HeaderText);

            var wavePeriodColumn = (DataGridViewTextBoxColumn) dataGridView.Columns[wavePeriodColumnIndex];
            Assert.AreEqual("Rekenwaarde Tp [s]", wavePeriodColumn.HeaderText);

            var d50Column = (DataGridViewTextBoxColumn) dataGridView.Columns[d50ColumnIndex];
            Assert.AreEqual("Rekenwaarde d50 [m]", d50Column.HeaderText);

            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);
            var button = (Button) buttonTester.TheObject;
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Data_DuneLocations_DataSet()
        {
            // Setup
            using (var view = new DuneLocationsView())
            {
                var duneLocations = new ObservableList<DuneLocation>();

                // Call
                view.Data = duneLocations;

                // Assert
                Assert.AreSame(duneLocations, view.Data);
            }
        }

        [Test]
        public void Data_OtherThanObservableListOfDuneLocations_ThrowsInvalidCastException()
        {
            // Setup
            var view = ShowDuneLocationsView();

            var locations = new List<DuneLocation>();

            // Call
            TestDelegate action = () => view.Data = locations;

            // Assert
            Assert.Throws<InvalidCastException>(action);
        }

        [Test]
        public void DuneLocationsView_DataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            ShowFullyConfiguredDuneLocationsView();

            // Assert
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            var cells = rows[0].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("1", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(1, 1).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("50", cells[coastalAreaIdColumnIndex].FormattedValue);
            Assert.AreEqual("320", cells[offsetColumnIndex].FormattedValue);
            Assert.AreEqual(0.000837.ToString(CultureInfo.CurrentCulture), cells[d50ColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", cells[wavePeriodColumnIndex].FormattedValue);

            cells = rows[1].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("2", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(2, 2).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("60", cells[coastalAreaIdColumnIndex].FormattedValue);
            Assert.AreEqual("230", cells[offsetColumnIndex].FormattedValue);
            Assert.AreEqual(0.000123.ToString(CultureInfo.CurrentCulture), cells[d50ColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(2.34.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(3.45.ToString(CultureInfo.CurrentCulture), cells[wavePeriodColumnIndex].FormattedValue);
        }

        [Test]
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Call
            using (var view = new DuneLocationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithLocations_ReturnsSelectedLocationWrappedInContext()
        {
            // Call
            using (var view = ShowFullyConfiguredDuneLocationsView())
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                var selectedLocationRow = dataGridView.Rows[0];
                selectedLocationRow.Cells[0].Value = true;

                // Assert
                var selection = view.Selection as DuneLocationContext;
                var dataBoundItem = selectedLocationRow.DataBoundItem as DuneLocationRow;

                Assert.NotNull(selection);
                Assert.NotNull(dataBoundItem);
                Assert.AreSame(dataBoundItem.DuneLocation, selection.DuneLocation);
            }
        }

        [Test]
        public void DuneLocationsView_DuneLocationsUpdated_DataGridViewCorrectlyUpdated()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            ObservableList<DuneLocation> locations = (ObservableList<DuneLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var dataGridViewSource = dataGridView.DataSource;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(2, rows.Count);

            var duneLocation = new DuneLocation(10, "10", new Point2D(10.0, 10.0), new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = 3,
                                                    Offset = 80,
                                                    D50 = 0.000321
                                                })
            {
                Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                                                {
                                                    WaterLevel = 3.21,
                                                    WaveHeight = 4.32,
                                                    WavePeriod = 5.43
                                                })
            };

            locations.Clear();
            locations.Add(duneLocation);

            // Call
            locations.NotifyObservers();

            // Assert
            Assert.AreNotSame(dataGridViewSource, dataGridView.DataSource);

            Assert.AreEqual(1, rows.Count);
            var cells = rows[0].Cells;
            Assert.AreEqual(10, cells.Count);
            Assert.AreEqual(false, cells[locationCalculateColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationNameColumnIndex].FormattedValue);
            Assert.AreEqual("10", cells[locationIdColumnIndex].FormattedValue);
            Assert.AreEqual(new Point2D(10, 10).ToString(), cells[locationColumnIndex].FormattedValue);
            Assert.AreEqual("3", cells[coastalAreaIdColumnIndex].FormattedValue);
            Assert.AreEqual("80", cells[offsetColumnIndex].FormattedValue);
            Assert.AreEqual(0.000321.ToString(CultureInfo.CurrentCulture), cells[d50ColumnIndex].FormattedValue);
            Assert.AreEqual(3.21.ToString(CultureInfo.CurrentCulture), cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(4.32.ToString(CultureInfo.CurrentCulture), cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(5.43.ToString(CultureInfo.CurrentCulture), cells[wavePeriodColumnIndex].FormattedValue);
        }

        [Test]
        public void DuneLocationsView_EachDuneLocationUpdated_DataGridViewRefreshedWithNewValues()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            ObservableList<DuneLocation> locations = (ObservableList<DuneLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);
            DataGridViewRow firstRow = rows[0];
            DataGridViewRow secondRow = rows[1];

            Assert.AreEqual("-", firstRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[wavePeriodColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(2.34.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(3.45.ToString(CultureInfo.CurrentCulture), secondRow.Cells[wavePeriodColumnIndex].FormattedValue);

            locations.ForEach(loc =>
                              {
                                  loc.Output = null;

                                  // Call
                                  loc.NotifyObservers();
                              });

            // Assert
            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual("-", firstRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[wavePeriodColumnIndex].FormattedValue);
            Assert.AreEqual("-", secondRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", secondRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", secondRow.Cells[wavePeriodColumnIndex].FormattedValue);
        }

        [Test]
        public void CalculateForSelectedButton_OneSelected_CallsCalculateSelectionNotChanged()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            ObservableList<DuneLocation> locations = (ObservableList<DuneLocation>) view.Data;
            var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
            var dataGridViewSource = dataGridView.DataSource;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite")
            };
            mocks.ReplayAll();

            locations.Attach(observer);

            view.AssessmentSection = assessmentSection;
            view.FailureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig())
            {
                view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                TestHelper.AssertLogMessages(() => buttonTester.Click(),
                                             messages =>
                                             {
                                                 var messageList = messages.ToList();

                                                 // Assert
                                                 Assert.AreEqual(5, messageList.Count);
                                                 StringAssert.StartsWith("Berekening van '1' gestart om: ", messageList[0]);
                                                 Assert.AreEqual("Duinafslag berekening voor locatie '1' is niet geconvergeerd.", messageList[1]);
                                                 StringAssert.StartsWith("Duinafslag berekening is uitgevoerd op de tijdelijke locatie", messageList[2]);
                                                 StringAssert.StartsWith("Berekening van '1' beëindigd om: ", messageList[3]);
                                                 Assert.AreEqual("Uitvoeren van '1' is gelukt.", messageList[4]);
                                             });
            }

            // Assert
            Assert.AreSame(dataGridViewSource, dataGridView.DataSource);
            Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
            Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);

            mocks.VerifyAll();
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            ShowFullyConfiguredDuneLocationsView();

            var dataGridView = (DataGridView)new ControlTester("dataGridView").TheObject;
            var rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var button = new ButtonTester("CalculateForSelectedButton", testForm);

            // Call
            TestDelegate test = () => button.Click();

            // Assert
            Assert.DoesNotThrow(test);
        }

        private DuneLocationsView ShowFullyConfiguredDuneLocationsView()
        {
            var view = ShowDuneLocationsView();
            view.Data = new ObservableList<DuneLocation>
            {
                new DuneLocation(1, "1", new Point2D(1.0, 1.0), new DuneLocation.ConstructionProperties
                                 {
                                     CoastalAreaId = 50,
                                     Offset = 320,
                                     D50 = 0.000837
                                 }),
                new DuneLocation(2, "2", new Point2D(2.0, 2.0), new DuneLocation.ConstructionProperties
                                 {
                                     CoastalAreaId = 60,
                                     Offset = 230,
                                     D50 = 0.000123
                                 })
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                                                    {
                                                        WaterLevel = 1.23,
                                                        WaveHeight = 2.34,
                                                        WavePeriod = 3.45
                                                    })
                }
            };

            return view;
        }

        private DuneLocationsView ShowDuneLocationsView()
        {
            var view = new DuneLocationsView();

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}