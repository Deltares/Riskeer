using System;
using System.Data.Entity;
using System.Linq;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.Converter
{
    /// <summary>
    /// Converter for ProjectEntity.
    /// </summary>
    public static class ProjectEntityConverter
    {
        /// <summary>
        /// Gets a new <see cref="Project"/>, based on the <see cref="ProjectEntity"/> found in the database.
        /// </summary>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <returns>A new <see cref="Project"/> or null when not found.</returns>
        public static Project GetProject(IDbSet<ProjectEntity> dbSet)
        {
            var entry = dbSet.FirstOrDefault();
            return entry == null ? null : ProjectEntityToProject(entry);
        }

        /// <summary>
        /// Updates the <see cref="ProjectEntity"/>, based upon the <paramref name="project"/>, in the database.
        /// </summary>
        /// <param name="dbSet">Database set of <see cref="ProjectEntity"/>.</param>
        /// <param name="project"><see cref="Project"/> to be saved in the database.</param>
        public static void UpdateProjectEntity(IDbSet<ProjectEntity> dbSet, Project project)
        {
            var entry = dbSet.SingleOrDefault(db => db.ProjectEntityId == project.StorageId);
            if (entry == null)
            {
                entry = new ProjectEntity();
                dbSet.Add(entry);
            }
            ProjectToProjectEntity(project, entry);
        }

        /// <summary>
        /// Converts <paramref name="projectEntity"/> to a new instance of <see cref="Project"/>.
        /// </summary>
        /// <param name="projectEntity"><see cref="ProjectEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="Project"/>, based on the properties of <paramref name="projectEntity"/>.</returns>
        public static Project ProjectEntityToProject( ProjectEntity projectEntity )
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
        public static void ProjectToProjectEntity(Project project, ProjectEntity projectEntity)
        {
            projectEntity.Name = project.Name;
            projectEntity.Description = project.Description;
            projectEntity.LastUpdated = new DateTime().Ticks;
        }

    }
}
