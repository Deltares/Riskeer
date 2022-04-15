﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class HydraulicLocationCalculationForTargetProbabilityCollectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((HydraulicLocationCalculationForTargetProbabilityCollectionEntity) null).Read(new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicLocationCalculationForTargetProbabilityCollectionEntity();

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

            var collectionEntity = new HydraulicLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = random.NextDouble(0, 0.1),
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

            // Call
            HydraulicBoundaryLocationCalculationsForTargetProbability calculations = collectionEntity.Read(collector);

            // Assert
            Assert.AreEqual(collectionEntity.TargetProbability, calculations.TargetProbability);

            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = calculations.HydraulicBoundaryLocationCalculations;
            Assert.AreEqual(collectionEntity.HydraulicLocationCalculationEntities.Count, hydraulicBoundaryLocationCalculations.Count());

            HydraulicBoundaryLocationCalculation calculationOne = hydraulicBoundaryLocationCalculations.ElementAt(0);
            Assert.AreEqual(Convert.ToBoolean(calculationEntityWithoutOutput.ShouldIllustrationPointsBeCalculated),
                            calculationOne.InputParameters.ShouldIllustrationPointsBeCalculated);
            Assert.AreSame(hydraulicBoundaryLocationOne, calculationOne.HydraulicBoundaryLocation);
            Assert.IsNull(calculationOne.Output);

            HydraulicBoundaryLocationCalculation calculationTwo = hydraulicBoundaryLocationCalculations.ElementAt(1);
            Assert.AreEqual(Convert.ToBoolean(calculationEntityWithOutput.ShouldIllustrationPointsBeCalculated),
                            calculationTwo.InputParameters.ShouldIllustrationPointsBeCalculated);
            Assert.AreSame(hydraulicBoundaryLocationTwo, calculationTwo.HydraulicBoundaryLocation);
            Assert.IsNotNull(calculationTwo.Output);
        }

        [Test]
        public void Read_SameHydraulicLocationCalculationForTargetProbabilityCollectionEntityTwice_ReturnsSameHydraulicBoundaryLocationCalculationsForTargetProbability()
        {
            // Setup
            var random = new Random(21);
            var collectionEntity = new HydraulicLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = random.NextDouble(0, 0.1)
            };

            var collector = new ReadConversionCollector();

            // Call
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsOne = collectionEntity.Read(collector);
            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsTwo = collectionEntity.Read(collector);

            // Assert
            Assert.AreSame(calculationsOne, calculationsTwo);
        }
    }
}