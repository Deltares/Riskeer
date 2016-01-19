using System;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converter for <see cref="ProjectEntity"/> to <see cref="Project"/> and <see cref="Project"/> to <see cref="ProjectEntity"/>.
    /// </summary>
    public static class ProjectEntityConverter
    {
        /// <summary>
        /// Gets a new <see cref="Project"/>, based on the <see cref="ProjectEntity"/> found in the database.
        /// </summary>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <returns>A new <see cref="Project"/> or null when not found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="dbSet"/> is null.</exception>
        public static Project GetProject(IDbSet<ProjectEntity> dbSet)
        {
            var entry = dbSet.FirstOrDefault();
            return entry == null ? null : ProjectEntityToProject(entry);
        }

        /// <summary>
        /// Updates the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the <paramref name="dbSet"/>.
        /// </summary>
        /// <remarks>Execute <paramref name="dbSet"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <param name="project"><see cref="Project"/> to be saved in the database.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dbSet"/> or <paramref name="project"/> is null.</exception>
        /// <exception cref="InvalidOperationException">When multiple instances are found that refer to <paramref name="project"/>.</exception>
        /// <exception cref="EntityNotFoundException">When no entities was found that refer to <paramref name="project"/>.</exception>
        public static void UpdateProjectEntity(IDbSet<ProjectEntity> dbSet, Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException();
            }
            var entry = dbSet.SingleOrDefault(db => db.ProjectEntityId == project.StorageId);
            if (entry == null)
            {
                throw new EntityNotFoundException();
            }
            ProjectToProjectEntity(project, entry);
        }

        /// <summary>
        /// Converts <paramref name="project"/> to <paramref name="projectEntity"/>.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to convert.</param>
        /// <param name="projectEntity">A reference to the <see cref="ProjectEntity"/> to be saved.</param>
        public static void ProjectToProjectEntity(Project project, ProjectEntity projectEntity)
        {
            projectEntity.Name = project.Name;
            projectEntity.Description = project.Description;
            projectEntity.LastUpdated = new DateTime().Ticks;
        }

        /// <summary>
        /// Converts <paramref name="projectEntity"/> to a new instance of <see cref="Project"/>.
        /// </summary>
        /// <param name="projectEntity"><see cref="ProjectEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="Project"/>, based on the properties of <paramref name="projectEntity"/>.</returns>
        private static Project ProjectEntityToProject(ProjectEntity projectEntity)
        {
            var project = new Project
            {
                StorageId = projectEntity.ProjectEntityId,
                Name = projectEntity.Name,
                Description = projectEntity.Description
            };

            return project;
        }
    }
}