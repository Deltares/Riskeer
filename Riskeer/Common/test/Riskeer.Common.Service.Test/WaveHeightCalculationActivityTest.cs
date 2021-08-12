// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class WaveHeightCalculationActivityTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        private static IEnumerable<TestCaseData> HydraulicBoundaryLocationCalculationsToPerform
        {
            get
            {
                yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("Test"))
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationCalculationOutput(1.0, CalculationConvergence.CalculatedConverged)
                });

                yield return new TestCaseData(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("Test")));
            }
        }

        [Test]
        public void Constructor_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation("locationName")),
                                                             null,
                                                             0.01,
                                                             "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";

            // Call
            var activity = new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName)),
                                                             CreateCalculationSettings(),
                                                             0.01,
                                                             calculationIdentifier);

            // Assert
            Assert.IsInstanceOf<CalculatableActivity>(activity);
            Assert.AreEqual(GetActivityDescription(locationName, calculationIdentifier), activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            string invalidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";

            var settings = new HydraulicBoundaryCalculationSettings(invalidFilePath,
                                                                    validHlcdFilePath,
                                                                    false,
                                                                    string.Empty);
            var activity = new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName)),
                                                             settings,
                                                             0.01,
                                                             calculationIdentifier);

            // Call
            void Call() => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"{GetActivityDescription(locationName, calculationIdentifier)} is gestart.", msgs[0]);
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
            const string invalidPreprocessorDirectory = "NonExistingPreprocessorDirectory";
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";

            var settings = new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                                    validHlcdFilePath,
                                                                    false,
                                                                    invalidPreprocessorDirectory);
            var activity = new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName)),
                                                             settings,
                                                             0.01,
                                                             calculationIdentifier);

            // Call
            void Call() => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"{GetActivityDescription(locationName, calculationIdentifier)} is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_InvalidTargetProbability_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            const string locationName = "locationName";
            const string calculationIdentifier = "1/1";

            var activity = new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName)),
                                                             CreateCalculationSettings(),
                                                             1.0,
                                                             calculationIdentifier);

            // Call
            void Call() => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"{GetActivityDescription(locationName, calculationIdentifier)} is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                Assert.AreEqual("Doelkans is te groot om een berekening uit te kunnen voeren.", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Run_ValidInput_PerformCalculationWithCorrectInput(bool usePreprocessor)
        {
            // Setup
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";
            const double targetProbability = 0.01;

            var calculator = new TestWaveHeightCalculator
            {
                Converged = true
            };
            string preprocessorDirectory = usePreprocessor ? validPreprocessorDirectory : string.Empty;
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                                               validHlcdFilePath,
                                                                               false,
                                                                               preprocessorDirectory);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(locationName);

            var activity = new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation),
                                                             calculationSettings,
                                                             targetProbability,
                                                             calculationIdentifier);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();

                // Assert
                WaveHeightCalculationInput waveHeightCalculationInput = calculator.ReceivedInputs.Single();

                Assert.AreEqual(hydraulicBoundaryLocation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(targetProbability), waveHeightCalculationInput.Beta);
            }

            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_ValidInput_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";
            const double targetProbability = 0.01;

            var calculator = new TestWaveHeightCalculator
            {
                Converged = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(locationName);

            var activity = new WaveHeightCalculationActivity(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation),
                                                             CreateCalculationSettings(),
                                                             targetProbability,
                                                             calculationIdentifier);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(6, messages.Length);
                    Assert.AreEqual($"{GetActivityDescription(locationName, calculationIdentifier)} is gestart.", messages[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[5]);
                });
            }

            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(HydraulicBoundaryLocationCalculationsToPerform))]
        public void Run_ValidCalculation_SetsWaveHeightAndConvergence(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
        {
            // Setup
            const double targetProbability = 0.01;
            const double expectedWaveHeight = 3.5;

            var calculator = new TestWaveHeightCalculator
            {
                Converged = true,
                WaveHeight = expectedWaveHeight
            };

            if (hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated)
            {
                calculator.IllustrationPointsResult = new TestGeneralResult();
            }

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocationCalculation,
                                                             CreateCalculationSettings(),
                                                             targetProbability,
                                                             "1/100");

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            HydraulicBoundaryLocationCalculationOutput calculationOutput = hydraulicBoundaryLocationCalculation.Output;
            Assert.IsNotNull(calculationOutput);
            Assert.AreEqual(expectedWaveHeight, calculationOutput.Result, calculationOutput.Result.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, calculationOutput.CalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculation_LogsErrorOutputNotUpdatedAndActivityStateFailed)
        })]
        public void Run_InvalidCalculation_LogsErrorOutputNotUpdatedAndActivityStateFailed(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";

            var calculator = new TestWaveHeightCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var output = new TestHydraulicBoundaryLocationCalculationOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName))
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = output
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocationCalculation,
                                                             CreateCalculationSettings(),
                                                             0.01,
                                                             calculationIdentifier);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(7, messages.Length);
                    Assert.AreEqual($"{GetActivityDescription(locationName, calculationIdentifier)} is gestart.", messages[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);

                    string expectedFailureMessage = string.IsNullOrEmpty(lastErrorFileContent)
                                                        ? $"Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie '{locationName}' ({calculationIdentifier}). Er is geen foutrapport beschikbaar."
                                                        : $"Er is een fout opgetreden tijdens de golfhoogte berekening voor locatie '{locationName}' ({calculationIdentifier}). Bekijk het foutrapport door op details te klikken.{Environment.NewLine}{lastErrorFileContent}";
                    Assert.AreEqual(expectedFailureMessage, messages[4]);

                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages[5]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[6]);
                });

                Assert.AreSame(output, hydraulicBoundaryLocationCalculation.Output);
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocationCalculation.Output.CalculationConvergence);
                Assert.AreEqual(ActivityState.Failed, activity.State);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationResultingInNoConvergence_LogWarningNoConvergence()
        {
            // Setup
            const string locationName = "locationName";
            const string calculationIdentifier = "1/100";

            var calculator = new TestWaveHeightCalculator
            {
                Converged = false,
                IllustrationPointsResult = new TestGeneralResult()
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName))
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                },
                Output = new TestHydraulicBoundaryLocationCalculationOutput(double.NaN, CalculationConvergence.CalculatedConverged)
            };

            var activity = new WaveHeightCalculationActivity(hydraulicBoundaryLocationCalculation,
                                                             CreateCalculationSettings(),
                                                             0.01,
                                                             calculationIdentifier);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                void Call() => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    Assert.AreEqual($"Golfhoogte berekening voor locatie 'locationName' ({calculationIdentifier}) is niet geconvergeerd.", msgs[4]);
                });
                Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocationCalculation.Output.CalculationConvergence);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(ActivityState.Executed)]
        [TestCase(ActivityState.Failed)]
        [TestCase(ActivityState.Canceled)]
        [TestCase(ActivityState.Skipped)]
        public void Finish_ActivityWithSpecificState_NotifyHydraulicBoundaryLocationCalculation(ActivityState state)
        {
            // Setup
            const string calculationIdentifier = "1/100";

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            hydraulicBoundaryLocationCalculation.Attach(observer);
            mockRepository.ReplayAll();

            var activity = new TestWaveHeightCalculationActivity(hydraulicBoundaryLocationCalculation,
                                                                 CreateCalculationSettings(),
                                                                 0.01,
                                                                 calculationIdentifier,
                                                                 state);

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }

        private static string GetActivityDescription(string locationName, string calculationIdentifier)
        {
            return $"Golfhoogte berekenen voor locatie '{locationName}' ({calculationIdentifier})";
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                            validHlcdFilePath,
                                                            false,
                                                            string.Empty);
        }

        private class TestWaveHeightCalculationActivity : WaveHeightCalculationActivity
        {
            public TestWaveHeightCalculationActivity(HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation,
                                                     HydraulicBoundaryCalculationSettings calculationSettings,
                                                     double targetProbability,
                                                     string calculationIdentifier,
                                                     ActivityState state)
                : base(hydraulicBoundaryLocationCalculation,
                       calculationSettings,
                       targetProbability,
                       calculationIdentifier)
            {
                State = state;
            }
        }
    }
}