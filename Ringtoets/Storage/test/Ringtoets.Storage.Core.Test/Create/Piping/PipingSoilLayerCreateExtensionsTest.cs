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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.Create.Piping;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.Piping
{
    [TestFixture]
    public class PipingSoilLayerCreateExtensionsTest
    {
        [Test]
        public void Create_SoilLayerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((PipingSoilLayer) null).Create(0);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("layer", parameterName);
        }

        [Test]
        public void Create_WithValidProperties_ReturnsEntityWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();
            var soilLayer = new PipingSoilLayer(random.NextDouble())
            {
                IsAquifer = random.NextBoolean(),
                Color = Color.FromKnownColor(random.NextEnumValue<KnownColor>()),
                MaterialName = "MaterialName",
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(1, double.MaxValue),
                    StandardDeviation = random.NextRoundedDouble(),
                    Shift = random.NextRoundedDouble()
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(1, double.MaxValue),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = random.NextRoundedDouble(1, double.MaxValue),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(soilLayer.Top, entity.Top);
            Assert.AreEqual(Convert.ToByte(soilLayer.IsAquifer), entity.IsAquifer);
            Assert.AreEqual(soilLayer.Color.ToInt64(), Convert.ToInt64(entity.Color));

            Assert.AreEqual(soilLayer.BelowPhreaticLevel.Mean, entity.BelowPhreaticLevelMean,
                            soilLayer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(soilLayer.BelowPhreaticLevel.StandardDeviation, entity.BelowPhreaticLevelDeviation,
                            soilLayer.BelowPhreaticLevel.GetAccuracy());
            Assert.AreEqual(soilLayer.BelowPhreaticLevel.Shift, entity.BelowPhreaticLevelShift,
                            soilLayer.BelowPhreaticLevel.GetAccuracy());

            Assert.AreEqual(soilLayer.DiameterD70.Mean, entity.DiameterD70Mean,
                            soilLayer.DiameterD70.GetAccuracy());
            Assert.AreEqual(soilLayer.DiameterD70.CoefficientOfVariation, entity.DiameterD70CoefficientOfVariation,
                            soilLayer.DiameterD70.GetAccuracy());

            Assert.AreEqual(soilLayer.Permeability.Mean, entity.PermeabilityMean,
                            soilLayer.Permeability.GetAccuracy());
            Assert.AreEqual(soilLayer.Permeability.CoefficientOfVariation, entity.PermeabilityCoefficientOfVariation,
                            soilLayer.Permeability.GetAccuracy());
            Assert.AreEqual(order, entity.Order);
        }

        [Test]
        public void Create_WithNaNProperties_ReturnsEntityWithPropertiesSetToNull()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(double.NaN)
            {
                BelowPhreaticLevel = new LogNormalDistribution
                {
                    Mean = RoundedDouble.NaN,
                    StandardDeviation = RoundedDouble.NaN,
                    Shift = RoundedDouble.NaN
                },
                DiameterD70 = new VariationCoefficientLogNormalDistribution
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                },
                Permeability = new VariationCoefficientLogNormalDistribution
                {
                    Mean = RoundedDouble.NaN,
                    CoefficientOfVariation = RoundedDouble.NaN
                }
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(0);

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.Top);
            Assert.IsNull(entity.BelowPhreaticLevelMean);
            Assert.IsNull(entity.BelowPhreaticLevelDeviation);
            Assert.IsNull(entity.BelowPhreaticLevelShift);
            Assert.IsNull(entity.DiameterD70Mean);
            Assert.IsNull(entity.DiameterD70CoefficientOfVariation);
            Assert.IsNull(entity.PermeabilityMean);
            Assert.IsNull(entity.PermeabilityCoefficientOfVariation);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string materialName = "MaterialName";
            var soilLayer = new PipingSoilLayer(0)
            {
                MaterialName = materialName
            };

            // Call
            PipingSoilLayerEntity entity = soilLayer.Create(0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(materialName, entity.MaterialName);
        }
    }
}