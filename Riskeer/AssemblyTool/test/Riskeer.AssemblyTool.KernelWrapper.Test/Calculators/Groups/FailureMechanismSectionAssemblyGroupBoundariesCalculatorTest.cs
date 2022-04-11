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
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Groups;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Groups;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Groups
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupBoundariesCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new FailureMechanismSectionAssemblyGroupBoundariesCalculator(factory);

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionAssemblyGroupBoundariesCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new FailureMechanismSectionAssemblyGroupBoundariesCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupBoundaries_WithValidInput_InputCorrectlySentToKernel()
        {
            // Setup
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.InterpretationCategoryLimits = CreateInterpretationCategories();

                var calculator = new FailureMechanismSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                calculator.CalculateFailureMechanismSectionAssemblyGroupBoundaries(signalFloodingProbability, maximumAllowableFloodingProbability);

                // Assert
                AssessmentSection assessmentSection = categoryLimitsKernel.AssessmentSection;
                ProbabilityAssert.AreEqual(maximumAllowableFloodingProbability, assessmentSection.MaximumAllowableFloodingProbability);
                ProbabilityAssert.AreEqual(signalFloodingProbability, assessmentSection.SignalFloodingProbability);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupBoundaries_KernelWithCompleteOutput_ReturnsExpectedFailureMechanismSectionAssembly()
        {
            // Setup
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                CategoriesList<InterpretationCategory> interpretationCategories = CreateInterpretationCategories();
                categoryLimitsKernel.InterpretationCategoryLimits = interpretationCategories;

                var calculator = new FailureMechanismSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                IEnumerable<FailureMechanismSectionAssemblyGroupBoundaries> result =
                    calculator.CalculateFailureMechanismSectionAssemblyGroupBoundaries(signalFloodingProbability, maximumAllowableFloodingProbability);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);

                FailureMechanismSectionAssemblyGroupBoundariesAssert.AssertFailureMechanismSectionAssemblyGroupBoundaries(interpretationCategories, result);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupBoundaries_KernelWithInCompleteOutput_ThrowsAssemblyGroupBoundariesCalculatorException()
        {
            // Setup
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                var interpretationCategories = new CategoriesList<InterpretationCategory>(new[]
                {
                    new InterpretationCategory((EInterpretationCategory) 99, new Probability(0), new Probability(1))
                });
                categoryLimitsKernel.InterpretationCategoryLimits = interpretationCategories;

                var calculator = new FailureMechanismSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                void Call() => calculator.CalculateFailureMechanismSectionAssemblyGroupBoundaries(signalFloodingProbability, maximumAllowableFloodingProbability);

                // Assert
                var exception = Assert.Throws<FailureMechanismSectionAssemblyGroupBoundariesCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);

                Assert.IsTrue(categoryLimitsKernel.Calculated);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupBoundaries_KernelThrowsException_ThrowsAssemblyGroupBoundariesCalculatorException()
        {
            // Setup
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.ThrowExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                void Call() => calculator.CalculateFailureMechanismSectionAssemblyGroupBoundaries(signalFloodingProbability, maximumAllowableFloodingProbability);

                // Assert
                Assert.IsFalse(categoryLimitsKernel.Calculated);

                var exception = Assert.Throws<FailureMechanismSectionAssemblyGroupBoundariesCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void CalculateFailureMechanismSectionAssemblyGroupBoundaries_KernelThrowsAssemblyException_ThrowsAssemblyGroupBoundariesCalculatorException()
        {
            // Setup
            const double maximumAllowableFloodingProbability = 0.001;
            const double signalFloodingProbability = 0.0001;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new FailureMechanismSectionAssemblyGroupBoundariesCalculator(factory);

                // Call
                void Call() => calculator.CalculateFailureMechanismSectionAssemblyGroupBoundaries(signalFloodingProbability, maximumAllowableFloodingProbability);

                // Assert
                Assert.IsFalse(categoryLimitsKernel.Calculated);

                var exception = Assert.Throws<FailureMechanismSectionAssemblyGroupBoundariesCalculatorException>(Call);
                var innerException = exception.InnerException as AssemblyException;
                Assert.IsNotNull(innerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(innerException.Errors), exception.Message);
            }
        }

        private static CategoriesList<InterpretationCategory> CreateInterpretationCategories()
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