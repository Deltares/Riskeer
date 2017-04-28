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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
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
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;
            const double probability1 = 1.0 / 1000000.0;
            const double probability2 = 1.0 / 2000000.0;

            PipingCalculationScenario pipingCalculationScenario1 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(probability1, section);
            PipingCalculationScenario pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(probability2, section);
            PipingCalculationScenario pipingCalculationScenario3 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(0.0, section);
            PipingCalculationScenario pipingCalculationScenario4 = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);

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
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculations);

            // Assert
            const double expectedProbability = probability1 * contribution1 + probability2 * contribution2;
            Assert.AreEqual(expectedProbability, assessmentLayerTwoA, 1e-8);
        }

        [Test]
        public void GetAssessmentLayerTwoA_ScenarioInvalidOutput_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[]
            {
                pipingCalculationScenario
            });

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoRelevantScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario calculationScenario1 = PipingCalculationScenarioFactory.CreateIrrelevantPipingCalculationScenario(section);
            PipingCalculationScenario calculationScenario2 = PipingCalculationScenarioFactory.CreateIrrelevantPipingCalculationScenario(section);

            var calculationScenarios = new[]
            {
                calculationScenario1,
                calculationScenario2
            };

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculationScenarios);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_ScenarioNotCalculated_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[]
            {
                pipingCalculationScenario
            });

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetTotalContribution_Always_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 0.3;

            PipingCalculationScenario pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
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
            FailureMechanismSection section = CreateSection();
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
        public void GetCalculationScenarioStatus_ScenarioInvalidOutput_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                pipingCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenarioInvalidOutputAndNotCalculated_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);
            pipingCalculationScenario.IsRelevant = true;

            PipingCalculationScenario pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble) 1.0;

            var calculationScenarios = new[]
            {
                pipingCalculationScenario,
                pipingCalculationScenario2
            };

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(calculationScenarios);

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenariosCalculated_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(0.1, section);
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
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("test", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });
        }
    }
}