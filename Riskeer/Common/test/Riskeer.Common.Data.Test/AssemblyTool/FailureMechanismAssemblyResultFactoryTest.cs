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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class FailureMechanismAssemblyResultFactoryTest
    {
        [Test]
        public void AssembleFailureMechanism_FailureMechanismSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(0, null, false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();
            bool applyLengthEffect = random.NextBoolean();
            IEnumerable<FailureMechanismSectionAssemblyResult> sectionResults = Enumerable.Empty<FailureMechanismSectionAssemblyResult>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(n, sectionResults, applyLengthEffect);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                Assert.AreEqual(n, calculator.FailureMechanismN);
                Assert.AreSame(sectionResults, calculator.SectionAssemblyResultsInput);
                Assert.AreEqual(applyLengthEffect, calculator.ApplyLengthEffect);
            }
        }

        [Test]
        public void AssembleFailureMechanism_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(21);
            double expectedAssemblyResult = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.AssemblyResultOutput = expectedAssemblyResult;

                // Call
                double assemblyResult = FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(
                    0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>(), false);

                // Assert
                Assert.AreEqual(expectedAssemblyResult, assemblyResult);
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

                // Call
                void Call() => FailureMechanismAssemblyResultFactory.AssembleFailureMechanism(
                    0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>(), false);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}