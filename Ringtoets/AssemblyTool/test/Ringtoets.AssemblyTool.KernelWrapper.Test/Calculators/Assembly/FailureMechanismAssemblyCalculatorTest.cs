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
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
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

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_WithKernelFactory_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new FailureMechanismAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Assemble_WithInvalidEnumInput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(new[]
                {
                    (FailureMechanismSectionAssemblyCategoryGroup) 99
                });

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var categoryGroups = new[]
            {
                random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = new Random(39).NextEnumValue<EFailureMechanismCategory>();

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.Assemble(categoryGroups);

                // Assert
                FailureMechanismSectionAssemblyCategoryGroup actualCategoryGroup =
                    FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(kernel.FmSectionAssemblyResultsInput.Single().Result);
                Assert.AreEqual(categoryGroups.Single(), actualCategoryGroup);
                Assert.IsFalse(kernel.PartialAssembly);
            }
        }

        [Test]
        public void Assemble_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = new Random(39).NextEnumValue<EFailureMechanismCategory>();

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                FailureMechanismAssemblyCategoryGroup category = calculator.Assemble(
                    new List<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                FailureMechanismAssemblyCategoryGroup expectedCategory = FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(kernel.FailureMechanismCategoryResult);
                Assert.AreEqual(expectedCategory, category);
            }
        }

        [Test]
        public void Assemble_KernelWithInvalidOutput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = (EFailureMechanismCategory) 99;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(new List<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_KernelThrowsException_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(new List<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_KernelThrowsAssemblyException_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(new List<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleWithProbabilities_WithInvalidEnumInput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(new[]
                {
                    new FailureMechanismSectionAssembly(new Random(39).NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99)
                }, CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleWithProbabilities_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var expectedResults = new[]
            {
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>())
            };
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub categoriesKernel = factory.LastCreatedAssemblyCategoriesKernel;
                categoriesKernel.FailureMechanismCategoriesOutput = CategoriesListTestFactory.CreateFailureMechanismCategories();

                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismAssemblyResult = new FailureMechanismAssemblyResult(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                                           random.NextDouble());
                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.Assemble(expectedResults, assemblyCategoriesInput);

                // Assert
                AssertFailureMechanismSectionAssembly(expectedResults.Single(), kernel.FmSectionAssemblyResultsWithProbabilityInput.Single());

                Assert.IsFalse(kernel.PartialAssembly);

                double expectedN = assemblyCategoriesInput.N;
                double expectedFailureMechanismContribution = assemblyCategoriesInput.FailureMechanismContribution;
                Assert.AreEqual(expectedN, kernel.FailureMechanismInput.LengthEffectFactor);
                Assert.AreEqual(expectedFailureMechanismContribution, kernel.FailureMechanismInput.FailureProbabilityMarginFactor);

                Assert.AreSame(categoriesKernel.FailureMechanismCategoriesOutput, kernel.CategoryLimits);
                Assert.AreEqual(assemblyCategoriesInput.LowerLimitNorm, categoriesKernel.LowerLimitNorm);
                Assert.AreEqual(assemblyCategoriesInput.SignalingNorm, categoriesKernel.SignalingNorm);
                Assert.AreEqual(expectedN, categoriesKernel.N);
                Assert.AreEqual(expectedFailureMechanismContribution, categoriesKernel.FailureMechanismContribution);
            }
        }

        [Test]
        public void AssembleWithProbabilities_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismAssemblyResult = new FailureMechanismAssemblyResult(random.NextEnumValue<EFailureMechanismCategory>(),
                                                                                           random.NextDouble());

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                FailureMechanismAssembly assembly = calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                                        CreateAssemblyCategoriesInput());

                // Assert
                FailureMechanismAssemblyCategoryGroup expectedGroup = FailureMechanismAssemblyCreator.CreateFailureMechanismAssemblyCategoryGroup(kernel.FailureMechanismAssemblyResult.Category);
                Assert.AreEqual(expectedGroup, assembly.Group);
                Assert.AreEqual(kernel.FailureMechanismAssemblyResult.FailureProbability, assembly.Probability);
            }
        }

        [Test]
        public void AssembleWithProbabilities_KernelWithInvalidOutput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismAssemblyResult = new FailureMechanismAssemblyResult((EFailureMechanismCategory) 99,
                                                                                           new Random(39).NextDouble());
                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                              CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleWithProbabilities_KernelThrowsException_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                              CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleWithProbabilities_KernelThrowsAssemblyException_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.Assemble(Enumerable.Empty<FailureMechanismSectionAssembly>(),
                                                              CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }

        private static void AssertFailureMechanismSectionAssembly(FailureMechanismSectionAssembly expectedSectionAssembly,
                                                                  FmSectionAssemblyDirectResultWithProbability actualResult)
        {
            FailureMechanismSectionAssemblyCategoryGroup actualGroup =
                FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(actualResult.Result);
            Assert.AreEqual(expectedSectionAssembly.Group, actualGroup);
            Assert.AreEqual(expectedSectionAssembly.Probability, actualResult.FailureProbability);
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(39);
            return new AssemblyCategoriesInput(random.NextDouble(1.0, 5.0),
                                               random.NextDouble(),
                                               random.NextDouble(0.0, 0.5),
                                               random.NextDouble(0.5, 1.0));
        }
    }
}