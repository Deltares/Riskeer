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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.MessageProviders;
using Riskeer.Common.Service.Structures;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Exceptions;
using Riskeer.HydraRing.Calculation.TestUtil;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Riskeer.Common.Service.Test.Structures
{
    [TestFixture]
    public class StructuresCalculationServiceBaseTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.Service, "HydraRingCalculation");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestStructuresCalculationService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("messageProvider", exception.ParamName);
        }

        [Test]
        public void Validate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            // Call
            void Call() => TestStructuresCalculationService.Validate(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Validate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new TestStructuresCalculation();

            // Call
            void Call() => TestStructuresCalculationService.Validate(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Validate_CalculationWithoutHydraulicBoundaryLocation_LogsErrorAndReturnsFalse()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism());
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    Structure = new TestStructure()
                }
            };

            var isValid = false;

            // Call
            void Call() => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism(),
                                                                                                           Path.Combine(testDataPath, "notexisting.sqlite"));
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First()
                }
            };

            var isValid = true;

            // Call
            void Call() => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                        "Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_HydraulicBoundaryDatabaseWithoutSettings_LogsErrorAndReturnsFalse()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism(),
                                                                                                           Path.Combine(testDataPath, "HRD nosettings.sqlite"));
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First()
                }
            };

            var isValid = false;

            // Call
            void Call() => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. " +
                                        "Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutStructures_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, validHrdFilePath);
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First()
                }
            };

            var isValid = false;

            // Call
            void Call() => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen kunstwerk geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_InputInvalidAccordingToValidationRule_LogErrorAndReturnFalse()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, validHrdFilePath);
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First(),
                    IsValid = false
                }
            };

            var isValid = false;

            // Call
            void Call() => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Error message", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();

            // Call
            void Call() => new TestStructuresCalculationService(messageProvider).Calculate(null, new GeneralTestInput(), CreateCalculationSettings());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();

            // Call
            void Call() => new TestStructuresCalculationService(messageProvider).Calculate(new TestStructuresCalculation(), null, CreateCalculationSettings());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
        }

        [Test]
        public void Calculate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();

            // Call
            void Call() => new TestStructuresCalculationService(messageProvider).Calculate(new TestStructuresCalculation(), new GeneralTestInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidInput_InputPropertiesCorrectlySentToCalculator(bool usePreprocessorClosure)
        {
            // Setup
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHlcdFilePath, validHrdFilePath,
                                                                               validHrdFileVersion, usePreprocessorClosure);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(
                                 Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(callInfo =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, callInfo.Arg<HydraRingCalculationSettings>());
                                 return calculator;
                             });

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            messageProvider.GetCalculationPerformedMessage(validHrdFilePath).Returns(performedCalculationMessage);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);

                // Call
                service.Calculate(calculation, new GeneralTestInput(), calculationSettings);

                // Assert
                ExceedanceProbabilityCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                var expectedInput = new TestExceedanceProbabilityCalculationInput(hydraulicBoundaryLocation.Id);
                ExceedanceProbabilityCalculationInput actualInput = calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidStructuresCalculation_SetsOutputAndLogs(bool readIllustrationPoints)
        {
            // Setup
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            messageProvider.GetCalculationPerformedMessage(validHrdFilePath).Returns(performedCalculationMessage);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ShouldIllustrationPointsBeCalculated = readIllustrationPoints
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);

                // Call
                void Call() => service.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(performedCalculationMessage, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });

                Assert.IsFalse(calculator.IsCanceled);

                Assert.IsNotNull(calculation.Output);
                Assert.AreEqual(readIllustrationPoints, calculation.Output.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_ValidInputButIllustrationPointsNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            const string parserMessage = "Parser error";
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsParserErrorMessage = parserMessage
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            messageProvider.GetCalculationPerformedMessage(validHrdFilePath).Returns(performedCalculationMessage);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ShouldIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);

                // Call
                void Call() => service.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(parserMessage, msgs[1]);
                    Assert.AreEqual(performedCalculationMessage, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_ValidInputCalculateIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsParserErrorMessage = "Parser error"
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            messageProvider.GetCalculationPerformedMessage(validHrdFilePath).Returns(performedCalculationMessage);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ShouldIllustrationPointsBeCalculated = false
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);

                // Call
                void Call() => service.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(performedCalculationMessage, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_ValidInputButIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            messageProvider.GetCalculationPerformedMessage(validHrdFilePath).Returns(performedCalculationMessage);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ShouldIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);

                // Call
                void Call() => service.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();
                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Het uitlezen van illustratiepunten is mislukt.", msgs[1]);
                    Assert.AreEqual(performedCalculationMessage, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);

                    Assert.IsInstanceOf<IllustrationPointConversionException>(tupleArray[1].Item3);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_CalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            messageProvider.GetCalculationPerformedMessage(validHrdFilePath).Returns(performedCalculationMessage);
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ShouldIllustrationPointsBeCalculated = true
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);

                // Call
                void Call() => service.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(Call, messages =>
                {
                    Tuple<string, Level, Exception>[] tupleArray = messages.ToArray();

                    string[] msgs = tupleArray.Select(tuple => tuple.Item1).ToArray();
                    Assert.AreEqual(4, msgs.Length);

                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual("Fout bij het uitlezen van de illustratiepunten voor berekening Nieuwe berekening: " +
                                    "Een of meerdere stochasten hebben dezelfde naam. Het uitlezen van illustratiepunten wordt overgeslagen.", msgs[1]);
                    Assert.AreEqual(performedCalculationMessage, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculation.Output.HasGeneralResult);
            }
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>();
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }
        }

        [Test]
        [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        {
            nameof(Calculate_CalculationFailed_ThrowsHydraRingCalculationExceptionAndLogError)
        })]
        public void Calculate_CalculationFailed_ThrowsHydraRingCalculationExceptionAndLogError(bool endInFailure,
                                                                                               string lastErrorFileContent,
                                                                                               string detailedReport)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
            calculatorFactory.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(Arg.Is<HydraRingCalculationSettings>(x => x != null))
                             .Returns(calculator);

            const string calculationFailedMessage = "Calculation failed";
            const string calculationPerformedMessage = "Calculation performed";
            var messageProvider = Substitute.For<IStructuresCalculationMessageProvider>();

            if (endInFailure && string.IsNullOrEmpty(lastErrorFileContent))
            {
                messageProvider.GetCalculationFailedMessage(calculation.Name).Returns(calculationFailedMessage);
            }
            else
            {
                messageProvider.GetCalculationFailedWithErrorReportMessage(calculation.Name,
                                                                           endInFailure && string.IsNullOrEmpty(lastErrorFileContent)
                                                                               ? calculator.HydraRingCalculationException.Message
                                                                               : lastErrorFileContent
                ).Returns(calculationFailedMessage);
            }

            messageProvider.GetCalculationPerformedMessage(calculator.OutputDirectory).Returns(calculationPerformedMessage);
            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;
                var structuresCalculationService = new TestStructuresCalculationService(messageProvider);

                // Call
                void Call()
                {
                    try
                    {
                        structuresCalculationService.Calculate(calculation, new GeneralTestInput(), CreateCalculationSettings());
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                }

                // Assert
                TestHelper.AssertLogMessages(Call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(calculationFailedMessage, msgs[1]);
                    Assert.AreEqual(calculationPerformedMessage, msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHlcdFilePath, validHrdFilePath, validHrdFileVersion, false);
        }

        private class TestStructuresCalculationService : StructuresCalculationServiceBase<TestStructureValidationRulesRegistry,
            TestStructuresInput, TestStructure, GeneralTestInput, ExceedanceProbabilityCalculationInput>
        {
            public TestStructuresCalculationService(IStructuresCalculationMessageProvider messageProvider) : base(messageProvider) {}

            protected override ExceedanceProbabilityCalculationInput CreateInput(TestStructuresInput structureInput,
                                                                                 GeneralTestInput generalInput,
                                                                                 string hrdFilePath)
            {
                return new TestExceedanceProbabilityCalculationInput(structureInput.HydraulicBoundaryLocation.Id);
            }
        }

        private class TestExceedanceProbabilityCalculationInput : ExceedanceProbabilityCalculationInput
        {
            public TestExceedanceProbabilityCalculationInput(long hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType { get; }

            public override int VariableId { get; }

            public override int FaultTreeModelId { get; }

            public override HydraRingSection Section => new HydraRingSection(0, 0, 0);
        }

        private class GeneralTestInput {}
    }
}