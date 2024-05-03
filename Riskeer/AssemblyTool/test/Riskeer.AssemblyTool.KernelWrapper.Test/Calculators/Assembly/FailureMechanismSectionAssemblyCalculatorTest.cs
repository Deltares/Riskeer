﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Collections.Generic;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
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
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Groups;
using Riskeer.Common.Primitives;

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
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetInputWithUndefinedProbability))]
        public void AssembleFailureMechanismSection_WithValidInputAndNoProbabilityDefined_InputCorrectlySentToKernel(
            FailureMechanismSectionAssemblyInput input, EAnalysisState expectedAnalysisState)
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var random = new Random(21);
                var categoryOutput = random.NextEnumValue<EInterpretationCategory>();
                failureMechanismSectionAssemblyKernel.CategoryOutput = categoryOutput;
                failureMechanismSectionAssemblyKernel.SectionProbability = new Probability(random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanismSection(input);

                // Assert
                Assert.AreEqual(expectedAnalysisState, failureMechanismSectionAssemblyKernel.AnalysisState);
                Assert.AreEqual(categoryOutput, failureMechanismSectionAssemblyKernel.CategoryInput);
            }
        }

        [Test]
        [TestCaseSource(nameof(GetInputWithDefinedProbability))]
        public void AssembleFailureMechanismSection_WithValidInputAndProbabilityDefined_InputCorrectlySentToKernel(
            FailureMechanismSectionAssemblyInput input)
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> interpretationCategories = CreateInterpretationCategories();
                categoryLimitsKernel.InterpretationCategoryLimits = interpretationCategories;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var sectionProbability = new Probability(random.NextDouble(0.0, 0.01));
                failureMechanismSectionAssemblyKernel.SectionProbability = sectionProbability;
                failureMechanismSectionAssemblyKernel.CategoryOutput = EInterpretationCategory.Zero;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanismSection(input);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(input.MaximumAllowableFloodingProbability, assessmentSection.MaximumAllowableFloodingProbability);
                ProbabilityAssert.AreEqual(input.SignalFloodingProbability, assessmentSection.SignalFloodingProbability);

                Assert.AreSame(interpretationCategories, failureMechanismSectionAssemblyKernel.Categories);
                Assert.AreEqual(input.FurtherAnalysisType != FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, failureMechanismSectionAssemblyKernel.RefinementNecessary);
                Assert.AreEqual(input.InitialSectionProbability, failureMechanismSectionAssemblyKernel.ProbabilityInitialMechanismSection);
                Assert.AreEqual(input.RefinedSectionProbability, failureMechanismSectionAssemblyKernel.RefinedProbabilitySection);
                Assert.AreEqual(sectionProbability, failureMechanismSectionAssemblyKernel.SectionProbabilityInput);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_InputWithProbabilityUndefinedAndKernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            var input = new FailureMechanismSectionAssemblyInput(
                0.001, 0.0001, false, random.NextBoolean(), double.NaN,
                random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(), double.NaN);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var categoryOutput = random.NextEnumValue<EInterpretationCategory>();
                var sectionProbability = new Probability(random.NextDouble(0.0, 0.01));
                failureMechanismSectionAssemblyKernel.CategoryOutput = categoryOutput;
                failureMechanismSectionAssemblyKernel.SectionProbability = sectionProbability;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyResultWrapper resultWrapper = calculator.AssembleFailureMechanismSection(input);

                // Assert
                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);

                FailureMechanismSectionAssemblyResult result = resultWrapper.AssemblyResult;
                Assert.AreEqual(sectionProbability, result.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(categoryOutput),
                                result.FailureMechanismSectionAssemblyGroup);
                Assert.AreEqual(AssemblyMethod.BOI0C2, resultWrapper.ProbabilityMethod);
                Assert.AreEqual(AssemblyMethod.BOI0C1, resultWrapper.AssemblyGroupMethod);
            }
        }

        [Test]
        public void AssembleFailureMechanismSection_InputWithProbabilityDefinedKernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            var random = new Random(21);
            var input = new FailureMechanismSectionAssemblyInput(
                0.001, 0.0001, true, true, random.NextDouble(),
                FailureMechanismSectionResultFurtherAnalysisType.Executed, random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.InterpretationCategoryLimits = CreateInterpretationCategories();

                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                var categoryOutput = random.NextEnumValue<EInterpretationCategory>();
                var sectionProbability = new Probability(random.NextDouble(0.0, 0.01));
                failureMechanismSectionAssemblyKernel.CategoryOutput = categoryOutput;
                failureMechanismSectionAssemblyKernel.SectionProbability = sectionProbability;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyResultWrapper resultWrapper = calculator.AssembleFailureMechanismSection(input);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                Assert.IsTrue(failureMechanismSectionAssemblyKernel.Calculated);

                FailureMechanismSectionAssemblyResult result = resultWrapper.AssemblyResult;
                Assert.AreEqual(sectionProbability, result.SectionProbability);
                Assert.AreEqual(FailureMechanismSectionAssemblyGroupConverter.ConvertTo(categoryOutput),
                                result.FailureMechanismSectionAssemblyGroup);
                Assert.AreEqual(AssemblyMethod.BOI0A1, resultWrapper.ProbabilityMethod);
                Assert.AreEqual(AssemblyMethod.BOI0B1, resultWrapper.AssemblyGroupMethod);
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
                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                failureMechanismSectionAssemblyKernel.ThrowExceptionOnCalculate = true;

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
                FailureMechanismSectionAssemblyKernelStub failureMechanismSectionAssemblyKernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                failureMechanismSectionAssemblyKernel.ThrowAssemblyExceptionOnCalculate = true;

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

        private static IEnumerable<TestCaseData> GetInputWithUndefinedProbability()
        {
            const double signalFloodingProbability = 0.0001;
            const double maximumAllowableFloodingProbability = 0.001;

            var random = new Random(21);
            yield return new TestCaseData(new FailureMechanismSectionAssemblyInput(
                                              maximumAllowableFloodingProbability, signalFloodingProbability, false, random.NextBoolean(),
                                              double.NaN, random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(), double.NaN),
                                          EAnalysisState.NotRelevant);
            yield return new TestCaseData(new FailureMechanismSectionAssemblyInput(
                                              maximumAllowableFloodingProbability, signalFloodingProbability, true, false, double.NaN,
                                              FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, double.NaN),
                                          EAnalysisState.NoProbabilityEstimationNecessary);
            yield return new TestCaseData(new FailureMechanismSectionAssemblyInput(
                                              maximumAllowableFloodingProbability, signalFloodingProbability, true, random.NextBoolean(), double.NaN,
                                              FailureMechanismSectionResultFurtherAnalysisType.Necessary, double.NaN),
                                          EAnalysisState.ProbabilityEstimationNecessary);
        }

        private static IEnumerable<TestCaseData> GetInputWithDefinedProbability()
        {
            const double signalFloodingProbability = 0.0001;
            const double maximumAllowableFloodingProbability = 0.001;

            var random = new Random(21);
            yield return new TestCaseData(new FailureMechanismSectionAssemblyInput(
                                              maximumAllowableFloodingProbability, signalFloodingProbability, true, true, random.NextDouble(),
                                              FailureMechanismSectionResultFurtherAnalysisType.NotNecessary, random.NextDouble()));
            yield return new TestCaseData(new FailureMechanismSectionAssemblyInput(
                                              maximumAllowableFloodingProbability, signalFloodingProbability, true, random.NextBoolean(), random.NextDouble(),
                                              FailureMechanismSectionResultFurtherAnalysisType.Executed, random.NextDouble()));
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

        private static CategoriesList<InterpretationCategory> CreateInterpretationCategories()
        {
            return new CategoriesList<InterpretationCategory>(new[]
            {
                new InterpretationCategory(EInterpretationCategory.I, new Probability(0), new Probability(1))
            });
        }
    }
}