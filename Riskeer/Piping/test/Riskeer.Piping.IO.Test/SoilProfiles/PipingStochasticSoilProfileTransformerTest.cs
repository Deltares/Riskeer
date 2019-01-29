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
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.TestUtil;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.IO.SoilProfiles;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Primitives.TestUtil;

namespace Riskeer.Piping.IO.Test.SoilProfiles
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

            StochasticSoilProfile stochasticSoilProfile = StochasticSoilProfileTestFactory.CreateStochasticSoilProfileWithValidProbability(soilProfile);

            // Call
            TestDelegate test = () => PipingStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Transform_InvalidStochasticSoilProfile_ThrowsImportedDataTransformException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            PipingSoilProfile pipingSoilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            var stochasticSoilProfile = new StochasticSoilProfile(double.NaN, soilProfile);

            // Call
            TestDelegate call = () => PipingStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, pipingSoilProfile);

            // Assert
            var exception = Assert.Throws<ImportedDataTransformException>(call);

            Exception innerException = exception.InnerException;
            Assert.IsInstanceOf<ArgumentException>(innerException);
            Assert.AreEqual(innerException.Message, exception.Message);
        }

        [Test]
        public void Transform_ValidStochasticSoilProfile_ReturnsExpectedPipingStochasticSoilProfile()
        {
            // Setup
            var random = new Random(21);

            var mockRepository = new MockRepository();
            var soilProfile = mockRepository.Stub<ISoilProfile>();
            mockRepository.ReplayAll();

            PipingSoilProfile pipingSoilProfile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            var stochasticSoilProfile = new StochasticSoilProfile(random.NextDouble(), soilProfile);

            // Call
            PipingStochasticSoilProfile pipingStochasticSoilProfile = PipingStochasticSoilProfileTransformer.Transform(stochasticSoilProfile, pipingSoilProfile);

            // Assert
            Assert.AreEqual(stochasticSoilProfile.Probability, pipingStochasticSoilProfile.Probability);
            Assert.AreSame(pipingSoilProfile, pipingStochasticSoilProfile.SoilProfile);
            mockRepository.VerifyAll();
        }
    }
}