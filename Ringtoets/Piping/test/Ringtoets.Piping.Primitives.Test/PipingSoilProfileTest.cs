﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.Piping.Primitives.Test
{
    [TestFixture]
    public class PipingSoilProfileTest
    {
        [Test]
        public void Constructor_WithNameBottomLayersAndAquifer_ReturnsInstanceWithPropsAndEquivalentLayerCollection()
        {
            // Setup
            const string name = "Profile";
            var random = new Random(22);
            double bottom = random.NextDouble();
            var type = random.NextEnumValue<SoilProfileType>();
            var layers = new Collection<PipingSoilLayer>
            {
                new PipingSoilLayer(bottom)
            };

            // Call
            var profile = new PipingSoilProfile(name, bottom, layers, type);

            // Assert
            Assert.AreNotSame(layers, profile.Layers);
            Assert.AreEqual(name, profile.Name);
            Assert.AreEqual(bottom, profile.Bottom);
            Assert.AreEqual(type, profile.SoilProfileType);
        }

        [Test]
        public void Constructor_WithNameBottomLayersEmpty_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(string.Empty, double.NaN, new Collection<PipingSoilLayer>(), SoilProfileType.SoilProfile1D);

            // Assert
            const string expectedMessage = "Geen lagen gevonden voor de ondergrondschematisatie.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithNameBottomLayersNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSoilProfile(string.Empty, double.NaN, null, SoilProfileType.SoilProfile1D);

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
            var equivalentLayers = new List<PipingSoilLayer>(layerCount);
            for (var i = 0; i < layerCount; i++)
            {
                equivalentLayers.Add(new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = i == 0
                });
            }

            var profile = new PipingSoilProfile(string.Empty, bottom, equivalentLayers, SoilProfileType.SoilProfile1D);

            // Call
            PipingSoilLayer[] result = profile.Layers.ToArray();

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
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(bottom - deltaBelowBottom),
                new PipingSoilLayer(1.1)
            };

            // Call
            TestDelegate test = () => new PipingSoilProfile(string.Empty, bottom, pipingSoilLayers, SoilProfileType.SoilProfile1D);

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
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D);

            // Call
            double thickness = profile.GetLayerThickness(pipingSoilLayers[layerIndex]);

            // Assert
            Assert.AreEqual(expectedThickness, thickness);
        }

        [Test]
        public void GetLayerThickness_LayerNotInProfile_ThrowsArgumentException()
        {
            // Setup
            var pipingSoilLayers = new[]
            {
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(1.1)
            };
            var profile = new PipingSoilProfile(string.Empty, 0.0, pipingSoilLayers, SoilProfileType.SoilProfile1D);

            // Call
            TestDelegate test = () => profile.GetLayerThickness(new PipingSoilLayer(1.1));

            // Assert
            const string expectedMessage = "Layer not found in profile.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void ToString_WithNullName_ReturnsStringEmpty()
        {
            // Setup
            var profile = new PipingSoilProfile(null, 0.0, new[]
            {
                new PipingSoilLayer(0.0)
            }, SoilProfileType.SoilProfile1D);

            // Call
            string text = profile.ToString();

            // Assert
            Assert.IsEmpty(text);
        }

        [Test]
        [TestCase("")]
        [TestCase("some name")]
        public void ToString_WithName_ReturnsName(string name)
        {
            // Setup
            var profile = new PipingSoilProfile(name, 0.0, new[]
            {
                new PipingSoilLayer(0.0)
            }, SoilProfileType.SoilProfile1D);

            // Call
            string text = profile.ToString();

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void GetHashCode_EqualProfiles_AreEqual()
        {
            // Setup
            PipingSoilProfile profileA = CreateRandomProfile(21);
            PipingSoilProfile profileB = CreateRandomProfile(21);

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
            PipingSoilProfile profile = CreateRandomProfile(2);
            var derivedProfile = new TestProfile(profile);

            // Call
            bool areEqual = profile.Equals(derivedProfile);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            PipingSoilProfile profile = CreateRandomProfile(2);

            // Call
            bool areEqual = profile.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            var profile = new PipingSoilProfile("name", 0, new[]
            {
                CreateRandomLayer(new Random(21))
            }, SoilProfileType.SoilProfile1D);

            // Call
            bool areEqual = profile.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(ProfileCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(PipingSoilProfile profile, PipingSoilProfile otherProfile, bool expectedEqual)
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
            PipingSoilProfile profileA = CreateRandomProfile(21);
            PipingSoilProfile profileB = CreateRandomProfile(21);
            PipingSoilProfile profileC = CreateRandomProfile(73);

            PipingSoilProfile profileD = CreateSingleLayerProfile("A", -3, SoilProfileType.SoilProfile1D);
            PipingSoilProfile profileE = CreateSingleLayerProfile("A", -3, SoilProfileType.SoilProfile2D);
            PipingSoilProfile profileF = CreateSingleLayerProfile("A", -2, SoilProfileType.SoilProfile1D);
            PipingSoilProfile profileG = CreateSingleLayerProfile("B", -3, SoilProfileType.SoilProfile1D);

            const int seed = 78;
            var random = new Random(seed);
            var profileH = new PipingSoilProfile(GetRandomName(random), -random.NextDouble(), new[]
            {
                CreateRandomLayer(random)
            }, random.NextEnumValue<SoilProfileType>());

            random = new Random(seed);
            var profileI = new PipingSoilProfile(GetRandomName(random), -random.NextDouble(), new[]
            {
                CreateRandomLayer(random),
                CreateRandomLayer(random)
            }, random.NextEnumValue<SoilProfileType>());

            var profileJ = new PipingSoilProfile("A", -3, new[]
            {
                new PipingSoilLayer(-2)
            }, SoilProfileType.SoilProfile1D);
            var profileK = new PipingSoilProfile("A", -3, new[]
            {
                new PipingSoilLayer(-2)
            }, SoilProfileType.SoilProfile1D);

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
                },
                new TestCaseData(profileJ, profileK, true)
                {
                    TestName = "Equals_DifferentIds_True"
                }
            };
        }

        private static PipingSoilProfile CreateSingleLayerProfile(string name, double bottom, SoilProfileType type)
        {
            return new PipingSoilProfile(name, bottom, new[]
            {
                new PipingSoilLayer(bottom + 1.0)
            }, type);
        }

        private static PipingSoilProfile CreateRandomProfile(int randomSeed)
        {
            var random = new Random(randomSeed);
            var layers = new Collection<PipingSoilLayer>();
            for (var i = 0; i < random.Next(2, 6); i++)
            {
                layers.Add(CreateRandomLayer(random));
            }
            return new PipingSoilProfile(GetRandomName(random), -1.0 - random.NextDouble(), layers, random.NextEnumValue<SoilProfileType>());
        }

        private static PipingSoilLayer CreateRandomLayer(Random random)
        {
            return new PipingSoilLayer(random.NextDouble())
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
            };
        }

        private static string GetRandomName(Random random)
        {
            return new string('x', random.Next(0, 40));
        }

        private class TestProfile : PipingSoilProfile
        {
            public TestProfile(PipingSoilProfile profile)
                : base(profile.Name,
                       profile.Bottom,
                       profile.Layers,
                       profile.SoilProfileType) {}
        }
    }
}