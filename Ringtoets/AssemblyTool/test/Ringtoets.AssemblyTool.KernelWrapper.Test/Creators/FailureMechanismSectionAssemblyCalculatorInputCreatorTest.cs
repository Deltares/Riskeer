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
using System.ComponentModel;
using System.Linq;
using AssemblyTool.Kernel.Assembly.CalculatorInput;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorInputCreatorTest
    {
        [Test]
        public void CreateSimpleCalculationResult_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult((SimpleAssessmentResultType) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, SimpleCalculationResult.None)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, SimpleCalculationResult.NVT)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, SimpleCalculationResult.FV)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, SimpleCalculationResult.VB)]
        public void CreateSimpleCalculationResult_ValidData_ReturnSimpleCalculationResult(SimpleAssessmentResultType originalResult,
                                                                                          SimpleCalculationResult expectedResult)
        {
            // Call
            SimpleCalculationResult result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResult(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateSimpleCalculationResultValidityOnly_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResultValidityOnly((SimpleAssessmentResultValidityOnlyType) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentResultValidityOnlyType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultValidityOnlyType.None, SimpleCalculationResultValidityOnly.None)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.NotApplicable, SimpleCalculationResultValidityOnly.NVT)]
        [TestCase(SimpleAssessmentResultValidityOnlyType.Applicable, SimpleCalculationResultValidityOnly.WVT)]
        public void CreateSimpleCalculationResultValidityOnly_ValidData_ReturnSimpleCalculationResultValidityOnly(
            SimpleAssessmentResultValidityOnlyType originalResult,
            SimpleCalculationResultValidityOnly expectedResult)
        {
            // Call
            SimpleCalculationResultValidityOnly result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateSimpleCalculationResultValidityOnly(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateDetailedCalculationInputFromProbability_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromProbability(
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                                random.NextDouble(),
                                                                (FailureMechanismSectionAssemblyCategoryGroup) 99)
                });

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void CreateDetailedCalculationInputFromProbability_CategoriesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromProbability(
                new Random(39).NextDouble(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, FailureMechanismSectionCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, FailureMechanismSectionCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, FailureMechanismSectionCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionCategoryGroup.VIIv)]
        public void CreateDetailedCalculationInputFromProbability_ValidData_ReturnSimpleCalculationResult(
            FailureMechanismSectionAssemblyCategoryGroup originalResult,
            FailureMechanismSectionCategoryGroup expectedResult)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);

            // Call
            DetailedCalculationInputFromProbability result =
                FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromProbability(
                    probability,
                    new[]
                    {
                        new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                    upperBoundary,
                                                                    originalResult)
                    });

            // Assert
            Assert.AreEqual(probability, result.Probability);

            FailureMechanismSectionCategory actualCategory = result.Categories.Single();
            Assert.AreEqual(expectedResult, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        public void CreateDetailedCalculationInputFromProbabilityWithLengthEffect_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromProbabilityWithLengthEffect(
                random.NextDouble(),
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(random.NextDouble(),
                                                                random.NextDouble(),
                                                                (FailureMechanismSectionAssemblyCategoryGroup) 99)
                },
                random.NextDouble());

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void CreateDetailedCalculationInputFromProbabilityWithLengthEffect_CategoriesNull_ThrowArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromProbabilityWithLengthEffect(
                random.NextDouble(),
                null,
                random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, FailureMechanismSectionCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, FailureMechanismSectionCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, FailureMechanismSectionCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionCategoryGroup.VIIv)]
        public void CreateDetailedCalculationInputFromProbabilityWithLengthEffect_ValidData_ReturnSimpleCalculationResult(
            FailureMechanismSectionAssemblyCategoryGroup originalResult,
            FailureMechanismSectionCategoryGroup expectedResult)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);
            double n = random.NextDouble(1.0, 10.0);

            // Call
            DetailedCalculationInputFromProbabilityWithLengthEffect result =
                FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromProbabilityWithLengthEffect(
                    probability,
                    new[]
                    {
                        new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                    upperBoundary,
                                                                    originalResult)
                    },
                    n);

            // Assert
            Assert.AreEqual(probability, result.Probability);
            Assert.AreEqual(n, result.NValue);

            FailureMechanismSectionCategory actualCategory = result.Categories.Single();
            Assert.AreEqual(expectedResult, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        public void CreateCombinedAssemblyCalculationInput_SimpleAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCombinedAssemblyCalculationInput(
                null,
                new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.None),
                new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.None));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("simpleAssembly", exception.ParamName);
        }

        [Test]
        public void CreateCombinedAssemblyCalculationInput_DetailedAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCombinedAssemblyCalculationInput(
                new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.None),
                null,
                new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.None));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("detailedAssembly", exception.ParamName);
        }

        [Test]
        public void CreateCombinedAssemblyCalculationInput_TailorMadeAssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCombinedAssemblyCalculationInput(
                new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.None),
                new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.None),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("tailorMadeAssembly", exception.ParamName);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, FailureMechanismSectionCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, FailureMechanismSectionCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, FailureMechanismSectionCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionCategoryGroup.VIIv)]
        public void CreateCombinedAssemblyCalculationInput_WithInput_ReturnFailureMechanismSectionAssemblyCategoryResults(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            FailureMechanismSectionCategoryGroup expectedGroup)
        {
            // Setup
            var random = new Random(11);
            var simpleAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), originalGroup);
            var detailedAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), originalGroup);
            var tailorMadeAssembly = new FailureMechanismSectionAssembly(random.NextDouble(), originalGroup);

            // Call
            FailureMechanismSectionAssemblyCategoryResult[] input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCombinedAssemblyCalculationInput(
                simpleAssembly, detailedAssembly, tailorMadeAssembly).ToArray();

            // Assert
            Assert.AreEqual(3, input.Length);
            FailureMechanismSectionAssemblyCategoryResult simpleAssemblyInput = input[0];
            Assert.AreEqual(simpleAssembly.Probability, simpleAssemblyInput.EstimatedProbabilityOfFailure);
            Assert.AreEqual(expectedGroup, simpleAssemblyInput.CategoryGroup);
            FailureMechanismSectionAssemblyCategoryResult detailedAssemblyInput = input[1];
            Assert.AreEqual(detailedAssembly.Probability, detailedAssemblyInput.EstimatedProbabilityOfFailure);
            Assert.AreEqual(expectedGroup, detailedAssemblyInput.CategoryGroup);
            FailureMechanismSectionAssemblyCategoryResult tailorMadeAssemblyInput = input[2];
            Assert.AreEqual(tailorMadeAssembly.Probability, tailorMadeAssemblyInput.EstimatedProbabilityOfFailure);
            Assert.AreEqual(expectedGroup, tailorMadeAssemblyInput.CategoryGroup);
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismSectionAssembliesWithInvalidEnum))]
        public void CreateCombinedAssemblyCalculationInput_WithInvalidEnumInput_ThrowInvalidEnumArgumentException(
            FailureMechanismSectionAssembly simpleAssembly,
            FailureMechanismSectionAssembly detailedAssembly,
            FailureMechanismSectionAssembly tailorMadeAssembly)
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCombinedAssemblyCalculationInput(
                simpleAssembly, detailedAssembly, tailorMadeAssembly);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismSectionAssembliesWithInvalidEnum()
        {
            var random = new Random(11);

            yield return new TestCaseData(
                new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup) 99),
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()), 
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));
            yield return new TestCaseData(
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup)99),
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()));
            yield return new TestCaseData(
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                new FailureMechanismSectionAssembly(random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                new FailureMechanismSectionAssembly(random.NextDouble(), (FailureMechanismSectionAssemblyCategoryGroup)99));
        }
    }
}