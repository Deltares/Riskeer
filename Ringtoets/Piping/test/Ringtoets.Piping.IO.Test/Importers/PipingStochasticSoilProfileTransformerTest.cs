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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class PipingStochasticSoilProfileTransformerTest
    {
        [Test]
        public void Transform_StochasticSoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            PipingSoilProfile soilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            // Call
            TestDelegate test = () => PipingStochasticSoilProfileTransformer.Transform(null, soilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("stochasticSoilProfile", exception.ParamName);
        }

        [Test]
        public void Transform_PipingSoilProfileNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<ISoilProfile>();
            mockRepository.ReplayAll();

            var stochasticSoilProfile = new StochasticSoilProfile(0, soilProfile);

            // Call
            TestDelegate test = () => PipingStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Transform_ValidStochasticSoilProfile_ReturnsExpectedPipingStochasticSoilProfile()
        {
            // Setup
            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<ISoilProfile>();
            mockRepository.ReplayAll();

            PipingSoilProfile pipingSoilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            var stochasticSoilProfile = new StochasticSoilProfile(new Random(9).NextDouble(), soilProfile);

            // Call
            PipingStochasticSoilProfile pipingStochasticSoilProfile = PipingStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, pipingSoilProfile);

            // Assert
            Assert.AreEqual(stochasticSoilProfile.Probability, pipingStochasticSoilProfile.Probability);
            Assert.AreSame(pipingSoilProfile, pipingStochasticSoilProfile.SoilProfile);
            mockRepository.VerifyAll();
        }
    }
}