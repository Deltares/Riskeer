using System;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class DuneAssessmentSectionEntityConverterTest
    {
        [Test]
        public void DefaultConstructor_Always_NewDuneAssessmentSectionEntityConverter()
        {
            // Call
            DuneAssessmentSectionEntityConverter converter = new DuneAssessmentSectionEntityConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<DuneAssessmentSection, DuneAssessmentSectionEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            DuneAssessmentSectionEntityConverter converter = new DuneAssessmentSectionEntityConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertEntityToModel_ValidDuneAssessmentSectionEntity_ReturnsTheDuneAssessmentSectionEntityAsDuneAssessmentSection()
        {
            // SetUp
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            DuneAssessmentSectionEntity duneAssessmentSectionEntity = new DuneAssessmentSectionEntity()
            {
                DuneAssessmentSectionEntityId = storageId,
                Name = name,
                ProjectEntityId = projectId,
                Norm = norm
            };
            DuneAssessmentSectionEntityConverter converter = new DuneAssessmentSectionEntityConverter();

            // Call
            DuneAssessmentSection assessmentSection = converter.ConvertEntityToModel(duneAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(duneAssessmentSectionEntity, assessmentSection);
            Assert.AreEqual(storageId, assessmentSection.StorageId);
            Assert.AreEqual(name, assessmentSection.Name);
            Assert.AreEqual(norm, assessmentSection.FailureMechanismContribution.Norm);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // SetUp
            DuneAssessmentSectionEntityConverter converter = new DuneAssessmentSectionEntityConverter();
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(duneAssessmentSection, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // SetUp
            DuneAssessmentSectionEntityConverter converter = new DuneAssessmentSectionEntityConverter();
            DuneAssessmentSectionEntity duneAssessmentSectionEntity = new DuneAssessmentSectionEntity();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, duneAssessmentSectionEntity);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void ConvertModelToEntity_ValidDuneAssessmentSection_UpdatesTheDuneAssessmentSectionAsDuneAssessmentSectionEntity()
        {
            // SetUp
            const long storageId = 1234L;
            const long projectId = 1L;
            const int norm = 30000;
            const string name = "test";
            DuneAssessmentSection duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = storageId,
                Name = name,
                FailureMechanismContribution =
                {
                    Norm = norm
                }
            };
            DuneAssessmentSectionEntity duneAssessmentSectionEntity = new DuneAssessmentSectionEntity
            {
                ProjectEntityId = projectId
            };
            DuneAssessmentSectionEntityConverter converter = new DuneAssessmentSectionEntityConverter();

            // Call
            converter.ConvertModelToEntity(duneAssessmentSection, duneAssessmentSectionEntity);

            // Assert
            Assert.AreNotEqual(duneAssessmentSectionEntity, duneAssessmentSection);
            Assert.AreEqual(storageId, duneAssessmentSectionEntity.DuneAssessmentSectionEntityId);
            Assert.AreEqual(projectId, duneAssessmentSectionEntity.ProjectEntityId);
            Assert.AreEqual(name, duneAssessmentSectionEntity.Name);
            Assert.AreEqual(norm, duneAssessmentSectionEntity.Norm);
        }
    }
}