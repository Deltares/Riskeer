﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultAssemblyFactoryTest
    {
        #region Simple Assessment

        [Test]
        public void AssembleSimpleAssessment_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
        }

        [Test]
        public void AssembleSimpleAssessment_WithSectionResult_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(failureMechanismSection)
            {
                SimpleAssessmentResult = random.NextEnumValue<SimpleAssessmentResultType>()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                PipingFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                Assert.AreEqual(sectionResult.SimpleAssessmentResult, calculator.SimpleAssessmentInput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_AssemblyRan_ReturnsOutput()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssembly actualOutput =
                    PipingFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.SimpleAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
            }
        }

        [Test]
        public void AssembleSimpleAssessment_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new PipingFailureMechanismSectionResult(failureMechanismSection);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleSimpleAssessment(sectionResult);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion

        #region Detailed Assessment

        [Test]
        public void AssembleDetailedAssembly_FailureMechanismSectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                null,
                Enumerable.Empty<PipingCalculationScenario>(),
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanismSectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssembly_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new PipingFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                null,
                new PipingFailureMechanism(),
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssembly_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new PipingFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                Enumerable.Empty<PipingCalculationScenario>(),
                null,
                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void AssembleDetailedAssembly_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                new PipingFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()),
                Enumerable.Empty<PipingCalculationScenario>(),
                new PipingFailureMechanism(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleDetailedAssembly_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new PipingFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var calculationScenarios = new[]
            {
                PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput()
            };

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                AssemblyCategoriesCalculatorStub categoryCalculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call

                PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    calculationScenarios,
                    failureMechanism,
                    assessmentSection);

                // Assert
                Assert.AreEqual(sectionResult.GetDetailedAssessmentProbability(
                                    calculationScenarios,
                                    failureMechanism,
                                    assessmentSection),
                                calculator.DetailedAssessmentProbabilityInput);
                Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.GetN(sectionResult.Section.Length),
                                calculator.DetailedAssessmentNInput);
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.SignalingNorm, categoryCalculator.SignalingNorm);
                Assert.AreEqual(assessmentSection.FailureMechanismContribution.LowerLimitNorm, categoryCalculator.LowerLimitNorm);
                Assert.AreEqual(failureMechanism.Contribution, categoryCalculator.FailureMechanismContribution);
                Assert.AreEqual(failureMechanism.PipingProbabilityAssessmentInput.GetN(
                                    failureMechanism.PipingProbabilityAssessmentInput.SectionLength),
                                categoryCalculator.N);
                Assert.AreSame(categoryCalculator.FailureMechanismSectionCategoriesOutput, calculator.DetailedAssessmentCategoriesInput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssembly_AssemblyRan_ReturnsOutput()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new PipingFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssembly actualOutput =
                    PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                        sectionResult,
                        new[]
                        {
                            PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput()
                        },
                        new PipingFailureMechanism(),
                        assessmentSection);

                // Assert
                FailureMechanismSectionAssembly calculatorOutput = calculator.DetailedAssessmentAssemblyOutput;
                Assert.AreSame(calculatorOutput, actualOutput);
                mocks.VerifyAll();
            }
        }

        [Test]
        public void AssembleDetailedAssembly_CalculatorThrowsExceptions_ThrowsAssemblyException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new PipingFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var sectionResult = new PipingFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorfactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                TestDelegate call = () => PipingFailureMechanismSectionResultAssemblyFactory.AssembleDetailedAssembly(
                    sectionResult,
                    new[]
                    {
                        PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput()
                    },
                    new PipingFailureMechanism(),
                    assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}