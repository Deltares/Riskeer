// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Rhino.Mocks;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilProfileTest
    {
        [TestCase]
        public void Constructor_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate test = () => new StochasticSoilProfile(random.NextDouble(), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilProfile", paramName);
        }

        [TestCase]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();

            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<ISoilProfile>();
            mockRepository.ReplayAll();

            // Call
            var profile = new StochasticSoilProfile(probability, soilProfile);

            // Assert
            Assert.AreEqual(probability, profile.Probability);
            Assert.AreSame(soilProfile, profile.SoilProfile);

            mockRepository.VerifyAll();
        }
    }
}