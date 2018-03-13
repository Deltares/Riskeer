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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
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
            Assert.AreEqual(row.SimpleAssessmentResult, result.SimpleAssessmentResult);
            Assert.AreEqual(result.GetDetailedAssessmentProbability(Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                    failureMechanism, assessmentSection),
                            row.DetailedAssessmentProbability);
            Assert.AreEqual(row.AssessmentLayerThree, result.TailorMadeAssessmentProbability);

            TestHelper.AssertTypeConverter<MacroStabilityInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(MacroStabilityInwardsFailureMechanismSectionResultRow.DetailedAssessmentProbability));
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
        public void SimpleAssessmentResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var random = new Random(39);
            var newValue = random.NextEnumValue<SimpleAssessmentResultType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new MacroStabilityInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new MacroStabilityInwardsFailureMechanismSectionResultRow(result, Enumerable.Empty<MacroStabilityInwardsCalculationScenario>(),
                                                                                new MacroStabilityInwardsFailureMechanism(), assessmentSection);

            // Call
            row.SimpleAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.SimpleAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentProbability_RelevantScenariosDone_ResultOfSection()
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
            double detailedAssessmentProbability = row.DetailedAssessmentProbability;

            // Assert
            double expected = result.GetDetailedAssessmentProbability(new[]
            {
                scenario
            }, failureMechanism, assessmentSection);
            Assert.AreEqual(expected, detailedAssessmentProbability, 1e-6);
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
            Assert.AreEqual(assessmentLayerThree, sectionResult.TailorMadeAssessmentProbability);
            mocks.VerifyAll();
        }
    }
}