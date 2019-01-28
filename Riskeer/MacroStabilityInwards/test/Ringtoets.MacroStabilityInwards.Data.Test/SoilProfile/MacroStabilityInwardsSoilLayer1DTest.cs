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
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Setup
            double top = new Random(22).NextDouble();

            // Call
            TestDelegate test = () => new MacroStabilityInwardsSoilLayer1D(top, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("data", paramName);
        }

        [Test]
        public void Constructor_WithTop_ReturnsNewInstance()
        {
            // Setup
            double top = new Random(22).NextDouble();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer1D(top);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.AreEqual(top, layer.Top);
            Assert.IsNotNull(layer.Data);
        }

        [Test]
        public void Constructor_WithTopAndData_ReturnsNewInstance()
        {
            // Setup
            double top = new Random(22).NextDouble();
            var data = new MacroStabilityInwardsSoilLayerData();

            // Call
            var layer = new MacroStabilityInwardsSoilLayer1D(top, data);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsSoilLayer>(layer);
            Assert.AreEqual(top, layer.Top);
            Assert.AreSame(data, layer.Data);
        }

        [TestFixture]
        private class MacroStabilityInwardsSoilLayer1DEqualsTest
            : EqualsTestFixture<MacroStabilityInwardsSoilLayer1D, DerivedMacroStabilityInwardsSoilLayer1D>
        {
            protected override MacroStabilityInwardsSoilLayer1D CreateObject()
            {
                return CreateRandomLayer(21);
            }

            protected override DerivedMacroStabilityInwardsSoilLayer1D CreateDerivedObject()
            {
                return new DerivedMacroStabilityInwardsSoilLayer1D(CreateRandomLayer(21));
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                var random = new Random(32);
                MacroStabilityInwardsSoilLayer1D baseLayer = CreateRandomLayer(21);

                yield return new TestCaseData(new MacroStabilityInwardsSoilLayer1D(baseLayer.Top))
                    .SetName("SoilLayerData");
                yield return new TestCaseData(new MacroStabilityInwardsSoilLayer1D(baseLayer.Top + random.NextDouble(),
                                                                                   baseLayer.Data))
                    .SetName("Top");
            }

            private static MacroStabilityInwardsSoilLayer1D CreateRandomLayer(int seed)
            {
                var random = new Random(seed);

                return new MacroStabilityInwardsSoilLayer1D(random.NextDouble())
                {
                    Data =
                    {
                        Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>())
                    }
                };
            }
        }

        private class DerivedMacroStabilityInwardsSoilLayer1D : MacroStabilityInwardsSoilLayer1D
        {
            public DerivedMacroStabilityInwardsSoilLayer1D(MacroStabilityInwardsSoilLayer1D layer) : base(layer.Top, layer.Data) {}
        }
    }
}