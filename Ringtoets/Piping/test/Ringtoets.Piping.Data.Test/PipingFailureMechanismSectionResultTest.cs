﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            PipingFailureMechanismSectionResult sectionResult = new PipingFailureMechanismSectionResult(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsFalse(sectionResult.AssessmentLayerOne);
            Assert.AreEqual((RoundedDouble) 0, sectionResult.GetAssessmentLayerTwoA(new PipingCalculationScenario[0]));
            Assert.AreEqual((RoundedDouble) 0, sectionResult.AssessmentLayerThree);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingFailureMechanismSectionResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_Always_ReturnsSetValue(bool newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerOne);
        }

        [Test]
        public void AssessmentLayerTwoA_MultipleScenarios_ReturnsValueBasedOnRelevantAndDoneScenarios()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var contribution1 = 0.2;
            var contribution2 = 0.8;
            var probability1 = 1.0 / 1000000;
            var probability2 = 1.0 / 2000000;

            var pipingCalculationScenario1 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(probability1, section);
            var pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(probability2, section);
            var pipingCalculationScenario3 = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(0.0, section);
            var pipingCalculationScenario4 = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);

            pipingCalculationScenario1.IsRelevant = true;
            pipingCalculationScenario1.Contribution = (RoundedDouble)contribution1;

            pipingCalculationScenario2.IsRelevant = true;
            pipingCalculationScenario2.Contribution = (RoundedDouble)contribution2;

            pipingCalculationScenario3.IsRelevant = false;

            pipingCalculationScenario4.IsRelevant = true;

            var calculations = new []
            {
                pipingCalculationScenario1, pipingCalculationScenario2, pipingCalculationScenario3, pipingCalculationScenario4
            };

            // Call
            RoundedDouble? assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculations);

            // Assert
            Assert.AreEqual(1.0 / ((probability1) * contribution1 + (probability2) * contribution2), assessmentLayerTwoA, 1e-8);
        }

        [Test]
        public void AssessmentLayerTwoA_ScenarioInvalidOutput_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            RoundedDouble assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[] { pipingCalculationScenario });

            // Assert
            Assert.AreEqual((RoundedDouble) 0, assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_NoScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            RoundedDouble? assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            Assert.AreEqual((RoundedDouble) 0.0, assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_NoRelevantScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var calculationScenarioMock = PipingCalculationScenarioFactory.CreateIrreleveantPipingCalculationScenario(section);
            var calculationScenarioMock2 = PipingCalculationScenarioFactory.CreateIrreleveantPipingCalculationScenario(section);

            var calculationScenarios = new[] { calculationScenarioMock, calculationScenarioMock2 };

            // Call
            RoundedDouble assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculationScenarios);

            // Assert
            Assert.AreEqual((RoundedDouble) 0.0, assessmentLayerTwoA);
        }

        [Test]
        public void TotalContribution_Always_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 0.3;

            var pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble)0.5;

            var pipingCalculationScenario3 = PipingCalculationScenarioFactory.CreateIrreleveantPipingCalculationScenario(section);

            var calculationScenarios = new[] { pipingCalculationScenario, pipingCalculationScenario2, pipingCalculationScenario3 };

            // Call
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution(calculationScenarios);

            // Assert
            Assert.AreEqual((RoundedDouble) 0.8, totalContribution);
        }

        [Test]
        public void AssessmentLayerThree_Always_ReturnsSetValue()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);
            var assessmentLayerThreeValue = (RoundedDouble) 3.0;

            // Call
            failureMechanismSectionResult.AssessmentLayerThree = assessmentLayerThreeValue;

            // Assert
            Assert.AreEqual(assessmentLayerThreeValue, failureMechanismSectionResult.AssessmentLayerThree);
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioNotCalculated_ReturnsStatusNotCalculated()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[] { pipingCalculationScenario });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioInvalidOutput_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[] { pipingCalculationScenario });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioInvalidOutputAndNotCalculated_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);
            pipingCalculationScenario.IsRelevant = true;

            var pipingCalculationScenario2 = PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);
            pipingCalculationScenario2.Contribution = (RoundedDouble) 1.0;
            
            var calculationScenarios = new[] { pipingCalculationScenario, pipingCalculationScenario2 };

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(calculationScenarios);

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void CalculationScenarioStatus_ScenariosCalculated_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var expectedProbability = 41661830;
            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(expectedProbability, section);
            pipingCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[] { pipingCalculationScenario });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        [Test]
        public void CalculationScenarioStatus_NoScenarios_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        [Test]
        public void AssessmentLayerTwoA_ScenarioNotCalculated_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var pipingCalculationScenario = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Call
            RoundedDouble assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new [] { pipingCalculationScenario });

            // Assert
            Assert.AreEqual((RoundedDouble) 0, assessmentLayerTwoA);
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