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
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilLayerBaseTest
    {
        [Test]
        public void Constructor_ReturnsNewInstanceWithTopSet()
        {
            // Call
            var layer = new TestSoilLayerBase();

            // Assert
            Assert.IsNotNull(layer);
            Assert.IsFalse(layer.IsAquifer);
            Assert.IsEmpty(layer.MaterialName);
            Assert.AreEqual(Color.Empty, layer.Color);

            Assert.IsNull(layer.BelowPhreaticLevelDistribution);
            Assert.IsNaN(layer.BelowPhreaticLevelShift);
            Assert.IsNaN(layer.BelowPhreaticLevelMean);
            Assert.IsNaN(layer.BelowPhreaticLevelDeviation);

            Assert.IsNull(layer.DiameterD70Distribution);
            Assert.IsNaN(layer.DiameterD70Shift);
            Assert.IsNaN(layer.DiameterD70Mean);
            Assert.IsNaN(layer.DiameterD70CoefficientOfVariation);

            Assert.IsNull(layer.PermeabilityDistribution);
            Assert.IsNaN(layer.PermeabilityShift);
            Assert.IsNaN(layer.PermeabilityMean);
            Assert.IsNaN(layer.PermeabilityCoefficientOfVariation);

            Assert.IsNull(layer.UsePop);
            Assert.IsNull(layer.ShearStrengthModel);

            Assert.IsNull(layer.AbovePhreaticLevelDistribution);
            Assert.IsNaN(layer.AbovePhreaticLevelMean);
            Assert.IsNaN(layer.AbovePhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(layer.AbovePhreaticLevelShift);

            Assert.IsNull(layer.CohesionDistribution);
            Assert.IsNaN(layer.CohesionMean);
            Assert.IsNaN(layer.CohesionCoefficientOfVariation);
            Assert.IsNaN(layer.CohesionShift);

            Assert.IsNull(layer.FrictionAngleDistribution);
            Assert.IsNaN(layer.FrictionAngleMean);
            Assert.IsNaN(layer.FrictionAngleCoefficientOfVariation);
            Assert.IsNaN(layer.FrictionAngleShift);

            Assert.IsNull(layer.ShearStrengthRatioDistribution);
            Assert.IsNaN(layer.ShearStrengthRatioMean);
            Assert.IsNaN(layer.ShearStrengthRatioCoefficientOfVariation);
            Assert.IsNaN(layer.ShearStrengthRatioShift);

            Assert.IsNull(layer.StrengthIncreaseExponentDistribution);
            Assert.IsNaN(layer.StrengthIncreaseExponentMean);
            Assert.IsNaN(layer.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.IsNaN(layer.StrengthIncreaseExponentShift);

            Assert.IsNull(layer.PopDistribution);
            Assert.IsNaN(layer.PopMean);
            Assert.IsNaN(layer.PopCoefficientOfVariation);
            Assert.IsNaN(layer.PopShift);
        }

        [Test]
        public void MaterialName_Null_ThrowsArgumentNullException()
        {
            // Setup
            var soilLayer = new TestSoilLayerBase();

            // Call
            TestDelegate test = () => soilLayer.MaterialName = null;

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("value", paramName);
        }

        [Test]
        public void MaterialName_NotNullValue_ValueSet()
        {
            // Setup
            var soilLayer = new TestSoilLayerBase();
            const string materialName = "a name";

            // Call
            soilLayer.MaterialName = materialName;

            // Assert
            Assert.AreEqual(materialName, soilLayer.MaterialName);
        }

        private class TestSoilLayerBase : SoilLayerBase {}
    }
}