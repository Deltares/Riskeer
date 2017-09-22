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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.Views;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationsViewTest
    {
        private const int locationCalculateColumnIndex = 0;
        private const int waterLevelColumnIndex = 6;
        private const int waveHeightColumnIndex = 7;
        private const int wavePeriodColumnIndex = 8;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private Form testForm;
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            testForm = new Form();
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            testForm.Dispose();
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var view = new DuneLocationsView())
            {
                // Assert
                Assert.IsInstanceOf<DuneLocationsViewBase>(view);
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            DuneLocationsView view = ShowDuneLocationsView();

            // Assert
            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];

            var expectedHeaderNames = new[]
            {
                "Berekenen",
                "Naam",
                "ID",
                "Coördinaten [m]",
                "Kustvaknummer",
                "Metrering [dam]",
                "Rekenwaarde waterstand [m+NAP]",
                "Rekenwaarde Hs [m]",
                "Rekenwaarde Tp [s]",
                "Rekenwaarde d50 [m]"
            };
            DataGridViewTestHelper.AssertExpectedHeaders(expectedHeaderNames, dataGridView);
            var expectedColumnTypes = new[]
            {
                typeof(DataGridViewCheckBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn),
                typeof(DataGridViewTextBoxColumn)
            };
            DataGridViewTestHelper.AssertColumnTypes(expectedColumnTypes, dataGridView);

            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.IsFalse(button.Enabled);
        }

        [Test]
        public void Data_SetDuneLocations_GetNewlySetData()
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
            DuneLocationsView view = ShowDuneLocationsView();

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
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();

            // Assert
            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);

            var expectedRow0Values = new object[]
            {
                false,
                "1",
                "1",
                new Point2D(1, 1).ToString(),
                "50",
                "320",
                "-",
                "-",
                "-",
                0.000837.ToString(CultureInfo.CurrentCulture)
            };
            DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow0Values, rows[0]);

            var expectedRow1Values = new object[]
            {
                false,
                "2",
                "2",
                new Point2D(2, 2).ToString(),
                "60",
                "230",
                1.23.ToString(CultureInfo.CurrentCulture),
                2.34.ToString(CultureInfo.CurrentCulture),
                3.45.ToString(CultureInfo.CurrentCulture),
                0.000123.ToString(CultureInfo.CurrentCulture)
            };
            DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow1Values, rows[1]);
        }

        [Test]
        public void GivenDuneLocationsView_WhenSettingAssessmentSectionAndDisposingView_ThenCorrectlyAttachAndDetachAssessmentSection()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            assessmentSection.Expect(a => a.Attach(Arg<IObserver>.Is.NotNull)).IgnoreArguments();
            assessmentSection.Expect(a => a.Detach(Arg<IObserver>.Is.NotNull)).IgnoreArguments();
            mocks.ReplayAll();

            // Call
            view.AssessmentSection = assessmentSection;

            // Assert
            // Assertions based on mock expectancies
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
        public void Selection_WithSelectedLocation_ReturnsSelectedLocationWrappedInContext()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRow selectedLocationRow = dataGridView.Rows[0];

            // Call
            selectedLocationRow.Cells[0].Value = true;

            // Assert
            var selection = view.Selection as DuneLocationContext;
            var dataBoundItem = selectedLocationRow.DataBoundItem as DuneLocationRow;

            Assert.NotNull(selection);
            Assert.NotNull(dataBoundItem);
            Assert.AreSame(dataBoundItem.CalculatableObject, selection.DuneLocation);
        }

        [Test]
        public void GivenFullyConfiguredDuneLocationsView_WhenDuneLocationsUpdatedAndNotified_ThenDataGridViewRowCorrectlyUpdated()
        {
            // Given
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            var locations = (ObservableList<DuneLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            object originalDataSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;
            Assert.AreEqual(2, rows.Count);

            // When
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
            locations.NotifyObservers();

            // Then
            Assert.AreNotSame(originalDataSource, dataGridView.DataSource);

            var expectedRowValues = new object[]
            {
                false,
                "10",
                "10",
                new Point2D(10, 10).ToString(),
                "3",
                "80",
                3.21.ToString(CultureInfo.CurrentCulture),
                4.32.ToString(CultureInfo.CurrentCulture),
                5.43.ToString(CultureInfo.CurrentCulture),
                0.000321.ToString(CultureInfo.CurrentCulture)
            };
            DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRowValues, rows[0]);
        }

        [Test]
        public void GivenFullyConfiguredDuneLocationsView_WhenEachDuneLocationOutputClearedAndNotified_ThenDataGridViewRowsRefreshedWithNewValues()
        {
            // Given
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();
            var locations = (ObservableList<DuneLocation>) view.Data;

            // Precondition
            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRowCollection rows = dataGridView.Rows;
            Assert.AreEqual(2, rows.Count);
            DataGridViewRow firstRow = rows[0];
            DataGridViewRow secondRow = rows[1];

            Assert.AreEqual("-", firstRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[wavePeriodColumnIndex].FormattedValue);
            Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual(2.34.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual(3.45.ToString(CultureInfo.CurrentCulture), secondRow.Cells[wavePeriodColumnIndex].FormattedValue);

            // When
            locations.ForEach(loc =>
            {
                loc.Output = null;
                loc.NotifyObservers();
            });

            // Then
            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual("-", firstRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", firstRow.Cells[wavePeriodColumnIndex].FormattedValue);
            Assert.AreEqual("-", secondRow.Cells[waterLevelColumnIndex].FormattedValue);
            Assert.AreEqual("-", secondRow.Cells[waveHeightColumnIndex].FormattedValue);
            Assert.AreEqual("-", secondRow.Cells[wavePeriodColumnIndex].FormattedValue);
        }

        [Test]
        public void CalculateForSelectedButton_OneLocationSelected_CalculateForSelectedLocationAndKeepOriginalSelection()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();

            var locations = (ObservableList<DuneLocation>) view.Data;

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            object originalDataSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(ass => ass.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite")
            };
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath))
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mocks.ReplayAll();

            locations.Attach(observer);

            view.AssessmentSection = assessmentSection;
            view.FailureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

            using (var viewParent = new Form())
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                // Call
                Action action = () => buttonTester.Click();

                // Assert
                TestHelper.AssertLogMessages(action,
                                             messages =>
                                             {
                                                 List<string> messageList = messages.ToList();

                                                 // Assert
                                                 Assert.AreEqual(8, messageList.Count);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie '1' is gestart.", messageList[0]);
                                                 CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                                                 CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                                                 CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekening voor locatie '1' is niet geconvergeerd.", messageList[4]);
                                                 StringAssert.StartsWith("Hydraulische randvoorwaarden berekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                                                 CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);
                                                 Assert.AreEqual("Hydraulische randvoorwaarden berekenen voor locatie '1' is gelukt.", messageList[7]);
                                             });

                Assert.AreSame(originalDataSource, dataGridView.DataSource);

                Assert.IsTrue((bool) rows[0].Cells[locationCalculateColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[locationCalculateColumnIndex].Value);
            }
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
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
        public void GivenDuneLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                              bool contributionNotZero,
                                                                                                                                                              string expectedErrorMessage)
        {
            // Given
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView();

            // When
            if (rowSelected)
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            var failureMechanism = new DuneErosionFailureMechanism();
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

        private DuneLocationsView ShowFullyConfiguredDuneLocationsView()
        {
            DuneLocationsView view = ShowDuneLocationsView();
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