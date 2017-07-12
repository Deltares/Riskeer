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
using Ringtoets.Common.Service.Structures;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => new TestStructuresCalculationService().Calculate(null,
                                                                                       assessmentSection,
                                                                                       failureMechanism,
                                                                                       string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new TestStructuresCalculation();
            var failureMechanism = new TestFailureMechanism();

            // Call
            TestDelegate test = () => new TestStructuresCalculationService().Calculate(calculation,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Calculate_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new TestStructuresCalculation();

            // Call
            TestDelegate test = () => new TestStructuresCalculationService().Calculate(calculation,
                                                                                       assessmentSection,
                                                                                       null,
                                                                                       string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_Always_InputPropertiesCorrectlySentToCalculator()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  failureMechanism,
                                  validFilePath);

                // Assert
                ExceedanceProbabilityCalculationInput[] calculationInputs = calculator.ReceivedInputs.ToArray();
                Assert.AreEqual(1, calculationInputs.Length);

                var expectedInput = new TestExceedanceProbabilityCalculationInput(1300001);
                ExceedanceProbabilityCalculationInput actualInput = calculationInputs[0];
                HydraRingDataEqualityHelper.AreEqual(expectedInput, actualInput);
                Assert.IsFalse(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculate_CancelCalculationWithValidInput_CancelsCalculatorAndHasNullOutput()
        {
            // Setup
            var failureMechanism = new TestFailureMechanism();

            var mockRepository = new MockRepository();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism,
                                                                                                           mockRepository);
            var calculator = new TestStructuresCalculator<ExceedanceProbabilityCalculationInput>();
            var calculatorFactory = mockRepository.StrictMock<IHydraRingCalculatorFactory>();
            calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<ExceedanceProbabilityCalculationInput>(testDataPath))
                             .Return(calculator);
            mockRepository.ReplayAll();

            var calculation = new TestStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = assessmentSectionStub.HydraulicBoundaryDatabase.Locations.First(hl => hl.Id == 1300001)
                }
            };

            using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
            {
                var service = new TestStructuresCalculationService();
                calculator.CalculationFinishedHandler += (s, e) => service.Cancel();

                // Call
                service.Calculate(calculation,
                                  assessmentSectionStub,
                                  failureMechanism,
                                  validFilePath);

                // Assert
                Assert.IsNull(calculation.Output);
                Assert.IsTrue(calculator.IsCanceled);
            }
            mockRepository.VerifyAll();
        }

        private class TestStructuresCalculationService : StructuresCalculationServiceBase<TestStructureValidationRulesRegistry,
            TestStructuresInput, TestStructure, TestFailureMechanism, ExceedanceProbabilityCalculationInput>
        {
            protected override ExceedanceProbabilityCalculationInput CreateInput(StructuresCalculation<TestStructuresInput> calculation,
                                                                                 TestFailureMechanism failureMechanism,
                                                                                 string hydraulicBoundaryDatabaseFilePath)
            {
                return new TestExceedanceProbabilityCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id);
            }

            protected override void PerformCalculation(IStructuresCalculator<ExceedanceProbabilityCalculationInput> calculator,
                                                       ExceedanceProbabilityCalculationInput input,
                                                       StructuresCalculation<TestStructuresInput> calculation,
                                                       IAssessmentSection assessmentSection,
                                                       TestFailureMechanism failureMechanism)
            {
                calculator.Calculate(input);
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
    }
}