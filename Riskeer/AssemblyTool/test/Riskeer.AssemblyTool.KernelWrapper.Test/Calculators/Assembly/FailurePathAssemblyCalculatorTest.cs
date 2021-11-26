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
using Assembly.Kernel.Model.FailurePathSections;
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

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class FailurePathAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new FailurePathAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IFailurePathAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailurePathAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Assemble_SectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new FailurePathAssemblyCalculator(kernelFactory);

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
            double failurePathN = random.NextDouble();
            FailureMechanismSectionAssemblyResult[] sectionAssemblyResults =
            {
                CreateSectionAssemblyResult(random.Next()),
                CreateSectionAssemblyResult(random.Next())
            };

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailurePathAssemblyKernelStub kernel = factory.LastCreatedFailurePathAssemblyKernel;
                kernel.ProbabilityResult = new Probability(random.NextDouble());

                var calculator = new FailurePathAssemblyCalculator(factory);

                // Call
                calculator.Assemble(failurePathN, sectionAssemblyResults);

                // Assert
                Assert.AreEqual(failurePathN, kernel.LenghtEffectFactor);
                Assert.IsFalse(kernel.PartialAssembly);
                AssertFailurePathSectionAssemblyResults(sectionAssemblyResults, kernel.FailurePathSectionAssemblyResults);
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
                FailurePathAssemblyKernelStub kernel = factory.LastCreatedFailurePathAssemblyKernel;
                var output = new Probability(random.NextDouble());
                kernel.ProbabilityResult = output;

                var calculator = new FailurePathAssemblyCalculator(factory);

                // Call
                double assemblyResult = calculator.Assemble(random.NextDouble(), Enumerable.Empty<FailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsTrue(kernel.Calculated);
                ProbabilityAssert.AreEqual(assemblyResult, output);
            }
        }

        [Test]
        public void Assemble_WithInvalidInput_ThrowsFailurePathAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var invalidInput = new FailureMechanismSectionAssemblyResult(probability, probability, random.NextDouble(),
                                                                         (FailureMechanismSectionAssemblyGroup) 99);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailurePathAssemblyKernelStub kernel = factory.LastCreatedFailurePathAssemblyKernel;

                var calculator = new FailurePathAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(random.NextDouble(), new[]
                {
                    invalidInput
                });

                // Assert
                Assert.IsFalse(kernel.Calculated);

                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_KernelThrowsException_ThrowsFailurePathAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailurePathAssemblyKernelStub kernel = factory.LastCreatedFailurePathAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailurePathAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(random.NextDouble(), Enumerable.Empty<FailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsFalse(kernel.Calculated);

                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void Assemble_KernelThrowsAssemblyException_ThrowsFailurePathAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                FailurePathAssemblyKernelStub kernel = factory.LastCreatedFailurePathAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailurePathAssemblyCalculator(factory);

                // Call
                void Call() => calculator.Assemble(random.NextDouble(), Enumerable.Empty<FailureMechanismSectionAssemblyResult>());

                // Assert
                Assert.IsFalse(kernel.Calculated);

                var exception = Assert.Throws<FailureMechanismAssemblyCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
            }
        }

        private static FailureMechanismSectionAssemblyResult CreateSectionAssemblyResult(int seed)
        {
            var random = new Random(seed);
            double probability = random.NextDouble();
            return new FailureMechanismSectionAssemblyResult(probability, probability + 0.001, random.NextDouble(),
                                                             random.NextEnumValue<FailureMechanismSectionAssemblyGroup>());
        }

        private static void AssertFailurePathSectionAssemblyResults(IEnumerable<FailureMechanismSectionAssemblyResult> expected,
                                                                    IEnumerable<FailurePathSectionAssemblyResult> actual)
        {
            int nrOfExpectedResults = expected.Count();
            Assert.AreEqual(nrOfExpectedResults, actual.Count());

            for (int i = 0; i < nrOfExpectedResults; i++)
            {
                AssertFailurePathSectionAssemblyResult(expected.ElementAt(i), actual.ElementAt(i));
            }
        }

        private static void AssertFailurePathSectionAssemblyResult(FailureMechanismSectionAssemblyResult expected,
                                                                   FailurePathSectionAssemblyResult actual)
        {
            ProbabilityAssert.AreEqual(expected.ProfileProbability, actual.ProbabilityProfile);
            ProbabilityAssert.AreEqual(expected.SectionProbability, actual.ProbabilitySection);
            Assert.AreEqual(GetInterpretationCategory(expected.AssemblyGroup), actual.InterpretationCategory);
        }

        private static EInterpretationCategory GetInterpretationCategory(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            switch (assemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.ND:
                    return EInterpretationCategory.ND;
                case FailureMechanismSectionAssemblyGroup.III:
                    return EInterpretationCategory.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return EInterpretationCategory.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return EInterpretationCategory.I;
                case FailureMechanismSectionAssemblyGroup.ZeroPlus:
                    return EInterpretationCategory.ZeroPlus;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return EInterpretationCategory.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return EInterpretationCategory.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return EInterpretationCategory.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return EInterpretationCategory.IIIMin;
                case FailureMechanismSectionAssemblyGroup.D:
                    return EInterpretationCategory.D;
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return EInterpretationCategory.Gr;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}