// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.FailureMechanism
{
    [TestFixture]
    public class FailureMechanismSectionResultTest
    {
        [Test]
        public void Constructor_DefaultValues()
        {
            // Setup
            var section = CreateSection();

            // Call
            var sectionResult = new FailureMechanismSectionResult(section);

            // Assert
            Assert.AreSame(section, sectionResult.Section);
            Assert.IsFalse(sectionResult.AssessmentLayerOne);
            Assert.AreEqual((RoundedDouble)0, sectionResult.AssessmentLayerTwoA);
            Assert.AreEqual((RoundedDouble)0, sectionResult.AssessmentLayerTwoB);
            Assert.AreEqual((RoundedDouble)0, sectionResult.AssessmentLayerThree);
            CollectionAssert.IsEmpty(sectionResult.CalculationScenarios);
        }

        [Test]
        public void Constructor_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionResult(null);

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
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            // Call
            failureMechanismSectionResult.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, failureMechanismSectionResult.AssessmentLayerOne);
        }

        [Test]
        public void AssessmentLayerTwoA_ScenariosCalculated_ReturnsValue()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Expect(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Expect(cs => cs.Contribution).Return((RoundedDouble)1.0);
            var expectedProbability = (RoundedDouble)41661830;
            calculationScenarioMock.Expect(cs => cs.Probability).Return(expectedProbability).Repeat.Twice();

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            var assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(expectedProbability, assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_ScenarioNotCalculated_ReturnsNull()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Expect(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Expect(cs => cs.Probability).Return(null);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            var assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.IsNull(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_ScenarioInvalidOutput_ReturnsNaN()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Expect(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Expect(cs => cs.Probability).Return((RoundedDouble?) double.NaN).Repeat.Twice();
            calculationScenarioMock.Expect(cs => cs.Contribution).Return((RoundedDouble) 1.0);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);

            // Call
            var assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_NoScenarios_ReturnsValue()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

            // Call
            var assessmentLayerTwoA = failureMechanismSectionResult.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual((RoundedDouble)0.0, assessmentLayerTwoA);
        }

        [Test]
        public void TotalContribution_Always_ReturnsTotalRelevantScenarioContribution()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);
            
            var mocks = new MockRepository();
            var calculationScenarioMock = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock.Expect(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock.Expect(cs => cs.Contribution).Return((RoundedDouble)0.3);
            
            var calculationScenarioMock2 = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock2.Expect(cs => cs.IsRelevant).Return(true);
            calculationScenarioMock2.Expect(cs => cs.Contribution).Return((RoundedDouble)0.5);
            
            var calculationScenarioMock3 = mocks.StrictMock<ICalculationScenario>();
            calculationScenarioMock3.Expect(cs => cs.IsRelevant).Return(false);

            mocks.ReplayAll();

            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock2);
            failureMechanismSectionResult.CalculationScenarios.Add(calculationScenarioMock3);

            // Call
            var totalContribution = failureMechanismSectionResult.TotalContribution;

            // Assert
            Assert.AreEqual((RoundedDouble)0.8, totalContribution);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoB_Always_ReturnsSetValue()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);
            var assessmentLayerTwoBValue = (RoundedDouble) 3.0;

            // Call
            failureMechanismSectionResult.AssessmentLayerTwoB = assessmentLayerTwoBValue;

            // Assert
            Assert.AreEqual(assessmentLayerTwoBValue, failureMechanismSectionResult.AssessmentLayerTwoB);
        }

        [Test]
        public void AssessmentLayerThree_Always_ReturnsSetValue()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);
            var assessmentLayerThreeValue = (RoundedDouble)3.0;

            // Call
            failureMechanismSectionResult.AssessmentLayerThree = assessmentLayerThreeValue;

            // Assert
            Assert.AreEqual(assessmentLayerThreeValue, failureMechanismSectionResult.AssessmentLayerThree);
        }

        [Test]
        public void CalculationScenarios_Always_ReturnsAddedCalculationScenarios()
        {
            // Setup
            var section = CreateSection();
            var failureMechanismSectionResult = new FailureMechanismSectionResult(section);

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
            var scenarios = failureMechanismSectionResult.CalculationScenarios;

            // Assert
            CollectionAssert.AreEqual(new[] {calculationScenarioMock, calculationScenarioMock2, calculationScenarioMock3}, scenarios);
            mocks.VerifyAll();
        }

        private static FailureMechanismSection CreateSection()
        {
            var points = new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            };

            var section = new FailureMechanismSection("test", points);
            return section;
        }
    }
}