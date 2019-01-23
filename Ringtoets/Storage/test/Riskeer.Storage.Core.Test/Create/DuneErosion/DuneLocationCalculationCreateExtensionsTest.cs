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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.DuneErosion;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((DuneLocationCalculation) null).Create(new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var calculation = new DuneLocationCalculation(new TestDuneLocation());

            // Call
            TestDelegate call = () => calculation.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_CalculationWithoutOutput_ReturnsDuneLocationCalculationEntity()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var calculation = new DuneLocationCalculation(duneLocation);

            var registry = new PersistenceRegistry();
            var duneLocationEntity = new DuneLocationEntity();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            DuneLocationCalculationEntity entity = calculation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            CollectionAssert.IsEmpty(entity.DuneLocationCalculationOutputEntities);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnsDuneLocationCalculationEntityWithOutput()
        {
            // Setup
            var random = new Random(21);
            var output = new DuneLocationCalculationOutput(random.NextEnumValue<CalculationConvergence>(), new DuneLocationCalculationOutput.ConstructionProperties
            {
                WaterLevel = random.NextDouble(),
                WaveHeight = random.NextDouble(),
                WavePeriod = random.NextDouble(),
                CalculatedProbability = random.NextDouble(),
                CalculatedReliability = random.NextDouble(),
                TargetProbability = random.NextDouble(),
                TargetReliability = random.NextDouble()
            });

            var duneLocation = new TestDuneLocation();
            var calculation = new DuneLocationCalculation(duneLocation)
            {
                Output = output
            };

            var registry = new PersistenceRegistry();
            var duneLocationEntity = new DuneLocationEntity();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            DuneLocationCalculationEntity entity = calculation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);

            DuneLocationCalculationOutputEntity outputEntity = entity.DuneLocationCalculationOutputEntities.Single();
            Assert.AreEqual(output.WaterLevel, outputEntity.WaterLevel, output.WaterLevel.GetAccuracy());
            Assert.AreEqual(output.WaveHeight, outputEntity.WaveHeight, output.WaveHeight.GetAccuracy());
            Assert.AreEqual(output.WavePeriod, outputEntity.WavePeriod, output.WavePeriod.GetAccuracy());
            Assert.AreEqual(output.TargetProbability, outputEntity.TargetProbability);
            Assert.AreEqual(output.TargetReliability, outputEntity.TargetReliability, output.TargetReliability.GetAccuracy());
            Assert.AreEqual(output.CalculatedProbability, outputEntity.CalculatedProbability);
            Assert.AreEqual(output.CalculatedReliability, outputEntity.CalculatedReliability, output.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(Convert.ToByte(output.CalculationConvergence), outputEntity.CalculationConvergence);
        }

        [Test]
        public void Create_CalculationWithAlreadyRegisteredDuneLocation_ReturnsEntityWithDuneLocationEntity()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var calculation = new DuneLocationCalculation(duneLocation);

            var registry = new PersistenceRegistry();
            var duneLocationEntity = new DuneLocationEntity();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            DuneLocationCalculationEntity entity = calculation.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreSame(duneLocationEntity, entity.DuneLocationEntity);
        }
    }
}