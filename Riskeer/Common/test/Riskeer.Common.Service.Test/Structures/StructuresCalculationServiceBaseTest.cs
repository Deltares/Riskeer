﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Rhino.Mocks;
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
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "Hlcd.sqlite");

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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => TestStructuresCalculationService.Validate(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
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
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism(),
                                                                                                           mocks,
                                                                                                           Path.Combine(testDataPath, "notexisting.sqlite"));
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ValidCalculationValidHydraulicBoundaryDatabaseNoSettings_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism(),
                                                                                                           mocks,
                                                                                                           Path.Combine(testDataPath, "HRD nosettings.sqlite"));
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_WithoutImportedHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism(),
                                                                                                           mocks);
            mocks.ReplayAll();

            var isValid = false;

            // Call
            void Call() => isValid = TestStructuresCalculationService.Validate(new TestStructuresCalculation(), assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(Call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische belastingendatabase geïmporteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_CalculationInputWithoutHydraulicBoundaryLocationValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(new TestCalculatableFailureMechanism(),
                                                                                                           mocks,
                                                                                                           validHrdFilePath);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_CalculationWithoutStructuresValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHrdFilePath);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
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

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_InputInvalidAccordingToValidationRule_LogErrorAndReturnFalse()
        {
            // Setup
            var failureMechanism = new TestCalculatableFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHrdFilePath);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
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

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IStructuresCalculationMessageProvider>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestStructuresCalculationService(messageProvider).Calculate(null, new GeneralTestInput(), CreateCalculationSettings());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IStructuresCalculationMessageProvider>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestStructuresCalculationService(messageProvider).Calculate(new TestStructuresCalculation(), null, CreateCalculationSettings());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("generalInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationSettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var messageProvider = mocks.Stub<IStructuresCalculationMessageProvider>();
            mocks.ReplayAll();

            // Call
            void Call() => new TestStructuresCalculationService(messageProvider).Calculate(new TestStructuresCalculation(), new GeneralTestInput(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationSettings", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidInput_InputPropertiesCorrectlySentToCalculator(bool usePreprocessorClosure)
        {
            // Setup
            var calculationSettings = new HydraulicBoundaryCalculationSettings(validHrdFilePath, validHlcdFilePath, usePreprocessorClosure);

            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(
                                         Arg<HydraRingCalculationSettings>.Is.NotNull))
                             .WhenCalled(invocation =>
                             {
                                 HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                     calculationSettings, (HydraRingCalculationSettings) invocation.Arguments[0]);
                             })
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validHrdFilePath)).Return(performedCalculationMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidStructuresCalculation_SetsOutputAndLogs(bool readIllustrationPoints)
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validHrdFilePath)).Return(performedCalculationMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButIllustrationPointsNull_IllustrationPointsNotSetAndLogs()
        {
            // Setup
            const string parserMessage = "Parser error";
            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsParserErrorMessage = parserMessage
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validHrdFilePath)).Return(performedCalculationMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputCalculateIllustrationPointsFalseAndIllustrationPointsParserErrorMessageNotNull_DoesNotLog()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsParserErrorMessage = "Parser error"
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validHrdFilePath)).Return(performedCalculationMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_ValidInputButIllustrationPointResultsOfIncorrectType_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validHrdFilePath)).Return(performedCalculationMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationRanErrorInSettingIllustrationPoints_IllustrationPointsNotSetAndLogsWarning()
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validHrdFilePath,
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validHrdFilePath)).Return(performedCalculationMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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

            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            var messageProvider = mocks.Stub<IStructuresCalculationMessageProvider>();
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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

            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                EndInFailure = endInFailure,
                LastErrorFileContent = lastErrorFileContent
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(null))
                             .IgnoreArguments()
                             .Return(calculator);

            const string calculationFailedMessage = "Calculation failed";
            const string calculationPerformedMessage = "Calculation performed";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();

            if (endInFailure && string.IsNullOrEmpty(lastErrorFileContent))
            {
                messageProvider.Expect(mp => mp.GetCalculationFailedMessage(calculation.Name)).Return(calculationFailedMessage);
            }
            else
            {
                messageProvider.Expect(mp => mp.GetCalculationFailedWithErrorReportMessage(calculation.Name,
                                                                                           endInFailure && string.IsNullOrEmpty(lastErrorFileContent)
                                                                                               ? calculator.HydraRingCalculationException.Message
                                                                                               : lastErrorFileContent
                                       )).Return(calculationFailedMessage);
            }

            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(calculator.OutputDirectory)).Return(calculationPerformedMessage);
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        private static HydraulicBoundaryCalculationSettings CreateCalculationSettings()
        {
            return new HydraulicBoundaryCalculationSettings(validHrdFilePath, validHlcdFilePath, false);
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

        private class GeneralTestInput
        {
            public double N => 0;
        }
    }
}