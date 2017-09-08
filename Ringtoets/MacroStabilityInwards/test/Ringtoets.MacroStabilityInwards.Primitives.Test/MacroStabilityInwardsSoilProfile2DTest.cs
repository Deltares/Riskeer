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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils;
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

            var preconsolidationStresses = new[]
            {
                CreateRandomPreconsolidationStress(30)
            };

            // Call
            var profile = new MacroStabilityInwardsSoilProfile2D(name, layers, preconsolidationStresses);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilProfile>(profile);
            Assert.AreNotSame(layers, profile.Layers);
            TestHelper.AssertCollectionsAreEqual(layers, profile.Layers,
                                                 new ReferenceEqualityComparer<MacroStabilityInwardsSoilLayer2D>());
            Assert.AreNotSame(preconsolidationStresses, profile.PreconsolidationStresses);
            TestHelper.AssertCollectionsAreEqual(preconsolidationStresses, profile.PreconsolidationStresses,
                                                 new ReferenceEqualityComparer<MacroStabilityInwardsPreconsolidationStress>());
            Assert.AreEqual(name, profile.Name);
        }

        [Test]
        public void Constructor_LayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(string.Empty,
                                                                             new Collection<MacroStabilityInwardsSoilLayer2D>(),
                                                                             Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(null,
                                                                             new Collection<MacroStabilityInwardsSoilLayer2D>(),
                                                                             Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_LayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilProfile2D(string.Empty, null, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_PreconsolidationStressesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSoilProfile2D(string.Empty, new Collection<MacroStabilityInwardsSoilLayer2D>
            {
                CreateRandomLayer(21)
            }, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preconsolidationStresses", exception.ParamName);
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
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

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
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Call
            bool areEqual = profile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(ProfileCombinationsResultFalse))]
        public void Equals_DifferentPropertyValue_ReturnsFalse(MacroStabilityInwardsSoilProfile2D profile,
                                                               MacroStabilityInwardsSoilProfile2D otherProfile)
        {
            // Call
            bool areEqualOne = profile.Equals(otherProfile);
            bool areEqualTwo = otherProfile.Equals(profile);

            // Assert
            Assert.IsFalse(areEqualOne);
            Assert.IsFalse(areEqualTwo);
        }

        [Test]
        public void GetHashCode_AllPropertiesEqual_ReturnsSameHashCode()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileB = CreateRandomProfile(21);

            // Call
            int hashCodeA = profileA.GetHashCode();
            int hashCodeB = profileB.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeA, hashCodeB);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileB = profileA;

            // Call
            bool profileAEqualB = profileA.Equals(profileB);
            bool profileBEqualA = profileB.Equals(profileA);

            // Assert
            Assert.IsTrue(profileAEqualB);
            Assert.IsTrue(profileBEqualA);
        }

        private static IEnumerable<TestCaseData> ProfileCombinationsResultFalse()
        {
            const int seed = 78;
            var random = new Random(seed);
            var baseProfile = new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), new[]
            {
                CreateRandomLayer(seed),
                CreateRandomLayer(seed)
            }, new[]
            {
                CreateRandomPreconsolidationStress(seed),
                CreateRandomPreconsolidationStress(seed)
            });

            yield return new TestCaseData(baseProfile,
                                          new MacroStabilityInwardsSoilProfile2D("Different Name",
                                                                                 baseProfile.Layers,
                                                                                 baseProfile.PreconsolidationStresses))
                .SetName("Different Name");

            yield return new TestCaseData(baseProfile,
                                          new MacroStabilityInwardsSoilProfile2D(baseProfile.Name,
                                                                                 new[]
                                                                                 {
                                                                                     CreateRandomLayer(seed)
                                                                                 },
                                                                                 baseProfile.PreconsolidationStresses))
                .SetName("Different SoilLayer count");

            var differentLayers = new[]
            {
                CreateRandomLayer(seed),
                CreateRandomLayer(seed + 1)
            };
            yield return new TestCaseData(baseProfile,
                                          new MacroStabilityInwardsSoilProfile2D(baseProfile.Name,
                                                                                 differentLayers,
                                                                                 baseProfile.PreconsolidationStresses))
                .SetName("Different SoilLayers");

            yield return new TestCaseData(baseProfile,
                                          new MacroStabilityInwardsSoilProfile2D(baseProfile.Name,
                                                                                 baseProfile.Layers,
                                                                                 Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>()))
                .SetName("Different Stress count");

            var differentStresses = new[]
            {
                CreateRandomPreconsolidationStress(seed),
                CreateRandomPreconsolidationStress(seed + 1)
            };
            yield return new TestCaseData(baseProfile,
                                          new MacroStabilityInwardsSoilProfile2D(baseProfile.Name,
                                                                                 baseProfile.Layers,
                                                                                 differentStresses))
                .SetName("Different Stresses");
        }

        private static MacroStabilityInwardsSoilProfile2D CreateRandomProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            var layers = new Collection<MacroStabilityInwardsSoilLayer2D>();
            for (var i = 0; i < random.Next(2, 6); i++)
            {
                layers.Add(CreateRandomLayer(random));
            }

            var stresses = new Collection<MacroStabilityInwardsPreconsolidationStress>();
            for (var i = 0; i < random.Next(2, 6); i++)
            {
                stresses.Add(CreateRandomPreconsolidationStress(i));
            }
            return new MacroStabilityInwardsSoilProfile2D(GetRandomName(random), layers, stresses);
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

        private static MacroStabilityInwardsPreconsolidationStress CreateRandomPreconsolidationStress(int seed)
        {
            var random = new Random(seed);
            return new MacroStabilityInwardsPreconsolidationStress(random.NextDouble(),
                                                                   random.NextDouble(),
                                                                   random.NextDouble(),
                                                                   random.NextDouble(),
                                                                   random.NextDouble());
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }
    }
}