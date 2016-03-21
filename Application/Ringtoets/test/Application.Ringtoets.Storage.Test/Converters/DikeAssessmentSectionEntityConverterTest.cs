﻿using System;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class DikeAssessmentSectionEntityConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_NewDikeAssessmentSectionEntityConverter()
        {
            // Call
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<DikeAssessmentSection, DikeAssessmentSectionEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null, () => new DikeAssessmentSection());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertEntityToModel_ValidDikeAssessmentSectionEntityNullModel_ThrowsArgumentNullException()
        {
            // Setup
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity()
            {
                DikeAssessmentSectionEntityId = storageId,
                Name = name,
                ProjectEntityId = projectId,
                Norm = norm
            };

            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(dikeAssessmentSectionEntity, () => null);

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
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity()
            {
                DikeAssessmentSectionEntityId = storageId,
                Name = name,
                ProjectEntityId = projectId,
                Norm = norm
            };
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Call
            DikeAssessmentSection assessmentSection = converter.ConvertEntityToModel(dikeAssessmentSectionEntity, () => new DikeAssessmentSection());

            // Assert
            Assert.AreNotEqual(dikeAssessmentSectionEntity, assessmentSection);
            Assert.AreEqual(storageId, assessmentSection.StorageId);
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();
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
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();
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
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity
            {
                ProjectEntityId = projectId
            };
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Call
            converter.ConvertModelToEntity(dikeAssessmentSection, dikeAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(dikeAssessmentSectionEntity, dikeAssessmentSection);
            Assert.AreEqual(storageId, dikeAssessmentSectionEntity.DikeAssessmentSectionEntityId);
            Assert.AreEqual(projectId, dikeAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(name, dikeAssessmentSectionEntity.Name);
            Assert.AreEqual(norm, dikeAssessmentSectionEntity.Norm);
        }
    }
}