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
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class DikeAssessmentSectionConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_NewDikeAssessmentSectionEntityConverter()
        {
            // Call
            DikeAssessmentSectionConverter converter = new DikeAssessmentSectionConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<DikeAssessmentSection, DikeAssessmentSectionEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            DikeAssessmentSectionConverter converter = new DikeAssessmentSectionConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertEntityToModel_ValidDikeAssessmentSectionEntity_ReturnsTheDikeAssessmentSectionEntityAsDikeAssessmentSection()
        {
            // Setup
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            const string hydraulicDatabaseVersion = "1.0";
            const string hydraulicDatabasePath = "testPath";
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity()
            {
                DikeAssessmentSectionEntityId = storageId,
                Name = name,
                ProjectEntityId = projectId,
                Norm = norm,
                HydraulicDatabaseVersion = hydraulicDatabaseVersion,
                HydraulicDatabaseLocation = hydraulicDatabasePath
            };
            DikeAssessmentSectionConverter converter = new DikeAssessmentSectionConverter();

            // Call
            DikeAssessmentSection assessmentSection = converter.ConvertEntityToModel(dikeAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(dikeAssessmentSectionEntity, assessmentSection);
            Assert.AreEqual(storageId, assessmentSection.StorageId);
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);
            Assert.AreEqual(hydraulicDatabaseVersion, assessmentSection.HydraulicBoundaryDatabase.Version);
            Assert.AreEqual(hydraulicDatabasePath, assessmentSection.HydraulicBoundaryDatabase.FilePath);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            DikeAssessmentSectionConverter converter = new DikeAssessmentSectionConverter();
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(dikeAssessmentSection, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            DikeAssessmentSectionConverter converter = new DikeAssessmentSectionConverter();
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, dikeAssessmentSectionEntity);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_ValidDikeAssessmentSection_UpdatesTheDikeAssessmentSectionAsDikeAssessmentSectionEntity()
        {
            // Setup
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            const string hydraulicDatabaseVersion = "1.0";
            const string hydraulicDatabasePath = "testPath";
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
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
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity
            {
                ProjectEntityId = projectId
            };
            DikeAssessmentSectionConverter converter = new DikeAssessmentSectionConverter();

            // Call
            converter.ConvertModelToEntity(dikeAssessmentSection, dikeAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(dikeAssessmentSectionEntity, dikeAssessmentSection);
            Assert.AreEqual(storageId, dikeAssessmentSectionEntity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(projectId, dikeAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(name, dikeAssessmentSectionEntity.Name);
            Assert.AreEqual(norm, dikeAssessmentSectionEntity.Norm);
            Assert.AreEqual(hydraulicDatabaseVersion, dikeAssessmentSectionEntity.HydraulicDatabaseVersion);
            Assert.AreEqual(hydraulicDatabasePath, dikeAssessmentSectionEntity.HydraulicDatabaseLocation);
        }
    }
}