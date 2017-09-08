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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfile1DTest
    {
        [Test]
        public void Constructor_WithNameBottomLayersAndAquifer_ReturnsInstanceWithPropsAndEquivalentLayerCollection()
        {
            // Setup
            const string name = "Profile";
            var random = new Random(22);
            double bottom = random.NextDouble();
            var layers = new Collection<MacroStabilityInwardsSoilLayer1D>
            {
                new MacroStabilityInwardsSoilLayer1D(bottom)
            };

            // Call
            var profile = new MacroStabilityInwardsSoilProfile1D(name, bottom, layers);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilProfile>(profile);
            Assert.AreNotSame(layers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
        }

        [Test]
        public void Constructor_LayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile1D(string.Empty, double.NaN, new Collection<MacroStabilityInwardsSoilLayer1D>());

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile1D(null, double.NaN, new Collection<MacroStabilityInwardsSoilLayer1D>());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile1D(string.Empty, double.NaN, null);

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(15)]
        public void Layers_Always_ReturnsDescendingByTopOrderedList(int layerCount)
        {
            // Setup
            var random = new Random(21);
            const double bottom = 0.0;
            var equivalentLayers = new List<MacroStabilityInwardsSoilLayer1D>(layerCount);
            for (var i = 0; i < layerCount; i++)
            {
                equivalentLayers.Add(new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
                {
                    Properties =
                    {
                        IsAquifer = i == 0
                    }
                });
            }

            var profile = new MacroStabilityInwardsSoilProfile1D(string.Empty, bottom, equivalentLayers);

            // Call
            MacroStabilityInwardsSoilLayer1D[] result = profile.Layers.ToArray();

            // Assert
            CollectionAssert.AreEquivalent(equivalentLayers, result);
            CollectionAssert.AreEqual(equivalentLayers.OrderByDescending(l => l.Top).Select(l => l.Top), result.Select(l => l.Top));
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(4)]
        public void Constructor_WithNameBottomLayersBelowBottom_ThrowsArgumentException(double deltaBelowBottom)
        {
            // Setup
            const double bottom = 0.0;
            var soilLayers = new[]
            {
                new MacroStabilityInwardsSoilLayer1D(bottom - deltaBelowBottom),
                new MacroStabilityInwardsSoilLayer1D(1.1)
            };

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile1D(string.Empty, bottom, soilLayers);

            // Assert
            const string expectedMessage = "Eén of meerdere lagen hebben een top onder de bodem van de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 1.1)]
        public void GetLayerThickness_LayerInProfile_ReturnsThicknessOfLayer(int layerIndex, double expectedThickness)
        {
            // Setup
            var soilLayers = new[]
            {
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(1.1)
            };
            var profile = new MacroStabilityInwardsSoilProfile1D(string.Empty, 0.0, soilLayers);

            // Call
            double thickness = profile.GetLayerThickness(soilLayers[layerIndex]);

            // Assert
            Assert.AreEqual(expectedThickness, thickness);
        }

        [Test]
        public void GetLayerThickness_LayerNotInProfile_ThrowsArgumentException()
        {
            // Setup
            var soilLayers = new[]
            {
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(1.1)
            };
            var profile = new MacroStabilityInwardsSoilProfile1D(string.Empty, 0.0, soilLayers);

            // Call
            TestDelegate test = () => profile.GetLayerThickness(new MacroStabilityInwardsSoilLayer1D(1.1));

            // Assert
            const string expectedMessage = "Layer not found in profile.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfile1D(name, 0.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(0.0)
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
            MacroStabilityInwardsSoilProfile1D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile1D profileB = CreateRandomProfile(21);

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
            var profile = new MacroStabilityInwardsSoilProfile1D("name", 0, new[]
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
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilProfile1D profile, MacroStabilityInwardsSoilProfile1D otherProfile, bool expectedEqual)
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
            MacroStabilityInwardsSoilProfile1D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile1D profileB = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile1D profileC = CreateRandomProfile(73);

            MacroStabilityInwardsSoilProfile1D profileD = CreateSingleLayerProfile("A", -3);
            MacroStabilityInwardsSoilProfile1D profileE = CreateSingleLayerProfile("A", -2);
            MacroStabilityInwardsSoilProfile1D profileF = CreateSingleLayerProfile("B", -3);

            const int seed = 78;
            var random = new Random(seed);
            var profileG = new MacroStabilityInwardsSoilProfile1D(GetRandomName(random), -random.NextDouble(), new[]
            {
                CreateRandomLayer(random)
            });

            random = new Random(seed);
            var profileH = new MacroStabilityInwardsSoilProfile1D(GetRandomName(random), -random.NextDouble(), new[]
            {
                CreateRandomLayer(random),
                CreateRandomLayer(random)
            });

            var profileI = new MacroStabilityInwardsSoilProfile1D("A", -3, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-2)
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
                new TestCaseData(profileD, profileF, false)
                {
                    TestName = "Equals_ProfileDProfileF_False"
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

        private static MacroStabilityInwardsSoilProfile1D CreateSingleLayerProfile(string name, double bottom)
        {
            return new MacroStabilityInwardsSoilProfile1D(name, bottom, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(bottom + 1.0)
            });
        }

        private static MacroStabilityInwardsSoilProfile1D CreateRandomProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            var layers = new Collection<MacroStabilityInwardsSoilLayer1D>();
            for (var i = 0; i < random.Next(2, 6); i++)
            {
                layers.Add(CreateRandomLayer(random));
            }
            return new MacroStabilityInwardsSoilProfile1D(GetRandomName(random), -1.0 - random.NextDouble(), layers);
        }

        private static MacroStabilityInwardsSoilLayer1D CreateRandomLayer(Random random)
        {
            return new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            {
                Properties =
                {
                    MaterialName = GetRandomName(random),
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                    IsAquifer = random.NextBoolean()
                }
            };
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }
    }
}