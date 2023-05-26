﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Common.Util.Reflection;
using Core.Gui.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.DuneErosion.Forms.Views;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationCalculationsViewTest
    {
        private const int calculateColumnIndex = 0;
        private const int waterLevelColumnIndex = 6;
        private const int waveHeightColumnIndex = 7;
        private const int wavePeriodColumnIndex = 8;
        private const int meanTidalAmplitudeColumnIndex = 9;
        private const int waveDirectionalSpreadColumnIndex = 10;
        private const int tideSurgePhaseDifferenceColumnIndex = 11;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "complete.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

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
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationCalculationsView(null,
                                                            new DuneErosionFailureMechanism(),
                                                            assessmentSection,
                                                            () => 0.01,
                                                            () => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                            null,
                                                            assessmentSection,
                                                            () => 0.01,
                                                            () => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                            new DuneErosionFailureMechanism(),
                                                            null,
                                                            () => 0.01,
                                                            () => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_GetTargetProbabilityFunc_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                            new DuneErosionFailureMechanism(),
                                                            assessmentSection,
                                                            null,
                                                            () => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getTargetProbabilityFunc", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_GetCalculationIdentifierFuncNull_ThrowsArgumentException(string calculationIdentifier)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                            new DuneErosionFailureMechanism(),
                                                            assessmentSection,
                                                            () => 0.01,
                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getCalculationIdentifierFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                // Assert
                Assert.IsInstanceOf<DuneLocationCalculationsViewBase>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);
            }
        }

        [Test]
        public void OnLoad_DataGridViewCorrectlyInitialized()
        {
            // Setup 
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                                                        new DuneErosionFailureMechanism(),
                                                                                        assessmentSection))
            {
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
                    "Rekenwaarde gemiddelde getijamplitude [m]",
                    "Rekenwaarde golfrichtingspreiding [-]",
                    "Rekenwaarde faseverschuiving tussen getij en opzet [uur]"
                };
                DataGridViewTestHelper.AssertExpectedHeaders(expectedHeaderNames, dataGridView);
                Type[] expectedColumnTypes =
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
                    typeof(DataGridViewTextBoxColumn),
                    typeof(DataGridViewTextBoxColumn),
                    typeof(DataGridViewTextBoxColumn)
                };
                DataGridViewTestHelper.AssertColumnTypes(expectedColumnTypes, dataGridView);

                var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void DuneLocationCalculationsView_DataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(hydraulicBoundaryData);
            mocks.ReplayAll();

            // Call
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection,
                                                                                                       hydraulicBoundaryLocation))
            {
                // Assert
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);

                var expectedRow0Values = new object[]
                {
                    false,
                    "1",
                    "0",
                    new Point2D(0, 0).ToString(),
                    "50",
                    "320",
                    "-",
                    "-",
                    "-",
                    "-",
                    "-",
                    "-"
                };
                DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow0Values, rows[0]);

                var expectedRow1Values = new object[]
                {
                    false,
                    "2",
                    "0",
                    new Point2D(0, 0).ToString(),
                    "60",
                    "230",
                    1.23.ToString(CultureInfo.CurrentCulture),
                    2.34.ToString(CultureInfo.CurrentCulture),
                    3.45.ToString(CultureInfo.CurrentCulture),
                    4.35.ToString(CultureInfo.CurrentCulture),
                    5.54.ToString(CultureInfo.CurrentCulture),
                    6.45.ToString(CultureInfo.CurrentCulture)
                };
                DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRow1Values, rows[1]);
            }
        }

        [Test]
        public void Selection_WithoutCalculations_ReturnsNull()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (var view = new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                               new DuneErosionFailureMechanism(),
                                                               assessmentSection,
                                                               () => 0.01,
                                                               () => "1/100"))
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithSelectedCalculation_ReturnsSelectedCalculation()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection,
                                                                                                       new TestHydraulicBoundaryLocation()))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRow selectedCalculationRow = dataGridView.Rows[0];

                // Call
                selectedCalculationRow.Cells[0].Value = true;

                // Assert
                var selection = view.Selection as DuneLocationCalculation;
                var dataBoundItem = selectedCalculationRow.DataBoundItem as DuneLocationCalculationRow;

                Assert.NotNull(selection);
                Assert.NotNull(dataBoundItem);
                Assert.AreSame(dataBoundItem.CalculatableObject, selection);
            }
        }

        [Test]
        public void GivenFullyConfiguredDuneLocationCalculationsView_WhenDuneLocationCalculationsUpdatedAndNotified_ThenDataGridCorrectlyUpdated()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculations = new ObservableList<DuneLocationCalculation>();
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        new DuneErosionFailureMechanism(),
                                                                                        assessmentSection))
            {
                // Precondition
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                object originalDataSource = dataGridView.DataSource;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(0, rows.Count);

                // When
                var duneLocation = new DuneLocation(
                    "10", new HydraulicBoundaryLocation(10, string.Empty, 10.0, 10.0),
                    new DuneLocation.ConstructionProperties
                    {
                        CoastalAreaId = 3,
                        Offset = 80
                    });
                var duneLocationCalculation = new DuneLocationCalculation(duneLocation)
                {
                    Output = new DuneLocationCalculationOutput(
                        CalculationConvergence.CalculatedConverged,
                        new DuneLocationCalculationOutput.ConstructionProperties
                        {
                            WaterLevel = 3.21,
                            WaveHeight = 4.32,
                            WavePeriod = 5.43,
                            MeanTidalAmplitude = 4.35,
                            WaveDirectionalSpread = 5.54,
                            TideSurgePhaseDifference = 6.45
                        })
                };
                calculations.Add(duneLocationCalculation);
                calculations.NotifyObservers();

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
                    4.35.ToString(CultureInfo.CurrentCulture),
                    5.54.ToString(CultureInfo.CurrentCulture),
                    6.45.ToString(CultureInfo.CurrentCulture)
                };
                DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRowValues, rows[0]);
            }
        }

        [Test]
        public void GivenFullyConfiguredDuneLocationCalculationsView_WhenEachDuneLocationCalculationOutputClearedAndNotified_ThenDataGridViewRowsRefreshedWithNewValues()
        {
            // Given
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            IObservableEnumerable<DuneLocationCalculation> calculations = GenerateDuneLocationCalculations(hydraulicBoundaryLocation);
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        new DuneErosionFailureMechanism(),
                                                                                        assessmentSection))
            {
                // Precondition
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(2, rows.Count);
                DataGridViewRow firstRow = rows[0];
                DataGridViewRow secondRow = rows[1];

                Assert.AreEqual("-", firstRow.Cells[waterLevelColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[wavePeriodColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[meanTidalAmplitudeColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[waveDirectionalSpreadColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[tideSurgePhaseDifferenceColumnIndex].FormattedValue);
                
                Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waterLevelColumnIndex].FormattedValue);
                Assert.AreEqual(2.34.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual(3.45.ToString(CultureInfo.CurrentCulture), secondRow.Cells[wavePeriodColumnIndex].FormattedValue);
                Assert.AreEqual(4.35.ToString(CultureInfo.CurrentCulture), secondRow.Cells[meanTidalAmplitudeColumnIndex].FormattedValue);
                Assert.AreEqual(5.54.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waveDirectionalSpreadColumnIndex].FormattedValue);
                Assert.AreEqual(6.45.ToString(CultureInfo.CurrentCulture), secondRow.Cells[tideSurgePhaseDifferenceColumnIndex].FormattedValue);
                // When
                calculations.ForEachElementDo(calculation =>
                {
                    calculation.Output = null;
                    calculation.NotifyObservers();
                });

                // Then
                Assert.AreEqual(2, rows.Count);
                Assert.AreEqual("-", firstRow.Cells[waterLevelColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[wavePeriodColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[meanTidalAmplitudeColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[waveDirectionalSpreadColumnIndex].FormattedValue);
                Assert.AreEqual("-", firstRow.Cells[tideSurgePhaseDifferenceColumnIndex].FormattedValue);
                
                Assert.AreEqual("-", secondRow.Cells[waterLevelColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[wavePeriodColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[meanTidalAmplitudeColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[waveDirectionalSpreadColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[tideSurgePhaseDifferenceColumnIndex].FormattedValue);
            }
        }

        [Test]
        public void CalculateForSelectedButton_OneCalculationSelected_CalculateForSelectedCalculationAndKeepOriginalSelection()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(hydraulicBoundaryData);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculationsObserver = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mocks.ReplayAll();

            IObservableEnumerable<DuneLocationCalculation> calculations = GenerateDuneLocationCalculations(hydraulicBoundaryLocation);
            var failureMechanism = new DuneErosionFailureMechanism();

            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        failureMechanism,
                                                                                        assessmentSection))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                object originalDataSource = dataGridView.DataSource;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                calculations.Attach(calculationsObserver);

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new TestViewParentForm())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    buttonTester.Click();

                    // Assert
                    Assert.AreSame(originalDataSource, dataGridView.DataSource);

                    Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
                    Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);
                }
            }
        }

        [Test]
        public void CalculateForSelectedButton_OneCalculationSelected_CalculateForSelectedCalculationAndLogsMessages()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(hydraulicBoundaryData);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculationsObserver = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mocks.ReplayAll();

            IObservableEnumerable<DuneLocationCalculation> calculations = GenerateDuneLocationCalculations(hydraulicBoundaryLocation);
            var failureMechanism = new DuneErosionFailureMechanism();

            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        failureMechanism,
                                                                                        assessmentSection))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                calculations.Attach(calculationsObserver);

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new TestViewParentForm())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    void Call() => buttonTester.Click();

                    // Assert
                    string expectedDuneLocationName = calculations.ElementAt(0).DuneLocation.Name;

                    TestHelper.AssertLogMessages(Call,
                                                 messages =>
                                                 {
                                                     List<string> messageList = messages.ToList();

                                                     // Assert
                                                     Assert.AreEqual(8, messageList.Count);
                                                     Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{expectedDuneLocationName}' (1/100) is gestart.", messageList[0]);
                                                     CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                                                     CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                                                     CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                                                     Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{expectedDuneLocationName}' (1/100) is niet geconvergeerd.", messageList[4]);
                                                     StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                                                     CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);
                                                     Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{expectedDuneLocationName}' (1/100) is gelukt.", messageList[7]);
                                                 });
                }
            }
        }

        [Test]
        public void CalculateForSelectedButton_OneSelectedButCalculationGuiServiceNotSet_DoesNotThrowException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection,
                                                                                                       new TestHydraulicBoundaryLocation()))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                var button = new ButtonTester("CalculateForSelectedButton", testForm);

                // Call
                void Call() => button.Click();

                // Assert
                Assert.DoesNotThrow(Call);
            }
        }

        [Test]
        [TestCase(false, "Er zijn geen berekeningen geselecteerd.")]
        [TestCase(true, "")]
        public void GivenDuneLocationCalculationsView_WhenSpecificCombinationOfRowSelectionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected, string expectedErrorMessage)
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection,
                                                                                                       new TestHydraulicBoundaryLocation()))
            {
                // When
                if (rowSelected)
                {
                    var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                    DataGridViewRowCollection rows = dataGridView.Rows;
                    rows[0].Cells[calculateColumnIndex].Value = true;
                }

                // Then
                var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
                Assert.AreEqual(rowSelected, button.Enabled);
                var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
                Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CreateDunesBoundaryConditionsCalculatorCalledAsExpected(bool usePreprocessorClosure)
        {
            // Setup
            const double targetProbability = 0.01;

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            IObservableEnumerable<DuneLocationCalculation> duneLocationCalculations = GenerateDuneLocationCalculations(hydraulicBoundaryLocation);

            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = validHlcdFilePath
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        FilePath = validHrdFilePath,
                        UsePreprocessorClosure = usePreprocessorClosure,
                        Version = validHrdFileVersion,
                        Locations =
                        {
                            hydraulicBoundaryLocation
                        }
                    }
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryData).Return(hydraulicBoundaryData);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryData,
                                                                                                hydraulicBoundaryLocation),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneLocationCalculationsView(duneLocationCalculations,
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => targetProbability,
                                                               () => "1/100"))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new TestViewParentForm())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    buttonTester.Click();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(0, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        private DuneLocationCalculationsView ShowFullyConfiguredDuneLocationCalculationsView(IAssessmentSection assessmentSection,
                                                                                             HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var failureMechanism = new DuneErosionFailureMechanism();

            return ShowDuneLocationCalculationsView(GenerateDuneLocationCalculations(hydraulicBoundaryLocation),
                                                    failureMechanism,
                                                    assessmentSection);
        }

        private DuneLocationCalculationsView ShowDuneLocationCalculationsView(IObservableEnumerable<DuneLocationCalculation> calculations,
                                                                              DuneErosionFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
        {
            var view = new DuneLocationCalculationsView(calculations,
                                                        failureMechanism,
                                                        assessmentSection,
                                                        () => 0.01,
                                                        () => "1/100");

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static IObservableEnumerable<DuneLocationCalculation> GenerateDuneLocationCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new DuneLocation("1", hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 50,
                    Offset = 320
                })),
                new DuneLocationCalculation(new DuneLocation("2", hydraulicBoundaryLocation, new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 60,
                    Offset = 230
                }))
                {
                    Output = new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationCalculationOutput.ConstructionProperties
                    {
                        WaterLevel = 1.23,
                        WaveHeight = 2.34,
                        WavePeriod = 3.45,
                        MeanTidalAmplitude = 4.35,
                        WaveDirectionalSpread = 5.54,
                        TideSurgePhaseDifference = 6.45
                    })
                }
            };
        }
    }
}