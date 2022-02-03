// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;
using AssemblyFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new FailureMechanismSectionAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanismSection_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new FailureMechanismSectionAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.AssembleFailureMechanismSection(null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("input"));
        }

        [Test]
        [TestCase(false, true, ESectionInitialMechanismProbabilitySpecification.NotRelevant)]
        [TestCase(false, false, ESectionInitialMechanismProbabilitySpecification.NotRelevant)]
        [TestCase(true, true, ESectionInitialMechanismProbabilitySpecification.RelevantWithProbabilitySpecification)]
        [TestCase(true, false, ESectionInitialMechanismProbabilitySpecification.RelevantNoProbabilitySpecification)]
        public void AssembleFailureMechanismSection_WithValidInput_InputCorrectlySendToKernel(
            bool isRelevant, bool hasProbabilitySpecified,
            ESectionInitialMechanismProbabilitySpecification expectedInitialMechanismProbabilitySpecification)
        {
            // Setup
            const double signalingNorm = 0.0001;
            const double lowerLimitNorm = 0.001;

            var random = new Random(21);
            var input = new FailureMechanismSectionWithProfileProbabilityAssemblyInput(lowerLimitNorm, signalingNorm,
                                                                                       isRelevant, hasProbabilitySpecified,
                                                                                       random.NextDouble(), random.NextDouble(),
                                                                                       random.NextBoolean(),
                                                                                       random.NextDouble(), random.NextDouble());
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> categoryLimits = CreateCategoryLimits();
                categoryLimitsKernel.CategoryLimits = categoryLimits;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResult = new AssemblyFailureMechanismSectionAssemblyResult(
                    new Probability(random.NextDouble(0.0, 0.01)), new Probability(random.NextDouble(0.01, 0.02)), random.NextEnumValue<EInterpretationCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanismSection(input);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(lowerLimitNorm, assessmentSection.FailureProbabilityLowerLimit);
                ProbabilityAssert.AreEqual(signalingNorm, assessmentSection.FailureProbabilitySignallingLimit);

                Assert.AreSame(categoryLimits, failureMechanismSectionAssemblyKernel.Categories);
                Assert.AreEqual(expectedInitialMechanismProbabilitySpecification, failureMechanismSectionAssemblyKernel.InitialMechanismProbabilitySpecification);
                Assert.AreEqual(input.InitialProfileProbability, failureMechanismSectionAssemblyKernel.ProbabilityInitialMechanismProfile);
                Assert.AreEqual(input.InitialSectionProbability, failureMechanismSectionAssemblyKernel.ProbabilityInitialMechanismSection);
                Assert.AreEqual(input.FurtherAnalysisNeeded, failureMechanismSectionAssemblyKernel.NeedsRefinement);
                Assert.AreEqual(input.RefinedProfileProbability, failureMechanismSectionAssemblyKernel.RefinedProbabilityProfile);
                Assert.AreEqual(input.RefinedSectionProbability, failureMechanismSectionAssemblyKernel.RefinedProbabilitySection);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.CategoryLimits = CreateCategoryLimits();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var kernelResult = new AssemblyFailureMechanismSectionAssemblyResult(new Probability(random.NextDouble()),
                                                                                     new Probability(random.NextDouble()),
                                                                                     random.NextEnumValue<EInterpretationCategory>());
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResult = kernelResult;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                RiskeerFailureMechanismSectionAssemblyResult result = calculator.AssembleFailureMechanismSection(input);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);

                Assert.AreEqual(kernelResult.ProbabilityProfile, result.ProfileProbability);
                Assert.AreEqual(kernelResult.ProbabilitySection, result.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyResultCreator.CreateFailureMechanismSectionAssemblyGroup(kernelResult.InterpretationCategory),
                                result.AssemblyGroup);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelWithInvalidOutput_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.CategoryLimits = CreateCategoryLimits();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var kernelResult = new AssemblyFailureMechanismSectionAssemblyResult(new Probability(random.NextDouble()),
                                                                                     new Probability(random.NextDouble()),
                                                                                     (EInterpretationCategory) 99);
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResult = kernelResult;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(input);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);

                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelThrowsException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.ThrowExceptionOnCalculate = true;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(input);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);

                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelThrowsAssemblyException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.ThrowAssemblyExceptionOnCalculate = true;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(input);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);

                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        private static FailureMechanismSectionWithProfileProbabilityAssemblyInput CreateFailureMechanismSectionAssemblyInput()
        {
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            var random = new Random(21);
            return new FailureMechanismSectionWithProfileProbabilityAssemblyInput(lowerLimitNorm, signalingNorm,
                                                                                  random.NextBoolean(), random.NextBoolean(),
                                                                                  random.NextDouble(), random.NextDouble(),
                                                                                  random.NextBoolean(), random.NextDouble(), random.NextDouble());
        }

        private static CategoriesList<InterpretationCategory> CreateCategoryLimits()
        {
            return new CategoriesList<InterpretationCategory>(new[]
            {
                new InterpretationCategory(EInterpretationCategory.I, new Probability(0), new Probability(1))
            });
        }
    }
}