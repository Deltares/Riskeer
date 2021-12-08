﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Integration.Data.StandAlone.Input;
using Riskeer.MacroStabilityInwards.Data;
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
            CollectionAssert.IsEmpty(section.ReferenceLine.Points);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = section.HydraulicBoundaryDatabase;
            Assert.IsNotNull(hydraulicBoundaryDatabase);
            CollectionAssert.IsEmpty(hydraulicBoundaryDatabase.Locations);
            Assert.IsNull(hydraulicBoundaryDatabase.FilePath);
            Assert.IsNull(hydraulicBoundaryDatabase.Version);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.IsNull(settings.FilePath);
            Assert.IsNull(settings.ScenarioName);
            Assert.AreEqual(0, settings.Year);
            Assert.IsNull(settings.Scope);
            Assert.IsNull(settings.SeaLevel);
            Assert.IsNull(settings.RiverDischarge);
            Assert.IsNull(settings.LakeLevel);
            Assert.IsNull(settings.WindDirection);
            Assert.IsNull(settings.WindSpeed);
            Assert.IsNull(settings.Comment);
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
            entity.HydraulicBoundaryDatabaseEntities.Add(CreateHydraulicDatabaseEntity());
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            HydraulicLocationEntity hydraulicLocationEntity = HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity();
            hydraulicLocationEntity.Name = "A";
            hydraulicLocationEntity.Order = 1;
            entity.HydraulicLocationEntities.Add(hydraulicLocationEntity);

            entity.HydraulicLocationCalculationCollectionEntity = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 1);
            entity.HydraulicLocationCalculationCollectionEntity1 = CreateHydraulicLocationCollectionCalculationEntity(hydraulicLocationEntity, 2);
            SetHydraulicLocationCalculationForTargetProbabilityCollectionEntities(entity, hydraulicLocationEntity);

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryLocation hydraulicBoundaryLocation = section.HydraulicBoundaryDatabase.Locations.Single();

            HydraulicBoundaryLocationCalculation calculation = section.WaterLevelCalculationsForSignalingNorm.Single();
            HydraulicLocationCalculationEntity hydraulicLocationCalculationEntity = entity.HydraulicLocationCalculationCollectionEntity1
                                                                                          .HydraulicLocationCalculationEntities
                                                                                          .Single();
            AssertHydraulicBoundaryLocationCalculation(hydraulicLocationCalculationEntity,
                                                       hydraulicBoundaryLocation,
                                                       calculation);

            calculation = section.WaterLevelCalculationsForLowerLimitNorm.Single();
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
        public void Read_WithoutHydraulicBoundaryLocationDatabase_ReturnsNewAssessmentSectionWithoutHydraulicBoundaryLocationAndCalculations()
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

            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForSignalingNorm);
            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForLowerLimitNorm);

            CollectionAssert.IsEmpty(section.WaterLevelCalculationsForUserDefinedTargetProbabilities);
            CollectionAssert.IsEmpty(section.WaveHeightCalculationsForUserDefinedTargetProbabilities);
        }

        [Test]
        public void Read_WithHydraulicDatabaseLocations_ReturnsNewAssessmentSectionWithLocationsSet()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            entity.HydraulicBoundaryDatabaseEntities.Add(CreateHydraulicDatabaseEntity());
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
        }

        [Test]
        public void Read_WithHydraulicBoundaryLocationDatabase_ReturnsNewAssessmentSectionWithHydraulicLocationConfigurationSettingsSet()
        {
            // Setup
            var random = new Random(21);
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var hydraulicBoundaryDatabaseEntity = new HydraulicBoundaryDatabaseEntity
            {
                FilePath = "hydraulicBoundaryDatabaseFilePath",
                Version = "hydraulicBoundaryDatabaseVersion",
                HydraulicLocationConfigurationSettingsFilePath = "hlcdFilePath",
                HydraulicLocationConfigurationSettingsScenarioName = "ScenarioName",
                HydraulicLocationConfigurationSettingsYear = random.Next(),
                HydraulicLocationConfigurationSettingsScope = "Scope",
                HydraulicLocationConfigurationSettingsSeaLevel = "SeaLevel",
                HydraulicLocationConfigurationSettingsRiverDischarge = "RiverDischarge",
                HydraulicLocationConfigurationSettingsLakeLevel = "LakeLevel",
                HydraulicLocationConfigurationSettingsWindDirection = "WindDirection",
                HydraulicLocationConfigurationSettingsWindSpeed = "WindSpeed",
                HydraulicLocationConfigurationSettingsComment = "Comment"
            };

            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());
            entity.HydraulicBoundaryDatabaseEntities.Add(hydraulicBoundaryDatabaseEntity);
            entity.HydraulicLocationCalculationCollectionEntity = new HydraulicLocationCalculationCollectionEntity();
            entity.HydraulicLocationCalculationCollectionEntity1 = new HydraulicLocationCalculationCollectionEntity();

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = section.HydraulicBoundaryDatabase;
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.FilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.Version, hydraulicBoundaryDatabase.Version);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsFilePath, settings.FilePath);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsScenarioName, settings.ScenarioName);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsYear, settings.Year);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsScope, settings.Scope);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsSeaLevel, settings.SeaLevel);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsRiverDischarge, settings.RiverDischarge);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsLakeLevel, settings.LakeLevel);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsWindDirection, settings.WindDirection);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsWindSpeed, settings.WindSpeed);
            Assert.AreEqual(hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsComment, settings.Comment);
        }

        [Test]
        public void Read_WithPipingFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double parameterA = random.NextDouble() / 10;
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
            Assert.AreEqual(inAssembly, section.Piping.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.Piping.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.Piping.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.Piping.NotInAssemblyComments.Body);
            Assert.AreEqual(parameterA, section.Piping.PipingProbabilityAssessmentInput.A);
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
            double parameterA = random.NextDouble();
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
            Assert.AreEqual(inAssembly, section.MacroStabilityInwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.MacroStabilityInwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.MacroStabilityInwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.MacroStabilityInwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.MacroStabilityInwards.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.MacroStabilityInwards.StochasticSoilModels.SourcePath);
            Assert.IsNull(section.MacroStabilityInwards.SurfaceLines.SourcePath);

            MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput = section.MacroStabilityInwards
                                                                                                .MacroStabilityInwardsProbabilityAssessmentInput;
            Assert.AreEqual(parameterA, probabilityAssessmentInput.A);
        }

        [Test]
        public void Read_WithMacroStabilityOutwardsFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInMacroStabilityOutwardsFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double parameterA = random.NextDouble();
            const string inAssemblyInputComments = "Some input text";
            const string inAssemblyOutputComments = "Some output text";
            const string notInAssemblyComments = "Really not in assembly";
            const string calculationsInputComments = "Some calculations comments";

            var failureMechanismEntity = new FailureMechanismEntity
            {
                FailureMechanismType = (int) FailureMechanismType.MacroStabilityOutwards,
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = inAssemblyInputComments,
                InAssemblyOutputComments = inAssemblyOutputComments,
                NotInAssemblyComments = notInAssemblyComments,
                CalculationsInputComments = calculationsInputComments,
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
            Assert.AreEqual(inAssembly, section.MacroStabilityOutwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.MacroStabilityOutwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.MacroStabilityOutwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.MacroStabilityOutwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.MacroStabilityOutwards.FailureMechanismSectionSourcePath);

            MacroStabilityOutwardsProbabilityAssessmentInput probabilityAssessmentInput = section.MacroStabilityOutwards
                                                                                                 .MacroStabilityOutwardsProbabilityAssessmentInput;
            Assert.AreEqual(parameterA, probabilityAssessmentInput.A);
        }

        [Test]
        public void Read_WithGrassCoverErosionInwardsWithProperties_ReturnsGrassCoverErosionInwardsWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double n = random.NextDouble(1.0, 20.0);
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
            Assert.AreEqual(inAssembly, section.GrassCoverErosionInwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.GrassCoverErosionInwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.GrassCoverErosionInwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(originalNotInAssemblyText, section.GrassCoverErosionInwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.GrassCoverErosionInwards.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.GrassCoverErosionInwards.DikeProfiles.SourcePath);

            RoundedDouble actualN = section.GrassCoverErosionInwards.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithGrassCoverErosionOutwardsWithProperties_ReturnsGrassCoverErosionOutwardsWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double n = random.NextDouble(1.0, 20.0);
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
                    new GrassCoverErosionOutwardsFailureMechanismMetaEntity
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
            Assert.AreEqual(inAssembly, section.GrassCoverErosionOutwards.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.GrassCoverErosionOutwards.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.GrassCoverErosionOutwards.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.GrassCoverErosionOutwards.NotInAssemblyComments.Body);
            Assert.IsNull(section.GrassCoverErosionOutwards.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.GrassCoverErosionOutwards.ForeshoreProfiles.SourcePath);

            RoundedDouble actualN = section.GrassCoverErosionOutwards.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithStabilityStoneCoverWithProperties_ReturnsStabilityStoneCoverWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double n = random.NextDouble(1.0, 20.0);
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
            Assert.AreEqual(inAssembly, section.StabilityStoneCover.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.StabilityStoneCover.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.StabilityStoneCover.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.StabilityStoneCover.NotInAssemblyComments.Body);
            Assert.IsNull(section.StabilityStoneCover.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.StabilityStoneCover.ForeshoreProfiles.SourcePath);

            RoundedDouble actualN = section.StabilityStoneCover.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithWaveImpactAsphaltCoverWithProperties_ReturnsWaveImpactAsphaltCoverWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double deltaL = random.NextDouble(1.0, 20.0);
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
            Assert.AreEqual(inAssembly, section.WaveImpactAsphaltCover.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.WaveImpactAsphaltCover.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.WaveImpactAsphaltCover.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.WaveImpactAsphaltCover.NotInAssemblyComments.Body);
            Assert.IsNull(section.WaveImpactAsphaltCover.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.WaveImpactAsphaltCover.ForeshoreProfiles.SourcePath);

            RoundedDouble actualDeltaL = section.WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.DeltaL;
            Assert.AreEqual(deltaL, actualDeltaL, actualDeltaL.GetAccuracy());
        }

        [Test]
        public void Read_WithHeightStructuresWithProperties_ReturnsHeightStructuresWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double n = random.NextDouble(1.0, 20.0);
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
            Assert.AreEqual(inAssembly, section.HeightStructures.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.HeightStructures.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.HeightStructures.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.HeightStructures.NotInAssemblyComments.Body);
            Assert.IsNull(section.HeightStructures.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.HeightStructures.HeightStructures.SourcePath);

            RoundedDouble actualN = section.HeightStructures.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithClosingStructuresWithProperties_ReturnsClosingStructuresWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            int n2a = random.Next(1, 40);
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
            Assert.AreEqual(inAssembly, section.ClosingStructures.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.ClosingStructures.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.ClosingStructures.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.ClosingStructures.NotInAssemblyComments.Body);
            Assert.IsNull(section.ClosingStructures.FailureMechanismSectionSourcePath);
            Assert.AreEqual(n2a, section.ClosingStructures.GeneralInput.N2A);
        }

        [Test]
        public void Read_WithStabilityPointStructuresWithProperties_ReturnsStabilityPointStructuresWithProperties()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double n = random.NextDouble(1.0, 20.0);
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
            Assert.AreEqual(inAssembly, section.StabilityPointStructures.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.StabilityPointStructures.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.StabilityPointStructures.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.StabilityPointStructures.NotInAssemblyComments.Body);
            Assert.IsNull(section.StabilityPointStructures.FailureMechanismSectionSourcePath);
            Assert.IsNull(section.StabilityPointStructures.StabilityPointStructures.SourcePath);

            RoundedDouble actualN = section.StabilityPointStructures.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
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
            double n = random.NextDouble(1.0, 20.0);

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
                    new DuneErosionFailureMechanismMetaEntity
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
            Assert.AreEqual(inAssembly, section.DuneErosion.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.DuneErosion.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.DuneErosion.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.DuneErosion.NotInAssemblyComments.Body);
            Assert.IsNull(section.DuneErosion.FailureMechanismSectionSourcePath);

            RoundedDouble actualN = section.DuneErosion.GeneralInput.N;
            Assert.AreEqual(n, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithPipingStructureFailureMechanismProperties_ReturnsNewAssessmentSectionWithPropertiesInPipingStructureFailureMechanism()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);
            bool inAssembly = random.NextBoolean();
            double parameterN = random.NextDouble(1.0, 20.0);
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
                CalculationsInputComments = calculationsInputComments,
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
            Assert.AreEqual(inAssembly, section.PipingStructure.InAssembly);
            Assert.AreEqual(inAssemblyInputComments, section.PipingStructure.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments, section.PipingStructure.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments, section.PipingStructure.NotInAssemblyComments.Body);
            Assert.IsNull(section.PipingStructure.FailureMechanismSectionSourcePath);

            RoundedDouble actualN = section.PipingStructure.N;
            Assert.AreEqual(parameterN, actualN, actualN.GetAccuracy());
        }

        [Test]
        public void Read_WithStandAloneFailureMechanisms_ReturnsNewAssessmentSectionWithFailureMechanismsSet()
        {
            // Setup
            var random = new Random(31);
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();

            bool microstabilityInAssembly = random.NextBoolean();
            bool strengthAndStabilityParallelConstructionInAssembly = random.NextBoolean();
            bool waterOverpressureAsphaltRevetmentInAssembly = random.NextBoolean();
            bool grassRevetmentSlidingOutwardsInAssembly = random.NextBoolean();
            bool grassRevetmentSlidingInwardsInAssembly = random.NextBoolean();
            bool technicalInnovationsInAssembly = random.NextBoolean();

            FailureMechanismEntity microstability = CreateMicrostabilityFailureMechanismEntity(
                microstabilityInAssembly,
                FailureMechanismType.Microstability);
            FailureMechanismEntity strengthAndStabilityParallelConstruction = CreateFailureMechanismEntity(
                strengthAndStabilityParallelConstructionInAssembly,
                FailureMechanismType.StrengthAndStabilityParallelConstruction);
            FailureMechanismEntity waterOverpressureAsphaltRevetment = CreateFailureMechanismEntity(
                waterOverpressureAsphaltRevetmentInAssembly,
                FailureMechanismType.WaterOverpressureAsphaltRevetment);
            FailureMechanismEntity grassRevetmentSlidingOutwards = CreateFailureMechanismEntity(
                grassRevetmentSlidingOutwardsInAssembly,
                FailureMechanismType.GrassRevetmentSlidingOutwards);
            FailureMechanismEntity grassRevetmentSlidingInwards = CreateFailureMechanismEntity(
                grassRevetmentSlidingInwardsInAssembly,
                FailureMechanismType.GrassRevetmentSlidingInwards);
            FailureMechanismEntity technicalInnovation = CreateFailureMechanismEntity(
                technicalInnovationsInAssembly,
                FailureMechanismType.TechnicalInnovations);

            entity.FailureMechanismEntities.Add(microstability);
            entity.FailureMechanismEntities.Add(strengthAndStabilityParallelConstruction);
            entity.FailureMechanismEntities.Add(waterOverpressureAsphaltRevetment);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingOutwards);
            entity.FailureMechanismEntities.Add(grassRevetmentSlidingInwards);
            entity.FailureMechanismEntities.Add(technicalInnovation);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            AssertFailureMechanismEqual(microstabilityInAssembly,
                                        microstability.InAssemblyInputComments,
                                        microstability.InAssemblyOutputComments,
                                        microstability.NotInAssemblyComments,
                                        microstability.CalculationsInputComments,
                                        section.Microstability);
            AssertFailureMechanismEqual(strengthAndStabilityParallelConstructionInAssembly,
                                        strengthAndStabilityParallelConstruction.InAssemblyInputComments,
                                        strengthAndStabilityParallelConstruction.InAssemblyOutputComments,
                                        strengthAndStabilityParallelConstruction.NotInAssemblyComments,
                                        strengthAndStabilityParallelConstruction.CalculationsInputComments,
                                        section.StrengthStabilityLengthwiseConstruction);
            AssertFailureMechanismEqual(waterOverpressureAsphaltRevetmentInAssembly,
                                        waterOverpressureAsphaltRevetment.InAssemblyInputComments,
                                        waterOverpressureAsphaltRevetment.InAssemblyOutputComments,
                                        waterOverpressureAsphaltRevetment.NotInAssemblyComments,
                                        waterOverpressureAsphaltRevetment.CalculationsInputComments,
                                        section.WaterPressureAsphaltCover);
            AssertFailureMechanismEqual(grassRevetmentSlidingOutwardsInAssembly,
                                        grassRevetmentSlidingOutwards.InAssemblyInputComments,
                                        grassRevetmentSlidingOutwards.InAssemblyOutputComments,
                                        grassRevetmentSlidingOutwards.NotInAssemblyComments,
                                        grassRevetmentSlidingOutwards.CalculationsInputComments,
                                        section.GrassCoverSlipOffOutwards);
            AssertFailureMechanismEqual(grassRevetmentSlidingInwardsInAssembly,
                                        grassRevetmentSlidingInwards.InAssemblyInputComments,
                                        grassRevetmentSlidingInwards.InAssemblyOutputComments,
                                        grassRevetmentSlidingInwards.NotInAssemblyComments,
                                        grassRevetmentSlidingInwards.CalculationsInputComments,
                                        section.GrassCoverSlipOffInwards);
            AssertFailureMechanismEqual(technicalInnovationsInAssembly,
                                        technicalInnovation.InAssemblyInputComments,
                                        technicalInnovation.InAssemblyOutputComments,
                                        technicalInnovation.NotInAssemblyComments,
                                        technicalInnovation.CalculationsInputComments,
                                        section.TechnicalInnovation);
        }

        [Test]
        public void Read_WithSpecificFailurePathProperties_ReturnsNewAssessmentSectionWithPropertiesInSpecificFailurePath()
        {
            // Setup
            AssessmentSectionEntity entity = CreateAssessmentSectionEntity();
            var random = new Random(21);

            bool inAssembly1 = random.NextBoolean();
            const string name1 = "Specific failure path name";
            const string inAssemblyInputComments1 = "Some input text";
            const string inAssemblyOutputComments1 = "Some output text";
            const string notInAssemblyComments1 = "Some not relevant text";

            bool inAssembly2 = random.NextBoolean();
            const string name2 = "Specific failure path name2";
            const string inAssemblyInputComments2 = "Some input text2";
            const string inAssemblyOutputComments2 = "Some output text2";
            const string notInAssemblyComments2 = "Some not relevant text2";

            RoundedDouble n1 = random.NextRoundedDouble(1, 20);
            RoundedDouble n2 = random.NextRoundedDouble(1, 20);
            var firstSpecificFailurePathEntity = new SpecificFailurePathEntity
            {
                Name = name1,
                InAssembly = Convert.ToByte(inAssembly1),
                N = n1,
                InAssemblyInputComments = inAssemblyInputComments1,
                InAssemblyOutputComments = inAssemblyOutputComments1,
                NotInAssemblyComments = notInAssemblyComments1
            };

            var secondSpecificFailurePathEntity = new SpecificFailurePathEntity
            {
                Name = name2,
                InAssembly = Convert.ToByte(inAssembly2),
                N = n2,
                InAssemblyInputComments = inAssemblyInputComments2,
                InAssemblyOutputComments = inAssemblyOutputComments2,
                NotInAssemblyComments = notInAssemblyComments2
            };

            entity.SpecificFailurePathEntities.Add(firstSpecificFailurePathEntity);
            entity.SpecificFailurePathEntities.Add(secondSpecificFailurePathEntity);
            entity.BackgroundDataEntities.Add(CreateBackgroundDataEntity());

            var collector = new ReadConversionCollector();

            // Call
            AssessmentSection section = entity.Read(collector);

            // Assert
            var specificFailurePath1 = section.SpecificFailurePaths[0] as SpecificFailurePath;
            Assert.IsNotNull(specificFailurePath1);
            Assert.AreEqual(name1, specificFailurePath1.Name);
            Assert.AreEqual(inAssembly1, specificFailurePath1.InAssembly);
            Assert.AreEqual(inAssemblyInputComments1, specificFailurePath1.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments1, specificFailurePath1.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments1, specificFailurePath1.NotInAssemblyComments.Body);
            Assert.AreEqual(n1, specificFailurePath1.Input.N, specificFailurePath1.Input.N.GetAccuracy());
            Assert.IsNull(specificFailurePath1.FailureMechanismSectionSourcePath);

            var specificFailurePath2 = section.SpecificFailurePaths[1] as SpecificFailurePath;
            Assert.IsNotNull(specificFailurePath2);
            Assert.AreEqual(name2, specificFailurePath2.Name);
            Assert.AreEqual(inAssembly2, specificFailurePath2.InAssembly);
            Assert.AreEqual(inAssemblyInputComments2, specificFailurePath2.InAssemblyInputComments.Body);
            Assert.AreEqual(inAssemblyOutputComments2, specificFailurePath2.InAssemblyOutputComments.Body);
            Assert.AreEqual(notInAssemblyComments2, specificFailurePath2.NotInAssemblyComments.Body);
            Assert.AreEqual(n2, specificFailurePath2.Input.N, specificFailurePath2.Input.N.GetAccuracy());
            Assert.IsNull(specificFailurePath2.FailureMechanismSectionSourcePath);
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

        private static HydraulicBoundaryDatabaseEntity CreateHydraulicDatabaseEntity()
        {
            return new HydraulicBoundaryDatabaseEntity
            {
                FilePath = "hydraulicBoundaryDatabaseFilePath",
                Version = "hydraulicBoundaryDatabaseVersion",
                HydraulicLocationConfigurationSettingsFilePath = "hlcdFilePath",
                HydraulicLocationConfigurationSettingsScenarioName = "ScenarioName",
                HydraulicLocationConfigurationSettingsYear = 1,
                HydraulicLocationConfigurationSettingsScope = "Scope"
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
        
        private static FailureMechanismEntity CreateMicrostabilityFailureMechanismEntity(bool inAssembly,
                                                                           FailureMechanismType failureMechanismType)
        {
            return new FailureMechanismEntity
            {
                FailureMechanismType = (short) failureMechanismType,
                InAssembly = Convert.ToByte(inAssembly),
                InAssemblyInputComments = string.Concat("InputComment", failureMechanismType.ToString()),
                InAssemblyOutputComments = string.Concat("OutputComment", failureMechanismType.ToString()),
                NotInAssemblyComments = string.Concat("NotInAssemblyText", failureMechanismType.ToString()),
                CalculationsInputComments = string.Concat("CalculationsCommentText", failureMechanismType.ToString()),
                MicrostabilityFailureMechanismMetaEntities = new List<MicrostabilityFailureMechanismMetaEntity>
                {
                    new MicrostabilityFailureMechanismMetaEntity
                    {
                        N = 1.0
                    }
                }
            };
        }

        private static void AssertFailureMechanismEqual(bool expectedInAssembly,
                                                        string expectedInAssemblyInputComments, string expectedInAssemblyOutputComments,
                                                        string expectedNotInAssemblyComments, string expectedCalculationsInputComments,
                                                        IFailureMechanism failureMechanism)
        {
            Assert.AreEqual(expectedInAssembly, failureMechanism.InAssembly);
            Assert.AreEqual(expectedInAssemblyInputComments, failureMechanism.InAssemblyInputComments.Body);
            Assert.AreEqual(expectedInAssemblyOutputComments, failureMechanism.InAssemblyOutputComments.Body);
            Assert.AreEqual(expectedNotInAssemblyComments, failureMechanism.NotInAssemblyComments.Body);
            Assert.AreEqual(expectedCalculationsInputComments, failureMechanism.CalculationsInputComments.Body);
            Assert.IsNull(failureMechanism.FailureMechanismSectionSourcePath);
        }
    }
}