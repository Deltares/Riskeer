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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Views;
using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingFailureMechanismSectionResultRow(null, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sectionResult", paramName);
        }

        [Test]
        public void Constructor_WithSectionResult_PropertiesFromSectionAndResult()
        {
            // Setup
            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            var row = new PipingFailureMechanismSectionResultRow(result, null);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);

            Assert.IsTrue(TypeUtils.HasTypeConverter<PipingFailureMechanismSectionResultRow,
                              FailureMechanismSectionResultNoProbabilityValueDoubleConverter>(
                                  r => r.AssessmentLayerTwoA));
            Assert.IsTrue(TypeUtils.HasTypeConverter<PipingFailureMechanismSectionResultRow,
                              NoValueRoundedDoubleConverter>(
                                  r => r.AssessmentLayerThree));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            result.Attach(observer);

            var row = new PipingFailureMechanismSectionResultRow(result, null);

            // Call
            row.AssessmentLayerOne = newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerOne);

            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_NoScenarios_ReturnNaN()
        {
            // Setup
            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            var row = new PipingFailureMechanismSectionResultRow(result, Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            Assert.IsNaN(row.AssessmentLayerTwoA);
        }

        [Test]
        [TestCase(0.2, 0.8 - 1e5)]
        [TestCase(0.0, 0.5)]
        [TestCase(0.3, 0.7 + 1e-5)]
        [TestCase(-5, -8)]
        [TestCase(13, 2)]
        public void AssessmentLayerTwoA_RelevantScenarioContributionDontAddUpTo1_ReturnNaN(double contributionA, double contributionB)
        {
            // Setup
            var section = CreateSection();

            var scenarioA = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);
            var scenarioB = PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section);
            scenarioA.Contribution = (RoundedDouble) contributionA;
            scenarioB.Contribution = (RoundedDouble) contributionB;

            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, new[]
            {
                scenarioA,
                scenarioB
            });

            // Call
            var assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        [TestCase(CalculationScenarioStatus.NotCalculated)]
        [TestCase(CalculationScenarioStatus.Failed)]
        public void AssessmentLayerTwoA_NoRelevantScenariosDone_ReturnNaN(CalculationScenarioStatus status)
        {
            // Setup
            var section = CreateSection();

            var scenario = status.Equals(CalculationScenarioStatus.NotCalculated)
                               ? PipingCalculationScenarioFactory.CreateNotCalculatedPipingCalculationScenario(section)
                               : PipingCalculationScenarioFactory.CreateFailedPipingCalculationScenario(section);

            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, new[]
            {
                scenario
            });

            // Call
            var assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            Assert.IsNaN(assessmentLayerTwoA);
        }

        [Test]
        public void AssessmentLayerTwoA_RelevantScenariosDone_ResultOfSection()
        {
            // Setup            
            var section = CreateSection();
            var scenario = PipingCalculationScenarioFactory.CreatePipingCalculationScenario(0.2, section);
            scenario.Contribution = (RoundedDouble) 1.0;

            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, new[]
            {
                scenario
            });

            // Call
            var assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            var expected = result.GetAssessmentLayerTwoA(new[]
            {
                scenario
            });
            Assert.AreEqual(expected, assessmentLayerTwoA, 1e-6);
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result, null);

            // Call
            row.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 0)
            });
        }
    }
}