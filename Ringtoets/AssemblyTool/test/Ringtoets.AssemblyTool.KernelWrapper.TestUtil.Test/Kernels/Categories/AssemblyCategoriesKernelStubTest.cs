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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Categories;
using AssemblyTool.Kernel.Categories.CalculatorInput;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
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
            var kernelStub = new AssemblyCategoriesKernelStub();

            // Assert
            Assert.IsInstanceOf<ICategoriesCalculator>(kernelStub);
            Assert.AreEqual(0, kernelStub.LowerLimitNorm);
            Assert.AreEqual(0, kernelStub.SignalingNorm);
            Assert.IsFalse(kernelStub.Calculated);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            var lowerLimitNorm = new Probability(random.NextDouble());
            var signalingNorm = new Probability(random.NextDouble());

            var kernelStub = new AssemblyCategoriesKernelStub();
            var input = new CalculateAssessmentSectionCategoriesInput(signalingNorm, lowerLimitNorm);

            // Call
            kernelStub.CalculateAssessmentSectionCategories(input);

            // Assert
            Assert.AreEqual(signalingNorm, kernelStub.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernelStub.LowerLimitNorm);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernelStub.Calculated);

            // Call
            kernelStub.CalculateAssessmentSectionCategories(new CalculateAssessmentSectionCategoriesInput(
                                                                new Probability(0), new Probability(0)));

            // Assert
            Assert.IsTrue(kernelStub.Calculated);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub
            {
                AssessmentSectionCategoriesOutput = new CalculationOutput<AssessmentSectionCategory[]>(new AssessmentSectionCategory[0])
            };

            // Call
            CalculationOutput<AssessmentSectionCategory[]> output = kernelStub.CalculateAssessmentSectionCategories(new CalculateAssessmentSectionCategoriesInput(new Probability(0), new Probability(0)));

            // Assert
            Assert.AreSame(kernelStub.AssessmentSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateAssessmentSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernelStub.Calculated);

            // Call
            TestDelegate test = () => kernelStub.CalculateAssessmentSectionCategories(new CalculateAssessmentSectionCategoriesInput(new Probability(0), new Probability(0)));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernelStub.Calculated);
            Assert.IsNull(kernelStub.AssessmentSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            var lowerLimitNorm = new Probability(random.NextDouble());
            var signalingNorm = new Probability(random.NextDouble());
            double probabilityDistributionFactor = random.NextDouble();
            double n = random.NextDouble(1, 5);

            var kernelStub = new AssemblyCategoriesKernelStub();
            var input = new CalculateFailureMechanismSectionCategoriesInput(signalingNorm, lowerLimitNorm, probabilityDistributionFactor, n);

            // Call
            kernelStub.CalculateFailureMechanismSectionCategories(input);

            // Assert
            Assert.AreEqual(signalingNorm, kernelStub.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernelStub.LowerLimitNorm);
            Assert.AreEqual(probabilityDistributionFactor, kernelStub.ProbabilityDistributionFactor);
            Assert.AreEqual(n, kernelStub.N);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernelStub.Calculated);

            // Call
            kernelStub.CalculateFailureMechanismSectionCategories(new CalculateFailureMechanismSectionCategoriesInput(new Probability(0), new Probability(0), 0, 1));

            // Assert
            Assert.IsTrue(kernelStub.Calculated);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub
            {
                FailureMechanismSectionCategoriesOutput = new CalculationOutput<FailureMechanismSectionCategory[]>(new FailureMechanismSectionCategory[0])
            };

            // Call
            CalculationOutput<FailureMechanismSectionCategory[]> output = kernelStub.CalculateFailureMechanismSectionCategories(
                new CalculateFailureMechanismSectionCategoriesInput(new Probability(0), new Probability(0), 0, 1));

            // Assert
            Assert.AreSame(kernelStub.FailureMechanismSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernelStub.Calculated);

            // Call
            TestDelegate test = () => kernelStub.CalculateFailureMechanismSectionCategories(
                new CalculateFailureMechanismSectionCategoriesInput(new Probability(0), new Probability(0), 0, 1));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernelStub.Calculated);
            Assert.IsNull(kernelStub.FailureMechanismSectionCategoriesOutput);
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            var lowerLimitNorm = new Probability(random.NextDouble());
            var signalingNorm = new Probability(random.NextDouble());
            double probabilityDistributionFactor = random.NextDouble();
            double n = random.NextDouble(1, 5);

            var kernelStub = new AssemblyCategoriesKernelStub();
            var input = new CalculateFailureMechanismSectionCategoriesInput(signalingNorm, lowerLimitNorm, probabilityDistributionFactor, n);

            // Call
            kernelStub.CalculateGeotechnicFailureMechanismSectionCategories(input);

            // Assert
            Assert.AreEqual(signalingNorm, kernelStub.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, kernelStub.LowerLimitNorm);
            Assert.AreEqual(probabilityDistributionFactor, kernelStub.ProbabilityDistributionFactor);
            Assert.AreEqual(n, kernelStub.N);
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_SetCalculatedTrue()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub();

            // Precondition
            Assert.IsFalse(kernelStub.Calculated);

            // Call
            kernelStub.CalculateGeotechnicFailureMechanismSectionCategories(new CalculateFailureMechanismSectionCategoriesInput(new Probability(0), new Probability(0), 0, 1));

            // Assert
            Assert.IsTrue(kernelStub.Calculated);
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateFalse_ReturnAssessmentSectionCategories()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub
            {
                FailureMechanismSectionCategoriesOutput = new CalculationOutput<FailureMechanismSectionCategory[]>(new FailureMechanismSectionCategory[0])
            };

            // Call
            CalculationOutput<FailureMechanismSectionCategory[]> output = kernelStub.CalculateGeotechnicFailureMechanismSectionCategories(
                new CalculateFailureMechanismSectionCategoriesInput(new Probability(0), new Probability(0), 0, 1));

            // Assert
            Assert.AreSame(kernelStub.FailureMechanismSectionCategoriesOutput, output);
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_ThrowExceptionOnCalculateTrue_ThrowsException()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub
            {
                ThrowExceptionOnCalculate = true
            };

            // Precondition
            Assert.IsFalse(kernelStub.Calculated);

            // Call
            TestDelegate test = () => kernelStub.CalculateGeotechnicFailureMechanismSectionCategories(
                new CalculateFailureMechanismSectionCategoriesInput(new Probability(0), new Probability(0), 0, 1));

            // Assert
            var exception = Assert.Throws<Exception>(test);
            Assert.AreEqual("Message", exception.Message);
            Assert.IsNotNull(exception.InnerException);
            Assert.IsFalse(kernelStub.Calculated);
            Assert.IsNull(kernelStub.FailureMechanismSectionCategoriesOutput);
        }

        [Test]
        public void CalculateFailureMechanismCategories_Always_ThrowsNotImplementedException()
        {
            // Setup
            var kernelStub = new AssemblyCategoriesKernelStub();

            // Call
            TestDelegate test = () => kernelStub.CalculateFailureMechanismCategories(null);

            // Assert
            Assert.Throws<NotImplementedException>(test);
        }
    }
}