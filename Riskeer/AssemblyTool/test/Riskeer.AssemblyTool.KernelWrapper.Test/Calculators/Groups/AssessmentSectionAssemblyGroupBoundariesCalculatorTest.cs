// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Groups;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Groups
{
    [TestFixture]
    public class AssessmentSectionAssemblyGroupBoundariesCalculatorTest
    {
        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionAssemblyGroupBoundariesCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            var calculator = new AssessmentSectionAssemblyGroupBoundariesCalculator(factory);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionAssemblyGroupBoundariesCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyGroupBoundaries_WithInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub kernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                kernel.AssessmentSectionCategoryLimits = CreateAssessmentSectionCategories();

                var calculator = new AssessmentSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                calculator.CalculateAssessmentSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.AreEqual(lowerLimitNorm, kernel.AssessmentSection.FailureProbabilityLowerLimit.Value);
                Assert.AreEqual(signalingNorm, kernel.AssessmentSection.FailureProbabilitySignallingLimit.Value);
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyGroupBoundaries_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);
            CategoriesList<AssessmentSectionCategory> assessmentSectionCategories = CreateAssessmentSectionCategories();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub kernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                kernel.AssessmentSectionCategoryLimits = assessmentSectionCategories;

                var calculator = new AssessmentSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                IEnumerable<AssessmentSectionAssemblyGroupBoundaries> result = calculator.CalculateAssessmentSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm);

                // Assert
                AssessmentSectionAssemblyGroupAssert.AssertAssessmentSectionAssemblyGroups(assessmentSectionCategories, result);
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyGroupBoundaries_KernelThrowsException_ThrowAssessmentSectionAssemblyGroupBoundariesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub kernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                void Call() => calculator.CalculateAssessmentSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyGroupBoundariesCalculatorException>(Call);
                Assert.IsNotNull(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateAssessmentSectionAssemblyGroupBoundaries_KernelThrowsAssemblyException_ThrowAssessmentSectionAssemblyGroupBoundariesCalculatorException()
        {
            // Setup
            var random = new Random(11);
            double lowerLimitNorm = random.NextDouble(0.5, 1.0);
            double signalingNorm = random.NextDouble(0.0, 0.5);

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub kernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                void Call() => calculator.CalculateAssessmentSectionAssemblyGroupBoundaries(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyGroupBoundariesCalculatorException>(Call);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    new AssemblyErrorMessage(string.Empty, EAssemblyErrors.EmptyResultsList)
                }), exception.Message);
            }
        }

        private static CategoriesList<AssessmentSectionCategory> CreateAssessmentSectionCategories()
        {
            var random = new Random(21);

            return new CategoriesList<AssessmentSectionCategory>(new[]
            {
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), new Probability(0), new Probability(0.25)),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), new Probability(0.25), new Probability(0.5)),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), new Probability(0.5), new Probability(0.75)),
                new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), new Probability(0.75), new Probability(1.0))
            });
        }
    }
}