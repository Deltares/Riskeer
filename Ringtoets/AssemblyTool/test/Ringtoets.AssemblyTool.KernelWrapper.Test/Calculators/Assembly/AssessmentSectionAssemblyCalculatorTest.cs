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
using System.ComponentModel;
using System.Linq;
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
    public class AssessmentSectionAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_WithFactory_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssessmentSectionAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssessmentSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void AssemblyFailureMechanismsWithProbability_WithInvalidEnumInput_ThrowAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(39);
            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(), (FailureMechanismAssemblyCategoryGroup) 99);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssembly
                }, CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            AssemblyCategoriesInput assemblyCategoriesInput = CreateAssemblyCategoriesInput();

            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentGradeResult = new Random(39).NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssembly
                }, assemblyCategoriesInput);

                // Assert
                Assert.IsFalse(kernel.PartialAssembly);
                Assert.AreEqual(assemblyCategoriesInput.LowerLimitNorm, kernel.AssessmentSectionInput.FailureProbabilityLowerLimit);
                Assert.AreEqual(assemblyCategoriesInput.SignalingNorm, kernel.AssessmentSectionInput.FailureProbabilitySignallingLimit);

                FailureMechanismAssemblyResult actualFailureMechanismAssemblyInput = kernel.FailureMechanismAssemblyResults.Single();
                Assert.AreEqual(GetGroup(failureMechanismAssembly.Group), actualFailureMechanismAssemblyInput.Category);
                Assert.AreEqual(failureMechanismAssembly.Probability, actualFailureMechanismAssemblyInput.FailureProbability);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowException = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(), CreateAssemblyCategoriesInput());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_WithInvalidEnumInput_ThrowAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory)AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(new[]
                {
                    (FailureMechanismAssemblyCategoryGroup)99
                });

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var failureMechanismAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory)AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentGradeResult = new Random(39).NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssemblyCategoryGroup
                });

                // Assert
                Assert.IsFalse(kernel.PartialAssembly);

                FailureMechanismAssemblyResult actualFailureMechanismAssemblyInput = kernel.FailureMechanismAssemblyResults.Single();
                Assert.AreEqual(GetGroup(failureMechanismAssemblyCategoryGroup), actualFailureMechanismAssemblyInput.Category);
                Assert.IsNull(actualFailureMechanismAssemblyInput.FailureProbability);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory)AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowException = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private static AssemblyCategoriesInput CreateAssemblyCategoriesInput()
        {
            var random = new Random(21);
            return new AssemblyCategoriesInput(random.NextDouble(1.0, 5.0),
                                               random.NextDouble(),
                                               random.NextDouble(0.0, 0.5),
                                               random.NextDouble(0.5, 1.0));
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