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
using System.Collections.ObjectModel;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class PipingFailureMechanismPersistorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullDataSet_ThrowsArgumentNullException()
        {
            // Call
            PipingFailureMechanismPersistor p = new PipingFailureMechanismPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            // Call
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<PipingFailureMechanismPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.LoadModel(null, new PipingFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void LoadModel_NullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            FailureMechanismEntity entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => persistor.LoadModel(entity, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void LoadModel_EntityWithIncorrectType_ThrowsArgumentException()
        {
            // Setup
            const long storageId = 1234L;
            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.StoneRevetmentFailureMechanism,
            };
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

            // Call
            TestDelegate test = () => persistor.LoadModel(entity, new PipingFailureMechanism());

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void LoadModel_ValidEntity_UpdatedModel()
        {
            // Setup
            const long storageId = 1234L;
            PipingFailureMechanism model = new PipingFailureMechanism()
            {
                StorageId = storageId
            };
            FailureMechanismEntity entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism,
            };
            var ringtoetsEntities = mockRepository.StrictMock<IRingtoetsEntities>();
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

            var assessmentSection = new AssessmentSection();

            // Call
            persistor.LoadModel(entity, assessmentSection.PipingFailureMechanism);

            // Assert
            PipingFailureMechanism loadedModel = assessmentSection.PipingFailureMechanism;
            Assert.IsInstanceOf<PipingFailureMechanism>(loadedModel);
            Assert.AreEqual(loadedModel.StorageId, entity.FailureMechanismEntityId);
            Assert.AreEqual(model.StorageId, loadedModel.StorageId);
        }

        [Test]
        public void InsertModel_NullDataset_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism model = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => persistor.InsertModel(null, model);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyDatasetNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyDatasetValidModel_ValidEntityInDataModel()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();
            PipingFailureMechanism model = new PipingFailureMechanism();

            // Call
            persistor.InsertModel(parentNavigationProperty, model);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var entity = parentNavigationProperty[0];
            Assert.AreNotEqual(model, entity);
            Assert.AreEqual((int) FailureMechanismType.PipingFailureMechanism, entity.FailureMechanismType);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertySinglePipingFailureMechanismWithSameStorageId_PipingFailureMechanismAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            FailureMechanismEntity entityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                entityToDelete
            };
            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            // Call
            persistor.InsertModel(parentNavigationProperty, pipingFailureMechanism);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullDataSetValidModel_ThrowsArgumentNullException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, model);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDataSetNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplicateEntityInDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                new FailureMechanismEntity
                {
                    FailureMechanismEntityId = storageId,
                    FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
                },
                new FailureMechanismEntity
                {
                    FailureMechanismEntityId = storageId,
                    FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
                }
            };
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySinglePipingFailureMechanismWithStorageId_UpdatedPipingFailureMechanismAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            ICollection<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>
            {
                new FailureMechanismEntity
                {
                    FailureMechanismEntityId = storageId,
                    FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
                }
            };
            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NoStorageIdSet_InsertNewEntity()
        {
            var mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            IList<FailureMechanismEntity> parentNavigationProperty = new List<FailureMechanismEntity>();

            PipingFailureMechanism model = new PipingFailureMechanism
            {
                StorageId = 0
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, model);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInParentNavigationPropertySinglePipingFailureMechanismWithoutStorageId_UpdatedPipingFailureMechanismAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            FailureMechanismEntity entityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4567L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };

            ObservableCollection<FailureMechanismEntity> parentNavigationProperty = new ObservableCollection<FailureMechanismEntity>
            {
                entityToDelete
            };

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            ringtoetsEntities.FailureMechanismEntities.Add(entityToDelete);

            // Precondition
            persistor.InsertModel(parentNavigationProperty, pipingFailureMechanism);

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            CollectionAssert.IsEmpty(ringtoetsEntities.FailureMechanismEntities);
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.FailureMechanismEntityId == storageId);
            Assert.IsNotNull(entity);
            Assert.AreEqual(storageId, entity.FailureMechanismEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertySingleModelStorageId_UpdatedPipingFailureMechanismAsEntityAndOtherDeletedInDbSet()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            FailureMechanismEntity entityToUpdate = new FailureMechanismEntity
            {
                FailureMechanismEntityId = storageId,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            FailureMechanismEntity entityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4567L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            ringtoetsEntities.FailureMechanismEntities.Add(entityToUpdate);
            ringtoetsEntities.FailureMechanismEntities.Add(entityToDelete);

            ObservableCollection<FailureMechanismEntity> parentNavigationProperty = new ObservableCollection<FailureMechanismEntity>
            {
                entityToDelete,
                entityToUpdate
            };

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism
            {
                StorageId = storageId
            };

            TestDelegate updateTest = () => persistor.UpdateModel(parentNavigationProperty, pipingFailureMechanism);
            Assert.DoesNotThrow(updateTest, "Precondition failed: Update should not throw exception.");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.FailureMechanismEntities.Count());
            var entityUpdated = ringtoetsEntities.FailureMechanismEntities.FirstOrDefault();
            Assert.AreEqual(entityToUpdate, entityUpdated);
            Assert.AreEqual(2, parentNavigationProperty.Count);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertyEmptyPipingFailureMechanism_EmptyDatabaseSet()
        {
            // Setup
            FailureMechanismEntity firstEntityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1234L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            FailureMechanismEntity secondEntityToDelete = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4567L,
                FailureMechanismType = (int) FailureMechanismType.PipingFailureMechanism
            };
            ObservableCollection<FailureMechanismEntity> parentNavigationProperty = new ObservableCollection<FailureMechanismEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.FailureMechanismEntities.Add(firstEntityToDelete);
            ringtoetsEntities.FailureMechanismEntities.Add(secondEntityToDelete);

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            PipingFailureMechanism pipingFailureMechanism = new PipingFailureMechanism();

            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, pipingFailureMechanism);
            Assert.DoesNotThrow(test, "Precondition failed: UpdateModel");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            CollectionAssert.IsEmpty(ringtoetsEntities.FailureMechanismEntities);
            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var insertedFailureMechanismEntities = new List<FailureMechanismEntity>();

            IList<PipingFailureMechanism> pipingFailureMechanisms = new List<PipingFailureMechanism>();
            for (var i = 0; i < numberOfInserts; i++)
            {
                pipingFailureMechanisms.Add(new PipingFailureMechanism
                {
                    StorageId = 0L
                });
            }

            PipingFailureMechanismPersistor persistor = new PipingFailureMechanismPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            foreach (var pipingFailureMechanism in pipingFailureMechanisms)
            {
                try
                {
                    persistor.UpdateModel(insertedFailureMechanismEntities, pipingFailureMechanism);
                }
                catch (Exception)
                {
                    Assert.Fail("Precondition failed: persistor.UpdateModel");
                }
            }

            // Call
            for (var i = 0; i < insertedFailureMechanismEntities.Count; i++)
            {
                insertedFailureMechanismEntities[i].FailureMechanismEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(pipingFailureMechanisms.Count, insertedFailureMechanismEntities.Count);
            foreach (var entity in insertedFailureMechanismEntities)
            {
                var insertedModel = pipingFailureMechanisms.SingleOrDefault(x => x.StorageId == entity.FailureMechanismEntityId);
                Assert.IsInstanceOf<PipingFailureMechanism>(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}