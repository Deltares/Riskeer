﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class SemiProbabilisticPipingScenarioRowTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            // Call
            void Call() => new SemiProbabilisticPipingScenarioRow(new SemiProbabilisticPipingCalculationScenario(),
                                                                  new PipingFailureMechanism(), sectionConfiguration, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            // Call
            void Call() => new SemiProbabilisticPipingScenarioRow(new SemiProbabilisticPipingCalculationScenario(),
                                                                  null, sectionConfiguration, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Construction_SectionConfigurationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            void Call() => new SemiProbabilisticPipingScenarioRow(new SemiProbabilisticPipingCalculationScenario(),
                                                                  new PipingFailureMechanism(), null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionConfiguration", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            // Call
            var row = new SemiProbabilisticPipingScenarioRow(calculation, failureMechanism, sectionConfiguration, assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingScenarioRow<SemiProbabilisticPipingCalculationScenario>>(row);
            Assert.AreSame(calculation, row.CalculationScenario);

            TestHelper.AssertTypeConverter<SemiProbabilisticPipingScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(SemiProbabilisticPipingScenarioRow.FailureProbabilityUplift));
            TestHelper.AssertTypeConverter<SemiProbabilisticPipingScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(SemiProbabilisticPipingScenarioRow.FailureProbabilityHeave));
            TestHelper.AssertTypeConverter<SemiProbabilisticPipingScenarioRow, NoProbabilityValueDoubleConverter>(
                nameof(SemiProbabilisticPipingScenarioRow.FailureProbabilitySellmeijer));
        }

        [Test]
        [TestCase(1)]
        [TestCase(1000)]
        public void Constructor_CalculationWithOutput_ExpectedValues(double sectionLength)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };
            var failureMechanismSection = new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0),
                new Point2D(sectionLength, 0)
            });
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            // Call
            var row = new SemiProbabilisticPipingScenarioRow(calculation, failureMechanism, sectionConfiguration, assessmentSection);

            // Assert
            DerivedSemiProbabilisticPipingOutput expectedDerivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(calculation.Output, assessmentSection.FailureMechanismContribution.NormativeProbability);

            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            Assert.AreEqual(Math.Min(1.0, expectedDerivedOutput.PipingProbability * sectionConfiguration.GetN(
                                              failureMechanism.GeneralInput.B)),
                            row.SectionFailureProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationWithoutOutput_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            // Call
            var row = new SemiProbabilisticPipingScenarioRow(calculation, new PipingFailureMechanism(), sectionConfiguration, assessmentSection);

            // Assert
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.FailureProbabilityUplift);
            Assert.IsNaN(row.FailureProbabilityHeave);
            Assert.IsNaN(row.FailureProbabilitySellmeijer);
            Assert.IsNaN(row.SectionFailureProbability);
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

            var calculation = new SemiProbabilisticPipingCalculationScenario();
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            var row = new SemiProbabilisticPipingScenarioRow(calculation, failureMechanism, sectionConfiguration, assessmentSection);

            // Precondition
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.FailureProbabilityUplift);
            Assert.IsNaN(row.FailureProbabilityHeave);
            Assert.IsNaN(row.FailureProbabilitySellmeijer);
            Assert.IsNaN(row.SectionFailureProbability);

            // When
            calculation.Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput();
            row.Update();

            // Then
            DerivedSemiProbabilisticPipingOutput expectedDerivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(calculation.Output, assessmentSection.FailureMechanismContribution.NormativeProbability);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability * sectionConfiguration.GetN(
                                failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);
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

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            var row = new SemiProbabilisticPipingScenarioRow(calculation, failureMechanism, sectionConfiguration, assessmentSection);

            // Precondition
            DerivedSemiProbabilisticPipingOutput expectedDerivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(calculation.Output, assessmentSection.FailureMechanismContribution.NormativeProbability);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability * sectionConfiguration.GetN(
                                failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);

            // When
            calculation.Output = null;
            row.Update();

            // Then
            Assert.IsNaN(row.FailureProbability);
            Assert.IsNaN(row.FailureProbabilityUplift);
            Assert.IsNaN(row.FailureProbabilityHeave);
            Assert.IsNaN(row.FailureProbabilitySellmeijer);
            Assert.IsNaN(row.SectionFailureProbability);
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

            var calculation = new SemiProbabilisticPipingCalculationScenario
            {
                Output = PipingTestDataGenerator.GetRandomSemiProbabilisticPipingOutput()
            };
            FailureMechanismSection failureMechanismSection = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionConfiguration = new PipingFailureMechanismSectionConfiguration(failureMechanismSection);

            var row = new SemiProbabilisticPipingScenarioRow(calculation, failureMechanism, sectionConfiguration, assessmentSection);

            // Precondition
            DerivedSemiProbabilisticPipingOutput expectedDerivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(calculation.Output, assessmentSection.FailureMechanismContribution.NormativeProbability);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(expectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(expectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(expectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            Assert.AreEqual(expectedDerivedOutput.PipingProbability * sectionConfiguration.GetN(
                                failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);

            var random = new Random(11);

            // When
            calculation.Output = PipingTestDataGenerator.GetSemiProbabilisticPipingOutput(random.NextDouble(), random.NextDouble(), random.NextDouble());
            row.Update();

            // Then
            DerivedSemiProbabilisticPipingOutput newExpectedDerivedOutput = DerivedSemiProbabilisticPipingOutputFactory.Create(calculation.Output, assessmentSection.FailureMechanismContribution.NormativeProbability);
            Assert.AreEqual(newExpectedDerivedOutput.PipingProbability, row.FailureProbability);
            Assert.AreEqual(newExpectedDerivedOutput.UpliftProbability, row.FailureProbabilityUplift);
            Assert.AreEqual(newExpectedDerivedOutput.HeaveProbability, row.FailureProbabilityHeave);
            Assert.AreEqual(newExpectedDerivedOutput.SellmeijerProbability, row.FailureProbabilitySellmeijer);
            Assert.AreEqual(newExpectedDerivedOutput.PipingProbability * sectionConfiguration.GetN(
                                failureMechanism.GeneralInput.B),
                            row.SectionFailureProbability);
            mocks.VerifyAll();
        }
    }
}