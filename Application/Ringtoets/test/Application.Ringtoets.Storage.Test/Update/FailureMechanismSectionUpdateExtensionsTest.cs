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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class FailureMechanismSectionUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ArgumentNullException()
        {
            // Setup
            var section = new TestFailureMechanismSection();

            // Call
            TestDelegate test = () => section.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ArgumentNullException()
        {
            // Setup
            var section = new TestFailureMechanismSection();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    section.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoFailureMechanismSection_EntityNotFoundException()
        {
            // Setup
            var section = new TestFailureMechanismSection();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    section.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismSectionEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoFailureMechanismSectionWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var section = new TestFailureMechanismSection
            {
                StorageId = storageId
            };

            ringtoetsEntities.FailureMechanismSectionEntities.Add(new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 2
            });

            // Call
            TestDelegate test = () => section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismSectionEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_FailureMechanismSectionWithNewGeometry_GeometryAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var points = new[]
            {
                new Point2D(1, 2)
            };
            var section = new FailureMechanismSection("", points)
            {
                StorageId = 1
            };

            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1,
                FailureMechanismSectionPointData = new Point2DBinaryConverter().ToBytes(points)
            };
            ringtoetsEntities.FailureMechanismSectionEntities.Add(entity);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            byte[] expectedBinaryData = new Point2DBinaryConverter().ToBytes(points);
            CollectionAssert.AreEqual(expectedBinaryData, entity.FailureMechanismSectionPointData);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_FailureMechanismSectionWithExistingGeometry_PointsRemovedFromContextAndNewPointAddedToEntity()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();
            
            var points = new[]
            {
                new Point2D(1, 2)
            };
            var section = new FailureMechanismSection("", points)
            {
                StorageId = 1
            };

            var binaryConverter = new Point2DBinaryConverter();
            var entity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1,
                FailureMechanismSectionPointData = binaryConverter.ToBytes(new[]{new Point2D(2,3)})
            };
            ringtoetsEntities.FailureMechanismSectionEntities.Add(entity);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            byte[] expectedBinaryData = binaryConverter.ToBytes(points);
            CollectionAssert.AreEqual(expectedBinaryData, entity.FailureMechanismSectionPointData);

            mocks.VerifyAll();
        }
         
    }
}