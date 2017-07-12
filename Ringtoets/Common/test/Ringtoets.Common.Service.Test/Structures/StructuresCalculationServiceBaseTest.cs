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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
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

namespace Ringtoets.Common.Service.Test.Structures
{
    [TestFixture]
    public class StructuresCalculationServiceBaseTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
        private static readonly string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");

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
        public void Validate_ValidCalculationInvalidHydraulicBoundaryDatabase_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(), mocks);
            mocks.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "notexisting.sqlite");

            var calculation = new TestStructuresCalculation();

            var isValid = true;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_ValidCalculationValidHydraulicBoundaryDatabaseNoSettings_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(), mocks);
            mocks.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = Path.Combine(testDataPath, "HRD nosettings.sqlite");

            var calculation = new TestStructuresCalculation();

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                StringAssert.StartsWith("Validatie mislukt: Fout bij het lezen van bestand", msgs[1]);
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
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(new TestFailureMechanism(), mocks);
            mocks.ReplayAll();

            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;

            const string name = "<very nice name>";

            var calculation = new TestStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    Structure = new TestStructure()
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
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
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mocks.ReplayAll();

            const string name = "<a very nice name>";
            var calculation = new TestStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation()
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Er is geen kunstwerk geselecteerd.", msgs[1]);
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
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mocks.ReplayAll();

            const string name = "<a very nice name>";
            var calculation = new TestStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    IsValid = false
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSectionStub);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
                Assert.AreEqual("Validatie mislukt: Error message", msgs[1]);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[2]);
            });
            Assert.IsFalse(isValid);

            mocks.VerifyAll();
        }

        [Test]
        public void Validate_InputValidAccordingToValidationRule_ReturnTrue()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            assessmentSectionStub.HydraulicBoundaryDatabase.FilePath = validFilePath;
            mocks.ReplayAll();

            const string name = "<a very nice name>";
            var calculation = new TestStructuresCalculation
            {
                Name = name,
                InputParameters =
                {
                    Structure = new TestStructure(),
                    HydraulicBoundaryLocation = new TestHydraulicBoundaryLocation(),
                    IsValid = true
                }
            };

            var isValid = false;

            // Call
            Action call = () => isValid = TestStructuresCalculationService.Validate(calculation, assessmentSectionStub);

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
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks);
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new TestStructuresCalculationService(new TestMessageProvider()).Calculate(null,
                                                                                                                new GeneralTestInput(),
                                                                                                                0,
                                                                                                                assessmentSection.FailureMechanismContribution.Norm,
                                                                                                                failureMechanism.Contribution,
                                                                                                                string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_GeneralInputNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                       mocks);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            // Call
            TestDelegate test = () => new TestStructuresCalculationService(new TestMessageProvider()).Calculate(calculation,
                                                                                                                null,
                                                                                                                0,
                                                                                                                assessmentSection.FailureMechanismContribution.Norm,
                                                                                                                failureMechanism.Contribution,
                                                                                                                string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalInput", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySentToCalculator()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(new TestMessageProvider());

                // Call
                service.Calculate(calculation,
                                  new GeneralTestInput(),
                                  0,
                                  assessmentSectionStub.FailureMechanismContribution.Norm,
                                  failureMechanism.Contribution,
                                  validFilePath);

                // Assert
                ExceedanceProbabilityCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                var expectedInput = new TestExceedanceProbabilityCalculationInput(1300001);
                ExceedanceProbabilityCalculationInput actualInput = calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>();
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService(new TestMessageProvider());
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  new GeneralTestInput(),
                                  0,
                                  assessmentSectionStub.FailureMechanismContribution.Norm,
                                  failureMechanism.Contribution,
                                  validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var generalInput = new GeneralTestInput();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                LastErrorFileContent = "An error occurred",
                EndInFailure = true
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new TestStructuresCalculationService(new TestMessageProvider()).Calculate(calculation,
                                                                                                  generalInput,
                                                                                                  generalInput.N,
                                                                                                  assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                                  failureMechanism.Contribution,
                                                                                                  validFilePath);
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
                    Assert.AreEqual($"Calculation '{calculation.Name}' failed with report 'An error occurred'.", msgs[1]);
                    Assert.AreEqual("Calculation performed in directory ''.", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithExceptionAndNoLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var generalInput = new GeneralTestInput();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                EndInFailure = true
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new TestStructuresCalculationService(new TestMessageProvider()).Calculate(calculation,
                                                                                                  generalInput,
                                                                                                  generalInput.N,
                                                                                                  assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                                  failureMechanism.Contribution,
                                                                                                  validFilePath);
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
                    Assert.AreEqual($"Calculation '{calculation.Name}' failed.", msgs[1]);
                    StringAssert.StartsWith("Calculation performed in directory ''", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculationFailedWithoutExceptionAndWithLastErrorPresent_LogErrorAndThrowException()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test section", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1)
            }));

            var generalInput = new GeneralTestInput();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mocks);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>
            {
                EndInFailure = false,
                LastErrorFileContent = "An error occurred"
            };
            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var exceptionThrown = false;
                string exceptionMessage = string.Empty;

                // Call
                Action call = () =>
                {
                    try
                    {
                        new TestStructuresCalculationService(new TestMessageProvider()).Calculate(calculation,
                                                                                                  generalInput,
                                                                                                  generalInput.N,
                                                                                                  assessmentSectionStub.FailureMechanismContribution.Norm,
                                                                                                  failureMechanism.Contribution,
                                                                                                  validFilePath);
                    }
                    catch (HydraRingCalculationException e)
                    {
                        exceptionThrown = true;
                        exceptionMessage = e.Message;
                    }
                };

                // Assert
                TestHelper.AssertLogMessages(call, messages =>
                {
                    string[] msgs = messages.ToArray();
                    Assert.AreEqual(4, msgs.Length);
                    CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
                    StringAssert.StartsWith($"Calculation '{calculation.Name}' failed with report 'An error occurred'.", msgs[1]);
                    StringAssert.StartsWith("Calculation performed in directory ''", msgs[2]);
                    CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[3]);
                });
                Assert.IsTrue(exceptionThrown);
                Assert.IsNull(calculation.Output);
                Assert.AreEqual(calculator.LastErrorFileContent, exceptionMessage);
            }
            mocks.VerifyAll();
        }

        private class TestStructuresCalculationService : StructuresCalculationServiceBase<TestStructureValidationRulesRegistry,
            TestStructuresInput, TestStructure, GeneralTestInput, ExceedanceProbabilityCalculationInput>
        {
            public TestStructuresCalculationService(IStructuresCalculationMessageProvider messageProvider) : base(messageProvider) {}

            protected override ExceedanceProbabilityCalculationInput CreateInput(StructuresCalculation<TestStructuresInput> calculation,
                                                                                 GeneralTestInput generalInput,
                                                                                 string hydraulicBoundaryDatabaseFilePath)
            {
                return new TestExceedanceProbabilityCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id);
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

        private class TestMessageProvider : IStructuresCalculationMessageProvider
        {
            public string GetCalculationFailedMessage(string calculationSubject)
            {
                return $"Calculation '{calculationSubject}' failed.";
            }

            public string GetCalculationFailedWithErrorReportMessage(string calculationSubject, string errorReport)
            {
                return $"Calculation '{calculationSubject}' failed with report '{errorReport}'.";
            }

            public string GetCalculationPerformedMessage(string outputDirectory)
            {
                return $"Calculation performed in directory '{outputDirectory}'.";
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