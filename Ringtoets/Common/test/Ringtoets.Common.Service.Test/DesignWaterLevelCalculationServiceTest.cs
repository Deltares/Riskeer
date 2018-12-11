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
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class DesignWaterLevelCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validHydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var calculationService = new DesignWaterLevelCalculationService();

            // Assert
            Assert.IsInstanceOf<TargetProbabilityCalculationService>(calculationService);
        }

        [Test]
        public void Calculate_HydraulicBoundaryLocationCalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new DesignWaterLevelCalculationService().Calculate(null,
                                                                                         HydraulicBoundaryCalculationSettingsTestFactory.CreateSettings(),
                                                                                         1,
                                                                                         calculationMessageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationService().Calculate(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                                                                                         null,
                                                                                         1,
                                                                                         calculationMessageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_MessageProviderNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DesignWaterLevelCalculationService().Calculate(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                                                                                         HydraulicBoundaryCalculationSettingsTestFactory.CreateSettings(),
                                                                                         1,
                                                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidData_StartsCalculationWithRightParameters(bool usePreprocessor)
        {
            // Setup
            const double norm = 1.0 / 30;
            string preprocessorDirectory = usePreprocessor
                                               ? validPreprocessorDirectory
                                               : string.Empty;

            var calculator = new TestDesignWaterLevelCalculator
            {
                Converged = true
            };

            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                                               validHlcdFilePath,
                                                                               preprocessorDirectory);

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                   calculationSettings,
                                                                   norm,
                                                                   calculationMessageProvider);

                // Assert
                AssessmentLevelCalculationInput expectedInput = CreateInput(hydraulicBoundaryLocation.Id, norm);
                AssessmentLevelCalculationInput actualInput = calculator.ReceivedInputs.Single();
                AssertInput(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
                Assert.AreEqual(usePreprocessor, actualInput.PreprocessorSetting.RunPreprocessor);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidDesignWaterLevelCalculationAndConverges_SetsOutputAndLogs(bool readIllustrationPoints)
        {
            // Setup
            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsResult = new TestGeneralResult(),
                Converged = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = readIllustrationPoints
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                       CreateCalculationSettings(),
                                                                                       1.0 / 30,
                                                                                       calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });

                Assert.IsFalse(calculator.IsCanceled);
                HydraulicBoundaryLocationCalculationOutput actualOutput = hydraulicBoundaryLocationCalculation.Output;
                Assert.IsNotNull(actualOutput);
                Assert.AreEqual(readIllustrationPoints, actualOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidDesignWaterLevelCalculationAndDoesNotConverge_SetsOutputAndLogs(bool readIllustrationPoints)
        {
            // Setup
            const string locationName = "locationName";
            const string failedConvergenceMessage = "failedConvergenceMessage";

            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsResult = new TestGeneralResult(),
                Converged = false
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            calculationMessageProvider.Expect(c => c.GetCalculatedNotConvergedMessage(locationName)).Return(failedConvergenceMessage);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(locationName);
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = readIllustrationPoints
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                       CreateCalculationSettings(),
                                                                                       1.0 / 30,
                                                                                       calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(failedConvergenceMessage, msgs[1]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });

                Assert.IsFalse(calculator.IsCanceled);
                HydraulicBoundaryLocationCalculationOutput actualOutput = hydraulicBoundaryLocationCalculation.Output;
                Assert.IsNotNull(actualOutput);
                Assert.AreEqual(readIllustrationPoints, actualOutput.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationButIllustrationPointResultsOfIncorrectType_IllustrationPointNotSetAndLog()
        {
            // Setup
            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithFaultTreeIllustrationPoints(),
                Converged = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                       CreateCalculationSettings(),
                                                                                       1.0 / 30,
                                                                                       calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();
                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[1]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[1].Item3);
                });
                Assert.IsNotNull(hydraulicBoundaryLocationCalculation.Output);
                Assert.IsFalse(hydraulicBoundaryLocationCalculation.Output.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            const string locationName = "locationName";

            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts(),
                Converged = true
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.StrictMock<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName))
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                       CreateCalculationSettings(),
                                                                                       1.0 / 30,
                                                                                       calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();
                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"Fout bij het uitlezen van de illustratiepunten voor berekening {locationName}: " +
                                    "Een of meerdere stochasten hebben dezelfde naam. " +
                                    "Het uitlezen van illustratiepunten wordt overgeslagen.", msgs[1]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsNotNull(hydraulicBoundaryLocationCalculation.Output);
                Assert.IsFalse(hydraulicBoundaryLocationCalculation.Output.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationThrowsException_ThrowsHydraRingFileParserException()
        {
            // Setup
            const string locationName = "locationName";

            var expectedException = new HydraRingFileParserException();

            var mockRepository = new MockRepository();
            var calculator = mockRepository.Stub<IDesignWaterLevelCalculator>();
            calculator.Expect(c => c.Calculate(Arg<AssessmentLevelCalculationInput>.Is.TypeOf))
                      .Throw(expectedException);
            calculator.Stub(c => c.LastErrorFileContent).Return(string.Empty);
            calculator.Stub(c => c.OutputDirectory).Return(string.Empty);

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            calculationMessageProvider.Stub(mp => mp.GetCalculatedNotConvergedMessage(locationName)).Return(string.Empty);
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName));

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                TestDelegate call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                             CreateCalculationSettings(),
                                                                                             1.0 / 30,
                                                                                             calculationMessageProvider);

                // Assert
                var thrownException = Assert.Throws<HydraRingFileParserException>(call);
                Assert.AreSame(expectedException, thrownException);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationAndOutputButIllustrationPointsAreNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            const string parserErrorMessage = "Some Error Message";
            var mockRepository = new MockRepository();
            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsParserErrorMessage = parserErrorMessage,
                Converged = true
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation())
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                       CreateCalculationSettings(),
                                                                                       1.0 / 30,
                                                                                       calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(parserErrorMessage, msgs[1]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsFalse(hydraulicBoundaryLocationCalculation.Output.HasGeneralResult);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationCalculateIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var mockRepository = new MockRepository();
            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsParserErrorMessage = "Some Error Message",
                Converged = true
            };

            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                Action call = () => new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                                       CreateCalculationSettings(),
                                                                                       1.0 / 30,
                                                                                       calculationMessageProvider);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var calculator = new TestDesignWaterLevelCalculator();

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new DesignWaterLevelCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(hydraulicBoundaryLocationCalculation,
                                  CreateCalculationSettings(),
                                  1.0 / 30,
                                  calculationMessageProvider);

                // Assert
                Assert.IsTrue(calculator.IsCanceled);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        {
            nameof(Run_InvalidCalculation_LogsErrorAndThrowException)
        })]
        public void Run_InvalidCalculation_LogsErrorAndThrowException(bool endInFailure, string lastErrorFileContent)
        {
            // Setup
            const string locationName = "locationName";
            const string calculationFailedMessage = "calculationFailedMessage";

            var calculator = new TestDesignWaterLevelCalculator
            {
                LastErrorFileContent = lastErrorFileContent,
                EndInFailure = endInFailure
            };

            var mockRepository = new MockRepository();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var calculationMessageProvider = mockRepository.Stub<ICalculationMessageProvider>();
            if (endInFailure && string.IsNullOrEmpty(lastErrorFileContent))
            {
                calculationMessageProvider.Expect(mp => mp.GetCalculationFailedMessage(locationName)).Return(calculationFailedMessage);
            }
            else
            {
                calculationMessageProvider.Expect(mp => mp.GetCalculationFailedWithErrorReportMessage(locationName,
                                                                                                      endInFailure && string.IsNullOrEmpty(lastErrorFileContent)
                                                                                                          ? calculator.HydraRingCalculationException.Message
                                                                                                          : lastErrorFileContent
                                                  )).Return(calculationFailedMessage);
            }

            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation(locationName));

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                HydraRingCalculationException exception = null;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                           CreateCalculationSettings(),
                                                                           1.0 / 30,
                                                                           calculationMessageProvider);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exception = e;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(calculationFailedMessage, msgs[1]);
                    Assert.AreEqual($"Waterstand berekening is uitgevoerd op de tijdelijke locatie '{calculator.OutputDirectory}'. " +
                                    "Gedetailleerde invoer en uitvoer kan in de bestanden op deze locatie worden gevonden.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });

                Assert.IsInstanceOf<HydraRingCalculationException>(exception);
            }

            mockRepository.VerifyAll();
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHydraulicBoundaryDatabaseFilePath,
                                                            validHlcdFilePath,
                                                            string.Empty);
        }

        private static void AssertInput(AssessmentLevelCalculationInput expectedInput, HydraRingCalculationInput hydraRingCalculationInput)
        {
            Assert.AreEqual(expectedInput.Section.SectionId, hydraRingCalculationInput.Section.SectionId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, hydraRingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Beta, hydraRingCalculationInput.Beta);
        }

        private static AssessmentLevelCalculationInput CreateInput(long hydraulicBoundaryLocationId, double norm)
        {
            return new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocationId, norm);
        }
    }
}