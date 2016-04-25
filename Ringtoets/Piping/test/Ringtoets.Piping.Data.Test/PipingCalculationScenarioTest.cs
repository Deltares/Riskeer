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

using Core.Common.Base.Data;
using NUnit.Framework;

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
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            // Call
            var scenario = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInputParameters);

            // Assert
            Assert.IsInstanceOf<PipingCalculation>(scenario);
            Assert.AreSame(semiProbabilisticInputParameters, scenario.SemiProbabilisticParameters);
            Assert.IsTrue(scenario.IsRelevant);
            Assert.AreEqual((RoundedDouble) 1.0, scenario.Contribution);
            Assert.IsNull(scenario.Probability);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsRelevant_Always_ReturnsSetValue(bool isRelevant)
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            var scenario = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInputParameters);

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
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            var scenario = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInputParameters);

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
            RoundedDouble expectedProbability = new RoundedDouble(0, 49862180);

            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            var scenario =  new PipingCalculationScenario(generalInputParameters, semiProbabilisticInputParameters);
            scenario.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, expectedProbability, 0, 0);
            
            // Call
            var probability = scenario.Probability;

            // Assert
            Assert.AreEqual(expectedProbability, probability);
        }

        [Test]
        public void Probability_PipingOutputNull_ReturnsNull()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            var scenario = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInputParameters);

            // Call
            var propability = scenario.Probability;

            // Assert
            Assert.IsNull(propability);
        }

        [Test]
        public void Probabilty_ScenarioInvalid_ReturnsNaN()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();
            var semiProbabilisticInputParameters = new SemiProbabilisticPipingInput();

            var scenario = new PipingCalculationScenario(generalInputParameters, semiProbabilisticInputParameters);
            scenario.SemiProbabilisticOutput = new PipingSemiProbabilisticOutput(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, double.NaN, 0, 0);

            // Call
            var propability = scenario.Probability;

            // Assert
            Assert.IsNaN(propability.Value);
        }
    }
}