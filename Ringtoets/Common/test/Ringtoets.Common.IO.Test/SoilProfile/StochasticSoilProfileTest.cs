// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilProfileTest
    {
        [Test]
        [SetCulture("nl-NL")]
        [TestCase(12.5)]
        [TestCase(1 + 1e-6)]
        [TestCase(0 - 1e-6)]
        [TestCase(-28.5)]
        [TestCase(double.NaN)]
        public void Constructor_WithInvalidProbabilities_ThrowsArgumentOutOfRangeException(double probability)
        {
            // Setup
            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<ISoilProfile>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => new StochasticSoilProfile(probability, soilProfile);

            // Assert
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);

            mockRepository.VerifyAll();
        }

        [TestCase]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<ISoilProfile>();
            mockRepository.ReplayAll();

            var random = new Random(9);
            double probability = random.NextDouble();

            // Call
            var profile = new StochasticSoilProfile(probability, soilProfile);

            // Assert
            Assert.AreEqual(probability, profile.Probability);
            Assert.AreSame(soilProfile, profile.SoilProfile);

            mockRepository.VerifyAll();
        }
    }
}