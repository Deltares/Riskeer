// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class FailureMechanismAssemblyResultFactoryTest
    {
        [Test]
        public void AssembleFailureMechanism_FailureMechanismSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(0, null, false, new FailureMechanismAssemblyResult());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_FailureMechanismAssemblyResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(0, Array.Empty<FailureMechanismSectionAssemblyResult>(), false, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismAssemblyResult", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_WithInputWithProbabilityResultTypeAutomaticIndependentSections_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();
            var failureMechanismResult = new FailureMechanismAssemblyResult
            {
                ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P1
            };
            bool applyLengthEffect = random.NextBoolean();
            IEnumerable<FailureMechanismSectionAssemblyResult> sectionResults = Enumerable.Empty<FailureMechanismSectionAssemblyResult>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(n, sectionResults, applyLengthEffect, failureMechanismResult);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                Assert.AreSame(sectionResults, calculator.SectionAssemblyResultsInput);
                Assert.IsFalse(calculator.AssembleWithWorstSectionResultCalled);
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithInputWithProbabilityResultTypeAutomaticWorstSectionOrProfile_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();
            var failureMechanismResult = new FailureMechanismAssemblyResult
            {
                ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P2
            };
            bool applyLengthEffect = random.NextBoolean();
            IEnumerable<FailureMechanismSectionAssemblyResult> sectionResults = Enumerable.Empty<FailureMechanismSectionAssemblyResult>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(n, sectionResults, applyLengthEffect, failureMechanismResult);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                Assert.AreSame(sectionResults, calculator.SectionAssemblyResultsInput);
                Assert.IsTrue(calculator.AssembleWithWorstSectionResultCalled);
            }
        }

        [Test]
        public void AssembleFailureMechanism_WithInputWithProbabilityResultTypeNone_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();
            var failureMechanismResult = new FailureMechanismAssemblyResult
            {
                ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.None
            };
            bool applyLengthEffect = random.NextBoolean();
            IEnumerable<FailureMechanismSectionAssemblyResult> sectionResults = Enumerable.Empty<FailureMechanismSectionAssemblyResult>();

            // Call
            void Call() => FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(n, sectionResults, applyLengthEffect, failureMechanismResult);

            // Assert
            var exception = Assert.Throws<AssemblyException>(Call);
            Assert.AreEqual("Er ontbreekt invoer voor de assemblage rekenmodule waardoor de assemblage niet uitgevoerd kan worden.", exception.Message);
        }

        [Test]
        public void AssembleFailureMechanism_WithInputWithProbabilityResultTypeManual_NoInputOnCalculatorAndReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            var failureMechanismResult = new FailureMechanismAssemblyResult
            {
                ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.Manual,
                ManualFailureMechanismAssemblyProbability = random.NextDouble()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailureMechanismAssemblyResultWrapper assemblyResult = FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(
                    random.NextDouble(), Enumerable.Empty<FailureMechanismSectionAssemblyResult>(),
                    random.NextBoolean(), failureMechanismResult);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                Assert.IsNull(calculator.SectionAssemblyResultsInput);
                Assert.IsFalse(calculator.AssembleWithWorstSectionResultCalled);

                Assert.AreEqual(failureMechanismResult.ManualFailureMechanismAssemblyProbability, assemblyResult.AssemblyResult);
                Assert.AreEqual(AssemblyMethod.Manual, assemblyResult.AssemblyMethod);
            }
        }

        [Test]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P1)]
        [TestCase(FailureMechanismAssemblyProbabilityResultType.P2)]
        public void AssembleFailureMechanism_CalculatorRan_ReturnsOutput(FailureMechanismAssemblyProbabilityResultType failureMechanismAssemblyProbabilityResultType)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                var failureMechanismAssemblyResult = new FailureMechanismAssemblyResult
                {
                    ProbabilityResultType = failureMechanismAssemblyProbabilityResultType
                };

                // Call
                FailureMechanismAssemblyResultWrapper assemblyResult = FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(
                    0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>(), false, failureMechanismAssemblyResult);

                // Assert
                Assert.AreSame(calculator.AssemblyResultOutput, assemblyResult);
            }
        }

        [Test]
        public void AssembleFailureMechanism_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                var failureMechanismAssemblyResult = new FailureMechanismAssemblyResult
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P1
                };

                // Call
                void Call() =>
                    FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(
                        0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>(), false, failureMechanismAssemblyResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        [Test]
        public void AssembleFailureMechanism_NotSupportedProbabilityResultType_ThrowsNotSupportedException()
        {
            // Setup
            var failureMechanismAssemblyResult = new FailureMechanismAssemblyResult
            {
                ProbabilityResultType = 0
            };

            // Call
            void Call() => FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(
                0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>(), false, failureMechanismAssemblyResult);

            // Assert
            Assert.Throws<NotSupportedException>(Call);
        }
    }
}