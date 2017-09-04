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
        [Test]
        public void Constructor_WithNameAndLayers_ReturnsInstanceWithPropsAndEquivalentLayerCollection()
        {
            // Setup
            const string name = "Profile";
            var layers = new Collection<MacroStabilityInwardsSoilLayer2D>
            {
                CreateRandomLayer(21)
            };

            // Call
            var profile = new MacroStabilityInwardsSoilProfile2D(name, layers);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilProfile>(profile);
            Assert.AreNotSame(layers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
        }

        [Test]
        public void Constructor_LayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(string.Empty, new Collection<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(null, new Collection<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(string.Empty, null);

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
            });

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
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfile2D("name", new[]
            {
                CreateRandomLayer(new Random(21))
            });

            // Call
            bool areEqual = profile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(ProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilProfile2D profile,
                                                                    MacroStabilityInwardsSoilProfile2D otherProfile,
                                                                    bool expectedEqual)
        {
            // Call
            bool areEqualOne = profile.Equals(otherProfile);
            bool areEqualTwo = otherProfile.Equals(profile);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private static TestCaseData[] ProfileCombinations()
        {
            MacroStabilityInwardsSoilProfile2D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileB = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileC = CreateRandomProfile(73);

            MacroStabilityInwardsSoilProfile2D profileD = CreateSingleLayerProfile("A");
            MacroStabilityInwardsSoilProfile2D profileE = CreateSingleLayerProfile("B");

            const int seed = 78;
            var random = new Random(seed);
            var profileG = new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), new[]
            {
                CreateRandomLayer(random)
            });

            random = new Random(seed);
            var profileH = new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), new[]
            {
                CreateRandomLayer(random),
                CreateRandomLayer(random)
            });

            var profileI = new MacroStabilityInwardsSoilProfile2D("A", new[]
            {
                CreateRandomLayer(21)
            });

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
                }
            };
        }

        private static MacroStabilityInwardsSoilProfile2D CreateSingleLayerProfile(string name)
        {
            return new MacroStabilityInwardsSoilProfile2D(name, new[]
            {
                CreateRandomLayer(2)
            });
        }

        private static MacroStabilityInwardsSoilProfile2D CreateRandomProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            var layers = new Collection<MacroStabilityInwardsSoilLayer2D>();
            for (var i = 0; i < random.Next(2, 6); i++)
            {
                layers.Add(CreateRandomLayer(random));
            }
            return new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), layers);
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