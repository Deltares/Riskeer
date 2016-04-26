using System;
using System.Linq;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Data;
using Ringtoets.Integration.Data;
using Resources = Application.Ringtoets.Storage.Properties.Resources;

namespace Application.Ringtoets.Storage.DbContext
{
    public static class ProjectUpdateExtensions
    {
        public static void Update(this Project project, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = ReadSingleProject(project, context);
            entity.Description = project.Description;

            foreach (var result in project.Items.OfType<AssessmentSection>())
            {
                if (result.IsNew())
                {
                    entity.AssessmentSectionEntities.Add(result.Create(collector));
                }
                else
                {
                    result.Update(collector, context);
                }
            }

            collector.Update(entity);
        }

        private static ProjectEntity ReadSingleProject(Project project, IRingtoetsEntities context)
        {
            try
            {
                return context.ProjectEntities.Single(pe => pe.ProjectEntityId == project.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(ProjectEntity).Name, project.StorageId), exception);
            }
        }
    }
}