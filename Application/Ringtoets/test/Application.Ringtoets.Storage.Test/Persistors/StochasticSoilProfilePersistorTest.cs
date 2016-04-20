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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class StochasticSoilProfilePersistorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_EmptyDataSet_NewInstance()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            // Call
            StochasticSoilProfilePersistor persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            // Assert
            Assert.IsInstanceOf<StochasticSoilProfilePersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_NullDataSet_ThrowsAgrumentNullException()
        {
            // Call
            TestDelegate test = () => new StochasticSoilProfilePersistor(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("ringtoetsContext", exception.ParamName);
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            // Call
            TestDelegate test = () => persistor.LoadModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_ValidEntityValidModel_EntityAsModel()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            var probability = Convert.ToDecimal(1.0);
            var entity = new StochasticSoilProfileEntity
            {
                Probability = probability,
                SoilProfileEntity = new SoilProfileEntity
                {
                    SoilLayerEntities = new List<SoilLayerEntity>
                    {
                        new SoilLayerEntity()
                    }
                }
            };

            // Call
            StochasticSoilProfile model = persistor.LoadModel(entity);

            // Assert
            Assert.AreEqual(probability, model.Probability);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            // Call
            TestDelegate test = () => persistor.InsertModel(null, new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, -1));

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentNavigationProperty", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullModel_DoesNotAddEntityInParentNavigationProperty()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            var parentNavigationProperty = new List<StochasticSoilProfileEntity>();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null);

            // Assert
            Assert.DoesNotThrow(test);
            Assert.IsEmpty(parentNavigationProperty);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertyStochasticSoilProfileWithSameStorageId_StochasticSoilProfileAsEntityInParentNavigationProperty()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            const long storageId = 1234L;
            StochasticSoilProfileEntity entityToDelete = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = storageId
            };

            IList<StochasticSoilProfileEntity> parentNavigationProperty = new List<StochasticSoilProfileEntity>
            {
                entityToDelete
            };

            StochasticSoilProfile soilProfile = new StochasticSoilProfile(0.1, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = storageId,
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            persistor.InsertModel(parentNavigationProperty, soilProfile);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.StochasticSoilProfileEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullDatasetValidModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            var soilProfile = new StochasticSoilProfile(0.1, SoilProfileType.SoilProfile1D, -1);

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, soilProfile);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentNavigationProperty", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDatasetNullModel_DoesNotThrow()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilProfileEntity> parentNavigationProperty = new List<StochasticSoilProfileEntity>();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void UpdateModel_EmptyDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilProfileEntity> parentNavigationProperty = new List<StochasticSoilProfileEntity>();

            var soilProfile = new StochasticSoilProfile(0.1, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = storageId,
                SoilProfile = new TestPipingSoilProfile()
            };
                

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, soilProfile);

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilProfileEntity' met id '{0}' is niet gevonden.", storageId);
            var exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplicateEntityInDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            IList<StochasticSoilProfileEntity> parentNavigationProperty = new List<StochasticSoilProfileEntity>
            {
                new StochasticSoilProfileEntity
                {
                    StochasticSoilProfileEntityId = storageId
                },
                new StochasticSoilProfileEntity
                {
                    StochasticSoilProfileEntityId = storageId
                }
            };

            var soilProfile = new StochasticSoilProfile(0.1, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = storageId,
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, soilProfile);

            // Assert
            var expectedMessage = String.Format("Het object 'StochasticSoilProfileEntity' met id '{0}' is niet gevonden.", storageId);
            var exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleStochasticSoilProfileWithStorageId_UpdatedStochasticSoilProfileAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            var parentNavigationProperty = new []
            {
                new StochasticSoilProfileEntity
                {
                    StochasticSoilProfileEntityId = storageId
                }
            };

            var probability = new Random(21).NextDouble();
            var soilProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = storageId,
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, soilProfile);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Length);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.StochasticSoilProfileEntityId);
            Assert.AreEqual(probability, entity.Probability);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NoStorageIdSet_InsertNewEntity()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            IList<StochasticSoilProfileEntity> parentNavigationProperty = new List<StochasticSoilProfileEntity>();

            var probability = new Random(21).NextDouble();
            var soilProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1)
            {
                StorageId = 0,
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, soilProfile);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleStochasticSoilProfileWithoutStorageId_UpdatedStochasticSoilProfileAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            StochasticSoilProfileEntity entityToDelete = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 4567L
            };

            ringtoetsEntitiesMock.StochasticSoilProfileEntities.Add(entityToDelete);

            var parentNavigationProperty = new List<StochasticSoilProfileEntity>
            {
                entityToDelete
            };

            mockRepository.ReplayAll();

            StochasticSoilProfilePersistor persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            var probability = new Random(21).NextDouble();
            var soilProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, soilProfile);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            StochasticSoilProfileEntity entity = parentNavigationProperty.SingleOrDefault(x => x.StochasticSoilProfileEntityId == storageId);
            Assert.IsNotNull(entity);
            Assert.AreEqual(storageId, entity.StochasticSoilProfileEntityId);
            Assert.AreEqual(probability, entity.Probability);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleStochasticSoilProfileWithoutStorageId_DbSetCleared()
        {
            // Setup
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved

            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            StochasticSoilProfileEntity entityToDelete = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = 4567L
            };

            ringtoetsEntitiesMock.StochasticSoilProfileEntities.Add(entityToDelete);

            var parentNavigationProperty = new List<StochasticSoilProfileEntity>
            {
                entityToDelete
            };

            mockRepository.ReplayAll();

            StochasticSoilProfilePersistor persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);
            var probability = new Random(21).NextDouble();
            var soilProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            };

            persistor.UpdateModel(parentNavigationProperty, soilProfile);
            
            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            CollectionAssert.IsEmpty(ringtoetsEntitiesMock.StochasticSoilProfileEntities);

            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            // Call
            TestDelegate test = () => persistor.PerformPostSaveActions();

            // Assert
            Assert.DoesNotThrow(test);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void PerformPostSaveActions_MultipleModelsInsertedWithoutStorageId_ModelsWithStorageId(int numberOfInserts)
        {
            // Setup
            var ringtoetsEntitiesMock = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var parentNavigationProperty = new List<StochasticSoilProfileEntity>();

            IList<StochasticSoilProfile> stochasticSoilProfiles = new List<StochasticSoilProfile>();

            var persistor = new StochasticSoilProfilePersistor(ringtoetsEntitiesMock);

            try
            {
                for (var i = 0; i < numberOfInserts; i++)
                {
                    var soilProfile = new StochasticSoilProfile(0.1, SoilProfileType.SoilProfile1D, -1)
                    {
                        StorageId = 0,
                        SoilProfile = new TestPipingSoilProfile()
                    };
                    stochasticSoilProfiles.Add(soilProfile);
                    persistor.UpdateModel(parentNavigationProperty, soilProfile);
                }
            }
            catch (Exception)
            {
                Assert.Fail("Precondition failed: persistor.UpdateModel");
            }

            // Call
            for (var i = 0; i < parentNavigationProperty.Count; i++)
            {
                parentNavigationProperty[i].StochasticSoilProfileEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(stochasticSoilProfiles.Count, parentNavigationProperty.Count);
            foreach (var entity in parentNavigationProperty)
            {
                StochasticSoilProfile insertedModel = stochasticSoilProfiles.SingleOrDefault(x => x.StorageId == entity.StochasticSoilProfileEntityId);
                Assert.IsNotNull(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}