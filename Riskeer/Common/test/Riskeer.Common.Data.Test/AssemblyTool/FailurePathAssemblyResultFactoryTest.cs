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
    public class FailurePathAssemblyResultFactoryTest
    {
        [Test]
        public void AssembleFailurePath_FailureMechanismSectionAssemblyResultsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailurePathAssemblyResultFactory.AssemblyFailurePath(0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismSectionAssemblyResults", exception.ParamName);
        }

        [Test]
        public void AssembleFailurePath_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();
            IEnumerable<FailureMechanismSectionAssemblyResult> sectionResults = Enumerable.Empty<FailureMechanismSectionAssemblyResult>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                FailurePathAssemblyResultFactory.AssemblyFailurePath(n, sectionResults);

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailurePathAssemblyCalculator;

                Assert.AreEqual(n, calculator.FailurePathN);
                Assert.AreSame(sectionResults, calculator.SectionAssemblyResultsInput);
            }
        }

        [Test]
        public void AssembleFailurePath_CalculatorRan_ReturnsOutput()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                double assemblyResult = FailurePathAssemblyResultFactory.AssemblyFailurePath(
                    0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>());

                // Assert
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailurePathAssemblyCalculator;

                Assert.AreEqual(calculator.AssemblyResult, assemblyResult);
            }
        }

        [Test]
        public void AssembleFailurePath_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailurePathAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailurePathAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => FailurePathAssemblyResultFactory.AssemblyFailurePath(
                    0, Enumerable.Empty<FailureMechanismSectionAssemblyResult>());

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailurePathAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}