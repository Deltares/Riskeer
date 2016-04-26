using System;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Core.Common.Base.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class ProjectCreateExtensions
    {
        public static ProjectEntity Create(this Project project, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new ProjectEntity
            {
                Description = project.Description
            };

            CreateAssessmentSections(project, entity, collector);

            collector.Add(entity, project);
            return entity;
        }

        private static void CreateAssessmentSections(Project project, ProjectEntity entity, CreateConversionCollector collector)
        {
            foreach (var result in project.Items.OfType<AssessmentSection>())
            {
                entity.AssessmentSectionEntities.Add(result.Create(collector));
            }
        }
    }
}