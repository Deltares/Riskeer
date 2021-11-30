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
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.Exceptions;

namespace Riskeer.Common.Data.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyToolGroupLimitsFactoryTest
    {
        [Test]
        public void CreateFailureMechanismSectionAssemblyGroupBoundaries_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;

                // Call
                AssemblyToolGroupLimitsFactory.CreateFailureMechanismSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(signalingNorm, calculator.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNorm);
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyGroupBoundaries_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;

                // Call
                FailureMechanismSectionAssemblyGroupBoundaries[] output =
                    AssemblyToolGroupLimitsFactory.CreateFailureMechanismSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm)
                                                  .ToArray();

                // Assert
                FailureMechanismSectionAssemblyGroupBoundaries[] calculatorOutput = calculator.FailureMechanismSectionAssemblyGroupLimitsOutput.ToArray();

                int expectedNrOfOutputs = calculatorOutput.Length;
                Assert.AreEqual(expectedNrOfOutputs, output.Length);
                for (var i = 0; i < expectedNrOfOutputs; i++)
                {
                    FailureMechanismSectionAssemblyGroupBoundaries expectedOutput = calculatorOutput[i];
                    FailureMechanismSectionAssemblyGroupBoundaries actualOutput = calculatorOutput[i];

                    Assert.AreEqual(expectedOutput.Group, actualOutput.Group);
                    Assert.AreEqual(expectedOutput.LowerBoundary, actualOutput.LowerBoundary);
                    Assert.AreEqual(expectedOutput.UpperBoundary, actualOutput.UpperBoundary);
                }
            }
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyGroupBoundaries_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyGroupBoundariesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyGroupBoundariesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssemblyToolGroupLimitsFactory.CreateFailureMechanismSectionAssemblyGroupBoundaries(0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}