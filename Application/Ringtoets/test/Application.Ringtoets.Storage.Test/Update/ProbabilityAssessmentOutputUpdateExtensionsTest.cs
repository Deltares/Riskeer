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
using Application.Ringtoets.Storage.Update;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Probability;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class ProbabilityAssessmentOutputUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);

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
            var output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);

            // Call
            TestDelegate call = () => output.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_RingtoetsEntitiesWithoutProbabilisticOutputEntities_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();

            var output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1);

            // Call
            TestDelegate call = () => output.Update(registry, ringtoetsEntities);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
        }

        [Test]
        public void Update_RingtoetsEntitiesWithUnmatchingProbabilisticOutputEntity_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var entity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 12
            };
            ringtoetsEntities.ProbabilisticOutputEntities.Add(entity);

            var registry = new PersistenceRegistry();

            var output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)
            {
                StorageId = 2
            };

            // Call
            TestDelegate call = () => output.Update(registry, ringtoetsEntities);

            // Assert
            Assert.Throws<EntityNotFoundException>(call);
        }

        [Test]
        public void Update_RingtoetsEntitiesWithMatchingProbabilisticOutputEntity_RegisterEntity()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var persistentEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 12
            };
            var orphanedEntity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 17
            };
            ringtoetsEntities.ProbabilisticOutputEntities.Add(persistentEntity);
            ringtoetsEntities.ProbabilisticOutputEntities.Add(orphanedEntity);

            var registry = new PersistenceRegistry();

            var output = new ProbabilityAssessmentOutput(1, 1, 1, 1, 1)
            {
                StorageId = persistentEntity.ProbabilisticOutputEntityId
            };

            // Call
            output.Update(registry, ringtoetsEntities);

            // Assert
            registry.RemoveUntouched(ringtoetsEntities);
            CollectionAssert.Contains(ringtoetsEntities.ProbabilisticOutputEntities, persistentEntity);
            CollectionAssert.DoesNotContain(ringtoetsEntities.ProbabilisticOutputEntities, orphanedEntity);
        }
    }
}