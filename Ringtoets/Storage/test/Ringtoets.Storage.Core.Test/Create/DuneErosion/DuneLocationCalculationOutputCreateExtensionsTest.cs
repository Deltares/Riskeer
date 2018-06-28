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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.Storage.Core.Create.DuneErosion;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationOutputCreateExtensionsTest
    {
        [Test]
        public void Create_DuneLocationCalculationOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((DuneLocationCalculationOutput) null).Create();

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("output", paramName);
        }

        [Test]
        public void Create_WithValidParameters_ReturnsDuneLocationCalculationOutputEntity()
        {
            // Setup
            var random = new Random(21);
            var output = new DuneLocationCalculationOutput(random.NextEnumValue<CalculationConvergence>(), new DuneLocationCalculationOutput.ConstructionProperties
            {
                WaterLevel = random.NextDouble(),
                WaveHeight = random.NextDouble(),
                WavePeriod = random.NextDouble(),
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble()
            });

            // Call
            DuneLocationCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.WaterLevel, entity.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(output.WaveHeight, entity.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(output.WavePeriod, entity.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(output.CalculationConvergence), entity.CalculationConvergence);
        }

        [Test]
        public void Create_WithNaNParameters_ReturnsDuneLocationCalculationOutputEntityWithNullValues()
        {
            // Setup
            var random = new Random(21);
            var output = new DuneLocationCalculationOutput(random.NextEnumValue<CalculationConvergence>(), new DuneLocationCalculationOutput.ConstructionProperties
            {
                WaterLevel = double.NaN,
                WaveHeight = double.NaN,
                WavePeriod = double.NaN,
                TargetProbability = double.NaN,
                TargetReliability = double.NaN,
                CalculatedProbability = double.NaN,
                CalculatedReliability = double.NaN
            });

            // Call
            DuneLocationCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.WaterLevel);
            Assert.IsNull(entity.WaveHeight);
            Assert.IsNull(entity.WavePeriod);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual(Convert.ToByte(output.CalculationConvergence), entity.CalculationConvergence);
        }
    }
}