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
using System.Collections.Generic;
using System.ComponentModel;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
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
            TestDelegate call = () => new FailureMechanismSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        private static void AssertCalculatorOutput(FmSectionAssemblyDirectResult original, FailureMechanismSectionAssembly actual)
        {
            Assert.IsNaN(actual.Probability);
            Assert.AreEqual(AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(original.Result), actual.Group);
        }

        private static void AssertCalculatorOutput(FmSectionAssemblyDirectResultWithProbability original, FailureMechanismSectionAssembly actual)
        {
            Assert.AreEqual(original.FailureProbability, actual.Probability);
            Assert.AreEqual(AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(original.Result), actual.Group);
        }

        private static void AssertAssemblyCategoriesInput(AssemblyCategoriesInput assemblyCategoriesInput,
                                                          AssemblyCategoriesKernelStub categoriesKernel,
                                                          CategoriesList<FmSectionCategory> categories,
                                                          FailureMechanismSectionAssemblyKernelStub kernel)
        {
            Assert.AreEqual(assemblyCategoriesInput.SignalingNorm, categoriesKernel.SignalingNorm);
            Assert.AreEqual(assemblyCategoriesInput.LowerLimitNorm, categoriesKernel.LowerLimitNorm);
            Assert.AreEqual(assemblyCategoriesInput.N, categoriesKernel.N);
            Assert.AreEqual(assemblyCategoriesInput.FailureMechanismContribution, categoriesKernel.FailureMechanismContribution);

            Assert.AreSame(categories, kernel.FailureMechanismSectionCategories);
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(39);
            return new AssemblyCategoriesInput(random.NextDouble(1.0, 5.0),
                                               random.NextDouble(),
                                               random.NextDouble(0.0, 0.5),
                                               random.NextDouble(0.5, 1.0));
        }

        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<SimpleAssessmentResultType>();
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                EAssessmentResultTypeE1 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE1(assessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeE1Input);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentResultType>());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentValidityOnlyResultType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>();
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                EAssessmentResultTypeE2 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE2(assessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeE2Input);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(random.NextEnumValue<SimpleAssessmentValidityOnlyResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AssembleDetailedAssessmentWithResult_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment((DetailedAssessmentResultType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(detailedAssessmentResult);

                // Assert
                EAssessmentResultTypeG1 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG1(detailedAssessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeG1Input);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithResult_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResult = random.NextEnumValue<DetailedAssessmentResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(detailedAssessmentResult);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithResult_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    kernel.FailureMechanismSectionDirectResult.Result);
                Assert.AreEqual(expectedResult, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithResult_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithResult_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    (DetailedAssessmentProbabilityOnlyResultType) 99,
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var detailedAssessment = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    detailedAssessment,
                    probability,
                    assemblyCategoriesInput);

                // Assert
                EAssessmentResultTypeG2 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessment);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeG2Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);

                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_WithProbabilityResultAndNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    DetailedAssessmentProbabilityOnlyResultType.Probability,
                    double.NaN,
                    assemblyCategoriesInput);

                // Assert
                Assert.IsNaN(kernel.FailureProbabilityInput);
                Assert.AreEqual(EAssessmentResultTypeG2.Gr, kernel.AssessmentResultTypeG2Input);

                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithProbability_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    (DetailedAssessmentProbabilityOnlyResultType) 99,
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble(1, 10);
            double failureMechanismContribution = random.NextDouble();
            var detailedAssessment = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    detailedAssessment,
                    probability,
                    normativeNorm, 
                    n, 
                    failureMechanismContribution);

                // Assert
                Assert.AreEqual(normativeNorm, categoriesKernel.AssessmentSectionNorm);
                Assert.AreEqual(n, categoriesKernel.N);
                Assert.AreEqual(failureMechanismContribution, categoriesKernel.FailureMechanismContribution);
                Assert.AreSame(categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02, kernel.FailureMechanismSectionCategories);

                EAssessmentResultTypeG2 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessment);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeG2Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_WithProbabilityResultAndNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble(1, 10);
            double failureMechanismContribution = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    DetailedAssessmentProbabilityOnlyResultType.Probability,
                    double.NaN,
                    normativeNorm,
                    n,
                    failureMechanismContribution);

                // Assert
                Assert.AreEqual(normativeNorm, categoriesKernel.AssessmentSectionNorm);
                Assert.AreEqual(n, categoriesKernel.N);
                Assert.AreEqual(failureMechanismContribution, categoriesKernel.FailureMechanismContribution);
                Assert.AreSame(categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02, kernel.FailureMechanismSectionCategories);
                
                Assert.IsNaN(kernel.FailureProbabilityInput);
                Assert.AreEqual(EAssessmentResultTypeG2.Gr, kernel.AssessmentResultTypeG2Input);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithNormativeNorm_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    (DetailedAssessmentProbabilityOnlyResultType) 99,
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble();
            var detailedAssessment = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    detailedAssessment,
                    probability,
                    n,
                    assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                EAssessmentResultTypeG2 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessment);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeG2Input);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_WithProbabilityResultAndNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double n = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    DetailedAssessmentProbabilityOnlyResultType.Probability,
                    double.NaN,
                    n,
                    assemblyCategoriesInput);

                // Assert
                Assert.IsNaN(kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                Assert.AreEqual(EAssessmentResultTypeG2.Gr, kernel.AssessmentResultTypeG2Input);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithLengthEffect_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    (DetailedAssessmentResultType) 99,
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(detailedAssessmentResultForFactorizedSignalingNorm,
                                                      detailedAssessmentResultForSignalingNorm,
                                                      detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                                                      detailedAssessmentResultForLowerLimitNorm,
                                                      detailedAssessmentResultForFactorizedLowerLimitNorm);

                // Assert
                Dictionary<EFmSectionCategory, ECategoryCompliancy> results = kernel.CategoryCompliancyResultsInput.GetCompliancyResults();
                Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForFactorizedSignalingNorm), results[EFmSectionCategory.Iv]);
                Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForSignalingNorm), results[EFmSectionCategory.IIv]);
                Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForMechanismSpecificLowerLimitNorm), results[EFmSectionCategory.IIIv]);
                Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForLowerLimitNorm), results[EFmSectionCategory.IVv]);
                Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForFactorizedLowerLimitNorm), results[EFmSectionCategory.Vv]);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    detailedAssessmentResultForFactorizedSignalingNorm,
                    detailedAssessmentResultForSignalingNorm,
                    detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                    detailedAssessmentResultForLowerLimitNorm,
                    detailedAssessmentResultForFactorizedLowerLimitNorm);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    kernel.FailureMechanismSectionDirectResult.Result);
                Assert.AreEqual(expectedResult, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessmentWithCategoryResults_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>(),
                    random.NextEnumValue<DetailedAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        private static ECategoryCompliancy GetCategoryCompliance(DetailedAssessmentResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentResultType.None:
                    return ECategoryCompliancy.NoResult;
                case DetailedAssessmentResultType.Sufficient:
                    return ECategoryCompliancy.Complies;
                case DetailedAssessmentResultType.Insufficient:
                    return ECategoryCompliancy.DoesNotComply;
                case DetailedAssessmentResultType.NotAssessed:
                    return ECategoryCompliancy.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void AssembleTailorMadeAssessmentWithResult_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment((TailorMadeAssessmentResultType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult);

                // Assert
                EAssessmentResultTypeT1 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT1(tailorMadeAssessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeT1Input);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithResult_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithResult_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentResultType>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    kernel.FailureMechanismSectionDirectResult.Result);
                Assert.AreEqual(expectedResult, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithResult_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithResult_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    (TailorMadeAssessmentProbabilityAndDetailedCalculationResultType) 99,
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();
            double probability = random.NextDouble();
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble(1, 10);
            double failureMechanismContribution = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, normativeNorm, n, failureMechanismContribution);

                // Assert
                Assert.AreEqual(normativeNorm, categoriesKernel.AssessmentSectionNorm);
                Assert.AreEqual(n, categoriesKernel.N);
                Assert.AreEqual(failureMechanismContribution, categoriesKernel.FailureMechanismContribution);

                Assert.AreSame(categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02, kernel.FailureMechanismSectionCategories);
                EAssessmentResultTypeT4 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT4(tailorMadeAssessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeT4Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_WithProbabilityResultAndNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double normativeNorm = random.NextDouble();
            double n = random.NextDouble(1, 10);
            double failureMechanismContribution = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability,
                                                        double.NaN,
                                                        normativeNorm,
                                                        n,
                                                        failureMechanismContribution);

                // Assert
                Assert.AreEqual(normativeNorm, categoriesKernel.AssessmentSectionNorm);
                Assert.AreEqual(n, categoriesKernel.N);
                Assert.AreEqual(failureMechanismContribution, categoriesKernel.FailureMechanismContribution);

                Assert.AreSame(categoriesKernel.FailureMechanismSectionCategoriesOutputWbi02, kernel.FailureMechanismSectionCategories);
                Assert.AreEqual(EAssessmentResultTypeT4.Gr, kernel.AssessmentResultTypeT4Input);
                Assert.IsNaN(kernel.FailureProbabilityInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionDirectResult, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    random.NextDouble(1, 10),
                    random.NextDouble());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    (TailorMadeAssessmentProbabilityCalculationResultType) 99,
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double probability = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, assemblyCategoriesInput);

                // Assert
                EAssessmentResultTypeT3 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3(tailorMadeAssessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeT3Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_WithProbabilityResultAndNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                        double.NaN,
                                                        assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(EAssessmentResultTypeT3.Gr, kernel.AssessmentResultTypeT3Input);
                Assert.IsNaN(kernel.FailureProbabilityInput);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityCalculationResult_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    (TailorMadeAssessmentProbabilityCalculationResultType) 99,
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double probability = random.NextDouble();
            double n = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, n, assemblyCategoriesInput);

                // Assert
                EAssessmentResultTypeT3 expectedResultType = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3(tailorMadeAssessmentResult);
                Assert.AreEqual(expectedResultType, kernel.AssessmentResultTypeT3Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_WithProbabilityResultAndNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double n = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                                                        double.NaN,
                                                        n,
                                                        assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(EAssessmentResultTypeT3.Gr, kernel.AssessmentResultTypeT3Input);
                Assert.IsNaN(kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithLengthEffect_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_WithInvalidAssessmentResultTypeEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment((TailorMadeAssessmentCategoryGroupResultType) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var categoryGroupResult = random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(categoryGroupResult);

                // Assert
                Tuple<EAssessmentResultTypeT3, EFmSectionCategory?> expectedInput =
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3WithCategoryGroupResult(categoryGroupResult);

                Assert.AreEqual(expectedInput.Item1, kernel.AssessmentResultTypeT3Input);
                Assert.AreEqual(expectedInput.Item2, kernel.SectionCategoryInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    kernel.FailureMechanismSectionDirectResult.Result);
                Assert.AreEqual(expectedResult, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentCategoryGroupResultType>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        #endregion

        #region Combined Assembly

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly);

                // Assert
                AssertAssembly(simpleAssembly, kernel.SimpleAssessmentResultInput);
                Assert.IsNull(kernel.DetailedAssessmentResultInput);
                Assert.IsNull(kernel.TailorMadeAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                AssertCalculatorOutput((FmSectionAssemblyDirectResultWithProbability) kernel.FailureMechanismAssessmentResult, assembly);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilitiesAndSimpleAssemblyOnly_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var detailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());
            var tailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

                // Assert
                AssertAssembly(simpleAssembly, kernel.SimpleAssessmentResultInput);
                AssertAssembly(detailedAssembly, kernel.DetailedAssessmentResultInput);
                AssertAssembly(tailorMadeAssembly, kernel.TailorMadeAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                AssertCalculatorOutput((FmSectionAssemblyDirectResultWithProbability) kernel.FailureMechanismAssessmentResult, assembly);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithProbabilities_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined((FailureMechanismSectionAssemblyCategoryGroup) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly);

                // Assert
                Assert.AreEqual(simpleAssembly, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernel.SimpleAssessmentResultInput.Result));
                Assert.IsNull(kernel.DetailedAssessmentResultInput);
                Assert.IsNull(kernel.TailorMadeAssessmentResultInput);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup group = calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    ((FmSectionAssemblyDirectResultWithProbability) kernel.FailureMechanismAssessmentResult).Result);
                Assert.AreEqual(expectedResult, group);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAssemblyOnly_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined((FailureMechanismSectionAssemblyCategoryGroup) 99,
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                           random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, tailorMadeAssembly);

                // Assert
                Assert.AreEqual(simpleAssembly, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernel.SimpleAssessmentResultInput.Result));
                Assert.IsNull(kernel.DetailedAssessmentResultInput);
                Assert.AreEqual(tailorMadeAssembly, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernel.TailorMadeAssessmentResultInput.Result));
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup group = calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    ((FmSectionAssemblyDirectResult) kernel.FailureMechanismAssessmentResult).Result);
                Assert.AreEqual(expectedResult, group);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedWithSimpleAndTailorMadeAssemblyOnly_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined((FailureMechanismSectionAssemblyCategoryGroup) 99,
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var simpleAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var detailedAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            var tailorMadeAssembly = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

                // Assert
                Assert.AreEqual(simpleAssembly, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernel.SimpleAssessmentResultInput.Result));
                Assert.AreEqual(detailedAssembly, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernel.DetailedAssessmentResultInput.Result));
                Assert.AreEqual(tailorMadeAssembly, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernel.TailorMadeAssessmentResultInput.Result));
            }
        }

        [Test]
        public void AssembleCombined_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup group = calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup expectedResult = AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(
                    ((FmSectionAssemblyDirectResult) kernel.FailureMechanismAssessmentResult).Result);
                Assert.AreEqual(expectedResult, group);
            }
        }

        [Test]
        public void AssembleCombined_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssessmentResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleCombined_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        private static void AssertAssembly(FailureMechanismSectionAssembly assembly, FmSectionAssemblyDirectResult kernelAssemblyResult)
        {
            Assert.AreEqual(assembly.Group, AssemblyCategoryAssert.GetFailureMechanismSectionCategoryGroup(kernelAssemblyResult.Result));

            var kernelAssemblyResultWithProbability = kernelAssemblyResult as FmSectionAssemblyDirectResultWithProbability;
            Assert.IsNotNull(kernelAssemblyResultWithProbability);
            Assert.AreEqual(assembly.Probability, kernelAssemblyResultWithProbability.FailureProbability);
        }

        #endregion

        #region Manual Assembly

        [Test]
        public void AssembleManualWithProbability_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleManual(probability, assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(EAssessmentResultTypeG2.ResultSpecified, kernel.AssessmentResultTypeG2Input);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);

            }
        }

        [Test]
        public void AssembleManualWithProbability_WithNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleManual(double.NaN, assemblyCategoriesInput);

                // Assert
                Assert.IsNaN(kernel.FailureProbabilityInput);
                Assert.AreEqual(EAssessmentResultTypeG2.Gr, kernel.AssessmentResultTypeG2Input);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleManualWithProbability_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleManual(random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleManualWithProbability_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleManual(random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleManualWithProbability_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleManual(random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleManualWithProbability_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleManual(random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleManualWithLengthEffect_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double n = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleManual(
                    probability,
                    n,
                    assemblyCategoriesInput);

                // Assert
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                Assert.AreEqual(EAssessmentResultTypeG2.ResultSpecified, kernel.AssessmentResultTypeG2Input);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleManualWithLengthEffect_WithNaNValue_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double n = random.NextDouble();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01 = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleManual(double.NaN, n, assemblyCategoriesInput);

                // Assert
                Assert.IsNaN(kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                Assert.AreEqual(EAssessmentResultTypeG2.Gr, kernel.AssessmentResultTypeG2Input);
                AssertAssemblyCategoriesInput(assemblyCategoriesInput,
                                              categoriesKernel,
                                              categoriesKernel.FailureMechanismSectionCategoriesOutputWbi01,
                                              kernel);
            }
        }

        [Test]
        public void AssembleManualWithLengthEffect_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleManual(random.NextDouble(), random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismAssemblyDirectResultWithProbability, assembly);
            }
        }

        [Test]
        public void AssembleManualWithLengthEffect_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability((EFmSectionCategory) 99,
                                                                                                                              random.NextDouble());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleManual(random.NextDouble(), random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleManualWithLengthEffect_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleManual(random.NextDouble(), random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleManualWithLengthEffect_KernelThrowsAssemblyException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleManual(random.NextDouble(), random.NextDouble(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        #endregion
    }
}