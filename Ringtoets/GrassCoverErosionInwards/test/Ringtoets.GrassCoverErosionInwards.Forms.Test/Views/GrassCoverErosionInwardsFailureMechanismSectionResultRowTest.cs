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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.Primitives;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<GrassCoverErosionInwardsFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.SimpleAssessmentResult, row.SimpleAssessmentResult);
            Assert.AreEqual(result.GetDetailedAssessmentProbability(failureMechanism, assessmentSection), row.DetailedAssessmentProbability);
            Assert.AreEqual(row.AssessmentLayerThree, result.AssessmentLayerThree);

            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.DetailedAssessmentProbability));
            TestHelper.AssertTypeConverter<GrassCoverErosionInwardsFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(GrassCoverErosionInwardsFailureMechanismSectionResultRow.AssessmentLayerThree));
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
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, null, assessmentSection);

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
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result,
                                                                                                   new GrassCoverErosionInwardsFailureMechanism(),
                                                                                                   null);

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
            var newValue = random.NextEnumValue<SimpleAssessmentResultValidityOnlyType>();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result,
                                                                                   new GrassCoverErosionInwardsFailureMechanism(),
                                                                                   assessmentSection);

            // Call
            row.SimpleAssessmentResult = newValue;

            // Assert
            Assert.AreEqual(newValue, result.SimpleAssessmentResult);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentProbability_NoCalculationSet_ReturnNaN()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(sectionResult.Calculation);

            var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection);

            // Call
            double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(CalculationScenarioStatus.Failed)]
        [TestCase(CalculationScenarioStatus.NotCalculated)]
        public void DetailedAssessmentProbability_CalculationNotDone_ReturnNaN(CalculationScenarioStatus status)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            if (status == CalculationScenarioStatus.Failed)
            {
                calculation.Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(double.NaN),
                                                                        new TestDikeHeightOutput(double.NaN),
                                                                        new TestOvertoppingRateOutput(double.NaN));
            }

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection);

            // Call
            double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

            // Assert
            Assert.IsNaN(detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void DetailedAssessmentProbability_CalculationSuccessful_ReturnDetailedAssessmentProbability()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(0.95),
                                                            new TestDikeHeightOutput(0),
                                                            new TestOvertoppingRateOutput(0))
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = calculation
            };

            var resultRow = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection);

            // Call
            double detailedAssessmentProbability = resultRow.DetailedAssessmentProbability;

            // Assert
            Assert.AreEqual(0.17105612630848185, detailedAssessmentProbability);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionResultCalculation_NoCalculationSetOnSectionResult_ReturnNull()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section);

            // Precondition
            Assert.IsNull(result.Calculation);

            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection);

            // Call
            GrassCoverErosionInwardsCalculation calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.IsNull(calculation);
            mocks.VerifyAll();
        }

        [Test]
        public void GetSectionResultCalculation_WithCalculationSetOnSectionResult_ReturnCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var grassCoverErosionInwardsCalculation = new GrassCoverErosionInwardsCalculation();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new GrassCoverErosionInwardsFailureMechanismSectionResult(section)
            {
                Calculation = grassCoverErosionInwardsCalculation
            };

            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(result, failureMechanism, assessmentSection);

            // Call
            GrassCoverErosionInwardsCalculation calculation = row.GetSectionResultCalculation();

            // Assert
            Assert.AreSame(grassCoverErosionInwardsCalculation, calculation);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_ValueSet_ReturnExpectedValue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            var random = new Random(21);
            double assessmentLayerThree = random.NextDouble();

            var sectionResult = new GrassCoverErosionInwardsFailureMechanismSectionResult(
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
            var row = new GrassCoverErosionInwardsFailureMechanismSectionResultRow(sectionResult, failureMechanism, assessmentSection);

            // Call
            row.AssessmentLayerThree = assessmentLayerThree;

            // Assert
            Assert.AreEqual(assessmentLayerThree, sectionResult.AssessmentLayerThree);
            mocks.VerifyAll();
        }
    }
}