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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilProfileTest
    {
        private static readonly Random profileIdRandom = new Random(32);

        [Test]
        [TestCase(0.1, SoilProfileType.SoilProfile1D, 123L)]
        [TestCase(0.26, SoilProfileType.SoilProfile2D, 123L)]
        public void Constructor_WithValidProbabilities_ExpectedValues(double probability, SoilProfileType soilProfileType, long soilProfileId)
        {
            // Call
            var stochasticSoilProfileProbability = new MacroStabilityInwardsStochasticSoilProfile(probability, soilProfileType, soilProfileId);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilProfileProbability);
            Assert.AreEqual(probability, stochasticSoilProfileProbability.Probability);
            Assert.AreEqual(soilProfileType, stochasticSoilProfileProbability.SoilProfileType);
            Assert.AreEqual(soilProfileId, stochasticSoilProfileProbability.SoilProfileId);
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
            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1);

            // Assert
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0)]
        [TestCase(0.23)]
        [TestCase(0.41)]
        public void AddProbability_DifferentValues_ProbabilityIncreasedAsExpected(double probabilityToAdd)
        {
            // Setup
            double startProbability = new Random(21).NextDouble() * 0.5;
            var profile = new MacroStabilityInwardsStochasticSoilProfile(startProbability, SoilProfileType.SoilProfile1D, -1);

            // Call
            profile.AddProbability(probabilityToAdd);

            // Assert
            Assert.AreEqual(startProbability + probabilityToAdd, profile.Probability);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(double.MaxValue)]
        [TestCase(double.MinValue)]
        [TestCase(1.0)]
        [TestCase(-1.0)]
        [TestCase(double.NaN)]
        public void AddProbability_DifferentValuesMakingProbabilityInvalid_ThrowsArgumentOutOfRangeException(double probabilityToAdd)
        {
            // Setup
            double startProbability = new Random(21).NextDouble() * 0.5;
            var profile = new MacroStabilityInwardsStochasticSoilProfile(startProbability, SoilProfileType.SoilProfile1D, -1);

            // Call
            TestDelegate test = () => profile.AddProbability(probabilityToAdd);

            // Assert
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
            Assert.AreEqual(startProbability, profile.Probability);
        }

        [Test]
        public void Update_WithNullProfile_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => stochasticProfile.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromProfile", paramName);
        }

        [Test]
        [TestCaseSource(nameof(StochasticProfileUnequalCombinations))]
        public void Update_WithValidProfile_UpdatesProperties(MacroStabilityInwardsStochasticSoilProfile stochasticProfile, MacroStabilityInwardsStochasticSoilProfile otherStochasticProfile)
        {
            // Call
            bool updated = stochasticProfile.Update(otherStochasticProfile);

            // Assert
            Assert.IsTrue(updated);
            Assert.AreEqual(otherStochasticProfile.Probability, stochasticProfile.Probability);
            Assert.AreEqual(otherStochasticProfile.SoilProfileType, stochasticProfile.SoilProfileType);
            Assert.AreSame(otherStochasticProfile.SoilProfile, stochasticProfile.SoilProfile);
        }

        [Test]
        public void Update_WithEqualProfile_ReturnsFalse()
        {
            // Setup
            const double probability = 1.0;
            var profile = new TestSoilProfile();
            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = profile
            };

            var otherStochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = profile
            };

            // Precondition
            Assert.AreEqual(stochasticProfile, otherStochasticProfile);

            // Call
            bool updated = stochasticProfile.Update(otherStochasticProfile);

            // Assert
            Assert.IsFalse(updated);
            Assert.AreEqual(probability, stochasticProfile.Probability);
            Assert.AreSame(profile, stochasticProfile.SoilProfile);
        }

        [Test]
        public void Equals_OtherType_ReturnsFalse()
        {
            // Setup
            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            bool areEqual = stochasticProfile.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0);

            // Call
            bool areEqual = stochasticProfile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(StochasticProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsStochasticSoilProfile profile, MacroStabilityInwardsStochasticSoilProfile otherProfile, bool expectedEqual)
        {
            // Call
            bool areEqualOne = profile.Equals(otherProfile);
            bool areEqualTwo = otherProfile.Equals(profile);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        [Test]
        public void GetHashCode_EqualStochasticSoilProfile_ReturnSameHashCode()
        {
            // Setup
            var stochasticSoilProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 234);
            var stochasticSoilProfileB = new MacroStabilityInwardsStochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 234);

            // Call
            int hashCodeOne = stochasticSoilProfileA.GetHashCode();
            int hashCodeTwo = stochasticSoilProfileB.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        [Test]
        public void ToString_WithProfile_ReturnsToStringResultOfProfile()
        {
            // Setup
            var profile = new TestSoilProfile();
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = profile
            };

            // Call
            string text = stochasticSoilProfile.ToString();

            // Assert
            Assert.AreEqual(profile.ToString(), text);
        }

        private static TestCaseData[] StochasticProfileUnequalCombinations()
        {
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1.0, SoilProfileType.SoilProfile1D, 0);
            var otherStochasticSoilProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 0);
            var otherStochasticSoilProfileB = new MacroStabilityInwardsStochasticSoilProfile(1.0, SoilProfileType.SoilProfile2D, 0);

            return new[]
            {
                new TestCaseData(stochasticSoilProfile, otherStochasticSoilProfileA)
                {
                    TestName = "Update_ProfileWithProfileA_UpdatesProperties"
                },
                new TestCaseData(stochasticSoilProfile, otherStochasticSoilProfileB)
                {
                    TestName = "Update_ProfileWithProfileB_UpdatesProperties"
                }
            };
        }

        private static TestCaseData[] StochasticProfileCombinations()
        {
            MacroStabilityInwardsStochasticSoilProfile profileA = CreateRandomStochasticProfile(21);
            MacroStabilityInwardsStochasticSoilProfile profileB = CreateRandomStochasticProfile(21);
            MacroStabilityInwardsStochasticSoilProfile profileC = CreateRandomStochasticProfile(73);
            MacroStabilityInwardsStochasticSoilProfile profileD = CreateRandomStochasticProfile(21);
            var profileE = new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 25);
            var profileF = new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile1D, 45);
            var profileG = new MacroStabilityInwardsStochasticSoilProfile(0.5, SoilProfileType.SoilProfile2D, 25);
            var profileH = new MacroStabilityInwardsStochasticSoilProfile(0.15, SoilProfileType.SoilProfile1D, 25);
            var profileI = new MacroStabilityInwardsStochasticSoilProfile(0.15, SoilProfileType.SoilProfile1D, 25)
            {
                SoilProfile = CreateRandomProfile(new Random(12))
            };
            var profileJ = new MacroStabilityInwardsStochasticSoilProfile(0.15, SoilProfileType.SoilProfile1D, 25)
            {
                SoilProfile = CreateRandomProfile(new Random(32))
            };

            return new[]
            {
                new TestCaseData(profileA, profileB, true)
                {
                    TestName = "Equals_ProfileAProfileB_True"
                },
                new TestCaseData(profileB, profileD, true)
                {
                    TestName = "Equals_ProfileBProfileD_True"
                },
                new TestCaseData(profileA, profileD, true)
                {
                    TestName = "Equals_ProfileAProfileD_True"
                },
                new TestCaseData(profileB, profileC, false)
                {
                    TestName = "Equals_ProfileBProfileC_False"
                },
                new TestCaseData(profileA, profileC, false)
                {
                    TestName = "Equals_ProfileAProfileC_False"
                },
                new TestCaseData(profileE, profileF, true)
                {
                    TestName = "Equals_DifferentIds_True"
                },
                new TestCaseData(profileE, profileG, false)
                {
                    TestName = "Equals_DifferentTypes_False"
                },
                new TestCaseData(profileE, profileH, false)
                {
                    TestName = "Equals_DifferentProbability_False"
                },
                new TestCaseData(profileI, profileJ, false)
                {
                    TestName = "Equals_DifferentProfile_False"
                }
            };
        }

        private static MacroStabilityInwardsStochasticSoilProfile CreateRandomStochasticProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new MacroStabilityInwardsStochasticSoilProfile(random.NextDouble(), random.NextEnumValue<SoilProfileType>(), profileIdRandom.Next())
            {
                SoilProfile = CreateRandomProfile(random)
            };
        }

        private static TestSoilProfile CreateRandomProfile(Random random)
        {
            return new TestSoilProfile(GetRandomName(random));
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }

        private class TestSoilProfile : ISoilProfile
        {
            public TestSoilProfile() {}

            public TestSoilProfile(string name)
            {
                Name = name;
            }

            public string Name { get; }

            public override int GetHashCode()
            {
                return 0;
            }

            public override bool Equals(object obj)
            {
                var other = obj as ISoilProfile;
                return other != null && Name.Equals(other.Name);
            }
        }
    }
}