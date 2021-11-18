// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class AssessmentSectionCreateExtensionsTest
    {
        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void Create_WithoutCollector_ThrowsArgumentNullException(AssessmentSectionComposition assessmentSectionComposition)
        {
            // Setup
            var assessmentSection = new AssessmentSection(assessmentSectionComposition);

            // Call
            TestDelegate test = () => assessmentSection.Create(null);

            // Assert
            string parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("registry", parameterName);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void Create_WithCollector_ReturnsAssessmentSectionEntityWithCompositionAndFailureMechanisms(AssessmentSectionComposition assessmentSectionComposition)
        {
            // Setup
            const string testId = "testId";
            const string testName = "testName";
            const string comments = "Some text";
            const double lowerLimitNorm = 0.05;
            const double signalingNorm = 0.02;

            var random = new Random(65);
            const string mapDataName = "map data name";
            const double transparency = 0.3;
            const bool isVisible = true;
            const BackgroundDataType backgroundType = BackgroundDataType.Wmts;
            var normativeNorm = random.NextEnumValue<NormType>();
            IEnumerable<SpecificFailurePath> specificFailurePaths = Enumerable.Repeat(new SpecificFailurePath(), random.Next(1, 10))
                                                                              .ToArray();

            var assessmentSection = new AssessmentSection(assessmentSectionComposition)
            {
                Id = testId,
                Name = testName,
                Comments =
                {
                    Body = comments
                },
                FailureMechanismContribution =
                {
                    LowerLimitNorm = lowerLimitNorm,
                    SignalingNorm = signalingNorm,
                    NormativeNorm = normativeNorm
                },
                BackgroundData =
                {
                    Name = mapDataName,
                    Transparency = (RoundedDouble) transparency,
                    IsVisible = isVisible,
                    Configuration = new WmtsBackgroundDataConfiguration(false, null, null, null)
                }
            };

            assessmentSection.SpecificFailurePaths.AddRange(specificFailurePaths);
            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToByte(assessmentSectionComposition), entity.Composition);
            Assert.AreEqual(testId, entity.Id);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(comments, entity.Comments);
            Assert.AreEqual(lowerLimitNorm, entity.LowerLimitNorm);
            Assert.AreEqual(signalingNorm, entity.SignalingNorm);
            Assert.AreEqual(Convert.ToByte(normativeNorm), entity.NormativeNormType);
            Assert.AreEqual(18, entity.FailureMechanismEntities.Count);
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.Piping));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.GrassRevetmentTopErosionAndInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.MacroStabilityInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.MacroStabilityOutwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.Microstability));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.StabilityStoneRevetment));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.WaveImpactOnAsphaltRevetment));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.WaterOverpressureAsphaltRevetment));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.GrassRevetmentErosionOutwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.GrassRevetmentSlidingOutwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.GrassRevetmentSlidingInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.StructureHeight));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.ReliabilityClosingOfStructure));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.PipingAtStructure));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.StabilityPointStructures));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.StrengthAndStabilityParallelConstruction));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.DuneErosion));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.TechnicalInnovations));
            Assert.AreEqual(assessmentSection.SpecificFailurePaths.Count, entity.SpecificFailurePathEntities.Count);

            Assert.IsNull(entity.ReferenceLinePointXml);

            Assert.AreEqual(1, entity.BackgroundDataEntities.Count);
            BackgroundDataEntity backgroundDataEntity = entity.BackgroundDataEntities.Single();
            Assert.IsNotNull(backgroundDataEntity);
            Assert.AreEqual(mapDataName, backgroundDataEntity.Name);
            Assert.AreEqual(transparency, backgroundDataEntity.Transparency);
            Assert.AreEqual(Convert.ToByte(isVisible), backgroundDataEntity.IsVisible);
            Assert.AreEqual(Convert.ToByte(backgroundType), backgroundDataEntity.BackgroundDataType);

            Assert.AreEqual(1, backgroundDataEntity.BackgroundDataMetaEntities.Count);
            BackgroundDataMetaEntity isConfiguredMetaEntity = backgroundDataEntity.BackgroundDataMetaEntities.Single();
            Assert.AreEqual("IsConfigured", isConfiguredMetaEntity.Key);
            Assert.AreEqual("0", isConfiguredMetaEntity.Value);
        }

        [Test]
        public void Create_StringPropertiesDoNotShareReference()
        {
            // Setup
            const string originalName = "name";
            const string originalComments = "comments";

            var section = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = originalName,
                Comments =
                {
                    Body = originalComments
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = section.Create(registry);

            // Assert
            Assert.AreNotSame(originalName, entity.Name,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");
            Assert.AreNotSame(originalComments, entity.Comments,
                              "To create stable binary representations/fingerprints, it's really important that strings are not shared.");

            Assert.AreEqual(originalName, entity.Name);
            Assert.AreEqual(originalComments, entity.Comments);
        }

        [Test]
        public void Create_HydraulicBoundaryDatabaseNotLinked_SetsExpectedPropertiesToEntity()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.SetHydraulicBoundaryLocationCalculations(assessmentSection.HydraulicBoundaryDatabase.Locations);

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry);

            // Assert
            CollectionAssert.IsEmpty(entity.HydraulicBoundaryDatabaseEntities);
            CollectionAssert.IsEmpty(entity.HydraulicLocationEntities);

            AssertHydraulicLocationCalculationCollectionEntities(assessmentSection, entity);
        }

        [Test]
        public void Create_HydraulicBoundaryDatabaseLinked_SetsExpectedPropertiesToEntity()
        {
            // Setup
            var random = new Random(21);
            const string testFilePath = "path";
            const string testVersion = "1";

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = testFilePath,
                    Version = testVersion,
                    Locations =
                    {
                        new HydraulicBoundaryLocation(-1, "name", 1, 2)
                    }
                },
                WaveHeightCalculationsForUserDefinedTargetProbabilities =
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
                },
                WaterLevelCalculationsForUserDefinedTargetProbabilities =
                {
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1)),
                    new HydraulicBoundaryLocationCalculationsForTargetProbability(random.NextDouble(0, 0.1))
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(assessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationCalculationInputsOfAssessmentSection(assessmentSection);

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry);

            // Assert
            HydraulicBoundaryDatabaseEntity hydraulicBoundaryDatabaseEntity = entity.HydraulicBoundaryDatabaseEntities.Single();

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
            Assert.AreEqual(hydraulicBoundaryDatabase.FilePath, hydraulicBoundaryDatabaseEntity.FilePath);
            Assert.AreEqual(hydraulicBoundaryDatabase.Version, hydraulicBoundaryDatabaseEntity.Version);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(settings.FilePath, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsFilePath);
            Assert.AreEqual(settings.ScenarioName, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsScenarioName);
            Assert.AreEqual(settings.Year, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsYear);
            Assert.AreEqual(settings.Scope, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsScope);
            Assert.AreEqual(settings.SeaLevel, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsSeaLevel);
            Assert.AreEqual(settings.RiverDischarge, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsRiverDischarge);
            Assert.AreEqual(settings.LakeLevel, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsLakeLevel);
            Assert.AreEqual(settings.WindDirection, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsWindDirection);
            Assert.AreEqual(settings.WindSpeed, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsWindSpeed);
            Assert.AreEqual(settings.Comment, hydraulicBoundaryDatabaseEntity.HydraulicLocationConfigurationSettingsComment);

            int expectedNrOfHydraulicBoundaryLocations = hydraulicBoundaryDatabase.Locations.Count;
            Assert.AreEqual(expectedNrOfHydraulicBoundaryLocations, entity.HydraulicLocationEntities.Count);

            AssertHydraulicLocationCalculationCollectionEntities(assessmentSection, entity);
        }

        [Test]
        public void Create_WithReferenceLine_AddsReferenceLinePointEntities()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1, 0),
                new Point2D(2, 3),
                new Point2D(5, 3)
            };

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.ReferenceLine.SetGeometry(points);

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry);

            // Assert
            string expectedXml = new Point2DCollectionXmlSerializer().ToXml(points);
            Assert.AreEqual(expectedXml, entity.ReferenceLinePointXml);
        }

        private static void AssertHydraulicLocationCalculationCollectionEntities(AssessmentSection assessmentSection, AssessmentSectionEntity entity)
        {
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity1.HydraulicLocationCalculationEntities);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity.HydraulicLocationCalculationEntities);

            AssertHydraulicLocationCalculationForTargetProbabilityCollectionEntity(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                   entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntities
                                                                                         .Where(e => e.HydraulicBoundaryLocationCalculationType == (short) HydraulicBoundaryLocationCalculationType.WaveHeight));

            AssertHydraulicLocationCalculationForTargetProbabilityCollectionEntity(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                                                                   entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntities
                                                                                         .Where(e => e.HydraulicBoundaryLocationCalculationType == (short) HydraulicBoundaryLocationCalculationType.WaterLevel));
        }

        private static void SetHydraulicBoundaryLocationCalculationInputsOfAssessmentSection(AssessmentSection assessmentSection)
        {
            var random = new Random(21);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForSignalingNorm, random.Next());
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, random.Next());

            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                           .SelectMany(calc => calc.HydraulicBoundaryLocationCalculations), random.Next());

            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                           .SelectMany(calc => calc.HydraulicBoundaryLocationCalculations), random.Next());
        }

        private static void SetHydraulicBoundaryLocationCalculationInputs(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                          int seed)
        {
            var random = new Random(seed);
            foreach (HydraulicBoundaryLocationCalculation calculation in calculations)
            {
                calculation.InputParameters.ShouldIllustrationPointsBeCalculated = random.NextBoolean();
            }
        }

        private static void AssertHydraulicLocationCalculationForTargetProbabilityCollectionEntity(
            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> expectedCalculationCollections,
            IEnumerable<HydraulicLocationCalculationForTargetProbabilityCollectionEntity> actualCalculationCollectionEntities)
        {
            Assert.AreEqual(expectedCalculationCollections.Count(), actualCalculationCollectionEntities.Count());

            var i = 0;
            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability expectedCalculationCollection in expectedCalculationCollections)
            {
                HydraulicLocationCalculationForTargetProbabilityCollectionEntity actualCalculationCollectionEntity = actualCalculationCollectionEntities.ElementAt(i);
                AssertHydraulicLocationCalculationForTargetProbabilityCollectionEntity(expectedCalculationCollection, actualCalculationCollectionEntity);
                i++;
            }
        }

        private static void AssertHydraulicLocationCalculationForTargetProbabilityCollectionEntity(
            HydraulicBoundaryLocationCalculationsForTargetProbability expectedCalculations,
            HydraulicLocationCalculationForTargetProbabilityCollectionEntity entity)
        {
            Assert.AreEqual(expectedCalculations.TargetProbability, entity.TargetProbability);
            AssertHydraulicLocationCalculationCollectionEntity(expectedCalculations.HydraulicBoundaryLocationCalculations,
                                                               entity.HydraulicLocationCalculationEntities);
        }

        private static void AssertHydraulicLocationCalculationCollectionEntity(IEnumerable<HydraulicBoundaryLocationCalculation> expectedCalculations,
                                                                               IEnumerable<HydraulicLocationCalculationEntity> hydraulicLocationCalculationEntities)
        {
            Assert.AreEqual(expectedCalculations.Count(), hydraulicLocationCalculationEntities.Count());

            var i = 0;
            foreach (HydraulicLocationCalculationEntity actualCalculationEntity in hydraulicLocationCalculationEntities)
            {
                HydraulicBoundaryLocationCalculation expectedCalculation = expectedCalculations.ElementAt(i);
                Assert.AreEqual(Convert.ToByte(expectedCalculation.InputParameters.ShouldIllustrationPointsBeCalculated),
                                actualCalculationEntity.ShouldIllustrationPointsBeCalculated);
                i++;
            }
        }
    }
}