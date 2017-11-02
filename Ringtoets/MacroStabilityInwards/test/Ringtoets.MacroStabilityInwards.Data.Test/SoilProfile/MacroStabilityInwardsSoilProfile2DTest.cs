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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
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
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilProfile<MacroStabilityInwardsSoilLayer2D>>(profile);
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
                                                                             Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>(),
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
                                                                             new[]
                                                                             {
                                                                                 MacroStabilityInwardsSoilLayer2DTestFactory.CreateMacroStabilityInwardsSoilLayer2D()
                                                                             },
                                                                             Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("value", exception.ParamName);
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

        [Test]
        public void Equals_ToDerivedClass_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D profile = CreateRandomProfile(21);
            var derivedProfile = new DerivedSoilProfile(profile.Name, profile.Layers, profile.PreconsolidationStresses);

            // Call
            bool profileEqualsDerivedProfile = profile.Equals(derivedProfile);

            // Assert
            Assert.IsFalse(profileEqualsDerivedProfile);
        }

        [Test]
        public void Equals_TransitiveProperty_ReturnsTrue()
        {
            // Setup
            MacroStabilityInwardsSoilProfile2D profileA = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileB = CreateRandomProfile(21);
            MacroStabilityInwardsSoilProfile2D profileC = CreateRandomProfile(21);

            // Call
            bool aEqualsB = profileA.Equals(profileB);
            bool bEqualsC = profileB.Equals(profileC);
            bool aEqualsC = profileA.Equals(profileC);

            // Assert
            Assert.IsTrue(aEqualsB);
            Assert.IsTrue(bEqualsC);
            Assert.IsTrue(aEqualsC);
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
                CopyAndModifySoilLayer(CreateRandomLayer(seed))
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
                CopyAndModifyPreconsolidationsStress(CreateRandomPreconsolidationStress(seed))
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
            return new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(random.Next()),
                                                        new MacroStabilityInwardsSoilLayerData
                                                        {
                                                            Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                                                        },
                                                        new[]
                                                        {
                                                            new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(random.Next())),
                                                            new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(random.Next()))
                                                        });
        }

        private static MacroStabilityInwardsSoilLayer2D CopyAndModifySoilLayer(MacroStabilityInwardsSoilLayer2D soilLayer)
        {
            return new MacroStabilityInwardsSoilLayer2D(soilLayer.OuterRing,
                                                        new MacroStabilityInwardsSoilLayerData
                                                        {
                                                            Color = soilLayer.Data.Color
                                                        },
                                                        new[]
                                                        {
                                                            soilLayer.NestedLayers.ElementAt(0)
                                                        });
        }

        private static MacroStabilityInwardsPreconsolidationStress CreateRandomPreconsolidationStress(int seed)
        {
            var random = new Random(seed);
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = random.NextRoundedDouble()
            };

            return new MacroStabilityInwardsPreconsolidationStress(location, distribution);
        }

        private static MacroStabilityInwardsPreconsolidationStress CopyAndModifyPreconsolidationsStress(
            MacroStabilityInwardsPreconsolidationStress preconsolidationStress)
        {
            var random = new Random(29);
            var modifiedLocation = new Point2D(preconsolidationStress.Location.X + random.NextDouble(),
                                               preconsolidationStress.Location.Y);
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = preconsolidationStress.Stress.Mean,
                CoefficientOfVariation = preconsolidationStress.Stress.CoefficientOfVariation
            };

            return new MacroStabilityInwardsPreconsolidationStress(modifiedLocation, distribution);
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }

        private class DerivedSoilProfile : MacroStabilityInwardsSoilProfile2D
        {
            public DerivedSoilProfile(string name,
                                      IEnumerable<MacroStabilityInwardsSoilLayer2D> layers,
                                      IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses)
                : base(name, layers, preconsolidationStresses) {}
        }
    }
}