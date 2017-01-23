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
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class StochasticSoilProfileTest
    {
        [Test]
        [TestCase(1.0, SoilProfileType.SoilProfile1D, 123L)]
        [TestCase(2.0, SoilProfileType.SoilProfile2D, 123L)]
        public void Constructor_Always_ExpectedValues(double probability, SoilProfileType soilProfileType, long soilProfileId)
        {
            // Call
            StochasticSoilProfile stochasticSoilProfileProbability = new StochasticSoilProfile(probability, soilProfileType, soilProfileId);

            // Assert
            Assert.IsInstanceOf<StochasticSoilProfile>(stochasticSoilProfileProbability);
            Assert.AreEqual(probability, stochasticSoilProfileProbability.Probability);
            Assert.AreEqual(soilProfileType, stochasticSoilProfileProbability.SoilProfileType);
            Assert.AreEqual(soilProfileId, stochasticSoilProfileProbability.SoilProfileId);
        }

        [Test]
        public void Update_WithNullProfile_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => stochasticProfile.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromProfile", paramName);
        }

        [Test]
        public void Update_WithValidProfile_UpdatesProperties()
        {
            // Setup
            var newProbability = 1.0;
            var newProfile = new TestPipingSoilProfile();
            var otherStochasticProfile = new StochasticSoilProfile(newProbability, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = newProfile
            };

            var stochasticProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            stochasticProfile.Update(otherStochasticProfile);

            // Assert
            Assert.AreEqual(newProbability, stochasticProfile.Probability);
            Assert.AreSame(newProfile, stochasticProfile.SoilProfile);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var stochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(name, 0.0, new[]
                {
                    new PipingSoilLayer(0.0)
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call & Assert
            Assert.AreEqual(name, stochasticSoilProfile.ToString());
        }
    }
}