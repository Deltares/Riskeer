// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util;
using Core.Common.Util.Extensions;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.Views;
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

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(HydraulicBoundaryDatabase));
        private static readonly string hydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "complete.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

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
            TestDelegate call = () => new DuneLocationCalculationsView(null,
                                                                       new DuneErosionFailureMechanism(),
                                                                       assessmentSection,
                                                                       () => 0.01,
                                                                       "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                                       null,
                                                                       assessmentSection,
                                                                       () => 0.01,
                                                                       "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                                       new DuneErosionFailureMechanism(),
                                                                       null,
                                                                       () => 0.01,
                                                                       "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_GetNormFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                                       new DuneErosionFailureMechanism(),
                                                                       assessmentSection,
                                                                       null,
                                                                       "A");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getNormFunc", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_CategoryBoundaryNameInvalid_ThrowsArgumentException(string categoryBoundaryName)
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                                       new DuneErosionFailureMechanism(),
                                                                       assessmentSection,
                                                                       () => 0.01,
                                                                       categoryBoundaryName);

            // Assert
            const string expectedMessage = "'categoryBoundaryName' must have a value.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
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
                                                               "A"))
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
                                                                                        assessmentSection,
                                                                                        "A"))
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
                    "Rekenwaarde d50 [m]"
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection))
            {
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
                                                               "A"))
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

            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection))
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
                                                                                        assessmentSection,
                                                                                        "A"))
            {
                // Precondition
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                object originalDataSource = dataGridView.DataSource;
                DataGridViewRowCollection rows = dataGridView.Rows;
                Assert.AreEqual(0, rows.Count);

                // When
                var duneLocation = new DuneLocation(10, "10", new Point2D(10.0, 10.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 3,
                    Offset = 80,
                    D50 = 0.000321
                });
                var duneLocationCalculation = new DuneLocationCalculation(duneLocation)
                {
                    Output = new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationCalculationOutput.ConstructionProperties
                    {
                        WaterLevel = 3.21,
                        WaveHeight = 4.32,
                        WavePeriod = 5.43
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
                    0.000321.ToString(CultureInfo.CurrentCulture)
                };
                DataGridViewTestHelper.AssertExpectedRowFormattedValues(expectedRowValues, rows[0]);
            }
        }

        [Test]
        public void GivenFullyConfiguredDuneLocationCalculationsView_WhenEachDuneLocationCalculationOutputClearedAndNotified_ThenDataGridViewRowsRefreshedWithNewValues()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            IObservableEnumerable<DuneLocationCalculation> calculations = GenerateDuneLocationCalculations();
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        new DuneErosionFailureMechanism(),
                                                                                        assessmentSection,
                                                                                        "A"))
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
                Assert.AreEqual(1.23.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waterLevelColumnIndex].FormattedValue);
                Assert.AreEqual(2.34.ToString(CultureInfo.CurrentCulture), secondRow.Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual(3.45.ToString(CultureInfo.CurrentCulture), secondRow.Cells[wavePeriodColumnIndex].FormattedValue);

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
                Assert.AreEqual("-", secondRow.Cells[waterLevelColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[waveHeightColumnIndex].FormattedValue);
                Assert.AreEqual("-", secondRow.Cells[wavePeriodColumnIndex].FormattedValue);
            }
        }

        [Test]
        public void CalculateForSelectedButton_OneCalculationSelected_CalculateForSelectedCalculationAndKeepOriginalSelection()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculationsObserver = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mocks.ReplayAll();

            IObservableEnumerable<DuneLocationCalculation> calculations = GenerateDuneLocationCalculations();
            var failureMechanism = new DuneErosionFailureMechanism();

            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        failureMechanism,
                                                                                        assessmentSection,
                                                                                        "A"))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                object originalDataSource = dataGridView.DataSource;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                calculations.Attach(calculationsObserver);

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new Form())
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
            const string categoryBoundaryName = "A";

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculationsObserver = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mocks.ReplayAll();

            IObservableEnumerable<DuneLocationCalculation> calculations = GenerateDuneLocationCalculations();
            var failureMechanism = new DuneErosionFailureMechanism();

            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView(calculations,
                                                                                        failureMechanism,
                                                                                        assessmentSection,
                                                                                        categoryBoundaryName))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                calculations.Attach(calculationsObserver);

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new Form())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    Action action = () => buttonTester.Click();

                    // Assert
                    string expectedDuneLocationName = calculations.ElementAt(0).DuneLocation.Name;

                    TestHelper.AssertLogMessages(action,
                                                 messages =>
                                                 {
                                                     List<string> messageList = messages.ToList();

                                                     // Assert
                                                     Assert.AreEqual(8, messageList.Count);
                                                     Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{expectedDuneLocationName}' (Categoriegrens {categoryBoundaryName}) is gestart.", messageList[0]);
                                                     CalculationServiceTestHelper.AssertValidationStartMessage(messageList[1]);
                                                     CalculationServiceTestHelper.AssertValidationEndMessage(messageList[2]);
                                                     CalculationServiceTestHelper.AssertCalculationStartMessage(messageList[3]);
                                                     Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{expectedDuneLocationName}' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", messageList[4]);
                                                     StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messageList[5]);
                                                     CalculationServiceTestHelper.AssertCalculationEndMessage(messageList[6]);
                                                     Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{expectedDuneLocationName}' (Categoriegrens {categoryBoundaryName}) is gelukt.", messageList[7]);
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

            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection))
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                var button = new ButtonTester("CalculateForSelectedButton", testForm);

                // Call
                TestDelegate test = () => button.Click();

                // Assert
                Assert.DoesNotThrow(test);
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

            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView(assessmentSection))
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
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CreateDunesBoundaryConditionsCalculatorCalledAsExpected()
        {
            // Setup
            const double norm = 0.1;
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneLocationCalculationsView(GenerateDuneLocationCalculations(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => norm,
                                                               "A"))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new Form())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    buttonTester.Click();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(1, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_CreateDunesBoundaryConditionsCalculatorCalledAsExpected()
        {
            // Setup
            const double norm = 0.1;
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = validPreprocessorDirectory
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneLocationCalculationsView(GenerateDuneLocationCalculations(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => norm,
                                                               "A"))
            {
                testForm.Controls.Add(view);
                testForm.Show();

                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new Form())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    buttonTester.Click();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(1, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_CreateDunesBoundaryConditionsCalculatorCalledAsExpected()
        {
            // Setup
            const double norm = 0.1;
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = hydraulicBoundaryDatabaseFilePath,
                CanUsePreprocessor = true,
                UsePreprocessor = false,
                PreprocessorDirectory = "InvalidPreprocessorDirectory"
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     HydraulicBoundaryCalculationSettingsFactory.CreateSettings(hydraulicBoundaryDatabase),
                                     (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            using (var view = new DuneLocationCalculationsView(GenerateDuneLocationCalculations(),
                                                               failureMechanism,
                                                               assessmentSection,
                                                               () => norm,
                                                               "A"))
            {
                testForm.Controls.Add(view);
                testForm.Show();
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[calculateColumnIndex].Value = true;

                var buttonTester = new ButtonTester("CalculateForSelectedButton", testForm);

                using (var viewParent = new Form())
                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                {
                    view.CalculationGuiService = new DuneLocationCalculationGuiService(viewParent);

                    // Call
                    buttonTester.Click();

                    // Assert
                    DunesBoundaryConditionsCalculationInput dunesBoundaryConditionsCalculationInput = dunesBoundaryConditionsCalculator.ReceivedInputs.First();

                    Assert.AreEqual(1, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
                    Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), dunesBoundaryConditionsCalculationInput.Beta);
                }
            }
        }

        private DuneLocationCalculationsView ShowFullyConfiguredDuneLocationCalculationsView(IAssessmentSection assessmentSection)
        {
            var failureMechanism = new DuneErosionFailureMechanism();

            return ShowDuneLocationCalculationsView(GenerateDuneLocationCalculations(),
                                                    failureMechanism,
                                                    assessmentSection,
                                                    "A");
        }

        private DuneLocationCalculationsView ShowDuneLocationCalculationsView(IObservableEnumerable<DuneLocationCalculation> calculations,
                                                                              DuneErosionFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection,
                                                                              string categoryBoundaryName)
        {
            var view = new DuneLocationCalculationsView(calculations,
                                                        failureMechanism,
                                                        assessmentSection,
                                                        () => 1.0 / 30,
                                                        categoryBoundaryName);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }

        private static IObservableEnumerable<DuneLocationCalculation> GenerateDuneLocationCalculations()
        {
            return new ObservableList<DuneLocationCalculation>
            {
                new DuneLocationCalculation(new DuneLocation(1, "1", new Point2D(1.0, 1.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 50,
                    Offset = 320,
                    D50 = 0.000837
                })),
                new DuneLocationCalculation(new DuneLocation(2, "2", new Point2D(2.0, 2.0), new DuneLocation.ConstructionProperties
                {
                    CoastalAreaId = 60,
                    Offset = 230,
                    D50 = 0.000123
                }))
                {
                    Output = new DuneLocationCalculationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationCalculationOutput.ConstructionProperties
                    {
                        WaterLevel = 1.23,
                        WaveHeight = 2.34,
                        WavePeriod = 3.45
                    })
                }
            };
        }
    }
}