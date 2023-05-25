﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismAssemblyFactoryTest
    {
        [Test]
        public void AssembleFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P2
                }
            };
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, "APath");

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreEqual(failureMechanism.GeneralInput.N, failureMechanismAssemblyCalculator.FailureMechanismN);
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput.AssemblyResult, failureMechanismAssemblyCalculator.SectionAssemblyResultsInput.Single());
                Assert.AreEqual(failureMechanism.GeneralInput.ApplyLengthEffectInSection, failureMechanismAssemblyCalculator.ApplyLengthEffect);
            }
        }

        [Test]
        public void AssembleFailureMechanism_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P2
                }
            };

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                FailureMechanismAssemblyResultWrapper result = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(
                    failureMechanism, assessmentSection);

                // Assert
                Assert.AreSame(calculator.AssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleFailureMechanism_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                AssemblyResult =
                {
                    ProbabilityResultType = FailureMechanismAssemblyProbabilityResultType.P2
                }
            };

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #region AssembleSection

        [Test]
        public void AssembleSection_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSection_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(sectionResult, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(sectionResult, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleSection_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var random = new Random(21);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = random.NextBoolean(),
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultProfileProbability = random.NextDouble(),
                FurtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                ProbabilityRefinementType = ProbabilityRefinementType.Both,
                RefinedSectionProbability = random.NextDouble()
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(sectionResult, failureMechanism, assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalFloodingProbability, calculatorInput.SignalFloodingProbability);
                Assert.AreEqual(failureMechanismContribution.MaximumAllowableFloodingProbability, calculatorInput.MaximumAllowableFloodingProbability);

                Assert.AreEqual(sectionResult.IsRelevant, calculatorInput.IsRelevant);
                Assert.IsTrue(calculatorInput.HasProbabilitySpecified);

                Assert.AreEqual(sectionResult.ManualInitialFailureMechanismResultSectionProbability, calculatorInput.InitialSectionProbability);
                Assert.AreEqual(sectionResult.FurtherAnalysisType, calculatorInput.FurtherAnalysisType);
                Assert.AreEqual(sectionResult.RefinedSectionProbability, calculatorInput.RefinedSectionProbability);
            }
        }

        [Test]
        public void AssembleSection_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new AdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResultWrapper result = GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(
                    sectionResult, failureMechanism, assessmentSection);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.SetSections(new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection()
            }, "APath");
            AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => GrassCoverErosionInwardsFailureMechanismAssemblyFactory.AssembleSection(
                    sectionResult, failureMechanism, assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }

        #endregion
    }
}