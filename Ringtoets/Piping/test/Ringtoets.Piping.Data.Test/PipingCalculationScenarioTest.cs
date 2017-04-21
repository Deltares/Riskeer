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
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationScenarioTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();

            // Call
            var scenario = new PipingCalculationScenario(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<PipingCalculation>(scenario);
            Assert.IsInstanceOf<ICalculationScenario>(scenario);
            Assert.IsTrue(scenario.IsRelevant);
            Assert.AreEqual((RoundedDouble) 1.0, scenario.Contribution);
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, scenario.Status);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsRelevant_Always_ReturnsSetValue(bool isRelevant)
        {
            // Setup
            var scenario = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            scenario.IsRelevant = isRelevant;

            // Assert
            Assert.AreEqual(isRelevant, scenario.IsRelevant);
        }

        [Test]
        [TestCase(1)]
        [TestCase(15.0)]
        public void Contribution_Always_ReturnsSetValue(double newValue)
        {
            // Setup
            var scenario = new PipingCalculationScenario(new GeneralPipingInput());
            var roundedDouble = (RoundedDouble) newValue;

            // Call
            scenario.Contribution = roundedDouble;

            // Assert
            Assert.AreEqual(roundedDouble, scenario.Contribution);
        }

        [Test]
        public void Probability_PipingOutputSet_ReturnsPipingOutputProbability()
        {
            // Setup
            double expectedProbability = 1.0 / 49862180;

            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput(expectedProbability)
            };

            // Call
            double probability = scenario.Probability;

            // Assert
            Assert.AreEqual(expectedProbability, probability);
        }

        [Test]
        public void Probability_ScenarioStatusNotDone_ThrowsInvalidOperationException()
        {
            // Setup
            var scenario = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            double probability;
            TestDelegate call = () => probability = scenario.Probability;

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void CalculationScenarioStatus_OutputNull_ReturnsStatusNotCalculated()
        {
            // Setup
            var scenario = new PipingCalculationScenario(new GeneralPipingInput());

            // Call
            CalculationScenarioStatus status = scenario.Status;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.NotCalculated, status);
        }

        [Test]
        public void CalculationScenarioStatus_SemiProbabilisticOutputNull_ReturnsStatusFailed()
        {
            // Setup
            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = null
            };

            // Call
            CalculationScenarioStatus status = scenario.Status;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void CalculationScenarioStatus_ScenarioInvalid_ReturnsStatusFailed()
        {
            // Setup
            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput(double.NaN)
            };

            // Call
            CalculationScenarioStatus status = scenario.Status;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Failed, status);
        }

        [Test]
        public void CalculationScenarioStatus_PipingOutputSet_ReturnsStatusDone()
        {
            // Setup
            double expectedProbability = 1.0 / 49862180;

            var scenario = new PipingCalculationScenario(new GeneralPipingInput())
            {
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput(expectedProbability)
            };

            // Call
            CalculationScenarioStatus status = scenario.Status;

            // Assert
            Assert.AreEqual(CalculationScenarioStatus.Done, status);
        }
    }
}