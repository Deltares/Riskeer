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
using System.ComponentModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
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
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;
using Riskeer.Integration.Data.StandAlone.SectionResults;

namespace Riskeer.Integration.Data.Test.StandAlone.AssemblyFactories
{
    [TestFixture]
    public class MacroStabilityOutwardsFailureMechanismAssemblyFactoryTest
    {
        #region Simple Assembly

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleSimpleAssessment_WithSectionResult_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssessment_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism
            {
                Contribution = random.NextDouble(0, 100)
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
                Assert.AreEqual(sectionResult.DetailedAssessmentProbability, calculator.DetailedAssessmentProbabilityInput);

                Assert.AreEqual(assessmentSection.FailureMechanismContribution.Norm, calculator.DetailedAssessmentNormativeNormInput);
                double expectedN = failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.GetN(
                    assessmentSection.ReferenceLine.Length);
                Assert.AreEqual(expectedN, calculator.DetailedAssessmentFailureMechanismNInput);
                Assert.AreEqual(failureMechanism.Contribution / 100, calculator.DetailedAssessmentFailureMechanismContribution);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyOutput.Group, actualOutput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }

            mocks.VerifyAll();
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(failureMechanismSection)
            {
                TailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>()
            };

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism
            {
                Contribution = random.NextDouble(0, 100)
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentProbability, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput);
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.Norm, calculator.TailorMadeAssessmentNormativeNormInput);
                double expectedN = failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length);
                Assert.AreEqual(expectedN, calculator.TailorMadeAssessmentFailureMechanismNInput);
                Assert.AreEqual(failureMechanism.Contribution / 100, calculator.TailorMadeAssessmentFailureMechanismContributionInput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                Assert.AreEqual(calculator.TailorMadeAssessmentAssemblyOutput.Group, actualOutput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }

            mocks.VerifyAll();
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssessment_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None)]
        [TestCase(SimpleAssessmentResultType.AssessFurther)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithAllInformation_SetsInputOnCalculator(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult, failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyOutput.Group, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(calculator.TailorMadeAssessmentAssemblyOutput.Group, calculator.CombinedTailorMadeAssemblyGroupInput);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.NotApplicable)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithOnlySimpleAssessmentInput_SetsInputOnCalculator(
            SimpleAssessmentResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(sectionResult, failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual((FailureMechanismSectionAssemblyCategoryGroup) 0, calculator.CombinedTailorMadeAssemblyGroupInput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.CombinedAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }

            mocks.VerifyAll();
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(calculator.SimpleAssessmentAssemblyOutput.Group, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(calculator.DetailedAssessmentAssemblyOutput.Group, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(calculator.TailorMadeAssessmentAssemblyOutput.Group, calculator.CombinedTailorMadeAssemblyGroupInput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, categoryGroup);
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualTrue_ReturnsOutput()
        {
            // Setup
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>()
            };

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Call
            FailureMechanismSectionAssemblyCategoryGroup categoryGroup = MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                sectionResult,
                failureMechanism,
                assessmentSection,
                true);

            // Assert
            Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                            categoryGroup);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInputAndUseManualFalse_ReturnsOutput()
        {
            // Setup
            var random = new Random(39);
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>()
            };

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    false);

                // Assert
                Assert.AreEqual(calculator.CombinedAssemblyCategoryOutput, categoryGroup);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInputAndCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithUseManualAndUseManualInputTrueAndInvalidManualFailureMechanismSectionAssemblyCategoryGroup_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = (ManualFailureMechanismSectionAssemblyCategoryGroup) 99
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                sectionResult,
                failureMechanism,
                assessmentSection,
                true);

            // Assert
            var exception = Assert.Throws<AssemblyException>(call);
            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
            Assert.AreEqual(innerException.Message, exception.Message);
            mocks.VerifyAll();
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                null,
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                new MacroStabilityOutwardsFailureMechanism(),
                null,
                new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismNotInAssembly_ReturnsNotApplicableCategory()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism
            {
                InAssembly = false
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Call
            FailureMechanismAssemblyCategoryGroup category = MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                failureMechanism,
                assessmentSection,
                new Random(39).NextBoolean());

            // Assert
            Assert.AreEqual(FailureMechanismAssemblyResultFactory.CreateNotApplicableCategory(), category);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism,
                                                                                               assessmentSection,
                                                                                               new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(sectionCalculator.CombinedAssemblyCategoryOutput, calculator.FailureMechanismSectionCategories.Single());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputAndUseManualTrue_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MacroStabilityOutwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = new Random(39).NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism,
                                                                                               assessmentSection,
                                                                                               true);

                // Assert
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                calculator.FailureMechanismSectionCategories.Single());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInputAndUseManualFalse_SetsCombinedInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            MacroStabilityOutwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.IIv;

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism,
                                                                                               assessmentSection,
                                                                                               false);

                // Assert
                Assert.AreEqual(sectionCalculator.CombinedAssemblyCategoryOutput, calculator.FailureMechanismSectionCategories.Single());
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                FailureMechanismAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism,
                                                                                                   assessmentSection,
                                                                                                   new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(calculator.FailureMechanismAssemblyCategoryGroupOutput, actualOutput);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection,
                    new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismSectionCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism,
                                                                                                                         assessmentSection,
                                                                                                                         new Random(39).NextBoolean());

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere vakken kan geen resultaat worden bepaald.", exception.Message);
            }

            mocks.VerifyAll();
        }

        #endregion
    }
}