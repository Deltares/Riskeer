using System;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;

namespace Application.Ringtoets.Storage.Converters
{
    internal enum AssessmentSectionTypes
    {
        DikeAssessment = 1,
        DuneAssessmentSection = 2
    }

    internal class AssessmentSectionEntityConverter
    {
        private readonly long projectEntityId;

        public AssessmentSectionEntityConverter(long projectId)
        {
            projectEntityId = projectId;
        }

        public DikeAssessmentSection DikeAssessmentSection(IDbSet<AssessmentSectionEntity> dbSet, long storageId)
        {
            var assessmentSectionEntity = GetAssessmentSectionEntity(dbSet, AssessmentSectionTypes.DikeAssessment, storageId);
            if (assessmentSectionEntity == null)
            {
                return null;
            }

            return DikeAssessmentSectionEntityToAssessmentSection(assessmentSectionEntity);
        }

        public DuneAssessmentSection DuneAssessmentSection(IDbSet<AssessmentSectionEntity> dbSet, long storageId)
        {
            var a = GetAssessmentSectionEntity(dbSet, AssessmentSectionTypes.DuneAssessmentSection, storageId);
            throw new NotImplementedException();
        }

        private AssessmentSectionEntity GetAssessmentSectionEntity(IDbSet<AssessmentSectionEntity> dbSet, AssessmentSectionTypes type, long storageId)
        {
            return dbSet.SingleOrDefault(db => db.AssessmentSectionEntityId == storageId && db.ProjectEntityId == projectEntityId && db.Type == (int)type);
        }

        private void DikeAssessmentSectionToAssessmentSectionEntity(DikeAssessmentSection dikeAssessmentSection, AssessmentSectionEntity assessmentSectionEntity)
        {
            if (dikeAssessmentSection == null || assessmentSectionEntity == null)
            {
                throw new ArgumentNullException();
            }
            assessmentSectionEntity.ProjectEntityId = projectEntityId;
            assessmentSectionEntity.AssessmentSectionEntityId = dikeAssessmentSection.StorageId;
            assessmentSectionEntity.Name = dikeAssessmentSection.Name;
        }

        private DikeAssessmentSection DikeAssessmentSectionEntityToAssessmentSection(AssessmentSectionEntity assessmentSectionEntity)
        {
            if (assessmentSectionEntity == null)
            {
                throw new ArgumentNullException();
            }
            var dikeAssessmentSection = new DikeAssessmentSection
            {
                StorageId = assessmentSectionEntity.AssessmentSectionEntityId,
                Name = assessmentSectionEntity.Name
            };

            if (assessmentSectionEntity.Norm != null)
            {
                dikeAssessmentSection.FailureMechanismContribution.Norm = assessmentSectionEntity.Norm.Value;
            }

            return dikeAssessmentSection;
        }
    }
}