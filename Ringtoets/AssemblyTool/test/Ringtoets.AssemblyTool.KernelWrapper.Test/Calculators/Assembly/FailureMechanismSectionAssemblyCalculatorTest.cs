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
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
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
            Assert.AreEqual(GetGroup(original.Result), actual.Group);
            Assert.AreEqual(original.FailureProbability, actual.Probability);
        }

        private static FailureMechanismSectionAssemblyCategoryGroup GetGroup(EFmSectionCategory originalGroup)
        {
            switch (originalGroup)
            {
                case EFmSectionCategory.Iv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Iv;
                case EFmSectionCategory.IIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIv;
                case EFmSectionCategory.IIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IIIv;
                case EFmSectionCategory.IVv:
                    return FailureMechanismSectionAssemblyCategoryGroup.IVv;
                case EFmSectionCategory.Vv:
                    return FailureMechanismSectionAssemblyCategoryGroup.Vv;
                case EFmSectionCategory.VIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIv;
                case EFmSectionCategory.VIIv:
                    return FailureMechanismSectionAssemblyCategoryGroup.VIIv;
                case EFmSectionCategory.Gr:
                    return FailureMechanismSectionAssemblyCategoryGroup.None;
                default:
                    throw new NotSupportedException();
            }
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            const SimpleAssessmentResultType assessmentResult = SimpleAssessmentResultType.AssessFurther;
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                Assert.AreEqual(SimpleCalculationResult.VB, kernel.SimpleAssessmentFailureMechanismsInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentResultType.AssessFurther);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            const SimpleAssessmentValidityOnlyResultType assessmentResult = SimpleAssessmentValidityOnlyResultType.Applicable;
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleSimpleAssessment(assessmentResult);

                // Assert
                Assert.AreEqual(SimpleCalculationResultValidityOnly.WVT, kernel.SimpleAssessmentFailureMechanismsValidityOnlyInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType.NotApplicable);

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType.Applicable);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleSimpleAssessmentValidityOnly_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType.Applicable);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Assert.AreEqual(kernel.AssessmentResultTypeG1Input, GetAssessmentResultTypeG1(detailedAssessmentResult));
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Assert.AreEqual(GetGroup(kernel.FailureMechanismSectionDirectResult.Result), assembly);
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
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4)),
                    new FailureMechanism(random.NextDouble(), random.NextDouble()));

                // Assert
                string expectedMessage = $"The value of argument 'detailedAssessmentResult' (99) is invalid for Enum type '{nameof(DetailedAssessmentProbabilityOnlyResultType)}'.";
                string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
                Assert.AreEqual("detailedAssessmentResult", parameterName);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            var detailedAssessment = random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>();

            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    detailedAssessment,
                    probability,
                    assessmentSection,
                    failureMechanism);

                // Assert
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(GetAssessmentResultTypeG2(detailedAssessment), kernel.AssessmentResultTypeG2Input);
                Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
                Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(EFmSectionCategory.Iv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                AssertCalculatorOutput(kernel.DetailedAssessmentResultInput, assembly);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
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
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleDetailedAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.5, 0.9), random.NextDouble(0.0, 0.4)),
                    new FailureMechanism(random.NextDouble(), random.NextDouble()));

                // Assert
                string expectedMessage = $"The value of argument 'detailedAssessmentResult' (99) is invalid for Enum type '{nameof(DetailedAssessmentProbabilityOnlyResultType)}'.";
                string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
                Assert.AreEqual("detailedAssessmentResult", parameterName);
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

            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(
                    detailedAssessment,
                    probability,
                    n,
                    assessmentSection,
                    failureMechanism);

                // Assert
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(probability, kernel.LengthEffectFactorInput);
                Assert.AreEqual(GetAssessmentResultTypeG2(detailedAssessment), kernel.AssessmentResultTypeG2Input);
                Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
                Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
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
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(EFmSectionCategory.Iv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                AssertCalculatorOutput(kernel.DetailedAssessmentResultInput, assembly);
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
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleDetailedAssessment(
                    random.NextEnumValue<DetailedAssessmentProbabilityOnlyResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleDetailedAssessment(detailedAssessmentResultForFactorizedSignalingNorm,
                                                      detailedAssessmentResultForSignalingNorm,
                                                      detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                                                      detailedAssessmentResultForLowerLimitNorm,
                                                      detailedAssessmentResultForFactorizedLowerLimitNorm);

                // Assert
                Dictionary<EFmSectionCategory, ECategoryCompliancy> results = kernel.CategoryCompliancyResultsInput.GetCompliancyResults();
                Assert.AreEqual(results[EFmSectionCategory.Iv], GetAssessmentResultTypeG1(detailedAssessmentResultForFactorizedSignalingNorm));
                Assert.AreEqual(results[EFmSectionCategory.IIv], GetAssessmentResultTypeG1(detailedAssessmentResultForSignalingNorm));
                Assert.AreEqual(results[EFmSectionCategory.IIIv], GetAssessmentResultTypeG1(detailedAssessmentResultForMechanismSpecificLowerLimitNorm));
                Assert.AreEqual(results[EFmSectionCategory.IVv], GetAssessmentResultTypeG1(detailedAssessmentResultForLowerLimitNorm));
                Assert.AreEqual(results[EFmSectionCategory.Vv], GetAssessmentResultTypeG1(detailedAssessmentResultForFactorizedLowerLimitNorm));
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Assert.AreEqual(GetGroup(kernel.FailureMechanismSectionDirectResult.Result), assembly);
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
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private static EAssessmentResultTypeG1 GetAssessmentResultTypeG1(DetailedAssessmentResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentResultType.None:
                    return EAssessmentResultTypeG1.Gr;
                case DetailedAssessmentResultType.Sufficient:
                    return EAssessmentResultTypeG1.V;
                case DetailedAssessmentResultType.Insufficient:
                    return EAssessmentResultTypeG1.Vn;
                case DetailedAssessmentResultType.NotAssessed:
                    return EAssessmentResultTypeG1.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        private static EAssessmentResultTypeG2 GetAssessmentResultTypeG2(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentProbabilityOnlyResultType.Probability:
                    return EAssessmentResultTypeG2.ResultSpecified;
                case DetailedAssessmentProbabilityOnlyResultType.NotAssessed:
                    return EAssessmentResultTypeG2.Ngo;
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult);

                // Assert
                Assert.AreEqual(kernel.AssessmentResultTypeT1Input, GetAssessmentResultTypeT1(tailorMadeAssessmentResult));
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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Assert.AreEqual(GetGroup(kernel.FailureMechanismSectionDirectResult.Result), assembly);
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
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithProbabilityAndDetailedCalculationResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityAndDetailedCalculationResultType>();
            double probability = random.NextDouble();
            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, assessmentSection, failureMechanism);

                // Assert
                Assert.AreEqual(GetAssessmentResultTypeT4(tailorMadeAssessmentResult), kernel.AssessmentResultTypeT4Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
                Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithInvalidEnumInput_ThrowFailureMechanismSectionAssemblyCalculatorException()
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var tailorMadeAssessmentResult = random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>();
            double probability = random.NextDouble();
            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, assessmentSection, failureMechanism);

                // Assert
                Assert.AreEqual(GetAssessmentResultTypeT3(tailorMadeAssessmentResult), kernel.AssessmentResultTypeT3Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
                Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
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
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionDirectResult, assembly);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_KernelWithInvalidOutput_ThrowFailureMechanismSectionAssemblyCalculatorException()
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
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessment_KernelThrowsException_ThrowFailureMechanismSectionAssemblyCalculatorException()
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(tailorMadeAssessmentResult, probability, n, assessmentSection, failureMechanism);

                // Assert
                Assert.AreEqual(GetAssessmentResultTypeT3(tailorMadeAssessmentResult), kernel.AssessmentResultTypeT3Input);
                Assert.AreEqual(probability, kernel.FailureProbabilityInput);
                Assert.AreEqual(n, kernel.LengthEffectFactorInput);
                Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
                Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
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
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>());

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionDirectResult, assembly);
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
                kernel.FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult((EFmSectionCategory) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    random.NextDouble(),
                    random.NextDouble(),
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                    new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                TestDelegate test = () => calculator.AssembleTailorMadeAssessment((FailureMechanismSectionAssemblyCategoryGroup) 99);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleTailorMadeAssessmentWithCategoryResult_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var categoryGroupResult = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>();
            
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleTailorMadeAssessment(categoryGroupResult);

                // Assert
                Tuple<EAssessmentResultTypeT3, EFmSectionCategory?> expectedInput = 
                    FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup(categoryGroupResult);

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
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                Assert.AreEqual(GetGroup(kernel.FailureMechanismSectionDirectResult.Result), assembly);
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
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private static EAssessmentResultTypeT1 GetAssessmentResultTypeT1(TailorMadeAssessmentResultType tailorMadeAssessmentResult)
        {
            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentResultType.None:
                    return EAssessmentResultTypeT1.Gr;
                case TailorMadeAssessmentResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT1.Fv;
                case TailorMadeAssessmentResultType.Sufficient:
                    return EAssessmentResultTypeT1.V;
                case TailorMadeAssessmentResultType.Insufficient:
                    return EAssessmentResultTypeT1.Vn;
                case TailorMadeAssessmentResultType.NotAssessed:
                    return EAssessmentResultTypeT1.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        private static EAssessmentResultTypeT3 GetAssessmentResultTypeT3(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult)
        {
            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentProbabilityCalculationResultType.None:
                    return EAssessmentResultTypeT3.Gr;
                case TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT3.Fv;
                case TailorMadeAssessmentProbabilityCalculationResultType.Probability:
                    return EAssessmentResultTypeT3.ResultSpecified;
                case TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed:
                    return EAssessmentResultTypeT3.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        private static EAssessmentResultTypeT4 GetAssessmentResultTypeT4(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult)
        {
            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None:
                    return EAssessmentResultTypeT4.Gr;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT4.Fv;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability:
                    return EAssessmentResultTypeT4.ResultSpecified;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient:
                    return EAssessmentResultTypeT4.V;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient:
                    return EAssessmentResultTypeT4.Vn;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed:
                    return EAssessmentResultTypeT4.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Combined Assembly

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
                TestDelegate test = () => calculator.AssembleCombined(new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99),
                                                                      new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                                                                      new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

                // Assert
                AssertAssembly(simpleAssembly, kernel.CombinedSimpleAssessmentInput);
                AssertAssembly(detailedAssembly, kernel.CombinedDetailedAssessmentInput);
                AssertAssembly(tailorMadeAssembly, kernel.CombinedTailorMadeAssessmentInput);
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
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionCategoryGroup.Iv, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssembly assembly = calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                AssertCalculatorOutput(kernel.FailureMechanismSectionAssemblyCategoryResult, assembly);
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
                kernel.FailureMechanismSectionAssemblyCategoryResult = new CalculationOutput<FailureMechanismSectionAssemblyCategoryResult>(
                    new FailureMechanismSectionAssemblyCategoryResult((FailureMechanismSectionCategoryGroup) 99, Probability.NaN));

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                    new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                FailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismSectionAssemblyKernel;
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    FailureMechanismSectionCategoryGroup.VIv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined((FailureMechanismSectionAssemblyCategoryGroup) 99,
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                                                                      random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    FailureMechanismSectionCategoryGroup.VIv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleCombined(simpleAssembly, detailedAssembly, tailorMadeAssembly);

                // Assert
                Assert.AreEqual(simpleAssembly, GetGroup(kernel.CombinedSimpleAssessmentGroupInput.Value));
                Assert.AreEqual(detailedAssembly, GetGroup(kernel.CombinedDetailedAssessmentGroupInput.Value));
                Assert.AreEqual(tailorMadeAssembly, GetGroup(kernel.CombinedTailorMadeAssessmentGroupInput.Value));
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
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    FailureMechanismSectionCategoryGroup.VIv);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                FailureMechanismSectionAssemblyCategoryGroup group = calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                Assert.AreEqual(GetGroup(kernel.FailureMechanismSectionAssemblyCategoryGroup.Result), group);
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
                kernel.FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<FailureMechanismSectionCategoryGroup>(
                    (FailureMechanismSectionCategoryGroup) 99);

                var calculator = new FailureMechanismSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleCombined(
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>(),
                    random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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
                Exception innerException = exception.InnerException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        private static void AssertAssembly(FailureMechanismSectionAssembly simpleAssembly, FailureMechanismSectionAssemblyCategoryResult kernelInput)
        {
            Assert.AreEqual(simpleAssembly.Probability, kernelInput.EstimatedProbabilityOfFailure);
            Assert.AreEqual(simpleAssembly.Group, GetGroup(kernelInput.CategoryGroup));
        }

        #endregion
    }
}