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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class AssessmentSectionEntityTest
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
            var testName = "testName";
            var entityId = new Random(21).Next(1, 502);
            var entity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = entityId,
                Name = testName,
                Composition = (short) assessmentSectionComposition
            };
            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(entityId, section.StorageId);
            Assert.AreEqual(testName, section.Name);
            Assert.AreEqual(assessmentSectionComposition, section.Composition);
            Assert.IsNull(section.HydraulicBoundaryDatabase);
            Assert.IsNull(section.ReferenceLine);
        }

        [Test]
        public void Read_WithReferenceLineEntities_ReturnsNewAssessmentSectionWithReferenceLineSet()
        {
            // Setup
            var entity = new AssessmentSectionEntity();

            var random = new Random(21);
            double firstX = random.NextDouble();
            double firstY = random.NextDouble();
            double secondX = random.NextDouble();
            double secondY = random.NextDouble();
            entity.ReferenceLinePointEntities.Add(new ReferenceLinePointEntity
            {
                Order = 2, X = Convert.ToDecimal(firstX), Y = Convert.ToDecimal(firstY)
            });
            entity.ReferenceLinePointEntities.Add(new ReferenceLinePointEntity
            {
                Order = 1, X = Convert.ToDecimal(secondX), Y = Convert.ToDecimal(secondY)
            });

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.ReferenceLine.Points.Count());
            Assert.AreEqual(secondX, section.ReferenceLine.Points.ElementAt(0).X, 1e-6);
            Assert.AreEqual(secondY, section.ReferenceLine.Points.ElementAt(0).Y, 1e-6);
            Assert.AreEqual(firstX, section.ReferenceLine.Points.ElementAt(1).X, 1e-6);
            Assert.AreEqual(firstY, section.ReferenceLine.Points.ElementAt(1).Y, 1e-6);
        }

        [Test]
        public void Read_WithHydraulicDatabaseLocationSet_ReturnsNewAssessmentSectionWithHydraulicDatabaseSet()
        {
            // Setup
            var entity = new AssessmentSectionEntity();

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
        public void Read_WithFailureMechanismWithStochasticSoilModelsSet_ReturnsNewAssessmentSectionWithStochasticSoilModelsInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = new AssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                IsRelevant = Convert.ToByte(isRelevant),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity(),
                    new StochasticSoilModelEntity()
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
        public void Read_WithFailureMechanismWithSurfaceLinesSet_ReturnsNewAssessmentSectionWithSurfaceLinesInPipingFailureMechanism(bool isRelevant)
        {
            // Setup
            var entity = new AssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int)FailureMechanismType.Piping,
                IsRelevant = Convert.ToByte(isRelevant),
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity(),
                    new SurfaceLineEntity()
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
        public void Read_WithPipingFailureMechanismWithFailureMechanismSectionsSet_ReturnsNewAssessmentSectionWithFailureMechanismSectionsInPipingFailureMechanism()
        {
            // Setup
            var entity = new AssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.Piping,
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.PipingFailureMechanism.Sections.Count());
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsFailureMechanismWithFailureMechanismSectionsSet_ReturnsNewAssessmentSectionWithFailureMechanismSectionsInGrassCoverErosionInwardsFailureMechanism()
        {
            // Setup
            var entity = new AssessmentSectionEntity();
            var entityId = new Random(21).Next(1, 502);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
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
            var entity = new AssessmentSectionEntity();
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

        private static FailureMechanismEntity CreateFailureMechanismEntity(bool isRelevant, int entityId, FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                FailureMechanismType = (short) failureMechanismType,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
        }

        private static void AssertFailureMechanismEqual(bool isRelevant, int entityId, int sectionCount, IFailureMechanism failureMechanism)
        {
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(sectionCount, failureMechanism.Sections.Count());
        }

        private static FailureMechanismSectionEntity[] CreateFailureMechanismSectionEntities()
        {
            return new[]
            {
                new FailureMechanismSectionEntity
                {
                    FailureMechanismSectionEntityId = 1,
                    Name = "",
                    FailureMechanismSectionPointEntities =
                    {
                        new FailureMechanismSectionPointEntity()
                    }
                },
                new FailureMechanismSectionEntity
                {
                    FailureMechanismSectionEntityId = 2,
                    Name = "",
                    FailureMechanismSectionPointEntities =
                    {
                        new FailureMechanismSectionPointEntity()
                    }
                }
            };
        }
    }
}