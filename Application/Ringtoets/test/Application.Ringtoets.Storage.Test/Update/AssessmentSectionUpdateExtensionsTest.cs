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
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.TestUtil;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Update
{
    [TestFixture]
    public class AssessmentSectionUpdateExtensionsTest
    {
        private readonly int totalAmountOfFailureMechanismsInAssessmentSection = 18;

        [Test]
        public void Update_WithoutContext_ThrowsArgumentNullException()
        {
            // Setup
            var section = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () => section.Update(new PersistenceRegistry(), null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("context", paramName);
        }

        [Test]
        public void Update_WithoutPersistenceRegistry_ThrowsArgumentNullException()
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
            Assert.AreEqual("registry", paramName);
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
                    section.Update(new PersistenceRegistry(), ringtoetsEntities);
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            TestDelegate test = () => section.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();

            const string newName = "newName";
            const string comments = "Some text";
            const int norm = int.MaxValue;
            var composition = AssessmentSectionComposition.Dune;
            var section = InitializeCreatedDikeAssessmentSection(AssessmentSectionComposition.Dune);
            section.Name = newName;
            section.Comments = comments;
            section.FailureMechanismContribution.Norm = norm;

            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                Name = string.Empty,
                Composition = (short) AssessmentSectionComposition.Dike
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(newName, entity.Name);
            Assert.AreEqual(comments, entity.Comments);
            Assert.AreEqual(norm, entity.Norm);
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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            Assert.AreEqual(1, entity.ReferenceLinePointEntities.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void Update_AssessmentSectionWithExistingReferenceLine_PointsRemovedFromContextAndNewPointAddedToEntity()
        {
            // Setup
            MockRepository mocks = new MockRepository();
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            ringtoetsEntities.ReferenceLinePointEntities.Add(referenceLinePointEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);
            ringtoetsEntities.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

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
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });
            ringtoetsEntities.FailureMechanismEntities.Add(failureMechanismEntity);
            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

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
            var ringtoetsEntities = RingtoetsEntitiesHelper.CreateStub(mocks);

            mocks.ReplayAll();
            var section = InitializeCreatedDikeAssessmentSection();
            section.StorageId = 1;

            int id = 1;
            var macrostabilityInwardsEntity = InitializeModelAndCreateEntity(section.MacrostabilityInwards, id++);
            var macrostabilityOutwardsEntity = InitializeModelAndCreateEntity(section.MacrostabilityOutwards, id++);
            var microstabilityEntity = InitializeModelAndCreateEntity(section.Microstability, id++);
            var stabilityStoneCoverEntity = InitializeModelAndCreateEntity(section.StabilityStoneCover, id++);
            var waveImpactAsphaltCoverEntity = InitializeModelAndCreateEntity(section.WaveImpactAsphaltCover, id++);
            var waterPressureAsphaltCoverEntity = InitializeModelAndCreateEntity(section.WaterPressureAsphaltCover, id++);
            var grassCoverErosionOutwardsEntity = InitializeModelAndCreateEntity(section.GrassCoverErosionOutwards, id++);
            var grassCoverSlipOffOutwardsEntity = InitializeModelAndCreateEntity(section.GrassCoverSlipOffOutwards, id++);
            var grassCoverSlipOffInwardsEntity = InitializeModelAndCreateEntity(section.GrassCoverSlipOffInwards, id++);
            var heightStructuresEntity = InitializeModelAndCreateEntity(section.HeightStructures, id++);
            var closingStructuresEntity = InitializeModelAndCreateEntity(section.ClosingStructure, id++);
            var strengthStabilityPointConstructionEntity = InitializeModelAndCreateEntity(section.StrengthStabilityPointConstruction, id++);
            var strengthStabilityLengthwiseConstructionEntity = InitializeModelAndCreateEntity(section.StrengthStabilityLengthwiseConstruction, id++);
            var duneErosionEntity = InitializeModelAndCreateEntity(section.DuneErosion, id++);
            var technicalInnovationEntity = InitializeModelAndCreateEntity(section.TechnicalInnovation, id++);

            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = 1,
                FailureMechanismEntities =
                {
                    macrostabilityInwardsEntity,
                    macrostabilityOutwardsEntity,
                    microstabilityEntity,
                    stabilityStoneCoverEntity,
                    waveImpactAsphaltCoverEntity,
                    waterPressureAsphaltCoverEntity,
                    grassCoverErosionOutwardsEntity,
                    grassCoverSlipOffOutwardsEntity,
                    grassCoverSlipOffInwardsEntity,
                    heightStructuresEntity,
                    closingStructuresEntity,
                    strengthStabilityPointConstructionEntity,
                    strengthStabilityLengthwiseConstructionEntity,
                    duneErosionEntity,
                    technicalInnovationEntity
                }
            };
            ringtoetsEntities.AssessmentSectionEntities.Add(entity);
            ringtoetsEntities.CalculationGroupEntities.Add(new CalculationGroupEntity
            {
                CalculationGroupEntityId = section.PipingFailureMechanism.CalculationsGroup.StorageId
            });
            ringtoetsEntities.PipingFailureMechanismMetaEntities.Add(new PipingFailureMechanismMetaEntity
            {
                PipingFailureMechanismMetaEntityId = section.PipingFailureMechanism.PipingProbabilityAssessmentInput.StorageId,
                FailureMechanismEntityId = section.PipingFailureMechanism.StorageId
            });

            ringtoetsEntities.FailureMechanismEntities.Add(macrostabilityInwardsEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(macrostabilityOutwardsEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(microstabilityEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(stabilityStoneCoverEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(waveImpactAsphaltCoverEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(waterPressureAsphaltCoverEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(grassCoverErosionOutwardsEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(grassCoverSlipOffOutwardsEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(grassCoverSlipOffInwardsEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(heightStructuresEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(closingStructuresEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(strengthStabilityPointConstructionEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(strengthStabilityLengthwiseConstructionEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(duneErosionEntity);
            ringtoetsEntities.FailureMechanismEntities.Add(technicalInnovationEntity);

            FillWithFailureMechanismEntities(ringtoetsEntities.FailureMechanismEntities);

            // Call
            section.Update(new PersistenceRegistry(), ringtoetsEntities);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                macrostabilityInwardsEntity,
                macrostabilityOutwardsEntity,
                microstabilityEntity,
                stabilityStoneCoverEntity,
                waveImpactAsphaltCoverEntity,
                waterPressureAsphaltCoverEntity,
                grassCoverErosionOutwardsEntity,
                grassCoverSlipOffOutwardsEntity,
                grassCoverSlipOffInwardsEntity,
                heightStructuresEntity,
                closingStructuresEntity,
                strengthStabilityPointConstructionEntity,
                strengthStabilityLengthwiseConstructionEntity,
                duneErosionEntity,
                technicalInnovationEntity
            }, entity.FailureMechanismEntities);

            mocks.VerifyAll();
        }

        private FailureMechanismEntity InitializeModelAndCreateEntity(IFailureMechanism failureMechanism, long id)
        {
            failureMechanism.StorageId = id;
            failureMechanism.IsRelevant = true;
            failureMechanism.Contribution = new Random(21).NextDouble();

            return new FailureMechanismEntity
            {
                FailureMechanismEntityId = id,
                IsRelevant = Convert.ToByte(false)
            };
        }

        private AssessmentSection InitializeCreatedDikeAssessmentSection(AssessmentSectionComposition composition = AssessmentSectionComposition.Dike)
        {
            var failureMechanismId = 1;
            return new AssessmentSection(composition)
            {
                StorageId = 1,
                PipingFailureMechanism =
                {
                    StorageId = failureMechanismId++,
                    CalculationsGroup =
                    {
                        StorageId = 1
                    },
                    PipingProbabilityAssessmentInput =
                    {
                        StorageId = 1
                    }
                },
                GrassCoverErosionInwards =
                {
                    StorageId = failureMechanismId++
                },
                MacrostabilityInwards =
                {
                    StorageId = failureMechanismId++
                },
                MacrostabilityOutwards =
                {
                    StorageId = failureMechanismId++
                },
                Microstability =
                {
                    StorageId = failureMechanismId++
                },
                HeightStructures =
                {
                    StorageId = failureMechanismId++
                },
                ClosingStructure =
                {
                    StorageId = failureMechanismId++
                },
                StrengthStabilityPointConstruction =
                {
                    StorageId = failureMechanismId++
                },
                StrengthStabilityLengthwiseConstruction =
                {
                    StorageId = failureMechanismId++
                },
                StabilityStoneCover =
                {
                    StorageId = failureMechanismId++
                },
                WaveImpactAsphaltCover =
                {
                    StorageId = failureMechanismId++
                },
                WaterPressureAsphaltCover =
                {
                    StorageId = failureMechanismId++
                },
                GrassCoverErosionOutwards =
                {
                    StorageId = failureMechanismId++
                },
                GrassCoverSlipOffOutwards =
                {
                    StorageId = failureMechanismId++
                },
                GrassCoverSlipOffInwards =
                {
                    StorageId = failureMechanismId++
                },
                PipingStructure =
                {
                    StorageId = failureMechanismId++
                },
                DuneErosion =
                {
                    StorageId = failureMechanismId++
                },
                TechnicalInnovation =
                {
                    StorageId = failureMechanismId++
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
            for (var i = 1; i <= totalAmountOfFailureMechanismsInAssessmentSection - count; i++)
            {
                failureMechanismEntities.Add(new FailureMechanismEntity
                {
                    FailureMechanismEntityId = startId + i
                });
            }
        }
    }
}