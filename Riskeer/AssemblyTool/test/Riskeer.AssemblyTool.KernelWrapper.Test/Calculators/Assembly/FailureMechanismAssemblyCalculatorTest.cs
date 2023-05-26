﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
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
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailureMechanismAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismAssemblyCalculator(null);

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
            var calculator = new FailureMechanismAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        private static RiskeerFailureMechanismSectionAssemblyResult CreateSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            double probability = random.NextDouble();
            return new RiskeerFailureMechanismSectionAssemblyResult(probability, probability + 0.001, random.NextDouble(),
                                                                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
        }

        # region Assemble

        [Test]
        public void Assemble_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new FailureMechanismAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.Assemble(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionAssemblyResults", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Assemble_WithValidInput_SendsCorrectInputToKernel()
        {
            // Setup
            var random = new Random(21);
            RiskeerFailureMechanismSectionAssemblyResult[] sectionAssemblyResults =
            {
                CreateSectionAssemblyResult(random.Next()),
                CreateSectionAssemblyResult(random.Next())
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ProbabilityResult = new Probability(random.NextDouble());

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.Assemble(sectionAssemblyResults);

                // Assert
                Assert.AreEqual(0, kernel.LenghtEffectFactor);
                Assert.IsFalse(kernel.PartialAssembly);

                CollectionAssert.AreEqual(sectionAssemblyResults.Select(r => new Probability(r.SectionProbability)),
                                          kernel.FailureMechanismSectionProbabilities);
            }
        }

        [Test]
        public void Assemble_WithValidOutput_ReturnsExpectedOutput()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                var output = new Probability(random.NextDouble());
                kernel.ProbabilityResult = output;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                FailureMechanismAssemblyResultWrapper assemblyResultWrapper = calculator.Assemble(Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsTrue(kernel.Calculated);
                ProbabilityAssert.AreEqual(assemblyResultWrapper.AssemblyResult, output);
                Assert.AreEqual(assemblyResultWrapper.AssemblyMethod, AssemblyMethod.BOI1A1);
            }
        }

        [Test]
        public void Assemble_KernelThrowsException_ThrowsFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
                Assert.IsFalse(kernel.Calculated);
            }
        }

        [Test]
        public void Assemble_KernelThrowsAssemblyException_ThrowsFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
                Assert.IsFalse(kernel.Calculated);
            }
        }

        # endregion

        #region Assemble with FailureMechanismN and ApplyLengthEffect

        [Test]
        public void AssembleWithFailureMechanismNAndApplyLengthEffect_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new FailureMechanismAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.Assemble(random.NextDouble(), null, random.NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionAssemblyResults", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetApplyLengthEffectCases))]
        public void AssembleWithFailureMechanismNAndApplyLengthEffect_WithValidInput_SendsCorrectInputToKernel(
            bool applySectionLengthEffect, Func<RiskeerFailureMechanismSectionAssemblyResult, double> getExpectedProbabilityFunc)
        {
            // Setup
            var random = new Random(21);
            double failureMechanismN = random.NextDouble();
            RiskeerFailureMechanismSectionAssemblyResult[] sectionAssemblyResults =
            {
                CreateSectionAssemblyResult(random.Next()),
                CreateSectionAssemblyResult(random.Next())
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ProbabilityResult = new Probability(random.NextDouble());

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.Assemble(failureMechanismN, sectionAssemblyResults, applySectionLengthEffect);

                // Assert
                Assert.AreEqual(failureMechanismN, kernel.LenghtEffectFactor);
                Assert.IsFalse(kernel.PartialAssembly);
                Assert.AreEqual(sectionAssemblyResults.Length, kernel.FailureMechanismSectionAssemblyResults.Count());

                CollectionAssert.AreEqual(sectionAssemblyResults.Select(sr => new Probability(getExpectedProbabilityFunc(sr))),
                                          kernel.FailureMechanismSectionAssemblyResults);
            }
        }

        [Test]
        public void AssembleWithFailureMechanismNAndApplyLengthEffect_WithValidOutput_ReturnsExpectedOutput()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                var output = new Probability(random.NextDouble());
                kernel.ProbabilityResult = output;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                FailureMechanismAssemblyResultWrapper assemblyResultWrapper = calculator.Assemble(
                    random.NextDouble(), Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>(), true);

                // Assert
                Assert.IsTrue(kernel.Calculated);
                ProbabilityAssert.AreEqual(assemblyResultWrapper.AssemblyResult, output);
                Assert.AreEqual(assemblyResultWrapper.AssemblyMethod, AssemblyMethod.BOI1A2);
            }
        }

        [Test]
        public void AssembleWithFailureMechanismNAndApplyLengthEffect_KernelThrowsException_ThrowsFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(random.NextDouble(), Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>(),
                                                   random.NextBoolean());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
                Assert.IsFalse(kernel.Calculated);
            }
        }

        [Test]
        public void AssembleWithFailureMechanismNAndApplyLengthEffect_KernelThrowsAssemblyException_ThrowsFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(random.NextDouble(), Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>(),
                                                   random.NextBoolean());

                // Assert
                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
                Assert.IsFalse(kernel.Calculated);
            }
        }

        private static IEnumerable<TestCaseData> GetApplyLengthEffectCases()
        {
            return new[]
            {
                new TestCaseData(true, new Func<RiskeerFailureMechanismSectionAssemblyResult, double>(sr => sr.ProfileProbability)),
                new TestCaseData(false, new Func<RiskeerFailureMechanismSectionAssemblyResult, double>(sr => sr.SectionProbability))
            };
        }

        #endregion
    }
}