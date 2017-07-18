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
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class WaveHeightCalculationActivityTest
    {
        private const string validFile = "HRD dutch coast south.sqlite";
        private MockRepository mockRepository;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");

        private static IEnumerable<TestCaseData> HydraulicBoundaryLocationsToCalculate
        {
            get
            {
                yield return new TestCaseData(new TestHydraulicBoundaryLocation("WithOutputWithoutIllustrationPoints")
                {
                    WaveHeightCalculation =
                    {
                        InputParameters =
                        {
                            ShouldIllustrationPointsBeCalculated = true
                        },
                        Output = new TestHydraulicBoundaryLocationOutput(1.0, CalculationConvergence.CalculatedConverged)
                    }
                });

                yield return new TestCaseData(new TestHydraulicBoundaryLocation("WithoutOutput"));
            }
        }

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            const string locationName = "locationName";
            const string activityDescription = "activityDescription";

            var calculation = new WaveHeightCalculation(new TestHydraulicBoundaryLocation(locationName));

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            var activity = new WaveHeightCalculationActivity(calculation, validFilePath, 1, calculationMessageProvider);

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
            Assert.AreSame(activityDescription, activity.Description);
            Assert.IsNull(activity.ProgressText);
            Assert.AreEqual(ActivityState.None, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_NullCalculationServiceMessageProvider_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new WaveHeightCalculation(new TestHydraulicBoundaryLocation(string.Empty));

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new WaveHeightCalculationActivity(calculation, validFilePath, 1, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void ParameteredConstructor_WaveHeightCalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            // Call
            TestDelegate call = () => new WaveHeightCalculationActivity(null, validFilePath, 1, calculationMessageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveHeightCalculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_InvalidHydraulicBoundaryDatabase_PerformValidationAndLogStartAndEndAndError()
        {
            // Setup
            string inValidFilePath = Path.Combine(testDataPath, "notexisting.sqlite");
            const string locationName = "testLocation";
            const string activityDescription = "activityDescription";

            var calculation = new WaveHeightCalculation(new TestHydraulicBoundaryLocation(locationName));

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            mockRepository.ReplayAll();

            var activity = new WaveHeightCalculationActivity(calculation, inValidFilePath, 1, calculationMessageProvider);

            // Call
            Action call = () => activity.Run();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                Assert.AreEqual($"{activityDescription} is gestart.", msgs[0]);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[1]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand", msgs[2]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[3]);
            });
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidHydraulicBoundaryLocation_PerformValidationAndCalculationAndLogStartAndEnd()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const string activityDescription = "activityDescription";
            const double norm = 1.0 / 30;

            var calculation = new WaveHeightCalculation(new TestHydraulicBoundaryLocation(locationName));
            var calculator = new TestWaveHeightCalculator
            {
                Converged = true
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(activityDescription);
            mockRepository.ReplayAll();

            var activity = new WaveHeightCalculationActivity(calculation,
                                                             validFilePath,
                                                             norm, calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, m =>
                {
                    string[] messages = m.ToArray();
                    Assert.AreEqual(6, messages.Length);
                    Assert.AreEqual($"{activityDescription} is gestart.", messages[0]);
                    CalculationServiceTestHelper.AssertValidationStartMessage(messages[1]);
                    CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                    StringAssert.StartsWith("Golfhoogte berekening is uitgevoerd op de tijdelijke locatie", messages[4]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(messages[5]);
                });
                WaveHeightCalculationInput waveHeightCalculationInput = calculator.ReceivedInputs.Single();

                Assert.AreEqual(calculation.Id, waveHeightCalculationInput.HydraulicBoundaryLocationId);
                Assert.AreEqual(StatisticsConverter.ProbabilityToReliability(norm), waveHeightCalculationInput.Beta);
            }
            Assert.AreEqual(ActivityState.Executed, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_HydraulicLocationWaveHeightSet_ValidationAndCalculationNotPerformedAndStateSkipped()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, validFile);
            const string locationName = "locationName";
            const double norm = 1.0 / 30;

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = false
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(3.0, CalculationConvergence.CalculatedConverged)
                }
            };
            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            var activity = new WaveHeightCalculationActivity(calculation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Skipped, activity.State);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(HydraulicBoundaryLocationsToCalculate))]
        public void Run_ValidCalculation_SetsWaveHeightAndConvergence(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            // Setup
            const double norm = 1.0 / 30;
            const double expectedWaveHeight = 3.5;
            string locationName = hydraulicBoundaryLocation.Name;

            var calculator = new TestWaveHeightCalculator
            {
                Converged = true,
                WaveHeight = expectedWaveHeight
            };

            if (hydraulicBoundaryLocation.WaveHeightCalculation.InputParameters.ShouldIllustrationPointsBeCalculated)
            {
                calculator.IllustrationPointsResult = new TestGeneralResult();
            }

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            var activity = new WaveHeightCalculationActivity(calculation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                activity.Run();
            }

            // Assert
            HydraulicBoundaryLocationOutput calculationOutput = hydraulicBoundaryLocation.WaveHeightCalculation.Output;
            Assert.IsNotNull(calculationOutput);
            Assert.AreEqual(expectedWaveHeight, calculationOutput.Result, calculationOutput.Result.GetAccuracy());
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, calculationOutput.CalculationConvergence);
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculation_LogsErrorOutputNotUpdated)
        })]
        public void Run_InvalidCalculation_LogsErrorOutputNotUpdated(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string locationName = "locationName";

            var calculator = new TestWaveHeightCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            const string failureMessage = "Failed calculation";
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            if (string.IsNullOrEmpty(lastErrorFileContent))
            {
                calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(null))
                                          .IgnoreArguments()
                                          .Return(failureMessage);
            }
            else
            {
                calculationMessageProvider.Stub(calc => calc.GetCalculationFailedWithErrorReportMessage(null, null))
                                          .IgnoreArguments()
                                          .Return(failureMessage);
            }

            mockRepository.ReplayAll();

            var output = new TestHydraulicBoundaryLocationOutput(double.NaN, CalculationConvergence.CalculatedConverged);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = output
                }
            };

            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            string validFilePath = Path.Combine(testDataPath, validFile);

            var activity = new WaveHeightCalculationActivity(calculation,
                                                             validFilePath,
                                                             30,
                                                             calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessageIsGenerated(call, failureMessage, 7);
                Assert.AreSame(output, hydraulicBoundaryLocation.WaveHeightCalculation.Output);
                Assert.AreEqual(CalculationConvergence.CalculatedConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Run_CalculationResultingInNoConvergence_LogWarningNoConvergence()
        {
            // Setup
            const string locationName = "locationName";
            const string calculationNotConvergedMessage = "calculationNotConvergedMessage";

            var calculator = new TestWaveHeightCalculator
            {
                Converged = false,
                IllustrationPointsResult = new TestGeneralResult()
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(calculationNotConvergedMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, locationName, 0, 0)
            {
                WaveHeightCalculation =
                {
                    InputParameters =
                    {
                        ShouldIllustrationPointsBeCalculated = true
                    },
                    Output = new TestHydraulicBoundaryLocationOutput(double.NaN, CalculationConvergence.CalculatedConverged)
                }
            };

            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            string validFilePath = Path.Combine(testDataPath, validFile);
            const double norm = 1.0 / 300;
            var activity = new WaveHeightCalculationActivity(calculation,
                                                             validFilePath,
                                                             norm, calculationMessageProvider);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => activity.Run();

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(7, msgs.Length);
                    StringAssert.StartsWith(calculationNotConvergedMessage, msgs[4]);
                });
                Assert.AreEqual(CalculationConvergence.CalculatedNotConverged, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_ErrorInCalculation_PerformValidationAndCalculationAndLogStartAndEndAndError)
        })]
        public void Run_ErrorInCalculation_PerformValidationAndCalculationAndLogStartAndEndAndError(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string locationName = "locationName 1";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, locationName, 0, 0);

            var calculator = new TestWaveHeightCalculator
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath)).Return(calculator);
            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(calc => calc.GetActivityDescription(locationName)).Return(string.Empty);
            if (string.IsNullOrEmpty(lastErrorFileContent))
            {
                calculationMessageProvider.Stub(calc => calc.GetCalculationFailedMessage(locationName)).Return(string.Empty);
            }
            else
            {
                calculationMessageProvider.Stub(calc => calc.GetCalculationFailedWithErrorReportMessage(locationName, lastErrorFileContent)).Return(string.Empty);
            }
            calculationMessageProvider.Stub(calc => calc.GetCalculatedNotConvergedMessage(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            string validFilePath = Path.Combine(testDataPath, validFile);

            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            const double norm = 1.0 / 30;
            var activity = new WaveHeightCalculationActivity(calculation,
                                                             validFilePath,
                                                             norm,
                                                             calculationMessageProvider);

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
        public void Finish_ActivityWithSpecificState_NotifyHydraulicBoundaryLocation(ActivityState state)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new WaveHeightCalculation(hydraulicBoundaryLocation);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            hydraulicBoundaryLocation.Attach(observer);
            mockRepository.ReplayAll();

            var activity = new TestWaveHeightCalculationActivity(calculation,
                                                                 Path.Combine(testDataPath, validFile),
                                                                 1.0,
                                                                 calculationMessageProvider,
                                                                 state);

            // Call
            activity.Finish();

            // Assert
            mockRepository.VerifyAll();
        }

        private class TestWaveHeightCalculationActivity : WaveHeightCalculationActivity
        {
            public TestWaveHeightCalculationActivity(WaveHeightCalculation waveHeightCalculation,
                                                     string hydraulicBoundaryDatabaseFilePath,
                                                     double norm,
                                                     ICalculationMessageProvider messageProvider,
                                                     ActivityState state)
                : base(waveHeightCalculation,
                       hydraulicBoundaryDatabaseFilePath,
                       norm,
                       messageProvider)
            {
                State = state;
            }
        }
    }
}