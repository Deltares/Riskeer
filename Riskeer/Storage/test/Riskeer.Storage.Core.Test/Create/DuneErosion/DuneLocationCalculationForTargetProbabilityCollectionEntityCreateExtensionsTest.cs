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
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.DuneErosion;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationForTargetProbabilityCollectionEntityCreateExtensionsTest
    {
        [Test]
        public void Create_CalculationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            void Call() => ((DuneLocationCalculationsForTargetProbability) null).Create(random.Next(),
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
            var calculations = new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1));

            // Call
            void Call() => calculations.Create(random.Next(), null);

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

            var duneLocation = new TestDuneLocation();
            var calculation = new DuneLocationCalculation(duneLocation);

            var calculations = new DuneLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
            {
                DuneLocationCalculations =
                {
                    calculation
                }
            };

            var duneLocationEntity = new DuneLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            DuneLocationCalculationForTargetProbabilityCollectionEntity entity = calculations.Create(order, registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(order, entity.Order);
            Assert.AreEqual(calculations.TargetProbability, entity.TargetProbability);

            DuneLocationCalculationEntity duneLocationCalculationEntity = entity.DuneLocationCalculationEntities.Single();
            Assert.AreSame(duneLocationEntity, duneLocationCalculationEntity.DuneLocationEntity);
            CollectionAssert.IsEmpty(duneLocationCalculationEntity.DuneLocationCalculationOutputEntities);
        }
    }
}