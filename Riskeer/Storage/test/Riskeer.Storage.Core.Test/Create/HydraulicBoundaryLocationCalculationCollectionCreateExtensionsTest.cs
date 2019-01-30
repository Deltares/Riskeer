// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationCalculationCollectionCreateExtensionsTest
    {
        [Test]
        public void Create_CalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                ((IEnumerable<HydraulicBoundaryLocationCalculation>) null).Create(new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("calculations", exception.ParamName);
        }

        [Test]
        public void Create_PersistenceRegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations =
                Enumerable.Empty<HydraulicBoundaryLocationCalculation>();

            // Call
            TestDelegate call = () => calculations.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidCollection_ReturnsEntityWithExpectedResults()
        {
            // Setup
            var random = new Random(21);

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation)
            {
                InputParameters =
                {
                    ShouldIllustrationPointsBeCalculated = random.NextBoolean()
                }
            };

            var calculationCollection = new ObservableList<HydraulicBoundaryLocationCalculation>
            {
                calculation
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity();
            var registry = new PersistenceRegistry();
            registry.Register(hydraulicLocationEntity, hydraulicBoundaryLocation);

            // Call
            HydraulicLocationCalculationCollectionEntity entity = calculationCollection.Create(registry);

            // Assert
            Assert.IsNotNull(entity);

            HydraulicLocationCalculationEntity hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationEntities.Single();
            Assert.AreSame(hydraulicLocationEntity, hydraulicLocationCalculationEntity.HydraulicLocationEntity);
            Assert.AreEqual(Convert.ToByte(calculation.InputParameters.ShouldIllustrationPointsBeCalculated),
                            hydraulicLocationCalculationEntity.ShouldIllustrationPointsBeCalculated);
            CollectionAssert.IsEmpty(hydraulicLocationCalculationEntity.HydraulicLocationOutputEntities);
        }
    }
}