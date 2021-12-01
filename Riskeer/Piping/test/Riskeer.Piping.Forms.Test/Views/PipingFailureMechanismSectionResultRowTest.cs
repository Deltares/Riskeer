// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.Views;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.TestUtil.SemiProbabilistic;
using Riskeer.Piping.Forms.Views;

namespace Riskeer.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            void Call() => new PipingFailureMechanismSectionResultRow(
                result, null, new PipingFailureMechanism(), assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
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
            void Call() => new PipingFailureMechanismSectionResultRow(
                result, Enumerable.Empty<IPipingCalculationScenario<PipingInput>>(),
                null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
            void Call() => new PipingFailureMechanismSectionResultRow(
                result, Enumerable.Empty<SemiProbabilisticPipingCalculationScenario>(),
                new PipingFailureMechanism(), null);

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

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            SemiProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section)
            };

            // Call
            var row = new PipingFailureMechanismSectionResultRow(result, calculationScenarios, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismSectionResultRow<PipingFailureMechanismSectionResult>>(row);
            Assert.AreEqual(result.IsRelevant, row.IsRelevant);
            Assert.AreEqual(result.InitialFailureMechanismResult, row.InitialFailureMechanismResult);
            Assert.AreEqual(result.ManualInitialFailureMechanismResultProfileProbability, row.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(result.ManualInitialFailureMechanismResultSectionProbability, row.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(result.FurtherAnalysisNeeded, row.FurtherAnalysisNeeded);
            Assert.AreEqual(result.ProbabilityRefinementType, row.ProbabilityRefinementType);
            Assert.AreEqual(result.RefinedProfileProbability, row.RefinedProfileProbability);
            Assert.AreEqual(result.RefinedSectionProbability, row.RefinedSectionProbability);

            TestHelper.AssertTypeConverter<PipingFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingFailureMechanismSectionResultRow.ManualInitialFailureMechanismResultProfileProbability));
            TestHelper.AssertTypeConverter<PipingFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingFailureMechanismSectionResultRow.ManualInitialFailureMechanismResultSectionProbability));
            TestHelper.AssertTypeConverter<PipingFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingFailureMechanismSectionResultRow.RefinedProfileProbability));
            TestHelper.AssertTypeConverter<PipingFailureMechanismSectionResultRow, NoProbabilityValueDoubleConverter>(
                nameof(PipingFailureMechanismSectionResultRow.RefinedSectionProbability));
            mocks.VerifyAll();
        }

        [Test]
        public void IsRelevant_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const bool newValue = false;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.IsRelevant = newValue,
                result => result.IsRelevant,
                newValue);
        }

        [Test]
        public void InitialFailureMechanismResult_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const InitialFailureMechanismResultType newValue = InitialFailureMechanismResultType.NoFailureProbability;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.InitialFailureMechanismResult = newValue,
                result => result.InitialFailureMechanismResult,
                newValue);
        }

        [Test]
        [TestCaseSource(nameof(GetValidProbabilities))]
        public void ManualInitialFailureMechanismResultProfileProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.ManualInitialFailureMechanismResultProfileProbability = newValue,
                result => result.ManualInitialFailureMechanismResultProfileProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetInvalidProbabilities))]
        public void ManualInitialFailureMechanismResultProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.ManualInitialFailureMechanismResultProfileProbability = value);
        }

        [Test]
        [TestCaseSource(nameof(GetValidProbabilities))]
        public void ManualInitialFailureMechanismResultSectionProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.ManualInitialFailureMechanismResultSectionProbability = newValue,
                result => result.ManualInitialFailureMechanismResultSectionProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetInvalidProbabilities))]
        public void ManualInitialFailureMechanismResultSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.ManualInitialFailureMechanismResultSectionProbability = value);
        }

        [Test]
        public void FurtherAnalysisNeeded_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const bool newValue = true;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.FurtherAnalysisNeeded = newValue,
                result => result.FurtherAnalysisNeeded,
                newValue);
        }

        [Test]
        public void ProbabilityRefinementType_SetNewValue_NotifyObserversAndPropertyChanged()
        {
            const ProbabilityRefinementType newValue = ProbabilityRefinementType.Both;
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.ProbabilityRefinementType = newValue,
                result => result.ProbabilityRefinementType,
                newValue);
        }

        [Test]
        [TestCaseSource(nameof(GetValidProbabilities))]
        public void RefinedProfileProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.RefinedProfileProbability = newValue,
                result => result.RefinedProfileProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetInvalidProbabilities))]
        public void RefinedProfileProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.RefinedProfileProbability = value);
        }

        [Test]
        [TestCaseSource(nameof(GetValidProbabilities))]
        public void RefinedSectionProbability_SetNewValue_NotifyObserversAndPropertyChanged(double newValue)
        {
            Property_SetNewValue_NotifyObserversAndPropertyChanged(
                row => row.RefinedSectionProbability = newValue,
                result => result.RefinedSectionProbability,
                newValue);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetInvalidProbabilities))]
        public void RefinedSectionProbability_InvalidValue_ThrowsArgumentOutOfRangeException(double value)
        {
            ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(row => row.ManualInitialFailureMechanismResultSectionProbability = value);
        }

        private static void Property_SetNewValue_NotifyObserversAndPropertyChanged<T>(
            Action<PipingFailureMechanismSectionResultRow> setPropertyAction,
            Func<PipingFailureMechanismSectionResult, T> assertPropertyFunc,
            T newValue)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);
            result.Attach(observer);

            SemiProbabilisticPipingCalculationScenario[] calculationScenarios =
            {
                SemiProbabilisticPipingCalculationTestFactory.CreateCalculation<SemiProbabilisticPipingCalculationScenario>(section)
            };

            var row = new PipingFailureMechanismSectionResultRow(result, calculationScenarios, failureMechanism, assessmentSection);

            // Call
            setPropertyAction(row);

            // Assert
            Assert.AreEqual(newValue, assertPropertyFunc(result));
            mocks.VerifyAll();
        }

        private static void ProbabilityProperty_SetInvalidValue_ThrowsArgumentOutOfRangeException(
            Action<PipingFailureMechanismSectionResultRow> setPropertyAction)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(failureMechanism, mocks);
            mocks.ReplayAll();

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var result = new PipingFailureMechanismSectionResult(section);

            var row = new PipingFailureMechanismSectionResultRow(result, Enumerable.Empty<IPipingCalculationScenario<PipingInput>>(),
                                                                 failureMechanism, assessmentSection);

            // Call
            void Call() => setPropertyAction(row);

            // Assert
            const string expectedMessage = "De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
            mocks.VerifyAll();
        }
        
        private static IEnumerable<TestCaseData> GetValidProbabilities()
        {
            yield return new TestCaseData(0);
            yield return new TestCaseData(1);
            yield return new TestCaseData(0.5);
            yield return new TestCaseData(1e-6);
            yield return new TestCaseData(double.NaN);
        }

        private static IEnumerable<TestCaseData> GetInvalidProbabilities()
        {
            yield return new TestCaseData(-20);
            yield return new TestCaseData(-1e-6);
            yield return new TestCaseData(1 + 1e-6);
            yield return new TestCaseData(12);
        }
    }
}