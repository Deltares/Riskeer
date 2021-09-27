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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class HydraulicLocationCalculationForTargetProbabilityCollectionCreateExtensionTest
    {
        [Test]
        public void Create_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var calculationType = random.NextEnumValue<HydraulicBoundaryLocationCalculationType>();

            // Call
            void Call() => ((HydraulicBoundaryLocationCalculationsForTargetProbability) null).Create(calculationType, 
                                                                                                     random.Next(),
                                                                                                     new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var calculationType = random.NextEnumValue<HydraulicBoundaryLocationCalculationType>();
            var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1));

            // Call
            void Call() => calculations.Create(calculationType, random.Next(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidCollection_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();
            var calculationType = random.NextEnumValue<HydraulicBoundaryLocationCalculationType>();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = random.NextBoolean()
                }
            };

            var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
            {
                HydraulicBoundaryLocationCalculations =
                {
                    calculation
                }
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            HydraulicLocationCalculationForTargetProbabilityCollectionEntity entity = calculations.Create(calculationType, order, registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(calculations.TargetProbability, entity.TargetProbability);
            Assert.AreEqual(Convert.ToByte(calculationType), entity.HydraulicBoundaryLocationCalculationType);

            HydraulicLocationCalculationEntity hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationEntities.Single();
            Assert.AreSame(hydraulicLocationEntity, hydraulicLocationCalculationEntity.HydraulicLocationEntity);
            Assert.AreEqual(Convert.ToByte(calculation.InputParameters.ShouldIllustrationPointsBeCalculated),
                            hydraulicLocationCalculationEntity.ShouldIllustrationPointsBeCalculated);
            CollectionAssert.IsEmpty(hydraulicLocationCalculationEntity.HydraulicLocationOutputEntities);
        }

        [Test]
        public void Create_HydraulicLocationCalculationForTargetProbabilitySavedMultipleTimes_ReturnsSameEntity()
        {
            // Setup
            var random = new Random(21);
            var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1));

            var registry = new PersistenceRegistry();
            
            // Call
            HydraulicLocationCalculationForTargetProbabilityCollectionEntity entityOne = 
                calculations.Create(random.NextEnumValue<HydraulicBoundaryLocationCalculationType>(), random.Next(), registry);
            HydraulicLocationCalculationForTargetProbabilityCollectionEntity entityTwo = 
                calculations.Create(random.NextEnumValue<HydraulicBoundaryLocationCalculationType>(), random.Next(), registry);

            // Assert
            Assert.AreSame(entityOne, entityTwo);
        }
    }
}