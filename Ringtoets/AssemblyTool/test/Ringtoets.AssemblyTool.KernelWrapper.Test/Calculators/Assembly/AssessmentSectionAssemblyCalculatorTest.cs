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
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;
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
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);

            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(), (FailureMechanismAssemblyCategoryGroup) 99);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssembly
                }, signalingNorm, lowerLimitNorm);

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
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);

            var failureMechanismAssembly = new FailureMechanismAssembly(random.NextDouble(),
                                                                        random.NextEnumValue<FailureMechanismAssemblyCategoryGroup>());

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult(random.NextEnumValue<EAssessmentGrade>(),
                                                                                             random.NextDouble());

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleFailureMechanisms(new[]
                {
                    failureMechanismAssembly
                }, signalingNorm, lowerLimitNorm);

                // Assert
                Assert.IsFalse(kernel.PartialAssembly);
                Assert.AreEqual(signalingNorm, kernel.AssessmentSectionInput.FailureProbabilitySignallingLimit);
                Assert.AreEqual(lowerLimitNorm, kernel.AssessmentSectionInput.FailureProbabilityLowerLimit);

                FailureMechanismAssemblyResult actualFailureMechanismAssemblyInput = kernel.FailureMechanismAssemblyResults.Single();
                Assert.AreEqual(GetGroup(failureMechanismAssembly.Group), actualFailureMechanismAssemblyInput.Category);
                Assert.AreEqual(failureMechanismAssembly.Probability, actualFailureMechanismAssemblyInput.FailureProbability);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithProbability_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult(random.NextEnumValue<EAssessmentGrade>(),
                                                                                             random.NextDouble());

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                AssessmentSectionAssembly assembly = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                                          signalingNorm,
                                                                                          lowerLimitNorm);

                // Assert
                AssessmentSectionAssemblyResult expectedResult = kernel.AssessmentSectionAssemblyResult;
                AssessmentSectionAssemblyCategoryGroup expectedGroup = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategory(expectedResult.Category);
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

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult((EAssessmentGrade) 99,
                                                                                             random.NextDouble());

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                               signalingNorm,
                                                                               lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                               signalingNorm,
                                                                               lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithProbability_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalingNorm = random.NextDouble(0, 0.5);
            double lowerLimitNorm = random.NextDouble(0.5, 1);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssembly>(),
                                                                               signalingNorm,
                                                                               lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(exception.Message, AssemblyErrorMessageTranslator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }));
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_WithInvalidEnumInput_ThrowAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(new[]
                {
                    (FailureMechanismAssemblyCategoryGroup) 99
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
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
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
        public void AssembleFailureMechanismWithoutProbability_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(39);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentGradeResult = random.NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                AssessmentSectionAssemblyCategoryGroup assembly = calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                AssessmentSectionAssemblyCategoryGroup expectedAssembly = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategory(kernel.AssessmentGradeResult);
                Assert.AreEqual(expectedAssembly, assembly);
            }
        }

        [Test]
        public void AssembleFailureMechanismWithoutProbability_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentGradeResult = (EAssessmentGrade) 99;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

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

        [Test]
        public void AssembleFailureMechanismsWithoutProbability_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleFailureMechanisms(Enumerable.Empty<FailureMechanismAssemblyCategoryGroup>());

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(exception.Message, AssemblyErrorMessageTranslator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }));
            }
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.None, (AssessmentSectionAssemblyCategoryGroup) 99, TestName = "Invalid Input Failure Mechanisms With Probability")]
        [TestCase((AssessmentSectionAssemblyCategoryGroup) 99, AssessmentSectionAssemblyCategoryGroup.None, TestName = "Invalid Input Failure Mechanisms Without Probability")]
        public void AssembleAssessmentSection_WithInvalidInput_ThrowsAssessmentSectionAssemblyCalculatorException(
            AssessmentSectionAssemblyCategoryGroup categoryGroupInput1,
            AssessmentSectionAssemblyCategoryGroup categoryGroupInput2)
        {
            // Setup
            var random = new Random(39);
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 categoryGroupInput1);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(categoryGroupInput2, failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(39);
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult(random.NextEnumValue<EAssessmentGrade>());

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability, failureMechanismsWithProbability);

                // Assert
                AssessmentSectionAssemblyResult actualKernelInputNoFailureProbability = kernel.AssemblyResultNoFailureProbability;
                AssessmentSectionAssemblyResult expectedKernelInputNoFailureProbability =
                    AssessmentSectionAssemblyInputCreator.CreateAssessementSectionAssemblyResult(failureMechanismsWithoutProbability);
                Assert.AreEqual(expectedKernelInputNoFailureProbability.Category, actualKernelInputNoFailureProbability.Category);
                Assert.AreEqual(expectedKernelInputNoFailureProbability.FailureProbability,
                                actualKernelInputNoFailureProbability.FailureProbability);

                AssessmentSectionAssemblyResult actualKernelInputFailureProbability = kernel.AssemblyResultWithFailureProbability;
                AssessmentSectionAssemblyResult expectedKernelInputFailureProbability =
                    AssessmentSectionAssemblyInputCreator.CreateAssessementSectionAssemblyResult(failureMechanismsWithProbability);
                Assert.AreEqual(expectedKernelInputFailureProbability.Category, actualKernelInputFailureProbability.Category);
                Assert.AreEqual(expectedKernelInputFailureProbability.FailureProbability,
                                actualKernelInputFailureProbability.FailureProbability);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult(random.NextEnumValue<EAssessmentGrade>());

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                AssessmentSectionAssemblyCategoryGroup assembly = calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                                                       failureMechanismsWithProbability);

                // Assert
                AssessmentSectionAssemblyCategoryGroup expectedAssembly = AssemblyCategoryCreator.CreateAssessmentSectionAssemblyCategory(kernel.AssessmentGradeResult);
                Assert.AreEqual(expectedAssembly, assembly);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssessmentSectionAssemblyResult = new AssessmentSectionAssemblyResult((EAssessmentGrade) 99);

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                               failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                               failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismsWithProbability = new AssessmentSectionAssembly(random.NextDouble(),
                                                                                 random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>());
            var failureMechanismsWithoutProbability = random.NextEnumValue<AssessmentSectionAssemblyCategoryGroup>();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                TestDelegate test = () => calculator.AssembleAssessmentSection(failureMechanismsWithoutProbability,
                                                                               failureMechanismsWithProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(test);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(exception.Message, AssemblyErrorMessageTranslator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }));
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_WithInvalidInput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                var calculator = new AssessmentSectionAssemblyCalculator(factory);

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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
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

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(new FailureMechanism(random.NextDouble(1, 2), random.NextDouble()), new[]
                    {
                        new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                    })
                }, new[]
                {
                    new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

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

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(new FailureMechanism(random.NextDouble(1, 2), random.NextDouble()), new[]
                    {
                        new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                    })
                }, new[]
                {
                    new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                CombinedFailureMechanismSectionAssembly[] output = calculator.AssembleCombinedFailureMechanismSections(new[]
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

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.AssemblyResult = new AssemblyResult(new[]
                {
                    new FailureMechanismSectionList(new FailureMechanism(random.NextDouble(1, 2), random.NextDouble()), new[]
                    {
                        new FmSectionWithDirectCategory(0, 1, (EFmSectionCategory) 99)
                    })
                }, new[]
                {
                    new FmSectionWithDirectCategory(0, 1, random.NextEnumValue<EFmSectionCategory>())
                });

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<InvalidEnumArgumentException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

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
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<Exception>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleCombinedFailureMechanismSections_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CombinedFailureMechanismSectionAssemblyKernelStub kernel = factory.LastCreatedCombinedFailureMechanismSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

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
                Assert.AreEqual(exception.Message, AssemblyErrorMessageTranslator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }));
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