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
            var top = random.NextDouble();

            // Call
            var layer = new SoilLayer1D(top);

            // Assert
            Assert.AreEqual(top, layer.Top);
            Assert.IsNull(layer.AbovePhreaticLevel);
            Assert.IsNull(layer.DryUnitWeight);
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
            Assert.IsNull(layer.DiameterD70Deviation);

            Assert.IsNull(layer.PermeabilityDistribution);
            Assert.IsNull(layer.PermeabilityShift);
            Assert.IsNull(layer.PermeabilityMean);
            Assert.IsNull(layer.PermeabilityDeviation);
        }

        [Test]
        [TestCase(1.0)]
        [TestCase(1.0+1e-12)]
        [TestCase(2.0)]
        public void AsPipingSoilLayer_PropertiesSetWithDifferentIsAquifer_PropertiesAreSetInPipingSoilLayer(double isAquifer)
        {
            // Setup
            var random = new Random(22);
            var top = random.NextDouble();
            var materialName = "materialX";
            var abovePhreaticLevel = random.NextDouble();
            var dryUnitWeight = random.NextDouble();
            var color = Color.BlanchedAlmond;

            var belowPhreaticLevelDistribution = random.Next();
            var belowPhreaticLevelShift = random.NextDouble();
            var belowPhreaticLevelMean = random.NextDouble();
            var belowPhreaticLevelDeviation = random.NextDouble();

            var diameterD70Distribution = random.Next();
            var diameterD70Shift = random.NextDouble();
            var diameterD70Mean = random.NextDouble();
            var diameterD70Deviation = random.NextDouble();

            var permeabilityDistribution = random.Next();
            var permeabilityShift = random.NextDouble();
            var permeabilityMean = random.NextDouble();
            var permeabilityDeviation = random.NextDouble();

            var layer = new SoilLayer1D(top)
            {
                MaterialName = materialName,
                IsAquifer = isAquifer,
                AbovePhreaticLevel = abovePhreaticLevel,
                DryUnitWeight = dryUnitWeight,
                Color = color.ToArgb(),

                BelowPhreaticLevelDistribution = belowPhreaticLevelDistribution,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,

                DiameterD70Distribution = diameterD70Distribution,
                DiameterD70Shift = diameterD70Shift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70Deviation = diameterD70Deviation,

                PermeabilityDistribution = permeabilityDistribution,
                PermeabilityShift = permeabilityShift,
                PermeabilityMean = permeabilityMean,
                PermeabilityDeviation = permeabilityDeviation
            };

            // Call
            var result = layer.AsPipingSoilLayer();

            // Assert
            Assert.AreEqual(top, result.Top);
            Assert.AreEqual(isAquifer.Equals(1.0), result.IsAquifer);
            Assert.AreEqual(abovePhreaticLevel, result.AbovePhreaticLevel);
            Assert.AreEqual(belowPhreaticLevelMean, result.BelowPhreaticLevel);
            Assert.AreEqual(dryUnitWeight, result.DryUnitWeight);
            Assert.AreEqual(materialName, result.MaterialName);
            Assert.AreEqual(Color.FromArgb(color.ToArgb()), result.Color);
        }

        [Test]
        public void AsPipingSoilLayer_PropertiesSetWithNullMaterialName_MaterialNameEmptyInPipingSoilLayer()
        {
            // Setup
            var random = new Random(22);
            var top = random.NextDouble();
            var layer = new SoilLayer1D(top);

            // Call
            var result = layer.AsPipingSoilLayer();

            // Assert
            Assert.IsEmpty(result.MaterialName);
        }
    }
}