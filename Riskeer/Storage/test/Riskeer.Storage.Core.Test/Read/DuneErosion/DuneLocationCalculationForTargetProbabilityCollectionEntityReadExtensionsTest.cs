// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Read.DuneErosion;

namespace Riskeer.Storage.Core.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationForTargetProbabilityCollectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((DuneLocationCalculationForTargetProbabilityCollectionEntity) null).Read(new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new DuneLocationCalculationForTargetProbabilityCollectionEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithValidValues_SetsCalculationsWithExpectedValues()
        {
            // Setup
            var random = new Random(21);

            var duneLocationEntityOne = new DuneLocationEntity();
            var calculationEntityWithoutOutput = new DuneLocationCalculationEntity
            {
                DuneLocationEntity = duneLocationEntityOne
            };

            var duneLocationEntityTwo = new DuneLocationEntity();
            var calculationEntityWithOutput = new DuneLocationCalculationEntity
            {
                DuneLocationEntity = duneLocationEntityTwo,
                DuneLocationCalculationOutputEntities =
                {
                    new DuneLocationCalculationOutputEntity()
                }
            };

            var collectionEntity = new DuneLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = random.NextDouble(0, 0.1),
                DuneLocationCalculationEntities =
                {
                    calculationEntityWithoutOutput,
                    calculationEntityWithOutput
                }
            };

            var duneLocationOne = new TestDuneLocation("1");
            var duneLocationTwo = new TestDuneLocation("2");
            var collector = new ReadConversionCollector();
            collector.Read(duneLocationEntityOne, duneLocationOne);
            collector.Read(duneLocationEntityTwo, duneLocationTwo);

            // Call
            DuneLocationCalculationsForTargetProbability calculations = collectionEntity.Read(collector);

            // Assert
            Assert.AreEqual(collectionEntity.TargetProbability, calculations.TargetProbability);

            IEnumerable<DuneLocationCalculation> duneLocationCalculations = calculations.DuneLocationCalculations;
            Assert.AreEqual(collectionEntity.DuneLocationCalculationEntities.Count, duneLocationCalculations.Count());
         
            DuneLocationCalculation calculationOne = duneLocationCalculations.ElementAt(0);
            Assert.AreSame(collector.Get(duneLocationEntityOne), calculationOne.DuneLocation);
            Assert.IsNull(calculationOne.Output);

            DuneLocationCalculation calculationTwo = duneLocationCalculations.ElementAt(1);
            Assert.AreSame(collector.Get(duneLocationEntityTwo), calculationTwo.DuneLocation);
            Assert.IsNotNull(calculationTwo.Output);
        }
    }
}