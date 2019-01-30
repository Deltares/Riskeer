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
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Primitives.TestUtil;

namespace Riskeer.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer2DTest
    {
        [Test]
        public void Constructor_OuterRingNullWithoutDataAndNestedLayers_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_OuterRingNullWithDataAndNestedLayers_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(null,
                                                                           new MacroStabilityInwardsSoilLayerData(),
                                                                           Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("outerRing", exception.ParamName);
        }

        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(outerRing, null, Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_NestedLayersNull_ThrowsArgumentNullException()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer2D(outerRing, new MacroStabilityInwardsSoilLayerData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("nestedLayers", exception.ParamName);
        }

        [Test]
        public void Constructor_WithOuterRing_ReturnsNewInstance()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.AreSame(outerRing, layer.OuterRing);
            Assert.IsNotNull(layer.Data);
            Assert.IsEmpty(layer.NestedLayers);
        }

        [Test]
        public void Constructor_WithOuterRingDataAndNestedLayers_ReturnsNewInstance()
        {
            // Setup
            Ring outerRing = RingTestFactory.CreateRandomRing();
            var data = new MacroStabilityInwardsSoilLayerData();
            IEnumerable<MacroStabilityInwardsSoilLayer2D> nestedLayers = Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing, data, nestedLayers);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.AreSame(outerRing, layer.OuterRing);
            Assert.AreSame(data, layer.Data);
            Assert.AreSame(nestedLayers, layer.NestedLayers);
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilLayer2DEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsSoilLayer2D, DerivedMacroStabilityInwardsSoilLayer2D>
        {
            protected override MacroStabilityInwardsSoilLayer2D CreateObject()
            {
                return CreateRandomLayer(21);
            }

            protected override DerivedMacroStabilityInwardsSoilLayer2D CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsSoilLayer2D(CreateRandomLayer(21));
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                MacroStabilityInwardsSoilLayer2D baseLayer = CreateRandomLayer(21);

                yield return new TestCaseData(new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(30),
                                                                                   baseLayer.Data,
                                                                                   baseLayer.NestedLayers))
                    .SetName("OuterRing");
                yield return new TestCaseData(new MacroStabilityInwardsSoilLayer2D(baseLayer.OuterRing,
                                                                                   new MacroStabilityInwardsSoilLayerData(),
                                                                                   baseLayer.NestedLayers))
                    .SetName("Data");
                yield return new TestCaseData(new MacroStabilityInwardsSoilLayer2D(baseLayer.OuterRing,
                                                                                   baseLayer.Data,
                                                                                   new[]
                                                                                   {
                                                                                       baseLayer.NestedLayers.First(),
                                                                                       new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(21))
                                                                                   }))
                    .SetName("Nested Layer Count");
                yield return new TestCaseData(new MacroStabilityInwardsSoilLayer2D(baseLayer.OuterRing,
                                                                                   baseLayer.Data,
                                                                                   new[]
                                                                                   {
                                                                                       new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(30))
                                                                                   }))
                    .SetName("Nested Layer content");
            }

            private static MacroStabilityInwardsSoilLayer2D CreateRandomLayer(int randomSeed)
            {
                var random = new Random(randomSeed);

                return new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(randomSeed),
                                                            new MacroStabilityInwardsSoilLayerData
                                                            {
                                                                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                                                            },
                                                            new[]
                                                            {
                                                                new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(randomSeed))
                                                            });
            }
        }

        private class DerivedMacroStabilityInwardsSoilLayer2D : MacroStabilityInwardsSoilLayer2D
        {
            public DerivedMacroStabilityInwardsSoilLayer2D(MacroStabilityInwardsSoilLayer2D layer)
                : base(layer.OuterRing, layer.Data, layer.NestedLayers) {}
        }
    }
}