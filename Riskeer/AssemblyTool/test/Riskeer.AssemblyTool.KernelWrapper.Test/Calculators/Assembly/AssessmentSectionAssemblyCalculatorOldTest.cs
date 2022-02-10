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
using Assembly.Kernel.Old.Exceptions;
using Assembly.Kernel.Old.Model;
using Assembly.Kernel.Old.Model.FmSectionTypes;
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

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyCalculatorOldTest
    {
        [Test]
        public void Constructor_WithFactory_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactoryOld>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssessmentSectionAssemblyCalculatorOld(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionAssemblyCalculatorOld>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionAssemblyCalculatorOld(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void AssemblyFailureMechanismsWithProbability_WithInvalidEnumInput_ThrowAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);
            double failureProbabilityMarginFactor = random.NextDouble();

            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(), (FailureMechanismAssemblyCategoryGroup) 99);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssembly
                }, signalingNorm, lowerLimitNorm, failureProbabilityMarginFactor);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);
            double failureProbabilityMarginFactor = random.NextDouble();

            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssemblyCategoriesKernelStubOld categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismCategoriesOutput = CategoriesListTestFactory.CreateFailureMechanismCategories();

                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyResult = new FailureMechanismAssemblyResult(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                                           random.NextDouble());

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssembly
                }, signalingNorm, lowerLimitNorm, failureProbabilityMarginFactor);

                // Assert
                Assert.IsFalse(kernel.PartialAssembly);
                Assert.AreEqual(signalingNorm, categoriesKernel.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, categoriesKernel.LowerLimitNorm);
                Assert.AreEqual(1, categoriesKernel.N);
                Assert.AreEqual(failureProbabilityMarginFactor, categoriesKernel.FailureMechanismContribution);

                FailureMechanismAssemblyResult actualFailureMechanismAssemblyInput = kernel.FailureMechanismAssemblyResults.Single();
                Assert.AreEqual(GetGroup(failureMechanismAssembly.Group), actualFailureMechanismAssemblyInput.Category);
                Assert.AreEqual(failureMechanismAssembly.Probability, actualFailureMechanismAssemblyInput.FailureProbability);
                Assert.AreSame(categoriesKernel.FailureMechanismCategoriesOutput, kernel.FailureMechanismCategories);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbability_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);
            double failureProbabilityMarginFactor = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyResult = new FailureMechanismAssemblyResult(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                                           random.NextDouble());

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                FailureMechanismAssembly assembly = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                                         signalingNorm, lowerLimitNorm, failureProbabilityMarginFactor);

                // Assert
                FailureMechanismAssemblyResult expectedResult = kernel.FailureMechanismAssemblyResult;
                FailureMechanismAssemblyCategoryGroup expectedGroup =
                    FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(expectedResult.Category);
                Assert.AreEqual(expectedResult.FailureProbability, assembly.Probability);
                Assert.AreEqual(expectedGroup, assembly.Group);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbability_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);
            double failureProbabilityMarginFactor = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyResult = new FailureMechanismAssemblyResult((EFailureMechanismCategory) 99,
                                                                                           random.NextDouble());

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                               signalingNorm, lowerLimitNorm, failureProbabilityMarginFactor);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);
            double failureProbabilityMarginFactor = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                               signalingNorm, lowerLimitNorm, failureProbabilityMarginFactor);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);
            double failureProbabilityMarginFactor = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                               signalingNorm, lowerLimitNorm, failureProbabilityMarginFactor);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_WithInvalidEnumInput_ThrowAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(new[]
                {
                    (FailureMechanismAssemblyCategoryGroup) 99
                });

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.FailureMechanismCategoryResult = new Random(39).NextEnumValue<EFailureMechanismCategory>();

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssemblyCategoryGroup
                });

                // Assert
                Assert.IsFalse(kernel.PartialAssembly);

                FailureMechanismAssemblyResult actualFailureMechanismAssemblyInput = kernel.FailureMechanismAssemblyResults.Single();
                Assert.AreEqual(GetGroup(failureMechanismAssemblyCategoryGroup), actualFailureMechanismAssemblyInput.Category);
                Assert.IsNaN(actualFailureMechanismAssemblyInput.FailureProbability);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithoutProbability_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>();

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                FailureMechanismAssemblyCategoryGroup assembly = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                FailureMechanismAssemblyCategoryGroup expectedAssembly = FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(kernel.FailureMechanismCategoryResult);
                Assert.AreEqual(expectedAssembly, assembly);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithoutProbability_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.FailureMechanismCategoryResult = (EFailureMechanismCategory) 99;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        [TestCase(FailureMechanismAssemblyCategoryGroup.None, (FailureMechanismAssemblyCategoryGroup) 99, TestName = "Invalid Input Failure Mechanisms With Probability")]
        [TestCase((FailureMechanismAssemblyCategoryGroup) 99, FailureMechanismAssemblyCategoryGroup.None, TestName = "Invalid Input Failure Mechanisms Without Probability")]
        public void AssembleAssessmentSection_WithInvalidInput_ThrowsAssessmentSectionAssemblyCalculatorException(
            FailureMechanismAssemblyCategoryGroup categoryGroupInputFailureMechanismWithProbability,
            FailureMechanismAssemblyCategoryGroup categoryGroupInputFailureMechanismWithoutProbability)
        {
            // Setup
            var random = new Random(39);
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                categoryGroupInputFailureMechanismWithProbability);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(categoryGroupInputFailureMechanismWithoutProbability, failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = random.NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability, failureMechanismsWithProbability);

                // Assert
                FailureMechanismAssemblyResult expectedKernelInputFailureWithoutProbability =
                    AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(failureMechanismsWithoutProbability);
                Assert.AreEqual(expectedKernelInputFailureWithoutProbability.Category, kernel.AssemblyResultNoFailureProbability);

                FailureMechanismAssemblyResult actualKernelInputFailureProbability = kernel.AssemblyResultWithFailureProbability;
                FailureMechanismAssemblyResult expectedKernelInputFailureProbability =
                    AssessmentSectionAssemblyInputCreator.CreateFailureMechanismAssemblyResult(failureMechanismsWithProbability);
                Assert.AreEqual(expectedKernelInputFailureProbability.Category,
                                actualKernelInputFailureProbability.Category);
                Assert.AreEqual(expectedKernelInputFailureProbability.FailureProbability,
                                actualKernelInputFailureProbability.FailureProbability);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = random.NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                AssessmentSectionAssemblyCategoryGroup assembly = calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                                                       failureMechanismsWithProbability);

                // Assert
                AssessmentSectionAssemblyCategoryGroup expectedAssembly = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategory(kernel.AssessmentSectionAssemblyResult);
                Assert.AreEqual(expectedAssembly, assembly);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = (EAssessmentGrade) 99;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                               failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                               failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new FailureMechanismAssembly(random.NextDouble(),
                                                                                random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssessmentSectionAssemblyKernelStubOld kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                               failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_WithInvalidInput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombinedFailureMechanismSections(new[]
                {
                    new[]
                    {
                        new CombinedAssemblyFailureMechanismSection(0, 1, (FailureMechanismSectionAssemblyCategoryGroup) 99)
                    }
                }, random.Next());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var input = new[]
            {
                new[]
                {
                    new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                }
            };
            double assessmentSectionLength = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(string.Empty, new[]
                    {
                        new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                    })
                }, new[]
                {
                    new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                calculator.AssembleCombinedFailureMechanismSections(input, assessmentSectionLength);

                // Assert
                Assert.AreEqual(assessmentSectionLength, kernel.AssessmentSectionLengthInput.Value);
                CombinedFailureMechanismSectionsInputAssert.AssertCombinedFailureMechanismInput(input, kernel.FailureMechanismSectionListsInput);
                Assert.IsFalse(kernel.PartialAssembly);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(string.Empty, new[]
                    {
                        new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                    })
                }, new[]
                {
                    new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                CombinedFailureMechanismSectionAssemblyOld[] output = calculator.AssembleCombinedFailureMechanismSections(new[]
                {
                    new[]
                    {
                        new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                    }
                }, random.NextDouble()).ToArray();

                // Assert
                CombinedFailureMechanismSectionAssemblyAssert.AssertAssembly(kernel.AssemblyResult, output);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(string.Empty, new[]
                    {
                        new FmSectionWithDirectCategory(0, 1, (EFmSectionCategory) 99)
                    })
                }, new[]
                {
                    new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombinedFailureMechanismSections(new[]
                {
                    new[]
                    {
                        new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                    }
                }, random.NextDouble()).ToArray();

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombinedFailureMechanismSections(new[]
                {
                    new[]
                    {
                        new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                    }
                }, random.NextDouble()).ToArray();

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculatorOld(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombinedFailureMechanismSections(new[]
                {
                    new[]
                    {
                        new CombinedAssemblyFailureMechanismSection(0, 1, random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
                    }
                }, random.NextDouble()).ToArray();

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        private static EFailureMechanismCategory GetGroup(FailureMechanismAssemblyCategoryGroup originalGroup)
        {
            switch (originalGroup)
            {
                case FailureMechanismAssemblyCategoryGroup.None:
                    return EFailureMechanismCategory.Gr;
                case FailureMechanismAssemblyCategoryGroup.NotApplicable:
                    return EFailureMechanismCategory.Nvt;
                case FailureMechanismAssemblyCategoryGroup.It:
                    return EFailureMechanismCategory.It;
                case FailureMechanismAssemblyCategoryGroup.IIt:
                    return EFailureMechanismCategory.IIt;
                case FailureMechanismAssemblyCategoryGroup.IIIt:
                    return EFailureMechanismCategory.IIIt;
                case FailureMechanismAssemblyCategoryGroup.IVt:
                    return EFailureMechanismCategory.IVt;
                case FailureMechanismAssemblyCategoryGroup.Vt:
                    return EFailureMechanismCategory.Vt;
                case FailureMechanismAssemblyCategoryGroup.VIt:
                    return EFailureMechanismCategory.VIt;
                case FailureMechanismAssemblyCategoryGroup.VIIt:
                    return EFailureMechanismCategory.VIIt;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}