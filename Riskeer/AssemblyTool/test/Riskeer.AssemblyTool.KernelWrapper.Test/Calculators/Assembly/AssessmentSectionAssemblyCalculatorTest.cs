// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Groups;

namespace Riskeer.AssemblyTool.KernelWrapper.Test.Calculators.Assembly
{
    [TestFixture]
    public class AssessmentSectionAssemblyCalculatorTest
    {
        [Test]
        public void Constructor_WithFactory_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            // Call
            var calculator = new AssessmentSectionAssemblyCalculator(kernelFactory);

            // Assert
            Assert.IsInstanceOf<IAssessmentSectionAssemblyCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new AssessmentSectionAssemblyCalculator(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("factory", exception.ParamName);
        }

        [Test]
        public void AssembleAssessmentSection_FailureMechanismProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.AssembleAssessmentSection(null, random.NextDouble(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismProbabilities", exception.ParamName);
        }

        [Test]
        public void AssembleAssessmentSection_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            IEnumerable<double> failureMechanismProbabilities = Enumerable.Repeat(random.NextDouble(), random.Next(1, 10))
                                                                          .ToArray();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                var assessmentSectionCategories = new CategoriesList<AssessmentSectionCategory>(new[]
                {
                    new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), new Probability(0), new Probability(1))
                });
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.AssessmentSectionCategoryLimits = assessmentSectionCategories;

                AssessmentSectionAssemblyKernelStub assessmentSectionAssemblyKernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                var assemblyProbability = new Probability(random.NextDouble());
                assessmentSectionAssemblyKernel.AssemblyProbability = assemblyProbability;
                assessmentSectionAssemblyKernel.AssemblyGroup = random.NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleAssessmentSection(failureMechanismProbabilities, maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                ProbabilityAssert.AreEqual(maximumAllowableFloodingProbability, categoryLimitsKernel.AssessmentSection.MaximumAllowableFloodingProbability);
                ProbabilityAssert.AreEqual(signalFloodingProbability, categoryLimitsKernel.AssessmentSection.SignalFloodingProbability);

                Assert.IsTrue(assessmentSectionAssemblyKernel.ProbabilityCalculated);
                Assert.IsTrue(assessmentSectionAssemblyKernel.AssemblyGroupCalculated);
                Assert.IsFalse(assessmentSectionAssemblyKernel.PartialAssembly);
                Assert.AreSame(assessmentSectionCategories, assessmentSectionAssemblyKernel.Categories);

                AssertProbabilitiesInput(failureMechanismProbabilities, assessmentSectionAssemblyKernel.FailureMechanismProbabilities);

                Assert.AreEqual(assemblyProbability, assessmentSectionAssemblyKernel.AssemblyProbabilityInput);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                var assemblyProbability = new Probability(random.NextDouble());
                var assemblyGroup = random.NextEnumValue<EAssessmentGrade>();
                kernel.AssemblyProbability = assemblyProbability;
                kernel.AssemblyGroup = assemblyGroup;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                AssessmentSectionAssemblyResultWrapper resultWrapper = calculator.AssembleAssessmentSection(
                    Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                AssessmentSectionAssemblyResult result = resultWrapper.AssemblyResult;
                Assert.AreEqual(assemblyProbability, result.Probability);
                Assert.AreEqual(AssessmentSectionAssemblyGroupCreator.CreateAssessmentSectionAssemblyGroup(assemblyGroup),
                                result.AssemblyGroup);
                Assert.AreEqual(AssemblyMethod.BOI2A1, resultWrapper.ProbabilityMethod);
                Assert.AreEqual(AssemblyMethod.BOI2B1, resultWrapper.AssemblyGroupMethod);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssemblyProbability = new Probability(random.NextDouble());
                kernel.AssemblyGroup = (EAssessmentGrade) 99;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleAssessmentSection(Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleAssessmentSection(Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSection_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleAssessmentSection(Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    AssemblyErrorMessageTestHelper.Create(string.Empty, EAssemblyErrors.InvalidCategoryLimits)
                }), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_CorrelatedFailureMechanismProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.AssembleAssessmentSection(null, Enumerable.Empty<double>(), random.NextDouble(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("correlatedFailureMechanismProbabilities", exception.ParamName);
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_UncorrelatedFailureMechanismProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var kernelFactory = mocks.Stub<IAssemblyToolKernelFactory>();
            mocks.ReplayAll();

            var random = new Random(21);
            var calculator = new AssessmentSectionAssemblyCalculator(kernelFactory);

            // Call
            void Call() => calculator.AssembleAssessmentSection(Enumerable.Empty<double>(), null, random.NextDouble(), random.NextDouble());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("uncorrelatedFailureMechanismProbabilities", exception.ParamName);
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_WithValidInput_InputCorrectlySetToKernel()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            IEnumerable<double> correlatedFailureMechanismProbabilities = Enumerable.Repeat(random.NextDouble(), random.Next(1, 10))
                                                                                    .ToArray();

            IEnumerable<double> uncorrelatedFailureMechanismProbabilities = Enumerable.Repeat(random.NextDouble(), random.Next(1, 10))
                                                                                      .ToArray();

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;

                var assessmentSectionCategories = new CategoriesList<AssessmentSectionCategory>(new[]
                {
                    new AssessmentSectionCategory(random.NextEnumValue<EAssessmentGrade>(), new Probability(0), new Probability(1))
                });
                AssemblyCategoryLimitsKernelStub categoryLimitsKernel = factory.LastCreatedAssemblyCategoryLimitsKernel;
                categoryLimitsKernel.AssessmentSectionCategoryLimits = assessmentSectionCategories;

                AssessmentSectionAssemblyKernelStub assessmentSectionAssemblyKernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                var assemblyProbability = new Probability(random.NextDouble());
                assessmentSectionAssemblyKernel.AssemblyProbability = assemblyProbability;
                assessmentSectionAssemblyKernel.AssemblyGroup = random.NextEnumValue<EAssessmentGrade>();

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                calculator.AssembleAssessmentSection(correlatedFailureMechanismProbabilities, uncorrelatedFailureMechanismProbabilities,
                                                     maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                Assert.IsTrue(categoryLimitsKernel.Calculated);
                ProbabilityAssert.AreEqual(maximumAllowableFloodingProbability, categoryLimitsKernel.AssessmentSection.MaximumAllowableFloodingProbability);
                ProbabilityAssert.AreEqual(signalFloodingProbability, categoryLimitsKernel.AssessmentSection.SignalFloodingProbability);

                Assert.IsTrue(assessmentSectionAssemblyKernel.ProbabilityCalculated);
                Assert.IsTrue(assessmentSectionAssemblyKernel.AssemblyGroupCalculated);
                Assert.IsFalse(assessmentSectionAssemblyKernel.PartialAssembly);
                Assert.AreSame(assessmentSectionCategories, assessmentSectionAssemblyKernel.Categories);

                AssertProbabilitiesInput(correlatedFailureMechanismProbabilities, assessmentSectionAssemblyKernel.CorrelatedFailureMechanismProbabilities);
                AssertProbabilitiesInput(uncorrelatedFailureMechanismProbabilities, assessmentSectionAssemblyKernel.UncorrelatedFailureMechanismProbabilities);

                Assert.AreEqual(assemblyProbability, assessmentSectionAssemblyKernel.AssemblyProbabilityInput);
            }
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_KernelWithCompleteOutput_OutputCorrectlyReturnedByCalculator()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                var assemblyProbability = new Probability(random.NextDouble());
                var assemblyGroup = random.NextEnumValue<EAssessmentGrade>();
                kernel.AssemblyProbability = assemblyProbability;
                kernel.AssemblyGroup = assemblyGroup;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                AssessmentSectionAssemblyResultWrapper resultWrapper = calculator.AssembleAssessmentSection(
                    Enumerable.Empty<double>(), Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                AssessmentSectionAssemblyResult result = resultWrapper.AssemblyResult;
                Assert.AreEqual(assemblyProbability, result.Probability);
                Assert.AreEqual(AssessmentSectionAssemblyGroupCreator.CreateAssessmentSectionAssemblyGroup(assemblyGroup),
                                result.AssemblyGroup);
                Assert.AreEqual(AssemblyMethod.BOI2A2, resultWrapper.ProbabilityMethod);
                Assert.AreEqual(AssemblyMethod.BOI2B1, resultWrapper.AssemblyGroupMethod);
            }
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_KernelWithInvalidOutput_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.AssemblyProbability = new Probability(random.NextDouble());
                kernel.AssemblyGroup = (EAssessmentGrade) 99;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleAssessmentSection(
                    Enumerable.Empty<double>(), Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<InvalidEnumArgumentException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_KernelThrowsException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleAssessmentSection(
                    Enumerable.Empty<double>(), Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<Exception>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), exception.Message);
            }
        }

        [Test]
        public void AssembleAssessmentSectionWithCorrelatedFailureMechanismProbabilities_KernelThrowsAssemblyException_ThrowsAssessmentSectionAssemblyCalculatorException()
        {
            // Setup
            var random = new Random(21);
            double signalFloodingProbability = random.NextDouble();
            double maximumAllowableFloodingProbability = signalFloodingProbability + 1e-3;

            using (new AssemblyToolKernelFactoryConfig())
            {
                var factory = (TestAssemblyToolKernelFactory) AssemblyToolKernelFactory.Instance;
                AssessmentSectionAssemblyKernelStub kernel = factory.LastCreatedAssessmentSectionAssemblyKernel;
                kernel.ThrowAssemblyExceptionOnCalculate = true;

                var calculator = new AssessmentSectionAssemblyCalculator(factory);

                // Call
                void Call() => calculator.AssembleAssessmentSection(
                    Enumerable.Empty<double>(), Enumerable.Empty<double>(), maximumAllowableFloodingProbability, signalFloodingProbability);

                // Assert
                var exception = Assert.Throws<AssessmentSectionAssemblyCalculatorException>(Call);
                Assert.IsInstanceOf<AssemblyException>(exception.InnerException);
                Assert.AreEqual(AssemblyErrorMessageCreator.CreateErrorMessage(new[]
                {
                    AssemblyErrorMessageTestHelper.Create(string.Empty, EAssemblyErrors.InvalidCategoryLimits)
                }), exception.Message);
            }
        }

        private static void AssertProbabilitiesInput(IEnumerable<double> expectedProbabilities, IEnumerable<Probability> actualProbabilitiesInput)
        {
            int nrOfProbabilities = expectedProbabilities.Count();
            Assert.AreEqual(nrOfProbabilities, actualProbabilitiesInput.Count());
            for (var i = 0; i < nrOfProbabilities; i++)
            {
                ProbabilityAssert.AreEqual(expectedProbabilities.ElementAt(i),
                                           actualProbabilitiesInput.ElementAt(i));
            }
        }
    }
}