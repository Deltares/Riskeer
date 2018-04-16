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
using Assembly.Kernel.Model.AssessmentResultTypes;
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
        #region Simple Assessment

        [Test]
        public void CreateAssessmentResultE1_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultE1((SimpleAssessmentResultType) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, EAssessmentResultTypeE1.Gr)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, EAssessmentResultTypeE1.Nvt)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, EAssessmentResultTypeE1.Fv)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, EAssessmentResultTypeE1.Vb)]
        public void CreateAssessmentResultE1_ValidData_ReturnSimpleCalculationResult(SimpleAssessmentResultType originalResult,
                                                                                     EAssessmentResultTypeE1 expectedResult)
        {
            // Call
            EAssessmentResultTypeE1 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultE1(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateAssessmentResultTypeE2_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE2((SimpleAssessmentValidityOnlyResultType) 99);

            // Assert
            const string expectedMessage = "The value of argument 'input' (99) is invalid for Enum type 'SimpleAssessmentValidityOnlyResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None, EAssessmentResultTypeE2.Gr)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.NotApplicable, EAssessmentResultTypeE2.Nvt)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable, EAssessmentResultTypeE2.Wvt)]
        public void CreateAssessmentResultTypeE2_ValidData_ReturnSimpleCalculationResultValidityOnly(
            SimpleAssessmentValidityOnlyResultType originalResult,
            EAssessmentResultTypeE2 expectedResult)
        {
            // Call
            EAssessmentResultTypeE2 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE2(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        #endregion

        #region Detailed Assessment

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
        public void CreateDetailedCalculationInputFromProbability_ValidData_ReturnDetailedCalculationInputFromProbabilityWithLengthEffect(
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
        public void CreateDetailedCalculationInputFromProbabilityWithLengthEffect_ValidData_ReturnDetailedCalculationInputFromProbabilityWithLengthEffect(
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
        [TestCaseSource(nameof(InvalidDetailedAssessmentCategoryResults))]
        public void CreateDetailedCalculationInputFromCategoryResults_InvalidEnumInput_ThrowInvalidEnumArgumentException(
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm)
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromCategoryResults(
                detailedAssessmentResultForFactorizedSignalingNorm,
                detailedAssessmentResultForSignalingNorm,
                detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                detailedAssessmentResultForLowerLimitNorm,
                detailedAssessmentResultForFactorizedLowerLimitNorm);

            // Assert
            string expectedMessage = $"The value of argument 'detailedAssessmentResult' (99) is invalid for Enum type '{nameof(DetailedAssessmentResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void CreateDetailedCalculationInputFromCategoryResults_ValidInput_ReturnsDetailedCategoryBoundariesCalculationResult()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();

            // Call
            DetailedCategoryBoundariesCalculationResult result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromCategoryResults(
                detailedAssessmentResultForFactorizedSignalingNorm,
                detailedAssessmentResultForSignalingNorm,
                detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                detailedAssessmentResultForLowerLimitNorm,
                detailedAssessmentResultForFactorizedLowerLimitNorm);

            // Assert
            Assert.AreEqual(result.ResultItoII, GetAssessmentResultTypeG1(detailedAssessmentResultForFactorizedSignalingNorm));
            Assert.AreEqual(result.ResultIItoIII, GetAssessmentResultTypeG1(detailedAssessmentResultForSignalingNorm));
            Assert.AreEqual(result.ResultIIItoIV, GetAssessmentResultTypeG1(detailedAssessmentResultForMechanismSpecificLowerLimitNorm));
            Assert.AreEqual(result.ResultIVtoV, GetAssessmentResultTypeG1(detailedAssessmentResultForLowerLimitNorm));
            Assert.AreEqual(result.ResultVtoVI, GetAssessmentResultTypeG1(detailedAssessmentResultForFactorizedLowerLimitNorm));
        }

        [Test]
        public void CreateAssessmentResultTypeG1_InvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG1((DetailedAssessmentResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'detailedAssessmentResult' (99) is invalid for Enum type '{nameof(DetailedAssessmentResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(DetailedAssessmentResultType.None)]
        [TestCase(DetailedAssessmentResultType.Insufficient)]
        [TestCase(DetailedAssessmentResultType.Sufficient)]
        [TestCase(DetailedAssessmentResultType.NotAssessed)]
        public void CreateAssessmentResultTypeG1_ValidInput_ReturnsDetailedCalculationResult(DetailedAssessmentResultType detailedAssessmentResult)
        {
            // Call
            EAssessmentResultTypeG1 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG1(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(result, GetAssessmentResultTypeG1(detailedAssessmentResult));
        }

        [Test]
        public void CreateAssessmentResultTypeG2_InvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(
                (DetailedAssessmentProbabilityOnlyResultType) 99);

            // Assert
            string expectedMessage = "The value of argument 'detailedAssessmentResult' (99) is invalid for Enum type " +
                                     $"'{nameof(DetailedAssessmentProbabilityOnlyResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.Probability)]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.NotAssessed)]
        public void CreateAssessmentResultTypeG2_ValidInput_ReturnsDetailedCalculationResult(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult)
        {
            // Call
            EAssessmentResultTypeG2 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(result, GetAssessmentResultTypeG2(detailedAssessmentResult));
        }


        private static IEnumerable<TestCaseData> InvalidDetailedAssessmentCategoryResults
        {
            get
            {
                var random = new Random(39);
                yield return new TestCaseData((DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>());
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>());
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>());
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>());
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99);
            }
        }

        private static EAssessmentResultTypeG1 GetAssessmentResultTypeG1(DetailedAssessmentResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentResultType.None:
                    return EAssessmentResultTypeG1.Gr;
                case DetailedAssessmentResultType.Sufficient:
                    return EAssessmentResultTypeG1.V;
                case DetailedAssessmentResultType.Insufficient:
                    return EAssessmentResultTypeG1.Vn;
                case DetailedAssessmentResultType.NotAssessed:
                    return EAssessmentResultTypeG1.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        private static EAssessmentResultTypeG2 GetAssessmentResultTypeG2(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentProbabilityOnlyResultType.Probability:
                    return EAssessmentResultTypeG2.ResultSpecified;
                case DetailedAssessmentProbabilityOnlyResultType.NotAssessed:
                    return EAssessmentResultTypeG2.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region Failure Mechanism Section Assembly

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategoryResult_AssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assembly", exception.ParamName);
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
        public void CreateFailureMechanismSectionAssemblyCategoryResult_WithAssembly_ReturnFailureMechanismSectionAssemblyCategoryResult(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            FailureMechanismSectionCategoryGroup expectedGroup)
        {
            // Setup
            var random = new Random(11);
            var assembly = new FailureMechanismSectionAssembly(random.NextDouble(), originalGroup);

            // Call
            FailureMechanismSectionAssemblyCategoryResult input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(
                assembly);

            // Assert
            Assert.AreEqual(assembly.Probability, input.EstimatedProbabilityOfFailure);
            Assert.AreEqual(expectedGroup, input.CategoryGroup);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyCategoryResult_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(
                new FailureMechanismSectionAssembly(0, (FailureMechanismSectionAssemblyCategoryGroup) 99));

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
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
        public void ConvertFailureMechanismSectionAssemblyCategoryGroup_ValidGroup_ReturnFailureMechanismSectionCategoryGroup(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            FailureMechanismSectionCategoryGroup expectedGroup)
        {
            // Call
            FailureMechanismSectionCategoryGroup actualGroup = FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionAssemblyCategoryGroup(
                originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, actualGroup);
        }

        [Test]
        public void ConvertFailureMechanismSectionAssemblyCategoryGroup_InvalidGroup_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionAssemblyCategoryGroup(
                (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void CreateTailorMadeCalculationInputFromProbability_CategoriesNull_ThrowArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbability_WithInvalidResultEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(
                (TailorMadeAssessmentProbabilityCalculationResultType) 99,
                new Random(39).NextDouble(),
                Enumerable.Empty<FailureMechanismSectionAssemblyCategory>());

            // Assert
            const string expectedMessage = "The value of argument 'tailorMadeAssessmentResult' (99) is invalid for Enum type 'TailorMadeAssessmentProbabilityCalculationResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbability_WithInvalidCategoryEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
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
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, FailureMechanismSectionCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, FailureMechanismSectionCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, FailureMechanismSectionCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionCategoryGroup.VIIv)]
        public void CreateTailorMadeCalculationInputFromProbability_ValidData_ReturnTailorMadeCalculationInputFromProbabilityWithCorrectCategories(
            FailureMechanismSectionAssemblyCategoryGroup originalResult,
            FailureMechanismSectionCategoryGroup expectedResult)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);

            // Call
            TailorMadeCalculationInputFromProbability result =
                FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    probability,
                    new[]
                    {
                        new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                    upperBoundary,
                                                                    originalResult)
                    });

            // Assert
            FailureMechanismSectionCategory actualCategory = result.Categories.Single();
            Assert.AreEqual(expectedResult, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.None, TailorMadeProbabilityCalculationResultGroup.None)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible, TailorMadeProbabilityCalculationResultGroup.FV)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed, TailorMadeProbabilityCalculationResultGroup.NGO)]
        public void CreateTailorMadeCalculationInputFromProbability_ValidDataNotProbability_ReturnTailorMadeCalculationInputFromProbability(
            TailorMadeAssessmentProbabilityCalculationResultType originalResult,
            TailorMadeProbabilityCalculationResultGroup expectedResult)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);
            const FailureMechanismSectionAssemblyCategoryGroup categoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IIv;

            // Call
            TailorMadeCalculationInputFromProbability input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(
                originalResult,
                probability,
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                upperBoundary,
                                                                categoryGroup)
                });

            // Assert
            Assert.AreEqual(expectedResult, input.Result.CalculationResultGroup);
            Assert.AreEqual(0.0, input.Result.Probability);

            FailureMechanismSectionCategory actualCategory = input.Categories.Single();
            Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbability_ValidDataProbability_ReturnTailorMadeCalculationInputFromProbability()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);
            const FailureMechanismSectionAssemblyCategoryGroup categoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IIv;

            // Call
            TailorMadeCalculationInputFromProbability input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(
                TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                probability,
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                upperBoundary,
                                                                categoryGroup)
                });

            // Assert
            Assert.AreEqual(TailorMadeProbabilityCalculationResultGroup.Probability, input.Result.CalculationResultGroup);
            Assert.AreEqual(probability, input.Result.Probability);

            FailureMechanismSectionCategory actualCategory = input.Categories.Single();
            Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor_CategoriesNull_ThrowArgumentNullException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                random.NextDouble(),
                null,
                random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("categories", exception.ParamName);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor_WithInvalidResultEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                (TailorMadeAssessmentProbabilityCalculationResultType) 99,
                random.NextDouble(),
                Enumerable.Empty<FailureMechanismSectionAssemblyCategory>(),
                random.NextDouble());

            // Assert
            const string expectedMessage = "The value of argument 'tailorMadeAssessmentResult' (99) is invalid for Enum type 'TailorMadeAssessmentProbabilityCalculationResultType'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor_WithInvalidCategoryEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(39);

            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
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
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, FailureMechanismCategoryGroup.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, FailureMechanismSectionCategoryGroup.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, FailureMechanismSectionCategoryGroup.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, FailureMechanismSectionCategoryGroup.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, FailureMechanismSectionCategoryGroup.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, FailureMechanismSectionCategoryGroup.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, FailureMechanismSectionCategoryGroup.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, FailureMechanismSectionCategoryGroup.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, FailureMechanismSectionCategoryGroup.VIIv)]
        public void CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor_ValidData_ReturnTailorMadeCalculationInputFromProbabilityWithLengthEffectFactorWithCorrectCategories(
            FailureMechanismSectionAssemblyCategoryGroup originalResult,
            FailureMechanismSectionCategoryGroup expectedResult)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);

            // Call
            TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor result =
                FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                    random.NextEnumValue<TailorMadeAssessmentProbabilityCalculationResultType>(),
                    probability,
                    new[]
                    {
                        new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                    upperBoundary,
                                                                    originalResult)
                    },
                    random.NextDouble(1.0, 10.0));

            // Assert
            FailureMechanismSectionCategory actualCategory = result.Categories.Single();
            Assert.AreEqual(expectedResult, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.None, TailorMadeProbabilityCalculationResultGroup.None)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible, TailorMadeProbabilityCalculationResultGroup.FV)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed, TailorMadeProbabilityCalculationResultGroup.NGO)]
        public void CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor_ValidDataNotProbability_ReturnTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
            TailorMadeAssessmentProbabilityCalculationResultType originalResult,
            TailorMadeProbabilityCalculationResultGroup expectedResult)
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);
            double n = random.NextDouble(1.0, 10.0);
            const FailureMechanismSectionAssemblyCategoryGroup categoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IIv;

            // Call
            TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                originalResult,
                probability,
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                upperBoundary,
                                                                categoryGroup)
                },
                n);

            // Assert
            Assert.AreEqual(expectedResult, input.Result.CalculationResultGroup);
            Assert.AreEqual(0.0, input.Result.Probability);
            Assert.AreEqual(n, input.NValue);

            FailureMechanismSectionCategory actualCategory = input.Categories.Single();
            Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        public void CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor_ValidDataProbability_ReturnTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor()
        {
            // Setup
            var random = new Random(39);
            double probability = random.NextDouble();
            double lowerBoundary = random.NextDouble(0.0, 0.5);
            double upperBoundary = random.NextDouble(0.6, 1.0);
            double n = random.NextDouble(1.0, 10.0);
            const FailureMechanismSectionAssemblyCategoryGroup categoryGroup = FailureMechanismSectionAssemblyCategoryGroup.IIv;

            // Call
            TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                probability,
                new[]
                {
                    new FailureMechanismSectionAssemblyCategory(lowerBoundary,
                                                                upperBoundary,
                                                                categoryGroup)
                },
                n);

            // Assert
            Assert.AreEqual(TailorMadeProbabilityCalculationResultGroup.Probability, input.Result.CalculationResultGroup);
            Assert.AreEqual(probability, input.Result.Probability);
            Assert.AreEqual(n, input.NValue);

            FailureMechanismSectionCategory actualCategory = input.Categories.Single();
            Assert.AreEqual(FailureMechanismSectionCategoryGroup.IIv, actualCategory.CategoryGroup);
            Assert.AreEqual(lowerBoundary, actualCategory.LowerBoundary);
            Assert.AreEqual(upperBoundary, actualCategory.UpperBoundary);
        }

        [Test]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, TailorMadeCategoryCalculationResult.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, TailorMadeCategoryCalculationResult.None)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, TailorMadeCategoryCalculationResult.FV)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, TailorMadeCategoryCalculationResult.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, TailorMadeCategoryCalculationResult.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, TailorMadeCategoryCalculationResult.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, TailorMadeCategoryCalculationResult.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, TailorMadeCategoryCalculationResult.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, TailorMadeCategoryCalculationResult.NGO)]
        public void ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup_ValidGroup_ReturnFailureMechanismSectionCategoryGroup(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            TailorMadeCategoryCalculationResult expectedGroup)
        {
            // Call
            TailorMadeCategoryCalculationResult actualGroup = FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup(
                originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, actualGroup);
        }

        [Test]
        public void ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup_InvalidGroup_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup(
                (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            const string expectedMessage = "The value of argument 'category' (99) is invalid for Enum type 'FailureMechanismSectionAssemblyCategoryGroup'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        public void CreateAssessmentResultTypeT1_InvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT1((TailorMadeAssessmentResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'tailorMadeAssessmentResult' (99) is invalid for Enum type '{nameof(TailorMadeAssessmentResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(TailorMadeAssessmentResultType.None)]
        [TestCase(TailorMadeAssessmentResultType.ProbabilityNegligible)]
        [TestCase(TailorMadeAssessmentResultType.Insufficient)]
        [TestCase(TailorMadeAssessmentResultType.Sufficient)]
        [TestCase(TailorMadeAssessmentResultType.NotAssessed)]
        public void CreateAssessmentResultTypeT1_ValidInput_ReturnsTailorMadeCalculationResult(TailorMadeAssessmentResultType detailedAssessmentResult)
        {
            // Call
            EAssessmentResultTypeT1 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT1(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(result, GetTailorMadeCalculationResult(detailedAssessmentResult));
        }

        private static EAssessmentResultTypeT1 GetTailorMadeCalculationResult(TailorMadeAssessmentResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case TailorMadeAssessmentResultType.None:
                    return EAssessmentResultTypeT1.Gr;
                case TailorMadeAssessmentResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT1.Fv;
                case TailorMadeAssessmentResultType.Sufficient:
                    return EAssessmentResultTypeT1.V;
                case TailorMadeAssessmentResultType.Insufficient:
                    return EAssessmentResultTypeT1.Vn;
                case TailorMadeAssessmentResultType.NotAssessed:
                    return EAssessmentResultTypeT1.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}