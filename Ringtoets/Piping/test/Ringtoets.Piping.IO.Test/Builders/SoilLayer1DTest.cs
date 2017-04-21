// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Builders
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

            Assert.IsNull(layer.BelowPhreaticLevelDistribution);
            Assert.IsNull(layer.BelowPhreaticLevelShift);
            Assert.IsNull(layer.BelowPhreaticLevelMean);
            Assert.IsNull(layer.BelowPhreaticLevelDeviation);

            Assert.IsNull(layer.DiameterD70Distribution);
            Assert.IsNull(layer.DiameterD70Shift);
            Assert.IsNull(layer.DiameterD70Mean);
            Assert.IsNull(layer.DiameterD70CoefficientOfVariation);

            Assert.IsNull(layer.PermeabilityDistribution);
            Assert.IsNull(layer.PermeabilityShift);
            Assert.IsNull(layer.PermeabilityMean);
            Assert.IsNull(layer.PermeabilityCoefficientOfVariation);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(1.0 + 1e-12)]
        [TestCase(2.0)]
        public void AsPipingSoilLayer_PropertiesSetWithCorrectDistributionsAndDifferentLayerParameters_PropertiesAreSetInPipingSoilLayer(double isAquifer)
        {
            // Setup
            var random = new Random(22);
            double top = random.NextDouble();
            var materialName = "materialX";
            Color color = Color.BlanchedAlmond;

            var belowPhreaticLevelDistribution = 3;
            var belowPhreaticLevelShift = 0;
            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelDeviation = random.NextDouble();

            var diameterD70Distribution = 3;
            var diameterD70Shift = 0;
            double diameterD70Mean = random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();

            var permeabilityDistribution = 3;
            var permeabilityShift = 0;
            double permeabilityMean = random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();

            var layer = new SoilLayer1D(top)
            {
                MaterialName = materialName,
                IsAquifer = isAquifer,
                Color = color.ToArgb(),
                BelowPhreaticLevelDistribution = belowPhreaticLevelDistribution,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                DiameterD70Distribution = diameterD70Distribution,
                DiameterD70Shift = diameterD70Shift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70CoefficientOfVariation = diameterD70CoefficientOfVariation,
                PermeabilityDistribution = permeabilityDistribution,
                PermeabilityShift = permeabilityShift,
                PermeabilityMean = permeabilityMean,
                PermeabilityCoefficientOfVariation = permeabilityCoefficientOfVariation
            };

            // Call
            PipingSoilLayer result = layer.AsPipingSoilLayer();

            // Assert
            Assert.AreEqual(top, result.Top);
            Assert.AreEqual(isAquifer.Equals(1.0), result.IsAquifer);
            Assert.AreEqual(belowPhreaticLevelMean, result.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, result.BelowPhreaticLevelDeviation);
            Assert.AreEqual(belowPhreaticLevelShift, result.BelowPhreaticLevelShift);
            Assert.AreEqual(diameterD70Mean, result.DiameterD70Mean);
            Assert.AreEqual(diameterD70CoefficientOfVariation, result.DiameterD70CoefficientOfVariation);
            Assert.AreEqual(permeabilityMean, result.PermeabilityMean);
            Assert.AreEqual(permeabilityCoefficientOfVariation, result.PermeabilityCoefficientOfVariation);
            Assert.AreEqual(materialName, result.MaterialName);
            Assert.AreEqual(Color.FromArgb(color.ToArgb()), result.Color);
        }

        [Test]
        public void AsPipingSoilLayer_IncorrectShiftedLogNormalDistributionType_ThrowsSoilLayerConversionException()
        {
            // Setup
            var layer = new SoilLayer1D(0.0)
            {
                BelowPhreaticLevelDistribution = -1,
            };

            // Call
            TestDelegate test = () => layer.AsPipingSoilLayer();

            // Assert
            string message = Assert.Throws<SoilLayerConversionException>(test).Message;
            Assert.AreEqual(string.Format("Parameter '{0}' is niet verschoven lognormaal verdeeld.", "Verzadigd gewicht"), message);
        }

        [Test]
        [TestCase(false, true, "Korrelgrootte")]
        [TestCase(true, false, "Doorlatendheid")]
        public void AsPipingSoilLayer_IncorrectLogNormalDistributionType_ThrowsSoilLayerConversionException(
            bool isDiameterD70DistributionValid,
            bool isPermeabilityDistributionValid,
            string expectedParameter)
        {
            // Setup
            var validShift = 0.0;
            var layer = new SoilLayer1D(0.0)
            {
                BelowPhreaticLevelDistribution = SoilLayerConstants.LogNormalDistributionValue,
                DiameterD70Distribution = isDiameterD70DistributionValid ? SoilLayerConstants.LogNormalDistributionValue : -1,
                DiameterD70Shift = validShift,
                PermeabilityDistribution = isPermeabilityDistributionValid ? SoilLayerConstants.LogNormalDistributionValue : -1,
                PermeabilityShift = validShift
            };

            // Call
            TestDelegate test = () => layer.AsPipingSoilLayer();

            // Assert
            string message = Assert.Throws<SoilLayerConversionException>(test).Message;
            Assert.AreEqual(string.Format("Parameter '{0}' is niet lognormaal verdeeld.", expectedParameter), message);
        }

        [Test]
        [TestCase(-1e-6, 0.0, "Korrelgrootte")]
        [TestCase(0.0, 9, "Doorlatendheid")]
        public void AsPipingSoilLayer_ShiftNotZero_ThrowsSoilLayerConversionException(
            double diameterD70Shift,
            double permeabilityShift,
            string expectedParameter)
        {
            // Setup
            long validDistribution = SoilLayerConstants.LogNormalDistributionValue;
            var layer = new SoilLayer1D(1.0)
            {
                DiameterD70Distribution = validDistribution,
                DiameterD70Shift = diameterD70Shift,
                PermeabilityDistribution = validDistribution,
                PermeabilityShift = permeabilityShift
            };

            // Call
            TestDelegate test = () => layer.AsPipingSoilLayer();

            // Assert
            string message = Assert.Throws<SoilLayerConversionException>(test).Message;
            Assert.AreEqual(string.Format("Parameter '{0}' is niet lognormaal verdeeld.", expectedParameter), message);
        }

        [Test]
        public void AsPipingSoilLayer_PropertiesSetWithNullMaterialName_MaterialNameEmptyInPipingSoilLayer()
        {
            // Setup
            var random = new Random(22);
            double top = random.NextDouble();
            var layer = new SoilLayer1D(top);

            // Call
            PipingSoilLayer result = layer.AsPipingSoilLayer();

            // Assert
            Assert.IsEmpty(result.MaterialName);
        }
    }
}