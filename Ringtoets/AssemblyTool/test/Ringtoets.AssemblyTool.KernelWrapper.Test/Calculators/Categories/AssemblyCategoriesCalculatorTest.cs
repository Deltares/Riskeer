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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;
using Ringtoets.Common.Data.AssemblyTool;

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
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = CreateAssessmentSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerBoundaryNorm);

                // Assert
                Assert.AreEqual(lowerBoundaryNorm, kernel.LowerBoundaryNorm);
                Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            CalculationOutput<AssessmentSectionCategory[]> output = CreateAssessmentSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<AssessmentSectionAssemblyCategory> result = calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerBoundaryNorm);

                // Assert
                AssemblyCategoryAssert.AssertAssessmentSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerBoundaryNorm);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            double probabilityDistributionFactor = random.NextDouble();
            double n = random.Next(1, 5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = CreateFailureMechanismSectionCategoryKernelOutput();

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                calculator.CalculateFailureMechanismSectionCategories(signalingNorm, lowerBoundaryNorm, probabilityDistributionFactor, n);

                // Assert
                Assert.AreEqual(lowerBoundaryNorm, kernel.LowerBoundaryNorm);
                Assert.AreEqual(signalingNorm, kernel.SignalingNorm);
                Assert.AreEqual(probabilityDistributionFactor, kernel.ProbabilityDistributionFactor);
                Assert.AreEqual(n, kernel.N);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            double probabilityDistributionFactor = random.NextDouble();
            double n = random.Next(1, 5);
            CalculationOutput<FailureMechanismSectionCategory[]> output = CreateFailureMechanismSectionCategoryKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.FailureMechanismSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyCategory> result = calculator.CalculateFailureMechanismSectionCategories(signalingNorm, lowerBoundaryNorm,
                                                                                                                                    probabilityDistributionFactor, n);

                // Assert
                AssemblyCategoryAssert.AssertFailureMechanismSectionAssemblyCategories(output, result);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionCategories_KernelThrowsException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            double probabilityDistributionFactor = random.NextDouble();
            double n = random.NextDouble();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoriesKernelStub kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                TestDelegate test = () => calculator.CalculateFailureMechanismSectionCategories(signalingNorm, lowerBoundaryNorm,
                                                                                                probabilityDistributionFactor, n);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(test);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(exception.InnerException.Message, exception.Message);
            }
        }

        private static CalculationOutput<AssessmentSectionCategory[]> CreateAssessmentSectionCategoryKernelOutput()
        {
            var random = new Random(11);

            return new CalculationOutput<AssessmentSectionCategory[]>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1))),
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1))),
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1))),
                new AssessmentSectionCategory(random.NextEnumValue<AssessmentSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1)))
            });
        }

        private static CalculationOutput<FailureMechanismSectionCategory[]> CreateFailureMechanismSectionCategoryKernelOutput()
        {
            var random = new Random(11);

            return new CalculationOutput<FailureMechanismSectionCategory[]>(new[]
            {
                new FailureMechanismSectionCategory(random.NextEnumValue<FailureMechanismSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1))),
                new FailureMechanismSectionCategory(random.NextEnumValue<FailureMechanismSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1))),
                new FailureMechanismSectionCategory(random.NextEnumValue<FailureMechanismSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1))),
                new FailureMechanismSectionCategory(random.NextEnumValue<FailureMechanismSectionCategoryGroup>(), new Probability(random.GetFromRange(0, 0.5)), new Probability(random.GetFromRange(0.5, 1)))
            });
        }
    }
}