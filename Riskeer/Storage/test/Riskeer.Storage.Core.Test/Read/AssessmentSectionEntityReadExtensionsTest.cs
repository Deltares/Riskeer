﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read
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
            const double maximumAllowableFloodingProbability = 0.05;
            const double signalFloodingProbability = 0.02;

            var random = new Random(9);
            var normativeProbabilityType = random.NextEnumValue<NormativeProbabilityType>();
            bool areFailureMechanismsCorrelated = random.NextBoolean();

            var entity = new AssessmentSectionEntity
            {
                Id = testId,
                Name = testName,
                AreFailureMechanismsCorrelated = Convert.ToByte(areFailureMechanismsCorrelated),
                Composition = Convert.ToByte(assessmentSectionComposition),
                Comments = comments,
                MaximumAllowableFloodingProbability = maximumAllowableFloodingProbability,
                SignalFloodingProbability = signalFloodingProbability,
                NormativeProbabilityType = Convert.ToByte(normativeProbabilityType)
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
            Assert.AreEqual(areFailureMechanismsCorrelated, section.AreFailureMechanismsCorrelated);

            Assert.AreEqual(maximumAllowableFloodingProbability, section.FailureMechanismContribution.MaximumAllowableFloodingProbability);
            Assert.AreEqual(signalFloodingProbability, section.FailureMechanismContribution.SignalFloodingProbability);
            Assert.AreEqual(normativeProbabilityType, section.FailureMechanismContribution.NormativeProbabilityType);

            Assert.AreEqual(assessmentSectionComposition, section.Composition);
            CollectionAssert.IsEmpty(section.ReferenceLine.Points);

            HydraulicBoundaryData hydraulicBoundaryData = section.HydraulicBoundaryData;
            Assert.IsNotNull(hydraulicBoundaryData);
            CollectionAssert.IsEmpty(hydraulicBoundaryData.GetLocations());
            CollectionAssert.IsEmpty(hydraulicBoundaryData.HydraulicBoundaryDatabases);

            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            Assert.IsNull(hydraulicLocationConfigurationDatabase.FilePath);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.ScenarioName);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.Year);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.Scope);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.SeaLevel);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.RiverDischarge);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.LakeLevel);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.WindDirection);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.WindSpeed);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.Comment);
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

            var wellKnownTileSource = random.NextEnumValue<RiskeerWellKnownTileSource>();
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
        public void Read_WithReferenceLineEntities_ReturnsNewAssessmentSectionWithReferenceLineWithGeometry()
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
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            HydraulicBoundaryDataEntity hydraulicBoundaryDataEntity = CreateHydraulicBoundaryDataEntity();
            entity.HydraulicBoundaryDataEntities.Add(hydraulicBoundaryDataEntity);

            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntity.Name = "A";
            hydraulicLocationEntity.Order = 1;
            hydraulicBoundaryDataEntity.HydraulicBoundaryDatabaseEntities.Single().HydraulicLocationEntities.Add(hydraulicLocationEntity);

            entity.HydraulicLocationCalculationCollectionEntity = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 1);
            entity.HydraulicLocationCalculationCollectionEntity1 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 2);
            SetHydraulicLocationCalculationForTargetProbabilityCollectionEntities(entity, hydraulicLocationEntity);

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryLocation hydraulicBoundaryLocation = section.HydraulicBoundaryData
                                                                         .HydraulicBoundaryDatabases.Single()
                                                                         .Locations.Single();

            HydraulicBoundaryLocationCalculation calculation = section.WaterLevelCalculationsForSignalFloodingProbability.Single();
            HydraulicLocationCalculationEntity hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity1
                                                                                          .HydraulicLocationCalculationEntities
                                                                                          .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaterLevelCalculationsForMaximumAllowableFloodingProbability.Single();
            hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity
                                                       .HydraulicLocationCalculationEntities
                                                       .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            AssertHydraulicLocationCalculationsForTargetProbability(entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntities
                                                                          .Where(e => e.HydraulicBoundaryLocationCalculationType == (short) HydraulicBoundaryLocationCalculationType.WaterLevel)
                                                                          .OrderBy(e => e.Order),
                                                                    hydraulicBoundaryLocation,
                                                                    section.WaterLevelCalculationsForUserDefinedTargetProbabilities);
            AssertHydraulicLocationCalculationsForTargetProbability(entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntities
                                                                          .Where(e => e.HydraulicBoundaryLocationCalculationType == (short) HydraulicBoundaryLocationCalculationType.WaveHeight)
                                                                          .OrderBy(e => e.Order),
                                                                    hydraulicBoundaryLocation,
                                                                    section.WaveHeightCalculationsForUserDefinedTargetProbabilities);
        }

        [Test]
        public void Read_WithoutHydraulicBoundaryData_ReturnsNewAssessmentSectionWithoutHydraulicBoundaryLocationAndCalculations()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryData hydraulicBoundaryData = section.HydraulicBoundaryData;
            CollectionAssert.IsEmpty(hydraulicBoundaryData.GetLocations());
            CollectionAssert.IsEmpty(hydraulicBoundaryData.HydraulicBoundaryDatabases);

            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForSignalFloodingProbability);
            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForMaximumAllowableFloodingProbability);

            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForUserDefinedTargetProbabilities);
            CollectionAssert.IsEmpty(section.WaveHeightCalculationsForUserDefinedTargetProbabilities);
        }

        [Test]
        public void Read_WithHydraulicBoundaryDatabaseLocations_ReturnsNewAssessmentSectionWithLocationsSet()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            HydraulicBoundaryDataEntity hydraulicBoundaryDataEntity = CreateHydraulicBoundaryDataEntity();
            entity.HydraulicBoundaryDataEntities.Add(hydraulicBoundaryDataEntity);
            HydraulicBoundaryDatabaseEntity hydraulicBoundaryDatabaseEntity = hydraulicBoundaryDataEntity.HydraulicBoundaryDatabaseEntities.Single();

            HydraulicLocationEntity hydraulicLocationEntityOne = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntityOne.Name = "A";
            hydraulicLocationEntityOne.Order = 1;
            hydraulicBoundaryDatabaseEntity.HydraulicLocationEntities.Add(hydraulicLocationEntityOne);
            HydraulicLocationEntity hydraulicLocationEntityTwo = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntityOne.Name = "B";
            hydraulicLocationEntityOne.Order = 0;
            hydraulicBoundaryDatabaseEntity.HydraulicLocationEntities.Add(hydraulicLocationEntityTwo);

            entity.HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity();

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = section.HydraulicBoundaryData.HydraulicBoundaryDatabases
                                                                                       .SelectMany(hbd => hbd.Locations);
            Assert.AreEqual(2, hydraulicBoundaryLocations.Count());
            CollectionAssert.AreEqual(new[]
            {
                "B",
                "A"
            }, hydraulicBoundaryLocations.Select(l => l.Name));
        }

        [Test]
        public void Read_WithHydraulicBoundaryData_ReturnsNewAssessmentSectionWithHydraulicLocationConfigurationDatabaseSet()
        {
            // Setup
            var random = new Random(21);
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var hydraulicBoundaryDatabaseEntity = new HydraulicBoundaryDatabaseEntity
            {
                FilePath = "hrdFilePath",
                Version = "1.0"
            };
            var hydraulicBoundaryDataEntity = new HydraulicBoundaryDataEntity
            {
                HydraulicLocationConfigurationDatabaseFilePath = "hlcdFilePath",
                HydraulicLocationConfigurationDatabaseScenarioName = "ScenarioName",
                HydraulicLocationConfigurationDatabaseYear = random.Next(),
                HydraulicLocationConfigurationDatabaseScope = "Scope",
                HydraulicLocationConfigurationDatabaseSeaLevel = "SeaLevel",
                HydraulicLocationConfigurationDatabaseRiverDischarge = "RiverDischarge",
                HydraulicLocationConfigurationDatabaseLakeLevel = "LakeLevel",
                HydraulicLocationConfigurationDatabaseWindDirection = "WindDirection",
                HydraulicLocationConfigurationDatabaseWindSpeed = "WindSpeed",
                HydraulicLocationConfigurationDatabaseComment = "Comment",
                HydraulicBoundaryDatabaseEntities =
                {
                    hydraulicBoundaryDatabaseEntity
                }
            };

            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());
            entity.HydraulicBoundaryDataEntities.Add(hydraulicBoundaryDataEntity);
            entity.HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity();

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryData hydraulicBoundaryData = section.HydraulicBoundaryData;

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = hydraulicBoundaryData.HydraulicBoundaryDatabases.Single();
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.FilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.Version, hydraulicBoundaryDatabase.Version);

            HydraulicLocationConfigurationDatabase database = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseFilePath, database.FilePath);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseScenarioName, database.ScenarioName);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseYear, database.Year);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseScope, database.Scope);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseSeaLevel, database.SeaLevel);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseRiverDischarge, database.RiverDischarge);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseLakeLevel, database.LakeLevel);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseWindDirection, database.WindDirection);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseWindSpeed, database.WindSpeed);
            Assert.AreEqual(hydraulicBoundaryDataEntity.HydraulicLocationConfigurationDatabaseComment, database.Comment);
        }

        [Test]
        public void Read_WithPipingFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.Piping,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
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
            Assert.AreEqual(inAssembly, section.Piping.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.Piping.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.Piping.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.Piping.NotInAssemblyComments.Body);
            Assert.IsNull(section.Piping.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.Piping.StochasticSoilModels.SourcePath);
            Assert.IsNull(section.Piping.SurfaceLines.SourcePath);
        }

        [Test]
        public void Read_WithMacroStabilityInwardsFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInMacroStabilityInwardsFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                MacroStabilityInwardsFailureMechanismMetaEntities =
                {
                    new MacroStabilityInwardsFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.MacroStabilityInwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.MacroStabilityInwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.MacroStabilityInwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.MacroStabilityInwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.MacroStabilityInwards.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.MacroStabilityInwards.StochasticSoilModels.SourcePath);
            Assert.IsNull(section.MacroStabilityInwards.SurfaceLines.SourcePath);
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsWithProperties_ReturnsGrassCoverErosionInwardsWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string originalNotInAssemblyText = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = originalNotInAssemblyText,
                CalculationsInputComments = calculationsInputComments,
                GrassCoverErosionInwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionInwardsFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.GrassCoverErosionInwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.GrassCoverErosionInwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.GrassCoverErosionInwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(originalNotInAssemblyText, section.GrassCoverErosionInwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.GrassCoverErosionInwards.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.GrassCoverErosionInwards.DikeProfiles.SourcePath);
        }

        [Test]
        public void Read_WithGrassCoverErosionOutwardsWithProperties_ReturnsGrassCoverErosionOutwardsWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.GrassRevetmentErosionOutwards,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                GrassCoverErosionOutwardsFailureMechanismMetaEntities =
                {
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.GrassCoverErosionOutwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.GrassCoverErosionOutwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.GrassCoverErosionOutwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.GrassCoverErosionOutwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.GrassCoverErosionOutwards.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.GrassCoverErosionOutwards.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void Read_WithStabilityStoneCoverWithProperties_ReturnsStabilityStoneCoverWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityStoneRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                StabilityStoneCoverFailureMechanismMetaEntities =
                {
                    new StabilityStoneCoverFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.StabilityStoneCover.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.StabilityStoneCover.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.StabilityStoneCover.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.StabilityStoneCover.NotInAssemblyComments.Body);
            Assert.IsNull(section.StabilityStoneCover.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.StabilityStoneCover.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void Read_WithWaveImpactAsphaltCoverWithProperties_ReturnsWaveImpactAsphaltCoverWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.WaveImpactOnAsphaltRevetment,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                WaveImpactAsphaltCoverFailureMechanismMetaEntities =
                {
                    new WaveImpactAsphaltCoverFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.WaveImpactAsphaltCover.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.WaveImpactAsphaltCover.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.WaveImpactAsphaltCover.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.WaveImpactAsphaltCover.NotInAssemblyComments.Body);
            Assert.IsNull(section.WaveImpactAsphaltCover.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.WaveImpactAsphaltCover.ForeshoreProfiles.SourcePath);
        }

        [Test]
        public void Read_WithHeightStructuresWithProperties_ReturnsHeightStructuresWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StructureHeight,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                HeightStructuresFailureMechanismMetaEntities =
                {
                    new HeightStructuresFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.HeightStructures.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.HeightStructures.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.HeightStructures.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.HeightStructures.NotInAssemblyComments.Body);
            Assert.IsNull(section.HeightStructures.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.HeightStructures.HeightStructures.SourcePath);
        }

        [Test]
        public void Read_WithClosingStructuresWithProperties_ReturnsClosingStructuresWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.ReliabilityClosingOfStructure,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                ClosingStructuresFailureMechanismMetaEntities =
                {
                    new ClosingStructuresFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.ClosingStructures.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.ClosingStructures.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.ClosingStructures.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.ClosingStructures.NotInAssemblyComments.Body);
            Assert.IsNull(section.ClosingStructures.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void Read_WithStabilityPointStructuresWithProperties_ReturnsStabilityPointStructuresWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.StabilityPointStructures,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                StabilityPointStructuresFailureMechanismMetaEntities =
                {
                    new StabilityPointStructuresFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.StabilityPointStructures.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.StabilityPointStructures.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.StabilityPointStructures.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.StabilityPointStructures.NotInAssemblyComments.Body);
            Assert.IsNull(section.StabilityPointStructures.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.StabilityPointStructures.StabilityPointStructures.SourcePath);
        }

        [Test]
        public void Read_WithDuneErosionWithProperties_ReturnsDuneErosionWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var random = new Random(39);
            bool inAssembly = random.NextBoolean();

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.DuneErosion,
                CalculationGroupEntity = new CalculationGroupEntity(),
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
                DuneErosionFailureMechanismMetaEntities =
                {
                    new DuneErosionFailureMechanismMetaEntity()
                }
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.DuneErosion.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.DuneErosion.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.DuneErosion.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.DuneErosion.NotInAssemblyComments.Body);
            Assert.IsNull(section.DuneErosion.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void Read_WithPipingStructureFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingStructureFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.PipingAtStructure,
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments
            };
            entity.FailureMechanismEntities.Add(failureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            Assert.AreEqual(inAssembly, section.PipingStructure.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.PipingStructure.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.PipingStructure.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.PipingStructure.NotInAssemblyComments.Body);
            Assert.IsNull(section.PipingStructure.FailureMechanismSectionSourcePath);
        }

        [Test]
        public void Read_WithStandAloneFailureMechanisms_ReturnsNewAssessmentSectionWithFailureMechanismsSet()
        {
            // Setup
            var random = new Random(31);
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            bool microstabilityInAssembly = random.NextBoolean();
            bool waterPressureAsphaltCoverInAssembly = random.NextBoolean();
            bool grassCoverSlipOffOutwardsInAssembly = random.NextBoolean();
            bool grassCoverSlipOffInwardsInAssembly = random.NextBoolean();

            FailureMechanismEntity microstabilityEntity = CreateFailureMechanismEntity(
                microstabilityInAssembly,
                FailureMechanismType.Microstability);

            FailureMechanismEntity waterPressureAsphaltCoverEntity = CreateFailureMechanismEntity(
                waterPressureAsphaltCoverInAssembly,
                FailureMechanismType.WaterOverpressureAsphaltRevetment);

            FailureMechanismEntity grassRevetmentSlidingOutwardsEntity = CreateFailureMechanismEntity(
                grassCoverSlipOffOutwardsInAssembly,
                FailureMechanismType.GrassRevetmentSlidingOutwards);

            FailureMechanismEntity grassRevetmentSlidingInwardsEntity = CreateFailureMechanismEntity(
                grassCoverSlipOffInwardsInAssembly,
                FailureMechanismType.GrassRevetmentSlidingInwards);

            entity.FailureMechanismEntities.Add(microstabilityEntity);
            entity.FailureMechanismEntities.Add(waterPressureAsphaltCoverEntity);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingOutwardsEntity);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingInwardsEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            AssertFailureMechanismEqual(microstabilityInAssembly,
                                        microstabilityEntity,
                                        section.Microstability);

            AssertFailureMechanismEqual(waterPressureAsphaltCoverInAssembly,
                                        waterPressureAsphaltCoverEntity,
                                        section.WaterPressureAsphaltCover);

            AssertFailureMechanismEqual(grassCoverSlipOffOutwardsInAssembly,
                                        grassRevetmentSlidingOutwardsEntity,
                                        section.GrassCoverSlipOffOutwards);

            AssertFailureMechanismEqual(grassCoverSlipOffInwardsInAssembly,
                                        grassRevetmentSlidingInwardsEntity,
                                        section.GrassCoverSlipOffInwards);
        }

        [Test]
        public void Read_WithSpecificFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInSpecificFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);

            bool inAssembly1 = random.NextBoolean();
            const string name1 = "Specific failure mechanism name";
            const string inAssemblyInputComments1 = "Some input text";
            const string inAssemblyOutputComments1 = "Some output text";
            const string notInAssemblyComments1 = "Some not relevant text";

            bool inAssembly2 = random.NextBoolean();
            const string name2 = "Specific failure mechanism name2";
            const string inAssemblyInputComments2 = "Some input text2";
            const string inAssemblyOutputComments2 = "Some output text2";
            const string notInAssemblyComments2 = "Some not relevant text2";

            var firstSpecificFailureMechanismEntity = new SpecificFailureMechanismEntity
            {
                Name = name1,
                InAssembly = Convert.ToByte(inAssembly1),
                InAssemblyInputComments = inAssemblyInputComments1,
                InAssemblyOutputComments = inAssemblyOutputComments1,
                NotInAssemblyComments = notInAssemblyComments1
            };

            var secondSpecificFailureMechanismEntity = new SpecificFailureMechanismEntity
            {
                Name = name2,
                InAssembly = Convert.ToByte(inAssembly2),
                InAssemblyInputComments = inAssemblyInputComments2,
                InAssemblyOutputComments = inAssemblyOutputComments2,
                NotInAssemblyComments = notInAssemblyComments2
            };

            entity.SpecificFailureMechanismEntities.Add(firstSpecificFailureMechanismEntity);
            entity.SpecificFailureMechanismEntities.Add(secondSpecificFailureMechanismEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            SpecificFailureMechanism firstSpecificFailureMechanism = section.SpecificFailureMechanisms[0];
            Assert.IsNotNull(firstSpecificFailureMechanism);
            Assert.AreEqual(name1, firstSpecificFailureMechanism.Name);
            Assert.AreEqual(inAssembly1, firstSpecificFailureMechanism.InAssembly);
            Assert.AreEqual(inAssemblyInputComments1, firstSpecificFailureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments1, firstSpecificFailureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments1, firstSpecificFailureMechanism.NotInAssemblyComments.Body);
            Assert.IsNull(firstSpecificFailureMechanism.FailureMechanismSectionSourcePath);

            SpecificFailureMechanism secondSpecificFailureMechanism = section.SpecificFailureMechanisms[1];
            Assert.IsNotNull(secondSpecificFailureMechanism);
            Assert.AreEqual(name2, secondSpecificFailureMechanism.Name);
            Assert.AreEqual(inAssembly2, secondSpecificFailureMechanism.InAssembly);
            Assert.AreEqual(inAssemblyInputComments2, secondSpecificFailureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments2, secondSpecificFailureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments2, secondSpecificFailureMechanism.NotInAssemblyComments.Body);
            Assert.IsNull(secondSpecificFailureMechanism.FailureMechanismSectionSourcePath);
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

        private static void AssertHydraulicLocationCalculationsForTargetProbability(
            IEnumerable<HydraulicLocationCalculationForTargetProbabilityCollectionEntity> expectedCalculationCollectionEntities,
            HydraulicBoundaryLocation expectedHydraulicBoundaryLocation,
            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> actualCalculationCollections)
        {
            int expectedNrOfEntities = expectedCalculationCollectionEntities.Count();
            Assert.AreEqual(expectedNrOfEntities, actualCalculationCollections.Count());

            for (var j = 0; j < expectedNrOfEntities; j++)
            {
                AssertHydraulicLocationCalculationsForTargetProbability(expectedCalculationCollectionEntities.ElementAt(j),
                                                                        expectedHydraulicBoundaryLocation,
                                                                        actualCalculationCollections.ElementAt(j));
            }
        }

        private static void AssertHydraulicLocationCalculationsForTargetProbability(HydraulicLocationCalculationForTargetProbabilityCollectionEntity expectedCalculationCollectionEntity,
                                                                                    HydraulicBoundaryLocation expectedHydraulicBoundaryLocation,
                                                                                    HydraulicBoundaryLocationCalculationsForTargetProbability actualCalculations)
        {
            Assert.AreEqual(expectedCalculationCollectionEntity.TargetProbability, actualCalculations.TargetProbability);
            ICollection<HydraulicLocationCalculationEntity> expectedCalculations = expectedCalculationCollectionEntity.HydraulicLocationCalculationEntities;
            int expectedNrOfCalculations = expectedCalculations.Count;
            Assert.AreEqual(expectedNrOfCalculations, actualCalculations.HydraulicBoundaryLocationCalculations.Count);

            for (var i = 0; i < expectedNrOfCalculations; i++)
            {
                HydraulicBoundaryLocationCalculation actualCalculation = actualCalculations.HydraulicBoundaryLocationCalculations[i];
                AssertHydraulicBoundaryLocationCalculation(expectedCalculations.ElementAt(i),
                                                           expectedHydraulicBoundaryLocation,
                                                           actualCalculation);
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

        private static void SetHydraulicLocationCalculationForTargetProbabilityCollectionEntities(AssessmentSectionEntity entity,
                                                                                                  HydraulicLocationEntity hydraulicLocationEntity)
        {
            var random = new Random(21);
            int nrOfCollections = random.Next(1, 10);
            for (int i = nrOfCollections; i >= 0; i--)
            {
                entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntities.Add(CreateHydraulicLocationCalculationForTargetProbabilityCollectionEntity(hydraulicLocationEntity, i));
            }
        }

        private static HydraulicLocationCalculationForTargetProbabilityCollectionEntity CreateHydraulicLocationCalculationForTargetProbabilityCollectionEntity(
            HydraulicLocationEntity hydraulicLocationEntity,
            int seed)
        {
            var random = new Random(seed);
            return new HydraulicLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = random.NextDouble(0, 0.1),
                Order = seed,
                HydraulicBoundaryLocationCalculationType = Convert.ToByte(random.NextEnumValue<HydraulicBoundaryLocationCalculationType>()),
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
                MaximumAllowableFloodingProbability = 1.0 / 30000,
                SignalFloodingProbability = 1.0 / 300000,
                NormativeProbabilityType = Convert.ToByte(NormativeProbabilityType.SignalFloodingProbability),
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

        private static HydraulicBoundaryDataEntity CreateHydraulicBoundaryDataEntity()
        {
            return new HydraulicBoundaryDataEntity
            {
                HydraulicBoundaryDatabaseEntities =
                {
                    new HydraulicBoundaryDatabaseEntity
                    {
                        FilePath = "hrdFilePath",
                        Version = "1.0"
                    }
                },
                HydraulicLocationConfigurationDatabaseFilePath = "hlcdFilePath",
                HydraulicLocationConfigurationDatabaseScenarioName = "ScenarioName",
                HydraulicLocationConfigurationDatabaseYear = 1,
                HydraulicLocationConfigurationDatabaseScope = "Scope"
            };
        }

        private static FailureMechanismEntity CreateFailureMechanismEntity(bool inAssembly,
                                                                           FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismEntity
            {
                FailureMechanismType = (short) failureMechanismType,
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = string.Concat("InputComment", failureMechanismType.ToString()),
                InAssemblyOutputComments = string.Concat("OutputComment", failureMechanismType.ToString()),
                NotInAssemblyComments = string.Concat("NotInAssemblyText", failureMechanismType.ToString()),
                CalculationsInputComments = string.Concat("CalculationsCommentText", failureMechanismType.ToString())
            };
        }

        private static void AssertFailureMechanismEqual<T>(bool expectedInAssembly,
                                                           FailureMechanismEntity entity,
                                                           T failureMechanism)
            where T : IFailureMechanism
        {
            Assert.AreEqual(expectedInAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(entity.InAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(entity.InAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(entity.NotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
        }
    }
}