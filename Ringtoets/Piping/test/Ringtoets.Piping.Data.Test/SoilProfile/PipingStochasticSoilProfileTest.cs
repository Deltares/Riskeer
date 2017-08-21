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
using System.Drawing;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test.SoilProfile
{
    [TestFixture]
    public class PipingStochasticSoilProfileTest
    {
        [Test]
        public void Constructor_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingStochasticSoilProfile(0.0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        [TestCase(0.0)]
        [TestCase(0.5)]
        [TestCase(1.0)]
        public void Constructor_WithValidProbabilities_ExpectedValues(double probability)
        {
            // Setup
            PipingSoilProfile profile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            // Call
            var stochasticSoilProfile = new PipingStochasticSoilProfile(probability, profile);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilProfile);
            Assert.AreEqual(probability, stochasticSoilProfile.Probability);
            Assert.AreSame(profile, stochasticSoilProfile.SoilProfile);
            Assert.AreEqual(profile.ToString(), stochasticSoilProfile.ToString());
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(12.5)]
        [TestCase(1 + 1e-6)]
        [TestCase(0 - 1e-6)]
        [TestCase(-66.3)]
        [TestCase(double.NaN)]
        public void Constructor_WithInvalidProbabilities_ThrowsArgumentOutOfRangeException(double probability)
        {
            // Setup
            PipingSoilProfile profile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();

            // Call
            TestDelegate test = () => new PipingStochasticSoilProfile(probability, profile);

            // Assert
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void Update_WithNullProfile_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticProfile = new PipingStochasticSoilProfile(0.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile());

            // Call
            TestDelegate test = () => stochasticProfile.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromProfile", paramName);
        }

        [Test]
        [TestCaseSource(nameof(PipingStochasticProfileUnequalCombinations))]
        public void Update_WithValidProfile_UpdatesProperties(PipingStochasticSoilProfile stochasticProfile,
                                                              PipingStochasticSoilProfile otherStochasticProfile)
        {
            // Call
            stochasticProfile.Update(otherStochasticProfile);

            // Assert
            Assert.AreEqual(otherStochasticProfile.Probability, stochasticProfile.Probability);
            Assert.AreSame(otherStochasticProfile.SoilProfile, stochasticProfile.SoilProfile);
        }

        [Test]
        public void Equals_OtherType_ReturnsFalse()
        {
            // Setup
            PipingSoilProfile profile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var stochasticProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            bool areEqual = stochasticProfile.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            PipingSoilProfile profile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var stochasticProfile = new PipingStochasticSoilProfile(0.0, profile);

            // Call
            bool areEqual = stochasticProfile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(StochasticProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsTrue(PipingStochasticSoilProfile profile,
                                                          PipingStochasticSoilProfile otherProfile,
                                                          bool expectedEqual)
        {
            // Call
            bool areEqualOne = profile.Equals(otherProfile);
            bool areEqualTwo = otherProfile.Equals(profile);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        [Test]
        public void GetHashCode_EqualPipingStochasticSoilProfiles_ReturnSameHashCode()
        {
            // Setup
            PipingSoilProfile profile = PipingSoilProfileTestFactory.CreatePipingSoilProfile();
            var stochasticSoilProfileA = new PipingStochasticSoilProfile(0.2, profile);
            var stochasticSoilProfileB = new PipingStochasticSoilProfile(0.2, profile);

            // Call
            int hashCodeOne = stochasticSoilProfileA.GetHashCode();
            int hashCodeTwo = stochasticSoilProfileB.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        private static TestCaseData[] PipingStochasticProfileUnequalCombinations()
        {
            const string profileName = "newProfile";
            var stochasticSoilProfileA = new PipingStochasticSoilProfile(
                1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName,
                                                                          SoilProfileType.SoilProfile1D));
            var stochasticSoilProfileB = new PipingStochasticSoilProfile(
                0.5, PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName,
                                                                          SoilProfileType.SoilProfile1D));
            var stochasticSoilProfileC = new PipingStochasticSoilProfile(
                1.0, PipingSoilProfileTestFactory.CreatePipingSoilProfile(profileName,
                                                                          SoilProfileType.SoilProfile2D));

            return new[]
            {
                new TestCaseData(stochasticSoilProfileA, stochasticSoilProfileB)
                {
                    TestName = "Update_ProfileWithProfileA_UpdatesProperties"
                },
                new TestCaseData(stochasticSoilProfileA, stochasticSoilProfileC)
                {
                    TestName = "Update_ProfileWithProfileB_UpdatesProperties"
                }
            };
        }

        private static TestCaseData[] StochasticProfileCombinations()
        {
            PipingStochasticSoilProfile profileA = CreateRandomStochasticProfile(21);
            PipingStochasticSoilProfile profileB = CreateRandomStochasticProfile(21);
            PipingStochasticSoilProfile profileC = CreateRandomStochasticProfile(73);

            return new[]
            {
                new TestCaseData(profileA, profileA, true)
                {
                    TestName = "Equals_SameProfile_ReturnsTrue"
                },
                new TestCaseData(profileA, profileB, true)
                {
                    TestName = "Equals_ProfileAProfileB_ReturnsTrue"
                },
                new TestCaseData(profileB, profileC, false)
                {
                    TestName = "Equals_ProfileBProfileC_ReturnsFalse"
                },
                new TestCaseData(profileA, profileC, false)
                {
                    TestName = "Equals_ProfileAProfileC_ReturnsFalse"
                }
            };
        }

        private static PipingStochasticSoilProfile CreateRandomStochasticProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new PipingStochasticSoilProfile(random.NextDouble(), CreateRandomProfile(random));
        }

        private static PipingSoilProfile CreateRandomProfile(Random random)
        {
            return new PipingSoilProfile(GetRandomName(random), -1.0 - random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    MaterialName = GetRandomName(random),
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    IsAquifer = random.NextBoolean(),
                    BelowPhreaticLevelDeviation = random.NextDouble(),
                    BelowPhreaticLevelMean = random.NextDouble(),
                    BelowPhreaticLevelShift = random.NextDouble(),
                    DiameterD70CoefficientOfVariation = random.NextDouble(),
                    DiameterD70Mean = random.NextDouble(),
                    PermeabilityCoefficientOfVariation = random.NextDouble(),
                    PermeabilityMean = random.NextDouble()
                }
            }, random.NextEnumValue<SoilProfileType>());
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }
    }
}