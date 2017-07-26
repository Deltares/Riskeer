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
using System.Collections.ObjectModel;
using System.Drawing;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile2DTest
    {
        private static readonly Random profileIdRandom = new Random(32);

        [Test]
        [TestCase(SoilProfileType.SoilProfile1D)]
        [TestCase(SoilProfileType.SoilProfile2D)]
        public void Constructor_WithNameAndLayers_ReturnsInstanceWithPropsAndEquivalentLayerCollection(SoilProfileType type)
        {
            // Setup
            const string name = "Profile";
            var layers = new Collection<MacroStabilityInwardsSoilLayer2D>
            {
                CreateRandomLayer(21)
            };
            const long soilProfileId = 1234L;

            // Call
            var profile = new MacroStabilityInwardsSoilProfile2D(name, layers, type, soilProfileId);

            // Assert
            Assert.IsInstanceOf<ISoilProfile>(profile);
            Assert.AreNotSame(layers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(type, profile.SoilProfileType);
            Assert.AreEqual(soilProfileId, profile.MacroStabilityInwardsSoilProfileId);
        }

        [Test]
        public void Constructor_LayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(string.Empty, new Collection<MacroStabilityInwardsSoilLayer2D>(), SoilProfileType.SoilProfile2D, 0);

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(null, new Collection<MacroStabilityInwardsSoilLayer2D>(), SoilProfileType.SoilProfile2D, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(string.Empty, null, SoilProfileType.SoilProfile2D, 0);

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfile2D(name, new[]
            {
                CreateRandomLayer(2)
            }, SoilProfileType.SoilProfile2D, 0);

            // Call
            string text = profile.ToString();

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void GetHashCode_EqualProfiles_AreEqual()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileB = CreateRandomProfile(21);

            // Precondition
            Assert.AreEqual(profileA, profileB);
            Assert.AreEqual(profileB, profileA);

            // Call & Assert
            Assert.AreEqual(profileA.GetHashCode(), profileB.GetHashCode());
            Assert.AreEqual(profileB.GetHashCode(), profileA.GetHashCode());
        }

        [Test]
        public void Equals_DerivedClassWithEqualProperties_ReturnsTrue()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D profile = CreateRandomProfile(2);
            var derivedProfile = new TestProfile(profile);

            // Call
            bool areEqual = profile.Equals(derivedProfile);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfile2D("name", new[]
            {
                CreateRandomLayer(new Random(21))
            }, SoilProfileType.SoilProfile2D, -1);

            // Call
            bool areEqual = profile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(ProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilProfile2D profile, MacroStabilityInwardsSoilProfile2D otherProfile, bool expectedEqual)
        {
            // Call
            bool areEqualOne = profile.Equals(otherProfile);
            bool areEqualTwo = otherProfile.Equals(profile);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private class TestProfile : MacroStabilityInwardsSoilProfile2D
        {
            public TestProfile(MacroStabilityInwardsSoilProfile2D profile)
                : base(profile.Name,
                       profile.Layers,
                       profile.SoilProfileType,
                       profile.MacroStabilityInwardsSoilProfileId) {}
        }

        private static TestCaseData[] ProfileCombinations()
        {
            MacroStabilityInwardsSoilProfile2D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileB = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileC = CreateRandomProfile(73);

            MacroStabilityInwardsSoilProfile2D profileD = CreateSingleLayerProfile("A", SoilProfileType.SoilProfile1D);
            MacroStabilityInwardsSoilProfile2D profileE = CreateSingleLayerProfile("A", SoilProfileType.SoilProfile2D);
            MacroStabilityInwardsSoilProfile2D profileG = CreateSingleLayerProfile("B", SoilProfileType.SoilProfile1D);

            const int seed = 78;
            var random = new Random(seed);
            var profileH = new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), new[]
            {
                CreateRandomLayer(random)
            }, random.NextEnumValue<SoilProfileType>(), random.Next());

            random = new Random(seed);
            var profileI = new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), new[]
            {
                CreateRandomLayer(random),
                CreateRandomLayer(random)
            }, random.NextEnumValue<SoilProfileType>(), random.Next());

            var profileJ = new MacroStabilityInwardsSoilProfile2D("A", new[]
            {
                CreateRandomLayer(21)
            }, SoilProfileType.SoilProfile2D, 35);
            var profileK = new MacroStabilityInwardsSoilProfile2D("A", new[]
            {
                CreateRandomLayer(21)
            }, SoilProfileType.SoilProfile2D, 56);

            return new[]
            {
                new TestCaseData(profileA, profileB, true)
                {
                    TestName = "Equals_ProfileAProfileB_True"
                },
                new TestCaseData(profileB, profileC, false)
                {
                    TestName = "Equals_ProfileBProfileC_False"
                },
                new TestCaseData(profileD, profileE, false)
                {
                    TestName = "Equals_ProfileDProfileE_False"
                },
                new TestCaseData(profileD, profileG, false)
                {
                    TestName = "Equals_ProfileDProfileG_False"
                },
                new TestCaseData(profileH, profileI, false)
                {
                    TestName = "Equals_ProfileHProfileI_False"
                },
                new TestCaseData(profileJ, profileK, true)
                {
                    TestName = "Equals_DifferentIds_True"
                }
            };
        }

        private static MacroStabilityInwardsSoilProfile2D CreateSingleLayerProfile(string name, SoilProfileType type)
        {
            return new MacroStabilityInwardsSoilProfile2D(name, new[]
            {
                CreateRandomLayer(2)
            }, type, profileIdRandom.Next());
        }

        private static MacroStabilityInwardsSoilProfile2D CreateRandomProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            var layers = new Collection<MacroStabilityInwardsSoilLayer2D>();
            for (var i = 0; i < random.Next(2, 6); i++)
            {
                layers.Add(CreateRandomLayer(random));
            }
            return new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), layers, random.NextEnumValue<SoilProfileType>(), profileIdRandom.Next());
        }

        private static MacroStabilityInwardsSoilLayer2D CreateRandomLayer(int seed)
        {
            return CreateRandomLayer(new Random(seed));
        }

        private static MacroStabilityInwardsSoilLayer2D CreateRandomLayer(Random random)
        {
            return new MacroStabilityInwardsSoilLayer2D(CreateRandomRing(random.Next()), new[]
            {
                CreateRandomRing(random.Next()),
                CreateRandomRing(random.Next())
            })
            {
                Properties =
                {
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                }
            };
        }

        private static Ring CreateRandomRing(int seed)
        {
            var random = new Random(seed);
            var pointA = new Point2D(random.NextDouble(), random.NextDouble());
            var pointB = new Point2D(random.NextDouble(), random.NextDouble());
            var pointC = new Point2D(random.NextDouble(), random.NextDouble());

            return new Ring(new[]
            {
                pointA,
                pointB,
                pointC
            });
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }
    }
}