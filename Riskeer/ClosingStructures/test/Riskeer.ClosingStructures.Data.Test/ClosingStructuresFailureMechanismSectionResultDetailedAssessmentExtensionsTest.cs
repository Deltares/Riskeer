// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.ClosingStructures.Data.Test
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultDetailedAssessmentExtensionsTest
    {
        [Test]
        public void GetDetailedAssessmentProbability_SectionResultNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => ClosingStructuresFailureMechanismSectionResultDetailedAssessmentExtensions.GetDetailedAssessmentProbability(
                null, Enumerable.Empty<StructuresCalculationScenario<ClosingStructuresInput>>(),
                new ClosingStructuresFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetDetailedAssessmentProbability(
                null, new ClosingStructuresFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
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
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetDetailedAssessmentProbability(
                Enumerable.Empty<StructuresCalculationScenario<ClosingStructuresInput>>(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            void Call() => failureMechanismSectionResult.GetDetailedAssessmentProbability(
                Enumerable.Empty<StructuresCalculationScenario<ClosingStructuresInput>>(),
                new ClosingStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetDetailedAssessmentProbability_MultipleScenarios_ReturnsValueBasedOnRelevantScenarios()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario1 = ClosingStructuresCalculationScenarioTestFactory.CreateClosingStructuresCalculationScenario(section);
            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario2 = ClosingStructuresCalculationScenarioTestFactory.CreateClosingStructuresCalculationScenario(section);
            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario3 = ClosingStructuresCalculationScenarioTestFactory.CreateClosingStructuresCalculationScenario(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) 0.2111;
            calculationScenario1.Output = new TestStructuresOutput(1.1);

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) 0.7889;
            calculationScenario1.Output = new TestStructuresOutput(2.2);

            calculationScenario3.IsRelevant = false;

            StructuresCalculationScenario<ClosingStructuresInput>[] calculations =
            {
                calculationScenario1,
                calculationScenario2,
                calculationScenario3
            };

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(calculations, failureMechanism, assessmentSection);

            // Assert
            Assert.AreEqual(0.3973850177700996, detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_NoScenarios_ReturnsNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(
                Enumerable.Empty<StructuresCalculationScenario<ClosingStructuresInput>>(),
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

            var failureMechanism = new ClosingStructuresFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario = ClosingStructuresCalculationScenarioTestFactory.CreateClosingStructuresCalculationScenario(section);
            calculationScenario.IsRelevant = false;

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(
                new[]
                {
                    calculationScenario
                },
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

            var failureMechanism = new ClosingStructuresFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);

            // Call
            double detailedAssessmentProbability = failureMechanismSectionResult.GetDetailedAssessmentProbability(new[]
            {
                calculationScenario
            }, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetDetailedAssessmentProbability_ScenarioWithNaNResults_ReturnsNaN()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            const double contribution1 = 0.2;
            const double contribution2 = 0.8;

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario1 = ClosingStructuresCalculationScenarioTestFactory.CreateClosingStructuresCalculationScenario(section);
            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario2 = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);

            calculationScenario1.IsRelevant = true;
            calculationScenario1.Contribution = (RoundedDouble) contribution1;

            calculationScenario2.IsRelevant = true;
            calculationScenario2.Contribution = (RoundedDouble) contribution2;
            calculationScenario2.Output = new TestStructuresOutput(double.NaN);

            StructuresCalculationScenario<ClosingStructuresInput>[] calculations =
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
        public void GetDetailedAssessmentProbability_RelevantScenarioContributionDoNotAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            StructuresCalculationScenario<ClosingStructuresInput> scenarioA = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            StructuresCalculationScenario<ClosingStructuresInput> scenarioB = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            scenarioA.Contribution = (RoundedDouble) contributionA;
            scenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new ClosingStructuresFailureMechanismSectionResult(section);

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
            void Call() => ((ClosingStructuresFailureMechanismSectionResult) null).GetTotalContribution(Enumerable.Empty<StructuresCalculationScenario<ClosingStructuresInput>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            void Call() => sectionResult.GetTotalContribution(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_WithScenarios_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var failureMechanismSectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            calculationScenario.Contribution = (RoundedDouble) 0.3211;

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario2 = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            calculationScenario2.Contribution = (RoundedDouble) 0.5435;

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario3 = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            calculationScenario3.IsRelevant = false;

            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario4 = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(5.0, 0.0),
                    new Point2D(10.0, 0.0)
                }));

            StructuresCalculationScenario<ClosingStructuresInput>[] calculationScenarios =
            {
                calculationScenario,
                calculationScenario2,
                calculationScenario3,
                calculationScenario4
            };

            // Call
            RoundedDouble totalContribution = failureMechanismSectionResult.GetTotalContribution(calculationScenarios);

            // Assert
            Assert.AreEqual((RoundedDouble) 0.8646, totalContribution);
        }

        [Test]
        public void GetCalculationScenarios_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((ClosingStructuresFailureMechanismSectionResult) null).GetCalculationScenarios(Enumerable.Empty<StructuresCalculationScenario<ClosingStructuresInput>>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarios_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);

            // Call
            void Call() => sectionResult.GetCalculationScenarios(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetCalculationScenarios_WithRelevantAndIrrelevantScenarios_ReturnsRelevantCalculationScenarios()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);
            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario2 = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(section);
            calculationScenario2.IsRelevant = false;

            // Call
            IEnumerable<StructuresCalculationScenario<ClosingStructuresInput>> relevantScenarios = sectionResult.GetCalculationScenarios(new[]
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
            var sectionResult = new ClosingStructuresFailureMechanismSectionResult(section);
            StructuresCalculationScenario<ClosingStructuresInput> calculationScenario = ClosingStructuresCalculationScenarioTestFactory.CreateNotCalculatedClosingStructuresCalculationScenario(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            IEnumerable<StructuresCalculationScenario<ClosingStructuresInput>> relevantScenarios = sectionResult.GetCalculationScenarios(new[]
            {
                calculationScenario
            });

            // Assert
            CollectionAssert.IsEmpty(relevantScenarios);
        }
    }
}