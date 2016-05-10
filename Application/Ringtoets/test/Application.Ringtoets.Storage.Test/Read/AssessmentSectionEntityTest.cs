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
        public void Read_WithFailureMechanismPlaceholdersSet_ReturnsNewAssessmentSectionWithFailureMechanismsSet(bool isRelevant)
        {
            // Setup
            var entity = new AssessmentSectionEntity();
            var macrostabilityInwardsEntityId = 2;
            var overtoppingEntityId = 3;
            var closingEntityId = 16;
            var failingOfConstructionEntityId = 22;
            var stoneRevetmentEntityId = 36;
            var asphaltRevetmentEntityId = 77;
            var grassRevetmentEntityId = 133;
            var duneErosionEntityId = 256;

            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = macrostabilityInwardsEntityId,
                FailureMechanismType = (short) FailureMechanismType.MacrostabilityInwards,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = overtoppingEntityId,
                FailureMechanismType = (short) FailureMechanismType.StructureHeight,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = closingEntityId,
                FailureMechanismType = (short) FailureMechanismType.ReliabilityClosingOfStructure,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = failingOfConstructionEntityId,
                FailureMechanismType = (short) FailureMechanismType.StrengthAndStabilityPointConstruction,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = stoneRevetmentEntityId,
                FailureMechanismType = (short) FailureMechanismType.StabilityStoneRevetment,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = asphaltRevetmentEntityId,
                FailureMechanismType = (short) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = grassRevetmentEntityId,
                FailureMechanismType = (short) FailureMechanismType.GrassRevetmentErosionOutwards,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = duneErosionEntityId,
                FailureMechanismType = (short) FailureMechanismType.DuneErosion,
                IsRelevant = Convert.ToByte(isRelevant),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            });

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(macrostabilityInwardsEntityId, section.MacrostabilityInwards.StorageId);
            Assert.AreEqual(isRelevant, section.MacrostabilityInwards.IsRelevant);
            Assert.AreEqual(2, section.MacrostabilityInwards.Sections.Count());

            Assert.AreEqual(overtoppingEntityId, section.HeightStructure.StorageId);
            Assert.AreEqual(isRelevant, section.HeightStructure.IsRelevant);
            Assert.AreEqual(2, section.HeightStructure.Sections.Count());

            Assert.AreEqual(closingEntityId, section.ClosingStructure.StorageId);
            Assert.AreEqual(isRelevant, section.ClosingStructure.IsRelevant);
            Assert.AreEqual(2, section.ClosingStructure.Sections.Count());

            Assert.AreEqual(failingOfConstructionEntityId, section.StrengthStabilityPointConstruction.StorageId);
            Assert.AreEqual(isRelevant, section.StrengthStabilityPointConstruction.IsRelevant);
            Assert.AreEqual(2, section.StrengthStabilityPointConstruction.Sections.Count());

            Assert.AreEqual(stoneRevetmentEntityId, section.StabilityStoneCover.StorageId);
            Assert.AreEqual(isRelevant, section.StabilityStoneCover.IsRelevant);
            Assert.AreEqual(2, section.StabilityStoneCover.Sections.Count());

            Assert.AreEqual(asphaltRevetmentEntityId, section.WaveImpactAsphaltCover.StorageId);
            Assert.AreEqual(isRelevant, section.WaveImpactAsphaltCover.IsRelevant);
            Assert.AreEqual(2, section.WaveImpactAsphaltCover.Sections.Count());

            Assert.AreEqual(grassRevetmentEntityId, section.GrassCoverErosionOutside.StorageId);
            Assert.AreEqual(isRelevant, section.GrassCoverErosionOutside.IsRelevant);
            Assert.AreEqual(2, section.GrassCoverErosionOutside.Sections.Count());

            Assert.AreEqual(duneErosionEntityId, section.DuneErosion.StorageId);
            Assert.AreEqual(isRelevant, section.DuneErosion.IsRelevant);
            Assert.AreEqual(2, section.DuneErosion.Sections.Count());
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