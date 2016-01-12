using Application.Ringtoets.Storage.Converter;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage
{
    public class StorageSqLite
    {
        /// <summary>
        /// Saves the <paramref name="project"/> at the default location.
        /// </summary>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes, see <see cref="IRingtoetsDbContext.SaveChanges()"/>.</returns>
        public int SaveProject(Project project)
        {
            using (var dbContext = new RingtoetsEntities())
            {
                ProjectEntityConverter.UpdateProjectEntity(dbContext.ProjectEntities, project);

                return dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the databaseconnection <see cref="RingtoetsEntities"/>.
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the database or <c>null</c> when not found.</returns>
        public Project LoadProject(long projectId)
        {
            using (var dbContext = new RingtoetsEntities())
            {
                return ProjectEntityConverter.GetProject(dbContext.ProjectEntities, projectId);
            }
        }
    }
}