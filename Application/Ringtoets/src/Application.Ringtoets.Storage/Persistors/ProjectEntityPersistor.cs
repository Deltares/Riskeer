using System;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Persistor for <see cref="ProjectEntity"/>.
    /// </summary>
    public class ProjectEntityPersistor : IEntityPersistor<Project, ProjectEntity>
    {
        private readonly ProjectEntityConverter converter;
        private readonly IDbSet<ProjectEntity> dbSet;

        /// <summary>
        /// Instanciate a new ProjectEntityPersistor.
        /// </summary>
        /// <param name="projectEntitiesSet">Sequence <see cref="IDbSet{ProjectEntity}"/> from the storage.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="projectEntitiesSet"/> is null.</exception>
        public ProjectEntityPersistor(IDbSet<ProjectEntity> projectEntitiesSet)
        {
            if (projectEntitiesSet == null)
            {
                throw new ArgumentNullException("projectEntitiesSet");
            }
            dbSet = projectEntitiesSet;
            converter = new ProjectEntityConverter();
        }

        /// <summary>
        /// Gets the only <see cref="ProjectEntity"/> as <see cref="Project"/> from the sequence.
        /// </summary>
        /// <returns>A new <see cref="ProjectEntity"/>, loaded from the sequence, or <c>null</c> when not found.</returns>
        /// <exception cref="InvalidOperationException">Thrown when there are more than one elements in the sequence.</exception>
        public Project GetEntityAsModel()
        {
            var entry = dbSet.SingleOrDefault();
            return entry == null ? null : converter.ConvertEntityToModel(entry);
        }

        /// <summary>
        /// Updates the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="DbContext"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="project"><see cref="Project"/> to be saved in the storage.</param>
        /// <returns>The <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="EntityNotFoundException">Thrown when <paramref name="project"/> is not found.</exception>
        public ProjectEntity UpdateEntity(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project", "Cannot update databaseSet when no project is set.");
            }
            var entry = dbSet.SingleOrDefault(db => db.ProjectEntityId == project.StorageId);
            if (entry == null)
            {
                throw new EntityNotFoundException();
            }
            converter.ConvertModelToEntity(project, entry);
            return entry;
        }

        /// <summary>
        /// Insert the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the sequence.
        /// </summary>
        /// <remarks>Execute <see cref="DbContext"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="project"><see cref="Project"/> to be inserted in the sequence.</param>
        /// <returns>New instance of <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        public ProjectEntity AddEntity(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }
            var projectEntity = new ProjectEntity();
            converter.ConvertModelToEntity(project, projectEntity);
            return dbSet.Add(projectEntity);
        }
    }
}