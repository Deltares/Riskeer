﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Data.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;

namespace Riskeer.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismAssemblyFactoryOldTest
    {
        private static void AssertAssemblyCategoriesInput(IAssessmentSection assessmentSection,
                                                          PipingFailureMechanism failureMechanism,
                                                          AssemblyCategoriesInput assemblyCategoriesInput)
        {
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, assemblyCategoriesInput.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, assemblyCategoriesInput.LowerLimitNorm);
            Assert.AreEqual(failureMechanism.Contribution, assemblyCategoriesInput.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length), assemblyCategoriesInput.N);
        }

        #region Simple Assembly

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleSimpleAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleSimpleAssessment_WithSectionResult_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResultOld(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResultOld(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyOld actualOutput =
                    PipingFailureMechanismAssemblyFactoryOld.AssembleSimpleAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssemblyOld calculatorOutput = calculator.SimpleAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResultOld(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleSimpleAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Detailed Assembly

        [Test]
        public void AssembleDetailedAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                null,
                Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssessment_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssessment_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssessment_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                    sectionResult,
                    Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(
                                    Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                                    assessmentSection.FailureMechanismContribution.Norm),
                                calculator.DetailedAssessmentProbabilityInput);
                Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.GetN(sectionResult.Section.Length),
                                calculator.DetailedAssessmentFailureMechanismSectionNInput);
                AssertAssemblyCategoriesInput(assessmentSection, failureMechanism, calculator.DetailedAssessmentAssemblyCategoriesInput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyOld actualOutput =
                    PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                        sectionResult,
                        new[]
                        {
                            SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                                hydraulicBoundaryLocation)
                        },
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyOld calculatorOutput = calculator.DetailedAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            }, true);

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleDetailedAssessment(
                    sectionResult,
                    new[]
                    {
                        SemiProbabilisticPipingCalculationTestFactory.CreateCalculationWithValidInput<SemiProbabilisticPipingCalculationScenario>(
                            hydraulicBoundaryLocation)
                    },
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Tailor Made Assembly

        [Test]
        public void AssembleTailorMadeAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleTailorMadeAssessment(
                null,
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssessment_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleTailorMadeAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleTailorMadeAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new PipingFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentProbability, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
                Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.GetN(sectionResult.Section.Length),
                                calculator.TailorMadeAssessmentFailureMechanismSectionNInput);
                AssertAssemblyCategoriesInput(assessmentSection, failureMechanism, calculator.TailorMadeAssessmentAssemblyCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyOld actualOutput =
                    PipingFailureMechanismAssemblyFactoryOld.AssembleTailorMadeAssessment(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyOld calculatorOutput = calculator.TailorMadeAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
                mocks.VerifyAll();
            }
        }

        #endregion

        #region Combined Assembly

        [Test]
        public void AssembleCombinedAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                null,
                Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssessment_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssessment_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssessment_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithAllInformation_SetsInputOnCalculator(SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                    sectionResult,
                    Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                    failureMechanism,
                    assessmentSection);

                // Assert
                AssemblyToolTestHelper.AssertAreEqual(calculator.SimpleAssessmentAssemblyOutput, calculator.CombinedSimpleAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(calculator.DetailedAssessmentAssemblyOutput, calculator.CombinedDetailedAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(calculator.TailorMadeAssessmentAssemblyOutput, calculator.CombinedTailorMadeAssemblyInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithOnlySimpleAssessmentInput_SetsInputOnCalculator(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                    sectionResult,
                    Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                    failureMechanism,
                    assessmentSection);

                // Assert
                AssemblyToolTestHelper.AssertAreEqual(calculator.SimpleAssessmentAssemblyOutput, calculator.CombinedSimpleAssemblyInput);
                Assert.IsNull(calculator.CombinedDetailedAssemblyInput);
                Assert.IsNull(calculator.CombinedTailorMadeAssemblyInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyOld actualOutput =
                    PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                        sectionResult,
                        Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                        failureMechanism,
                        assessmentSection);

                // Assert
                Assert.AreSame(calculator.CombinedAssemblyOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleCombinedAssessment(
                    sectionResult,
                    Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
                mocks.VerifyAll();
            }
        }

        #endregion

        #region GetSectionAssemblyCategoryGroup

        [Test]
        public void GetSectionAssemblyCategoryGroup_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                null,
                new PipingFailureMechanism(),
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new PipingFailureMechanism(),
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                AssemblyToolTestHelper.AssertAreEqual(calculator.SimpleAssessmentAssemblyOutput, calculator.CombinedSimpleAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(calculator.DetailedAssessmentAssemblyOutput, calculator.CombinedDetailedAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(calculator.TailorMadeAssessmentAssemblyOutput, calculator.CombinedTailorMadeAssemblyInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualTrue_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(39);
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyProbability = random.NextDouble(),
                TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };

            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    true);

                // Assert
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                Assert.AreEqual(0.0, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual((TailorMadeAssessmentProbabilityCalculationResultType) 0, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualFalse_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(39);
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyProbability = random.NextDouble(),
                TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                TailorMadeAssessmentProbability = random.NextDouble()
            };

            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    false);

                // Assert
                Assert.AreEqual(0.0, calculator.ManualAssemblyProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentProbability, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(calculator.CombinedAssemblyOutput.Group, categoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualTrue_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyProbability = new Random(39).NextDouble()
            };

            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    true);

                // Assert
                Assert.AreEqual(calculator.ManualAssemblyAssemblyOutput.Group, categoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualFalse_ReturnsOutput()
        {
            // Setup
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyProbability = new Random(39).NextDouble()
            };

            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    false);

                // Assert
                Assert.AreEqual(calculator.CombinedAssemblyOutput.Group, categoryGroup);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetSectionAssemblyCategoryGroup_CalculatorThrowsException_ThrowsAssemblyException(bool useManualAssembly)
        {
            // Setup
            var sectionResult = new PipingFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = useManualAssembly
            };
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                if (useManualAssembly)
                {
                    calculator.ThrowExceptionOnCalculate = true;
                }
                else
                {
                    calculator.ThrowExceptionOnCalculateCombinedAssembly = true;
                }

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    useManualAssembly);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
                mocks.VerifyAll();
            }
        }

        #endregion

        #region Failure Mechanism Assembly

        [Test]
        public void AssembleFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                null,
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                new PipingFailureMechanism(),
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismNotInAssembly_ReturnsNotApplicableAssembly()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                InAssembly = false
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Call
            FailureMechanismAssembly assembly = PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                failureMechanism,
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            AssemblyToolTestHelper.AssertAreEqual(FailureMechanismAssemblyResultFactoryOld.CreateNotApplicableAssembly(), assembly);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorOldStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                AssemblyToolTestHelper.AssertAreEqual(sectionCalculator.CombinedAssemblyOutput, calculator.FailureMechanismSectionAssemblies.Single());
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputAndUseManualTrue_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyProbability = new Random(39).NextDouble();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorOldStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection,
                    true);

                // Assert
                AssemblyToolTestHelper.AssertAreEqual(sectionCalculator.ManualAssemblyAssemblyOutput, calculator.FailureMechanismSectionAssemblies.Single());
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputAndUseManualFalse_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            PipingFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyProbability = new Random(39).NextDouble();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorOldStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection,
                    false);

                // Assert
                AssemblyToolTestHelper.AssertAreEqual(sectionCalculator.CombinedAssemblyOutput, calculator.FailureMechanismSectionAssemblies.Single());
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                FailureMechanismAssembly actualOutput =
                    PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                        failureMechanism,
                        assessmentSection,
                        new Random(39).NextBoolean());

                // Assert
                Assert.AreSame(calculator.FailureMechanismAssemblyOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismSectionCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                void Call() => PipingFailureMechanismAssemblyFactoryOld.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere vakken kan geen resultaat worden bepaald.", exception.Message);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}