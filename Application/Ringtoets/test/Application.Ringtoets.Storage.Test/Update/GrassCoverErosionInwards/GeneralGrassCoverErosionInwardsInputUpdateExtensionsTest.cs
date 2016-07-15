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

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Update.GrassCoverErosionInwards
{
    [TestFixture]
    public class GeneralGrassCoverErosionInwardsInputUpdateExtensionsTest
    {
        [Test]
        public void Update_RingtoetsEntitiesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var input = new GeneralGrassCoverErosionInwardsInput();
            var registry = new PersistenceRegistry();

            // Call
            TestDelegate call = () => input.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var input = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate call = () => input.Update(null, ringtoetsEntities);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithoutGrassCoverErosionInwardsFailureMechanismMetaEntity_EntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();

            var input = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate call = () => input.Update(registry, ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'GrassCoverErosionInwardsFailureMechanismMetaEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoGrassCoverErosionInwardsFailureMechanismMetaEntityWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var storageId = 1;
            var input = new GeneralGrassCoverErosionInwardsInput
            {
                StorageId = storageId
            };

            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                FailureMechanismEntityId = 2
            });

            // Call
            TestDelegate test = () => input.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'GrassCoverErosionInwardsFailureMechanismMetaEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithGrassCoverErosionInwardsFailureMechanismMetaEntity_PropertiesUpdated(
            [Random(1, 20, 1)]int n)
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            const int storageId = 1;
            var input = new GeneralGrassCoverErosionInwardsInput
            {
                N = n,
                StorageId = storageId
            };

            var entity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = input.StorageId,
            };
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(entity);

            var registry = new PersistenceRegistry();

            // Call
            input.Update(registry, ringtoetsEntities);

            // Assert
            Assert.AreEqual(n, entity.N);

            registry.RemoveUntouched(ringtoetsEntities);
            CollectionAssert.Contains(ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities, entity);

            mocks.VerifyAll();
        }
    }
}