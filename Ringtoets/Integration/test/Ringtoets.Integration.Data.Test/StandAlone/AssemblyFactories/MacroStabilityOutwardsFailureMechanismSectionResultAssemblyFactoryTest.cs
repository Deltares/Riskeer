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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.AssemblyFactories;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.Test.StandAlone.AssemblyFactories
{
    [TestFixture]
    public class MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactoryTest
    {
        private static void AssertCategoryCalculatorInput(IAssessmentSection assessmentSection,
                                                          AssemblyCategoriesCalculatorStub categoryCalculator,
                                                          MacroStabilityOutwardsFailureMechanism failureMechanism)
        {
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, categoryCalculator.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, categoryCalculator.LowerLimitNorm);
            Assert.AreEqual(failureMechanism.Contribution, categoryCalculator.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.GetN(
                                failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput.SectionLength), categoryCalculator.N);
        }

        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(null);

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
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.SimpleAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssembly_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                AssemblyCategoriesCalculatorStub categoryCalculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.DetailedAssessmentResult, calculator.DetailedAssessmentResultInput);
                Assert.AreEqual(sectionResult.DetailedAssessmentProbability, calculator.DetailedAssessmentProbabilityInput);
                AssertCategoryCalculatorInput(assessmentSection, categoryCalculator, failureMechanism);
                Assert.AreSame(categoryCalculator.FailureMechanismSectionCategoriesOutput, calculator.DetailedAssessmentCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssembly_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.DetailedAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssembly_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    new MacroStabilityOutwardsFailureMechanism(),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleTailorMadeAssembly_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                AssemblyCategoriesCalculatorStub categoryCalculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.TailorMadeAssessmentProbability, calculator.TailorMadeAssessmentProbabilityInput);
                Assert.AreEqual(sectionResult.TailorMadeAssessmentResult, calculator.TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput);
                AssertCategoryCalculatorInput(assessmentSection, categoryCalculator, failureMechanism);
                Assert.AreSame(categoryCalculator.FailureMechanismSectionCategoriesOutput, calculator.TailorMadeAssessmentCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssembly_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.TailorMadeAssessmentAssemblyOutput;
                Assert.AreEqual(calculatorOutput.Group, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleTailorMadeAssembly_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                    sectionResult,
                    new MacroStabilityOutwardsFailureMechanism(),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                null,
                new MacroStabilityOutwardsFailureMechanism(),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
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
            TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                new MacroStabilityOutwardsFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleCombinedAssembly_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedSimpleAssembly = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(
                    sectionResult);
                FailureMechanismSectionAssemblyCategoryGroup expectedDetailedAssembly = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);
                FailureMechanismSectionAssemblyCategoryGroup expectedTailorMadeAssembly = MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleTailorMadeAssembly(
                    sectionResult,
                    failureMechanism,
                    assessmentSection);

                Assert.AreEqual(expectedSimpleAssembly, calculator.CombinedSimpleAssemblyGroupInput);
                Assert.AreEqual(expectedDetailedAssembly, calculator.CombinedDetailedAssemblyGroupInput);
                Assert.AreEqual(expectedTailorMadeAssembly, calculator.CombinedTailorMadeAssemblyGroupInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssembly_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyCategoryGroup actualOutput =
                    MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                        sectionResult,
                        failureMechanism,
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup? calculatorOutput = calculator.CombinedAssemblyCategoryOutput;
                Assert.AreEqual(calculatorOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleCombinedAssembly_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new MacroStabilityOutwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculateCombinedAssembly = true;

                // Call
                TestDelegate call = () => MacroStabilityOutwardsFailureMechanismSectionResultAssemblyFactory.AssembleCombinedAssembly(
                    sectionResult,
                    new MacroStabilityOutwardsFailureMechanism(),
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