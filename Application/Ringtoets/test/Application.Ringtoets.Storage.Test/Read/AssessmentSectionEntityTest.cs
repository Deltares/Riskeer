﻿using System;
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
                Name =  testName,
                Composition = (short)assessmentSectionComposition
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
            entity.ReferenceLinePointEntities.Add(new ReferenceLinePointEntity { Order = 2, X = Convert.ToDecimal(firstX), Y = Convert.ToDecimal(firstY) });
            entity.ReferenceLinePointEntities.Add(new ReferenceLinePointEntity { Order = 1, X = Convert.ToDecimal(secondX), Y = Convert.ToDecimal(secondY) });

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
                FailureMechanismType = (int)FailureMechanismType.Piping,
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
                FailureMechanismType = (short)FailureMechanismType.MacrostabilityInwards,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = overtoppingEntityId,
                FailureMechanismType = (short)FailureMechanismType.StructureHeight,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = closingEntityId,
                FailureMechanismType = (short)FailureMechanismType.ReliabilityClosingOfStructure,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = failingOfConstructionEntityId,
                FailureMechanismType = (short)FailureMechanismType.StrengthAndStabilityPointConstruction,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = stoneRevetmentEntityId,
                FailureMechanismType = (short)FailureMechanismType.StabilityStoneRevetment,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = asphaltRevetmentEntityId,
                FailureMechanismType = (short)FailureMechanismType.WaveImpactOnAsphaltRevetment,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = grassRevetmentEntityId,
                FailureMechanismType = (short)FailureMechanismType.GrassRevetmentErosionOutwards,
                IsRelevant = Convert.ToByte(isRelevant)
            });
            entity.FailureMechanismEntities.Add(new FailureMechanismEntity
            {
                FailureMechanismEntityId = duneErosionEntityId,
                FailureMechanismType = (short)FailureMechanismType.DuneErosion,
                IsRelevant = Convert.ToByte(isRelevant)
            });

            var collector = new ReadConversionCollector();

            // Call
            var section = entity.Read(collector);

            // Assert
            Assert.AreEqual(macrostabilityInwardsEntityId, section.MacrostabilityInwards.StorageId);
            Assert.AreEqual(isRelevant, section.MacrostabilityInwards.IsRelevant);

            Assert.AreEqual(overtoppingEntityId, section.Overtopping.StorageId);
            Assert.AreEqual(isRelevant, section.Overtopping.IsRelevant);

            Assert.AreEqual(closingEntityId, section.Closing.StorageId);
            Assert.AreEqual(isRelevant, section.Closing.IsRelevant);

            Assert.AreEqual(failingOfConstructionEntityId, section.FailingOfConstruction.StorageId);
            Assert.AreEqual(isRelevant, section.FailingOfConstruction.IsRelevant);

            Assert.AreEqual(stoneRevetmentEntityId, section.StoneRevetment.StorageId);
            Assert.AreEqual(isRelevant, section.StoneRevetment.IsRelevant);

            Assert.AreEqual(asphaltRevetmentEntityId, section.AsphaltRevetment.StorageId);
            Assert.AreEqual(isRelevant, section.AsphaltRevetment.IsRelevant);

            Assert.AreEqual(grassRevetmentEntityId, section.GrassRevetment.StorageId);
            Assert.AreEqual(isRelevant, section.GrassRevetment.IsRelevant);

            Assert.AreEqual(duneErosionEntityId, section.DuneErosion.StorageId);
            Assert.AreEqual(isRelevant, section.DuneErosion.IsRelevant);
        }
    }
}