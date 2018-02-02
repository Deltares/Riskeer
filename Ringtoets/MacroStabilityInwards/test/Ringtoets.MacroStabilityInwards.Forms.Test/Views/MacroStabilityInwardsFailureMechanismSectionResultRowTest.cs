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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<MacroStabilityInwardsFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.GetAssessmentLayerTwoA(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                          failureMechanism, assessmentSection),
                            row.AssessmentLayerTwoA);
            Assert.AreEqual(row.AssessmentLayerThree, result.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.AssessmentLayerTwoA));
            TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.AssessmentLayerThree));
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate test = () => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, null,
                                                                                                failureMechanism, assessmentSection);

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
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
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
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                                new MacroStabilityInwardsFailureMechanism(), null);

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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);

            // Call
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            MacroStabilityInwardsCalculationScenario scenarioA =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            MacroStabilityInwardsCalculationScenario scenarioB =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);
            scenarioA.Contribution = (RoundedDouble) contributionA;
            scenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, new[]
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, new[]
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
        public void AssessmentLayerTwoA_NoRelevantScenarios_ReturnNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, new[]
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
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            MacroStabilityInwardsCalculationScenario scenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(0.2, section);
            scenario.Contribution = (RoundedDouble) 1.0;

            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, new[]
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var random = new Random(21);
            double assessmentLayerThree = random.NextDouble();

            var sectionResult = new MacroStabilityInwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(sectionResult,
                                                                                Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                failureMechanism, assessmentSection);

            // Call
            row.AssessmentLayerThree = assessmentLayerThree;

            // Assert
            Assert.AreEqual(assessmentLayerThree, sectionResult.AssessmentLayerThree);
            mocks.VerifyAll();
        }
    }
}