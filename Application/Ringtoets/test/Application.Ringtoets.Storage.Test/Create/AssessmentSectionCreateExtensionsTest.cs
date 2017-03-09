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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
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
            const double norm = 0.05;
            int order = new Random(65).Next();

            const string mapDataName = "map data name";
            const double transparency = 0.3;
            const bool isVisible = true;
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
                    Norm = norm
                },
                BackgroundData =
                {
                    Name = mapDataName,
                    Transparency = (RoundedDouble) transparency,
                    IsVisible = isVisible
                }
            };
            var registry = new PersistenceRegistry();

            // Call
            var entity = assessmentSection.Create(registry, order);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short) assessmentSectionComposition, entity.Composition);
            Assert.AreEqual(testId, entity.Id);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(comments, entity.Comments);
            Assert.AreEqual(norm, entity.Norm);
            Assert.AreEqual(18, entity.FailureMechanismEntities.Count);
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.Piping));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.GrassRevetmentTopErosionAndInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.MacrostabilityInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short) FailureMechanismType.MacrostabilityOutwards));
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

            Assert.IsNull(entity.HydraulicDatabaseLocation);
            Assert.IsNull(entity.HydraulicDatabaseVersion);
            Assert.IsEmpty(entity.HydraulicLocationEntities);

            Assert.IsNull(entity.ReferenceLinePointXml);

            Assert.AreEqual(1, entity.BackgroundDataEntities.Count);
            var backgroundMapDataEntity = entity.BackgroundDataEntities.Single();
            Assert.IsNotNull(backgroundMapDataEntity);
            Assert.AreEqual(mapDataName, backgroundMapDataEntity.Name);
            Assert.AreEqual(transparency, backgroundMapDataEntity.Transparency);
            Assert.AreEqual(Convert.ToByte(isVisible), backgroundMapDataEntity.IsVisible);
            CollectionAssert.IsEmpty(backgroundMapDataEntity.BackgroundDataMetaEntities);
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
        public void Create_WithHydraulicBoundaryDatabase_SetsPropertiesAndLocationsToEntity()
        {
            // Setup
            string testFilePath = "path";
            string testVersion = "1";

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
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
            var entity = assessmentSection.Create(registry, 0);

            // Assert
            Assert.AreEqual(testFilePath, entity.HydraulicDatabaseLocation);
            Assert.AreEqual(testVersion, entity.HydraulicDatabaseVersion);
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);
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
            var entity = assessmentSection.Create(registry, 0);

            // Assert
            string expectedXml = new Point2DXmlSerializer().ToXml(points);
            Assert.AreEqual(expectedXml, entity.ReferenceLinePointXml);
        }
    }
}