// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.MessageProviders;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class DesignWaterLevelCalculationServiceTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

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
            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
            // Call
            TestDelegate test = () => new DesignWaterLevelCalculationService().Calculate(null,
                                                                                         CreateCalculationSettings(),
                                                                                         1,
                                                                                         calculationMessageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", exception.ParamName);
        }

        [Test]
        public void Calculate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
            // Call
            TestDelegate call = () => new DesignWaterLevelCalculationService().Calculate(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                                                                                         null,
                                                                                         1,
                                                                                         calculationMessageProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        public void Calculate_MessageProviderNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new DesignWaterLevelCalculationService().Calculate(new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation()),
                                                                                         CreateCalculationSettings(),
                                                                                         1,
                                                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidData_StartsCalculationWithRightParameters(bool usePreprocessorClosure)
        {
            // Setup
            const double targetProbability = 1.0 / 30;

            var calculator = new TestDesignWaterLevelCalculator
            {
                Converged = true
            };

            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHlcdFilePath, validHrdFilePath,
                                                                               validHrdFileVersion, usePreprocessorClosure);
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory
                .CreateDesignWaterLevelCalculator(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                .Returns(callInfo =>
                {
                    HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                        calculationSettings,
                        callInfo.Arg<HydraRingCalculationSettings>());
                    return calculator;
                });

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                // Call
                new DesignWaterLevelCalculationService().Calculate(hydraulicBoundaryLocationCalculation,
                                                                   calculationSettings,
                                                                   targetProbability,
                                                                   calculationMessageProvider);

                // Assert
                AssessmentLevelCalculationInput expectedInput = CreateInput(hydraulicBoundaryLocation.Id, targetProbability);
                AssessmentLevelCalculationInput actualInput = calculator.ReceivedInputs.Single();
                AssertInput(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
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
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
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

            calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
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
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);
            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
            calculationMessageProvider.GetCalculatedNotConvergedMessage(locationName).Returns(failedConvergenceMessage);
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

                calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
                Assert.IsFalse(calculator.IsCanceled);
                HydraulicBoundaryLocationCalculationOutput actualOutput = hydraulicBoundaryLocationCalculation.Output;
                Assert.IsNotNull(actualOutput);
                Assert.AreEqual(readIllustrationPoints, actualOutput.HasGeneralResult);
            }
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
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
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
                calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
            }
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
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
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

            calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationThrowsException_ThrowsHydraRingFileParserException()
        {
            // Setup
            const string locationName = "locationName";

            var expectedException = new HydraRingFileParserException();
            var calculator = Substitute.For<IDesignWaterLevelCalculator>();
            calculator.When(_ => _.Calculate(Arg.Any<AssessmentLevelCalculationInput>())).Do(_ => throw expectedException);

            calculator.LastErrorFileContent.Returns(string.Empty);
            calculator.OutputDirectory.Returns(string.Empty);

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>()).Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
            calculationMessageProvider.GetCalculatedNotConvergedMessage(locationName).Returns(string.Empty);
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
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationAndOutputButIllustrationPointsAreNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            const string parserErrorMessage = "Some Error Message";
            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsParserErrorMessage = parserErrorMessage,
                Converged = true
            };

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();

            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
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
                calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
            }
        }

        [Test]
        public void Calculate_ValidDesignWaterLevelCalculationCalculateIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var calculator = new TestDesignWaterLevelCalculator
            {
                IllustrationPointsParserErrorMessage = "Some Error Message",
                Converged = true
            };

            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);
            ;

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
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
                calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
            }
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculator()
        {
            // Setup
            var calculator = new TestDesignWaterLevelCalculator();
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
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
                calculatorFactory.Received().CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>());
            }
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
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateDesignWaterLevelCalculator(Arg.Any<HydraRingCalculationSettings>())
                             .Returns(calculator);

            var calculationMessageProvider = Substitute.For<ICalculationMessageProvider>();
            if (endInFailure && string.IsNullOrEmpty(lastErrorFileContent))
            {
                calculationMessageProvider.GetCalculationFailedMessage(locationName).Returns(calculationFailedMessage);
            }
            else
            {
                calculationMessageProvider.GetCalculationFailedWithErrorReportMessage(locationName,
                                                                                      endInFailure && string.IsNullOrEmpty(lastErrorFileContent)
                                                                                          ? calculator.HydraRingCalculationException.Message
                                                                                          : lastErrorFileContent
                ).Returns(calculationFailedMessage);
            }

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
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHlcdFilePath, validHrdFilePath, validHrdFileVersion, false);
        }

        private static void AssertInput(AssessmentLevelCalculationInput expectedInput, HydraRingCalculationInput hydraRingCalculationInput)
        {
            Assert.AreEqual(expectedInput.Section.SectionId, hydraRingCalculationInput.Section.SectionId);
            Assert.AreEqual(expectedInput.HydraulicBoundaryLocationId, hydraRingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(expectedInput.Beta, hydraRingCalculationInput.Beta);
        }

        private static AssessmentLevelCalculationInput CreateInput(long hydraulicBoundaryLocationId, double targetProbability)
        {
            return new AssessmentLevelCalculationInput(1, hydraulicBoundaryLocationId, targetProbability);
        }
    }
}