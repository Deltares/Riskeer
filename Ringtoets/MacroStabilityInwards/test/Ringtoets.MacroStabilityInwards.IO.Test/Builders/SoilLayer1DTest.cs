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
using System.Drawing;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.IO.Builders;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.IO.Test.Builders
{
    [TestFixture]
    public class SoilLayer1DTest
    {
        [Test]
        public void Constructor_WithTop_TopSet()
        {
            // Setup
            var random = new Random(22);
            double top = random.NextDouble();

            // Call
            var layer = new SoilLayer1D(top);

            // Assert
            Assert.AreEqual(top, layer.Top);
            Assert.IsNull(layer.IsAquifer);
            Assert.IsNull(layer.MaterialName);
            Assert.IsNull(layer.Color);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(1.0 + 1e-12)]
        [TestCase(2.0)]
        public void AsMacroStabilityInwardsSoilLayer_PropertiesSetWithCorrectParameters_PropertiesAreSetInMacroStabilityInwardsSoilLayer(double isAquifer)
        {
            // Setup
            var random = new Random(22);
            double top = random.NextDouble();
            const string materialName = "materialX";
            Color color = Color.BlanchedAlmond;

            var layer = new SoilLayer1D(top)
            {
                MaterialName = materialName,
                IsAquifer = isAquifer,
                Color = color.ToArgb()
            };

            // Call
            MacroStabilityInwardsSoilLayer1D result = layer.AsMacroStabilityInwardsSoilLayer();

            // Assert
            Assert.AreEqual(top, result.Top);
            Assert.AreEqual(isAquifer.Equals(1.0), result.IsAquifer);
            Assert.AreEqual(materialName, result.MaterialName);
            Assert.AreEqual(Color.FromArgb(color.ToArgb()), result.Color);
        }

        [Test]
        public void AsMacroStabilityInwardsSoilLayer_PropertiesSetWithNullMaterialName_MaterialNameEmptyInMacroStabilityInwardsSoilLayer()
        {
            // Setup
            var random = new Random(22);
            double top = random.NextDouble();
            var layer = new SoilLayer1D(top);

            // Call
            MacroStabilityInwardsSoilLayer1D result = layer.AsMacroStabilityInwardsSoilLayer();

            // Assert
            Assert.IsEmpty(result.MaterialName);
        }
    }
}