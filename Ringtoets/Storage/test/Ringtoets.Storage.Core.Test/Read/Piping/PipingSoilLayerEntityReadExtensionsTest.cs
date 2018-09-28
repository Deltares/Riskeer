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
using System.Drawing;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.Piping;

namespace Ringtoets.Storage.Core.Test.Read.Piping
{
    [TestFixture]
    public class PipingSoilLayerEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((PipingSoilLayerEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_WithValues_ReturnsPipingSoilLayerWithDoubleParameterValues()
        {
            // Setup
            var random = new Random(21);
            double top = random.NextDouble();
            Color color = Color.FromKnownColor(random.NextEnumValue<KnownColor>());
            const string materialName = "sand";

            bool isAquifer = random.NextBoolean();
            double belowPhreaticLevelMean = random.NextDouble(1, double.MaxValue);
            double belowPhreaticLevelDeviation = random.NextDouble();
            double belowPhreaticLevelShift = random.NextDouble();
            double diameterD70Mean = random.NextDouble(1, double.MaxValue);
            double diameterD70CoefficientOfVariation = random.NextDouble();
            double permeabilityMean = random.NextDouble(1, double.MaxValue);
            double permeabilityCoefficientOfVariation = random.NextDouble();

            var entity = new PipingSoilLayerEntity
            {
                Top = top,
                IsAquifer = Convert.ToByte(isAquifer),
                Color = color.ToInt64(),
                MaterialName = materialName,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70CoefficientOfVariation = diameterD70CoefficientOfVariation,
                PermeabilityMean = permeabilityMean,
                PermeabilityCoefficientOfVariation = permeabilityCoefficientOfVariation
            };

            // Call
            PipingSoilLayer layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            Assert.AreEqual(top, layer.Top, 1e-6);
            Assert.AreEqual(isAquifer, layer.IsAquifer);
            Assert.IsNotNull(color);
            Assert.AreEqual(color.ToArgb(), layer.Color.ToArgb());
            Assert.AreEqual(materialName, layer.MaterialName);

            Assert.AreEqual(belowPhreaticLevelMean, layer.BelowPhreaticLevel.Mean,
                            layer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelDeviation, layer.BelowPhreaticLevel.StandardDeviation,
                            layer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelShift, layer.BelowPhreaticLevel.Shift,
                            layer.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(diameterD70Mean, layer.DiameterD70.Mean,
                            layer.DiameterD70.GetAccuracy());
            Assert.AreEqual(diameterD70CoefficientOfVariation, layer.DiameterD70.CoefficientOfVariation,
                            layer.DiameterD70.GetAccuracy());

            Assert.AreEqual(permeabilityMean, layer.Permeability.Mean,
                            layer.Permeability.GetAccuracy());
            Assert.AreEqual(permeabilityCoefficientOfVariation, layer.Permeability.CoefficientOfVariation,
                            layer.Permeability.GetAccuracy());
        }

        [Test]
        public void Read_WithNullValues_ReturnsPipingSoilLayerWithNaNValues()
        {
            // Setup
            var entity = new PipingSoilLayerEntity
            {
                MaterialName = nameof(PipingSoilLayerEntity)
            };

            // Call
            PipingSoilLayer layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            Assert.AreEqual(entity.MaterialName, layer.MaterialName);

            Assert.IsNaN(layer.Top);
            Assert.IsNaN(layer.BelowPhreaticLevel.Mean);
            Assert.IsNaN(layer.BelowPhreaticLevel.StandardDeviation);
            Assert.IsNaN(layer.BelowPhreaticLevel.Shift);
            Assert.IsNaN(layer.DiameterD70.Mean);
            Assert.IsNaN(layer.DiameterD70.CoefficientOfVariation);
            Assert.IsNaN(layer.Permeability.Mean);
            Assert.IsNaN(layer.Permeability.CoefficientOfVariation);
        }
    }
}