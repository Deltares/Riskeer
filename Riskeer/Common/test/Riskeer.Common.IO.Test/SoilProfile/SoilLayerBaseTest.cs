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

using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Riskeer.Common.IO.Test.SoilProfile
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
            Assert.IsNull(layer.IsAquifer);
            Assert.IsEmpty(layer.MaterialName);
            Assert.IsNull(layer.Color);

            Assert.IsNull(layer.BelowPhreaticLevelDistributionType);
            Assert.IsNaN(layer.BelowPhreaticLevelShift);
            Assert.IsNaN(layer.BelowPhreaticLevelMean);
            Assert.IsNaN(layer.BelowPhreaticLevelDeviation);
            Assert.IsNaN(layer.BelowPhreaticLevelCoefficientOfVariation);

            Assert.IsNull(layer.DiameterD70DistributionType);
            Assert.IsNaN(layer.DiameterD70Shift);
            Assert.IsNaN(layer.DiameterD70Mean);
            Assert.IsNaN(layer.DiameterD70CoefficientOfVariation);

            Assert.IsNull(layer.PermeabilityDistributionType);
            Assert.IsNaN(layer.PermeabilityShift);
            Assert.IsNaN(layer.PermeabilityMean);
            Assert.IsNaN(layer.PermeabilityCoefficientOfVariation);

            Assert.IsNull(layer.UsePop);
            Assert.IsNull(layer.ShearStrengthModel);

            Assert.IsNull(layer.AbovePhreaticLevelDistributionType);
            Assert.IsNaN(layer.AbovePhreaticLevelMean);
            Assert.IsNaN(layer.AbovePhreaticLevelCoefficientOfVariation);
            Assert.IsNaN(layer.AbovePhreaticLevelShift);

            Assert.IsNull(layer.CohesionDistributionType);
            Assert.IsNaN(layer.CohesionMean);
            Assert.IsNaN(layer.CohesionCoefficientOfVariation);
            Assert.IsNaN(layer.CohesionShift);

            Assert.IsNull(layer.FrictionAngleDistributionType);
            Assert.IsNaN(layer.FrictionAngleMean);
            Assert.IsNaN(layer.FrictionAngleCoefficientOfVariation);
            Assert.IsNaN(layer.FrictionAngleShift);

            Assert.IsNull(layer.ShearStrengthRatioDistributionType);
            Assert.IsNaN(layer.ShearStrengthRatioMean);
            Assert.IsNaN(layer.ShearStrengthRatioCoefficientOfVariation);
            Assert.IsNaN(layer.ShearStrengthRatioShift);

            Assert.IsNull(layer.StrengthIncreaseExponentDistributionType);
            Assert.IsNaN(layer.StrengthIncreaseExponentMean);
            Assert.IsNaN(layer.StrengthIncreaseExponentCoefficientOfVariation);
            Assert.IsNaN(layer.StrengthIncreaseExponentShift);

            Assert.IsNull(layer.PopDistributionType);
            Assert.IsNaN(layer.PopMean);
            Assert.IsNaN(layer.PopCoefficientOfVariation);
            Assert.IsNaN(layer.PopShift);
        }

        private class TestSoilLayerBase : SoilLayerBase {}
    }
}