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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Service.AssemblyTool;

namespace Ringtoets.Common.Service.Test.AssemblyTool
{
    [TestFixture]
    public class AssemblyToolCategoriesCalculationServiceTest
    {
        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_InputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerBoundaryNorm = random.NextDouble();
            var input = new AssemblyCategoryInput(signalingNorm, lowerBoundaryNorm);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(input);

                // Assert
                AssemblyCategoriesCalculatorInput actualInput = calculator.Input;
                Assert.AreEqual(signalingNorm, actualInput.SignalingNorm);
                Assert.AreEqual(lowerBoundaryNorm, actualInput.LowerBoundaryNorm);
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_CalculationRan_ReturnsOutput()
        {
            // Setup
            var random = new Random(11);
            double signalingNorm = random.NextDouble();
            double lowerBoundaryNorm = random.NextDouble();
            var input = new AssemblyCategoryInput(signalingNorm, lowerBoundaryNorm);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                AssessmentSectionAssemblyCategory[] output = AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(input).ToArray();

                // Assert
                AssessmentSectionAssemblyCategoryResult[] calculatorOutput = calculator.AssessmentSectionCategoriesOutput.ToArray();

                Assert.AreEqual(calculatorOutput.Length, output.Length);
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.LowerBoundary), output.Select(o => o.LowerBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => co.UpperBoundary), output.Select(o => o.UpperBoundary));
                CollectionAssert.AreEqual(calculatorOutput.Select(co => GetAssessmentSectionAssemblyCategoryType(co.Category)), output.Select(o => o.Type));
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_CalculatorThrowsException_LogErrorAndReturnEmptyOutput()
        {
            // Setup
            var input = new AssemblyCategoryInput(0, 0);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                IEnumerable<AssessmentSectionAssemblyCategory> output = null;
                Action test = () => output = AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(input);

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

        [Test]
        public void CalculateAssessmentSectionAssemblyCategories_ErrorInConversion_LogErrorAndReturnEmptyOutput()
        {
            // Setup
            var input = new AssemblyCategoryInput(0, 0);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;
                calculator.AssessmentSectionCategoriesOutput = new[]
                {
                    new AssessmentSectionAssemblyCategoryResult(0, 1, (AssessmentSectionAssemblyCategoryResultType) 99)
                };

                // Call
                IEnumerable<AssessmentSectionAssemblyCategory> output = null;
                Action test = () => output = AssemblyToolCategoriesCalculationService.CalculateAssessmentSectionAssemblyCategories(input);

                // Assert
                TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(test, tuples =>
                {
                    Tuple<string, Level, Exception>[] messages = tuples as Tuple<string, Level, Exception>[] ?? tuples.ToArray();
                    Assert.AreEqual(1, messages.Length);

                    Tuple<string, Level, Exception> tuple1 = messages[0];
                    Assert.AreEqual("Er is een onverwachte fout opgetreden bij het bepalen van categoriegrenzen.", tuple1.Item1);
                    Assert.AreEqual(Level.Error, tuple1.Item2);
                    Assert.IsInstanceOf<AssemblyCategoryConversionException>(tuple1.Item3);
                });
                CollectionAssert.IsEmpty(output);
            }
        }

        private static AssessmentSectionAssemblyCategoryType GetAssessmentSectionAssemblyCategoryType(
            AssessmentSectionAssemblyCategoryResultType categoryType)
        {
            switch (categoryType)
            {
                case AssessmentSectionAssemblyCategoryResultType.APlus:
                    return AssessmentSectionAssemblyCategoryType.APlus;
                case AssessmentSectionAssemblyCategoryResultType.A:
                    return AssessmentSectionAssemblyCategoryType.A;
                case AssessmentSectionAssemblyCategoryResultType.B:
                    return AssessmentSectionAssemblyCategoryType.B;
                case AssessmentSectionAssemblyCategoryResultType.C:
                    return AssessmentSectionAssemblyCategoryType.C;
                case AssessmentSectionAssemblyCategoryResultType.D:
                    return AssessmentSectionAssemblyCategoryType.D;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}