﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            PipingFailureMechanismSectionResult sectionResult = new PipingFailureMechanismSectionResult(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsFalse(sectionResult.AssessmentLayerOne);
            Assert.AreEqual((RoundedDouble) 0, sectionResult.AssessmentLayerTwoA);
            Assert.AreEqual((RoundedDouble) 0, sectionResult.AssessmentLayerThree);
            CollectionAssert.IsEmpty(sectionResult.CalculationScenarios);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingFailureMechanismSectionResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_Always_ReturnsSetValue(bool newValue)
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerOne);
        }

        [Test]
        public void AssessmentLayerTwoA_MultipleScenarios_ReturnsValueBasedOnRelevantAndDoneScenarios()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock1 = mocks.StrictMock<ICalculationScenario>();
            var calculationScenarioMock2 = mocks.StrictMock<ICalculationScenario>();
            var calculationScenarioMock3 = mocks.StrictMock<ICalculationScenario>();
            var calculationScenarioMock4 = mocks.StrictMock<ICalculationScenario>();

            var contribution1 = 0.2;
            var contribution2 = 0.8;
            var probability1 = (RoundedDouble) 1000000;
            var probability2 = (RoundedDouble) 2000000;

            calculationScenarioMock1.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock1.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Done);
            calculationScenarioMock1.Stub(cs => cs.Contribution).Return((RoundedDouble)contribution1);
            calculationScenarioMock1.Stub(cs => cs.Probability).Return(probability1);

            calculationScenarioMock2.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock2.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Done);
            calculationScenarioMock2.Stub(cs => cs.Contribution).Return((RoundedDouble)contribution2);
            calculationScenarioMock2.Stub(cs => cs.Probability).Return(probability2);

            calculationScenarioMock3.Stub(cs => cs.IsRelevant).Return(false);

            calculationScenarioMock4.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock4.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Failed);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock1);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock2);

            // Call
            RoundedDouble? assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(1.0 / ((1.0 / probability1) * contribution1 + (1.0 / probability2) * contribution2), assessmentLayerTwoA, 1e-8);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_ScenarioNotCalculated_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Status).Return(CalculationScenarioStatus.NotCalculated);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            RoundedDouble assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual((RoundedDouble) 0, assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_ScenarioInvalidOutput_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Probability).Return((RoundedDouble) double.NaN);
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Failed);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            RoundedDouble assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual((RoundedDouble) 0, assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_NoScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            RoundedDouble? assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual((RoundedDouble) 0.0, assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_NoRelevantScenarios_ReturnsZero()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(false);
            var calculationScenarioMock2 = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock2.Stub(cs => cs.IsRelevant).Return(false);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock2);

            // Call
            RoundedDouble assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual((RoundedDouble) 0.0, assessmentLayerTwoA);
        }

        [Test]
        public void TotalContribution_Always_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 0.3);

            var calculationScenarioMock2 = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock2.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock2.Stub(cs => cs.Contribution).Return((RoundedDouble) 0.5);

            var calculationScenarioMock3 = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock3.Stub(cs => cs.IsRelevant).Return(false);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock2);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock3);

            // Call
            RoundedDouble totalContribution = failureMechanismSectionResult.TotalContribution;

            // Assert
            Assert.AreEqual((RoundedDouble) 0.8, totalContribution);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_Always_ReturnsSetValue()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);
            var assessmentLayerThreeValue = (RoundedDouble) 3.0;

            // Call
            failureMechanismSectionResult.AssessmentLayerThree = assessmentLayerThreeValue;

            // Assert
            Assert.AreEqual(assessmentLayerThreeValue, failureMechanismSectionResult.AssessmentLayerThree);
        }

        [Test]
        public void CalculationScenarios_Always_ReturnsAddedCalculationScenarios()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Precondition
            CollectionAssert.IsEmpty(failureMechanismSectionResult.CalculationScenarios);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            var calculationScenarioMock2 = mocks.StrictMock<ICalculationScenario>();
            var calculationScenarioMock3 = mocks.StrictMock<ICalculationScenario>();

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock2);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock3);

            // Call
            List<ICalculationScenario> scenarios = failureMechanismSectionResult.CalculationScenarios;

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                calculationScenarioMock,
                calculationScenarioMock2,
                calculationScenarioMock3
            }, scenarios);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioNotCalculated_ReturnsStatusNotCalculated()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Status).Return(CalculationScenarioStatus.NotCalculated);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.CalculationScenarioStatus;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioInvalidOutput_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Probability).Return((RoundedDouble) double.NaN);
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Failed);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.CalculationScenarioStatus;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioInvalidOutputAndNotCalculated_ReturnsStatusFailed()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Status).Return(CalculationScenarioStatus.NotCalculated);

            var calculationScenarioMock2 = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock2.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock2.Stub(cs => cs.Probability).Return((RoundedDouble) double.NaN);
            calculationScenarioMock2.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            calculationScenarioMock2.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Failed);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock2);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.CalculationScenarioStatus;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationScenarioStatus_ScenariosCalculated_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Stub(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Stub(cs => cs.Contribution).Return((RoundedDouble) 1.0);
            var expectedProbability = (RoundedDouble) 41661830;
            calculationScenarioMock.Stub(cs => cs.Probability).Return(expectedProbability);
            calculationScenarioMock.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Done);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.CalculationScenarioStatus;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
            mocks.VerifyAll();
        }

        [Test]
        public void CalculationScenarioStatus_NoScenarios_ReturnsStatusDone()
        {
            // Setup
            FailureMechanismSection section = CreateSection();
            var failureMechanismSectionResult = new PipingFailureMechanismSectionResult(section);

            // Call
            CalculationScenarioStatus status = failureMechanismSectionResult.CalculationScenarioStatus;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("test", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });
        }
    }
}