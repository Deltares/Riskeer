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
using Application.Ringtoets.Storage.Create.DuneErosion;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;

namespace Application.Ringtoets.Storage.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationOutputCreateExtensionsTest
    {
        [Test]
        public void Create_DuneLocationOutputNull_ThrowsArgumentNullException()
        {
            // Setup
            DuneLocationOutput duneLocationOutput = null;

            // Call
            TestDelegate call = () => duneLocationOutput.Create();

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("output", paramName);
        }

        [Test]
        public void Create_WithValidParameters_ReturnsDuneLocationEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new DuneLocationOutput(random.NextEnumValue<CalculationConvergence>(), new DuneLocationOutput.ConstructionProperties
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
            DuneLocationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(output.WaterLevel, entity.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(output.WaveHeight, entity.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(output.WavePeriod, entity.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, entity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, entity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, entity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }

        [Test]
        public void Create_WithNaNParameters_ReturnsDuneLocationEntityWithOutputSet()
        {
            // Setup
            var random = new Random(21);
            var output = new DuneLocationOutput(random.NextEnumValue<CalculationConvergence>(), new DuneLocationOutput.ConstructionProperties
            {
                WaterLevel = RoundedDouble.NaN,
                WaveHeight = RoundedDouble.NaN,
                WavePeriod = RoundedDouble.NaN,
                TargetProbability = RoundedDouble.NaN,
                TargetReliability = RoundedDouble.NaN,
                CalculatedProbability = RoundedDouble.NaN,
                CalculatedReliability = RoundedDouble.NaN
            });

            // Call
            DuneLocationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.WaterLevel);
            Assert.IsNull(entity.WaveHeight);
            Assert.IsNull(entity.WavePeriod);
            Assert.IsNull(entity.TargetProbability);
            Assert.IsNull(entity.TargetReliability);
            Assert.IsNull(entity.CalculatedProbability);
            Assert.IsNull(entity.CalculatedReliability);
            Assert.AreEqual((byte) output.CalculationConvergence, entity.CalculationConvergence);
        }
    }
}