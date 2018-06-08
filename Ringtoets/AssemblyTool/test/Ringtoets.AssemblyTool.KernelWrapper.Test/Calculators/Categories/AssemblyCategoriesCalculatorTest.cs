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
using System.Collections.Generic;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Categories
{
    [TestFixture]
    public class AssemblyCategoriesCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyCategoriesCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssemblyCategoriesCalculator(factory);

            // Assert
            Assert.IsInstanceOf<IAssemblyCategoriesCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateAssessmentSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = CreateAssessmentSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            IEnumerable<AssessmentSectionCategoryLimits> output = CreateAssessmentSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<AssessmentSectionAssemblyCategory> result = calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                AssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelThrowsAssemblyException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            AssemblyCategoriesInput assemblyCategoriesInput = CreateRandomAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismCategoriesOutput = CreateFailureMechanismCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateFailureMechanismCategories(assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(assemblyCategoriesInput.LowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(assemblyCategoriesInput.SignalingNorm, kernel.SignalingNorm);
                Assert.AreEqual(assemblyCategoriesInput.FailureMechanismContribution, kernel.FailureMechanismContribution);
                Assert.AreEqual(assemblyCategoriesInput.N, kernel.N);
            }
        }

        [Test]
        public void CalculateFailureMechanismCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            IEnumerable<FailureMechanismCategoryLimits> output = CreateFailureMechanismCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismAssemblyCategory> result = calculator.CalculateFailureMechanismCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                AssemblyCategoryAssert.AssertFailureMechanismAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateFailureMechanismCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateFailureMechanismCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismCategories_KernelThrowsAssemblyException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateFailureMechanismCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            AssemblyCategoriesInput assemblyCategoriesInput = CreateRandomAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = CreateFailureMechanismSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateFailureMechanismSectionCategories(assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(assemblyCategoriesInput.LowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(assemblyCategoriesInput.SignalingNorm, kernel.SignalingNorm);
                Assert.AreEqual(assemblyCategoriesInput.FailureMechanismContribution, kernel.FailureMechanismContribution);
                Assert.AreEqual(assemblyCategoriesInput.N, kernel.N);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            IEnumerable<FmSectionCategoryLimits> output = CreateFailureMechanismSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateFailureMechanismSectionCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                AssemblyCategoryAssert.AssertFailureMechanismSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateFailureMechanismSectionCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelThrowsAssemblyException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateFailureMechanismSectionCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            AssemblyCategoriesInput assemblyCategoriesInput = CreateRandomAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = CreateFailureMechanismSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateGeotechnicalFailureMechanismSectionCategories(assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(assemblyCategoriesInput.LowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(assemblyCategoriesInput.SignalingNorm, kernel.SignalingNorm);
                Assert.AreEqual(assemblyCategoriesInput.FailureMechanismContribution, kernel.FailureMechanismContribution);
                Assert.AreEqual(assemblyCategoriesInput.N, kernel.N);
            }
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            IEnumerable<FmSectionCategoryLimits> output = CreateFailureMechanismSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateGeotechnicalFailureMechanismSectionCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                AssemblyCategoryAssert.AssertFailureMechanismSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateGeotechnicalFailureMechanismSectionCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateGeotechnicalFailureMechanismSectionCategories_KernelThrowsAssemblyException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateGeotechnicalFailureMechanismSectionCategories(
                    CreateRandomAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        private static AssemblyCategoriesInput CreateRandomAssemblyCategoriesInput()
        {
            var random = new Random(11);
            return new AssemblyCategoriesInput(random.NextDouble(1, 5),
                                               random.NextDouble(),
                                               random.NextDouble(0.0, 0.5),
                                               random.NextDouble(0.5, 1.0));
        }

        private static IEnumerable<AssessmentSectionCategoryLimits> CreateAssessmentSectionCategoryKernelOutput()
        {
            var random = new Random(11);

            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
        }

        private static IEnumerable<FmSectionCategoryLimits> CreateFailureMechanismSectionCategoryKernelOutput()
        {
            var random = new Random(11);

            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
        }

        private static IEnumerable<FailureMechanismCategoryLimits> CreateFailureMechanismCategoryKernelOutput()
        {
            var random = new Random(11);

            yield return new FailureMechanismCategoryLimits(random.NextEnumValue<EFailureMechanismCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FailureMechanismCategoryLimits(random.NextEnumValue<EFailureMechanismCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FailureMechanismCategoryLimits(random.NextEnumValue<EFailureMechanismCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FailureMechanismCategoryLimits(random.NextEnumValue<EFailureMechanismCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
        }
    }
}