// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
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
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilProfile<MacroStabilityInwardsSoilLayer1D>>(profile);
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
                    Data =
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

        [TestFixture]
        private class MacroStabilityInwardsSoilProfile1DEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsSoilProfile1D, DerivedMacroStabilityInwardsSoilProfile1D>
        {
            protected override MacroStabilityInwardsSoilProfile1D CreateObject()
            {
                return CreateSoilProfile();
            }

            protected override DerivedMacroStabilityInwardsSoilProfile1D CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsSoilProfile1D(CreateSoilProfile());
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                var random = new Random(21);
                MacroStabilityInwardsSoilProfile1D baseProfile = CreateSoilProfile();

                yield return new TestCaseData(new MacroStabilityInwardsSoilProfile1D("Different name",
                                                                                     baseProfile.Bottom,
                                                                                     baseProfile.Layers))
                    .SetName("Name");
                yield return new TestCaseData(new MacroStabilityInwardsSoilProfile1D(baseProfile.Name,
                                                                                     baseProfile.Bottom - random.NextDouble(),
                                                                                     baseProfile.Layers))
                    .SetName("Bottom");
                yield return new TestCaseData(new MacroStabilityInwardsSoilProfile1D(baseProfile.Name,
                                                                                     baseProfile.Bottom,
                                                                                     new[]
                                                                                     {
                                                                                         MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(random.NextDouble())
                                                                                     }))
                    .SetName("Layer");
                yield return new TestCaseData(new MacroStabilityInwardsSoilProfile1D(baseProfile.Name,
                                                                                     baseProfile.Bottom,
                                                                                     new[]
                                                                                     {
                                                                                         MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(random.NextDouble()),
                                                                                         MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D(random.NextDouble())
                                                                                     }))
                    .SetName("Layer Count");
            }

            private static MacroStabilityInwardsSoilProfile1D CreateSoilProfile()
            {
                var random = new Random(21);
                double bottom = -random.NextDouble();

                return new MacroStabilityInwardsSoilProfile1D("Profile Name", bottom, new[]
                {
                    MacroStabilityInwardsSoilLayer1DTestFactory.CreateMacroStabilityInwardsSoilLayer1D()
                });
            }
        }

        private class DerivedMacroStabilityInwardsSoilProfile1D : MacroStabilityInwardsSoilProfile1D
        {
            public DerivedMacroStabilityInwardsSoilProfile1D(MacroStabilityInwardsSoilProfile1D profile)
                : base(profile.Name, profile.Bottom, profile.Layers) {}
        }
    }
}