// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.Input;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read;
using Ringtoets.Storage.Core.Serializers;
using Ringtoets.Storage.Core.TestUtil;
using Ringtoets.Storage.Core.TestUtil.Hydraulics;
using Ringtoets.Storage.Core.TestUtil.MacroStabilityInwards;

namespace Ringtoets.Storage.Core.Test.Read
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
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
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
            const double lowerLimitNorm = 0.05;
            const double signalingNorm = 0.02;
            var normativeNorm = new Random(9).NextEnumValue<NormType>();
            var entity = new AssessmentSectionEntity
            {
                Id = testId,
                Name = testName,
                Composition = Convert.ToByte(assessmentSectionComposition),
                Comments = comments,
                LowerLimitNorm = lowerLimitNorm,
                SignalingNorm = signalingNorm,
                NormativeNormType = Convert.ToByte(normativeNorm)
            };
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.IsNotNull(section);
            Assert.AreEqual(testId, section.Id);
            Assert.AreEqual(testName, section.Name);
            Assert.AreEqual(comments, section.Comments.Body);

            Assert.AreEqual(lowerLimitNorm, section.FailureMechanismContribution.LowerLimitNorm);
            Assert.AreEqual(signalingNorm, section.FailureMechanismContribution.SignalingNorm);
            Assert.AreEqual(normativeNorm, section.FailureMechanismContribution.NormativeNorm);

            Assert.AreEqual(assessmentSectionComposition, section.Composition);
            Assert.IsNull(section.ReferenceLine);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = section.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
        }

        [Test]
        public void Read_WithBackgroundData_ReturnsNewAssessmentSectionWithBackgroundData()
        {
            // Setup
            var random = new Random(21);

            const string mapDataName = "Background";
            double transparency = random.NextDouble(0, 1);
            bool isVisible = random.NextBoolean();
            const BackgroundDataType backgroundDataType = BackgroundDataType.WellKnown;

            var wellKnownTileSource = random.NextEnumValue<RingtoetsWellKnownTileSource>();
            string wellKnownTileSourceValue = ((int) wellKnownTileSource).ToString();

            var backgroundDataMetaEntities = new[]
            {
                new BackgroundDataMetaEntity
                {
                    Key = BackgroundDataIdentifiers.WellKnownTileSource,
                    Value = wellKnownTileSourceValue
                }
            };

            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var backgroundDataEntity = new BackgroundDataEntity
            {
                Name = mapDataName,
                Transparency = transparency,
                IsVisible = Convert.ToByte(isVisible),
                BackgroundDataType = Convert.ToByte(backgroundDataType),
                BackgroundDataMetaEntities = backgroundDataMetaEntities
            };

            entity.BackgroundDataEntities.Add(backgroundDataEntity);

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            BackgroundData backgroundData = section.BackgroundData;
            Assert.AreEqual(isVisible, backgroundData.IsVisible);
            Assert.AreEqual(transparency, backgroundData.Transparency,
                            backgroundData.Transparency.GetAccuracy());
            Assert.AreEqual(mapDataName, backgroundData.Name);

            Assert.IsNotNull(backgroundData.Configuration);
            var configuration = (WellKnownBackgroundDataConfiguration) backgroundData.Configuration;
            Assert.AreEqual(wellKnownTileSource, configuration.WellKnownTileSource);
        }

        [Test]
        public void Read_WithReferenceLineEntities_ReturnsNewAssessmentSectionWithReferenceLineSet()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

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
            entity.ReferenceLinePointXml = new Point2DCollectionXmlSerializer().ToXml(points);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            CollectionAssert.AreEqual(points, section.ReferenceLine.Points);
        }

        [Test]
        public void Read_WithHydraulicBoundaryLocationCalculations_ReturnsNewAssessmentSectionWithHydraulicBoundaryLocationCalculationsSet()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            const string testLocation = "testLocation";
            const string testVersion = "testVersion";
            entity.HydraulicDatabaseLocation = testLocation;
            entity.HydraulicDatabaseVersion = testVersion;
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntity.Name = "A";
            hydraulicLocationEntity.Order = 1;
            entity.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            entity.HydraulicLocationCalculationCollectionEntity = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 1);
            entity.HydraulicLocationCalculationCollectionEntity1 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 2);
            entity.HydraulicLocationCalculationCollectionEntity2 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 3);
            entity.HydraulicLocationCalculationCollectionEntity3 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 4);
            entity.HydraulicLocationCalculationCollectionEntity4 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 5);
            entity.HydraulicLocationCalculationCollectionEntity5 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 6);
            entity.HydraulicLocationCalculationCollectionEntity6 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 7);
            entity.HydraulicLocationCalculationCollectionEntity7 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 8);

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryLocation hydraulicBoundaryLocation = section.HydraulicBoundaryDatabase.Locations.Single();

            HydraulicBoundaryLocationCalculation calculation = section.WaterLevelCalculationsForFactorizedSignalingNorm.Single();
            HydraulicLocationCalculationEntity hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity7
                                                                                          .HydraulicLocationCalculationEntities
                                                                                          .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaterLevelCalculationsForSignalingNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity6
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaterLevelCalculationsForLowerLimitNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity5
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaterLevelCalculationsForFactorizedLowerLimitNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity4
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaveHeightCalculationsForFactorizedSignalingNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity3
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaveHeightCalculationsForSignalingNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity2
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaveHeightCalculationsForLowerLimitNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity1
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaveHeightCalculationsForFactorizedLowerLimitNorm.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);
        }

        [Test]
        public void Read_WithoutHydraulicBoundaryLocationDatabase_ReturnsNewAssessementSectionWithoutHydraulicBoundaryLocationAndCalculations()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = section.HydraulicBoundaryDatabase;
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);
            Assert.IsFalse(hydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(hydraulicBoundaryDatabase.UsePreprocessor);

            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForFactorizedLowerLimitNorm);
            CollectionAssert.IsEmpty(section.WaveHeightCalculationsForFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(section.WaveHeightCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(section.WaveHeightCalculationsForLowerLimitNorm);
            CollectionAssert.IsEmpty(section.WaveHeightCalculationsForFactorizedLowerLimitNorm);

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = section.GrassCoverErosionOutwards;
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            CollectionAssert.IsEmpty(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        [Test]
        public void Read_WithHydraulicDatabaseLocations_ReturnsNewAssessmentSectionWithLocationsSet()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            const string testLocation = "testLocation";
            const string testVersion = "testVersion";
            entity.HydraulicDatabaseLocation = testLocation;
            entity.HydraulicDatabaseVersion = testVersion;
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            HydraulicLocationEntity hydraulicLocationEntityOne = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntityOne.Name = "A";
            hydraulicLocationEntityOne.Order = 1;
            entity.HydraulicLocationEntities.Add(hydraulicLocationEntityOne);
            HydraulicLocationEntity hydraulicLocationEntityTwo = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntityOne.Name = "B";
            hydraulicLocationEntityOne.Order = 0;
            entity.HydraulicLocationEntities.Add(hydraulicLocationEntityTwo);

            entity.HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity2 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity3 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity4 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity5 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity6 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity7 = new HydraulicLocationCalculationCollectionEntity();

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = section.HydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(2, hydraulicBoundaryLocations.Count());
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, hydraulicBoundaryLocations.Select(l => l.Name));

            GrassCoverErosionOutwardsFailureMechanism failureMechanism = section.GrassCoverErosionOutwards;
            AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);

            AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm);
            AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm);
            AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations, failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm);
        }

        [Test]
        public void Read_WithHydraulicBoundaryLocationDatabase_ReturnsNewAssessmentSectionWithHydraulicDatabaseSet()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            const string testLocation = "testLocation";
            const string testVersion = "testVersion";
            entity.HydraulicDatabaseLocation = testLocation;
            entity.HydraulicDatabaseVersion = testVersion;
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());
            entity.HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity2 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity3 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity4 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity5 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity6 = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity7 = new HydraulicLocationCalculationCollectionEntity();

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(testLocation, section.HydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(testVersion, section.HydraulicBoundaryDatabase.Version);
            Assert.IsFalse(section.HydraulicBoundaryDatabase.CanUsePreprocessor);
            Assert.IsFalse(section.HydraulicBoundaryDatabase.UsePreprocessor);
            Assert.IsNull(section.HydraulicBoundaryDatabase.PreprocessorDirectory);
        }

        [Test]
        public void Read_WithPipingFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double parameterA = random.NextDouble() / 10;
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        A = parameterA
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.Piping.IsRelevant);
            Assert.AreEqual(inputComments, section.Piping.InputComments.Body);
            Assert.AreEqual(outputComments, section.Piping.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.Piping.NotRelevantComments.Body);
            Assert.AreEqual(parameterA, section.Piping.PipingProbabilityAssessmentInput.A);
        }

        [Test]
        public void Read_WithPipingWithStochasticSoilModels_ReturnsPipingWithStochasticSoilModels()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var random = new Random(21);
            var geometry = new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            };
            string segmentPointXml = new Point2DCollectionXmlSerializer().ToXml(geometry);
            const string stochasticSoilModelSourcePath = "path";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        Name = "modelA",
                        StochasticSoilModelSegmentPointXml = segmentPointXml,
                        PipingStochasticSoilProfileEntities =
                        {
                            PipingStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        }
                    },
                    new StochasticSoilModelEntity
                    {
                        Name = "modelB",
                        StochasticSoilModelSegmentPointXml = segmentPointXml,
                        PipingStochasticSoilProfileEntities =
                        {
                            PipingStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        }
                    }
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        StochasticSoilModelCollectionSourcePath = stochasticSoilModelSourcePath
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.Piping.StochasticSoilModels.Count);
            Assert.AreEqual(stochasticSoilModelSourcePath, section.Piping.StochasticSoilModels.SourcePath);
        }

        [Test]
        public void Read_WithPipingWithSurfaceLines_ReturnsPipingWithSurfaceLines()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            string emptyPointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0]);
            const string surfaceLineSourcePath = "some/path";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml,
                        Name = "Line A"
                    },
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml,
                        Name = "Line B"
                    }
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity
                    {
                        SurfaceLineCollectionSourcePath = surfaceLineSourcePath
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.Piping.SurfaceLines.Count);
            Assert.AreEqual(surfaceLineSourcePath, section.Piping.SurfaceLines.SourcePath);
        }

        [Test]
        public void Read_WithPipingWithCalculationGroups_ReturnsPipingWithCalculationGroups()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                },
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            List<ICalculationBase> childCalculationGroups = section.Piping.CalculationsGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        public void Read_WithPipingWithFailureMechanismSections_ReturnsPipingWithFailureMechanismSections()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities(),
                PipingFailureMechanismMetaEntities =
                {
                    new PipingFailureMechanismMetaEntity()
                }
            };
            FailureMechanismSectionEntity sectionA = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0);
            FailureMechanismSectionEntity sectionB = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(1);
            sectionA.PipingSectionResultEntities.Add(new PipingSectionResultEntity
            {
                FailureMechanismSectionEntity = sectionA
            });
            sectionB.PipingSectionResultEntities.Add(new PipingSectionResultEntity
            {
                FailureMechanismSectionEntity = sectionB
            });

            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.Piping.Sections.Count());
        }

        [Test]
        public void Read_WithMacroStabilityInwardsFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInMacroStabilityInwardsFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double parameterA = random.NextDouble();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        A = parameterA
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.MacroStabilityInwards.IsRelevant);
            Assert.AreEqual(inputComments, section.MacroStabilityInwards.InputComments.Body);
            Assert.AreEqual(outputComments, section.MacroStabilityInwards.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.MacroStabilityInwards.NotRelevantComments.Body);

            MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput = section.MacroStabilityInwards
                                                                                                .MacroStabilityInwardsProbabilityAssessmentInput;
            Assert.AreEqual(parameterA, probabilityAssessmentInput.A);
        }

        [Test]
        public void Read_WithMacroStabilityInwardsWithStochasticSoilModels_ReturnsMacroStabilityInwardsWithStochasticSoilModels()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var random = new Random(21);
            string segmentPointsXml = new Point2DCollectionXmlSerializer().ToXml(new[]
            {
                new Point2D(random.NextDouble(), random.NextDouble())
            });
            const string stochasticSoilModelSourcePath = "path";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity
                    {
                        Name = "modelA",
                        StochasticSoilModelSegmentPointXml = segmentPointsXml,
                        MacroStabilityInwardsStochasticSoilProfileEntities =
                        {
                            MacroStabilityInwardsStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        }
                    },
                    new StochasticSoilModelEntity
                    {
                        Name = "modelB",
                        StochasticSoilModelSegmentPointXml = segmentPointsXml,
                        MacroStabilityInwardsStochasticSoilProfileEntities =
                        {
                            MacroStabilityInwardsStochasticSoilProfileEntityTestFactory.CreateStochasticSoilProfileEntity()
                        }
                    }
                },
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        StochasticSoilModelCollectionSourcePath = stochasticSoilModelSourcePath
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.MacroStabilityInwards.StochasticSoilModels.Count);
            Assert.AreEqual(stochasticSoilModelSourcePath, section.MacroStabilityInwards.StochasticSoilModels.SourcePath);
        }

        [Test]
        public void Read_WithMacroStabilityInwardsWithSurfaceLines_ReturnsMacroStabilityInwardsWithSurfaceLines()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            string emptyPointsXml = new Point3DCollectionXmlSerializer().ToXml(new Point3D[0]);
            const string surfaceLineSourcePath = "some/path";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                SurfaceLineEntities =
                {
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml,
                        Name = "Line A"
                    },
                    new SurfaceLineEntity
                    {
                        PointsXml = emptyPointsXml,
                        Name = "Line B"
                    }
                },
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity
                    {
                        SurfaceLineCollectionSourcePath = surfaceLineSourcePath
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.MacroStabilityInwards.SurfaceLines.Count);
            Assert.AreEqual(surfaceLineSourcePath, section.MacroStabilityInwards.SurfaceLines.SourcePath);
        }

        [Test]
        public void Read_WithMacroStabilityInwardsWithFailureMechanismSections_ReturnsMacroStabilityInwardsWithFailureMechanismSections()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities(),
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity()
                }
            };
            FailureMechanismSectionEntity sectionA = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(0);
            FailureMechanismSectionEntity sectionB = failureMechanismEntity.FailureMechanismSectionEntities.ElementAt(1);
            sectionA.MacroStabilityInwardsSectionResultEntities.Add(new MacroStabilityInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = sectionA
            });
            sectionB.MacroStabilityInwardsSectionResultEntities.Add(new MacroStabilityInwardsSectionResultEntity
            {
                FailureMechanismSectionEntity = sectionB
            });

            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.MacroStabilityInwards.Sections.Count());
        }

        [Test]
        public void Read_WithMacroStabilityOutwardsFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInMacroStabilityOutwardsFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double parameterA = random.NextDouble();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityOutwards,
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                MacroStabilityOutwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityOutwardsFailureMechanismMetaEntity
                    {
                        A = parameterA
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.MacroStabilityOutwards.IsRelevant);
            Assert.AreEqual(inputComments, section.MacroStabilityOutwards.InputComments.Body);
            Assert.AreEqual(outputComments, section.MacroStabilityOutwards.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.MacroStabilityOutwards.NotRelevantComments.Body);

            MacroStabilityOutwardsProbabilityAssessmentInput probabilityAssessmentInput = section.MacroStabilityOutwards
                                                                                                 .MacroStabilityOutwardsProbabilityAssessmentInput;
            Assert.AreEqual(parameterA, probabilityAssessmentInput.A);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithGrassCoverErosionInwardsWithProperties_ReturnsGrassCoverErosionInwardsWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string originalInput = "Some input text";
            const string originalOutput = "Some output text";
            const string originalNotRelevantText = "Really not relevant";

            RoundedDouble n = new Random(39).NextRoundedDouble(1.0, 20.0);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = originalInput,
                OutputComments = originalOutput,
                NotRelevantComments = originalNotRelevantText,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = n
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.GrassCoverErosionInwards.IsRelevant);
            Assert.AreEqual(originalInput, section.GrassCoverErosionInwards.InputComments.Body);
            Assert.AreEqual(originalOutput, section.GrassCoverErosionInwards.OutputComments.Body);
            Assert.AreEqual(originalNotRelevantText, section.GrassCoverErosionInwards.NotRelevantComments.Body);

            RoundedDouble actualN = section.GrassCoverErosionInwards.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsWithCalculationGroups_ReturnsGrassCoverErosionInwardsWithCalculationGroups()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            List<ICalculationBase> childCalculationGroups = section.GrassCoverErosionInwards.CalculationsGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsWithFailureMechanismSection_ReturnsGrassCoverErosionInwardsWithFailureMechanismSections()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var rootGroupEntity = new CalculationGroupEntity();
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities(),
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity
                    {
                        N = 1
                    }
                },
                CalculationGroupEntity = rootGroupEntity
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.GrassCoverErosionInwards.Sections.Count());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithGrassCoverErosionOutwardsWithProperties_ReturnsGrassCoverErosionOutwardsWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            RoundedDouble n = new Random(39).NextRoundedDouble(1.0, 20.0);
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = n,
                        HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity2 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity3 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity4 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity5 = new HydraulicLocationCalculationCollectionEntity()
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.GrassCoverErosionOutwards.IsRelevant);
            Assert.AreEqual(inputComments, section.GrassCoverErosionOutwards.InputComments.Body);
            Assert.AreEqual(outputComments, section.GrassCoverErosionOutwards.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.GrassCoverErosionOutwards.NotRelevantComments.Body);

            RoundedDouble actualN = section.GrassCoverErosionOutwards.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithGrassCoverErosionOutwardsWithWaveConditionsCalculationGroups_ReturnsGrassCoverErosionOutwardsWithWaveConditionsCalculationGroups()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 1,
                        HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity2 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity3 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity4 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity5 = new HydraulicLocationCalculationCollectionEntity()
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            List<ICalculationBase> childCalculationGroups = section.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        public void Read_WithGrassCoverErosionOutwardsWithForeshoreProfile_ReturnsGrassCoverErosionOutwardsWithForeshoreProfiles()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            const string profileAId = "profileA";
            const string profileBId = "profileB";
            const string fileLocation = "some/location";
            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
                    {
                        N = 2,
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity2 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity3 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity4 = new HydraulicLocationCalculationCollectionEntity(),
                        HydraulicLocationCalculationCollectionEntity5 = new HydraulicLocationCalculationCollectionEntity()
                    }
                },
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Order = 1,
                        Id = profileAId,
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    },
                    new ForeshoreProfileEntity
                    {
                        Order = 0,
                        Id = profileBId,
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = section.GrassCoverErosionOutwards.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                profileBId,
                profileAId
            }, foreshoreProfiles.Select(fp => fp.Id));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStabilityStoneCoverWithProperties_ReturnsStabilityStoneCoverWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";
            RoundedDouble n = new Random(39).NextRoundedDouble(1.0, 20.0);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        N = n
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.StabilityStoneCover.IsRelevant);
            Assert.AreEqual(inputComments, section.StabilityStoneCover.InputComments.Body);
            Assert.AreEqual(outputComments, section.StabilityStoneCover.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.StabilityStoneCover.NotRelevantComments.Body);

            RoundedDouble actualN = section.StabilityStoneCover.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithStabilityStoneCoverWithWaveConditionsCalculationGroups_ReturnsStabilityStoneCoverWithWaveConditionsCalculationGroups()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                },
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        N = 3.9
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            List<ICalculationBase> childCalculationGroups = section.StabilityStoneCover.WaveConditionsCalculationGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStabilityStoneCoverWithForeshoreProfiles_ReturnsStabilityStoneCoverWithForeshoreProfiles(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            const string profileAId = "profileA";
            const string profileBId = "profileB";
            const string fileLocation = "some/file/location";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Order = 1,
                        Id = profileAId,
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    },
                    new ForeshoreProfileEntity
                    {
                        Order = 0,
                        Id = profileBId,
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    }
                },
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        N = 3.9
                    }
                },
                IsRelevant = Convert.ToByte(isRelevant)
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = section.StabilityStoneCover.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                profileBId,
                profileAId
            }, foreshoreProfiles.Select(fp => fp.Id));
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithWaveImpactAsphaltCoverWithProperties_ReturnsWaveImpactAsphaltCoverWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";
            RoundedDouble deltaL = new Random(39).NextRoundedDouble(1.0, 2000.0);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        DeltaL = deltaL
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.WaveImpactAsphaltCover.IsRelevant);
            Assert.AreEqual(inputComments, section.WaveImpactAsphaltCover.InputComments.Body);
            Assert.AreEqual(outputComments, section.WaveImpactAsphaltCover.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.WaveImpactAsphaltCover.NotRelevantComments.Body);

            RoundedDouble actualDeltaL = section.WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.DeltaL;
            Assert.AreEqual(deltaL, actualDeltaL, actualDeltaL.GetAccuracy());
        }

        [Test]
        public void Read_WithWaveImpactAsphaltCoverWithForeshoreProfiles_ReturnsWaveImpactAsphaltCoverWithForeshoreProfiles()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            const string profileAId = "profileA";
            const string profileBId = "profileB";
            const string fileLocation = "some/location";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                ForeshoreProfileEntities =
                {
                    new ForeshoreProfileEntity
                    {
                        Order = 1,
                        Id = profileAId,
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    },
                    new ForeshoreProfileEntity
                    {
                        Order = 0,
                        Id = profileBId,
                        GeometryXml = new Point2DCollectionXmlSerializer().ToXml(Enumerable.Empty<Point2D>())
                    }
                },
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        ForeshoreProfileCollectionSourcePath = fileLocation,
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            ForeshoreProfileCollection foreshoreProfiles = section.WaveImpactAsphaltCover.ForeshoreProfiles;
            Assert.AreEqual(fileLocation, foreshoreProfiles.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                profileBId,
                profileAId
            }, foreshoreProfiles.Select(fp => fp.Id));
        }

        [Test]
        public void Read_WithWaveImpactAsphaltCoverWithWaveConditionsCalculationGroups_ReturnsWaveImpactAsphaltCoverWithWaveConditionsCalculationGroups()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity
                {
                    CalculationGroupEntity1 =
                    {
                        new CalculationGroupEntity
                        {
                            Order = 0
                        },
                        new CalculationGroupEntity
                        {
                            Order = 1
                        }
                    }
                },
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity
                    {
                        DeltaL = new Random(39).NextRoundedDouble(1.0, 2000.0)
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            List<ICalculationBase> childCalculationGroups = section.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children;
            Assert.AreEqual(2, childCalculationGroups.Count);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithHeightStructuresWithProperties_ReturnsHeightStructuresWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";
            RoundedDouble n = new Random(39).NextRoundedDouble(1.0, 20.0);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StructureHeight,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity
                    {
                        N = n
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.HeightStructures.IsRelevant);
            Assert.AreEqual(inputComments, section.HeightStructures.InputComments.Body);
            Assert.AreEqual(outputComments, section.HeightStructures.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.HeightStructures.NotRelevantComments.Body);

            RoundedDouble actualN = section.HeightStructures.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithClosingStructuresWithProperties_ReturnsClosingStructuresWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";
            int n2a = new Random(39).Next(1, 40);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.ReliabilityClosingOfStructure,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity
                    {
                        N2A = n2a
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.ClosingStructures.IsRelevant);
            Assert.AreEqual(inputComments, section.ClosingStructures.InputComments.Body);
            Assert.AreEqual(outputComments, section.ClosingStructures.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.ClosingStructures.NotRelevantComments.Body);
            Assert.AreEqual(n2a, section.ClosingStructures.GeneralInput.N2A);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithStabilityPointStructuresWithProperties_ReturnsStabilityPointStructuresWithProperties(bool isRelevant)
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";
            RoundedDouble n = new Random(39).NextRoundedDouble(1.0, 20.0);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityPointStructures,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity
                    {
                        N = n
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.StabilityPointStructures.IsRelevant);
            Assert.AreEqual(inputComments, section.StabilityPointStructures.InputComments.Body);
            Assert.AreEqual(outputComments, section.StabilityPointStructures.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.StabilityPointStructures.NotRelevantComments.Body);

            RoundedDouble actualN = section.StabilityPointStructures.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithDuneErosionWithProperties_ReturnsDuneErosionWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";
            var random = new Random(39);
            bool isRelevant = random.NextBoolean();
            RoundedDouble n = random.NextRoundedDouble(1.0, 20.0);

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.DuneErosion,
                CalculationGroupEntity = new CalculationGroupEntity(),
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                DuneErosionFailureMechanismMetaEntities =
                {
                    new DuneErosionFailureMechanismMetaEntity
                    {
                        N = n,
                        DuneLocationCalculationCollectionEntity = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity1 = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity2 = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity3 = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity4 = new DuneLocationCalculationCollectionEntity()
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.DuneErosion.IsRelevant);
            Assert.AreEqual(inputComments, section.DuneErosion.InputComments.Body);
            Assert.AreEqual(outputComments, section.DuneErosion.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.DuneErosion.NotRelevantComments.Body);

            RoundedDouble actualN = section.DuneErosion.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithDuneErosionWithFailureMechanismSection_ReturnsDuneErosionWithFailureMechanismSections()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.DuneErosion,
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities(),
                DuneErosionFailureMechanismMetaEntities =
                {
                    new DuneErosionFailureMechanismMetaEntity
                    {
                        N = 1,
                        DuneLocationCalculationCollectionEntity = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity1 = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity2 = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity3 = new DuneLocationCalculationCollectionEntity(),
                        DuneLocationCalculationCollectionEntity4 = new DuneLocationCalculationCollectionEntity()
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, section.DuneErosion.Sections.Count());
        }

        [Test]
        public void Read_WithPipingStructureFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingStructureFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool isRelevant = random.NextBoolean();
            double parameterN = 1 + random.NextDouble();
            const string inputComments = "Some input text";
            const string outputComments = "Some output text";
            const string notRelevantComments = "Really not relevant";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.PipingAtStructure,
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = inputComments,
                OutputComments = outputComments,
                NotRelevantComments = notRelevantComments,
                PipingStructureFailureMechanismMetaEntities =
                {
                    new PipingStructureFailureMechanismMetaEntity
                    {
                        N = parameterN
                    }
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(isRelevant, section.PipingStructure.IsRelevant);
            Assert.AreEqual(inputComments, section.PipingStructure.InputComments.Body);
            Assert.AreEqual(outputComments, section.PipingStructure.OutputComments.Body);
            Assert.AreEqual(notRelevantComments, section.PipingStructure.NotRelevantComments.Body);

            RoundedDouble actualN = section.PipingStructure.N;
            Assert.AreEqual(parameterN, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithStandAloneFailureMechanisms_ReturnsNewAssessmentSectionWithFailureMechanismsSet()
        {
            // Setup
            var random = new Random(31);
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            bool microstabilityIsRelevant = random.NextBoolean();
            bool strengthAndStabilityParallelConstructionIsRelevant = random.NextBoolean();
            bool waterOverpressureAsphaltRevetmentIsRelevant = random.NextBoolean();
            bool grassRevetmentSlidingOutwardsIsRelevant = random.NextBoolean();
            bool grassRevetmentSlidingInwardsIsRelevant = random.NextBoolean();
            bool technicalInnovationsIsRelevant = random.NextBoolean();

            FailureMechanismEntity microstability = CreateFailureMechanismEntity(
                microstabilityIsRelevant,
                FailureMechanismType.Microstability);
            FailureMechanismEntity strengthAndStabilityParallelConstruction = CreateFailureMechanismEntity(
                strengthAndStabilityParallelConstructionIsRelevant,
                FailureMechanismType.StrengthAndStabilityParallelConstruction);
            FailureMechanismEntity waterOverpressureAsphaltRevetment = CreateFailureMechanismEntity(
                waterOverpressureAsphaltRevetmentIsRelevant,
                FailureMechanismType.WaterOverpressureAsphaltRevetment);
            FailureMechanismEntity grassRevetmentSlidingOutwards = CreateFailureMechanismEntity(
                grassRevetmentSlidingOutwardsIsRelevant,
                FailureMechanismType.GrassRevetmentSlidingOutwards);
            FailureMechanismEntity grassRevetmentSlidingInwards = CreateFailureMechanismEntity(
                grassRevetmentSlidingInwardsIsRelevant,
                FailureMechanismType.GrassRevetmentSlidingInwards);
            FailureMechanismEntity technicalInnovations = CreateFailureMechanismEntity(
                technicalInnovationsIsRelevant,
                FailureMechanismType.TechnicalInnovations);

            entity.FailureMechanismEntities.Add(microstability);
            entity.FailureMechanismEntities.Add(strengthAndStabilityParallelConstruction);
            entity.FailureMechanismEntities.Add(waterOverpressureAsphaltRevetment);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingOutwards);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingInwards);
            entity.FailureMechanismEntities.Add(technicalInnovations);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            AssertFailureMechanismEqual(microstabilityIsRelevant,
                                        2,
                                        microstability.InputComments,
                                        microstability.OutputComments,
                                        microstability.NotRelevantComments,
                                        section.Microstability);
            AssertFailureMechanismEqual(strengthAndStabilityParallelConstructionIsRelevant,
                                        2,
                                        strengthAndStabilityParallelConstruction.InputComments,
                                        strengthAndStabilityParallelConstruction.OutputComments,
                                        strengthAndStabilityParallelConstruction.NotRelevantComments,
                                        section.StrengthStabilityLengthwiseConstruction);
            AssertFailureMechanismEqual(waterOverpressureAsphaltRevetmentIsRelevant,
                                        2,
                                        waterOverpressureAsphaltRevetment.InputComments,
                                        waterOverpressureAsphaltRevetment.OutputComments,
                                        waterOverpressureAsphaltRevetment.NotRelevantComments,
                                        section.WaterPressureAsphaltCover);
            AssertFailureMechanismEqual(grassRevetmentSlidingOutwardsIsRelevant,
                                        2,
                                        grassRevetmentSlidingOutwards.InputComments,
                                        grassRevetmentSlidingOutwards.OutputComments,
                                        grassRevetmentSlidingOutwards.NotRelevantComments,
                                        section.GrassCoverSlipOffOutwards);
            AssertFailureMechanismEqual(technicalInnovationsIsRelevant,
                                        2,
                                        technicalInnovations.InputComments,
                                        technicalInnovations.OutputComments,
                                        technicalInnovations.NotRelevantComments,
                                        section.TechnicalInnovation);
        }

        private static void AssertHydraulicBoundaryLocationCalculation(HydraulicLocationCalculationEntity expectedEntity,
                                                                       HydraulicBoundaryLocation expectedHydraulicBoundaryLocation,
                                                                       HydraulicBoundaryLocationCalculation actualCalculation)
        {
            Assert.AreSame(expectedHydraulicBoundaryLocation, actualCalculation.HydraulicBoundaryLocation);
            Assert.AreEqual(Convert.ToBoolean(expectedEntity.ShouldIllustrationPointsBeCalculated),
                            actualCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            Assert.IsNull(actualCalculation.Output);
        }

        private static void AssertGrassCoverErosionOutwardsHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                                 IEnumerable<HydraulicBoundaryLocationCalculation> actualCalculations)
        {
            Dictionary<HydraulicBoundaryLocation, HydraulicBoundaryLocationCalculation> dictionary =
                actualCalculations.ToDictionary(calc => calc.HydraulicBoundaryLocation, calc => calc);

            Assert.AreEqual(hydraulicBoundaryLocations.Count(), dictionary.Count);
            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in hydraulicBoundaryLocations)
            {
                HydraulicBoundaryLocationCalculation calculation = dictionary[hydraulicBoundaryLocation];
                Assert.IsNotNull(calculation);
                Assert.IsNull(calculation.Output);
                Assert.IsFalse(calculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            }
        }

        private static HydraulicLocationCalculationCollectionEntity CreateHydraulicLocationCollectionCalculationEntity(HydraulicLocationEntity hydraulicLocationEntity,
                                                                                                                       int seed)
        {
            var random = new Random(seed);
            return new HydraulicLocationCalculationCollectionEntity
            {
                HydraulicLocationCalculationEntities =
                {
                    new HydraulicLocationCalculationEntity
                    {
                        HydraulicLocationEntity = hydraulicLocationEntity,
                        ShouldIllustrationPointsBeCalculated = Convert.ToByte(random.NextBoolean())
                    }
                }
            };
        }

        private static AssessmentSectionEntity CreateAssessmentSectionEntity()
        {
            return new AssessmentSectionEntity
            {
                LowerLimitNorm = 1.0 / 30000,
                SignalingNorm = 1.0 / 300000,
                NormativeNormType = Convert.ToByte(NormType.Signaling),
                Composition = Convert.ToByte(AssessmentSectionComposition.Dike)
            };
        }

        private static BackgroundDataEntity CreateBackgroundDataEntity()
        {
            return new BackgroundDataEntity
            {
                Name = "Background",
                Transparency = 0.0,
                IsVisible = 0,
                BackgroundDataType = 1,
                BackgroundDataMetaEntities = new[]
                {
                    new BackgroundDataMetaEntity
                    {
                        Key = BackgroundDataIdentifiers.IsConfigured,
                        Value = "0"
                    }
                }
            };
        }

        private static FailureMechanismEntity CreateFailureMechanismEntity(bool isRelevant,
                                                                           FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismEntity
            {
                FailureMechanismType = (short) failureMechanismType,
                IsRelevant = Convert.ToByte(isRelevant),
                InputComments = string.Concat("InputComment", failureMechanismType.ToString()),
                OutputComments = string.Concat("OutputComment", failureMechanismType.ToString()),
                NotRelevantComments = string.Concat("NotRelevantComment", failureMechanismType.ToString()),
                FailureMechanismSectionEntities = CreateFailureMechanismSectionEntities()
            };
        }

        private static void AssertFailureMechanismEqual(bool expectedIsRelevant, int expectedSectionCount,
                                                        string expectedInputComments, string expectedOutputComments,
                                                        string expectedNotRelevantComments, IFailureMechanism failureMechanism)
        {
            Assert.AreEqual(expectedIsRelevant, failureMechanism.IsRelevant);
            Assert.AreEqual(expectedInputComments, failureMechanism.InputComments.Body);
            Assert.AreEqual(expectedOutputComments, failureMechanism.OutputComments.Body);
            Assert.AreEqual(expectedNotRelevantComments, failureMechanism.NotRelevantComments.Body);
            Assert.AreEqual(expectedSectionCount, failureMechanism.Sections.Count());
        }

        private static FailureMechanismSectionEntity[] CreateFailureMechanismSectionEntities()
        {
            var dummyPointData = new[]
            {
                new Point2D(0.0, 0.0)
            };
            string dummyPointXml = new Point2DCollectionXmlSerializer().ToXml(dummyPointData);
            return new[]
            {
                new FailureMechanismSectionEntity
                {
                    Name = "",
                    FailureMechanismSectionPointXml = dummyPointXml
                },
                new FailureMechanismSectionEntity
                {
                    Name = "",
                    FailureMechanismSectionPointXml = dummyPointXml
                }
            };
        }
    }
}