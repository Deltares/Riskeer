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
using Application.Ringtoets.Storage.Test.DbContext;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class HydraulicLocationEntityPersistorTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void Setup()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NullDataSet_ThrowsAgrumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicLocationEntityPersistor(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("ringtoetsContext", exception.ParamName);
        }

        [Test]
        public void Constructor_EmptyDataSet_NewInstance()
        {
            // Setup

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            mockRepository.ReplayAll();

            // Call
            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);

            // Assert
            Assert.IsInstanceOf<HydraulicLocationEntityPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entities", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_ValidEntityValidModel_EntityAsModel()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            mockRepository.ReplayAll();

            const string name = "test";
            const double designWaterLevel = 15.6;
            const long locationId = 1300001;
            const long storageId = 1234L;
            const decimal locationX = 253;
            const decimal locationY = 123;
            var entity = new HydraulicLocationEntity()
            {
                LocationId = locationId,
                Name = name,
                DesignWaterLevel = designWaterLevel,
                HydraulicLocationEntityId = storageId,
                LocationX = locationX,
                LocationY = locationY
            };

            // Call
            List<HydraulicBoundaryLocation> locations = persistor.LoadModel(new List<HydraulicLocationEntity>
            {
                entity
            }).ToList();

            // Assert
            Assert.AreEqual(1, locations.Count);
            var location = locations[0];
            Assert.AreEqual(locationId, location.Id);
            Assert.AreEqual(name, location.Name);
            Assert.AreEqual(designWaterLevel, location.DesignWaterLevel);
            Assert.AreEqual(locationX, location.Location.X);
            Assert.AreEqual(locationY, location.Location.Y);
            Assert.AreEqual(storageId, location.StorageId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "test", 1, 1));
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(null, hydraulicBoundaryDatabase);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentNavigationProperty", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullModel_DoesNotAddEntityInParentNavigationProperty()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            var parentNavigationProperty = new List<HydraulicLocationEntity>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null);

            // Assert
            Assert.DoesNotThrow(test);
            CollectionAssert.AreEquivalent(new List<HydraulicLocationEntity>(), parentNavigationProperty);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertyHydraulicLocationWithSameStorageId_HydraulicLocationAsEntityInParentNavigationProperty()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);

            const long storageId = 1234L;
            HydraulicLocationEntity entityToDelete = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = storageId
            };

            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>
            {
                entityToDelete
            };

            HydraulicBoundaryLocation model = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = storageId
            };

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(model);

            mockRepository.ReplayAll();

            // Call
            persistor.InsertModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.HydraulicLocationEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_LocationNull_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(null);

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_LocationToBig_ThrowsOverflowException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", Double.PositiveInfinity, 1));

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.Throws<OverflowException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullDatasetValidModel_ThrowsArgumentNullException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            mockRepository.ReplayAll();

            HydraulicBoundaryLocation model = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = storageId
            };

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(model);

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, hydraulicBoundaryDatabase);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentNavigationProperty", exception.ParamName);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDatasetNullModel_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("model", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();
            var expectedMessage = String.Format("Het object 'HydraulicLocationEntity' met id '{0}' is niet gevonden.", storageId);

            HydraulicBoundaryLocation model = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = storageId
            };

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(model);

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            var exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplicateEntityInDataset_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var expectedMessage = String.Format("Het object 'HydraulicLocationEntity' met id '{0}' is niet gevonden.", storageId);
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>
            {
                new HydraulicLocationEntity
                {
                    HydraulicLocationEntityId = storageId
                },
                new HydraulicLocationEntity
                {
                    HydraulicLocationEntityId = storageId
                }
            };

            HydraulicBoundaryLocation model = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = storageId
            };

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(model);

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            var exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleHydraulicLocationWithStorageId_UpdatedHydraulicLocationAsEntityInParentNavigationProperty()
        {
            // Setup
            const long storageId = 1234L;

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>
            {
                new HydraulicLocationEntity
                {
                    HydraulicLocationEntityId = storageId
                }
            };

            HydraulicBoundaryLocation model = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = storageId
            };

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(model);

            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.HydraulicLocationEntityId);
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NoStorageIdSet_InsertNewEntity()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            mockRepository.ReplayAll();

            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();
            HydraulicBoundaryLocation model = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = 0
            };
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(model);

            // Precondition
            Assert.AreEqual(0, parentNavigationProperty.Count, "Precondition failed: parentNavigationProperty should be empty");

            // Call
            persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_LocationNull_ThrowsArgumentException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(null);

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.Throws<ArgumentException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_LocationToBig_ThrowsOverflowException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            var persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            IList<HydraulicLocationEntity> parentNavigationProperty = new List<HydraulicLocationEntity>();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1, "name", Double.PositiveInfinity, 1));

            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);

            // Assert
            Assert.Throws<OverflowException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInParentNavigationPropertySingleHydraulicLocationWithoutStorageId_UpdatedHydraulicLocationAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved
            HydraulicLocationEntity entityToDelete = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 4567L,
                Name = "Entity to delete"
            };

            ObservableCollection<HydraulicLocationEntity> parentNavigationProperty = new ObservableCollection<HydraulicLocationEntity>
            {
                entityToDelete
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntitiesMock.Expect(x => x.HydraulicLocationEntities).Return(dbset);

            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            HydraulicBoundaryLocation location = new HydraulicBoundaryLocation(13001, "test", 13, 52);
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(location);
            mockRepository.ReplayAll();

            // Call
            persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            HydraulicLocationEntity entity = parentNavigationProperty.SingleOrDefault(x => x.HydraulicLocationEntityId == storageId);
            Assert.IsNotNull(entity);
            Assert.AreEqual(storageId, entity.HydraulicLocationEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertySingleModelStorageId_UpdatedHydraulicLocationmAsEntityAndOtherDeletedInDbSet()
        {
            // Setup
            const long storageId = 1234L;
            HydraulicLocationEntity entityToUpdate = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = storageId,
            };
            HydraulicLocationEntity entityToDelete = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 4567L,
            };

            ObservableCollection<HydraulicLocationEntity> parentNavigationProperty = new ObservableCollection<HydraulicLocationEntity>
            {
                entityToDelete,
                entityToUpdate
            };
            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(entityToDelete)).Return(entityToDelete);

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntitiesMock.Expect(x => x.HydraulicLocationEntities).Return(dbset);

            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(13001, "test", 13, 52)
            {
                StorageId = storageId
            };
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            mockRepository.ReplayAll();

            TestDelegate updateTest = () => persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);
            Assert.DoesNotThrow(updateTest, "Precondition failed: Update should not throw exception.");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            Assert.IsInstanceOf<HydraulicLocationEntity>(entityToUpdate);
            Assert.AreEqual(storageId, entityToUpdate.HydraulicLocationEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertyEmptyHydraulicLocation_EmptyDatabaseSet()
        {
            // Setup
            HydraulicLocationEntity firstEntityToDelete = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 1234L
            };
            HydraulicLocationEntity secondEntityToDelete = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 4567L
            };
            ObservableCollection<HydraulicLocationEntity> parentNavigationProperty = new ObservableCollection<HydraulicLocationEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var dbset = DbTestSet.GetDbTestSet(mockRepository, parentNavigationProperty);
            dbset.Expect(x => x.Remove(firstEntityToDelete)).Return(firstEntityToDelete);
            dbset.Expect(x => x.Remove(secondEntityToDelete)).Return(secondEntityToDelete);

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            ringtoetsEntitiesMock.Expect(x => x.HydraulicLocationEntities).Return(dbset).Repeat.Twice();
            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);

            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(13001, "test", 13, 52);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
            mockRepository.ReplayAll();

            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, hydraulicBoundaryDatabase);
            Assert.DoesNotThrow(test, "Precondition failed: UpdateModel");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            mockRepository.ReplayAll();

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
            var insertedHydraulicLocationEntities = new List<HydraulicLocationEntity>();
            var parentNavigationPropertyMock = mockRepository.StrictMock<ICollection<HydraulicLocationEntity>>();
            parentNavigationPropertyMock.Expect(m => m.Add(null)).IgnoreArguments().WhenCalled(x =>
            {
                var insertedDikeAssessmentSectionEntity = x.Arguments.GetValue(0);
                Assert.IsInstanceOf<HydraulicLocationEntity>(insertedDikeAssessmentSectionEntity);
                insertedHydraulicLocationEntities.Add((HydraulicLocationEntity) insertedDikeAssessmentSectionEntity);
            }).Repeat.Times(numberOfInserts);

            IList<HydraulicBoundaryLocation> hydraulicLocations = new List<HydraulicBoundaryLocation>();
            for (var i = 0; i < numberOfInserts; i++)
            {
                hydraulicLocations.Add(new HydraulicBoundaryLocation(13001, "test", 13, 52)
                {
                    StorageId = 0L
                });
            }

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.AddRange(hydraulicLocations);

            var ringtoetsEntitiesMock = mockRepository.StrictMock<IRingtoetsEntities>();
            HydraulicLocationEntityPersistor persistor = new HydraulicLocationEntityPersistor(ringtoetsEntitiesMock);
            mockRepository.ReplayAll();

            try
            {
                persistor.UpdateModel(parentNavigationPropertyMock, hydraulicBoundaryDatabase);
            }
            catch (Exception)
            {
                Assert.Fail("Precondition failed: persistor.UpdateModel");
            }

            // Call
            for (var i = 0; i < insertedHydraulicLocationEntities.Count; i++)
            {
                insertedHydraulicLocationEntities[i].HydraulicLocationEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(hydraulicLocations.Count, insertedHydraulicLocationEntities.Count);
            foreach (var entity in insertedHydraulicLocationEntities)
            {
                HydraulicBoundaryLocation insertedModel = hydraulicLocations.SingleOrDefault(x => x.StorageId == entity.HydraulicLocationEntityId);
                Assert.IsNotNull(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}