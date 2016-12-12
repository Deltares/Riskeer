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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.WaveImpactAsphaltCover;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Test.Read.WaveImpactAsphaltCover
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnOutputWithValues()
        {
            // Setup
            var random = new Random(12);
            var entity = new WaveImpactAsphaltCoverWaveConditionsOutputEntity
            {
                WaterLevel = random.NextDouble(),
                WaveHeight = random.NextDouble(),
                WavePeakPeriod = random.NextDouble(),
                WaveAngle = random.NextDouble(),
                WaveDirection = random.NextDouble(),
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                CalculationConvergence = (byte) CalculationConvergence.CalculatedConverged
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

            Assert.IsNotNull(entity.WaveDirection);
            Assert.AreEqual(entity.WaveDirection.Value, output.WaveDirection, output.WaveDirection.GetAccuracy());

            Assert.IsNotNull(entity.TargetProbability);
            Assert.AreEqual(entity.TargetProbability.Value, output.TargetProbability);

            Assert.IsNotNull(entity.TargetReliability);
            Assert.AreEqual(entity.TargetReliability.Value, output.TargetReliability, output.TargetReliability.GetAccuracy());

            Assert.IsNotNull(entity.CalculatedProbability);
            Assert.AreEqual(entity.CalculatedProbability.Value, output.CalculatedProbability);

            Assert.IsNotNull(entity.CalculatedReliability);
            Assert.AreEqual(entity.CalculatedReliability.Value, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());

            Assert.IsNotNull(entity.CalculationConvergence);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, output.CalculationConvergence);
        }

        [Test]
        public void Read_ValidEntityWithNullParameterValues_ReturnOutputWithNaNValues()
        {
            // Setup
            var entity = new WaveImpactAsphaltCoverWaveConditionsOutputEntity
            {
                CalculationConvergence = Convert.ToByte(CalculationConvergence.NotCalculated),
                WaterLevel = null,
                WaveHeight = null,
                WavePeakPeriod = null,
                WaveAngle = null,
                WaveDirection = null,
                TargetProbability = null,
                TargetReliability = null,
                CalculatedProbability = null,
                CalculatedReliability = null,
            };

            // Call
            WaveConditionsOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.WaterLevel);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeakPeriod);
            Assert.IsNaN(output.WaveAngle);
            Assert.IsNaN(output.WaveDirection);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
        }
    }
}