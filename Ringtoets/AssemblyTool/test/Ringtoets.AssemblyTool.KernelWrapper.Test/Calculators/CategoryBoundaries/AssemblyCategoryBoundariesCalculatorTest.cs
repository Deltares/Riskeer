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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.CategoriesOutput;
using AssemblyTool.Kernel.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data.Input;
using Ringtoets.AssemblyTool.Data.Output;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.CategoryBoundaries;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.CategoryBoundaries;

namespace Ringtoets.AssemblyTool.KernelWrapper.Test.Calculators.CategoryBoundaries
{
    [TestFixture]
    public class AssemblyCategoryBoundariesCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new AssemblyCategoryBoundariesCalculator(null);

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
            var calculator = new AssemblyCategoryBoundariesCalculator(factory);

            // Assert
            Assert.IsInstanceOf<IAssemblyCategoryBoundariesCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateAssessmentSectionCategories_InputNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var calculator = new AssemblyCategoryBoundariesCalculator(factory);

            // Call
            TestDelegate call = () => calculator.CalculateAssessmentSectionCategories(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("input", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateAssessmentSectionCategories_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();
            var input = new AssemblyCategoryBoundariesCalculatorInput(signalingNorm, lowerBoundaryNorm);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelWrapperFactory.Instance;
                AssemblyCategoryBoundariesKernelStub kernel = factory.LastCreatedAssemblyCategoryBoundariesKernel;
                kernel.AssessmentSectionCategoriesOutput = CreateKernelOutput();

                var calculator = new AssemblyCategoryBoundariesCalculator(factory);

                // Call
                calculator.CalculateAssessmentSectionCategories(input);

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
            var input = new AssemblyCategoryBoundariesCalculatorInput(signalingNorm, lowerBoundaryNorm);
            CalculationOutput<AssessmentSectionCategoriesOutput[]> output = CreateKernelOutput();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory)AssemblyToolKernelWrapperFactory.Instance;
                AssemblyCategoryBoundariesKernelStub kernel = factory.LastCreatedAssemblyCategoryBoundariesKernel;
                kernel.AssessmentSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoryBoundariesCalculator(factory);

                // Call
                AssemblyCategoryBoundariesResult<AssessmentSectionAssemblyCategoryResult> result = calculator.CalculateAssessmentSectionCategories(input);

                // Assert
                Assert.AreEqual(output.Result.Length, result.Categories.Count());

                CollectionAssert.AreEqual(output.Result.Select(o => o.LowerBoundary), result.Categories.Select(r => r.LowerBoundary));
                CollectionAssert.AreEqual(output.Result.Select(o => o.UpperBoundary), result.Categories.Select(r => r.UpperBoundary));
            }
        }

        private static CalculationOutput<AssessmentSectionCategoriesOutput[]> CreateKernelOutput()
        {
            var random = new Random(11);

            return new CalculationOutput<AssessmentSectionCategoriesOutput[]>(new[]
            {
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2)),
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2)),
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2)),
                new AssessmentSectionCategoriesOutput(random.NextEnumValue<AssessmentSectionAssemblyCategory>(), random.Next(1), random.Next(1, 2))
            });
        }
    }
}