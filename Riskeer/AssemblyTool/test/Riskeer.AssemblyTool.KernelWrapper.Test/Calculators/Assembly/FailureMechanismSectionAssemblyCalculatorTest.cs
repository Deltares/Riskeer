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
using System.ComponentModel;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
using Assembly.Kernel.Model.FailureMechanismSections;
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
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Groups;
using Riskeer.Common.Primitives;
using KernelFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
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
            void Call() => calculator.AssembleFailureMechanismSection((FailureMechanismSectionAssemblyInput) null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("input"));
        }

        [Test]
        public void AssembleFailureMechanismSection_InvalidInput_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var input = new FailureMechanismSectionAssemblyInput(
                0.01, 0.001, random.NextBoolean(), random.NextBoolean(), random.NextDouble(),
                (FailureMechanismSectionResultFurtherAnalysisType) 99, random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(input);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);

                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        [Test]
        [TestCase(false, true, ESectionInitialMechanismProbabilitySpecification.NotRelevant)]
        [TestCase(false, false, ESectionInitialMechanismProbabilitySpecification.NotRelevant)]
        [TestCase(true, true, ESectionInitialMechanismProbabilitySpecification.RelevantWithProbabilitySpecification)]
        [TestCase(true, false, ESectionInitialMechanismProbabilitySpecification.RelevantNoProbabilitySpecification)]
        public void AssembleFailureMechanismSection_WithValidInput_InputCorrectlySentToKernel(
            bool isRelevant, bool hasProbabilitySpecified,
            ESectionInitialMechanismProbabilitySpecification expectedInitialMechanismProbabilitySpecification)
        {
            // Setup
            const double signalFloodingProbability = 0.0001;
            const double maximumAllowableFloodingProbability = 0.001;

            var random = new Random(21);
            var input = new FailureMechanismSectionAssemblyInput(maximumAllowableFloodingProbability, signalFloodingProbability,
                                                                 isRelevant, hasProbabilitySpecified,
                                                                 random.NextDouble(),
                                                                 random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                                                                 random.NextDouble());
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> interpretationCategories = CreateInterpretationCategories();
                categoryLimitsKernel.InterpretationCategoryLimits = interpretationCategories;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResult = new KernelFailureMechanismSectionAssemblyResult(
                    new Probability(random.NextDouble(0.0, 0.01)), EInterpretationCategory.Zero);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanismSection(input);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(maximumAllowableFloodingProbability, assessmentSection.MaximumAllowableFloodingProbability);
                ProbabilityAssert.AreEqual(signalFloodingProbability, assessmentSection.SignalFloodingProbability);

                Assert.AreSame(interpretationCategories, failureMechanismSectionAssemblyKernel.Categories);
                Assert.AreEqual(expectedInitialMechanismProbabilitySpecification, failureMechanismSectionAssemblyKernel.InitialMechanismProbabilitySpecification);
                Assert.AreEqual(input.InitialSectionProbability, failureMechanismSectionAssemblyKernel.ProbabilityInitialMechanismSection);
                Assert.AreEqual(FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionResultFurtherAnalysisType(input.FurtherAnalysisType),
                                failureMechanismSectionAssemblyKernel.RefinementStatus);
                Assert.AreEqual(input.RefinedSectionProbability, failureMechanismSectionAssemblyKernel.RefinedProbabilitySection);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.InterpretationCategoryLimits = CreateInterpretationCategories();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var kernelResult = new KernelFailureMechanismSectionAssemblyResult(new Probability(random.NextDouble()),
                                                                                   random.NextEnumValue(new[]
                                                                                   {
                                                                                       EInterpretationCategory.III,
                                                                                       EInterpretationCategory.II,
                                                                                       EInterpretationCategory.I,
                                                                                       EInterpretationCategory.Zero,
                                                                                       EInterpretationCategory.IMin,
                                                                                       EInterpretationCategory.IIMin,
                                                                                       EInterpretationCategory.IIIMin
                                                                                   }));
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResult = kernelResult;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                RiskeerFailureMechanismSectionAssemblyResult result = calculator.AssembleFailureMechanismSection(input);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);

                Assert.AreEqual(kernelResult.ProbabilitySection, result.ProfileProbability);
                Assert.AreEqual(kernelResult.ProbabilitySection, result.SectionProbability);
                Assert.AreEqual(1.0, result.N);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(kernelResult.InterpretationCategory),
                                result.FailureMechanismSectionAssemblyGroup);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelThrowsException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

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
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);

                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_KernelThrowsAssemblyException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            FailureMechanismSectionAssemblyInput input = CreateFailureMechanismSectionAssemblyInput();

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

        [Test]
        public void AssembleFailureMechanismSectionWithProfileProbability_InputNull_ThrowsArgumentNullException()
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
        public void AssembleFailureMechanismSectionWithProfileProbability_InvalidInput_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var input = new FailureMechanismSectionWithProfileProbabilityAssemblyInput(
                0.01, 0.001, random.NextBoolean(), random.NextBoolean(), random.NextDouble(), random.NextDouble(),
                (FailureMechanismSectionResultFurtherAnalysisType) 99, random.NextDouble(), random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleFailureMechanismSection(input);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);

                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        [Test]
        [TestCase(false, true, ESectionInitialMechanismProbabilitySpecification.NotRelevant)]
        [TestCase(false, false, ESectionInitialMechanismProbabilitySpecification.NotRelevant)]
        [TestCase(true, true, ESectionInitialMechanismProbabilitySpecification.RelevantWithProbabilitySpecification)]
        [TestCase(true, false, ESectionInitialMechanismProbabilitySpecification.RelevantNoProbabilitySpecification)]
        public void AssembleFailureMechanismSectionWithProfileProbability_WithValidInput_InputCorrectlySentToKernel(
            bool isRelevant, bool hasProbabilitySpecified,
            ESectionInitialMechanismProbabilitySpecification expectedInitialMechanismProbabilitySpecification)
        {
            // Setup
            const double signalFloodingProbability = 0.0001;
            const double maximumAllowableFloodingProbability = 0.001;

            var random = new Random(21);
            var input = new FailureMechanismSectionWithProfileProbabilityAssemblyInput(maximumAllowableFloodingProbability, signalFloodingProbability,
                                                                                       isRelevant, hasProbabilitySpecified,
                                                                                       random.NextDouble(), random.NextDouble(),
                                                                                       random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                                                                                       random.NextDouble(), random.NextDouble());
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> interpretationCategories = CreateInterpretationCategories();
                categoryLimitsKernel.InterpretationCategoryLimits = interpretationCategories;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResultWithLengthEffect = new FailureMechanismSectionAssemblyResultWithLengthEffect(
                    new Probability(random.NextDouble(0.0, 0.01)), new Probability(random.NextDouble(0.01, 0.02)), EInterpretationCategory.Zero);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanismSection(input);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(maximumAllowableFloodingProbability, assessmentSection.MaximumAllowableFloodingProbability);
                ProbabilityAssert.AreEqual(signalFloodingProbability, assessmentSection.SignalFloodingProbability);

                Assert.AreSame(interpretationCategories, failureMechanismSectionAssemblyKernel.Categories);
                Assert.AreEqual(expectedInitialMechanismProbabilitySpecification, failureMechanismSectionAssemblyKernel.InitialMechanismProbabilitySpecification);
                Assert.AreEqual(input.InitialProfileProbability, failureMechanismSectionAssemblyKernel.ProbabilityInitialMechanismProfile);
                Assert.AreEqual(input.InitialSectionProbability, failureMechanismSectionAssemblyKernel.ProbabilityInitialMechanismSection);
                Assert.AreEqual(FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionResultFurtherAnalysisType(input.FurtherAnalysisType),
                                failureMechanismSectionAssemblyKernel.RefinementStatus);
                Assert.AreEqual(input.RefinedProfileProbability, failureMechanismSectionAssemblyKernel.RefinedProbabilityProfile);
                Assert.AreEqual(input.RefinedSectionProbability, failureMechanismSectionAssemblyKernel.RefinedProbabilitySection);
            }
        }

        [Test]
        public void AssembleFailureMechanismSectionWithProfileProbability_KernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionWithProfileProbabilityAssemblyInput();
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.InterpretationCategoryLimits = CreateInterpretationCategories();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var kernelResult = new FailureMechanismSectionAssemblyResultWithLengthEffect(new Probability(random.NextDouble()),
                                                                                             new Probability(random.NextDouble()),
                                                                                             random.NextEnumValue<EInterpretationCategory>());
                failureMechanismSectionAssemblyKernel.FailureMechanismSectionAssemblyResultWithLengthEffect = kernelResult;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                RiskeerFailureMechanismSectionAssemblyResult result = calculator.AssembleFailureMechanismSection(input);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);

                Assert.AreEqual(kernelResult.ProbabilityProfile, result.ProfileProbability);
                Assert.AreEqual(kernelResult.ProbabilitySection, result.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(kernelResult.InterpretationCategory),
                                result.FailureMechanismSectionAssemblyGroup);
            }
        }

        [Test]
        public void AssembleFailureMechanismSectionWithProfileProbability_KernelThrowsException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionWithProfileProbabilityAssemblyInput();

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
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);

                Assert.IsFalse(failureMechanismSectionAssemblyKernel.Calculated);
            }
        }

        [Test]
        public void AssembleFailureMechanismSectionWithProfileProbability_KernelThrowsAssemblyException_ThrowsFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            FailureMechanismSectionWithProfileProbabilityAssemblyInput input = CreateFailureMechanismSectionWithProfileProbabilityAssemblyInput();

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

        private static FailureMechanismSectionAssemblyInput CreateFailureMechanismSectionAssemblyInput()
        {
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            var random = new Random(21);
            return new FailureMechanismSectionAssemblyInput(maximumAllowableFloodingProbability, signalFloodingProbability,
                                                            random.NextBoolean(), random.NextBoolean(),
                                                            random.NextDouble(),
                                                            random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                                                            random.NextDouble());
        }

        private static FailureMechanismSectionWithProfileProbabilityAssemblyInput CreateFailureMechanismSectionWithProfileProbabilityAssemblyInput()
        {
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            var random = new Random(21);
            return new FailureMechanismSectionWithProfileProbabilityAssemblyInput(maximumAllowableFloodingProbability, signalFloodingProbability,
                                                                                  random.NextBoolean(), random.NextBoolean(),
                                                                                  random.NextDouble(), random.NextDouble(),
                                                                                  random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                                                                                  random.NextDouble(), random.NextDouble());
        }

        private static CategoriesList<InterpretationCategory> CreateInterpretationCategories()
        {
            return new CategoriesList<InterpretationCategory>(new[]
            {
                new InterpretationCategory(EInterpretationCategory.I, new Probability(0), new Probability(1))
            });
        }
    }
}