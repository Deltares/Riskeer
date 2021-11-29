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
using Assembly.Kernel.Model.AssessmentSection;
using Assembly.Kernel.Model.Categories;
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
    public class AssemblyGroupLimitsCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssemblyGroupLimitsCalculator(factory);

            // Assert
            Assert.IsInstanceOf<IAssemblyGroupLimitsCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssemblyGroupLimitsCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupLimits_WithValidInput_InputCorrectlySendToKernel()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> categoryLimits = CreateCategoryLimits();
                categoryLimitsKernel.CategoryLimits = categoryLimits;

                var calculator = new AssemblyGroupLimitsCalculator(factory);

                // Call
                calculator.CalculateFailureMechanismSectionAssemblyGroupLimits(signalingNorm, lowerLimitNorm);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(lowerLimitNorm, assessmentSection.FailureProbabilityLowerLimit);
                ProbabilityAssert.AreEqual(signalingNorm, assessmentSection.FailureProbabilitySignallingLimit);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupLimits_KernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> categoryLimits = CreateCategoryLimits();
                categoryLimitsKernel.CategoryLimits = categoryLimits;

                var calculator = new AssemblyGroupLimitsCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyGroupLimits> result =
                    calculator.CalculateFailureMechanismSectionAssemblyGroupLimits(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);

                AssemblyGroupLimitsAssert.AssertFailureMechanismSectionAssemblyGroupLimits(categoryLimits, result);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupLimits_KernelWithInCompleteOutput_ThrowsAssemblyGroupLimitsCalculatorException()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                var categoryLimits = new CategoriesList<InterpretationCategory>(new[]
                {
                    new InterpretationCategory((EInterpretationCategory) 99, new Probability(0), new Probability(1))
                });
                categoryLimitsKernel.CategoryLimits = categoryLimits;

                var calculator = new AssemblyGroupLimitsCalculator(factory);

                // Call
                void Call() => calculator.CalculateFailureMechanismSectionAssemblyGroupLimits(signalingNorm, lowerLimitNorm);

                // Assert
                var exception = Assert.Throws<AssemblyGroupLimitsCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);

                Assert.IsTrue(categoryLimitsKernel.Calculated);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupLimits_KernelThrowsException_ThrowsAssemblyGroupLimitsCalculatorException()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                categoryLimitsKernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssemblyGroupLimitsCalculator(factory);

                // Call
                void Call() => calculator.CalculateFailureMechanismSectionAssemblyGroupLimits(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.IsFalse(categoryLimitsKernel.Calculated);

                var exception = Assert.Throws<AssemblyGroupLimitsCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupLimits_KernelThrowsAssemblyException_ThrowsAssemblyGroupLimitsCalculatorException()
        {
            // Setup
            const double lowerLimitNorm = 0.001;
            const double signalingNorm = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                CategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedCategoryLimitsKernel;
                categoryLimitsKernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssemblyGroupLimitsCalculator(factory);

                // Call
                void Call() => calculator.CalculateFailureMechanismSectionAssemblyGroupLimits(signalingNorm, lowerLimitNorm);

                // Assert
                Assert.IsFalse(categoryLimitsKernel.Calculated);

                var exception = Assert.Throws<AssemblyGroupLimitsCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
            }
        }

        private static CategoriesList<InterpretationCategory> CreateCategoryLimits()
        {
            var random = new Random(21);
            return new CategoriesList<InterpretationCategory>(new[]
            {
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0), new Probability(0.25)),
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0.25), new Probability(0.5)),
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0.5), new Probability(0.75)),
                new InterpretationCategory(random.NextEnumValue<EInterpretationCategory>(), new Probability(0.75), new Probability(1))
            });
        }
    }
}