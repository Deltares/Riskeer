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

using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Update.GrassCoverErosionInwards
{
    [TestFixture]
    public class DikeProfileUpdateExtensionsTest
    {
        [Test]
        public void Update_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var ringtoetsEntities = mocks.Stub<IRingtoetsEntities>();
            mocks.ReplayAll();

            DikeProfile profile = CreateSimpleDikeProfile();

            // Call
            TestDelegate call = () => profile.Update(null, ringtoetsEntities);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_RingtoetsEntitiesNull_ThrowArgumentNullException()
        {
            // Setup
            var registry = new PersistenceRegistry();
            DikeProfile profile = CreateSimpleDikeProfile();

            // Call
            TestDelegate call = () => profile.Update(registry, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_ContextWithNoDikeProfileEntity_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();
            DikeProfile profile = CreateSimpleDikeProfile();

            // Call
            TestDelegate call = () => profile.Update(registry, ringtoetsEntities);

            // Assert
            string expectedMessage = string.Format("Het object 'DikeProfileEntity' met id '{0}' is niet gevonden.",
                                                   profile.StorageId);
            var exception = Assert.Throws<EntityNotFoundException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNoDikeProfileEntityWithId_ThrowEntityNotFoundException()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();
            DikeProfile profile = CreateSimpleDikeProfile();

            ringtoetsEntities.DikeProfileEntities.Add(new DikeProfileEntity
            {
                DikeProfileEntityId = 1
            });

            // Call
            TestDelegate call = () => profile.Update(registry, ringtoetsEntities);

            // Assert
            string expectedMessage = string.Format("Het object 'DikeProfileEntity' met id '{0}' is niet gevonden.",
                                                   profile.StorageId);
            var exception = Assert.Throws<EntityNotFoundException>(call);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithOrphanEntity_OrphanIsUntouchedOnUpdate()
        {
            // Setup
            var mocks = new MockRepository();
            IRingtoetsEntities ringtoetsEntitites = RingtoetsEntitiesHelper.CreateStub(mocks);
            mocks.ReplayAll();

            var registry = new PersistenceRegistry();
            DikeProfile profile = CreateSimpleDikeProfile();

            var orphanedEntity = new DikeProfileEntity
            {
                DikeProfileEntityId = 1
            };
            var persistentEntity = new DikeProfileEntity
            {
                DikeProfileEntityId = profile.StorageId
            };

            ringtoetsEntitites.DikeProfileEntities.Add(orphanedEntity);
            ringtoetsEntitites.DikeProfileEntities.Add(persistentEntity);

            // Call
            profile.Update(registry, ringtoetsEntitites);

            // Assert
            registry.RemoveUntouched(ringtoetsEntitites);
            CollectionAssert.Contains(ringtoetsEntitites.DikeProfileEntities, persistentEntity);
            CollectionAssert.DoesNotContain(ringtoetsEntitites.DikeProfileEntities, orphanedEntity);
            mocks.VerifyAll();
        }

        private static DikeProfile CreateSimpleDikeProfile()
        {
            return new DikeProfile(new Point2D(0, 0),
                                   new[]
                                   {
                                       new RoughnessPoint(new Point2D(1, 2), 0.75),
                                       new RoughnessPoint(new Point2D(3, 4), 0.75)
                                   },
                                   new[]
                                   {
                                       new Point2D(0, 0),
                                       new Point2D(1, 2)
                                   },
                                   null, new DikeProfile.ConstructionProperties())
            {
                StorageId = 49654
            };
        }
    }
}