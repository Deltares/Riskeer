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
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayer1DTest
    {
        [Test]
        public void Constructor_WithTop_ReturnsNewInstanceWithTopSet()
        {
            // Setup
            double top = new Random(22).NextDouble();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer1D(top);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.Null(layer.Data);
            Assert.AreEqual(top, layer.Top);
        }

        [Test]
        public void GetHashCode_EqualLayers_AreEqual()
        {
            // Setup
            MacroStabilityInwardsSoilLayer1D layerA = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer1D layerB = CreateRandomLayer(21);

            // Precondition
            Assert.AreEqual(layerA, layerB);
            Assert.AreEqual(layerB, layerA);

            // Call & Assert
            Assert.AreEqual(layerA.GetHashCode(), layerB.GetHashCode());
            Assert.AreEqual(layerB.GetHashCode(), layerA.GetHashCode());
        }

        [Test]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayer1D layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(new object());

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        public void Equals_Null_ReturnsFalse()
        {
            // Setup
            MacroStabilityInwardsSoilLayer1D layer = CreateRandomLayer(21);

            // Call
            bool areEqual = layer.Equals(null);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(LayerCombinations))]
        public void Equals_DifferentScenarios_ReturnsExpectedResult(MacroStabilityInwardsSoilLayer1D layer,
                                                                    MacroStabilityInwardsSoilLayer1D otherLayer,
                                                                    bool expectedEqual)
        {
            // Call
            bool areEqualOne = layer.Equals(otherLayer);
            bool areEqualTwo = otherLayer.Equals(layer);

            // Assert
            Assert.AreEqual(expectedEqual, areEqualOne);
            Assert.AreEqual(expectedEqual, areEqualTwo);
        }

        private static TestCaseData[] LayerCombinations()
        {
            MacroStabilityInwardsSoilLayer1D layerA = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer1D layerB = CreateRandomLayer(21);
            MacroStabilityInwardsSoilLayer1D layerC = CreateRandomLayer(73);
            MacroStabilityInwardsSoilLayer1D layerD = CreateRandomLayer(21);

            var layerE = new MacroStabilityInwardsSoilLayer1D(3)
            {
                Data = new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Blue
                }
            };
            var layerF = new MacroStabilityInwardsSoilLayer1D(4)
            {
                Data = new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Blue
                }
            };
            var layerG = new MacroStabilityInwardsSoilLayer1D(3)
            {
                Data = new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.Gold
                }
            };

            return new[]
            {
                new TestCaseData(layerA, layerA, true)
                {
                    TestName = "Equals_LayerALayerA_True"
                },
                new TestCaseData(layerA, layerB, true)
                {
                    TestName = "Equals_LayerALayerB_True"
                },
                new TestCaseData(layerB, layerD, true)
                {
                    TestName = "Equals_LayerALayerD_True"
                },
                new TestCaseData(layerA, layerD, true)
                {
                    TestName = "Equals_LayerALayerD_True"
                },
                new TestCaseData(layerB, layerC, false)
                {
                    TestName = "Equals_LayerBLayerC_False"
                },
                new TestCaseData(layerA, layerC, false)
                {
                    TestName = "Equals_LayerALayerC_False"
                },
                new TestCaseData(layerC, layerC, true)
                {
                    TestName = "Equals_LayerCLayerC_True"
                },
                new TestCaseData(layerE, layerF, false)
                {
                    TestName = "Equals_DifferentTop_False"
                },
                new TestCaseData(layerE, layerG, false)
                {
                    TestName = "Equals_DifferentProperties_False"
                }
            };
        }

        private static MacroStabilityInwardsSoilLayer1D CreateRandomLayer(int randomSeed)
        {
            var random = new Random(randomSeed);
            return new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
            {
                Data = new MacroStabilityInwardsSoilLayerData
                {
                    Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                }
            };
        }
    }
}