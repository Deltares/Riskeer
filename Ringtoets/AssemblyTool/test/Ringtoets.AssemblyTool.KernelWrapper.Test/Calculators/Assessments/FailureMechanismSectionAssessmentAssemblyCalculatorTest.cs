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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assessments;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assessments;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Assessments
{
    [TestFixture]
    public class FailureMechanismSectionAssessmentAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new FailureMechanismSectionAssessmentAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssessmentAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionAssessmentAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void AssembleSimpleAssessment_WithInvalidEnumInput_ThrowFailureMechanismSectionAssessmentAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                var calculator = new FailureMechanismSectionAssessmentAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment((SimpleAssessmentResultType) 99);

                // Assert
                const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentResultType'.";
                var exception = Assert.Throws<FailureMechanismSectionAssessmentAssemblyCalculatorException>(test);
                StringAssert.StartsWith(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
            }
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.NotApplicable, SimpleCalculationResult.NVT)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, SimpleCalculationResult.FV)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, SimpleCalculationResult.VB)]
        public void AssembleSimpleAssessment_WithValidInput_InputCorrectlySetToKernel(SimpleAssessmentResultType assessmentResult,
                                                                                      SimpleCalculationResult expectedKernelInput)
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssessmentAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssessmentAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssessmentAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                Assert.AreEqual(expectedKernelInput, kernel.SimpleAssessmentFailureMechanismsInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssessmentAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssessmentAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssessmentAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssessment assessment = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assessment);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssessmentAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssessmentAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssessmentAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssessmentAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                const string expectedMessage = "The value of argument 'originalGroup' (99) is invalid for Enum type 'FailureMechanismSectionCategoryGroup'.";
                var exception = Assert.Throws<FailureMechanismSectionAssessmentAssemblyCalculatorException>(test);
                StringAssert.StartsWith(expectedMessage, exception.Message);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssessmentAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssessmentAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssessmentAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssessmentAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssessmentAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static void AssertCalculatorOutput(CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> original, FailureMechanismSectionAssessment actual)
        {
            Assert.AreEqual(GetGroup(original.Result.CategoryGroup), actual.Group);
            Assert.AreEqual(original.Result.EstimatedProbabilityOfFailure, actual.Probability);
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetGroup(FailureMechanismSectionCategoryGroup originalGroup)
        {
            switch (originalGroup)
            {
                case FailureMechanismSectionCategoryGroup.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case FailureMechanismSectionCategoryGroup.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case FailureMechanismSectionCategoryGroup.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case FailureMechanismSectionCategoryGroup.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case FailureMechanismSectionCategoryGroup.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case FailureMechanismSectionCategoryGroup.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case FailureMechanismSectionCategoryGroup.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case FailureMechanismSectionCategoryGroup.None:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}