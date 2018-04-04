﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Create
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
            TestDelegate test = () => assessmentSection.Create(null, 0);

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
            int order = random.Next();

            const string mapDataName = "map data name";
            const double transparency = 0.3;
            const bool isVisible = true;
            const BackgroundDataType backgroundType = BackgroundDataType.Wmts;
            var normativeNorm = random.NextEnumValue<NormType>();
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
            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry, order);

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
            Assert.AreEqual(order, entity.Order);

            Assert.IsNull(entity.ReferenceLinePointXml);

            Assert.AreEqual(1, entity.BackgroundDataEntities.Count);
            BackgroundDataEntity backgroundDataEntity = entity.BackgroundDataEntities.Single();
            Assert.IsNotNull(backgroundDataEntity);
            Assert.AreEqual(mapDataName, backgroundDataEntity.Name);
            Assert.AreEqual(transparency, backgroundDataEntity.Transparency);
            Assert.AreEqual(Convert.ToByte(isVisible), backgroundDataEntity.IsVisible);
            Assert.AreEqual(Convert.ToByte(backgroundType), backgroundDataEntity.BackgroundDataType);

            Assert.AreEqual(1, backgroundDataEntity.BackgroundDataMetaEntities.Count);
            BackgroundDataMetaEntity isConfiguredMetaEntity = backgroundDataEntity.BackgroundDataMetaEntities.First();
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
            AssessmentSectionEntity entity = section.Create(registry, 0);

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
            AssessmentSectionEntity entity = assessmentSection.Create(registry, 0);

            // Assert
            Assert.IsNull(entity.HydraulicDatabaseLocation);
            Assert.IsNull(entity.HydraulicDatabaseVersion);
            CollectionAssert.IsEmpty(entity.HydraulicLocationEntities);
            CollectionAssert.IsEmpty(entity.HydraRingPreprocessorEntities);

            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity1);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForSignalingNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity2);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity3);

            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity4);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity5);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity6);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, 
                                                               entity.HydraulicLocationCalculationCollectionEntity7);
        }

        [Test]
        public void Create_HydraulicBoundaryDatabaseLinked_SetsExpectedPropertiesToEntity()
        {
            // Setup
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
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(assessmentSection.HydraulicBoundaryDatabase.Locations);
            SetHydraulicBoundaryLocationCalculationInputsOfAssessmentSection(assessmentSection);

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry, 0);

            // Assert
            TestHelper.AssertAreEqualButNotSame(testFilePath, entity.HydraulicDatabaseLocation);
            TestHelper.AssertAreEqualButNotSame(testVersion, entity.HydraulicDatabaseVersion);

            int expectedNrOfHydraulicBoundaryLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.Count;
            Assert.AreEqual(expectedNrOfHydraulicBoundaryLocations, entity.HydraulicLocationEntities.Count);

            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity1);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForSignalingNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity2);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity3);

            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity4);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity5);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForSignalingNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity6);
            AssertHydraulicLocationCalculationCollectionEntity(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm,
                                                               entity.HydraulicLocationCalculationCollectionEntity7);
        }

        [Test]
        public void Create_HydraulicBoundaryDatabaseLinkedWithCanUsePreprocessorFalse_SetsExpectedPropertiesToEntity()
        {
            // Setup
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
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry, 0);

            // Assert
            CollectionAssert.IsEmpty(entity.HydraRingPreprocessorEntities);
        }

        [Test]
        public void Create_HydraulicBoundaryDatabaseLinkedWithCanUsePreprocessorTrue_SetsExpectedPropertiesToEntity()
        {
            // Setup
            const string testFilePath = "path";
            const string testVersion = "1";
            const bool usePreprocessor = true;
            const string preprocessorDirectory = "directory";

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = testFilePath,
                    Version = testVersion,
                    Locations =
                    {
                        new HydraulicBoundaryLocation(-1, "name", 1, 2)
                    },
                    CanUsePreprocessor = true,
                    UsePreprocessor = usePreprocessor,
                    PreprocessorDirectory = preprocessorDirectory
                }
            };

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry, 0);

            // Assert
            Assert.AreEqual(1, entity.HydraRingPreprocessorEntities.Count);
            HydraRingPreprocessorEntity preprocessorEntity = entity.HydraRingPreprocessorEntities.First();
            Assert.AreEqual(Convert.ToByte(usePreprocessor), preprocessorEntity.UsePreprocessor);
            Assert.AreEqual(preprocessorDirectory, preprocessorEntity.PreprocessorDirectory);
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

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                ReferenceLine = new ReferenceLine()
            };
            assessmentSection.ReferenceLine.SetGeometry(points);

            var registry = new PersistenceRegistry();

            // Call
            AssessmentSectionEntity entity = assessmentSection.Create(registry, 0);

            // Assert
            string expectedXml = new Point2DXmlSerializer().ToXml(points);
            Assert.AreEqual(expectedXml, entity.ReferenceLinePointXml);
        }

        private static void SetHydraulicBoundaryLocationCalculationInputsOfAssessmentSection(AssessmentSection assessmentSection)
        {
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, 1);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForSignalingNorm, 2);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, 3);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, 4);

            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, 5);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaterLevelCalculationsForSignalingNorm, 6);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, 7);
            SetHydraulicBoundaryLocationCalculationInputs(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, 8);
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

        private static void AssertHydraulicLocationCalculationCollectionEntity(IEnumerable<HydraulicBoundaryLocationCalculation> expectedCalculations,
                                                                               HydraulicLocationCalculationCollectionEntity actualCollectionEntity)
        {
            Assert.IsNotNull(actualCollectionEntity);

            HydraulicBoundaryLocationCalculation[] expectedCalculationsArray = expectedCalculations.ToArray();
            ICollection<HydraulicLocationCalculationEntity> hydraulicLocationCalculationEntities = actualCollectionEntity.HydraulicLocationCalculationEntities;
            Assert.AreEqual(expectedCalculationsArray.Length, hydraulicLocationCalculationEntities.Count);

            var i = 0;
            foreach (HydraulicLocationCalculationEntity actualCalculationEntity in hydraulicLocationCalculationEntities)
            {
                HydraulicBoundaryLocationCalculation expectedCalculation = expectedCalculationsArray[i];
                Assert.AreEqual(Convert.ToByte(expectedCalculation.InputParameters.ShouldIllustrationPointsBeCalculated),
                                actualCalculationEntity.ShouldIllustrationPointsBeCalculated);
                i++;
            }
        }
    }
}