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
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.CategoryLimits;
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
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsNaN(kernel.LengthEffectFactorInput);
            Assert.IsNull(kernel.CategoryCompliancyResultsInput);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
            Assert.IsNull(kernel.FailureMechanismAssemblyDirectResultWithProbability);
            Assert.IsNull(kernel.FailureMechanismSectionCategories);
        }

        private static FailureMechanism CreateRandomFailureMechanism(Random random)
        {
            var failureMechanism = new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());
            return failureMechanism;
        }

        private static AssessmentSection CreateRandomAssessmentSection(Random random)
        {
            var assessmentSection = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.5), random.NextDouble(0.5, 1.0));
            return assessmentSection;
        }

        #region Simple Assessment

        [Test]
        public void TranslateAssessmentResultWbi0E1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeE1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0E1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeE1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E1_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0E1(0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
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
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E1(0);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentResultTypeE1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E2(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeE2>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0E3(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeE2Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0E3(0);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
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
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E3_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E3(0);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentResultTypeE2Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0E4_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0E4(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void TranslateAssessmentResultWbi0G1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeG1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G1_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeG1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeG1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G1(input);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentResultTypeG1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G2(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G3(assessmentSection, failureMechanism, assessmentResult, failureProbability);

            // Assert
            Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessmentResult, kernel.AssessmentResultTypeG2Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G3(assessmentSection,
                                                                                          failureMechanism,
                                                                                          assessmentResult,
                                                                                          failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G3(assessmentSection, failureMechanism, assessmentResult, failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G3_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G3(assessmentSection, failureMechanism, assessmentResult, failureProbability);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G4_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G4(0, null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();
            CategoriesList<FmSectionCategory> categories = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G5(lengthEffect, assessmentResult, failureProbability, categories);

            // Assert
            Assert.AreEqual(lengthEffect, kernel.LengthEffectFactorInput);
            Assert.AreEqual(assessmentResult, kernel.AssessmentResultTypeG2Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.AreSame(categories, kernel.FailureMechanismSectionCategories);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();
            CategoriesList<FmSectionCategory> categories = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismAssemblyDirectResultWithProbability = new FmSectionAssemblyDirectResultWithProbability(random.NextEnumValue<EFmSectionCategory>(),
                                                                                                                       random.NextDouble())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G5(lengthEffect,
                                                                                          assessmentResult,
                                                                                          failureProbability,
                                                                                          categories);

            // Assert
            Assert.AreSame(kernel.FailureMechanismAssemblyDirectResultWithProbability, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();
            CategoriesList<FmSectionCategory> categories = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G5(lengthEffect,
                                                                             assessmentResult,
                                                                             failureProbability,
                                                                             categories);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNaN(kernel.LengthEffectFactorInput);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsNull(kernel.FailureMechanismSectionCategories);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G5_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeG2>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();
            CategoriesList<FmSectionCategory> categories = CategoriesListTestFactory.CreateFailureMechanismSectionCategories();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G5(lengthEffect,
                                                                             assessmentResult,
                                                                             failureProbability,
                                                                             categories);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNaN(kernel.LengthEffectFactorInput);
            Assert.IsNull(kernel.AssessmentResultTypeG2Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsNull(kernel.FailureMechanismSectionCategories);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var input = new FmSectionCategoryCompliancyResults();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            Assert.AreSame(input, kernel.CategoryCompliancyResultsInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var input = new FmSectionCategoryCompliancyResults();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(new Random(39).NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var input = new FmSectionCategoryCompliancyResults();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.CategoryCompliancyResultsInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0G6_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var input = new FmSectionCategoryCompliancyResults();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0G6(input);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.CategoryCompliancyResultsInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        #endregion

        #region Tailor Made Assessment

        [Test]
        public void TranslateAssessmentResultWbi0T1_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            Assert.AreEqual(input, kernel.AssessmentResultTypeT1Input);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T1_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T1_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeT1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T1_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            var input = random.NextEnumValue<EAssessmentResultTypeT1>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T1(input);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentResultTypeT1Input);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T2_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T2(0);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T3(assessmentSection, failureMechanism, assessmentResult, failureProbability);

            // Assert
            Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessmentResult, kernel.AssessmentResultTypeT3Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T3(assessmentSection,
                                                                                          failureMechanism,
                                                                                          assessmentResult,
                                                                                          failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T3(assessmentSection,
                                                                             failureMechanism,
                                                                             assessmentResult,
                                                                             failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T3_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T3(assessmentSection,
                                                                             failureMechanism,
                                                                             assessmentResult,
                                                                             failureProbability);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T4_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            var category = random.NextEnumValue<EFmSectionCategory>();
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.TranslateAssessmentResultWbi0T4(assessmentResult, category);

            // Assert
            Assert.AreEqual(assessmentResult, kernel.AssessmentResultTypeT3Input);
            Assert.AreEqual(category, kernel.SectionCategoryInput);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T4_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            var category = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T4(assessmentResult, category);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T4_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            var category = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T4(assessmentResult, category);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.SectionCategoryInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T4_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            var category = random.NextEnumValue<EFmSectionCategory>();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T4(assessmentResult, category);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNull(kernel.SectionCategoryInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T5(assessmentSection, failureMechanism, lengthEffect, assessmentResult, failureProbability);

            // Assert
            Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessmentResult, kernel.AssessmentResultTypeT3Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.AreEqual(lengthEffect, kernel.LengthEffectFactorInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T5(assessmentSection,
                                                                                          failureMechanism,
                                                                                          lengthEffect,
                                                                                          assessmentResult,
                                                                                          failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T5(assessmentSection,
                                                                             failureMechanism,
                                                                             lengthEffect,
                                                                             assessmentResult,
                                                                             failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsNaN(kernel.LengthEffectFactorInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T5_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT3>();
            double failureProbability = random.NextDouble();
            double lengthEffect = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T5(assessmentSection,
                                                                             failureMechanism,
                                                                             lengthEffect,
                                                                             assessmentResult,
                                                                             failureProbability);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT3Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsNaN(kernel.LengthEffectFactorInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T6_Always_ThrowNotImplementedException()
        {
            // Setup
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T6(null, EAssessmentResultTypeT3.Ngo);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T7_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT4>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.TranslateAssessmentResultWbi0T7(assessmentSection, failureMechanism, assessmentResult, failureProbability);

            // Assert
            Assert.AreSame(assessmentSection, kernel.AssessmentSectionInput);
            Assert.AreSame(failureMechanism, kernel.FailureMechanismInput);
            Assert.AreEqual(assessmentResult, kernel.AssessmentResultTypeT4Input);
            Assert.AreEqual(failureProbability, kernel.FailureProbabilityInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T7_KernelDoesNotThrowException_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT4>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            FmSectionAssemblyDirectResult result = kernel.TranslateAssessmentResultWbi0T7(assessmentSection,
                                                                                          failureMechanism,
                                                                                          assessmentResult,
                                                                                          failureProbability);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T7_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT4>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T7(assessmentSection,
                                                                             failureMechanism,
                                                                             assessmentResult,
                                                                             failureProbability);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT4Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0T7_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(39);
            AssessmentSection assessmentSection = CreateRandomAssessmentSection(random);
            FailureMechanism failureMechanism = CreateRandomFailureMechanism(random);
            var assessmentResult = random.NextEnumValue<EAssessmentResultTypeT4>();
            double failureProbability = random.NextDouble();

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0T7(assessmentSection,
                                                                             failureMechanism,
                                                                             assessmentResult,
                                                                             failureProbability);

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.AssessmentSectionInput);
            Assert.IsNull(kernel.FailureMechanismInput);
            Assert.IsNull(kernel.AssessmentResultTypeT4Input);
            Assert.IsNaN(kernel.FailureProbabilityInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        #endregion

        #region Combined Assessment

        [Test]
        public void TranslateAssessmentResultWbi0A1_WithDirectResultAndThrowExceptionsFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble());
            var detailedAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble());
            var tailorMadeAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble());

            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            kernel.TranslateAssessmentResultWbi0A1(simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(simpleAssemblyResult, kernel.SimpleAssessmentResultInput);
            Assert.AreSame(detailedAssemblyResult, kernel.DetailedAssessmentResultInput);
            Assert.AreSame(tailorMadeAssemblyResult, kernel.TailorMadeAssessmentResultInput);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void TranslateAssessmentResultWbi0A1_WithDirectResultAndThrowExceptionsFalse_ReturnFailureMechanismSectionAssemblyResult()
        {
            // Setup
            var random = new Random(11);
            var simpleAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble());
            var detailedAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble());
            var tailorMadeAssemblyResult = new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble());

            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                FailureMechanismSectionDirectResult = new FmSectionAssemblyDirectResult(
                    random.NextEnumValue<EFmSectionCategory>())
            };

            // Call
            var result = (FmSectionAssemblyDirectResult) kernel.TranslateAssessmentResultWbi0A1(
                simpleAssemblyResult, detailedAssemblyResult, tailorMadeAssemblyResult);

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionDirectResult, result);
        }

        [Test]
        public void TranslateAssessmentResultWbi0A1_WithDirectResultAndThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0A1(
                new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble()),
                new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble()),
                new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble()));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsNull(kernel.SimpleAssessmentResultInput);
            Assert.IsNull(kernel.DetailedAssessmentResultInput);
            Assert.IsNull(kernel.TailorMadeAssessmentResultInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0A1_WithDirectResultAndThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(11);
            var kernel = new FailureMechanismSectionAssemblyKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0A1(
                new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble()),
                new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble()),
                new FmSectionAssemblyDirectResult(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble()));

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsNull(kernel.SimpleAssessmentResultInput);
            Assert.IsNull(kernel.DetailedAssessmentResultInput);
            Assert.IsNull(kernel.TailorMadeAssessmentResultInput);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionDirectResult);
        }

        [Test]
        public void TranslateAssessmentResultWbi0A1_WithIndirectResult_ThrowNotImplementedException()
        {
            // Setup
            var random = new Random(11);
            var kernel = new FailureMechanismSectionAssemblyKernelStub();

            // Call
            TestDelegate test = () => kernel.TranslateAssessmentResultWbi0A1(
                new FmSectionAssemblyIndirectResult(random.NextEnumValue<EIndirectAssessmentResult>()),
                new FmSectionAssemblyIndirectResult(random.NextEnumValue<EIndirectAssessmentResult>()),
                new FmSectionAssemblyIndirectResult(random.NextEnumValue<EIndirectAssessmentResult>()));

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }

        #endregion
    }
}