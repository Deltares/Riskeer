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
    public class GrassCoverErosionInwardsOutputUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var output = new GrassCoverErosionInwardsOutput(1, true, new ProbabilityAssessmentOutput(1,1,1,1,1), 1);

            // Call
            TestDelegate call = () => output.Update(null, ringtoetsEntities);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            var output = new GrassCoverErosionInwardsOutput(1, true, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1), 1);

            // Call
            TestDelegate call = () => output.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_RingtoetsEntitiesWithoutGrassCoverErosionOutputEntities_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();

            var output = new GrassCoverErosionInwardsOutput(1, true, new ProbabilityAssessmentOutput(1, 1, 1, 1, 1), 1);

            // Call
            TestDelegate call = () => output.Update(registry, ringtoetsEntities);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
        }

        [Test]
        public void Update_RingtoetsEntitiesWithUnmatchingGrassCoverErosionOutputEntity_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var entity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 23
            };
            var probabilisticOutputEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 12
            };
            ringtoetsEntities.GrassCoverErosionInwardsOutputEntities.Add(entity);
            ringtoetsEntities.ProbabilisticOutputEntities.Add(probabilisticOutputEntity);

            var registry = new PersistenceRegistry();

            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)
            {
                StorageId = 2
            };
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, 1)
            {
                StorageId = probabilisticOutputEntity.ProbabilisticOutputEntityId
            };

            // Call
            TestDelegate call = () => output.Update(registry, ringtoetsEntities);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
        }

        [Test]
        public void Update_RingtoetsEntitiesWithMatchingGrassCoverErosionInwardsOutputEntity_RegisterEntity()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var persistentEntity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 23
            };
            var persistentProbabilisticOutputEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 12
            };
            var orphanedEntity = new GrassCoverErosionInwardsOutputEntity
            {
                GrassCoverErosionInwardsOutputId = 42
            };
            var orphanedProbabilisticOutputEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 15
            };
            ringtoetsEntities.GrassCoverErosionInwardsOutputEntities.Add(persistentEntity);
            ringtoetsEntities.GrassCoverErosionInwardsOutputEntities.Add(orphanedEntity);
            ringtoetsEntities.ProbabilisticOutputEntities.Add(persistentProbabilisticOutputEntity);
            ringtoetsEntities.ProbabilisticOutputEntities.Add(orphanedProbabilisticOutputEntity);

            var registry = new PersistenceRegistry();

            var probabilityAssessmentOutput = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)
            {
                StorageId = persistentProbabilisticOutputEntity.ProbabilisticOutputEntityId
            };
            var output = new GrassCoverErosionInwardsOutput(1, true, probabilityAssessmentOutput, 1)
            {
                StorageId = persistentEntity.GrassCoverErosionInwardsOutputId
            };

            // Call
            output.Update(registry, ringtoetsEntities);

            // Assert
            registry.RemoveUntouched(ringtoetsEntities);
            CollectionAssert.Contains(ringtoetsEntities.GrassCoverErosionInwardsOutputEntities, persistentEntity);
            CollectionAssert.Contains(ringtoetsEntities.ProbabilisticOutputEntities, persistentProbabilisticOutputEntity);
            CollectionAssert.DoesNotContain(ringtoetsEntities.GrassCoverErosionInwardsOutputEntities, orphanedEntity);
            CollectionAssert.DoesNotContain(ringtoetsEntities.ProbabilisticOutputEntities, orphanedProbabilisticOutputEntity);
        }
    }
}