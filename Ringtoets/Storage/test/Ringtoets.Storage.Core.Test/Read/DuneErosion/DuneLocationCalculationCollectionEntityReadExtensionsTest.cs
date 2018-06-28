﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;

namespace Ringtoets.Storage.Core.Test.Read.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationCollectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ((DuneLocationCalculationCollectionEntity) null).Read(Enumerable.Empty<DuneLocationCalculation>(),
                                                                      new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new DuneLocationCalculationCollectionEntity();

            // Call
            TestDelegate call = () => entity.Read(null, new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new DuneLocationCalculationCollectionEntity();

            // Call
            TestDelegate call = () => entity.Read(Enumerable.Empty<DuneLocationCalculation>(),
                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithValidValues_SetsCalculationsWithExpectedValues()
        {
            // Setup
            DuneLocationEntity duneLocationEntityOne = CreateDuneLocationEntity();
            var calculationEntityWithoutOutput = new DuneLocationCalculationEntity
            {
                DuneLocationEntity = duneLocationEntityOne
            };

            DuneLocationEntity duneLocationEntityTwo = CreateDuneLocationEntity();
            var calculationEntityWithOutput = new DuneLocationCalculationEntity
            {
                DuneLocationEntity = duneLocationEntityTwo,
                DuneLocationCalculationOutputEntities =
                {
                    new DuneLocationCalculationOutputEntity()
                }
            };

            var collectionEntity = new DuneLocationCalculationCollectionEntity
            {
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

            var calculationOne = new DuneLocationCalculation(duneLocationOne);
            var calculationTwo = new DuneLocationCalculation(duneLocationTwo);

            // Call
            collectionEntity.Read(new[]
            {
                calculationOne,
                calculationTwo
            }, collector);

            // Assert
            Assert.IsNull(calculationOne.Output);
            Assert.IsNotNull(calculationTwo.Output);
        }

        private static DuneLocationEntity CreateDuneLocationEntity()
        {
            return new DuneLocationEntity
            {
                LocationId = 1,
                Name = "Dune Location",
                LocationX = 1,
                LocationY = 2
            };
        }
    }
}