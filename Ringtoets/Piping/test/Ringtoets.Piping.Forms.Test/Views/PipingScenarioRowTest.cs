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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenarioRowTest
    {
        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new PipingScenarioRow(null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("calculation", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            TestDelegate test = () => new PipingScenarioRow(calculation, null, assessmentSection);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            TestDelegate call = () => new PipingScenarioRow(calculation, new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationWithOutput_ExpectedValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            const string name = "Test";
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            RoundedDouble contribution = random.NextRoundedDouble();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = name,
                IsRelevant = isRelevant,
                Contribution = contribution,
                Output = PipingOutputTestFactory.Create()
            };

            // Call
            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);

            Assert.AreSame(calculation, row.Calculation);
            Assert.AreEqual(name, row.Name);
            Assert.AreEqual(isRelevant, row.IsRelevant);
            Assert.AreEqual(contribution * 100, row.Contribution, row.Contribution.GetAccuracy());
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.PipingProbability), row.FailureProbabilityPiping);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.UpliftProbability), row.FailureProbabilityUplift);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.HeaveProbability), row.FailureProbabilityHeave);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.SellmeijerProbability), row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationWithoutOutput_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string name = "Test";
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            RoundedDouble contribution = random.NextRoundedDouble();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Name = name,
                IsRelevant = isRelevant,
                Contribution = contribution
            };

            // Call
            var row = new PipingScenarioRow(calculation, new PipingFailureMechanism(), assessmentSection);

            // Assert
            Assert.AreSame(calculation, row.Calculation);
            Assert.AreEqual(name, row.Name);
            Assert.AreEqual(isRelevant, row.IsRelevant);
            Assert.AreEqual(contribution * 100, row.Contribution, row.Contribution.GetAccuracy());
            Assert.AreEqual("-", row.FailureProbabilityPiping);
            Assert.AreEqual("-", row.FailureProbabilityUplift);
            Assert.AreEqual("-", row.FailureProbabilityHeave);
            Assert.AreEqual("-", row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void IsRelevant_AlwaysOnChange_NotifyObserversAndCalculationPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            calculation.Attach(observer);

            var row = new PipingScenarioRow(calculation, new PipingFailureMechanism(), assessmentSection);

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
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            double newValue = new Random(21).NextDouble() * 100;

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());
            calculation.Attach(observer);

            var row = new PipingScenarioRow(calculation, new PipingFailureMechanism(), assessmentSection);

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
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            Assert.AreEqual("-", row.FailureProbabilityPiping);
            Assert.AreEqual("-", row.FailureProbabilityUplift);
            Assert.AreEqual("-", row.FailureProbabilityHeave);
            Assert.AreEqual("-", row.FailureProbabilitySellmeijer);

            // When
            calculation.Output = PipingOutputTestFactory.Create();
            row.Update();

            // Then
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.PipingProbability), row.FailureProbabilityPiping);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.UpliftProbability), row.FailureProbabilityUplift);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.HeaveProbability), row.FailureProbabilityHeave);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.SellmeijerProbability), row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };

            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.PipingProbability), row.FailureProbabilityPiping);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.UpliftProbability), row.FailureProbabilityUplift);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.HeaveProbability), row.FailureProbabilityHeave);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.SellmeijerProbability), row.FailureProbabilitySellmeijer);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.AreEqual("-", row.FailureProbabilityPiping);
            Assert.AreEqual("-", row.FailureProbabilityUplift);
            Assert.AreEqual("-", row.FailureProbabilityHeave);
            Assert.AreEqual("-", row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputChangedAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };

            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.PipingProbability), row.FailureProbabilityPiping);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.UpliftProbability), row.FailureProbabilityUplift);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.HeaveProbability), row.FailureProbabilityHeave);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(expectedDerivedOutput.SellmeijerProbability), row.FailureProbabilitySellmeijer);

            var random = new Random(11);

            // When
            calculation.Output = PipingOutputTestFactory.Create(random.NextDouble(), random.NextDouble(), random.NextDouble());
            row.Update();

            // Then
            DerivedPipingOutput newExpectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(newExpectedDerivedOutput.PipingProbability), row.FailureProbabilityPiping);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(newExpectedDerivedOutput.UpliftProbability), row.FailureProbabilityUplift);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(newExpectedDerivedOutput.HeaveProbability), row.FailureProbabilityHeave);
            Assert.AreEqual(ProbabilityFormattingHelper.Format(newExpectedDerivedOutput.SellmeijerProbability), row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }
    }
}