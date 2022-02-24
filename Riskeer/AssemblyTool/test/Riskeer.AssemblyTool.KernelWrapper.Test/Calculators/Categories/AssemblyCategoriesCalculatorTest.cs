﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using Assembly.Kernel.Old.Exceptions;
using Assembly.Kernel.Old.Model.CategoryLimits;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Categories
{
    [TestFixture]
    public class AssemblyCategoriesCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssemblyCategoriesCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolKernelFactoryOld>();
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

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssemblyCategoriesKernelStubOld kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = CategoriesListTestFactory.CreateAssessmentSectionCategories();

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
            CategoriesList<AssessmentSectionCategory> output = CategoriesListTestFactory.CreateAssessmentSectionCategories();

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssemblyCategoriesKernelStubOld kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.AssessmentSectionCategoriesOutput = output;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                IEnumerable<AssessmentSectionAssemblyGroupBoundaries> result = calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

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

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssemblyCategoriesKernelStubOld kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                void Call() => calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(Call);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateAssessmentSectionCategories_KernelThrowsAssemblyException_ThrowAssemblyCategoriesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfigOld())
            {
                var factory = (TestAssemblyToolKernelFactoryOld) AssemblyToolKernelFactoryOld.Instance;
                AssemblyCategoriesKernelStubOld kernel = factory.LastCreatedAssemblyCategoriesKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssemblyCategoriesCalculator(factory);

                // Call
                void Call() => calculator.CalculateAssessmentSectionCategories(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssemblyCategoriesCalculatorException>(Call);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreatorOld.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.CategoryLowerLimitOutOfRange)
                }), exception.Message);
            }
        }
    }
}