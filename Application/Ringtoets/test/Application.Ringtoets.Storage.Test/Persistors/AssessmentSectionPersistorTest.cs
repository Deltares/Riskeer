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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.TestUtil;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Persistors
{
    [TestFixture]
    public class AssessmentSectionPersistorTest
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
            AssessmentSectionPersistor p = new AssessmentSectionPersistor(null);
        }

        [Test]
        public void Constructor_EmptyDataset_NewInstance()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            mockRepository.ReplayAll();

            // Call
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            // Assert
            Assert.IsInstanceOf<AssessmentSectionPersistor>(persistor);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
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
            const long asphaltFailureMechanismStorageId = 2L;
            const long overtoppingFailureMechanismStorageId = 3L;

            const string hydraulicDatabaseVersion = "1.0";
            const string hydraulicDatabasePath = "/temp/test";
            const string hydraulicDatabaseLocationName = "test";
            const double hydraulicDatabaseLocationDesignWaterLevel = 15.6;
            const long hydraulicDatabaseLocationLocationId = 1300001L;
            const long hydraulicDatabaseLocationStorageId = 1234L;
            const decimal hydraulicDatabaseLocationX = 253;
            const decimal hydraulicDatabaseLocationY = 123;

            const string otherHydraulicDatabaseLocationName = "test2";
            const double otherHydraulicDatabaseLocationDesignWaterLevel = 18.6;
            const long otherHydraulicDatabaseLocationLocationId = 1300005L;
            const long otherHydraulicDatabaseLocationStorageId = 4321L;
            const decimal otherHydraulicDatabaseLocationX = 3927;
            const decimal otherHydraulicDatabaseLocationY = 372;
            const AssessmentSectionComposition composition = AssessmentSectionComposition.DikeAndDune;

            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId, Name = name, Norm = norm,
                FailureMechanismEntities = new List<FailureMechanismEntity>
                {
                    new FailureMechanismEntity
                    {
                        FailureMechanismType = (int) FailureMechanismType.Piping, 
                        FailureMechanismEntityId = pipingFailureMechanismStorageId,
                        IsRelevant = 0
                    },
                    new FailureMechanismEntity
                    {
                        FailureMechanismType = (short) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                        FailureMechanismEntityId = asphaltFailureMechanismStorageId,
                        IsRelevant = 0
                    },
                    new FailureMechanismEntity
                    {
                        FailureMechanismType = (short) FailureMechanismType.StructureHeight,
                        FailureMechanismEntityId = overtoppingFailureMechanismStorageId,
                        IsRelevant = 1
                    },
                },
                HydraulicDatabaseVersion = hydraulicDatabaseVersion, HydraulicDatabaseLocation = hydraulicDatabasePath,
                Composition = (short) composition,
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
                },
                ReferenceLinePointEntities = new []
                {
                    new ReferenceLinePointEntity
                    {
                        X = Convert.ToDecimal(1.1), Y = Convert.ToDecimal(2.3)
                    }
                }
            };
            mockRepository.ReplayAll();

            // Call
            AssessmentSection section = persistor.LoadModel(entity);

            // Assert
            Assert.AreEqual(storageId, section.StorageId);
            Assert.AreEqual(name, section.Name);
            Assert.AreEqual(composition, section.Composition);
            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);

            Assert.AreEqual(pipingFailureMechanismStorageId, section.PipingFailureMechanism.StorageId);
            Assert.IsFalse(section.PipingFailureMechanism.IsRelevant);

            Assert.AreEqual(asphaltFailureMechanismStorageId, section.AsphaltRevetment.StorageId);
            Assert.IsFalse(section.AsphaltRevetment.IsRelevant);

            Assert.AreEqual(overtoppingFailureMechanismStorageId, section.Overtopping.StorageId);
            Assert.IsTrue(section.Overtopping.IsRelevant);

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

            var line = section.ReferenceLine;
            Assert.AreEqual(1, line.Points.Count());
            Assert.AreEqual(1.1, line.Points.ElementAt(0).X);
            Assert.AreEqual(2.3, line.Points.ElementAt(0).Y);

            mockRepository.VerifyAll();
        }

        [Test]
        public void LoadModel_MultipleEntitiesInDataset_EntitiesAsModel()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = 1, Name = "test1", Norm = 12,
                    HydraulicDatabaseVersion = "1.0", HydraulicDatabaseLocation = "temp/test",
                    Composition = (short) AssessmentSectionComposition.Dune,
                    HydraulicLocationEntities = new List<HydraulicLocationEntity>
                    {
                        new HydraulicLocationEntity
                        {
                            Name = "test", DesignWaterLevel = 15.6, HydraulicLocationEntityId = 1234L, LocationId = 1300001, LocationX = 253, LocationY = 123
                        }
                    },
                    ReferenceLinePointEntities = new []
                    {
                        new ReferenceLinePointEntity
                        {
                            X = Convert.ToDecimal(1.1), Y = Convert.ToDecimal(2.3)
                        }
                    }
                },
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = 2, Name = "test2", Norm = 22,
                    HydraulicDatabaseVersion = "2.0", HydraulicDatabaseLocation = "test",
                    Composition = (short) AssessmentSectionComposition.Dike,
                    HydraulicLocationEntities = new List<HydraulicLocationEntity>
                    {
                        new HydraulicLocationEntity
                        {
                            Name = "test2", DesignWaterLevel = 135.6, HydraulicLocationEntityId = 134L, LocationId = 1400001, LocationX = 23, LocationY = 23
                        }
                    },
                    ReferenceLinePointEntities = new []
                    {
                        new ReferenceLinePointEntity
                        {
                            X = Convert.ToDecimal(2.2), Y = Convert.ToDecimal(6.3)
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
                Assert.AreEqual(parentNavigationPropertyList[i].AssessmentSectionEntityId, loadedModelsList[i].StorageId);
                Assert.AreEqual(parentNavigationPropertyList[i].Name, loadedModelsList[i].Name);
                Assert.AreEqual((AssessmentSectionComposition) parentNavigationPropertyList[i].Composition, loadedModelsList[i].Composition);
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

                var referenceLinePoints = parentNavigationPropertyList[i].ReferenceLinePointEntities;
                Assert.AreEqual(1, referenceLinePoints.Count);
                Assert.AreEqual(referenceLinePoints.ElementAt(0).X, loadedModelsList[i].ReferenceLine.Points.ElementAt(0).X);
                Assert.AreEqual(referenceLinePoints.ElementAt(0).Y, loadedModelsList[i].ReferenceLine.Points.ElementAt(0).Y);
            }

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () => persistor.InsertModel(null, assessmentSection, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void InsertModel_NullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<AssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void InsertModel_EmptyParentNavigationPropertySingleAssessmentSectionWithoutStorageId_AssessmentSectionAsEntityInParentNavigationProperty(AssessmentSectionComposition composition)
        {
            // Setup
            const string name = "test";
            const int norm = 30000;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>();
            AssessmentSection assessmentSection = new AssessmentSection(composition)
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
            };

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            // Call
            persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.AssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);
            Assert.AreEqual((short) composition, entity.Composition);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_PersistingFailureMechanismPlaceholders_FailureMechanismEntitiesHaveElementsAdded()
        {
            // Setup
            var ringtoetsContext = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new AssessmentSectionPersistor(ringtoetsContext);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                PipingFailureMechanism =
                {
                    IsRelevant = false
                },
                Closing =
                {
                    IsRelevant = false
                },
                AsphaltRevetment =
                {
                    IsRelevant = true
                }
            };

            var parentNavigationProperty = new List<AssessmentSectionEntity>();

            // Call
            persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            AssessmentSectionEntity assessmentSectionEntity = parentNavigationProperty[0];
            Assert.AreEqual(9, assessmentSectionEntity.FailureMechanismEntities.Count);

            FailureMechanismEntity pipingFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.Piping);
            Assert.AreEqual(0, pipingFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to false, so expecting a value of 0.");

            FailureMechanismEntity closingFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.ReliabilityClosingOfStructure);
            Assert.AreEqual(0, closingFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to false, so expecting a value of 0.");

            FailureMechanismEntity asphaltFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.WaveImpactOnAsphaltRevetment);
            Assert.AreEqual(1, asphaltFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to true, so expecting a value of 1.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_SingleEntityInParentNavigationPropertySingleAssessmentSectionWithSameStorageId_AssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            AssessmentSectionEntity entityToDelete = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId,
                Name = "Entity to delete"
            };
            IList<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                entityToDelete
            };

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };

            // Call
            persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[1];
            Assert.AreEqual(storageId, entity.AssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void InsertModel_ValidAssessmentSectionWithChildren_InsertedAssessmentSectionWithChildren()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            const string name = "test";
            const double designWaterLevel = 15.6;
            const long hydraulicLocationEntityId = 1234L;
            const long locationId = 1300001;
            const double locationX = 253;
            const double locationY = 123;

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        new HydraulicBoundaryLocation(locationId, name, locationX, locationY)
                        {
                            StorageId = hydraulicLocationEntityId, DesignWaterLevel = designWaterLevel
                        }
                    }
                },
                ReferenceLine = new ReferenceLine()
            };
            var pointX = 3.2;
            var pointY = 1.1;
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(pointX, pointY)
            });

            IList<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>();

            // Call
            persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.ToList()[0];
            Assert.AreNotEqual(assessmentSection, entity);

            Assert.AreEqual(9, entity.FailureMechanismEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismEntities.Count(db => db.FailureMechanismType.Equals((short) FailureMechanismType.Piping)));
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);
            var locationEntity = entity.HydraulicLocationEntities.First();
            Assert.AreEqual(name, locationEntity.Name);
            Assert.AreEqual(designWaterLevel, locationEntity.DesignWaterLevel);
            Assert.AreEqual(hydraulicLocationEntityId, locationEntity.HydraulicLocationEntityId);
            Assert.AreEqual(locationId, locationEntity.LocationId);
            Assert.AreEqual(locationX, locationEntity.LocationX);
            Assert.AreEqual(locationY, locationEntity.LocationY);

            Assert.AreEqual(pointX, entity.ReferenceLinePointEntities.First().X);
            Assert.AreEqual(pointY, entity.ReferenceLinePointEntities.First().Y);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_AssessmentSectionNeverPersistedBefore_FailureMechanismEntitiesHaveElementsAdded()
        {
            // Setup
            var ringtoetsContext = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new AssessmentSectionPersistor(ringtoetsContext);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                PipingFailureMechanism =
                {
                    IsRelevant = false
                },
                Overtopping = 
                {
                    IsRelevant = false
                },
                StoneRevetment = 
                {
                    IsRelevant = true
                }
            };

            // Precondition:
            Assert.AreEqual(0, assessmentSection.StorageId,
                "StorageId of 0 expected for assessment sections that haven't been persisted before.");

            var parentNavigationProperty = new List<AssessmentSectionEntity>();

            // Call
            persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            AssessmentSectionEntity assessmentSectionEntity = parentNavigationProperty[0];
            Assert.AreEqual(9, assessmentSectionEntity.FailureMechanismEntities.Count);

            FailureMechanismEntity pipingFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.Piping);
            Assert.AreEqual(0, pipingFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to false, so expecting a value of 0.");

            FailureMechanismEntity closingFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.StructureHeight);
            Assert.AreEqual(0, closingFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to false, so expecting a value of 0.");

            FailureMechanismEntity asphaltFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.StabilityStoneRevetment);
            Assert.AreEqual(1, asphaltFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to true, so expecting a value of 1.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_AssessmentSectionHadBeenPersistedBefore_FailureMechanismEntitiesHaveElementsUpdated()
        {
            // Setup
            var ringtoetsContext = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            var persistor = new AssessmentSectionPersistor(ringtoetsContext);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                PipingFailureMechanism =
                {
                    IsRelevant = false
                },
                FailingOfConstruction = 
                {
                    IsRelevant = false
                },
                MacrostabilityInwards = 
                {
                    IsRelevant = true
                }
            };

            var parentNavigationProperty = new List<AssessmentSectionEntity>();

            persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);
            parentNavigationProperty[0].AssessmentSectionEntityId = 1;
            AssessmentSectionEntity sectionEntity = parentNavigationProperty[0];
            for (int i = 0; i < sectionEntity.FailureMechanismEntities.Count; i++)
            {
                sectionEntity.FailureMechanismEntities.ElementAt(i).FailureMechanismEntityId = i+1;
            }
            persistor.PerformPostSaveActions();

            // Precondition:
            Assert.AreNotEqual(0, assessmentSection.StorageId,
                "StorageId > 0 expected for failure mechanism that have been persisted.");
            Assert.AreNotEqual(0, assessmentSection.PipingFailureMechanism.StorageId,
                               "StorageId > 0 expected for failure mechanisms that have been persisted.");
            Assert.AreNotEqual(0, assessmentSection.FailingOfConstruction.StorageId,
                               "StorageId > 0 expected for failure mechanisms that have been persisted.");
            Assert.AreNotEqual(0, assessmentSection.MacrostabilityInwards.StorageId,
                               "StorageId > 0 expected for failure mechanisms that have been persisted.");

            // Call
            persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);

            AssessmentSectionEntity assessmentSectionEntity = parentNavigationProperty[0];
            Assert.AreEqual(9, assessmentSectionEntity.FailureMechanismEntities.Count);

            FailureMechanismEntity pipingFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.Piping);
            Assert.AreEqual(0, pipingFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to false, so expecting a value of 0.");

            FailureMechanismEntity closingFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.StrengthAndStabilityPointConstruction);
            Assert.AreEqual(0, closingFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to false, so expecting a value of 0.");

            FailureMechanismEntity asphaltFailureMechanismEntity = assessmentSectionEntity.FailureMechanismEntities
                .First(fme => fme.FailureMechanismType == (short)FailureMechanismType.MacrostabilityInwards);
            Assert.AreEqual(1, asphaltFailureMechanismEntity.IsRelevant,
                "Set IsRelevant to true, so expecting a value of 1.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_NullParentNavigationProperty_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () => persistor.UpdateModel(null, assessmentSection, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateModel_NullAssessmentSection_ThrowsArgumentNullException()
        {
            // Setup
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            var parentNavigationProperty = mockRepository.StrictMock<ICollection<AssessmentSectionEntity>>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, null, 0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyParentNavigationPropertySingleAssessmentSectionWithoutStorageId_AssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            AssessmentSectionComposition composition = AssessmentSectionComposition.DikeAndDune;
            const int norm = 30000;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);

            mockRepository.ReplayAll();

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>();
            AssessmentSection assessmentSection = new AssessmentSection(composition)
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(0, entity.AssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual((short) composition, entity.Composition);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_EmptyParentNavigationPropertySingleAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            const int norm = 30000;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>();
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleAssessmentSectionWithUnknownStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = 4567L
                }
            };
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = storageId
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.Throws<EntityNotFoundException>(test);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_DuplucateEntityInParentNavigationPropertySingleAssessmentSectionWithStorageId_ThrowsEntityNotFoundException()
        {
            // Setup
            const long storageId = 1234L;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = storageId
                },
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = storageId
                }
            };
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = storageId
            };

            // Call
            TestDelegate test = () => persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_SingleEntityInParentNavigationPropertySingleAssessmentSectionWithStorageId_UpdatedAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            AssessmentSectionComposition composition = AssessmentSectionComposition.DikeAndDune;
            const int norm = 30000;
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            ICollection<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = storageId,
                    Name = "old name",
                    Norm = 1,
                    Composition = (short) AssessmentSectionComposition.Dune
                }
            };
            AssessmentSection assessmentSection = new AssessmentSection(composition)
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
            persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var parentNavigationPropertyList = parentNavigationProperty.ToList();
            var entity = parentNavigationPropertyList[0];
            Assert.AreEqual(storageId, entity.AssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual((short) composition, entity.Composition);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_MultipleEntitiesInParentNavigationPropertySingleAssessmentSectionWithStorageId_UpdatedAssessmentSectionAsEntityInParentNavigationProperty()
        {
            // Setup
            const string name = "UpdatedName";
            const long storageId = 1234L;
            AssessmentSectionComposition composition = AssessmentSectionComposition.Dike;
            const int norm = 30000;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            AssessmentSectionEntity entityToDelete = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };
            IList<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                entityToDelete,
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = storageId,
                    Name = "Entity to update",
                    Norm = 1,
                    Composition = (short) AssessmentSectionComposition.Dune
                }
            };

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            AssessmentSection assessmentSection = new AssessmentSection(composition)
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
            persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(2, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.SingleOrDefault(x => x.AssessmentSectionEntityId == storageId);
            Assert.IsInstanceOf<AssessmentSectionEntity>(entity);
            Assert.AreEqual(storageId, entity.AssessmentSectionEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual((short) composition, entity.Composition);
            Assert.AreEqual(norm, entity.Norm);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateModel_ValidAssessmentSectionWithChildren_UpdatedAssessmentSectionWithChildren()
        {
            // Setup
            const long storageId = 1234L;
            const long hydraulicLocationEntityId = 5678L;

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = storageId,
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase(),
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.HydraulicBoundaryDatabase.Locations.Add(new HydraulicBoundaryLocation(1300001, "test", 253, 123)
            {
                StorageId = hydraulicLocationEntityId,
            });
            var pointX = 2.4;
            var pointY = 3.0;
            assessmentSection.ReferenceLine.SetGeometry(new []
            {
                new Point2D(pointX, pointY)
            });

            IList<AssessmentSectionEntity> parentNavigationProperty = new List<AssessmentSectionEntity>
            {
                new AssessmentSectionEntity
                {
                    AssessmentSectionEntityId = storageId,
                    HydraulicLocationEntities = new List<HydraulicLocationEntity>
                    {
                        new HydraulicLocationEntity
                        {
                            HydraulicLocationEntityId = hydraulicLocationEntityId
                        }
                    },
                    ReferenceLinePointEntities = new List<ReferenceLinePointEntity>()
                }
            };

            // Call
            persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);

            // Assert
            Assert.AreEqual(1, parentNavigationProperty.Count);
            var entity = parentNavigationProperty.ToList()[0];
            Assert.AreNotEqual(assessmentSection, entity);

            Assert.AreEqual(9, entity.FailureMechanismEntities.Count);
            Assert.AreEqual(1, entity.FailureMechanismEntities.Count(db => db.FailureMechanismType.Equals((short) FailureMechanismType.Piping)));
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);

            var hydraulicLocationEntity = entity.HydraulicLocationEntities.First();
            Assert.AreEqual(hydraulicLocationEntityId, hydraulicLocationEntity.HydraulicLocationEntityId);

            var referenceLinePointEntity = entity.ReferenceLinePointEntities.First();
            Assert.AreEqual(pointX, referenceLinePointEntity.X);
            Assert.AreEqual(pointY, referenceLinePointEntity.Y);

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_SingleEntityInParentNavigationPropertyModelWithoutStorageId_UpdatedAssessmentSectionAsEntityInParentNavigationPropertyAndOthersDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const int norm = 30000;
            AssessmentSectionEntity entityToDelete = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 4567L,
                Name = "Entity to delete"
            };

            ObservableCollection<AssessmentSectionEntity> parentNavigationProperty = new ObservableCollection<AssessmentSectionEntity>
            {
                entityToDelete
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.AssessmentSectionEntities.Add(entityToDelete);

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            CollectionAssert.IsEmpty(ringtoetsEntities.AssessmentSectionEntities);
            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertySingleModelStorageId_UpdatedAssessmentSectionAsEntityAndOtherDeletedInDbSet()
        {
            // Setup
            const string name = "test";
            const long storageId = 1234L;
            AssessmentSectionComposition composition = AssessmentSectionComposition.DikeAndDune;
            const int norm = 30000;
            AssessmentSectionEntity entityToUpdate = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId,
                Name = "Entity to update"
            };
            AssessmentSectionEntity entityToDelete = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 4567L,
                Name = "First entity to delete"
            };

            ObservableCollection<AssessmentSectionEntity> parentNavigationProperty = new ObservableCollection<AssessmentSectionEntity>
            {
                entityToDelete,
                entityToUpdate
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.AssessmentSectionEntities.Add(entityToDelete);
            ringtoetsEntities.AssessmentSectionEntities.Add(entityToUpdate);

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);
            AssessmentSection assessmentSection = new AssessmentSection(composition)
            {
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                StorageId = storageId,
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };

            TestDelegate updateTest = () => persistor.UpdateModel(parentNavigationProperty, assessmentSection, 0);
            Assert.DoesNotThrow(updateTest, "Precondition failed: Update should not throw exception.");

            // Call
            persistor.RemoveUnModifiedEntries(parentNavigationProperty);

            // Assert
            Assert.AreEqual(1, ringtoetsEntities.AssessmentSectionEntities.Count());
            Assert.AreEqual(entityToUpdate, ringtoetsEntities.AssessmentSectionEntities.FirstOrDefault());

            mockRepository.VerifyAll();
        }

        [Test]
        public void RemoveUnModifiedEntries_MultipleEntitiesInParentNavigationPropertyEmptyAssessmentSection_EmptyDatabaseSet()
        {
            // Setup
            AssessmentSectionEntity firstEntityToDelete = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1234L,
                Name = "First entity to delete"
            };
            AssessmentSectionEntity secondEntityToDelete = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 4567L,
                Name = "Second entity to delete"
            };
            ObservableCollection<AssessmentSectionEntity> parentNavigationProperty = new ObservableCollection<AssessmentSectionEntity>
            {
                firstEntityToDelete,
                secondEntityToDelete
            };

            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mockRepository);
            mockRepository.ReplayAll();

            ringtoetsEntities.AssessmentSectionEntities.Add(firstEntityToDelete);
            ringtoetsEntities.AssessmentSectionEntities.Add(secondEntityToDelete);
            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            };
            mockRepository.ReplayAll();

            TestDelegate test = () => persistor.InsertModel(parentNavigationProperty, assessmentSection, 0);
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

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

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

            var insertedAssessmentSectionEntities = new List<AssessmentSectionEntity>();

            IList<AssessmentSection> assessmentSections = new List<AssessmentSection>();

            AssessmentSectionPersistor persistor = new AssessmentSectionPersistor(ringtoetsEntities);

            for (var i = 0; i < numberOfInserts; i++)
            {
                assessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    StorageId = 0L,
                    HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
                });
            }

            foreach (var assessmentSection in assessmentSections)
            {
                try
                {
                    persistor.UpdateModel(insertedAssessmentSectionEntities, assessmentSection, 0);
                }
                catch (Exception)
                {
                    Assert.Fail("Precondition failed: persistor.UpdateModel");
                }
            }

            // Call
            for (var i = 0; i < insertedAssessmentSectionEntities.Count; i++)
            {
                insertedAssessmentSectionEntities[i].AssessmentSectionEntityId = 1L + i;
            }
            persistor.PerformPostSaveActions();

            // Assert
            Assert.AreEqual(assessmentSections.Count, insertedAssessmentSectionEntities.Count);
            foreach (var entity in insertedAssessmentSectionEntities)
            {
                var insertedModel = assessmentSections.SingleOrDefault(x => x.StorageId == entity.AssessmentSectionEntityId);
                Assert.IsInstanceOf<AssessmentSection>(insertedModel);
            }

            mockRepository.VerifyAll();
        }
    }
}