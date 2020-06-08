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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingScenarioRowTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            void Call() => new PipingScenarioRow(calculation, null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            void Call() => new PipingScenarioRow(calculation, new PipingFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<PipingCalculationScenario>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);

            TestHelper.AssertTypeConverter<PipingScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingScenarioRow.FailureProbabilityUplift));
            TestHelper.AssertTypeConverter<PipingScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingScenarioRow.FailureProbabilityHeave));
            TestHelper.AssertTypeConverter<PipingScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingScenarioRow.FailureProbabilitySellmeijer));
        }

        [Test]
        public void Constructor_CalculationWithOutput_ExpectedValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };

            // Call
            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);

            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationWithoutOutput_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            var row = new PipingScenarioRow(calculation, new PipingFailureMechanism(), assessmentSection);

            // Assert
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.FailureProbabilityUplift);
            Assert.IsNaN(row.FailureProbabilityHeave);
            Assert.IsNaN(row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput());

            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.FailureProbabilityUplift);
            Assert.IsNaN(row.FailureProbabilityHeave);
            Assert.IsNaN(row.FailureProbabilitySellmeijer);

            // When
            calculation.Output = PipingOutputTestFactory.Create();
            row.Update();

            // Then
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };

            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.FailureProbabilityUplift);
            Assert.IsNaN(row.FailureProbabilityHeave);
            Assert.IsNaN(row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputChangedAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = PipingOutputTestFactory.Create()
            };

            var row = new PipingScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            DerivedPipingOutput expectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);

            var random = new Random(11);

            // When
            calculation.Output = PipingOutputTestFactory.Create(random.NextDouble(), random.NextDouble(), random.NextDouble());
            row.Update();

            // Then
            DerivedPipingOutput newExpectedDerivedOutput = DerivedPipingOutputFactory.Create(calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(newExpectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(newExpectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(newExpectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(newExpectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            mocks.VerifyAll();
        }
    }
}