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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;

using CommonBaseResources = Core.Common.Base.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.Views
{
    public class PipingFailureMechanismSectionResultRowTest
    {
        [Test]
        public void Constructor_WithoutSectionResult_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingFailureMechanismSectionResultRow(null);

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
            var row = new PipingFailureMechanismSectionResultRow(result);

            // Assert
            Assert.AreEqual(section.Name, row.Name);
            Assert.AreEqual(result.AssessmentLayerOne, row.AssessmentLayerOne);
            Assert.AreEqual(result.AssessmentLayerThree, row.AssessmentLayerThree);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AssessmentLayerOne_AlwaysOnChange_NotifyObserversOfResultAndResultPropertyChanged(bool newValue)
        {
            // Setup
            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result);

            int counter = 0;
            using (new Observer(() => counter++)
            {
                Observable = result
            })
            {
                // Call
                row.AssessmentLayerOne = newValue;

                // Assert
                Assert.AreEqual(1, counter);
                Assert.AreEqual(newValue, result.AssessmentLayerOne);
            }
        }

        [Test]
        public void AssessmentLayerTwoA_NoScenarios_ShowDash()
        {
            // Setup
            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);

            // Call
            var row = new PipingFailureMechanismSectionResultRow(result);

            // Assert
            Assert.AreEqual(Resources.FailureMechanismSectionResultRow_AssessmentLayerTwoA_No_result_dash, row.AssessmentLayerTwoA);
        }

        [Test]
        [TestCase(0.2,0.8-1e5)]
        [TestCase(0.0,0.5)]
        [TestCase(0.3,0.7+1e-5)]
        [TestCase(-5,-8)]
        [TestCase(13,2)]
        public void AssessmentLayerTwoA_RelevantScenarioContributionDontAddUpTo1_NaN(double contributionA, double contributionB)
        {
            // Setup
            var mocks = new MockRepository();
            var scenarioA = mocks.Stub<ICalculationScenario>();
            var scenarioB = mocks.Stub<ICalculationScenario>();
            scenarioA.Contribution = (RoundedDouble)contributionA;
            scenarioA.IsRelevant = true;
            scenarioA.Stub(s => s.Status).Return(CalculationScenarioStatus.Done);
            scenarioB.Contribution = (RoundedDouble)contributionB;
            scenarioB.IsRelevant = true;
            scenarioB.Stub(s => s.Status).Return(CalculationScenarioStatus.Done);
            mocks.ReplayAll();

            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result);
            result.CalculationScenarios.Add(scenarioA);
            result.CalculationScenarios.Add(scenarioB);

            // Call
            var assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(string.Format("{0}", double.NaN), assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(CalculationScenarioStatus.NotCalculated)]
        [TestCase(CalculationScenarioStatus.Failed)]
        public void AssessmentLayerTwoA_NoRelevantScenariosDone_Dash(CalculationScenarioStatus status)
        {
            // Setup
            var mocks = new MockRepository();
            var scenario = mocks.Stub<ICalculationScenario>();
            scenario.Stub(cs => cs.Status).Return(status);
            scenario.Contribution = (RoundedDouble)1.0;
            scenario.IsRelevant = true;
            mocks.ReplayAll();

            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result);
            result.CalculationScenarios.Add(scenario);

            // Call
            var assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            Assert.AreEqual(Resources.FailureMechanismSectionResultRow_AssessmentLayerTwoA_No_result_dash, assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerTwoA_RelevantScenariosDone_ResultOfSection()
        {
            // Setup
            var mocks = new MockRepository();
            var scenario = mocks.Stub<ICalculationScenario>();
            scenario.Stub(cs => cs.Status).Return(CalculationScenarioStatus.Done);
            scenario.Stub(cs => cs.Probability).Return((RoundedDouble)0.2);
            scenario.Contribution = (RoundedDouble)1.0;
            scenario.IsRelevant = true;
            mocks.ReplayAll();

            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result);
            result.CalculationScenarios.Add(scenario);
            
            // Call
            var assessmentLayerTwoA = row.AssessmentLayerTwoA;

            // Assert
            var expected = string.Format(CommonBaseResources.ProbabilityPerYearFormat, result.AssessmentLayerTwoA);
            Assert.AreEqual(expected, assessmentLayerTwoA);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentLayerThree_AlwaysOnChange_ResultPropertyChanged()
        {
            // Setup
            var random = new Random(21);
            var newValue = random.NextDouble();
            var section = CreateSection();
            var result = new PipingFailureMechanismSectionResult(section);
            var row = new PipingFailureMechanismSectionResultRow(result);

            // Call
            row.AssessmentLayerThree = (RoundedDouble) newValue;

            // Assert
            Assert.AreEqual(newValue, result.AssessmentLayerThree, row.AssessmentLayerThree.GetAccuracy());
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}