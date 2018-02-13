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
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Service.AssemblyTool;

namespace Ringtoets.Common.Service.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyToolCategoriesCalculationServiceTest
    {
        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(signalingNorm, calculator.SignalingNorm);
                Assert.AreEqual(lowerLimitNorm, calculator.LowerLimitNorm);
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_CalculationRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssessmentSectionAssemblyCategory[] output = AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(signalingNorm, lowerLimitNorm).ToArray();

                // Assert
                AssessmentSectionAssemblyCategory[] calculatorOutput = calculator.AssessmentSectionCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.Group), output.Select(o => o.Group));
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_CalculatorThrowsException_LogErrorAndReturnEmptyOutput()
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                IEnumerable<AssessmentSectionAssemblyCategory> output = null;
                Action test = () => output = AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(0, 0);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(test, tuples =>
                {
                    Tuple<string, Level, Exception>[] messages = tuples as Tuple<string, Level, Exception>[] ?? tuples.ToArray();
                    Assert.AreEqual(1, messages.Length);

                    Tuple<string, Level, Exception> tuple1 = messages[0];
                    Assert.AreEqual("Er is een onverwachte fout opgetreden bij het bepalen van categoriegrenzen.", tuple1.Item1);
                    Assert.AreEqual(Level.Error, tuple1.Item2);
                    Assert.IsInstanceOf<AssemblyCategoriesCalculatorException>(tuple1.Item3);
                });
                CollectionAssert.IsEmpty(output);
            }
        }
    }
}