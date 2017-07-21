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

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class SoilLayerPropertiesTest
    {
        [Test]
        public void DefaultConstructor_DefaultValuesSet()
        {
            // Call
            var properties = new SoilLayerProperties();

            // Assert
            Assert.IsFalse(properties.IsAquifer);
            Assert.IsEmpty(properties.MaterialName);
            Assert.AreEqual(Color.Empty, properties.Color);

            Assert.IsFalse(properties.UsePop);
            Assert.AreEqual(ShearStrengthModel.None, properties.ShearStrengthModel);

            Assert.IsNaN(properties.AbovePhreaticLevelMean);
            Assert.IsNaN(properties.AbovePhreaticLevelDeviation);

            Assert.IsNaN(properties.BelowPhreaticLevelMean);
            Assert.IsNaN(properties.BelowPhreaticLevelDeviation);

            Assert.IsNaN(properties.CohesionMean);
            Assert.IsNaN(properties.CohesionDeviation);
            Assert.IsNaN(properties.CohesionShift);

            Assert.IsNaN(properties.FrictionAngleMean);
            Assert.IsNaN(properties.FrictionAngleDeviation);
            Assert.IsNaN(properties.FrictionAngleShift);

            Assert.IsNaN(properties.ShearStrengthRatioMean);
            Assert.IsNaN(properties.ShearStrengthRatioDeviation);
            Assert.IsNaN(properties.ShearStrengthRatioShift);

            Assert.IsNaN(properties.StrengthIncreaseExponentMean);
            Assert.IsNaN(properties.StrengthIncreaseExponentDeviation);
            Assert.IsNaN(properties.StrengthIncreaseExponentShift);

            Assert.IsNaN(properties.PopMean);
            Assert.IsNaN(properties.PopDeviation);
            Assert.IsNaN(properties.PopShift);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            double top = new Random(22).NextDouble();
            var layer = new MacroStabilityInwardsSoilLayer(top);

            // Call
            TestDelegate test = () => layer.MaterialName = null;

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        [TestCase("")]
        [TestCase("A name")]
        public void MaterialName_NotNullValue_ValueSet(string materialName)
        {
            // Setup
            double top = new Random(22).NextDouble();
            var layer = new MacroStabilityInwardsSoilLayer(top);

            // Call
            layer.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, layer.MaterialName);
        }
    }
}