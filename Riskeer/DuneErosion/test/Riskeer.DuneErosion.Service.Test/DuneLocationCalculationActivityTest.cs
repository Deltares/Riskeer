// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.Common.Service.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;

namespace Riskeer.DuneErosion.Service.Test
{
    [TestFixture]
    public class DuneLocationCalculationActivityTest
    {
        private const double validNorm = 0.005;

        private MockRepository mockRepository;
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "HLCD.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            const string locationName = "locationName";
            const string categoryBoundaryName = "A";

            // Call
            TestDelegate call = () => new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                                          null,
                                                                          1,
                                                                          categoryBoundaryName);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            // Call
            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                               CreateCalculationSettings(),
                                                               1.0 / 30000,
                                                               categoryBoundaryName);

            // Assert
            Assert.IsInstanceOf<CalculatableActivity>(activity);
            Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName})", activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var settings = new HydraulicBoundaryCalculationSettings(invalidFilePath,
                                                                    validHlcdFilePath,
                                                                    string.Empty);
            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                               settings,
                                                               0.5,
                                                               categoryBoundaryName);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName}) is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_InvalidPreprocessorDirectory_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var settings = new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                                    validHlcdFilePath,
                                                                    "NonExistingPreprocessorDirectory");
            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                               settings,
                                                               0.5,
                                                               categoryBoundaryName);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName}) is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_InvalidNorm_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                               CreateCalculationSettings(),
                                                               1.0,
                                                               categoryBoundaryName);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName}) is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_VariousValidInputs_PerformsCalculationWithCorrectInput(bool usePreprocessor)
        {
            // Setup
            const double norm = 1.0 / 30;
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                Converged = true
            };

            string preprocessorDirectory = usePreprocessor ? validPreprocessorDirectory : string.Empty;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                                               validHlcdFilePath,
                                                                               preprocessorDirectory);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mockRepository.ReplayAll();

            var duneLocation = new TestDuneLocation(locationName);

            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(duneLocation),
                                                               calculationSettings,
                                                               norm,
                                                               categoryBoundaryName);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                DunesBoundaryConditionsCalculationInput calculationInput = calculator.ReceivedInputs.Single();
                Assert.AreEqual(duneLocation.Id, calculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), calculationInput.Beta);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidInput_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            const double norm = 1.0 / 30;
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                Converged = true
            };

            HydraulicBoundaryCalculationSettings calculationSettings = CreateCalculationSettings();

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var duneLocation = new TestDuneLocation(locationName);

            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(duneLocation),
                                                               calculationSettings,
                                                               norm,
                                                               categoryBoundaryName);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(6, messages.Length);
                    Assert.AreEqual($"Hydraulische belastingen berekenen voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName}) is gestart.", messages[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                    StringAssert.StartsWith("Hydraulische belastingenberekening is uitgevoerd op de tijdelijke locatie", messages[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[5]);
                });
            }

            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidCalculationAndRun_SetsOutput()
        {
            // Setup
            var random = new Random(123);
            double expectedWaterLevel = random.NextDouble();
            double expectedWaveHeight = random.NextDouble();
            double expectedWavePeriod = random.NextDouble();
            double expectedReliabilityIndex = random.NextDouble();

            var duneLocationCalculation = new DuneLocationCalculation(new TestDuneLocation());
            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                WaterLevel = expectedWaterLevel,
                WaveHeight = expectedWaveHeight,
                WavePeriod = expectedWavePeriod,
                ReliabilityIndex = expectedReliabilityIndex,
                Converged = true
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var activity = new DuneLocationCalculationActivity(duneLocationCalculation,
                                                               CreateCalculationSettings(),
                                                               validNorm,
                                                               "A");

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            DuneLocationCalculationOutput actualCalculationOutput = duneLocationCalculation.Output;
            Assert.IsNotNull(actualCalculationOutput);
            Assert.AreEqual(expectedWaterLevel, actualCalculationOutput.WaterLevel, actualCalculationOutput.WaterLevel.GetAccuracy());
            Assert.AreEqual(expectedWaveHeight, actualCalculationOutput.WaveHeight, actualCalculationOutput.WaveHeight.GetAccuracy());
            Assert.AreEqual(expectedWavePeriod, actualCalculationOutput.WavePeriod, actualCalculationOutput.WavePeriod.GetAccuracy());
            Assert.AreEqual(expectedReliabilityIndex, actualCalculationOutput.CalculatedReliability, actualCalculationOutput.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, actualCalculationOutput.CalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculation_LogsError)
        })]
        public void Run_InvalidCalculation_LogsError(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                               CreateCalculationSettings(),
                                                               0.5,
                                                               categoryBoundaryName);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                string expectedFailureMessage = string.IsNullOrEmpty(lastErrorFileContent)
                                                    ? $"Er is een fout opgetreden tijdens de hydraulische belastingenberekening '{locationName}' (Categoriegrens {categoryBoundaryName}). Er is geen foutrapport beschikbaar."
                                                    : $"Er is een fout opgetreden tijdens de hydraulische belastingenberekening '{locationName}' (Categoriegrens {categoryBoundaryName}). Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{lastErrorFileContent}";

                TestHelper.AssertLogMessageIsGenerated(call, expectedFailureMessage, 7);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationResultingInNoConvergence_LogWarningNoConvergence()
        {
            // Setup
            const string categoryBoundaryName = "A";
            const string locationName = "locationName";

            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                Converged = false
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var duneLocationCalculation = new DuneLocationCalculation(new TestDuneLocation(locationName));

            var activity = new DuneLocationCalculationActivity(duneLocationCalculation,
                                                               CreateCalculationSettings(),
                                                               0.5,
                                                               categoryBoundaryName);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    Assert.AreEqual($"Hydraulische belastingenberekening voor locatie '{locationName}' (Categoriegrens {categoryBoundaryName}) is niet geconvergeerd.", msgs[4]);
                });
                Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, duneLocationCalculation.Output.CalculationConvergence);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_ErrorInCalculation_ActivityStateSetToFailed)
        })]
        public void Run_ErrorInCalculation_ActivityStateSetToFailed(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string locationName = "locationName";

            var calculator = new TestDunesBoundaryConditionsCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var activity = new DuneLocationCalculationActivity(new DuneLocationCalculation(new TestDuneLocation(locationName)),
                                                               CreateCalculationSettings(),
                                                               0.5,
                                                               "A");

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(ActivityState.Executed)]
        [TestCase(ActivityState.Failed)]
        [TestCase(ActivityState.Canceled)]
        [TestCase(ActivityState.Skipped)]
        public void Finish_ActivityWithSpecificState_NotifyDuneLocationCalculation(ActivityState state)
        {
            // Setup
            var duneLocationCalculation = new DuneLocationCalculation(new TestDuneLocation());

            var calculationObserver = mockRepository.StrictMock<IObserver>();
            calculationObserver.Expect(o => o.UpdateObserver());
            duneLocationCalculation.Attach(calculationObserver);
            mockRepository.ReplayAll();

            var activity = new DuneLocationCalculationActivityWithState(duneLocationCalculation,
                                                                        CreateCalculationSettings(),
                                                                        1.0,
                                                                        "A",
                                                                        state);

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                            validHlcdFilePath,
                                                            string.Empty);
        }

        private class DuneLocationCalculationActivityWithState : DuneLocationCalculationActivity
        {
            public DuneLocationCalculationActivityWithState(DuneLocationCalculation duneLocationCalculation,
                                                            HydraulicBoundaryCalculationSettings calculationSettings,
                                                            double norm,
                                                            string categoryBoundaryName,
                                                            ActivityState state)
                : base(duneLocationCalculation,
                       calculationSettings,
                       norm,
                       categoryBoundaryName)
            {
                State = state;
            }
        }
    }
}