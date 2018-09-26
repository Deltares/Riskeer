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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class StochasticSoilProfileHelperTest
    {
        [Test]
        public void GetUniqueStochasticSoilProfiles_StochasticSoilProfilesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StochasticSoilProfileHelper.GetUniqueStochasticSoilProfiles(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("stochasticSoilProfiles", exception.ParamName);
        }

        [Test]
        public void GetUniqueStochasticSoilProfiles_SoilModelNameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                StochasticSoilProfileHelper.GetUniqueStochasticSoilProfiles(Enumerable.Empty<StochasticSoilProfile>(),
                                                                            null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilModelName", exception.ParamName);
        }

        [Test]
        public void GetUniqueStochasticSoilProfiles_DistinctSoilProfiles_ReturnsExpectedSoilProfiles()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfileOne = mocks.Stub<ISoilProfile>();
            var soilProfileTwo = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            var profileOne = new StochasticSoilProfile(0.5, soilProfileOne);
            var profileTwo = new StochasticSoilProfile(0.8, soilProfileTwo);
            StochasticSoilProfile[] stochasticSoilProfiles =
            {
                profileOne,
                profileTwo
            };

            // Call
            IEnumerable<StochasticSoilProfile> profilesToTransform =
                StochasticSoilProfileHelper.GetUniqueStochasticSoilProfiles(stochasticSoilProfiles,
                                                                            string.Empty);

            // Assert
            CollectionAssert.AreEqual(stochasticSoilProfiles, profilesToTransform);
            mocks.VerifyAll();
        }

        [Test]
        public void GetUniqueStochasticSoilProfiles_SameSoilProfile_ReturnsExpectedSoilProfiles()
        {
            // Setup
            const string soilProfileName = "A profile name";
            const string soilModelName = "A model name";

            var mocks = new MockRepository();
            var profile = mocks.Stub<ISoilProfile>();
            profile.Stub(p => p.Name).Return(soilProfileName);
            mocks.ReplayAll();

            const double probabilityOne = 0.5;
            const double probabilityTwo = 0.1;
            var profileOne = new StochasticSoilProfile(probabilityOne, profile);
            var profileTwo = new StochasticSoilProfile(probabilityTwo, profile);
            StochasticSoilProfile[] stochasticSoilProfiles =
            {
                profileOne,
                profileTwo
            };

            IEnumerable<StochasticSoilProfile> profilesToTransform = null;

            // Call
            Action call = () => profilesToTransform =
                                    StochasticSoilProfileHelper.GetUniqueStochasticSoilProfiles(stochasticSoilProfiles,
                                                                                                soilModelName);

            // Assert
            string expectedMessage = $"Ondergrondschematisatie '{soilProfileName}' is meerdere keren gevonden in ondergrondmodel '{soilModelName}'. " +
                                     "Kansen van voorkomen worden opgeteld.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn));

            Assert.IsNotNull(profilesToTransform);

            StochasticSoilProfile profileToTransform = profilesToTransform.Single();
            Assert.AreSame(profile, profileToTransform.SoilProfile);
            const double expectedProbability = probabilityOne + probabilityTwo;
            Assert.AreEqual(expectedProbability, profileToTransform.Probability, 1e-6);

            mocks.VerifyAll();
        }
    }
}