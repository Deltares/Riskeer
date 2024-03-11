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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
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
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.DuneErosion.Forms.Views;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationCalculationsViewBaseTest
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
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
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
            void Call() => new DuneLocationCalculationsViewBase(null,
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
            void Call() => new DuneLocationCalculationsViewBase(new ObservableList<DuneLocationCalculation>(),
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
            void Call() => new DuneLocationCalculationsViewBase(new ObservableList<DuneLocationCalculation>(),
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
            void Call() => new DuneLocationCalculationsViewBase(new ObservableList<DuneLocationCalculation>(),
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
            void Call() => new DuneLocationCalculationsViewBase(new ObservableList<DuneLocationCalculation>(),
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
            using (var view = new DuneLocationCalculationsViewBase(new ObservableList<DuneLocationCalculation>(),
                                                                   failureMechanism,
                                                                   assessmentSection,
                                                                   () => 0.01,
                                                                   () => "1/100"))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);
                Assert.AreSame(failureMechanism, view.FailureMechanism);
                Assert.AreSame(assessmentSection, view.AssessmentSection);

                Assert.AreEqual(new Size(526, 85), view.AutoScrollMinSize);
            }
        }

        [Test]
        public void Constructor_CalculateAllButtonCorrectlyInitialized()
        {
            // Setup & Call
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView())
            {
                // Assert
                var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
                Assert.IsFalse(button.Enabled);
            }
        }

        [Test]
        public void Constructor_DataGridViewCorrectlyInitialized()
        {
            // Setup & Call
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView())
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
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenSelectingCellInRow_ThenSelectionChangedFired()
        {
            // Given
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView())
            {
                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];

                // When
                dataGridView.CurrentCell = dataGridView.Rows[1].Cells[calculateColumnIndex];
                EventHelper.RaiseEvent(dataGridView, "CellClick", new DataGridViewCellEventArgs(0, 0));

                // Then
                Assert.AreEqual(1, selectionChangedCount);
            }
        }

        [Test]
        public void Selection_WithoutCalculations_ReturnsNull()
        {
            // Call
            using (DuneLocationCalculationsView view = ShowDuneLocationCalculationsView())
            {
                // Assert
                Assert.IsNull(view.Selection);
            }
        }

        [Test]
        public void SelectAllButton_SelectAllButtonClicked_AllCalculationsSelected()
        {
            // Setup
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView())
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                DataGridViewRowCollection rows = dataGridView.Rows;
                var button = new ButtonTester("SelectAllButton", testForm);

                // Precondition
                Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);

                // Call
                button.Click();

                // Assert
                Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
                Assert.IsTrue((bool) rows[1].Cells[calculateColumnIndex].Value);
            }
        }

        [Test]
        public void DeselectAllButton_AllCalculationsSelectedDeselectAllButtonClicked_AllCalculationsNotSelected()
        {
            // Setup
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView())
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];
                var button = new ButtonTester("DeselectAllButton", testForm);

                DataGridViewRowCollection rows = dataGridView.Rows;
                foreach (DataGridViewRow row in rows)
                {
                    row.Cells[calculateColumnIndex].Value = true;
                }

                // Precondition
                Assert.IsTrue((bool) rows[0].Cells[calculateColumnIndex].Value);
                Assert.IsTrue((bool) rows[1].Cells[calculateColumnIndex].Value);

                // Call
                button.Click();

                // Assert
                Assert.IsFalse((bool) rows[0].Cells[calculateColumnIndex].Value);
                Assert.IsFalse((bool) rows[1].Cells[calculateColumnIndex].Value);
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenNoRowsSelected_ThenCalculateForSelectedButtonDisabledAndErrorMessageProvided()
        {
            // Given & When
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView())
            {
                // Then
                var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
                Assert.IsFalse(button.Enabled);
                var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
                Assert.AreEqual("Er zijn geen berekeningen geselecteerd.", errorProvider.GetError(button));
            }
        }

        [Test]
        public void GivenFullyConfiguredView_WhenRowsSelected_ThenCalculateForSelectedButtonEnabledAndNoErrorMessageProvided()
        {
            // Given
            using (DuneLocationCalculationsView view = ShowFullyConfiguredDuneLocationCalculationsView())
            {
                var dataGridView = (DataGridView) view.Controls.Find("dataGridView", true)[0];

                // When
                dataGridView.Rows[0].Cells[calculateColumnIndex].Value = true;

                // Then
                var button = (Button) view.Controls.Find("CalculateForSelectedButton", true)[0];
                Assert.IsTrue(button.Enabled);
                var errorProvider = TypeUtils.GetField<ErrorProvider>(view, "CalculateForSelectedButtonErrorProvider");
                Assert.AreEqual("", errorProvider.GetError(button));
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

        private DuneLocationCalculationsView ShowFullyConfiguredDuneLocationCalculationsView()
        {
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

            var failureMechanism = new DuneErosionFailureMechanism();

            return ShowDuneLocationCalculationsView(GenerateDuneLocationCalculations(hydraulicBoundaryLocation),
                                                    failureMechanism,
                                                    assessmentSection);
        }

        private DuneLocationCalculationsView ShowFullyConfiguredDuneLocationCalculationsView(IAssessmentSection assessmentSection,
                                                                                             HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            var failureMechanism = new DuneErosionFailureMechanism();

            return ShowDuneLocationCalculationsView(GenerateDuneLocationCalculations(hydraulicBoundaryLocation),
                                                    failureMechanism,
                                                    assessmentSection);
        }

        private DuneLocationCalculationsView ShowDuneLocationCalculationsView()
        {
            return ShowDuneLocationCalculationsView(new ObservableList<DuneLocationCalculation>(),
                                                    new DuneErosionFailureMechanism(),
                                                    new AssessmentSectionStub());
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