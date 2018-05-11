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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.Categories
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
            Assert.AreEqual(0, kernel.N);
            Assert.IsFalse(kernel.Calculated);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 0.9);
            double signalingNorm = random.NextDouble(0.0, 0.4);
            var section = new AssessmentSection(random.NextDouble(), signalingNorm, lowerLimitNorm);
            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateAssessmentSectionCategoryLimitsWbi21(section);

            // Assert
            Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
            Assert.IsTrue(kernel.Calculated);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var random = new Random(11);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));
            var kernel = new AssemblyCategoriesKernelStub
            {
                AssessmentSectionCategoriesOutput = Enumerable.Empty<AssessmentSectionCategoryLimits>()
            };

            // Call
            IEnumerable<AssessmentSectionCategoryLimits> output = kernel.CalculateAssessmentSectionCategoryLimitsWbi21(section);

            // Assert
            Assert.AreSame(kernel.AssessmentSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateAssessmentSectionCategoryLimitsWbi21_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var section = new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9));

            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateAssessmentSectionCategoryLimitsWbi21(section);

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.AssessmentSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 0.9);
            double signalingNorm = random.NextDouble(0.0, 0.4);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateFailureMechanismCategoryLimitsWbi11(new AssessmentSection(random.NextDouble(), signalingNorm, lowerLimitNorm),
                                                                new FailureMechanism(n, failureMechanismContribution));

            // Assert
            Assert.IsTrue(kernel.Calculated);

            Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
            Assert.AreEqual(failureMechanismContribution, kernel.FailureMechanismContribution);
            Assert.AreEqual(n, kernel.N);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var random = new Random(11);
            var kernel = new AssemblyCategoriesKernelStub
            {
                FailureMechanismCategoriesOutput = Enumerable.Empty<FailureMechanismCategoryLimits>()
            };

            // Call
            IEnumerable<FailureMechanismCategoryLimits> output = kernel.CalculateFailureMechanismCategoryLimitsWbi11(
                new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

            // Assert
            Assert.AreSame(kernel.FailureMechanismCategoriesOutput, output);
        }

        [Test]
        public void CalculateFailureMechanismCategoryLimitsWbi11_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFailureMechanismCategoryLimitsWbi11(
                new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismCategoriesOutput);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 0.9);
            double signalingNorm = random.NextDouble(0.0, 0.4);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateFmSectionCategoryLimitsWbi01(new AssessmentSection(random.NextDouble(), signalingNorm, lowerLimitNorm),
                                                         new FailureMechanism(n, failureMechanismContribution));

            // Assert
            Assert.IsTrue(kernel.Calculated);

            Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
            Assert.AreEqual(failureMechanismContribution, kernel.FailureMechanismContribution);
            Assert.AreEqual(n, kernel.N);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var random = new Random(11);
            var kernel = new AssemblyCategoriesKernelStub
            {
                FailureMechanismSectionCategoriesOutput = Enumerable.Empty<FmSectionCategoryLimits>()
            };

            // Call
            IEnumerable<FmSectionCategoryLimits> output = kernel.CalculateFmSectionCategoryLimitsWbi01(
                new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi01_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFmSectionCategoryLimitsWbi01(
                new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernelAndCalculatedTrue()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 0.9);
            double signalingNorm = random.NextDouble(0.0, 0.4);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            var kernel = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            kernel.CalculateFmSectionCategoryLimitsWbi02(new AssessmentSection(random.NextDouble(), signalingNorm, lowerLimitNorm),
                                                         new FailureMechanism(n, failureMechanismContribution));

            // Assert
            Assert.IsTrue(kernel.Calculated);

            Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
            Assert.AreEqual(failureMechanismContribution, kernel.FailureMechanismContribution);
            Assert.AreEqual(n, kernel.N);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var random = new Random(11);
            var kernel = new AssemblyCategoriesKernelStub
            {
                FailureMechanismSectionCategoriesOutput = Enumerable.Empty<FmSectionCategoryLimits>()
            };

            // Call
            IEnumerable<FmSectionCategoryLimits> output = kernel.CalculateFmSectionCategoryLimitsWbi02(
                new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

            // Assert
            Assert.AreSame(kernel.FailureMechanismSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateFmSectionCategoryLimitsWbi02_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var random = new Random(11);
            var kernel = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernel.Calculated);

            // Call
            TestDelegate test = () => kernel.CalculateFmSectionCategoryLimitsWbi02(
                new AssessmentSection(random.NextDouble(), random.NextDouble(0.0, 0.4), random.NextDouble(0.5, 0.9)),
                new FailureMechanism(random.NextDouble(1, 5), random.NextDouble()));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernel.Calculated);
            Assert.IsNull(kernel.FailureMechanismSectionCategoriesOutput);
        }
    }
}