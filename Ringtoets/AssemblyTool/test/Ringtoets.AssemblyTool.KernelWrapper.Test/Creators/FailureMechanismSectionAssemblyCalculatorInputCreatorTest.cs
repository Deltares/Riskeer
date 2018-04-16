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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;
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
        [TestCaseSource(nameof(InvalidDetailedAssessmentCategoryResults))]
        public void CreateCategoryCompliancyResults_InvalidEnumInput_ThrowInvalidEnumArgumentException(
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm)
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCategoryCompliancyResults(
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
        public void CreateCategoryCompliancyResults_ValidInput_ReturnsFmSectionCategoryCompliancyResults()
        {
            // Setup
            var random = new Random(39);
            var detailedAssessmentResultForFactorizedSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForSignalingNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForMechanismSpecificLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();
            var detailedAssessmentResultForFactorizedLowerLimitNorm = random.NextEnumValue<DetailedAssessmentResultType>();

            // Call
            FmSectionCategoryCompliancyResults result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCategoryCompliancyResults(
                detailedAssessmentResultForFactorizedSignalingNorm,
                detailedAssessmentResultForSignalingNorm,
                detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                detailedAssessmentResultForLowerLimitNorm,
                detailedAssessmentResultForFactorizedLowerLimitNorm);

            // Assert
            Dictionary<EFmSectionCategory, ECategoryCompliancy> results = result.GetCompliancyResults();
            Assert.AreEqual(results[EFmSectionCategory.Iv], GetAssessmentResultTypeG1(detailedAssessmentResultForFactorizedSignalingNorm));
            Assert.AreEqual(results[EFmSectionCategory.IIv], GetAssessmentResultTypeG1(detailedAssessmentResultForSignalingNorm));
            Assert.AreEqual(results[EFmSectionCategory.IIIv], GetAssessmentResultTypeG1(detailedAssessmentResultForMechanismSpecificLowerLimitNorm));
            Assert.AreEqual(results[EFmSectionCategory.IVv], GetAssessmentResultTypeG1(detailedAssessmentResultForLowerLimitNorm));
            Assert.AreEqual(results[EFmSectionCategory.Vv], GetAssessmentResultTypeG1(detailedAssessmentResultForFactorizedLowerLimitNorm));
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
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, EFmSectionCategory.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, EFmSectionCategory.Gr)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, EFmSectionCategory.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, EFmSectionCategory.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, EFmSectionCategory.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, EFmSectionCategory.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, EFmSectionCategory.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, EFmSectionCategory.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, EFmSectionCategory.VIIv)]
        public void CreateFailureMechanismSectionAssemblyCategoryResult_WithAssembly_ReturnFailureMechanismSectionAssemblyCategoryResult(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            EFmSectionCategory expectedGroup)
        {
            // Setup
            var random = new Random(11);
            var assembly = new FailureMechanismSectionAssembly(random.NextDouble(), originalGroup);

            // Call
            FmSectionAssemblyDirectResult input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(
                assembly);

            // Assert
            Assert.AreEqual(assembly.Probability, input.FailureProbability);
            Assert.AreEqual(expectedGroup, input.Result);
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
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, EFmSectionCategory.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, EFmSectionCategory.Gr)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, EFmSectionCategory.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, EFmSectionCategory.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, EFmSectionCategory.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, EFmSectionCategory.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, EFmSectionCategory.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, EFmSectionCategory.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, EFmSectionCategory.VIIv)]
        public void ConvertFailureMechanismSectionAssemblyCategoryGroup_ValidGroup_ReturnEFmSectionCategory(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            EFmSectionCategory expectedGroup)
        {
            // Call
            EFmSectionCategory actualGroup = FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionAssemblyCategoryGroup(
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
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.NotApplicable)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.None, EAssessmentResultTypeT3.Gr, null)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Iv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.Iv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.IIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IIIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.IIIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.IVv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.IVv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.Vv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.Vv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.VIv)]
        [TestCase(FailureMechanismSectionAssemblyCategoryGroup.VIIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.VIIv)]
        public void ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup_ValidGroup_ReturnsExpectedItems(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            EAssessmentResultTypeT3 expectedResult,
            EFmSectionCategory? expectedGroup)
        {
            // Call
            Tuple<EAssessmentResultTypeT3, EFmSectionCategory?> actualGroup = 
                FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup(originalGroup);

            // Assert
            Assert.AreEqual(expectedResult, actualGroup.Item1);
            Assert.AreEqual(expectedGroup, actualGroup.Item2);
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
            Assert.AreEqual(result, GetAssessmentResultTypeT1(detailedAssessmentResult));
        }

        private static EAssessmentResultTypeT1 GetAssessmentResultTypeT1(TailorMadeAssessmentResultType detailedAssessmentResult)
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

        [Test]
        public void CreateAssessmentResultTypeT3_InvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3((TailorMadeAssessmentProbabilityCalculationResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'tailorMadeAssessmentResult' (99) is invalid for Enum type '{nameof(TailorMadeAssessmentProbabilityCalculationResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.None)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.Probability)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed)]
        public void CreateAssessmentResultTypeT3_ValidInput_ReturnsTailorMadeCalculationResult(TailorMadeAssessmentProbabilityCalculationResultType detailedAssessmentResult)
        {
            // Call
            EAssessmentResultTypeT3 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(result, GetAssessmentResultTypeT3(detailedAssessmentResult));
        }

        private static EAssessmentResultTypeT3 GetAssessmentResultTypeT3(TailorMadeAssessmentProbabilityCalculationResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case TailorMadeAssessmentProbabilityCalculationResultType.None:
                    return EAssessmentResultTypeT3.Gr;
                case TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT3.Fv;
                case TailorMadeAssessmentProbabilityCalculationResultType.Probability:
                    return EAssessmentResultTypeT3.ResultSpecified;
                case TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed:
                    return EAssessmentResultTypeT3.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        [Test]
        public void CreateAssessmentResultTypeT4_InvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT4((TailorMadeAssessmentProbabilityAndDetailedCalculationResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'tailorMadeAssessmentResult' (99) is invalid for Enum type '{nameof(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed)]
        public void CreateAssessmentResultTypeT4_ValidInput_ReturnsTailorMadeCalculationResult(
            TailorMadeAssessmentProbabilityAndDetailedCalculationResultType detailedAssessmentResult)
        {
            // Call
            EAssessmentResultTypeT4 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT4(detailedAssessmentResult);

            // Assert
            Assert.AreEqual(result, GetAssessmentResultTypeT4(detailedAssessmentResult));
        }

        private static EAssessmentResultTypeT4 GetAssessmentResultTypeT4(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None:
                    return EAssessmentResultTypeT4.Gr;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT4.Fv;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability:
                    return EAssessmentResultTypeT4.ResultSpecified;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient:
                    return EAssessmentResultTypeT4.V;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient:
                    return EAssessmentResultTypeT4.Vn;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed:
                    return EAssessmentResultTypeT4.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}