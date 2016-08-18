﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Piping.IO.Builders;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Builders
{
    [TestFixture]
    public class GenericSoilLayerParametersTest
    {
        [Test]
        public void DefaultConstructor_SetsDefaultProperties()
        {
            // Call
            var parameters = new TestGenericSoilLayerParameters();

            // Assert
            Assert.IsNull(parameters.IsAquifer);
            Assert.IsNull(parameters.Color);
            Assert.IsNull(parameters.MaterialName);
            Assert.IsNull(parameters.BelowPhreaticLevelDeviation);
            Assert.IsNull(parameters.BelowPhreaticLevelDistribution);
            Assert.IsNull(parameters.BelowPhreaticLevelMean);
            Assert.IsNull(parameters.BelowPhreaticLevelShift);
            Assert.IsNull(parameters.DiameterD70Deviation);
            Assert.IsNull(parameters.DiameterD70Distribution);
            Assert.IsNull(parameters.DiameterD70Mean);
            Assert.IsNull(parameters.DiameterD70Shift);
            Assert.IsNull(parameters.PermeabilityDeviation);
            Assert.IsNull(parameters.PermeabilityMean);
            Assert.IsNull(parameters.PermeabilityDistribution);
            Assert.IsNull(parameters.PermeabilityShift);
        }

        [Test]
        public void SetOptionalStochasticParameters_NullSoilLayer_ThrowsArgumentNullException()
        {
            // Setup
            var parameters = new TestGenericSoilLayerParameters();

            // Call
            TestDelegate test = () => parameters.CallSetOptionalStochasticParameters(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("pipingSoilLayer", paramName);
        }

        [Test]
        public void SetOptionalStochasticParameters_SoilLayerNoNullValues_SetsLayerProperties()
        {
            // Setup
            var random = new Random(21);
            var parameters = new TestGenericSoilLayerParameters
            {
                BelowPhreaticLevelMean = random.NextDouble(),
                BelowPhreaticLevelDeviation = random.NextDouble(),
                BelowPhreaticLevelShift = random.NextDouble(),
                DiameterD70Mean = random.NextDouble(),
                DiameterD70Deviation = random.NextDouble(),
                PermeabilityMean = random.NextDouble(),
                PermeabilityDeviation = random.NextDouble(),
            };
            var soilLayer = new PipingSoilLayer(3.0);

            // Call
            parameters.CallSetOptionalStochasticParameters(soilLayer);

            // Assert
            Assert.AreEqual(parameters.BelowPhreaticLevelMean, soilLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(parameters.BelowPhreaticLevelDeviation, soilLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(parameters.BelowPhreaticLevelShift, soilLayer.BelowPhreaticLevelShift);
            Assert.AreEqual(parameters.DiameterD70Mean, soilLayer.DiameterD70Mean);
            Assert.AreEqual(parameters.DiameterD70Deviation, soilLayer.DiameterD70Deviation);
            Assert.AreEqual(parameters.PermeabilityMean, soilLayer.PermeabilityMean);
            Assert.AreEqual(parameters.PermeabilityDeviation, soilLayer.PermeabilityDeviation);
        }

        [Test]
        public void SetOptionalStochasticParameters_SoilLayerNullValues_NoChange()
        {
            // Setup
            var random = new Random(21);
            var parameters = new TestGenericSoilLayerParameters();

            var belowPhreaticLevelMean = random.NextDouble();
            var belowPhreaticLevelDeviation = random.NextDouble();
            var belowPhreaticLevelShift = random.NextDouble();
            var diameterD70Mean = random.NextDouble();
            var diameterD70Deviation = random.NextDouble();
            var permeabilityMean = random.NextDouble();
            var permeabilityDeviation = random.NextDouble();

            var soilLayer = new PipingSoilLayer(3.0)
            {
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelDeviation = belowPhreaticLevelDeviation,
                BelowPhreaticLevelShift = belowPhreaticLevelShift,
                DiameterD70Mean = diameterD70Mean,
                DiameterD70Deviation = diameterD70Deviation,
                PermeabilityMean = permeabilityMean,
                PermeabilityDeviation = permeabilityDeviation,
            };

            // Call
            parameters.CallSetOptionalStochasticParameters(soilLayer);

            // Assert
            Assert.AreEqual(belowPhreaticLevelMean, soilLayer.BelowPhreaticLevelMean);
            Assert.AreEqual(belowPhreaticLevelDeviation, soilLayer.BelowPhreaticLevelDeviation);
            Assert.AreEqual(belowPhreaticLevelShift, soilLayer.BelowPhreaticLevelShift);
            Assert.AreEqual(diameterD70Mean, soilLayer.DiameterD70Mean);
            Assert.AreEqual(diameterD70Deviation, soilLayer.DiameterD70Deviation);
            Assert.AreEqual(permeabilityMean, soilLayer.PermeabilityMean);
            Assert.AreEqual(permeabilityDeviation, soilLayer.PermeabilityDeviation);
        }

        private class TestGenericSoilLayerParameters : GenericSoilLayerParameters
        {
            /// <summary>
            /// Simply calls the implementation of the protected 
            /// <see cref="GenericSoilLayerParameters.SetOptionalStochasticParameters"/>.
            /// </summary>
            /// <param name="pipingSoilLayer">The <see cref="PipingSoilLayer"/> to use as parameter.</param>
            public void CallSetOptionalStochasticParameters(PipingSoilLayer pipingSoilLayer)
            {
                SetOptionalStochasticParameters(pipingSoilLayer);
            }
        }
    }
}