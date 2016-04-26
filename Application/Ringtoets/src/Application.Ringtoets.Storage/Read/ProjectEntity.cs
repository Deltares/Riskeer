using Application.Ringtoets.Storage.Read;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class ProjectEntity
    {
        public Project Read()
        {
            var collector = new ReadConversionCollector();
            var project = new Project
            {
                StorageId = ProjectEntityId,
                Description = Description
            };

            foreach (var assessmentSectionEntity in AssessmentSectionEntities)
            {
                project.Items.Add(assessmentSectionEntity.Read(collector));
            }

            return project;
        } 
    }
}