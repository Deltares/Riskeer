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

using NUnit.Framework;
using Ringtoets.Piping.IO.SoilProfile;

namespace Ringtoets.Piping.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilProfileProbabilityTest
    {
        [Test]
        [TestCase(1.0, null, null)]
        [TestCase(2.0, 1234L, null)]
        [TestCase(3.0, 1235L, 5678L)]
        [TestCase(4.0, null, 5679L)]
        public void Constructor_Always_ExpectedValues(double probability, long? soilProfile1DId, long? soilProfile2DId)
        {
            // Call
            StochasticSoilProfileProbability stochasticSoilProfileProbability = new StochasticSoilProfileProbability(probability, soilProfile1DId, soilProfile2DId);

            // Assert
            Assert.IsInstanceOf<StochasticSoilProfileProbability>(stochasticSoilProfileProbability);
            Assert.AreEqual(probability, stochasticSoilProfileProbability.Probability);
            Assert.AreEqual(soilProfile1DId, stochasticSoilProfileProbability.SoilProfile1DId);
            Assert.AreEqual(soilProfile2DId, stochasticSoilProfileProbability.SoilProfile2DId);
        }
    }
}