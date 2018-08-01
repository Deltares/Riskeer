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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
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
    public class MacroStabilityInwardsFailureMechanismSectionResultDetailedAssessmentExtensionsTest
    {
        [Test]
        public void GetDetailedAssessmentProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => MacroStabilityInwardsFailureMechanismSectionResultDetailedAssessmentExtensions.GetDetailedAssessmentProbability(
                null, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetDetailedAssessmentProbability(null, new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetDetailedAssessmentProbability(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                                     null,
                                                                                                     assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => failureMechanismSectionResult.GetDetailedAssessmentProbability(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                                     new MacroStabilityInwardsFailureMechanism(),
                                                                                                     null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            const double factorOfStability1 = 1.0 / 10.0;
            const double factorOfStability2 = 1.0 / 20.0;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factorOfStability1, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factorOfStability2, section);
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(0.0, section);

            macroStabilityInwardsCalculationScenario1.IsRelevant = true;
            macroStabilityInwardsCalculationScenario1.Contribution = (RoundedDouble) 0.2111;

            macroStabilityInwardsCalculationScenario2.IsRelevant = true;
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) 0.7889;

            macroStabilityInwardsCalculationScenario3.IsRelevant = false;

            MacroStabilityInwardsCalculationScenario[] calculations =
            {
                macroStabilityInwardsCalculationScenario1,
                macroStabilityInwardsCalculationScenario2,
                macroStabilityInwardsCalculationScenario3
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(0.99012835996547233, detailedAssessmentProbability, 1e-8);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_NoRelevantScenarios_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
            {
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section)
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculationScenarios,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_ScenarioNotCalculated_ReturnsNaN()
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
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(new[]
            {
                macroStabilityInwardsCalculationScenario
            }, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_ScenarioWithNanResults_ReturnsNaN()
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
            calculationScenario1.Contribution = (RoundedDouble) contribution1;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;
            calculationScenario2.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            MacroStabilityInwardsCalculationScenario[] calculations =
            {
                calculationScenario1,
                calculationScenario2
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0.2, 0.8 - 1e5)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        [TestCase(-5, -8)]
        [TestCase(13, 2)]
        public void GetDetailedAssessmentProbability_RelevantScenarioContributionDontAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            MacroStabilityInwardsCalculationScenario scenarioA = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario scenarioB = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            scenarioA.Contribution = (RoundedDouble) contributionA;
            scenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            double detailedAssessmentProbability = result.GetDetailedAssessmentProbability(new[]
            {
                scenarioA,
                scenarioB
            }, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetTotalContribution_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsFailureMechanismSectionResult) null).GetTotalContribution(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => sectionResult.GetTotalContribution(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_WithScenarios_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            macroStabilityInwardsCalculationScenario.Contribution = (RoundedDouble) 0.3211;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(section);
            macroStabilityInwardsCalculationScenario2.Contribution = (RoundedDouble) 0.5435;

            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario3 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
            {
                macroStabilityInwardsCalculationScenario,
                macroStabilityInwardsCalculationScenario2,
                macroStabilityInwardsCalculationScenario3
            };

            // Call
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution(calculationScenarios);

            // Assert
            Assert.AreEqual((RoundedDouble) 0.8646, totalContribution);
        }

        [Test]
        public void GetCalculationScenarioStatus_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsFailureMechanismSectionResult) null).GetCalculationScenarioStatus(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarioStatus_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => sectionResult.GetCalculationScenarioStatus(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
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

            MacroStabilityInwardsCalculationScenario[] calculationScenarios =
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

        [Test]
        public void GetCalculationScenarios_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsFailureMechanismSectionResult) null).GetCalculationScenarios(
                Enumerable.Empty<MacroStabilityInwardsCalculationScenario>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarios_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => sectionResult.GetCalculationScenarios(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarios_WithRelevantAndIrrelevantScenarios_ReturnsRelevantCalculationScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            MacroStabilityInwardsCalculationScenario calculationScenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario calculationScenario2 =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            // Call
            IEnumerable<MacroStabilityInwardsCalculationScenario> relevantScenarios = sectionResult.GetCalculationScenarios(new[]
            {
                calculationScenario,
                calculationScenario2
            });

            // Assert
            Assert.AreEqual(calculationScenario, relevantScenarios.Single());
        }

        [Test]
        public void GetCalculationScenarios_WithoutScenarioIntersectingSection_ReturnsNoCalculationScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
            {
                new Point2D(999, 999),
                new Point2D(998, 998)
            });
            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            MacroStabilityInwardsCalculationScenario calculationScenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            IEnumerable<MacroStabilityInwardsCalculationScenario> relevantScenarios = sectionResult.GetCalculationScenarios(new[]
            {
                calculationScenario
            });

            // Assert
            CollectionAssert.IsEmpty(relevantScenarios);
        }
    }
}