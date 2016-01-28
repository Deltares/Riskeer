using System;
using System.Data.Entity;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    /// <summary>
    /// Converter for <see cref="ProjectEntity"/> to <see cref="Project"/> and <see cref="Project"/> to <see cref="ProjectEntity"/>.
    /// </summary>
    public static class ProjectEntityStorage
    {
        /// <summary>
        /// Gets the ProjectEntity from the <paramref name="dbSet"/>.
        /// </summary>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <returns>A new <see cref="ProjectEntity"/>, loaded from the database, or <c>null</c> when not found.</returns>
        public static ProjectEntity GetProjectEntity(IDbSet<ProjectEntity> dbSet)
        {
            return dbSet.FirstOrDefault();
        }

        /// <summary>
        /// Gets a new <see cref="Project"/>, based on the <see cref="ProjectEntity"/> found in the database.
        /// </summary>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <returns>A new <see cref="Project"/> or null when not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dbSet"/> is null.</exception>
        public static Project GetProject(IDbSet<ProjectEntity> dbSet)
        {
            var entry = GetProjectEntity(dbSet);
            return entry == null ? null : ProjectEntityToProject(entry);
        }

        /// <summary>
        /// Updates the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the <paramref name="dbSet"/>.
        /// </summary>
        /// <remarks>Execute <paramref name="dbSet"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <param name="project"><see cref="Project"/> to be saved in the database.</param>
        /// <returns>The <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dbSet"/> or <paramref name="project"/> is null.</exception>
        /// <exception cref="InvalidOperationException">When multiple instances are found that refer to <paramref name="project"/>.</exception>
        /// <exception cref="EntityNotFoundException">When no entities was found that refer to <paramref name="project"/>.</exception>
        public static ProjectEntity UpdateProjectEntity(IDbSet<ProjectEntity> dbSet, Project project)
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
            ProjectToProjectEntity(project, entry);
            return entry;
        }

        /// <summary>
        /// Insert the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the <paramref name="dbSet"/>.
        /// </summary>
        /// <remarks>Execute <paramref name="dbSet"/>.SaveChanges() afterwards to update the storage.</remarks>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <param name="project"><see cref="Project"/> to be saved in the database.</param>
        /// <returns>New instance of <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dbSet"/> or <paramref name="project"/> is null.</exception>
        public static ProjectEntity InsertProjectEntity(IDbSet<ProjectEntity> dbSet, Project project)
        {
            if (dbSet == null)
            {
                throw new ArgumentNullException("dbSet", "Cannot update databaseSet when no databaseSet is set.");
            }
            var projectEntity = new ProjectEntity();
            ProjectToProjectEntity(project, projectEntity);
            dbSet.Add(projectEntity);
            return projectEntity;
        }

        /// <summary>
        /// Converts <paramref name="projectEntity"/> to a new instance of <see cref="Project"/>.
        /// </summary>
        /// <param name="projectEntity"><see cref="ProjectEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="Project"/>, based on the properties of <paramref name="projectEntity"/>.</returns>
        public static Project ProjectEntityToProject(ProjectEntity projectEntity)
        {
            var project = new Project
            {
                StorageId = projectEntity.ProjectEntityId,
                Name = projectEntity.Name,
                Description = projectEntity.Description
            };

            return project;
        }

        /// <summary>
        /// Converts <paramref name="project"/> to <paramref name="projectEntity"/>.
        /// </summary>
        /// <param name="project">The <see cref="Project"/> to convert.</param>
        /// <param name="projectEntity">A reference to the <see cref="ProjectEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> or <paramref name="projectEntity"/> is null.</exception>
        private static void ProjectToProjectEntity(Project project, ProjectEntity projectEntity)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project", "Cannot convert Project to ProjectEntity when project is not supplied");
            }
            if (projectEntity == null)
            {
                throw new ArgumentNullException("projectEntity", "Cannot convert Project to ProjectEntity when ProjectEntity is not supplied");
            }
            projectEntity.Name = project.Name;
            projectEntity.Description = project.Description;
            projectEntity.LastUpdated = new DateTime().Ticks;
        }
    }
}