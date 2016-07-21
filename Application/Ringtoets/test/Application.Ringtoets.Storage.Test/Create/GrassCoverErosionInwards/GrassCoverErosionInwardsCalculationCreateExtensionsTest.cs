// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;

using NUnit.Framework;

using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => calculation.Create(null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        [TestCase("I have no comments", null, 0)]
        [TestCase("I do have a comment", "I am comment", 98)]
        public void Create_ValidCalculation_ReturnEntity(string name, string comment, int order)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Name = name,
                Comments = comment,
                Output = null
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, order);

            // Assert
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(comment, entity.Comments);
            Assert.AreEqual(order, entity.Order);

            Assert.IsNull(entity.GrassCoverErosionInwardsOutputEntity);
        }

        [Test]
        public void Create_ValidCalculation_EntityIsRegisteredInPersistenceRegistry()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            entity.GrassCoverErosionInwardsCalculationEntityId = 8734;
            registry.TransferIds();
            Assert.AreEqual(entity.GrassCoverErosionInwardsCalculationEntityId, calculation.StorageId);
        }

        [Test]
        public void Create_CalculationWithOutput_ReturnEntity()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(1, true, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1), 2)
            };

            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsCalculationEntity entity = calculation.Create(registry, 0);

            // Assert
            Assert.IsNotNull(entity.GrassCoverErosionInwardsOutputEntity);
        }
    }
}