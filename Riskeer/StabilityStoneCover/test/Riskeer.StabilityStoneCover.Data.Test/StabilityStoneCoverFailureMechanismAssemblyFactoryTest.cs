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

namespace Riskeer.StabilityStoneCover.Data.Test
{
    [TestFixture]
    public class StabilityStoneCoverFailureMechanismAssemblyFactoryTest
    {
        [Test]
        public void AssembleSection_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(null, failureMechanism, assessmentSection);

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
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            // Call
            void Call() => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(sectionResult, null, assessmentSection);

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
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            void Call() => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(sectionResult, failureMechanism, null);

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
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section)
            {
                IsRelevant = random.NextBoolean(),
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                FurtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                RefinedSectionProbability = random.NextDouble()
            };

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(sectionResult, failureMechanism, assessmentSection);

                // Assert
                FailureMechanismSectionAssemblyInput calculatorInput = calculator.FailureMechanismSectionAssemblyInput;
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanismContribution.SignalingNorm, calculatorInput.SignalingNorm);
                Assert.AreEqual(failureMechanismContribution.LowerLimitNorm, calculatorInput.LowerLimitNorm);

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
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;

                // Call
                FailureMechanismSectionAssemblyResult result = StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(
                    sectionResult, failureMechanism, assessmentSection);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleSection_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(section);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub calculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                calculator.ThrowExceptionOnCalculate = true;

                // Call
                void Call() => StabilityStoneCoverFailureMechanismAssemblyFactory.AssembleSection(
                    sectionResult, failureMechanism, assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismSectionAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}