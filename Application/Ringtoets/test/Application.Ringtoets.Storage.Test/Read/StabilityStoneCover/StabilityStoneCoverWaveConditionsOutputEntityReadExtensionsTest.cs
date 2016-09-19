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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.StabilityStoneCover;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Test.Read.StabilityStoneCover
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnOutput()
        {
            // Setup
            var random = new Random(12);
            var entity = new StabilityStoneCoverWaveConditionsOutputEntity
            {
                WaterLevel = random.NextDouble(),
                WaveHeight = random.NextDouble(),
                WavePeakPeriod = random.NextDouble(),
                WaveAngle = random.NextDouble()
            };

            // Call
            WaveConditionsOutput output = entity.Read();

            // Assert
            Assert.IsNotNull(entity.WaterLevel);
            Assert.AreEqual(entity.WaterLevel.Value, output.WaterLevel, output.WaterLevel.GetAccuracy());

            Assert.IsNotNull(entity.WaveHeight);
            Assert.AreEqual(entity.WaveHeight.Value, output.WaveHeight, output.WaveHeight.GetAccuracy());

            Assert.IsNotNull(entity.WavePeakPeriod);
            Assert.AreEqual(entity.WavePeakPeriod.Value, output.WavePeakPeriod, output.WavePeakPeriod.GetAccuracy());

            Assert.IsNotNull(entity.WaveAngle);
            Assert.AreEqual(entity.WaveAngle.Value, output.WaveAngle, output.WaveAngle.GetAccuracy());
        }

        [Test]
        public void Read_ValidEntityWithNullParameterValues_ReturnPipingOutput()
        {
            // Setup
            var entity = new StabilityStoneCoverWaveConditionsOutputEntity
            {
                WaterLevel = null,
                WaveHeight = null,
                WavePeakPeriod = null,
                WaveAngle = null
            };

            // Call
            WaveConditionsOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.WaterLevel);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeakPeriod);
            Assert.IsNaN(output.WaveAngle);
        }
    }
}