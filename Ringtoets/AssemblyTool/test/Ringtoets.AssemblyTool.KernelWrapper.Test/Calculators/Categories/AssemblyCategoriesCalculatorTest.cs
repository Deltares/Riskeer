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
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.Categories
{
    [TestFixture]
    public class AssemblyCategoriesCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyCategoriesCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssemblyCategoriesCalculator(factory);

            // Assert
            Assert.IsInstanceOf<IAssemblyCategoriesCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateAssessmentSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = CreateAssessmentSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            IEnumerable<AssessmentSectionCategoryLimits> output = CreateAssessmentSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<AssessmentSectionAssemblyCategory> result = calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                AssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.4);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = CreateFailureMechanismSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateFailureMechanismSectionCategories(signalingNorm, lowerLimitNorm, failureMechanismContribution, n);

                // Assert
                Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution, kernel.FailureMechanismContribution);
                Assert.AreEqual(n, kernel.N);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);
            IEnumerable<FmSectionCategoryLimits> output = CreateFailureMechanismSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateFailureMechanismSectionCategories(signalingNorm, lowerLimitNorm,
                                                                                                                                    failureMechanismContribution, n);

                // Assert
                AssemblyCategoryAssert.AssertFailureMechanismSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateFailureMechanismSectionCategories(signalingNorm, lowerLimitNorm,
                                                                                                failureMechanismContribution, n);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = CreateFailureMechanismSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateGeotechnicFailureMechanismSectionCategories(signalingNorm, lowerLimitNorm, failureMechanismContribution, n);

                // Assert
                Assert.AreEqual(lowerLimitNorm, kernel.LowerLimitNorm);
                Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution, kernel.FailureMechanismContribution);
                Assert.AreEqual(n, kernel.N);
            }
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);
            IEnumerable<FmSectionCategoryLimits> output = CreateFailureMechanismSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateGeotechnicFailureMechanismSectionCategories(
                    signalingNorm, lowerLimitNorm, failureMechanismContribution, n);

                // Assert
                AssemblyCategoryAssert.AssertFailureMechanismSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateGeotechnicFailureMechanismSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            double failureMechanismContribution = random.NextDouble();
            double n = random.NextDouble(1, 5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateGeotechnicFailureMechanismSectionCategories(signalingNorm, lowerLimitNorm,
                                                                                                          failureMechanismContribution, n);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static IEnumerable<AssessmentSectionCategoryLimits> CreateAssessmentSectionCategoryKernelOutput()
        {
            var random = new Random(11);

            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new AssessmentSectionCategoryLimits(random.NextEnumValue<EAssessmentGrade>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
        }

        private static IEnumerable<FmSectionCategoryLimits> CreateFailureMechanismSectionCategoryKernelOutput()
        {
            var random = new Random(11);

            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
            yield return new FmSectionCategoryLimits(random.NextEnumValue<EFmSectionCategory>(), random.NextDouble(0, 0.5), random.NextDouble(0.5, 1));
        }
    }
}