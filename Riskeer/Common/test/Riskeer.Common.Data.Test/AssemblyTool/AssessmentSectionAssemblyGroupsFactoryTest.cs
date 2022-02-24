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
    public class AssessmentSectionAssemblyGroupsFactoryTest
    {
        [Test]
        public void CreateAssessmentSectionAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssessmentSectionAssemblyGroupsFactory.CreateAssessmentSectionAssemblyCategories(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(signalingNorm, calculator.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNorm);
            }
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CalculatorRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssessmentSectionAssemblyGroupBoundaries[] output = AssessmentSectionAssemblyGroupsFactory.CreateAssessmentSectionAssemblyCategories(signalingNorm, lowerLimitNorm).ToArray();

                // Assert
                AssessmentSectionAssemblyGroupBoundaries[] calculatorOutput = calculator.AssessmentSectionCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CreateAssessmentSectionAssemblyCategories_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => AssessmentSectionAssemblyGroupsFactory.CreateAssessmentSectionAssemblyCategories(0, 0);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }
    }
}