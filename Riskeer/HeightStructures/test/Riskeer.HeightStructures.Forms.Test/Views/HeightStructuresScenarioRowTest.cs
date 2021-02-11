﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Views;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.Views;

namespace Riskeer.HeightStructures.Forms.Test.Views
{
    [TestFixture]
    public class HeightStructuresScenarioRowTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new HeightStructuresScenarioRow(new StructuresCalculationScenario<HeightStructuresInput>(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new HeightStructuresScenarioRow(new StructuresCalculationScenario<HeightStructuresInput>(),
                                                           new HeightStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>();

            // Call
            var row = new HeightStructuresScenarioRow(calculation, new HeightStructuresFailureMechanism(), assessmentSection);

            // Assert
            Assert.IsInstanceOf<ScenarioRow<StructuresCalculationScenario<HeightStructuresInput>>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithCalculationWithOutput_PropertiesFromCalculation()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            // Call
            var row = new HeightStructuresScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            ProbabilityAssessmentOutput expectedDerivedOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithCalculationWithoutOutput_PropertiesFromCalculation()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>();

            // Call
            var row = new HeightStructuresScenarioRow(calculation, failureMechanism, assessmentSection);

            // Assert
            Assert.IsNaN(row.FailureProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>();

            var row = new HeightStructuresScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            Assert.IsNaN(row.FailureProbability);

            // When
            calculation.Output = new TestStructuresOutput();
            row.Update();

            // Then
            ProbabilityAssessmentOutput expectedDerivedOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputSetToNullAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var row = new HeightStructuresScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            ProbabilityAssessmentOutput expectedDerivedOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.IsNaN(row.FailureProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenScenarioRow_WhenOutputChangedAndUpdate_ThenDerivedOutputUpdated()
        {
            // Given
            var failureMechanism = new HeightStructuresFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var row = new HeightStructuresScenarioRow(calculation, failureMechanism, assessmentSection);

            // Precondition
            ProbabilityAssessmentOutput expectedDerivedOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(expectedDerivedOutput.Probability, row.FailureProbability);

            var random = new Random(11);

            // When
            calculation.Output = new TestStructuresOutput(random.NextDouble());
            row.Update();

            // Then
            ProbabilityAssessmentOutput newExpectedDerivedOutput = HeightStructuresProbabilityAssessmentOutputFactory.Create(
                calculation.Output, failureMechanism, assessmentSection);
            Assert.AreEqual(newExpectedDerivedOutput.Probability, row.FailureProbability);
            mocks.VerifyAll();
        }
    }
}