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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;

using Core.Common.Base.Geometry;

using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class AssessmentSectionEntityReadExtensionsTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new AssessmentSectionEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void Read_WithCollector_ReturnsNewAssessmentSection(AssessmentSectionComposition assessmentSectionComposition)
        {
            // Setup
            const string testId = "testId";
            const string testName = "testName";
            const string comments = "Some text";
            const int norm = int.MaxValue;
            var entityId = new Random(21).Next(1, 502);
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = entityId,
                Id = testId,
                Name = testName,
                Composition = (short) assessmentSectionComposition,
                Comments = comments,
                Norm = norm
            };
            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(entityId, section.StorageId);
            Assert.AreEqual(testId, section.Id);
            Assert.AreEqual(testName, section.Name);
            Assert.AreEqual(comments, section.Comments);
            Assert.AreEqual(norm, section.FailureMechanismContribution.Norm);
            Assert.AreEqual(assessmentSectionComposition, section.Composition);
            Assert.IsNull(section.HydraulicBoundaryDatabase);
            Assert.IsNull(section.ReferenceLine);
        }

        [Test]
        public void Read_WithReferenceLineEntities_ReturnsNewAssessmentSectionWithReferenceLineSet()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var random = new Random(21);
            double firstX = random.NextDouble();
            double firstY = random.NextDouble();
            double secondX = random.NextDouble();
            double secondY = random.NextDouble();

            var points = new[]
            {
                new Point2D(firstX, firstY),
                new Point2D(secondX, secondY)
            };
            entity.ReferenceLinePointData = new Point2DBinaryConverter().ToBytes(points);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            CollectionAssert.AreEqual(points, section.ReferenceLine.Points);
        }

        [Test]
        public void Read_WithHydraulicDatabaseLocationSet_ReturnsNewAssessmentSectionWithHydraulicDatabaseSet()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();

            var testLocation = "testLocation";
            var testVersion = "testVersion";
            entity.HydraulicDatabaseLocation = testLocation;
            entity.HydraulicDatabaseVersion = testVersion;
            entity.HydraulicLocationEntities.Add(new HydraulicLocationEntity
            {
                Name = "someName"
            });
            entity.HydraulicLocationEntities.Add(new HydraulicLocationEntity
            {
                Name = "someName"
            });

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.HydraulicBoundaryDatabase.Locations.Count);
            Assert.AreEqual(testLocation, section.HydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(testVersion, section.HydraulicBoundaryDatabase.Version);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithPipingFailureMechnismPropertiesSet_ReturnsNewAssessmentSectionWithPropertiesInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            var parameterA = random.NextDouble()/10;
            var parameterUpliftCriticalSafetyFactor = random.NextDouble() + 0.1;
            const string comments = "Some text";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = comments,
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        A = parameterA,
                        UpliftCriticalSafetyFactor = parameterUpliftCriticalSafetyFactor
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(entityId, section.PipingFailureMechanism.StorageId);
            Assert.AreEqual(isRelevant, section.PipingFailureMechanism.IsRelevant);
            Assert.AreEqual(comments, section.PipingFailureMechanism.Comments);
            Assert.AreEqual(parameterA, section.PipingFailureMechanism.PipingProbabilityAssessmentInput.A);
        }

        [Test]
        [TestCase(true, TestName = "PipingStochasticSoilModelsSet_ReturnsNewAssessmentSectionWithStochasticSoilModelsInPipingFailureMechanism(true)")]
        [TestCase(false, TestName = "PipingStochasticSoilModelsSet_ReturnsNewAssessmentSectionWithStochasticSoilModelsInPipingFailureMechanism(false)")]
        public void Read_WithPipingFailureMechanismWithStochasticSoilModelsSet_ReturnsNewAssessmentSectionWithStochasticSoilModelsInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            byte[] emptySegmentPointsData = new Point2DBinaryConverter().ToBytes(new Point2D[0]);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                IsRelevant = Convert.ToByte(isRelevant),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointData = emptySegmentPointsData
                    },
                    new StochasticSoilModelEntity
                    {
                        StochasticSoilModelSegmentPointData = emptySegmentPointsData
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.StochasticSoilModels.Count);
            Assert.AreEqual(entityId, section.PipingFailureMechanism.StorageId);
            Assert.AreEqual(isRelevant, section.PipingFailureMechanism.IsRelevant);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithPipingFailureMechanismWithSurfaceLinesSet_ReturnsNewAssessmentSectionWithSurfaceLinesInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            var emptyPointsData = new Point3DBinaryConverter().ToBytes(new Point3D[0]);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                IsRelevant = Convert.ToByte(isRelevant),
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity
                    {
                        PointsData = emptyPointsData
                    },
                    new SurfaceLineEntity
                    {
                        PointsData = emptyPointsData
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.SurfaceLines.Count);
            Assert.AreEqual(entityId, section.PipingFailureMechanism.StorageId);
            Assert.AreEqual(isRelevant, section.PipingFailureMechanism.IsRelevant);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithPipingFailureMechanismWithCalculationGroupsSet_ReturnsNewAssessmentSectionWithCalculationGroupsInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            const int rootGroupEntityId = 1;
            const int childGroupEntity1Id = 2;
            const int childGroupEntity2Id = 3;

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = rootGroupEntityId,
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            CalculationGroupEntityId = childGroupEntity1Id,
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            CalculationGroupEntityId = childGroupEntity2Id,
                            Order = 1
                        }
                    }
                },
                IsRelevant = Convert.ToByte(isRelevant)
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(rootGroupEntityId, section.PipingFailureMechanism.CalculationsGroup.StorageId);
            IList<ICalculationBase> childCalculationGroups = section.PipingFailureMechanism.CalculationsGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
            Assert.AreEqual(childGroupEntity1Id, ((CalculationGroup) childCalculationGroups[0]).StorageId);
            Assert.AreEqual(childGroupEntity2Id, ((CalculationGroup) childCalculationGroups[1]).StorageId);
        }

        [Test]
        public void Read_WithPipingFailureMechanismWithFailureMechanismSectionsSet_ReturnsNewAssessmentSectionWithFailureMechanismSectionsInPipingFailureMechanism()
        {
            // Setup
            var random = new Random(21);
            var entity = CreateAssessmentSectionEntity();
            var entityId = random.Next(1, 502);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
            var sectionA = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0);
            var sectionB = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(1);
            long pipingSectionIdA = random.Next(1, 502);
            long pipingSectionIdB = random.Next(1, 502);
            sectionA.PipingSectionResultEntities.Add(new PipingSectionResultEntity
            {
                PipingSectionResultEntityId = pipingSectionIdA,
                FailureMechanismSectionEntity = sectionA
            });
            sectionB.PipingSectionResultEntities.Add(new PipingSectionResultEntity
            {
                PipingSectionResultEntityId = pipingSectionIdB,
                FailureMechanismSectionEntity = sectionB
            });

            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.Sections.Count());
            Assert.AreEqual(pipingSectionIdA, section.PipingFailureMechanism.SectionResults.ElementAt(0).StorageId);
            Assert.AreEqual(pipingSectionIdB, section.PipingFailureMechanism.SectionResults.ElementAt(1).StorageId);
        }

        [Test]
        [TestCase(true, TestName = "GrassCoverErosionInwardsPropertiesSet_ReturnsNewAssessmentSectionWithPropertiesInGrassCoverErosionInwardsFailureMechanism(true)")]
        [TestCase(false, TestName = "GrassCoverErosionInwardsPropertiesSet_ReturnsNewAssessmentSectionWithPropertiesInGrassCoverErosionInwardsFailureMechanism(false)")]
        public void Read_WithGrassCoverErosionInwardsFailureMechanismPropertiesSet_ReturnsNewAssessmentSectionWithPropertiesInGrassCoverErosionInwardsFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);
            const string comments = "Some text";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntityId = 1
                },
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = comments,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        GrassCoverErosionInwardsFailureMechanismMetaEntityId = 2,
                        N = 1
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(entityId, section.GrassCoverErosionInwards.StorageId);
            Assert.AreEqual(isRelevant, section.GrassCoverErosionInwards.IsRelevant);
            Assert.AreEqual(comments, section.GrassCoverErosionInwards.Comments);
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsFailureMechanismWithFailureMechanismSectionsSet_ReturnsNewAssessmentSectionWithFailureMechanismSectionsInGrassCoverErosionInwardsFailureMechanism()
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            var rootGroupEntity = new CalculationGroupEntity();
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities(),
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        GrassCoverErosionInwardsFailureMechanismMetaEntityId = 2,
                        N = 1
                    }
                },
                CalculationGroupEntity = rootGroupEntity
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.GrassCoverErosionInwards.Sections.Count());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStandAloneFailureMechanismsSet_ReturnsNewAssessmentSectionWithFailureMechanismsSet(bool isRelevant)
        {
            // Setup
            var entity = CreateAssessmentSectionEntity();
            var macrostabilityInwardsEntityId = 2;
            var macrostabilityOutwardsEntityId = 3;
            var microstabilityEntityId = 4;
            var structureHeightEntityId = 5;
            var closingEntityId = 16;
            var failingOfConstructionPointEntityId = 22;
            var failingOfConstructionLengthwiseEntityId = 23;
            var stoneRevetmentEntityId = 36;
            var waveImpactEntityId = 77;
            var waterPressureEntityId = 78;
            var grassCoverErosionOutwardsEntityId = 133;
            var grassCoverSlipoffOutwardsEntityId = 134;
            var grassCoverSlipoffInwardsEntityId = 135;
            var duneErosionEntityId = 256;
            var technicalInnovationsEntityId = 257;

            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, macrostabilityInwardsEntityId, FailureMechanismType.MacrostabilityInwards));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, macrostabilityOutwardsEntityId, FailureMechanismType.MacrostabilityOutwards));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, microstabilityEntityId, FailureMechanismType.Microstability));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, structureHeightEntityId, FailureMechanismType.StructureHeight));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, closingEntityId, FailureMechanismType.ReliabilityClosingOfStructure));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, failingOfConstructionPointEntityId, FailureMechanismType.StrengthAndStabilityPointConstruction));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, failingOfConstructionLengthwiseEntityId, FailureMechanismType.StrengthAndStabilityParallelConstruction));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, stoneRevetmentEntityId, FailureMechanismType.StabilityStoneRevetment));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, waveImpactEntityId, FailureMechanismType.WaveImpactOnAsphaltRevetment));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, waterPressureEntityId, FailureMechanismType.WaterOverpressureAsphaltRevetment));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, grassCoverErosionOutwardsEntityId, FailureMechanismType.GrassRevetmentErosionOutwards));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, grassCoverSlipoffOutwardsEntityId, FailureMechanismType.GrassRevetmentSlidingOutwards));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, grassCoverSlipoffInwardsEntityId, FailureMechanismType.GrassRevetmentSlidingInwards));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, duneErosionEntityId, FailureMechanismType.DuneErosion));
            entity.FailureMechanismEntities.Add(CreateFailureMechanismEntity(isRelevant, technicalInnovationsEntityId, FailureMechanismType.TechnicalInnovations));

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            AssertFailureMechanismEqual(isRelevant, macrostabilityInwardsEntityId, 2, section.MacrostabilityInwards);
            AssertFailureMechanismEqual(isRelevant, macrostabilityOutwardsEntityId, 2, section.MacrostabilityOutwards);
            AssertFailureMechanismEqual(isRelevant, microstabilityEntityId, 2, section.Microstability);
            AssertFailureMechanismEqual(isRelevant, structureHeightEntityId, 2, section.HeightStructures);
            AssertFailureMechanismEqual(isRelevant, closingEntityId, 2, section.ClosingStructure);
            AssertFailureMechanismEqual(isRelevant, failingOfConstructionPointEntityId, 2, section.StrengthStabilityPointConstruction);
            AssertFailureMechanismEqual(isRelevant, failingOfConstructionLengthwiseEntityId, 2, section.StrengthStabilityLengthwiseConstruction);
            AssertFailureMechanismEqual(isRelevant, stoneRevetmentEntityId, 2, section.StabilityStoneCover);
            AssertFailureMechanismEqual(isRelevant, waveImpactEntityId, 2, section.WaveImpactAsphaltCover);
            AssertFailureMechanismEqual(isRelevant, waterPressureEntityId, 2, section.WaterPressureAsphaltCover);
            AssertFailureMechanismEqual(isRelevant, grassCoverErosionOutwardsEntityId, 2, section.GrassCoverErosionOutwards);
            AssertFailureMechanismEqual(isRelevant, grassCoverSlipoffOutwardsEntityId, 2, section.GrassCoverSlipOffOutwards);
            AssertFailureMechanismEqual(isRelevant, grassCoverSlipoffInwardsEntityId, 2, section.GrassCoverSlipOffInwards);
            AssertFailureMechanismEqual(isRelevant, duneErosionEntityId, 2, section.DuneErosion);
            AssertFailureMechanismEqual(isRelevant, technicalInnovationsEntityId, 2, section.TechnicalInnovation);
        }

        private static AssessmentSectionEntity CreateAssessmentSectionEntity()
        {
            return new AssessmentSectionEntity
            {
                Norm = 30000
            };
        }

        private static FailureMechanismEntity CreateFailureMechanismEntity(bool isRelevant, int entityId, FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (short) failureMechanismType,
                IsRelevant = Convert.ToByte(isRelevant),
                Comments = entityId.ToString(),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
        }

        private static void AssertFailureMechanismEqual(bool expectedIsRelevant, int expectedEntityId,
                                                        int expectedSectionCount, IFailureMechanism failureMechanism)
        {
            Assert.AreEqual(expectedEntityId, failureMechanism.StorageId);
            Assert.AreEqual(expectedIsRelevant, failureMechanism.IsRelevant);
            var expectedComments = expectedEntityId.ToString();
            Assert.AreEqual(expectedComments, failureMechanism.Comments);
            Assert.AreEqual(expectedSectionCount, failureMechanism.Sections.Count());
        }

        private static FailureMechanismSectionEntity[] CreateFailureMechanismSectionEntities()
        {
            var dummyPointData = new[]
            {
                new Point2D(0.0, 0.0)
            };
            byte[] dummyPointBinaryData = new Point2DBinaryConverter().ToBytes(dummyPointData);
            return new[]
            {
                new FailureMechanismSectionEntity
                {
                    FailureMechanismSectionEntityId = 1,
                    Name = "",
                    FailureMechanismSectionPointData = dummyPointBinaryData
                },
                new FailureMechanismSectionEntity
                {
                    FailureMechanismSectionEntityId = 2,
                    Name = "",
                    FailureMechanismSectionPointData = dummyPointBinaryData
                }
            };
        }
    }
}