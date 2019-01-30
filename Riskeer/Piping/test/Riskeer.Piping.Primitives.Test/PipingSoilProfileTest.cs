// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.Piping.Primitives.Test
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
            Assert.AreEqual(type, profile.SoilProfileSourceType);
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
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(22);

            // Call
            TestDelegate test = () => new PipingSoilProfile(null,
                                                            double.NaN,
                                                            null,
                                                            random.NextEnumValue<SoilProfileType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
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

        [TestFixture]
        private class PipingSoilProfileEqualsTest : EqualsTestFixture<PipingSoilProfile, TestProfile>
        {
            private const string baseName = "Profile name";
            private const double baseBottom = 3.14;
            private const SoilProfileType type = SoilProfileType.SoilProfile1D;

            protected override PipingSoilProfile CreateObject()
            {
                return CreateSingleLayerProfile(baseName, baseBottom, type);
            }

            protected override TestProfile CreateDerivedObject()
            {
                PipingSoilProfile baseProfile = CreateSingleLayerProfile(baseName, baseBottom, type);
                return new TestProfile(baseProfile);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                PipingSoilProfile baseProfile = CreateSingleLayerProfile(baseName, baseBottom, type);

                var random = new Random(21);
                double offset = random.NextDouble();

                yield return new TestCaseData(CreateSingleLayerProfile("Different name",
                                                                       baseProfile.Bottom,
                                                                       baseProfile.SoilProfileSourceType))
                    .SetName("Name");

                yield return new TestCaseData(CreateSingleLayerProfile(baseProfile.Name,
                                                                       baseProfile.Bottom + offset,
                                                                       baseProfile.SoilProfileSourceType))
                    .SetName("Bottom");

                yield return new TestCaseData(CreateSingleLayerProfile(baseProfile.Name,
                                                                       baseProfile.Bottom,
                                                                       SoilProfileType.SoilProfile2D))
                    .SetName("SoilProfileType");

                yield return new TestCaseData(new PipingSoilProfile(baseProfile.Name,
                                                                    baseProfile.Bottom,
                                                                    new[]
                                                                    {
                                                                        new PipingSoilLayer(baseProfile.Bottom + offset)
                                                                    },
                                                                    baseProfile.SoilProfileSourceType))
                    .SetName("Layers");
            }

            private static PipingSoilProfile CreateSingleLayerProfile(string name, double bottom, SoilProfileType type)
            {
                return new PipingSoilProfile(name, bottom, new[]
                {
                    new PipingSoilLayer(bottom + 1.0)
                }, type);
            }
        }

        private class TestProfile : PipingSoilProfile
        {
            public TestProfile(PipingSoilProfile profile)
                : base(profile.Name,
                       profile.Bottom,
                       profile.Layers,
                       profile.SoilProfileSourceType) {}
        }
    }
}