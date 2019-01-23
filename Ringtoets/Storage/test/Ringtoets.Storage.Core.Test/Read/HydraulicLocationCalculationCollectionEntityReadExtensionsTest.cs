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
using Ringtoets.Storage.Core.TestUtil.Hydraulics;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Ringtoets.Storage.Core.Test.Read
{
    [TestFixture]
    public class HydraulicLocationCalculationCollectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ((HydraulicLocationCalculationCollectionEntity) null).Read(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                                           new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicLocationCalculationCollectionEntity();

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
            var entity = new HydraulicLocationCalculationCollectionEntity();

            // Call
            TestDelegate call = () => entity.Read(Enumerable.Empty<HydraulicBoundaryLocationCalculation>(),
                                                  null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithValidValues_SetsCalculationsWithExpectedValues()
        {
            // Setup
            var random = new Random(21);

            HydraulicLocationEntity hydraulicLocationEntityOne = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var calculationEntityWithoutOutput = new HydraulicLocationCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntityOne,
                ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean())
            };

            HydraulicLocationEntity hydraulicLocationEntityTwo = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            var calculationEntityWithOutput = new HydraulicLocationCalculationEntity
            {
                HydraulicLocationEntity = hydraulicLocationEntityTwo,
                ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean()),
                HydraulicLocationOutputEntities =
                {
                    new HydraulicLocationOutputEntity()
                }
            };

            var collectionEntity = new HydraulicLocationCalculationCollectionEntity
            {
                HydraulicLocationCalculationEntities =
                {
                    calculationEntityWithoutOutput,
                    calculationEntityWithOutput
                }
            };

            var hydraulicBoundaryLocationOne = new TestHydraulicBoundaryLocation("1");
            var hydraulicBoundaryLocationTwo = new TestHydraulicBoundaryLocation("2");
            var collector = new ReadConversionCollector();
            collector.Read(hydraulicLocationEntityOne, hydraulicBoundaryLocationOne);
            collector.Read(hydraulicLocationEntityTwo, hydraulicBoundaryLocationTwo);

            var calculationOne = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocationOne);
            var calculationTwo = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocationTwo);

            // Call
            collectionEntity.Read(new[]
            {
                calculationOne,
                calculationTwo
            }, collector);

            // Assert
            Assert.AreEqual(Convert.ToBoolean(calculationEntityWithoutOutput.ShouldIllustrationPointsBeCalculated),
                            calculationOne.InputParameters.ShouldIllustrationPointsBeCalculated);
            Assert.IsNull(calculationOne.Output);

            Assert.AreEqual(Convert.ToBoolean(calculationEntityWithOutput.ShouldIllustrationPointsBeCalculated),
                            calculationTwo.InputParameters.ShouldIllustrationPointsBeCalculated);
            Assert.IsNotNull(calculationTwo.Output);
        }
    }
}