﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
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
            TestDelegate test = () => assessmentSection.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        [TestCase(AssessmentSectionComposition.Dune)]
        public void Create_WithCollector_ReturnsAssessmentSectionEntityWithCompositionAndFailureMechanismForPiping(AssessmentSectionComposition assessmentSectionComposition)
        {
            // Setup
            string testName = "testName";
            var assessmentSection = new AssessmentSection(assessmentSectionComposition)
            {
                Name = testName
            };
            var collector = new PersistenceRegistry();

            // Call
            var entity = assessmentSection.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short)assessmentSectionComposition, entity.Composition);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(18, entity.FailureMechanismEntities.Count);
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.Piping));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.GrassRevetmentTopErosionAndInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.MacrostabilityInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.MacrostabilityOutwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.Microstability));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.StabilityStoneRevetment));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.WaveImpactOnAsphaltRevetment));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.WaterOverpressureAsphaltRevetment));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.GrassRevetmentErosionOutwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.GrassRevetmentSlidingOutwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.GrassRevetmentSlidingInwards));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.StructureHeight));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.ReliabilityClosingOfStructure));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.PipingAtStructure));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.StrengthAndStabilityPointConstruction));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.StrengthAndStabilityParallelConstruction));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.DuneErosion));
            Assert.IsNotNull(entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (short)FailureMechanismType.TechnicalInnovations));

            Assert.IsNull(entity.HydraulicDatabaseLocation);
            Assert.IsNull(entity.HydraulicDatabaseVersion);
            Assert.IsEmpty(entity.HydraulicLocationEntities);

            Assert.IsEmpty(entity.ReferenceLinePointEntities);
        }

        [Test]
        public void Create_WithHydraulicBoundaryDatabase_SetsPropertiesAndLocationsToEntity()
        {
            // Setup
            string testFilePath = "path";
            string testVersion = "1";

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = testFilePath,
                Version = testVersion,
                Locations =
                {
                    new HydraulicBoundaryLocation(-1, "name", 1, 2)
                }
            };
            var collector = new PersistenceRegistry();

            // Call
            var entity = assessmentSection.Create(collector);

            // Assert
            Assert.AreEqual(testFilePath, entity.HydraulicDatabaseLocation);
            Assert.AreEqual(testVersion, entity.HydraulicDatabaseVersion);
            Assert.AreEqual(1, entity.HydraulicLocationEntities.Count);
        } 

        [Test]
        public void Create_WithReferenceLine_AddsReferenceLinePointEntities()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.ReferenceLine = new ReferenceLine();
            assessmentSection.ReferenceLine.SetGeometry(new []
            {
                new Point2D(1,0),
                new Point2D(2,3),
                new Point2D(5,3)
            });
            var collector = new PersistenceRegistry();

            // Call
            var entity = assessmentSection.Create(collector);

            // Assert
            Assert.AreEqual(3, entity.ReferenceLinePointEntities.Count);
            Assert.AreEqual(new []{0,1,2}, entity.ReferenceLinePointEntities.Select(rpe => rpe.Order));
        } 
    }
}