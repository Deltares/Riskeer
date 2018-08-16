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
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.MessageProviders;
using Ringtoets.Common.Service.Structures;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Exceptions;
using Ringtoets.HydraRing.Calculation.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Calculation.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Service.Test.Structures
{
    [TestFixture]
    public class StructuresCalculationServiceBaseTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validPreprocessorDirectory = TestHelper.GetScratchPadPath();

        [Test]
        public void Constructor_MessageProviderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestStructuresCalculationService(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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
            TestDelegate test = () => TestStructuresCalculationService.Validate(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Validate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new TestStructuresCalculation();

            // Call
            TestDelegate test = () => TestStructuresCalculationService.Validate(calculation, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(),
                                                                                                       mocks,
                                                                                                       Path.Combine(testDataPath, "notexisting.sqlite"));
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            var isValid = true;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_ValidCalculationInvalidPreprocessorDirectory_LogsErrorAndReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                new TestFailureMechanism(), mocks, validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "NonExistingPreprocessorDirectory";

            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            var isValid = true;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", msgs[1]);
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(),
                                                                                                       mocks,
                                                                                                       Path.Combine(testDataPath, "HRD nosettings.sqlite"));
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(),
                                                                                                       mocks);
            mocks.ReplayAll();

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(new TestStructuresCalculation(), assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische randvoorwaardendatabase geïmporteerd.", msgs[1]);
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
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(),
                                                                                                       mocks,
                                                                                                       validFilePath);
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
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_CalculationWithoutStructuresValidHydraulicBoundaryDatabase_LogsErrorAndReturnsFalse()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
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
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
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
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
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
        public void Validate_ValidInputWithCanUseProcessorFalse_ReturnsTrue()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validFilePath);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    IsValid = true
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(isValid);

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ValidInputWithUseProcessorFalse_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                new TestFailureMechanism(), mocks, validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "NonExistingPreprocessorDirectory";

            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    IsValid = true
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(isValid);

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ValidInputWithUseProcessorTrue_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();

            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                new TestFailureMechanism(), mocks, validFilePath);

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = TestHelper.GetScratchPadPath();

            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    IsValid = true
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSection);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[1]);
            });
            Assert.IsTrue(isValid);

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
            TestDelegate test = () => new TestStructuresCalculationService(messageProvider)
                .Calculate(null, new GeneralTestInput(), string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
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

            var calculation = new TestStructuresCalculation();

            // Call
            TestDelegate test = () => new TestStructuresCalculationService(messageProvider)
                .Calculate(calculation, null, string.Empty, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Calculate_ValidInput_InputPropertiesCorrectlySentToCalculatorAndLogs(bool readIllustrationPoints)
        {
            // Setup
            var mocks = new MockRepository();
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                OutputDirectory = validFilePath,
                IllustrationPointsResult = new TestGeneralResult()
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validFilePath)).Return(performedCalculationMessage);
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
                Action call = () => service.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(3, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    Assert.AreEqual(performedCalculationMessage, msgs[1]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[2]);
                });

                ExceedanceProbabilityCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                var expectedInput = new TestExceedanceProbabilityCalculationInput(hydraulicBoundaryLocation.Id);
                ExceedanceProbabilityCalculationInput actualInput = calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsNotNull(calculation.Output);
                Assert.IsFalse(calculator.IsCanceled);
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
                OutputDirectory = validFilePath,
                IllustrationPointsParserErrorMessage = parserMessage
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validFilePath)).Return(performedCalculationMessage);
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
                Action call = () => service.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
                OutputDirectory = validFilePath,
                IllustrationPointsParserErrorMessage = "Parser error"
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validFilePath)).Return(performedCalculationMessage);
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
                Action call = () => service.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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
                OutputDirectory = validFilePath,
                IllustrationPointsResult = TestGeneralResult.CreateGeneralResultWithSubMechanismIllustrationPoints()
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validFilePath)).Return(performedCalculationMessage);
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
                Action call = () => service.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
                OutputDirectory = validFilePath,
                IllustrationPointsResult = GeneralResultTestFactory.CreateGeneralResultWithDuplicateStochasts()
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);

            const string performedCalculationMessage = "Calculation successful";
            var messageProvider = mocks.StrictMock<IStructuresCalculationMessageProvider>();
            messageProvider.Expect(mp => mp.GetCalculationPerformedMessage(validFilePath)).Return(performedCalculationMessage);
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
                Action call = () => service.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, messages =>
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
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
                             .Return(calculator);

            var messageProvider = mocks.Stub<IStructuresCalculationMessageProvider>();
            mocks.ReplayAll();

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(messageProvider);
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);

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
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath, validPreprocessorDirectory))
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
                Action call = () =>
                {
                    try
                    {
                        structuresCalculationService.Calculate(calculation, new GeneralTestInput(), validFilePath, validPreprocessorDirectory);
                    }
                    catch (HydraRingCalculationException)
                    {
                        exceptionThrown = true;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
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

        private class TestStructuresCalculationService : StructuresCalculationServiceBase<TestStructureValidationRulesRegistry,
            TestStructuresInput, TestStructure, GeneralTestInput, ExceedanceProbabilityCalculationInput>
        {
            public TestStructuresCalculationService(IStructuresCalculationMessageProvider messageProvider) : base(messageProvider) {}

            protected override ExceedanceProbabilityCalculationInput CreateInput(TestStructuresInput structureInput,
                                                                                 GeneralTestInput generalInput,
                                                                                 string hydraulicBoundaryDatabaseFilePath,
                                                                                 bool usePreprocessor)
            {
                return new TestExceedanceProbabilityCalculationInput(structureInput.HydraulicBoundaryLocation.Id);
            }
        }

        private class TestExceedanceProbabilityCalculationInput : ExceedanceProbabilityCalculationInput
        {
            public TestExceedanceProbabilityCalculationInput(long hydraulicBoundaryLocationId) : base(hydraulicBoundaryLocationId) {}

            public override HydraRingFailureMechanismType FailureMechanismType { get; }

            public override int VariableId { get; }

            public override HydraRingSection Section
            {
                get
                {
                    return new HydraRingSection(0, 0, 0);
                }
            }
        }

        private class GeneralTestInput
        {
            public double N
            {
                get
                {
                    return 0;
                }
            }
        }
    }
}