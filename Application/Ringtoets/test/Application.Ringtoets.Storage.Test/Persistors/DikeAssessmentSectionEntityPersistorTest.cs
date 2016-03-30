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
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class DikeAssessmentSectionEntityPersistorTest
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
            DikeAssessmentSectionEntityPersistor p = new DikeAssessmentSectionEntityPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            mockRepository.ReplayAll();

            // Call
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<DikeAssessmentSectionEntityPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.LoadModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_ValidEntity_EntityAsModel()
        {
            // Setup
            const long storageId = 1234L;
            const string name = "test";
            const int norm = 30000;
            const long pipingFailureMechanismStorageId = 1L;

            const string hydraulicDatabaseVersion = "1.0";
            const string hydraulicDatabasePath = "/temp/test";
            const string hydraulicDatabaseLocationName = "test";
            const double hydraulicDatabaseLocationDesignWaterLevel = 15.6;
            const long hydraulicDatabaseLocationLocationId = 1300001;
            const long hydraulicDatabaseLocationStorageId = 1234L;
            const decimal hydraulicDatabaseLocationX = 253;
            const decimal hydraulicDatabaseLocationY = 123;

            const string otherHydraulicDatabaseLocationName = "test2";
            const double otherHydraulicDatabaseLocationDesignWaterLevel = 18.6;
            const long otherHydraulicDatabaseLocationLocationId = 1300005;
            const long otherHydraulicDatabaseLocationStorageId = 4321L;
            const decimal otherHydraulicDatabaseLocationX = 3927;
            const decimal otherHydraulicDatabaseLocationY = 372;

            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var entity = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = storageId, Name = name, Norm = norm, FailureMechanismEntities = new List<FailureMechanismEntity>
                {
                    new FailureMechanismEntity
                    {
                        FailureMechanismType = (int) FailureMechanismType.DikesPipingFailureMechanism, FailureMechanismEntityId = pipingFailureMechanismStorageId
                    }
                },
                HydraulicDatabaseVersion = hydraulicDatabaseVersion, HydraulicDatabaseLocation = hydraulicDatabasePath,
                HydraulicLocationEntities = new List<HydraulicLocationEntity>
                {
                    new HydraulicLocationEntity
                    {
                        Name = hydraulicDatabaseLocationName, DesignWaterLevel = hydraulicDatabaseLocationDesignWaterLevel, HydraulicLocationEntityId = hydraulicDatabaseLocationStorageId,
                        LocationId = hydraulicDatabaseLocationLocationId, LocationX = hydraulicDatabaseLocationX, LocationY = hydraulicDatabaseLocationY,
                    },
                    new HydraulicLocationEntity
                    {
                        Name = otherHydraulicDatabaseLocationName, DesignWaterLevel = otherHydraulicDatabaseLocationDesignWaterLevel, HydraulicLocationEntityId = otherHydraulicDatabaseLocationStorageId,
                        LocationId = otherHydraulicDatabaseLocationLocationId, LocationX = otherHydraulicDatabaseLocationX, LocationY = otherHydraulicDatabaseLocationY,
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            DikeAssessmentSection section = persistor.LoadModel(entity);

            // Assert
            Assert.AreEqual(storageId, section.StorageId);
            Assert.AreEqual(name, section.Name);
            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);
            Assert.AreEqual(pipingFailureMechanismStorageId, section.PipingFailureMechanism.StorageId);
            Assert.AreEqual(hydraulicDatabaseVersion, section.HydraulicBoundaryDatabase.Version);
            Assert.AreEqual(hydraulicDatabasePath, section.HydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(2, section.HydraulicBoundaryDatabase.Locations.Count);

            var firstLocation = section.HydraulicBoundaryDatabase.Locations.First();
            Assert.AreEqual(hydraulicDatabaseLocationName, firstLocation.Name);
            Assert.AreEqual(hydraulicDatabaseLocationDesignWaterLevel, firstLocation.DesignWaterLevel);
            Assert.AreEqual(hydraulicDatabaseLocationStorageId, firstLocation.StorageId);
            Assert.AreEqual(hydraulicDatabaseLocationLocationId, firstLocation.Id);
            Assert.AreEqual(hydraulicDatabaseLocationX, firstLocation.Location.X);
            Assert.AreEqual(hydraulicDatabaseLocationY, firstLocation.Location.Y);

            var secondLocation = section.HydraulicBoundaryDatabase.Locations.ElementAt(1);
            Assert.AreEqual(otherHydraulicDatabaseLocationName, secondLocation.Name);
            Assert.AreEqual(otherHydraulicDatabaseLocationDesignWaterLevel, secondLocation.DesignWaterLevel);
            Assert.AreEqual(otherHydraulicDatabaseLocationStorageId, secondLocation.StorageId);
            Assert.AreEqual(otherHydraulicDatabaseLocationLocationId, secondLocation.Id);
            Assert.AreEqual(otherHydraulicDatabaseLocationX, secondLocation.Location.X);
            Assert.AreEqual(otherHydraulicDatabaseLocationY, secondLocation.Location.Y);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_MultipleEntitiesInDataset_EntitiesAsModel()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = 1, Name = "test1", Norm = 12,
                    HydraulicDatabaseVersion = "1.0", HydraulicDatabaseLocation = "temp/test",
                    HydraulicLocationEntities = new List<HydraulicLocationEntity>
                    {
                        new HydraulicLocationEntity
                        {
                            Name = "test", DesignWaterLevel = 15.6, HydraulicLocationEntityId = 1234L, LocationId = 1300001, LocationX = 253, LocationY = 123
                        }
                    }
                },
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = 2, Name = "test2", Norm = 22,
                    HydraulicDatabaseVersion = "2.0", HydraulicDatabaseLocation = "test",
                    HydraulicLocationEntities = new List<HydraulicLocationEntity>
                    {
                        new HydraulicLocationEntity
                        {
                            Name = "test2", DesignWaterLevel = 135.6, HydraulicLocationEntityId = 134L, LocationId = 1400001, LocationX = 23, LocationY = 23
                        }
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            var loadedModels = parentNavigationProperty.Select(entity => persistor.LoadModel(entity));

            // Assert
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var loadedModelsList = loadedModels.ToList();
            Assert.AreEqual(parentNavigationPropertyList.Count, loadedModelsList.Count);
            for (var i = 0; i < loadedModelsList.Count; i++)
            {
                Assert.AreEqual(parentNavigationPropertyList[i].DikeAssessmentSectionEntityId, loadedModelsList[i].StorageId);
                Assert.AreEqual(parentNavigationPropertyList[i].Name, loadedModelsList[i].Name);
                Assert.AreEqual(parentNavigationPropertyList[i].Norm, loadedModelsList[i].FailureMechanismContribution.Norm);
                Assert.AreEqual(parentNavigationPropertyList[i].HydraulicDatabaseVersion, loadedModelsList[i].HydraulicBoundaryDatabase.Version);
                Assert.AreEqual(parentNavigationPropertyList[i].HydraulicDatabaseLocation, loadedModelsList[i].HydraulicBoundaryDatabase.FilePath);

                var locations = parentNavigationPropertyList[i].HydraulicLocationEntities.ToList();

                for (int j = 0; j < loadedModelsList[i].HydraulicBoundaryDatabase.Locations.Count; j++)
                {
                    Assert.AreEqual(locations[j].HydraulicLocationEntityId, loadedModelsList[i].HydraulicBoundaryDatabase.Locations[j].StorageId);
                    Assert.AreEqual(locations[j].DesignWaterLevel, loadedModelsList[i].HydraulicBoundaryDatabase.Locations[j].DesignWaterLevel);
                    Assert.AreEqual(locations[j].Name, loadedModelsList[i].HydraulicBoundaryDatabase.Locations[j].Name);
                    Assert.AreEqual(locations[j].LocationId, loadedModelsList[i].HydraulicBoundaryDatabase.Locations[j].Id);
                    Assert.AreEqual(locations[j].LocationX, loadedModelsList[i].HydraulicBoundaryDatabase.Locations[j].Location.X);
                    Assert.AreEqual(locations[j].LocationY, loadedModelsList[i].HydraulicBoundaryDatabase.Locations[j].Location.Y);
                }
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var dikeAssessmentSection = new DikeAssessmentSection();

            // Call
            TestDelegate test = () => persistor.InsertModel(null, dikeAssessmentSection, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void InsertModel_NulldikeAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<DikeAssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_EmptyParentNavigationPropertySingleDikeAssessmentSectionWithoutStorageId_DikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    },
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                };

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            // Call
            persistor.InsertModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithSameStorageId_DikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = storageId,
                Name = "Entity to delete"
            };
            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                entityToDelete
            };

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };

            // Call
            persistor.InsertModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_ValidDikeAssessmentSectionWithChildren_InsertedDikeAssessmentSectionWithChildren()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            const string name = "test";
            const double designWaterLevel = 15.6;
            const long hydraulicLocationEntityId = 1234L;
            const long locationId = 1300001;
            const double locationX = 253;
            const double locationY = 123;

            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection();
            dikeAssessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            dikeAssessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(locationId, name, locationX, locationY)
            {
                StorageId = hydraulicLocationEntityId, DesignWaterLevel = designWaterLevel
            });

            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();

            // Call
            persistor.InsertModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.ToList()[0];
            Assert.AreNotEqual(dikeAssessmentSection, entity);

            Assert.AreEqual(1, entity.FailureMechanismEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismEntities.Count(db => db.FailureMechanismType.Equals((int) FailureMechanismType.DikesPipingFailureMechanism)));
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);
            var locationEntity = entity.HydraulicLocationEntities.First();
            Assert.AreEqual(name, locationEntity.Name);
            Assert.AreEqual(designWaterLevel, locationEntity.DesignWaterLevel);
            Assert.AreEqual(hydraulicLocationEntityId, locationEntity.HydraulicLocationEntityId);
            Assert.AreEqual(locationId, locationEntity.LocationId);
            Assert.AreEqual(locationX, locationEntity.LocationX);
            Assert.AreEqual(locationY, locationEntity.LocationY);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var dikeAssessmentSection = new DikeAssessmentSection();

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, dikeAssessmentSection, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateModel_NulldikeAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<DikeAssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyParentNavigationPropertySingleDikeAssessmentSectionWithoutStorageId_DikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);

            mockRepository.ReplayAll();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    },
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                }
                ;

            // Call
            persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>();
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    }
                };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithUnknownStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = 4567L
                }
            };
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    StorageId = storageId
                };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplucateEntityInParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId
                },
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId
                }
            };
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    StorageId = storageId
                };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_UpdatedDikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            ICollection<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId,
                    Name = "old name",
                    Norm = 1
                }
            };
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    },
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                };

            // Call
            persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_MultipleEntitiesInParentNavigationPropertySingleDikeAssessmentSectionWithStorageId_UpdatedDikeAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "UpdatedName";
            const long storageId = 1234L;
            const int norm = 30000;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };
            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                entityToDelete,
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId,
                    Name = "Entity to update",
                    Norm = 1
                }
            };

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    StorageId = storageId,
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    },
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                };

            // Call
            persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.DikeAssessmentSectionEntityId == storageId);
            Assert.IsInstanceOf<DikeAssessmentSectionEntity>(entity);
            Assert.AreEqual(storageId, entity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_ValidDikeAssessmentSectionWithChildren_UpdatedDikeAssessmentSectionWithChildren()
        {
            // Setup
            const long storageId = 1234L;
            const long hydraulicLocationEntityId = 5678L;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                StorageId = storageId,
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            dikeAssessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1300001, "test", 253, 123)
            {
                StorageId = hydraulicLocationEntityId,
            });

            IList<DikeAssessmentSectionEntity> parentNavigationProperty = new List<DikeAssessmentSectionEntity>
            {
                new DikeAssessmentSectionEntity
                {
                    DikeAssessmentSectionEntityId = storageId,
                    HydraulicLocationEntities = new List<HydraulicLocationEntity>
                    {
                        new HydraulicLocationEntity
                        {
                            HydraulicLocationEntityId = hydraulicLocationEntityId
                        }
                    }
                }
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.ToList()[0];
            Assert.AreNotEqual(dikeAssessmentSection, entity);

            Assert.AreEqual(1, entity.FailureMechanismEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismEntities.Count(db => db.FailureMechanismType.Equals((int) FailureMechanismType.DikesPipingFailureMechanism)));
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);

            var hydraulicLocationEntity = entity.HydraulicLocationEntities.First();
            Assert.AreEqual(hydraulicLocationEntityId, hydraulicLocationEntity.HydraulicLocationEntityId);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInParentNavigationPropertyModelWithoutStorageId_UpdatedDikeAssessmentSectionAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 0L; // Newly inserted entities have Id = 0 untill they are saved
            const int norm = 30000;
            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };

            ObservableCollection<DikeAssessmentSectionEntity> parentNavigationProperty = new ObservableCollection<DikeAssessmentSectionEntity>
            {
                entityToDelete
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.DikeAssessmentSectionEntities.Add(entityToDelete);

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            DikeAssessmentSection dikeAssessmentSection =
                new DikeAssessmentSection
                {
                    Name = name,
                    FailureMechanismContribution =
                    {
                        Norm = norm
                    },
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                };

            persistor.InsertModel(parentNavigationProperty, dikeAssessmentSection, 0);

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            CollectionAssert.IsEmpty(ringtoetsEntities.DikeAssessmentSectionEntities);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertySingleModelStorageId_UpdatedDikeAssessmentSectionAsEntityAndOtherDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            DikeAssessmentSectionEntity entityToUpdate = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = storageId,
                Name = "Entity to update"
            };
            DikeAssessmentSectionEntity entityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "First entity to delete"
            };

            ObservableCollection<DikeAssessmentSectionEntity> parentNavigationProperty = new ObservableCollection<DikeAssessmentSectionEntity>
            {
                entityToDelete,
                entityToUpdate
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.DikeAssessmentSectionEntities.Add(entityToDelete);
            ringtoetsEntities.DikeAssessmentSectionEntities.Add(entityToUpdate);

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                StorageId = storageId,
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            mockRepository.ReplayAll();

            TestDelegate updateTest = () => persistor.UpdateModel(parentNavigationProperty, dikeAssessmentSection, 0);
            Assert.DoesNotThrow(updateTest, "Precondition failed: Update should not throw exception.");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.DikeAssessmentSectionEntities.Count());
            Assert.AreEqual(entityToUpdate, ringtoetsEntities.DikeAssessmentSectionEntities.FirstOrDefault());

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertyEmptyDikeAssessmentSection_EmptyDatabaseSet()
        {
            // Setup
            DikeAssessmentSectionEntity firstEntityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 1234L,
                Name = "First entity to delete"
            };
            DikeAssessmentSectionEntity secondEntityToDelete = new DikeAssessmentSectionEntity
            {
                DikeAssessmentSectionEntityId = 4567L,
                Name = "Second entity to delete"
            };
            ObservableCollection<DikeAssessmentSectionEntity> parentNavigationProperty = new ObservableCollection<DikeAssessmentSectionEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.DikeAssessmentSectionEntities.Add(firstEntityToDelete);
            ringtoetsEntities.DikeAssessmentSectionEntities.Add(secondEntityToDelete);
            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            mockRepository.ReplayAll();

            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, dikeAssessmentSection, 0);
            Assert.DoesNotThrow(test, "Precondition failed: InsertModel");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void PerformPostSaveActions_NoInserts_DoesNotThrowException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

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

            var insertedDikeAssessmentSectionEntities = new List<DikeAssessmentSectionEntity>();

            IList<DikeAssessmentSection> dikeAssessmentSections = new List<DikeAssessmentSection>();

            DikeAssessmentSectionEntityPersistor persistor = new DikeAssessmentSectionEntityPersistor(ringtoetsEntities);

            for (var i = 0; i < numberOfInserts; i++)
            {
                dikeAssessmentSections.Add(new DikeAssessmentSection
                {
                    StorageId = 0L,
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                });
            }

            foreach (var dikeAssessmentSection in dikeAssessmentSections)
            {
                try
                {
                    persistor.UpdateModel(insertedDikeAssessmentSectionEntities, dikeAssessmentSection, 0);
                }
                catch (Exception)
                {
                    Assert.Fail("Precondition failed: persistor.UpdateModel");
                }
            }

            // Call
            for (var i = 0; i < insertedDikeAssessmentSectionEntities.Count; i++)
            {
                insertedDikeAssessmentSectionEntities[i].DikeAssessmentSectionEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(dikeAssessmentSections.Count, insertedDikeAssessmentSectionEntities.Count);
            foreach (var entity in insertedDikeAssessmentSectionEntities)
            {
                var insertedModel = dikeAssessmentSections.SingleOrDefault(x => x.StorageId == entity.DikeAssessmentSectionEntityId);
                Assert.IsInstanceOf<DikeAssessmentSection>(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}