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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.Data.TestUtil;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactoryTest
    {
        private static void AssertCategoryCalculatorInput(IAssessmentSection assessmentSection,
                                                          AssemblyCategoriesCalculatorStub categoryCalculator,
                                                          GrassCoverErosionInwardsFailureMechanism failureMechanism)
        {
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, categoryCalculator.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, categoryCalculator.LowerLimitNorm);
            Assert.AreEqual(failureMechanism.Contribution, categoryCalculator.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.GeneralInput.N, categoryCalculator.N);
        }

        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(null);

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
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultValidityOnlyType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

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
                    GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

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
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AssembleDetailedAssembly_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssembly_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssembly_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssembly_WithInput_SetsInputOnCalculator()
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
                AssemblyCategoriesCalculatorStub categoryCalculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(failureMechanism, assessmentSection),
                                calculator.DetailedAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentProbabilityOnlyResultInput);
                AssertCategoryCalculatorInput(assessmentSection, categoryCalculator, failureMechanism);
                Assert.AreSame(categoryCalculator.FailureMechanismSectionCategoriesOutput, calculator.DetailedAssessmentCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssembly_AssemblyRan_ReturnsOutput()
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
                    GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
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
        public void AssembleDetailedAssembly_CalculatorThrowsException_ThrowsAssemblyException()
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
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
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

        #region Tailor Made Assessment

        [Test]
        public void AssembleTailorMadeAssembly_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssembly_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleTailorMadeAssembly_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssembly_WithInput_SetsInputOnCalculator()
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
                AssemblyCategoriesCalculatorStub categoryCalculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentProbability, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityCalculationResultInput);
                AssertCategoryCalculatorInput(assessmentSection, categoryCalculator, failureMechanism);
                Assert.AreSame(categoryCalculator.FailureMechanismSectionCategoriesOutput, calculator.TailorMadeAssessmentCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssembly_AssemblyRan_ReturnsOutput()
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
                    GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
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
        public void AssembleTailorMadeAssembly_CalculatorThrowsException_ThrowsAssemblyException()
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
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
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
        public void AssembleCombinedAssembly_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                null,
                new GrassCoverErosionInwardsFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssembly_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleCombinedAssembly_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                new GrassCoverErosionInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new GrassCoverErosionInwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleCombinedAssembly_WithInput_SetsInputOnCalculator()
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
                GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssembly expectedSimpleAssembly = GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);
                FailureMechanismSectionAssembly expectedDetailedAssembly = GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);
                FailureMechanismSectionAssembly expectedTailorMadeAssembly = GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
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
        public void AssembleCombinedAssembly_AssemblyRan_ReturnsOutput()
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
                    GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
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
        public void AssembleCombinedAssembly_CalculatorThrowsException_ThrowsAssemblyException()
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
                TestDelegate call = () => GrassCoverErosionInwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
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
    }
}