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
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class AssessmentSectionUpdateExtensionsTest
    {
        [Test]
        public void Update_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var section = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () => section.Update(new UpdateConversionCollector(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var section = new AssessmentSection(AssessmentSectionComposition.Dike);

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
            Assert.AreEqual("collector", paramName);
        }

        [Test]
        public void Update_ContextWithNoAssessmentSection_EntityNotFoundException()
        {
            // Setup
            var section = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () =>
            {
                using (var ringtoetsEntities = new RingtoetsEntities())
                {
                    section.Update(new UpdateConversionCollector(), ringtoetsEntities);
                }
            };

            // Assert
            var expectedMessage = String.Format("Het object 'AssessmentSectionEntity' met id '{0}' is niet gevonden.", 0);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Update_ContextWithNoAssessmentSectionWithId_EntityNotFoundException()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var storageId = 1;
            var section = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                StorageId = storageId
            };

            ringtoetsEntities.AssessmentSectionEntities.Add(new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 2
            });

            // Call
            TestDelegate test = () => section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            var expectedMessage = String.Format("Het object 'AssessmentSectionEntity' met id '{0}' is niet gevonden.", storageId);
            EntityNotFoundException exception = Assert.Throws<EntityNotFoundException>(test);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ContextWithAssessmentSection_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var newName = "newName";
            var composition = AssessmentSectionComposition.Dune;
            var section = InitializeCreatedDikeAssessmentSection(AssessmentSectionComposition.Dune);
            section.Name = newName;

            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                Name = string.Empty,
                Composition = (short) AssessmentSectionComposition.Dike
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, entity.Name);
            Assert.AreEqual((short) composition, entity.Composition);
            Assert.IsEmpty(entity.ReferenceLinePointEntities);
            Assert.IsEmpty(entity.HydraulicLocationEntities);
            Assert.IsNull(entity.HydraulicDatabaseLocation);
            Assert.IsNull(entity.HydraulicDatabaseVersion);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithNewReferenceLine_ReferenceLineAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 2)
            });
            var section = InitializeCreatedDikeAssessmentSection();
            section.ReferenceLine = referenceLine;

            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, entity.ReferenceLinePointEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithExistingReferenceLine_PointsRemovedFromContextAndNewPointAddedToEntity()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            ReferenceLine referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 2)
            });
            var section = InitializeCreatedDikeAssessmentSection();
            section.ReferenceLine = referenceLine;

            var referenceLinePointEntity = new ReferenceLinePointEntity
            {
                X = 2, Y = 3
            };
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                ReferenceLinePointEntities =
                {
                    referenceLinePointEntity
                }
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            ringtoetsEntities.ReferenceLinePointEntities.Add(referenceLinePointEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(0, ringtoetsEntities.ReferenceLinePointEntities.Count());
            Assert.AreEqual(1, entity.ReferenceLinePointEntities.Count(p => p.X == 1 && p.Y == 2));

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithNewHydraulicDatabase_PropertiesUpdatedAndLocationsAdded()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var newVersion = "new version";
            var filePath = "new path";
            var section = InitializeCreatedDikeAssessmentSection();
            section.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                Version = newVersion,
                Locations =
                {
                    new HydraulicBoundaryLocation(-1, string.Empty, 0, 0)
                }
            };

            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(filePath, entity.HydraulicDatabaseLocation);
            Assert.AreEqual(newVersion, entity.HydraulicDatabaseVersion);
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithUpdatedHydraulicDatabase_PropertiesUpdatedAndLocationUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var newVersion = "new version";
            var filePath = "new path";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, string.Empty, 0, 0)
            {
                StorageId = 1
            };
            var section = InitializeCreatedDikeAssessmentSection();
            section.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = filePath,
                Version = newVersion,
                Locations =
                {
                    hydraulicBoundaryLocation
                }
            };

            var hydraulicLocationEntity = new HydraulicLocationEntity
            {
                HydraulicLocationEntityId = 1
            };
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                HydraulicDatabaseLocation = "old location",
                HydraulicDatabaseVersion = "old version",
                HydraulicLocationEntities =
                {
                    hydraulicLocationEntity
                }
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);
            ringtoetsEntities.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(filePath, entity.HydraulicDatabaseLocation);
            Assert.AreEqual(newVersion, entity.HydraulicDatabaseVersion);
            CollectionAssert.AreEqual(new[]
            {
                hydraulicLocationEntity
            }, entity.HydraulicLocationEntities);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithUpdatedPipingFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var section = InitializeCreatedDikeAssessmentSection();
            section.PipingFailureMechanism.StorageId = 1;
            section.PipingFailureMechanism.Contribution = 0.5;
            section.PipingFailureMechanism.IsRelevant = true;

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                FailureMechanismEntities =
                {
                    failureMechanismEntity
                }
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanismEntity
            }, entity.FailureMechanismEntities);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithUpdatedGrassCoverErosionInwardsFailureMechanism_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var section = InitializeCreatedDikeAssessmentSection();
            section.GrassCoverErosionInwards.Contribution = 0.5;
            section.GrassCoverErosionInwards.IsRelevant = true;

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                FailureMechanismEntities =
                {
                    failureMechanismEntity
                }
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanismEntity
            }, entity.FailureMechanismEntities);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithUpdatedStandAloneFailureMechanisms_PropertiesUpdated()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.Create(mocks);

            mocks.ReplayAll();

            var section = InitializeCreatedDikeAssessmentSection();
            section.StorageId = 1;
            section.MacrostabilityInwards.StorageId = 1;
            section.MacrostabilityInwards.Contribution = 0.5;
            section.MacrostabilityInwards.IsRelevant = true;
            section.HeightStructure.StorageId = 2;
            section.HeightStructure.Contribution = 0.5;
            section.HeightStructure.IsRelevant = true;
            section.ClosingStructure.StorageId = 3;
            section.ClosingStructure.Contribution = 0.5;
            section.ClosingStructure.IsRelevant = true;
            section.StrengthStabilityPointConstruction.StorageId = 4;
            section.StrengthStabilityPointConstruction.Contribution = 0.5;
            section.StrengthStabilityPointConstruction.IsRelevant = true;
            section.StabilityStoneCover.StorageId = 5;
            section.StabilityStoneCover.Contribution = 0.5;
            section.StabilityStoneCover.IsRelevant = true;
            section.WaveImpactAsphaltCover.StorageId = 6;
            section.WaveImpactAsphaltCover.Contribution = 0.5;
            section.WaveImpactAsphaltCover.IsRelevant = true;
            section.GrassCoverErosionOutside.StorageId = 7;
            section.GrassCoverErosionOutside.Contribution = 0.5;
            section.GrassCoverErosionOutside.IsRelevant = true;
            section.DuneErosion.StorageId = 8;
            section.DuneErosion.Contribution = 0.5;
            section.DuneErosion.IsRelevant = true;

            var macrostabilityInwardsEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 1,
                IsRelevant = Convert.ToByte(false)
            };
            var overtoppingEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 2,
                IsRelevant = Convert.ToByte(false)
            };
            var closingEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 3,
                IsRelevant = Convert.ToByte(false)
            };
            var failingOfConstructionEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 4,
                IsRelevant = Convert.ToByte(false)
            };
            var stoneRevetmentEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 5,
                IsRelevant = Convert.ToByte(false)
            };
            var asphaltRevetmentEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 6,
                IsRelevant = Convert.ToByte(false)
            };
            var grassRevetmentEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 7,
                IsRelevant = Convert.ToByte(false)
            };
            var duneErosionEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = 8,
                IsRelevant = Convert.ToByte(false)
            };
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                FailureMechanismEntities =
                {
                    macrostabilityInwardsEntity,
                    overtoppingEntity,
                    closingEntity,
                    failingOfConstructionEntity,
                    stoneRevetmentEntity,
                    asphaltRevetmentEntity,
                    grassRevetmentEntity,
                    duneErosionEntity
                }
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            ringtoetsEntities.FailureMechanismEntities.Add(macrostabilityInwardsEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(overtoppingEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(closingEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(failingOfConstructionEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(stoneRevetmentEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(asphaltRevetmentEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(grassRevetmentEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(duneErosionEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new UpdateConversionCollector(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                macrostabilityInwardsEntity,
                overtoppingEntity,
                closingEntity,
                failingOfConstructionEntity,
                stoneRevetmentEntity,
                asphaltRevetmentEntity,
                grassRevetmentEntity,
                duneErosionEntity
            }, entity.FailureMechanismEntities);

            mocks.VerifyAll();
        }

        private AssessmentSection InitializeCreatedDikeAssessmentSection(AssessmentSectionComposition composition = AssessmentSectionComposition.Dike)
        {
            return new AssessmentSection(composition)
            {
                StorageId = 1,
                PipingFailureMechanism =
                {
                    StorageId = 1
                },
                GrassCoverErosionInwards =
                {
                    StorageId = 2
                },
                MacrostabilityInwards =
                {
                    StorageId = 3
                },
                HeightStructure =
                {
                    StorageId = 4
                },
                ClosingStructure =
                {
                    StorageId = 5
                },
                StrengthStabilityPointConstruction =
                {
                    StorageId = 6
                },
                StabilityStoneCover =
                {
                    StorageId = 7
                },
                WaveImpactAsphaltCover =
                {
                    StorageId = 8
                },
                GrassCoverErosionOutside =
                {
                    StorageId = 9
                },
                GrassCoverSlipOffOutside =
                {
                    StorageId = 10
                },
                PipingStructure =
                {
                    StorageId = 11
                },
                DuneErosion =
                {
                    StorageId = 12
                }
            };
        }

        private void FillWithFailureMechanismEntities(DbSet<FailureMechanismEntity> failureMechanismEntities)
        {
            var failureMechanismEntity = failureMechanismEntities.LastOrDefault();
            long startId = 0;
            if (failureMechanismEntity != null)
            {
                startId = failureMechanismEntity.FailureMechanismEntityId;
            }

            var count = failureMechanismEntities.Count();
            for (var i = 1; i <= 12 - count; i++)
            {
                failureMechanismEntities.Add(new FailureMechanismEntity
                {
                    FailureMechanismEntityId = startId + i
                });
            }
        }
    }
}