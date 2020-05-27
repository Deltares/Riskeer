// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationScenarioTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var scenario = new GrassCoverErosionInwardsCalculationScenario();

            // Assert
            Assert.IsInstanceOf<GrassCoverErosionInwardsCalculation>(scenario);
            Assert.IsInstanceOf<ICalculationScenario>(scenario);

            Assert.IsTrue(scenario.IsRelevant);
            Assert.AreEqual(4, scenario.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, scenario.Contribution, scenario.Contribution.GetAccuracy());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsRelevant_Always_ReturnsSetValue(bool isRelevant)
        {
            // Setup
            var scenario = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            scenario.IsRelevant = isRelevant;

            // Assert
            Assert.AreEqual(isRelevant, scenario.IsRelevant);
        }

        [Test]
        public void Contribution_Always_ReturnsSetValue()
        {
            // Setup
            var random = new Random(21);
            RoundedDouble contribution = random.NextRoundedDouble();

            var scenario = new GrassCoverErosionInwardsCalculationScenario();

            // Call
            scenario.Contribution = contribution;

            // Assert
            Assert.AreEqual(4, scenario.Contribution.NumberOfDecimalPlaces);
            Assert.AreEqual(contribution, scenario.Contribution, scenario.Contribution.GetAccuracy());
        }
    }
}