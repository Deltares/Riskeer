// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Assembly.Kernel.Model.Categories;
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
using KernelFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
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

        [Test]
        public void Assemble_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new FailureMechanismAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.Assemble(random.NextDouble(), null);

            // Assert
            Assert.That(Call, Throws.TypeOf<ArgumentNullException>()
                                    .With.Property(nameof(ArgumentNullException.ParamName))
                                    .EqualTo("sectionAssemblyResults"));
            mocks.VerifyAll();
        }

        [Test]
        public void Assemble_WithValidInput_SendsCorrectInputToKernel()
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
                kernel.ProbabilityResult = new FailureMechanismAssemblyResult(new Probability(random.NextDouble()), EFailureMechanismAssemblyMethod.Correlated);

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                calculator.Assemble(failureMechanismN, sectionAssemblyResults);

                // Assert
                Assert.AreEqual(failureMechanismN, kernel.LenghtEffectFactor);
                Assert.IsFalse(kernel.PartialAssembly);
                AssertFailureMechanismSectionAssemblyResults(sectionAssemblyResults, kernel.FailureMechanismSectionAssemblyResults);
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
                var output = new FailureMechanismAssemblyResult(new Probability(random.NextDouble()), EFailureMechanismAssemblyMethod.Correlated);
                kernel.ProbabilityResult = output;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                double assemblyResult = calculator.Assemble(random.NextDouble(), Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsTrue(kernel.Calculated);
                ProbabilityAssert.AreEqual(assemblyResult, output.Probability);
            }
        }

        [Test]
        public void Assemble_WithInvalidInput_ThrowsFailureMechanismAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var invalidInput = new RiskeerFailureMechanismSectionAssemblyResult(probability, probability, random.NextDouble(),
                                                                                (FailureMechanismSectionAssemblyGroup) 99);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailureMechanismAssemblyKernelStub kernel = factory.LastCreatedFailureMechanismAssemblyKernel;

                var calculator = new FailureMechanismAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(random.NextDouble(), new[]
                {
                    invalidInput
                });

                // Assert
                Assert.IsFalse(kernel.Calculated);

                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_KernelThrowsException_ThrowsFailureMechanismAssemblyCalculatorException()
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
                void Call() => calculator.Assemble(random.NextDouble(), Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsFalse(kernel.Calculated);

                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_KernelThrowsAssemblyException_ThrowsFailureMechanismAssemblyCalculatorException()
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
                void Call() => calculator.Assemble(random.NextDouble(), Enumerable.Empty<RiskeerFailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsFalse(kernel.Calculated);

                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
            }
        }

        private static RiskeerFailureMechanismSectionAssemblyResult CreateSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            double probability = random.NextDouble();
            return new RiskeerFailureMechanismSectionAssemblyResult(probability, probability + 0.001, random.NextDouble(),
                                                                    random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
        }

        private static void AssertFailureMechanismSectionAssemblyResults(IEnumerable<RiskeerFailureMechanismSectionAssemblyResult> expected,
                                                                         IEnumerable<KernelFailureMechanismSectionAssemblyResult> actual)
        {
            int nrOfExpectedResults = expected.Count();
            Assert.AreEqual(nrOfExpectedResults, actual.Count());

            for (int i = 0; i < nrOfExpectedResults; i++)
            {
                AssertFailureMechanismSectionAssemblyResult(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        private static void AssertFailureMechanismSectionAssemblyResult(RiskeerFailureMechanismSectionAssemblyResult expected,
                                                                        KernelFailureMechanismSectionAssemblyResult actual)
        {
            ProbabilityAssert.AreEqual(expected.ProfileProbability, actual.ProbabilityProfile);
            ProbabilityAssert.AreEqual(expected.SectionProbability, actual.ProbabilitySection);
            Assert.AreEqual(GetInterpretationCategory(expected.FailureMechanismSectionAssemblyGroup), actual.InterpretationCategory);
        }

        private static EInterpretationCategory GetInterpretationCategory(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            switch (assemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.NotDominant:
                    return EInterpretationCategory.NotDominant;
                case FailureMechanismSectionAssemblyGroup.III:
                    return EInterpretationCategory.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return EInterpretationCategory.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return EInterpretationCategory.I;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return EInterpretationCategory.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return EInterpretationCategory.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return EInterpretationCategory.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return EInterpretationCategory.IIIMin;
                case FailureMechanismSectionAssemblyGroup.Dominant:
                    return EInterpretationCategory.Dominant;
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return EInterpretationCategory.NoResult;
                case FailureMechanismSectionAssemblyGroup.NotRelevant:
                    return EInterpretationCategory.NotRelevant;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}