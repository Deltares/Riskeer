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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update.GrassCoverErosionInwards;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Update.GrassCoverErosionInwards
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => calculation.Update(null, ringtoetsEntities, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => calculation.Update(registry, null, 0);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_RingtoetsEntitiesHasNoCalculations_ThrowsEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            TestDelegate call = () => calculation.Update(registry, ringtoetsEntities, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesHasNoMatchingCalculation_ThrowsEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities.Add(new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 1
            });

            var registry = new PersistenceRegistry();
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                StorageId = 2
            };

            // Call
            TestDelegate call = () => calculation.Update(registry, ringtoetsEntities, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesWithMatchingCalculation_UpdateAndRegisterEntity()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                StorageId = 2,
                Name = "Calculation yeah!",
                Comments = "Comments whoo!"
            };

            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = calculation.StorageId,
                Name = "A",
                Comments = "B",
                Order = 3
            };
            var orphanedEntity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = 564
            };
            ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities.Add(entity);
            ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities.Add(orphanedEntity);

            var registry = new PersistenceRegistry();

            const int order = 86;

            // Call
            calculation.Update(registry, ringtoetsEntities, order);

            // Assert
            Assert.AreEqual(calculation.Name, entity.Name);
            Assert.AreEqual(calculation.Comments, entity.Comments);
            Assert.AreEqual(order, entity.Order);

            registry.RemoveUntouched(ringtoetsEntities);
            CollectionAssert.Contains(ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities, entity);
            CollectionAssert.DoesNotContain(ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities, orphanedEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_CalculationWithNewOutput_EntityHasOutputEntityAdded()
        {
            // Setup
            var mocks=  new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                StorageId = 2,
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = calculation.StorageId
            };
            ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            calculation.Update(registry, ringtoetsEntities, 0);

            // Assert
            Assert.IsNotNull(entity.GrassCoverErosionInwardsOutputEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_CalculationWithSaveOutput_OutputRegistered()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                StorageId = 2,
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
                {
                    StorageId = 456,
                    ProbabilityAssessmentOutput =
                    {
                        StorageId = 45
                    }
                }
            };

            var outputEntity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = calculation.Output.StorageId,
                ProbabilisticOutputEntity = new ProbabilisticOutputEntity
                {
                    ProbabilisticOutputEntityId = calculation.Output.ProbabilityAssessmentOutput.StorageId
                }
            };
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = calculation.StorageId,
                GrassCoverErosionInwardsOutputEntity = outputEntity
            };
            ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities.Add(entity);
            ringtoetsEntities.GrassCoverErosionInwardsOutputEntities.Add(outputEntity);
            ringtoetsEntities.ProbabilisticOutputEntities.Add(outputEntity.ProbabilisticOutputEntity);

            var registry = new PersistenceRegistry();

            // Call
            calculation.Update(registry, ringtoetsEntities, 0);

            // Assert
            registry.RemoveUntouched(ringtoetsEntities);
            CollectionAssert.Contains(ringtoetsEntities.GrassCoverErosionInwardsOutputEntities, outputEntity);
            CollectionAssert.Contains(ringtoetsEntities.ProbabilisticOutputEntities, outputEntity.ProbabilisticOutputEntity);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_CalculationWithRemovedOutput_EntityHasOutputEntityNull()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                StorageId = 2,
                Output = null
            };

            var outputEntity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 3,
                ProbabilisticOutputEntity = new ProbabilisticOutputEntity
                {
                    ProbabilisticOutputEntityId = 12
                }
            };
            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                GrassCoverErosionInwardsCalculationEntityId = calculation.StorageId,
                GrassCoverErosionInwardsOutputEntity = outputEntity
            };
            ringtoetsEntities.GrassCoverErosionInwardsCalculationEntities.Add(entity);
            ringtoetsEntities.GrassCoverErosionInwardsOutputEntities.Add(outputEntity);
            ringtoetsEntities.ProbabilisticOutputEntities.Add(outputEntity.ProbabilisticOutputEntity);

            var registry = new PersistenceRegistry();

            // Call
            calculation.Update(registry, ringtoetsEntities, 0);

            // Assert
            Assert.IsNull(entity.GrassCoverErosionInwardsOutputEntity);

            registry.RemoveUntouched(ringtoetsEntities);
            CollectionAssert.DoesNotContain(ringtoetsEntities.GrassCoverErosionInwardsOutputEntities, outputEntity);
            CollectionAssert.DoesNotContain(ringtoetsEntities.ProbabilisticOutputEntities, outputEntity.ProbabilisticOutputEntity);
            mocks.VerifyAll();
        }
    }
}