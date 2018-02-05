﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            var row = new PipingFailureMechanismSectionResultRow(result, Enumerable.Empty<PipingCalculationScenario>(),
                                                                 failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<PipingFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.GetAssessmentLayerTwoA(Enumerable.Empty<PipingCalculationScenario>(),
                                                          failureMechanism, assessmentSection),
                            row.AssessmentLayerTwoA);
            Assert.AreEqual(row.AssessmentLayerThree, result.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<PipingFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingFailureMechanismSectionResultRow.AssessmentLayerTwoA));
            TestHelper.AssertTypeConverter<PipingFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingFailureMechanismSectionResultRow.AssessmentLayerThree));
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new PipingFailureMechanismSectionResultRow(result, null, failureMechanism, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculations", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new PipingFailureMechanismSectionResultRow(result, Enumerable.Empty<PipingCalculationScenario>(),
                                                                                 null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new PipingFailureMechanismSectionResultRow(result, Enumerable.Empty<PipingCalculationScenario>(),
                                                                                 new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void AssessmentLayerTwoA_NoScenarios_ReturnNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            var row = new PipingFailureMechanismSectionResultRow(result, Enumerable.Empty<PipingCalculationScenario>(),
                                                                 failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(row.AssessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0.2, 0.8 - 1e5)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        [TestCase(-5, -8)]
        [TestCase(13, 2)]
        public void AssessmentLayerTwoA_RelevantScenarioContributionDontAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            PipingCalculationScenario scenarioA = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(section);
            PipingCalculationScenario scenarioB = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(section);
            scenarioA.Contribution = (RoundedDouble) contributionA;
            scenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, new[]
            {
                scenarioA,
                scenarioB
            }, failureMechanism, assessmentSection);

            // Call
            double assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_NoRelevantScenariosDone_ReturnNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(section);

            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, new[]
            {
                scenario
            }, failureMechanism, assessmentSection);

            // Call
            double assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_RelevantScenariosDone_ResultOfSection()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(section);
            scenario.Contribution = (RoundedDouble) 1.0;

            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, new[]
            {
                scenario
            }, failureMechanism, assessmentSection);

            // Call
            double assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            double expected = result.GetAssessmentLayerTwoA(new[]
            {
                scenario
            }, failureMechanism, assessmentSection);
            Assert.AreEqual(expected, assessmentLayerTwoA, 1e-6);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_ValueSet_ReturnExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var random = new Random(21);
            double assessmentLayerThree = random.NextDouble();

            var sectionResult = new PipingFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var row = new PipingFailureMechanismSectionResultRow(sectionResult, Enumerable.Empty<PipingCalculationScenario>(),
                                                                 failureMechanism, assessmentSection);

            // Call
            row.AssessmentLayerThree = assessmentLayerThree;

            // Assert
            Assert.AreEqual(assessmentLayerThree, sectionResult.AssessmentLayerThree);
            mocks.VerifyAll();
        }
    }
}