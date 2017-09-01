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
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsStochasticSoilProfileTest
    {
        [Test]
        public void Constructor_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsStochasticSoilProfile(0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Constructor_WithValidProbabilities_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            double probability = new Random().Next(0, 1);

            // Call
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(probability, soilProfile);

            // Assert
            Assert.IsInstanceOf<Observable>(stochasticSoilProfile);
            Assert.AreEqual(probability, stochasticSoilProfile.Probability);
            Assert.AreSame(soilProfile, stochasticSoilProfile.SoilProfile);
            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsStochasticSoilProfile(probability, soilProfile);

            // Assert
            const string expectedMessage = "Het aandeel van de ondergrondschematisatie in het stochastische ondergrondmodel" +
                                           " moet in het bereik [0,0, 1,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_WithNullProfile_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);

            // Call
            TestDelegate test = () => stochasticProfile.Update(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("fromProfile", paramName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(StochasticProfileVariousCombinations))]
        public void Update_WithValidProfile_UpdatesProperties(MacroStabilityInwardsStochasticSoilProfile stochasticProfile,
                                                              MacroStabilityInwardsStochasticSoilProfile otherStochasticProfile)
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
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);

            // Call
            bool areEqual = stochasticProfile.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var stochasticProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);

            // Call
            bool areEqual = stochasticProfile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(StochasticProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsStochasticSoilProfile profile,
                                                                    MacroStabilityInwardsStochasticSoilProfile otherProfile,
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
        public void GetHashCode_EqualStochasticSoilProfile_ReturnSameHashCode()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var stochasticSoilProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);
            var stochasticSoilProfileB = new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile);

            // Call
            int hashCodeOne = stochasticSoilProfileA.GetHashCode();
            int hashCodeTwo = stochasticSoilProfileB.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
            mocks.VerifyAll();
        }

        [Test]
        public void ToString_WithProfile_ReturnsToStringResultOfProfile()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0.0, soilProfile);

            // Call
            string text = stochasticSoilProfile.ToString();

            // Assert
            Assert.AreEqual(soilProfile.ToString(), text);
        }

        private static TestCaseData[] StochasticProfileVariousCombinations()
        {
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(1.0, CreateRandomProfile(22));
            var otherStochasticSoilProfileA = new MacroStabilityInwardsStochasticSoilProfile(0.5, CreateRandomProfile(22));
            var otherStochasticSoilProfileB = new MacroStabilityInwardsStochasticSoilProfile(1.0, CreateRandomProfile(23));

            const double probability = 1.0;
            TestSoilProfile profile = CreateRandomProfile(22);
            var stochasticProfileSameProperties = new MacroStabilityInwardsStochasticSoilProfile(probability, profile);
            var otherStochasticProfileSameProperties = new MacroStabilityInwardsStochasticSoilProfile(probability, profile);

            return new[]
            {
                new TestCaseData(stochasticSoilProfile, otherStochasticSoilProfileA)
                {
                    TestName = "Update_ProfileWithProfileADifferentProbability_UpdatesProperties"
                },
                new TestCaseData(stochasticSoilProfile, otherStochasticSoilProfileB)
                {
                    TestName = "Update_ProfileWithProfileBDifferentSoilProfile_UpdatesProperties"
                },
                new TestCaseData(stochasticProfileSameProperties, otherStochasticProfileSameProperties)
                {
                    TestName = "Update_ProfileWithProfileSameProperties_PropertiesRemainSame"
                }
            };
        }

        private static TestCaseData[] StochasticProfileCombinations()
        {
            var profileA = new MacroStabilityInwardsStochasticSoilProfile(0.5, CreateRandomProfile(21));
            var profileB = new MacroStabilityInwardsStochasticSoilProfile(0.5, CreateRandomProfile(21));
            var profileC = new MacroStabilityInwardsStochasticSoilProfile(0.15, CreateRandomProfile(21));
            var profileD = new MacroStabilityInwardsStochasticSoilProfile(0.5, CreateRandomProfile(73));

            return new[]
            {
                new TestCaseData(profileA, profileB, true)
                {
                    TestName = "Equals_SameProbabilityAndProfile_True"
                },
                new TestCaseData(profileA, profileC, false)
                {
                    TestName = "Equals_DifferentProbability_False"
                },
                new TestCaseData(profileA, profileD, false)
                {
                    TestName = "Equals_DifferentProfile_False"
                }
            };
        }

        private static TestSoilProfile CreateRandomProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new TestSoilProfile(GetRandomName(random));
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }

        private class TestSoilProfile : IMacroStabilityInwardsSoilProfile
        {
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
                var other = obj as IMacroStabilityInwardsSoilProfile;
                return other != null && Name.Equals(other.Name);
            }
        }
    }
}