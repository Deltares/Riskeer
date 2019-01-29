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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.Create.DuneErosion;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create.DuneErosion
{
    [TestFixture]
    public class DuneLocationCalculationCollectionCreateExtensionsTest
    {
        [Test]
        public void Create_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ((IEnumerable<DuneLocationCalculation>) null).Create(new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<DuneLocationCalculation> calculations =
                Enumerable.Empty<DuneLocationCalculation>();

            // Call
            TestDelegate call = () => calculations.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidCollectionAndArguments_ReturnsEntity()
        {
            // Setup
            var duneLocation = new TestDuneLocation();
            var calculation = new DuneLocationCalculation(duneLocation);
            DuneLocationCalculation[] calculations =
            {
                calculation
            };

            var duneLocationEntity = new DuneLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(duneLocationEntity, duneLocation);

            // Call
            DuneLocationCalculationCollectionEntity entity = calculations.Create(registry);

            // Assert
            Assert.IsNotNull(entity);

            DuneLocationCalculationEntity calculationEntity = entity.DuneLocationCalculationEntities.Single();
            Assert.AreSame(duneLocationEntity, calculationEntity.DuneLocationEntity);
            CollectionAssert.IsEmpty(calculationEntity.DuneLocationCalculationOutputEntities);
        }
    }
}