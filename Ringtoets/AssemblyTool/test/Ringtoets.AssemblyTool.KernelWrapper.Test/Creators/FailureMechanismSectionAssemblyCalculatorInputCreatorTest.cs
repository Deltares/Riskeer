// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Primitives;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Creators;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyCalculatorInputCreatorTest
    {
        #region Simple Assessment

        [Test]
        public void CreateAssessmentResultTypeE1_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE1((SimpleAssessmentResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'input' (99) is invalid for Enum type '{nameof(SimpleAssessmentResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentResultType.None, EAssessmentResultTypeE1.Gr)]
        [TestCase(SimpleAssessmentResultType.NotApplicable, EAssessmentResultTypeE1.Nvt)]
        [TestCase(SimpleAssessmentResultType.ProbabilityNegligible, EAssessmentResultTypeE1.Fv)]
        [TestCase(SimpleAssessmentResultType.AssessFurther, EAssessmentResultTypeE1.Vb)]
        public void CreateAssessmentResultTypeE1_ValidData_ReturnsAssessmentResultTypeE1(SimpleAssessmentResultType originalResult,
                                                                                         EAssessmentResultTypeE1 expectedResult)
        {
            // Call
            EAssessmentResultTypeE1 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE1(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void CreateAssessmentResultTypeE2_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE2((SimpleAssessmentValidityOnlyResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'input' (99) is invalid for Enum type '{nameof(SimpleAssessmentValidityOnlyResultType)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(SimpleAssessmentValidityOnlyResultType.None, EAssessmentResultTypeE2.Gr)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.NotApplicable, EAssessmentResultTypeE2.Nvt)]
        [TestCase(SimpleAssessmentValidityOnlyResultType.Applicable, EAssessmentResultTypeE2.Wvt)]
        public void CreateAssessmentResultTypeE2_ValidData_ReturnsAssessmentResultTypeE2(
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
            Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForFactorizedSignalingNorm), results[EFmSectionCategory.Iv]);
            Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForSignalingNorm), results[EFmSectionCategory.IIv]);
            Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForMechanismSpecificLowerLimitNorm), results[EFmSectionCategory.IIIv]);
            Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForLowerLimitNorm), results[EFmSectionCategory.IVv]);
            Assert.AreEqual(GetCategoryCompliance(detailedAssessmentResultForFactorizedLowerLimitNorm), results[EFmSectionCategory.Vv]);
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
        [TestCase(DetailedAssessmentResultType.None, EAssessmentResultTypeG1.Gr)]
        [TestCase(DetailedAssessmentResultType.Insufficient, EAssessmentResultTypeG1.Vn)]
        [TestCase(DetailedAssessmentResultType.Sufficient, EAssessmentResultTypeG1.V)]
        [TestCase(DetailedAssessmentResultType.NotAssessed, EAssessmentResultTypeG1.Ngo)]
        public void CreateAssessmentResultTypeG1_ValidInput_ReturnsAssessmentResultTypeG1(DetailedAssessmentResultType originalResult,
                                                                                          EAssessmentResultTypeG1 expectedResult)
        {
            // Call
            EAssessmentResultTypeG1 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG1(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
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
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.Probability, EAssessmentResultTypeG2.ResultSpecified)]
        [TestCase(DetailedAssessmentProbabilityOnlyResultType.NotAssessed, EAssessmentResultTypeG2.Ngo)]
        public void CreateAssessmentResultTypeG2_ValidInput_ReturnsAssessmentResultTypeG2(
            DetailedAssessmentProbabilityOnlyResultType originalResult,
            EAssessmentResultTypeG2 expectedResult)
        {
            // Call
            EAssessmentResultTypeG2 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        private static ECategoryCompliancy GetCategoryCompliance(DetailedAssessmentResultType detailedAssessmentResult)
        {
            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentResultType.None:
                    return ECategoryCompliancy.NoResult;
                case DetailedAssessmentResultType.Sufficient:
                    return ECategoryCompliancy.Complies;
                case DetailedAssessmentResultType.Insufficient:
                    return ECategoryCompliancy.DoesNotComply;
                case DetailedAssessmentResultType.NotAssessed:
                    return ECategoryCompliancy.Ngo;
                default:
                    throw new NotSupportedException();
            }
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
                                              random.NextEnumValue<DetailedAssessmentResultType>())
                    .SetName($"DetailedAssessmentResultForFactorizedSignalingNorm invalid {nameof(DetailedAssessmentResultType)}");
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>())
                    .SetName($"DetailedAssessmentResultForSignalingNorm invalid {nameof(DetailedAssessmentResultType)}");
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>())
                    .SetName($"DetailedAssessmentResultForMechanismSpecificLowerLimitNorm invalid {nameof(DetailedAssessmentResultType)}");
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99,
                                              random.NextEnumValue<DetailedAssessmentResultType>())
                    .SetName($"DetailedAssessmentResultForLowerLimitNorm invalid {nameof(DetailedAssessmentResultType)}");
                yield return new TestCaseData(random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              random.NextEnumValue<DetailedAssessmentResultType>(),
                                              (DetailedAssessmentResultType) 99)
                    .SetName($"DetailedAssessmentResultForFactorizedLowerLimitNorm invalid {nameof(DetailedAssessmentResultType)}");
            }
        }

        #endregion

        #region Failure Mechanism Section Assembly

        [Test]
        public void CreateFailureMechanismSectionAssemblyDirectResult_AssemblyNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(null);

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
        public void CreateFailureMechanismSectionAssemblyDirectResult_WithAssembly_ReturnFmSectionAssemblyDirectResult(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            EFmSectionCategory expectedGroup)
        {
            // Setup
            var random = new Random(11);
            var assembly = new FailureMechanismSectionAssembly(random.NextDouble(), originalGroup);

            // Call
            FmSectionAssemblyDirectResultWithProbability input = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(
                assembly);

            // Assert
            Assert.AreEqual(assembly.Probability, input.FailureProbability);
            Assert.AreEqual(expectedGroup, input.Result);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyDirectResult_WithInvalidEnumInput_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(
                new FailureMechanismSectionAssembly(0, (FailureMechanismSectionAssemblyCategoryGroup) 99));

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
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
        public void CreateFailureMechanismSectionAssemblyDirectResult_WithValidGroup_ReturnFmSectionAssemblyDirectResult(
            FailureMechanismSectionAssemblyCategoryGroup originalGroup,
            EFmSectionCategory expectedGroup)
        {
            // Call
            FmSectionAssemblyDirectResult actualResult = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(
                originalGroup);

            // Assert
            Assert.AreEqual(expectedGroup, actualResult.Result);
        }

        [Test]
        public void CreateFailureMechanismSectionAssemblyDirectResult_WithInvalidGroup_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(
                (FailureMechanismSectionAssemblyCategoryGroup) 99);

            // Assert
            string expectedMessage = $"The value of argument 'category' (99) is invalid for Enum type '{nameof(FailureMechanismSectionAssemblyCategoryGroup)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage);
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.None, EAssessmentResultTypeT3.Gr, null)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.Iv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.Iv)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.IIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.IIv)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.IIIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.IIIv)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.IVv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.IVv)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.Vv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.Vv)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.VIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.VIv)]
        [TestCase(TailorMadeAssessmentCategoryGroupResultType.VIIv, EAssessmentResultTypeT3.ResultSpecified, EFmSectionCategory.VIIv)]
        public void CreateAssessmentResultTypeT3WithCategoryGroupResult_ValidGroup_ReturnsExpectedItems(
            TailorMadeAssessmentCategoryGroupResultType originalGroup,
            EAssessmentResultTypeT3 expectedResult,
            EFmSectionCategory? expectedGroup)
        {
            // Call
            Tuple<EAssessmentResultTypeT3, EFmSectionCategory?> actualGroup =
                FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3WithCategoryGroupResult(originalGroup);

            // Assert
            Assert.AreEqual(expectedResult, actualGroup.Item1);
            Assert.AreEqual(expectedGroup, actualGroup.Item2);
        }

        [Test]
        public void CreateAssessmentResultTypeT3WithCategoryGroupResult_InvalidGroup_ThrowInvalidEnumArgumentException()
        {
            // Call
            TestDelegate test = () => FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3WithCategoryGroupResult(
                (TailorMadeAssessmentCategoryGroupResultType) 99);

            // Assert
            string expectedMessage = $"The value of argument 'tailorMadeAssessmentResult' (99) is invalid for Enum type '{nameof(TailorMadeAssessmentCategoryGroupResultType)}'.";
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
        [TestCase(TailorMadeAssessmentResultType.None, EAssessmentResultTypeT1.Gr)]
        [TestCase(TailorMadeAssessmentResultType.ProbabilityNegligible, EAssessmentResultTypeT1.Fv)]
        [TestCase(TailorMadeAssessmentResultType.Insufficient, EAssessmentResultTypeT1.Vn)]
        [TestCase(TailorMadeAssessmentResultType.Sufficient, EAssessmentResultTypeT1.V)]
        [TestCase(TailorMadeAssessmentResultType.NotAssessed, EAssessmentResultTypeT1.Ngo)]
        public void CreateAssessmentResultTypeT1_ValidInput_ReturnsAssessmentResultTypeT1(
            TailorMadeAssessmentResultType originalResult,
            EAssessmentResultTypeT1 expectedResult)
        {
            // Call
            EAssessmentResultTypeT1 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT1(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
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
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.None, EAssessmentResultTypeT3.Gr)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible, EAssessmentResultTypeT3.Fv)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.Probability, EAssessmentResultTypeT3.ResultSpecified)]
        [TestCase(TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed, EAssessmentResultTypeT3.Ngo)]
        public void CreateAssessmentResultTypeT3_ValidInput_ReturnsAssessmentResultTypeT3(
            TailorMadeAssessmentProbabilityCalculationResultType originalResult,
            EAssessmentResultTypeT3 expectedResult)
        {
            // Call
            EAssessmentResultTypeT3 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
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
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None, EAssessmentResultTypeT4.Gr)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible, EAssessmentResultTypeT4.Fv)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability, EAssessmentResultTypeT4.ResultSpecified)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient, EAssessmentResultTypeT4.V)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient, EAssessmentResultTypeT4.Vn)]
        [TestCase(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed, EAssessmentResultTypeT4.Ngo)]
        public void CreateAssessmentResultTypeT4_ValidInput_ReturnsAssessmentResultTypeT4(
            TailorMadeAssessmentProbabilityAndDetailedCalculationResultType originalResult,
            EAssessmentResultTypeT4 expectedResult)
        {
            // Call
            EAssessmentResultTypeT4 result = FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT4(originalResult);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        #endregion
    }
}