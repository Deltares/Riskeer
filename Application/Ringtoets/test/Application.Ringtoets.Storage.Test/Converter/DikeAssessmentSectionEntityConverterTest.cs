using System;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Converter
{
    [TestFixture]
    public class DikeAssessmentSectionEntityConverterTest
    {
        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertEntityToModel_ValidDikeAssessmentSectionEntity_ReturnsTheDikeAssessmentSectionEntityAsDikeAssessmentSection()
        {
            // SetUp
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
            DikeAssessmentSection assessmentSection = converter.ConvertEntityToModel(dikeAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(dikeAssessmentSectionEntity, assessmentSection);
            Assert.AreEqual(storageId, assessmentSection.StorageId);
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
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
            // SetUp
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
            // SetUp
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            DikeAssessmentSection dikeAssessmentSection = new DikeAssessmentSection
            {
                StorageId = storageId,
                Name = name
            };
            dikeAssessmentSection.FailureMechanismContribution.Norm = norm;
            DikeAssessmentSectionEntity dikeAssessmentSectionEntity = new DikeAssessmentSectionEntity
            {
                ProjectEntityId = projectId
            };
            DikeAssessmentSectionEntityConverter converter = new DikeAssessmentSectionEntityConverter();

            // Call
            converter.ConvertModelToEntity(dikeAssessmentSection, dikeAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(dikeAssessmentSectionEntity, dikeAssessmentSection);
            Assert.AreEqual(storageId, dikeAssessmentSection.StorageId);
            Assert.AreEqual(name, dikeAssessmentSection.Name);
            Assert.AreEqual(norm, dikeAssessmentSection.FailureMechanismContribution.Norm);
        }
    }
}