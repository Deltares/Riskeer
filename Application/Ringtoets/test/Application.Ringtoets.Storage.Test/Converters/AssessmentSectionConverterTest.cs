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

using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class AssessmentSectionConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_NewAssessmentSectionEntityConverter()
        {
            // Call
            AssessmentSectionConverter converter = new AssessmentSectionConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<AssessmentSection, AssessmentSectionEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            AssessmentSectionConverter converter = new AssessmentSectionConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void ConvertEntityToModel_ValidAssessmentSectionEntity_ReturnsTheAssessmentSectionEntityAsAssessmentSection(AssessmentSectionComposition composition)
        {
            // Setup
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            const string hydraulicDatabaseVersion = "1.0";
            const string hydraulicDatabasePath = "testPath";
            AssessmentSectionEntity assessmentSectionEntity = new AssessmentSectionEntity
            {
                AssessmentSectionEntityId = storageId,
                Name = name,
                ProjectEntityId = projectId,
                Norm = norm,
                HydraulicDatabaseVersion = hydraulicDatabaseVersion,
                HydraulicDatabaseLocation = hydraulicDatabasePath,
                Composition = (short)composition
            };
            AssessmentSectionConverter converter = new AssessmentSectionConverter();

            // Call
            AssessmentSection assessmentSection = converter.ConvertEntityToModel(assessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(assessmentSectionEntity, assessmentSection);
            Assert.AreEqual(storageId, assessmentSection.StorageId);
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(composition, assessmentSection.Composition);
            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);
            double expectedFirstDistributionItemContribution = composition == AssessmentSectionComposition.Dune ?
                                                                   0.0 : 24.0;
            Assert.AreEqual(expectedFirstDistributionItemContribution, assessmentSection.FailureMechanismContribution.Distribution.First().Contribution);
            double expectedLastDistributionItemContribution = composition == AssessmentSectionComposition.DikeAndDune ?
                                                                  20.0 : 30.0;
            Assert.AreEqual(expectedLastDistributionItemContribution, assessmentSection.FailureMechanismContribution.Distribution.Last().Contribution);
            Assert.AreEqual(hydraulicDatabaseVersion, assessmentSection.HydraulicBoundaryDatabase.Version);
            Assert.AreEqual(hydraulicDatabasePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            AssessmentSectionConverter converter = new AssessmentSectionConverter();
            AssessmentSection assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(assessmentSection, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            AssessmentSectionConverter converter = new AssessmentSectionConverter();
            AssessmentSectionEntity assessmentSectionEntity = new AssessmentSectionEntity();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, assessmentSectionEntity);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(AssessmentSectionComposition.Dike)]
        [TestCase(AssessmentSectionComposition.Dune)]
        [TestCase(AssessmentSectionComposition.DikeAndDune)]
        public void ConvertModelToEntity_ValidAssessmentSection_UpdatesTheAssessmentSectionAsAssessmentSectionEntity(AssessmentSectionComposition composition)
        {
            // Setup
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            const string hydraulicDatabaseVersion = "1.0";
            const string hydraulicDatabasePath = "testPath";
            AssessmentSection assessmentSection = new AssessmentSection(composition)
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                },
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Version = hydraulicDatabaseVersion,
                    FilePath = hydraulicDatabasePath
                }
            };

            AssessmentSectionEntity assessmentSectionEntity = new AssessmentSectionEntity
            {
                ProjectEntityId = projectId
            };
            AssessmentSectionConverter converter = new AssessmentSectionConverter();

            // Call
            converter.ConvertModelToEntity(assessmentSection, assessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(assessmentSectionEntity, assessmentSection);
            Assert.AreEqual(storageId, assessmentSectionEntity.AssessmentSectionEntityId);
            Assert.AreEqual(projectId, assessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(name, assessmentSectionEntity.Name);
            Assert.AreEqual((short)composition, assessmentSectionEntity.Composition);
            Assert.AreEqual(norm, assessmentSectionEntity.Norm);
            Assert.AreEqual(hydraulicDatabaseVersion, assessmentSectionEntity.HydraulicDatabaseVersion);
            Assert.AreEqual(hydraulicDatabasePath, assessmentSectionEntity.HydraulicDatabaseLocation);
        }
    }
}