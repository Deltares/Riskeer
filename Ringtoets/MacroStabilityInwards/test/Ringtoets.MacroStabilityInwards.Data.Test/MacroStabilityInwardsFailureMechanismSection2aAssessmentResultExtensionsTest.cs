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
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSection2aAssessmentResultExtensionsTest
    {
        [Test]
        public void GetAssessmentLayerTwoA_MultipleScenarios_ReturnsValueBasedOnRelevantAndDoneScenarios()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;
            const double probability1 = 1.0 / 1000000.0;
            const double probability2 = 1.0 / 2000000.0;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario1 = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(probability1, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(probability2, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(0.0, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario4 = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);

            macroStabilityInwardsCalculationScenario1.IsRelevant = true;
            macroStabilityInwardsCalculationScenario1.Contribution = (RoundedDouble) contribution1;

            macroStabilityInwardsCalculationScenario2.IsRelevant = true;
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) contribution2;

            macroStabilityInwardsCalculationScenario3.IsRelevant = false;

            macroStabilityInwardsCalculationScenario4.IsRelevant = true;

            var calculations = new[]
            {
                macroStabilityInwardsCalculationScenario1,
                macroStabilityInwardsCalculationScenario2,
                macroStabilityInwardsCalculationScenario3,
                macroStabilityInwardsCalculationScenario4
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
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>());

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoRelevantScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario calculationScenario1 = MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario calculationScenario2 = MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

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
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void GetTotalContribution_Always_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario.Contribution = (RoundedDouble) 0.3;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) 0.5;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 = MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            var calculationScenarios = new[]
            {
                macroStabilityInwardsCalculationScenario,
                macroStabilityInwardsCalculationScenario2,
                macroStabilityInwardsCalculationScenario3
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
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenarioInvalidOutput_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenarioInvalidOutputAndNotCalculated_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario.IsRelevant = true;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) 1.0;

            var calculationScenarios = new[]
            {
                macroStabilityInwardsCalculationScenario,
                macroStabilityInwardsCalculationScenario2
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
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(0.1, section);
            macroStabilityInwardsCalculationScenario.Contribution = (RoundedDouble) 1.0;

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_NoScenarios_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>());

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