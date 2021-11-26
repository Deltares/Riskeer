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
using Assembly.Kernel.Model.FailurePathSections;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Exceptions;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorTest
    {
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
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanismSection_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new FailureMechanismSectionAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.AssembleFailureMechanismSection(random.NextDouble(), random.NextDouble(), null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("input"));
        }

        [Test]
        public void AssembleFailureMechanismSection_WithValidInput_InputCorrectlySendToKernel()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            var random = new Random(21);
            var input = new FailureMechanismSectionAssemblyInput(random.NextBoolean(),
                                                                 random.NextDouble(), random.NextDouble(),
                                                                 random.NextBoolean(),
                                                                 random.NextDouble(), random.NextDouble());
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> categoryLimits = CreateCategoryLimits();
                categoryLimitsKernel.CategoryLimits = categoryLimits;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                failureMechanismSectionAssemblyKernel.FailurePathSectionAssemblyResult = new FailurePathSectionAssemblyResult(new Probability(random.NextDouble()),
                                                                                                                              new Probability(random.NextDouble()),
                                                                                                                              random.NextEnumValue<EInterpretationCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanismSection(lowerLimitNorm, signalingNorm, input);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(lowerLimitNorm, assessmentSection.FailureProbabilityLowerLimit);
                ProbabilityAssert.AreEqual(signalingNorm, assessmentSection.FailureProbabilitySignallingLimit);

                Assert.AreSame(categoryLimits, failureMechanismSectionAssemblyKernel.Categories);
                Assert.AreEqual(input.IsRelevant, failureMechanismSectionAssemblyKernel.IsRelevant);
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
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            var random = new Random(21);
            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                categoryLimitsKernel.CategoryLimits = CreateCategoryLimits();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var kernelResult = new FailurePathSectionAssemblyResult(new Probability(random.NextDouble()),
                                                                        new Probability(random.NextDouble()),
                                                                        random.NextEnumValue<EInterpretationCategory>());
                failureMechanismSectionAssemblyKernel.FailurePathSectionAssemblyResult = kernelResult;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyResult result = calculator.AssembleFailureMechanismSection(lowerLimitNorm, signalingNorm, input);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);

                Assert.AreEqual(kernelResult.ProbabilityProfile, result.ProfileProbability);
                Assert.AreEqual(kernelResult.ProbabilitySection, result.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyGroup(kernelResult.InterpretationCategory),
                                result.AssemblyGroup);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelWithInvalidOutput_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            var random = new Random(21);
            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                categoryLimitsKernel.CategoryLimits = CreateCategoryLimits();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var kernelResult = new FailurePathSectionAssemblyResult(new Probability(random.NextDouble()),
                                                                        new Probability(random.NextDouble()),
                                                                        (EInterpretationCategory) 99);
                failureMechanismSectionAssemblyKernel.FailurePathSectionAssemblyResult = kernelResult;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(lowerLimitNorm, signalingNorm, input);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelThrowsException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                categoryLimitsKernel.ThrowExceptionOnCalculate = true;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(lowerLimitNorm, signalingNorm, input);

                // Assert
                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);

                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelThrowsAssemblyException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                categoryLimitsKernel.ThrowAssemblyExceptionOnCalculate = true;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(lowerLimitNorm, signalingNorm, input);

                // Assert
                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);

                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
            }
        }

        private static FailureMechanismSectionAssemblyInput CreateFailureMechanismSectionAssemblyInput()
        {
            var random = new Random(21);
            return new FailureMechanismSectionAssemblyInput(random.NextBoolean(),
                                                            random.NextDouble(), random.NextDouble(),
                                                            random.NextBoolean(),
                                                            random.NextDouble(), random.NextDouble());
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