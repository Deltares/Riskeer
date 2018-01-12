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
using AssemblyTool.Kernel.CategoriesOutput;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels.CategoryBoundaries;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.CategoryBoundaries;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test.Kernels.CategoryBoundaries
{
    [TestFixture]
    public class AssemblyCategoryBoundariesKernelStubTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernelStub = new AssemblyCategoryBoundariesKernelStub();

            // Assert
            Assert.IsInstanceOf<IAssemblyCategoryBoundariesKernel>(kernelStub);
            Assert.IsFalse(kernelStub.Calculated);
        }

        [Test]
        public void Calculate_Always_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerBoundaryNorm = random.NextDouble();
            double signalingNorm = random.NextDouble();

            var kernelStub = new AssemblyCategoryBoundariesKernelStub();
            
            // Call
            kernelStub.Calculate(signalingNorm, lowerBoundaryNorm);

            // Assert
            Assert.AreEqual(signalingNorm, kernelStub.SignalingNorm);
            Assert.AreEqual(lowerBoundaryNorm, kernelStub.LowerBoundaryNorm);
        }

        [Test]
        public void Calculate_Always_SetCalculatedTrue()
        {
            // Setup
            var kernelStub = new AssemblyCategoryBoundariesKernelStub();

            // Call
            kernelStub.Calculate(0, 0);

            // Assert
            Assert.IsTrue(kernelStub.Calculated);
        }

        [Test]
        public void Calculate_Always_ReturnAssessmentSectionCategoriesOutput()
        {
            // Setup
            var kernelStub = new AssemblyCategoryBoundariesKernelStub
            {
                AssessmentSectionCategoriesOutput = new CalculationOutput<AssessmentSectionCategoriesOutput[]>(new AssessmentSectionCategoriesOutput[0])
            };

            // Call
            CalculationOutput<AssessmentSectionCategoriesOutput[]> output = kernelStub.Calculate(0, 0);

            // Assert
            Assert.AreSame(kernelStub.AssessmentSectionCategoriesOutput, output);
        }
    }
}