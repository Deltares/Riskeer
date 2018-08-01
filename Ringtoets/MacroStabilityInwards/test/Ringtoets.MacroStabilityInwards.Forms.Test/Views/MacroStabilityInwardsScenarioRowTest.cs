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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    [TestFixture]
    public class MacroStabilityInwardsScenarioRowTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsScenarioRow(null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsScenarioRow(calculation, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_WithCalculationWithOutput_PropertiesFromCalculation()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            const string name = "Test";
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            RoundedDouble contribution = random.NextRoundedDouble();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = name,
                IsRelevant = isRelevant,
                Contribution = contribution,
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.AreSame(calculation, row.Calculation);
            Assert.AreEqual(name, row.Name);
            Assert.AreEqual(isRelevant, row.IsRelevant);
            Assert.AreEqual(2, row.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(contribution * 100, row.Contribution, row.Contribution.GetAccuracy());
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.MacroStabilityInwardsProbability), row.FailureProbabilityMacroStabilityInwards);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithCalculationWithoutOutput_PropertiesFromCalculation()
        {
            // Setup
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            const string name = "Test";
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            RoundedDouble contribution = random.NextRoundedDouble();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = name,
                IsRelevant = isRelevant,
                Contribution = contribution
            };

            // Call
            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.AreSame(calculation, row.Calculation);
            Assert.AreEqual(name, row.Name);
            Assert.AreEqual(isRelevant, row.IsRelevant);
            Assert.AreEqual(2, row.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(contribution * 100, row.Contribution, row.Contribution.GetAccuracy());
            Assert.AreEqual("-", row.FailureProbabilityMacroStabilityInwards);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChange_NotifyObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            calculation.Attach(observer);

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            // Call
            row.IsRelevant = newValue;

            // Assert
            Assert.AreEqual(newValue, calculation.IsRelevant);
            mocks.VerifyAll();
        }

        [Test]
        public void Contribution_AlwaysOnChange_NotifyObserverAndCalculationPropertyChanged()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculationScenario();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            calculation.Attach(observer);

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            double newValue = new Random(21).NextDouble(0, 100);

            // Call
            row.Contribution = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue / 100, calculation.Contribution, calculation.Contribution.GetAccuracy());
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario();

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            Assert.AreEqual("-", row.FailureProbabilityMacroStabilityInwards);

            // When
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();
            row.Update();

            // Then
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.MacroStabilityInwardsProbability), row.FailureProbabilityMacroStabilityInwards);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput()
            };

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.MacroStabilityInwardsProbability), row.FailureProbabilityMacroStabilityInwards);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.AreEqual("-", row.FailureProbabilityMacroStabilityInwards);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputChangedAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput()
            };

            var row = new MacroStabilityInwardsScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            DerivedMacroStabilityInwardsOutput expectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.MacroStabilityInwardsProbability), row.FailureProbabilityMacroStabilityInwards);

            var random = new Random(11);

            // When
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = random.NextDouble()
            });
            row.Update();

            // Then
            DerivedMacroStabilityInwardsOutput newExpectedDerivedOutput = DerivedMacroStabilityInwardsOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(newExpectedDerivedOutput.MacroStabilityInwardsProbability), row.FailureProbabilityMacroStabilityInwards);
            mocks.VerifyAll();
        }
    }
}