// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Categories
{
    [TestFixture]
    public class AssemblyCategoriesKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new AssemblyCategoriesKernelStub();

            // Assert
            Assert.IsInstanceOf<ICategoryLimitsCalculator>(kernel);
            Assert.AreEqual(0, kernel.LowerLimitNorm);
            Assert.AreEqual(0, kernel.SignalingNorm);
            Assert.AreEqual(0, kernel.AssessmentSectionNorm);
            Assert.AreEqual(0, kernel.FailureMechanismContribution);
            Assert.AreEqual(0, kernel.N);
            Assert.IsFalse(kernel.Calculated);

            Assert.IsNull(kernel.AssessmentSectionCategoriesOutput);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
            Assert.IsNull(kernel.FailureMechanismCategoriesOutput);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            AssessmentSection assessmentSection = CreateValidAssessmentSection();
            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateAssessmentSectionCategoryLimitsWbi21(assessmentSection);

            // Assert
            Assert.AreEqual(assessmentSection.FailureProbabilitySignallingLimit, kernel.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureProbabilityLowerLimit, kernel.LowerLimitNorm);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_KernelDoesNotThrowException_ReturnAssessmentSectionCategories()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                AssessmentSectionCategoriesOutput = CategoriesListTestFactory.CreateAssessmentSectionCategories()
            };

            // Call
            CategoriesList<AssessmentSectionCategory> output = kernel.CalculateAssessmentSectionCategoryLimitsWbi21(CreateValidAssessmentSection());

            // Assert
            Assert.AreSame(kernel.AssessmentSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateAssessmentSectionCategoryLimitsWbi21(CreateValidAssessmentSection());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.AssessmentSectionCategoriesOutput);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateAssessmentSectionCategoryLimitsWbi21(CreateValidAssessmentSection());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.AssessmentSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            AssessmentSection assessmentSection = CreateValidAssessmentSection();
            FailureMechanism failureMechanism = CreateValidFailureMechanism();

            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateFailureMechanismCategoryLimitsWbi11(assessmentSection,
                                                                failureMechanism);

            // Assert
            Assert.IsTrue(kernel.Calculated);

            Assert.AreEqual(assessmentSection.FailureProbabilitySignallingLimit, kernel.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureProbabilityLowerLimit, kernel.LowerLimitNorm);
            Assert.AreEqual(failureMechanism.FailureProbabilityMarginFactor, kernel.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.LengthEffectFactor, kernel.N);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_KernelDoesNotThrowException_ReturnAssessmentSectionCategories()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                FailureMechanismCategoriesOutput = CategoriesListTestFactory.CreateFailureMechanismCategories()
            };

            // Call
            CategoriesList<FailureMechanismCategory> output = kernel.CalculateFailureMechanismCategoryLimitsWbi11(
                CreateValidAssessmentSection(),
                CreateValidFailureMechanism());

            // Assert
            Assert.AreSame(kernel.FailureMechanismCategoriesOutput, output);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFailureMechanismCategoryLimitsWbi11(
                CreateValidAssessmentSection(),
                CreateValidFailureMechanism());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismCategoriesOutput);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFailureMechanismCategoryLimitsWbi11(
                CreateValidAssessmentSection(),
                CreateValidFailureMechanism());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismCategoriesOutput);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            AssessmentSection assessmentSection = CreateValidAssessmentSection();
            FailureMechanism failureMechanism = CreateValidFailureMechanism();

            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateFmSectionCategoryLimitsWbi01(assessmentSection,
                                                         failureMechanism);

            // Assert
            Assert.IsTrue(kernel.Calculated);

            Assert.AreEqual(assessmentSection.FailureProbabilitySignallingLimit, kernel.SignalingNorm);
            Assert.AreEqual(assessmentSection.FailureProbabilityLowerLimit, kernel.LowerLimitNorm);
            Assert.AreEqual(failureMechanism.FailureProbabilityMarginFactor, kernel.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.LengthEffectFactor, kernel.N);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_KernelDoesNotThrowException_ReturnAssessmentSectionCategories()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                FailureMechanismSectionCategoriesOutput = CategoriesListTestFactory.CreateFailureMechanismSectionCategories()
            };

            // Call
            CategoriesList<FmSectionCategory> output = kernel.CalculateFmSectionCategoryLimitsWbi01(
                CreateValidAssessmentSection(),
                CreateValidFailureMechanism());

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFmSectionCategoryLimitsWbi01(
                CreateValidAssessmentSection(),
                CreateValidFailureMechanism());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFmSectionCategoryLimitsWbi01(
                CreateValidAssessmentSection(),
                CreateValidFailureMechanism());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_KernelDoesNotThrowException_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(21);
            double assessmentSectionNorm = random.NextDouble();
            FailureMechanism failureMechanism = CreateValidFailureMechanism();

            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateFmSectionCategoryLimitsWbi02(assessmentSectionNorm,
                                                         failureMechanism);

            // Assert
            Assert.IsTrue(kernel.Calculated);

            Assert.AreEqual(assessmentSectionNorm, kernel.AssessmentSectionNorm);
            Assert.AreEqual(failureMechanism.FailureProbabilityMarginFactor, kernel.FailureMechanismContribution);
            Assert.AreEqual(failureMechanism.LengthEffectFactor, kernel.N);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_KernelDoesNotThrowException_ReturnAssessmentSectionCategories()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssemblyCategoriesKernelStub
            {
                FailureMechanismSectionCategoriesOutput = CategoriesListTestFactory.CreateFailureMechanismSectionCategories()
            };

            // Call
            CategoriesList<FmSectionCategory> output = kernel.CalculateFmSectionCategoryLimitsWbi02(
                random.NextDouble(),
                CreateValidFailureMechanism());

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFmSectionCategoryLimitsWbi02(
                random.NextDouble(),
                CreateValidFailureMechanism());

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_ThrowAssemblyExceptionOnCalculateTrue_ThrowsAssemblyException()
        {
            // Setup
            var random = new Random(21);
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowAssemblyExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFmSectionCategoryLimitsWbi02(
                random.NextDouble(),
                CreateValidFailureMechanism());

            // Assert
            var exception = Assert.Throws<AssemblyException>(test);
            AssemblyErrorMessage errorMessage = exception.Errors.Single();
            Assert.AreEqual("entity", errorMessage.EntityId);
            Assert.AreEqual(EAssemblyErrors.CategoryLowerLimitOutOfRange, errorMessage.ErrorCode);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
        }

        private static FailureMechanism CreateValidFailureMechanism()
        {
            var random = new Random(39);
            return new FailureMechanism(random.NextDouble(1, 5), random.NextDouble());
        }

        private static AssessmentSection CreateValidAssessmentSection()
        {
            var random = new Random(11);
            return new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
        }
    }
}