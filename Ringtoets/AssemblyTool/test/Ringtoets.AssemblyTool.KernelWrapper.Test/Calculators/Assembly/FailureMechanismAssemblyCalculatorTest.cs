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
using Assembly.Kernel.Model;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
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
        public void AssembleFailureMechanism_WithInvalidEnumInput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanism(new[]
                {
                    new FailureMechanismSectionAssembly(new Random(39).NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99)
                });

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var sectionResults = new List<FailureMechanismSectionAssembly>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = new Random(39).NextEnumValue<EFailureMechanismCategory>();

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanism(sectionResults);

                // Assert
                Assert.AreSame(sectionResults, kernel.FmSectionAssemblyResultsInput);
                Assert.IsFalse(kernel.PartialAssembly);
            }
        }

        [Test]
        public void AssembleFailureMechanism_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = new Random(39).NextEnumValue<EFailureMechanismCategory>();

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                FailureMechanismAssemblyCategoryGroup category = calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>());

                // Assert
                Assert.AreEqual(kernel.FailureMechanismCategoryResult, category);
            }
        }

        [Test]
        public void AssembleFailureMechanism_KernelWithInvalidOutput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = (EFailureMechanismCategory) 99;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanism_KernelThrowsException_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_WithInvalidEnumInput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanism(new[]
                {
                    new FailureMechanismSectionAssembly(new Random(39).NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99)
                }, CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var sectionResults = new List<FailureMechanismSectionAssembly>();
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>();

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanism(sectionResults, assemblyCategoriesInput);

                // Assert
                Assert.AreSame(sectionResults, kernel.FmSectionAssemblyResultsInput);
                Assert.IsFalse(kernel.PartialAssembly);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = random.NextEnumValue<EFailureMechanismCategory>();

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                FailureMechanismAssembly category = calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>(),
                                                                                        CreateAssemblyCategoriesInput());

                // Assert
                Assert.AreEqual(kernel.FailureMechanismAssemblyResult, category);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_KernelWithInvalidOutput_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.FailureMechanismCategoryResult = (EFailureMechanismCategory) 99;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>(),
                                                                              CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbabilities_KernelThrowsException_ThrowFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanism(new List<FailureMechanismSectionAssembly>(),
                                                                              CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(39);
            return new AssemblyCategoriesInput(random.NextDouble(1.0, 5.0),
                                               random.NextDouble(),
                                               random.NextDouble(0.0, 0.5),
                                               random.NextDouble(0.5, 1.0));
        }
    }
}