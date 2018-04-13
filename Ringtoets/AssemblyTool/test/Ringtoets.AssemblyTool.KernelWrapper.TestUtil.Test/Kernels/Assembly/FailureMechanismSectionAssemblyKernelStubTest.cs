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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Assembly
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Assert
            Assert.IsInstanceOf<IAssessmentResultsTranslator>(kernel);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.AssessmentResultTypeE1Input);
            Assert.IsNull(kernel.AssessmentResultTypeE2Input);
            Assert.IsNull(kernel.AssessmentResultTypeG1Input);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNull(kernel.AssessmentResultTypeT1Input);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.SectionCategoryInput);
            Assert.IsNull(kernel.SimpleAssessmentResultInput);
            Assert.IsNull(kernel.DetailedAssessmentResultInput);
            Assert.IsNull(kernel.TailorMadeAssessmentResultInput);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.FailureProbabilityInput);
            Assert.IsNull(kernel.LengthEffectFactorInput);
            Assert.IsNull(kernel.CategoryCompliancyResultsInput);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        #region Simple Assessment

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeE1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.TranslateAssessmentResultWbi0E1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeE1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), double.NaN)
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0E1(0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E1(0);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeE1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeE2>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.TranslateAssessmentResultWbi0E3(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeE2Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), double.NaN)
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0E3(0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E3(0);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeE2Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        [Test]
        public void SimpleAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.SimpleAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void DetailedAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<DetailedCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreEqual(input, kernel.DetailedCalculationResultInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<DetailedCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<EFmSectionCategory>(
                    random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            CalculationOutput<EFmSectionCategory> result = kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryGroup, result);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<DetailedCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.DetailedCalculationResultInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryGroup);
        }

        [Test]
        public void DetailedAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCalculationInputFromProbability(
                new Probability(random.NextDouble()),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(input, kernel.DetailedAssessmentFailureMechanismFromProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCalculationInputFromProbability(
                new Probability(random.NextDouble()),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(
                    new FailureMechanismSectionAssemblyResult(random.NextEnumValue<EFmSectionCategory>(), double.NaN))
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithProbability_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCalculationInputFromProbability(
                new Probability(random.NextDouble()),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.DetailedAssessmentFailureMechanismFromProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithBoundaries_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCategoryBoundariesCalculationResult(
                random.NextEnumValue<DetailedCalculationResult>(), random.NextEnumValue<DetailedCalculationResult>(),
                random.NextEnumValue<DetailedCalculationResult>(), random.NextEnumValue<DetailedCalculationResult>(),
                random.NextEnumValue<DetailedCalculationResult>());

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(input, kernel.DetailedAssessmentFailureMechanismFromCategoriesInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithBoundaries_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCategoryBoundariesCalculationResult(
                random.NextEnumValue<DetailedCalculationResult>(), random.NextEnumValue<DetailedCalculationResult>(),
                random.NextEnumValue<DetailedCalculationResult>(), random.NextEnumValue<DetailedCalculationResult>(),
                random.NextEnumValue<DetailedCalculationResult>());

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<EFmSectionCategory>(
                    random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            CalculationOutput<EFmSectionCategory> result = kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryGroup, result);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithBoundaries_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCategoryBoundariesCalculationResult(
                random.NextEnumValue<DetailedCalculationResult>(), random.NextEnumValue<DetailedCalculationResult>(),
                random.NextEnumValue<DetailedCalculationResult>(), random.NextEnumValue<DetailedCalculationResult>(),
                random.NextEnumValue<DetailedCalculationResult>());

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.DetailedAssessmentFailureMechanismFromCategoriesInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryGroup);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithLengthEffect_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCalculationInputFromProbabilityWithLengthEffect(
                new Probability(random.NextDouble()),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                },
                random.NextRoundedDouble(1.0, 40.0));

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(input, kernel.DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithLengthEffect_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCalculationInputFromProbabilityWithLengthEffect(
                new Probability(random.NextDouble()),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                },
                random.NextRoundedDouble(1.0, 40.0));

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(
                    new FailureMechanismSectionAssemblyResult(EFmSectionCategory.IIIv, double.NaN))
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void DetailedAssessmentDirectFailureMechanismsWithLengthEffect_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = new DetailedCalculationInputFromProbabilityWithLengthEffect(
                new Probability(random.NextDouble()),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                },
                random.NextRoundedDouble(1.0, 40.0));

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.DetailedAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<TailorMadeCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreEqual(input, kernel.TailorMadeCalculationResultInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<TailorMadeCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<EFmSectionCategory>(
                    random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            CalculationOutput<EFmSectionCategory> result = kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryGroup, result);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanisms_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<TailorMadeCalculationResult>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.TailorMadeCalculationResultInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryGroup);
        }

        [Test]
        public void TailorMadeAssessmentIndirectFailureMechanisms_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentIndirectFailureMechanisms(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbability(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(input, kernel.TailorMadeCalculationInputFromProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbability_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbability(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(
                    new FailureMechanismSectionAssemblyResult(EFmSectionCategory.IIIv, double.NaN))
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbability_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbability(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.TailorMadeCalculationInputFromProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithCategories_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var result = random.NextEnumValue<TailorMadeCategoryCalculationResult>();
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.TailorMadeAssessmentDirectFailureMechanisms(result);

            // Assert
            Assert.AreEqual(result, kernel.TailorMadeCalculationInputFromCategoryResultInput);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithCategories_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(random.NextEnumValue<TailorMadeCategoryCalculationResult>());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.TailorMadeCalculationInputFromProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithCategories_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbability(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                });

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.TailorMadeCalculationInputFromProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbabilityAndLengthEffect_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                },
                random.NextDouble(1.0, 10.0));

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(input, kernel.TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbabilityAndLengthEffect_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                },
                random.NextDouble(1.0, 10.0));

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(
                    new FailureMechanismSectionAssemblyResult(EFmSectionCategory.IIIv, double.NaN))
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void TailorMadeAssessmentDirectFailureMechanismsWithProbabilityAndLengthEffect_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = new TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                new TailorMadeProbabilityCalculationResult(new Probability(random.NextDouble())),
                new[]
                {
                    new FailureMechanismSectionCategory(random.NextEnumValue<EFmSectionCategory>(),
                                                        new Probability(random.NextRoundedDouble(0.0, 0.5)),
                                                        new Probability(random.NextRoundedDouble(0.6, 1.0)))
                },
                random.NextDouble(1.0, 10.0));

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TailorMadeAssessmentDirectFailureMechanisms(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        #endregion

        #region Combined Assessment

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var detailedAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var tailorMadeAssemblyResult = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreEqual(simpleAssemblyResult, kernel.CombinedSimpleAssessmentGroupInput);
            Assert.AreEqual(detailedAssemblyResult, kernel.CombinedDetailedAssessmentGroupInput);
            Assert.AreEqual(tailorMadeAssemblyResult, kernel.CombinedTailorMadeAssessmentGroupInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var detailedAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var tailorMadeAssemblyResult = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyCategoryGroup = new CalculationOutput<EFmSectionCategory>(
                    random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            CalculationOutput<EFmSectionCategory> result = kernel.CombinedAssessmentFromFailureMechanismSectionResults(
                simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyCategoryGroup, result);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResults_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var detailedAssemblyResult = random.NextEnumValue<EFmSectionCategory>();
            var tailorMadeAssemblyResult = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.CombinedSimpleAssessmentGroupInput);
            Assert.IsNull(kernel.CombinedDetailedAssessmentGroupInput);
            Assert.IsNull(kernel.CombinedTailorMadeAssessmentGroupInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyCategoryGroup);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithProbabilities_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var detailedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var tailorMadeAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(simpleAssemblyResult, kernel.CombinedSimpleAssessmentInput);
            Assert.AreSame(detailedAssemblyResult, kernel.CombinedDetailedAssessmentInput);
            Assert.AreSame(tailorMadeAssemblyResult, kernel.CombinedTailorMadeAssessmentInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithProbabilities_ThrowExceptionOnCalculateFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var detailedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var tailorMadeAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionAssemblyResult = new FmSectionAssemblyDirectResult(
                    new FailureMechanismSectionAssemblyResult(random.NextEnumValue<EFmSectionCategory>(), double.NaN))
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.CombinedAssessmentFromFailureMechanismSectionResults(
                simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionAssemblyResult, result);
        }

        [Test]
        public void CombinedAssessmentFromFailureMechanismSectionResultsWithProbabilities_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var detailedAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);
            var tailorMadeAssemblyResult = new FailureMechanismSectionAssemblyResult(
                random.NextEnumValue<EFmSectionCategory>(),
                double.NaN);

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.CombinedAssessmentFromFailureMechanismSectionResults(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.CombinedSimpleAssessmentInput);
            Assert.IsNull(kernel.CombinedDetailedAssessmentInput);
            Assert.IsNull(kernel.CombinedTailorMadeAssessmentInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionAssemblyResult);
        }

        #endregion
    }
}