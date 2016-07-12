﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.Update(null, ringtoetsEntities);
                }
            };

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Update_ContextWithNoGrassCoverErosionInwardsFailureMechanism_ThrowsEntityNotFoundException()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoGrassCoverErosionInwardsFailureMechanismWithId_ThrowsEntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = storageId
            };

            ringtoetsEntities.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = 2
            });

            // Call
            TestDelegate test = () => failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'FailureMechanismEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithGrassCoverErosionInwardsFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                IsRelevant = true,
                GeneralInput =
                {
                    StorageId = 62981,
                    N = 13
                }
            };

            var generalInputEntity = new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId,
                N = 2
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
                IsRelevant = Convert.ToByte(false),
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    generalInputEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(generalInputEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(Convert.ToByte(true), failureMechanismEntity.IsRelevant);
            Assert.AreEqual(failureMechanism.GeneralInput.N, generalInputEntity.N);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithNewFailureMechanismSections_FailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                GeneralInput =
                {
                    StorageId = 2
                }
            };
            failureMechanism.AddSection(new FailureMechanismSection("", new[]
            {
                new Point2D(0, 0)
            }));

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = failureMechanism.StorageId,
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId
            });

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities).Count());

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithUpdatedFailureMechanismSections_NoNewFailureMechanismSectionsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism
            {
                StorageId = 1,
                GeneralInput =
                {
                    StorageId = 2
                }
            };
            var testName = "testName";
            failureMechanism.AddSection(new FailureMechanismSection(testName, new[]
            {
                new Point2D(0, 0)
            })
            {
                StorageId = 1
            });

            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                FailureMechanismSectionEntityId = 1,
            };
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                FailureMechanismSectionEntities =
                {
                    failureMechanismSectionEntity
                }
            };

            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            ringtoetsEntities.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(new GrassCoverErosionInwardsFailureMechanismMetaEntity
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntityId = failureMechanism.GeneralInput.StorageId
            });
            ringtoetsEntities.FailureMechanismSectionEntities.Add(failureMechanismSectionEntity);

            // Call
            failureMechanism.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.Count);
            Assert.AreEqual(1, failureMechanismEntity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities).Count());
            Assert.AreEqual(testName, failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0).Name);

            mocks.VerifyAll();
        }
    }
}