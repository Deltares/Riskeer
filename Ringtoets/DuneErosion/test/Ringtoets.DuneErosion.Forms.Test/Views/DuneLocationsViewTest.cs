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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util;
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
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
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
        public void Constructor_LocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationsView(null,
                                                            dl => new DuneLocationCalculation(),
                                                            new DuneErosionFailureMechanism(),
                                                            assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void Constructor_GetCalculationFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationsView(new ObservableList<DuneLocation>(),
                                                            null,
                                                            new DuneErosionFailureMechanism(),
                                                            assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("getCalculationFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new DuneLocationsView(new ObservableList<DuneLocation>(),
                                                            dl => new DuneLocationCalculation(),
                                                            null,
                                                            assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DuneLocationsView(new ObservableList<DuneLocation>(),
                                                            dl => new DuneLocationCalculation(),
                                                            new DuneErosionFailureMechanism(),
                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            using (var view = new DuneLocationsView(new ObservableList<DuneLocation>(),
                                                    dl => new DuneLocationCalculation(),
                                                    failureMechanism,
                                                    assessmentSection))
            {
                // Assert
                Assert.IsInstanceOf<DuneLocationsViewBase>(view);
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
            DuneLocationsView view = ShowDuneLocationsView(new DuneErosionFailureMechanism(), assessmentSection);

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
        public void DuneLocationsView_DataSet_DataGridViewCorrectlyInitialized()
        {
            // Setup 
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

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
        public void Selection_WithoutLocations_ReturnsNull()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            using (var view = new DuneLocationsView(new ObservableList<DuneLocation>(),
                                                    dl => new DuneLocationCalculation(),
                                                    new DuneErosionFailureMechanism(),
                                                    assessmentSection))
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void Selection_WithSelectedLocation_ReturnsSelectedLocationWrappedInContext()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRow selectedLocationRow = dataGridView.Rows[0];

            // Call
            selectedLocationRow.Cells[0].Value = true;

            // Assert
            var selection = view.Selection as DuneLocation;
            var dataBoundItem = selectedLocationRow.DataBoundItem as DuneLocationRow;

            Assert.NotNull(selection);
            Assert.NotNull(dataBoundItem);
            Assert.AreSame(dataBoundItem.CalculatableObject, selection);
        }

        [Test]
        public void GivenFullyConfiguredDuneLocationsView_WhenDuneLocationsUpdatedAndNotified_ThenDataGridViewRowCorrectlyUpdated()
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);
            ObservableList<DuneLocation> locations = view.FailureMechanism.DuneLocations;

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
                Calculation =
                {
                    Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                    {
                        WaterLevel = 3.21,
                        WaveHeight = 4.32,
                        WavePeriod = 5.43
                    })
                }
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);
            ObservableList<DuneLocation> locations = view.FailureMechanism.DuneLocations;

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
                loc.Calculation.Output = null;
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite")
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var locationsObserver = mocks.StrictMock<IObserver>();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty))
                             .Return(new TestDunesBoundaryConditionsCalculator());
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);
            ObservableList<DuneLocation> locations = view.FailureMechanism.DuneLocations;

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            object originalDataSource = dataGridView.DataSource;
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

            locations.Attach(locationsObserver);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

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
        [TestCase(false, true, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, false, message)")]
        [TestCase(true, true, "De bijdrage van dit toetsspoor is nul.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, false, message)")]
        [TestCase(false, false, "Er zijn geen berekeningen geselecteerd.", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(false, true, message)")]
        [TestCase(true, false, "", TestName = "CalculateButton_RowSelectionContributionSet_SyncedAccordingly(true, true, message)")]
        public void GivenDuneLocationsView_WhenSpecificCombinationOfRowSelectionAndFailureMechanismContributionSet_ThenButtonAndErrorMessageSyncedAccordingly(bool rowSelected,
                                                                                                                                                              bool contributionZero,
                                                                                                                                                              string expectedErrorMessage)
        {
            // Given
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

            // When
            if (rowSelected)
            {
                var dataGridView = (DataGridView) new ControlTester("dataGridView").TheObject;
                DataGridViewRowCollection rows = dataGridView.Rows;
                rows[0].Cells[locationCalculateColumnIndex].Value = true;
            }

            if (contributionZero)
            {
                view.FailureMechanism.Contribution = 0;
                view.FailureMechanism.NotifyObservers();
            }

            // Then
            var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
            Assert.AreEqual(rowSelected && !contributionZero, button.Enabled);
            var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
            Assert.AreEqual(expectedErrorMessage, errorProvider.GetError(button));
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_CreateDunesBoundaryConditionsCalculatorCalledAsExpected()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite")
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty))
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

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
                double expectedProbability = view.FailureMechanism.GetMechanismSpecificNorm(assessmentSection.FailureMechanismContribution.Norm);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), dunesBoundaryConditionsCalculationInput.Beta);
            }
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorTrue_CreateDunesBoundaryConditionsCalculatorCalledAsExpected()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite"),
                CanUsePreprocessor = true,
                UsePreprocessor = true,
                PreprocessorDirectory = validPreprocessorDirectory
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, validPreprocessorDirectory))
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

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
                double expectedProbability = view.FailureMechanism.GetMechanismSpecificNorm(assessmentSection.FailureMechanismContribution.Norm);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), dunesBoundaryConditionsCalculationInput.Beta);
            }
        }

        [Test]
        public void CalculateForSelectedButton_HydraulicBoundaryDatabaseWithUsePreprocessorFalse_CreateDunesBoundaryConditionsCalculatorCalledAsExpected()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.Id).Return("1");
            assessmentSection.Stub(a => a.FailureMechanismContribution)
                             .Return(FailureMechanismContributionTestFactory.CreateFailureMechanismContribution());

            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(testDataPath, "complete.sqlite"),
                CanUsePreprocessor = true,
                UsePreprocessor = false,
                PreprocessorDirectory = "InvalidPreprocessorDirectory"
            });
            assessmentSection.Stub(a => a.Attach(null)).IgnoreArguments();
            assessmentSection.Stub(a => a.Detach(null)).IgnoreArguments();

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            var dunesBoundaryConditionsCalculator = new TestDunesBoundaryConditionsCalculator();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(testDataPath, string.Empty))
                             .Return(dunesBoundaryConditionsCalculator);
            mocks.ReplayAll();

            DuneLocationsView view = ShowFullyConfiguredDuneLocationsView(assessmentSection);

            var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
            DataGridViewRowCollection rows = dataGridView.Rows;
            rows[0].Cells[locationCalculateColumnIndex].Value = true;

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
                double expectedProbability = view.FailureMechanism.GetMechanismSpecificNorm(assessmentSection.FailureMechanismContribution.Norm);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(expectedProbability), dunesBoundaryConditionsCalculationInput.Beta);
            }
        }

        private DuneLocationsView ShowFullyConfiguredDuneLocationsView(IAssessmentSection assessmentSection)
        {
            var locations = new ObservableList<DuneLocation>
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
                    Calculation =
                    {
                        Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                        {
                            WaterLevel = 1.23,
                            WaveHeight = 2.34,
                            WavePeriod = 3.45
                        })
                    }
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 10
            };
            failureMechanism.DuneLocations.AddRange(locations);

            DuneLocationsView view = ShowDuneLocationsView(failureMechanism, assessmentSection);
            return view;
        }

        private DuneLocationsView ShowDuneLocationsView(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var view = new DuneLocationsView(failureMechanism.DuneLocations,
                                             dl => dl.Calculation,
                                             failureMechanism,
                                             assessmentSection);

            testForm.Controls.Add(view);
            testForm.Show();

            return view;
        }
    }
}