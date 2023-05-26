// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.DuneErosion;

namespace Riskeer.Storage.Core.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((DuneLocationCalculationOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_ValidParameters_ReturnsDuneLocationOutput()
        {
            // Setup
            var random = new Random(22);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double meanTidalAmplitude = random.NextDouble();
            double waveDirectionalSpread = random.NextDouble();
            double tideSurgePhaseDifference = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();
            var entity = new DuneLocationCalculationOutputEntity
            {
                WaterLevel = waterLevel,
                WaveHeight = waveHeight,
                WavePeriod = wavePeriod,
                MeanTidalAmplitude = meanTidalAmplitude,
                WaveDirectionalSpread = waveDirectionalSpread,
                TideSurgePhaseDifference = tideSurgePhaseDifference,
                TargetProbability = targetProbability,
                TargetReliability = targetReliability,
                CalculatedProbability = calculatedProbability,
                CalculatedReliability = calculatedReliability,
                CalculationConvergence = Convert.ToByte(convergence)
            };

            // Call
            DuneLocationCalculationOutput output = entity.Read();

            // Assert
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeriod, output.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(meanTidalAmplitude, output.MeanTidalAmplitude, output.MeanTidalAmplitude.GetAccuracy());
            Assert.AreEqual(waveDirectionalSpread, output.WaveDirectionalSpread, output.WaveDirectionalSpread.GetAccuracy());
            Assert.AreEqual(tideSurgePhaseDifference, output.TideSurgePhaseDifference, output.TideSurgePhaseDifference.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }

        [Test]
        public void Read_NullParameters_ReturnsDuneLocationOutputWithNaN()
        {
            // Setup
            var convergence = new Random(36).NextEnumValue<CalculationConvergence>();
            var entity = new DuneLocationCalculationOutputEntity
            {
                WaterLevel = null,
                WaveHeight = null,
                WavePeriod = null,
                MeanTidalAmplitude = null,
                WaveDirectionalSpread = null,
                TideSurgePhaseDifference = null,
                TargetProbability = null,
                TargetReliability = null,
                CalculatedProbability = null,
                CalculatedReliability = null,
                CalculationConvergence = Convert.ToByte(convergence)
            };

            // Call
            DuneLocationCalculationOutput output = entity.Read();

            // Assert
            Assert.IsNaN(output.WaterLevel);
            Assert.IsNaN(output.WaveHeight);
            Assert.IsNaN(output.WavePeriod);
            Assert.IsNaN(output.MeanTidalAmplitude);
            Assert.IsNaN(output.WaveDirectionalSpread);
            Assert.IsNaN(output.TideSurgePhaseDifference);
            Assert.IsNaN(output.TargetProbability);
            Assert.IsNaN(output.TargetReliability);
            Assert.IsNaN(output.CalculatedProbability);
            Assert.IsNaN(output.CalculatedReliability);
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}