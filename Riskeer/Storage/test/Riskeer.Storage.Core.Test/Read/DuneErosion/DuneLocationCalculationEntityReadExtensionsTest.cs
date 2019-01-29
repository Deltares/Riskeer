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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.DuneErosion;

namespace Riskeer.Storage.Core.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());

            // Call
            TestDelegate call = () => ((DuneLocationCalculationEntity) null).Read(calculation);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new DuneLocationCalculationEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Read_CalculationEntityWithOutput_SetsDuneLocationCalculationWithoutOutput()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());

            var entity = new DuneLocationCalculationEntity();

            // Call
            entity.Read(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void Read_CalculationEntityWithOutput_SetsDuneLocationCalculationWithOutput()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());

            var random = new Random(21);
            double waterLevel = random.NextDouble();
            double waveHeight = random.NextDouble();
            double wavePeriod = random.NextDouble();
            double targetProbability = random.NextDouble();
            double targetReliability = random.NextDouble();
            double calculatedProbability = random.NextDouble();
            double calculatedReliability = random.NextDouble();
            var convergence = random.NextEnumValue<CalculationConvergence>();

            var entity = new DuneLocationCalculationEntity
            {
                DuneLocationCalculationOutputEntities =
                {
                    new DuneLocationCalculationOutputEntity
                    {
                        WaterLevel = waterLevel,
                        WaveHeight = waveHeight,
                        WavePeriod = wavePeriod,
                        TargetProbability = targetProbability,
                        TargetReliability = targetReliability,
                        CalculatedProbability = calculatedProbability,
                        CalculatedReliability = calculatedReliability,
                        CalculationConvergence = Convert.ToByte(convergence)
                    }
                }
            };

            // Call
            entity.Read(calculation);

            // Assert
            DuneLocationCalculationOutput output = calculation.Output;
            Assert.IsNotNull(output);
            Assert.AreEqual(waterLevel, output.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(waveHeight, output.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(wavePeriod, output.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(targetProbability, output.TargetProbability);
            Assert.AreEqual(targetReliability, output.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(calculatedProbability, output.CalculatedProbability);
            Assert.AreEqual(calculatedReliability, output.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(convergence, output.CalculationConvergence);
        }
    }
}