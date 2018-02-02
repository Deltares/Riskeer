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

using System;
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSection2aAssessmentResultExtensionsTest
    {
        [Test]
        public void GetAssessmentLayerTwoA_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => MacroStabilityInwardsFailureMechanismSection2aAssessmentResultExtensions.GetAssessmentLayerTwoA(
                null, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetAssessmentLayerTwoA(null, new MacroStabilityInwardsFailureMechanism(),
                                                                                           assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                           null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                           new MacroStabilityInwardsFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetAssessmentLayerTwoA_MultipleScenarios_ReturnsValueBasedOnRelevantAndDoneScenarios()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;
            const double factorOfStability1 = 1.0 / 1000000.0;
            const double factorOfStability2 = 1.0 / 2000000.0;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factorOfStability1, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factorOfStability2, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(0.0, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario4 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

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
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(0.99801160064610306, assessmentLayerTwoA, 1e-8);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoScenarios_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                              failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario calculationScenario1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario calculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

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
        public void GetAssessmentLayerTwoA_ScenarioNotCalculated_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Call
            double assessmentLayerTwoA = failureMechanismSectionResult.GetAssessmentLayerTwoA(new[]
            {
                macroStabilityInwardsCalculationScenario
            }, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void GetAssessmentLayerTwoA_ScenarioWithNanResults_ReturnsNaN()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            MacroStabilityInwardsCalculationScenario calculationScenario1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(0, section);
            MacroStabilityInwardsCalculationScenario calculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble)contribution1;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble)contribution2;
            calculationScenario2.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            var calculations = new[]
            {
                calculationScenario1,
                calculationScenario2
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
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario.Contribution = (RoundedDouble) 0.3;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(section);
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) 0.5;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

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
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenarioNaNOutput_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(new[]
            {
                macroStabilityInwardsCalculationScenario
            });

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenariosWithNotCalculatedScenario_ReturnsStatusNotCalculated()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(section);

            var calculationScenarios = new[]
            {
                macroStabilityInwardsCalculationScenario,
                macroStabilityInwardsCalculationScenario2
            };

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(calculationScenarios);

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void GetCalculationScenarioStatus_ScenarioCalculated_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(0.1, section);
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
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.GetCalculationScenarioStatus(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>());

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }
    }
}