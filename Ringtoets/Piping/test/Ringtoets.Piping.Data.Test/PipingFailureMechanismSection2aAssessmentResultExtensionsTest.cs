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

using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSection2aAssessmentResultExtensionsTest
    {
        [Test]
        public void GetAssessmentLayerTwoA_MultipleScenarios_ReturnsValueBasedOnRelevantAndDoneScenarios()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            PipingCalculationScenario pipingCalculationScenario1 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(section);
            PipingCalculationScenario pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(section);
            PipingCalculationScenario pipingCalculationScenario3 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(section);
            PipingCalculationScenario pipingCalculationScenario4 = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble) contribution1;

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble) contribution2;

            pipingCalculationScenario3.IsRelevant = false;

            pipingCalculationScenario4.IsRelevant = true;

            var calculations = new[]
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2,
                pipingCalculationScenario3,
                pipingCalculationScenario4
            };

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(1.0231368235852602e-10, assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoScenarios_ReturnsZero()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<PipingCalculationScenario>(),
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoRelevantScenarios_ReturnsZero()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();


            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario calculationScenario1 = PipingCalculationScenarioFactory.CreateIrrelevantPipingCalculationScenario(section);
            PipingCalculationScenario calculationScenario2 = PipingCalculationScenarioFactory.CreateIrrelevantPipingCalculationScenario(section);

            var calculationScenarios = new[]
            {
                calculationScenario1,
                calculationScenario2
            };

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculationScenarios,
                                                                                              failureMechanism,
                                                                                              assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_ScenarioNotCalculated_ReturnsZero()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[]
            {
                pipingCalculationScenario
            }, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_ScenarioWithNanResults_ReturnsNaN()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            PipingCalculationScenario pipingCalculationScenario1 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(section);
            PipingCalculationScenario pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble)contribution1;

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble)contribution2;
            pipingCalculationScenario2.Output = new PipingOutput(new PipingOutput.ConstructionProperties());

            var calculations = new[]
            {
                pipingCalculationScenario1,
                pipingCalculationScenario2
            };

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetTotalContribution_Always_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 0.3;

            PipingCalculationScenario pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble) 0.5;

            PipingCalculationScenario pipingCalculationScenario3 = PipingCalculationScenarioFactory.CreateIrrelevantPipingCalculationScenario(section);

            var calculationScenarios = new[]
            {
                pipingCalculationScenario,
                pipingCalculationScenario2,
                pipingCalculationScenario3
            };

            // Call
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution(calculationScenarios);

            // Assert
            Assert.AreEqual((RoundedDouble) 0.8, totalContribution);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenarioNotCalculated_ReturnsStatusNotCalculated()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                pipingCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenariosCalculated_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                pipingCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_NoScenarios_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }
    }
}