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
using Application.Ringtoets.Storage.Read.DuneErosion;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;

namespace Application.Ringtoets.Storage.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidParameters_ReturnsDuneLocationOutput()
        {
            // Setup
            var random = new Random(22);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var entity = new DuneLocationOutputEntity
            {
                WaterLevel = waterLevel,
                WaveHeight = waveHeight,
                WavePeriod = wavePeriod,
                TargetProbability = targetProbability,
                TargetReliability = targetReliability,
                CalculatedProbability = calculatedProbability,
                CalculatedReliability = calculatedReliability,
                CalculationConvergence = (byte) convergence
            };

            // Call
            DuneLocationOutput output = entity.Read();

            // Assert
            Assert.AreEqual((RoundedDouble) waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual((RoundedDouble) waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual((RoundedDouble) wavePeriod, output.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual((RoundedDouble) targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual((RoundedDouble) calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }

        [Test]
        public void Read_NullParameters_ReturnsDuneLocationOutputWithNaN()
        {
            // Setup
            var convergence = new Random(36).NextEnumValue<CalculationConvergence>();
            var entity = new DuneLocationOutputEntity
            {
                WaterLevel = null,
                WaveHeight = null,
                WavePeriod = null,
                TargetProbability = null,
                TargetReliability = null,
                CalculatedProbability = null,
                CalculatedReliability = null,
                CalculationConvergence = (byte) convergence
            };

            // Call
            DuneLocationOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.WaterLevel);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeriod);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}