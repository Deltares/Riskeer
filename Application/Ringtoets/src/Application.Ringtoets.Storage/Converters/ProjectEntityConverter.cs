using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converter for <see cref="ProjectEntity"/> to <see cref="Project"/> 
    /// and <see cref="Project"/> to <see cref="ProjectEntity"/>.
    /// </summary>
    public class ProjectEntityConverter : IEntityConverter<Project, ProjectEntity>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="Project"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="Project"/>, based on the properties of <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public Project ConvertEntityToModel(ProjectEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var project = new Project
            {
                StorageId = entity.ProjectEntityId,
                Name = entity.Name,
                Description = entity.Description
            };

            return project;
        }

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="Project"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="ProjectEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="modelObject"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// </list></exception>
        public void ConvertModelToEntity(Project modelObject, ProjectEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.ProjectEntityId = modelObject.StorageId;
            entity.Name = modelObject.Name;
            entity.Description = modelObject.Description;
            entity.LastUpdated = new DateTime().Ticks;
        }
    }
}