﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Data.TestUtil;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismAssemblyFactoryTest
    {
        private static void AssertAssemblyCategoriesInput(IAssessmentSection assessmentSection,
                                                          GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                          AssemblyCategoriesInput assemblyCategoriesInput)
        {
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, assemblyCategoriesInput.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, assemblyCategoriesInput.LowerLimitNorm);
            Assert.AreEqual(failureMechanism.Contribution, assemblyCategoriesInput.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.GeneralInput.N, assemblyCategoriesInput.N);
        }

        #region Simple Assembly

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(null);

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
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentValidityOnlyInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssembly actualOutput =
                    GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.SimpleAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(sectionResult);

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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                DetailedAssessmentResult = new Random(39).NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(failureMechanism, assessmentSection),
                                calculator.DetailedAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
                AssertAssemblyCategoriesInput(assessmentSection, failureMechanism, calculator.AssemblyCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssembly actualOutput =
                    GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                        sectionResult,
                        new GrassCoverErosionInwardsFailureMechanism(),
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.DetailedAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    new GrassCoverErosionInwardsFailureMechanism(),
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
                mocks.VerifyAll();
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentProbability, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
                AssertAssemblyCategoriesInput(assessmentSection, failureMechanism, calculator.AssemblyCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssembly actualOutput =
                    GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.TailorMadeAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult,
                    new GrassCoverErosionInwardsFailureMechanism(),
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable)]
        public void AssembleCombinedAssessment_WithVariousSimpleAssessmentInputAssemblesWithAllInformation_SetsInputOnCalculator(
            SimpleAssessmentValidityOnlyResultType simpleAssessmentResult)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = simpleAssessmentResult
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedSimpleAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);
                FailureMechanismSectionAssembly expectedDetailedAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);
                FailureMechanismSectionAssembly expectedTailorMadeAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                AssemblyToolTestHelper.AssertAreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(expectedDetailedAssembly, calculator.CombinedDetailedAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(expectedTailorMadeAssembly, calculator.CombinedTailorMadeAssemblyInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssessment_WithInputSimpleAssessmentNotApplicable_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedSimpleAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);

                AssemblyToolTestHelper.AssertAreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyInput);
                Assert.IsNull(calculator.CombinedDetailedAssemblyInput);
                Assert.IsNull(calculator.CombinedTailorMadeAssemblyInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssembly actualOutput =
                    GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.CombinedAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssessment_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult,
                    new GrassCoverErosionInwardsFailureMechanism(),
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
                assessmentSection);

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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedSimpleAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);
                FailureMechanismSectionAssembly expectedDetailedAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleDetailedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);
                FailureMechanismSectionAssembly expectedTailorMadeAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleTailorMadeAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                AssemblyToolTestHelper.AssertAreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(expectedDetailedAssembly, calculator.CombinedDetailedAssemblyInput);
                AssemblyToolTestHelper.AssertAreEqual(expectedTailorMadeAssembly, calculator.CombinedTailorMadeAssemblyInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInput_SetsInputOnCalculator()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssemblyProbability = true,
                ManualAssemblyProbability = new Random(39).NextDouble()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.ManualAssemblyProbability, calculator.ManualAssemblyProbabilityInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithoutManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);
                Assert.AreEqual(categoryGroup, expectedAssembly.Group);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void GetSectionAssemblyCategoryGroup_WithManualInput_ReturnsOutput()
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssemblyProbability = true,
                ManualAssemblyProbability = new Random(39).NextDouble()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup categoryGroup = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedAssembly = calculator.AssembleManual(
                    sectionResult.ManualAssemblyProbability,
                    AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(0.0, failureMechanism, assessmentSection));
                Assert.AreEqual(categoryGroup, expectedAssembly.Group);
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetSectionAssemblyCategoryGroup_CalculatorThrowsException_ThrowsAssemblyException(bool useManualAssembly)
        {
            // Setup
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                UseManualAssemblyProbability = useManualAssembly
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                if (useManualAssembly)
                {
                    calculator.ThrowExceptionOnCalculate = true;
                }
                else
                {
                    calculator.ThrowExceptionOnCalculateCombinedAssembly = true;
                }

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.GetSectionAssemblyCategoryGroup(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
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
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismIsNotRelevant_ReturnsNotApplicableAssembly()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                IsRelevant = false
            };

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            // Call
            FailureMechanismAssembly assembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                failureMechanism,
                assessmentSection);

            // Assert
            AssemblyToolTestHelper.AssertAreEqual(FailureMechanismAssemblyResultFactory.CreateNotApplicableAssembly(), assembly);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_WithoutManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedAssembly = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleCombinedAssessment(
                    failureMechanism.SectionResults.Single(),
                    failureMechanism,
                    assessmentSection);
                AssemblyToolTestHelper.AssertAreEqual(expectedAssembly, calculator.FailureMechanismSectionAssemblies.Single());
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithManualInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });
            GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssemblyProbability = true;
            sectionResult.ManualAssemblyProbability = new Random(39).NextDouble();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                FailureMechanismSectionAssembly expectedAssembly = sectionCalculator.AssembleManual(sectionResult.ManualAssemblyProbability,
                                                                                                    AssemblyCategoriesInputFactory.CreateAssemblyCategoriesInput(
                                                                                                        failureMechanism.GeneralInput.N,
                                                                                                        failureMechanism,
                                                                                                        assessmentSection));
                AssemblyToolTestHelper.AssertAreEqual(expectedAssembly, calculator.FailureMechanismSectionAssemblies.Single());
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                FailureMechanismAssembly actualOutput =
                    GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                        failureMechanism,
                        assessmentSection);

                // Assert
                Assert.AreSame(calculator.FailureMechanismAssemblyOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismCalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
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
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            });

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    failureMechanism,
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyException>(innerException);
                Assert.AreEqual("Voor een of meerdere vakken kan geen resultaat worden bepaald.", exception.Message);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}