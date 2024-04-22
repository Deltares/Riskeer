﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.AssemblyFactories;

namespace Riskeer.Integration.Data.Test.StandAlone.AssemblyFactories
{
    [TestFixture]
    public class PipingStructureFailureMechanismAssemblyFactoryTest
    {
        [Test]
        public void AssembleFailureMechanism_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void AssembleFailureMechanism_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism();

            // Call
            void Call() => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssembleFailureMechanism_WithInput_SetsInputOnCalculator()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism
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
                PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreSame(calculator.FailureMechanismSectionAssemblyResultOutput.AssemblyResult, failureMechanismAssemblyCalculator.SectionAssemblyResultsInput.Single());
            }
        }

        [Test]
        public void AssembleFailureMechanism_CalculatorRan_ReturnsExpectedOutput()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism
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
                FailureMechanismAssemblyResultWrapper result = PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                Assert.AreSame(calculator.AssemblyResultOutput, result);
            }
        }

        [Test]
        public void AssembleFailureMechanism_CalculatorThrowsException_ThrowsAssemblyException()
        {
            // Setup
            var failureMechanism = new PipingStructureFailureMechanism
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
                void Call() => PipingStructureFailureMechanismAssemblyFactory.AssembleFailureMechanism(failureMechanism, assessmentSection);

                // Assert
                var exception = Assert.Throws<AssemblyException>(Call);
                Exception innerException = exception.InnerException;
                Assert.IsInstanceOf<FailureMechanismAssemblyCalculatorException>(innerException);
                Assert.AreEqual(innerException.Message, exception.Message);
            }
        }
    }
}