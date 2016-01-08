using System;
using System.Linq;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage
{
    public class StorageSqLite : IStoreProject
    {
        public int SaveProject(Project project)
        {
            using (var dbContext = new RingtoetsEntities())
            {
                var entry = dbContext.ProjectEntities.SingleOrDefault(db => db.ProjectEntityId == project.StorageId);
                if (entry == null)
                {
                    return 0;
                }

                entry.Name = project.Name;
                entry.Description = project.Description;
                entry.LastUpdated = new DateTime().Ticks;

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
                var entry = dbContext.ProjectEntities.SingleOrDefault(db => db.ProjectEntityId == projectId);
                if (entry == null)
                {
                    return null;
                }

                var project = new Project
                {
                    StorageId = entry.ProjectEntityId,
                    Name = entry.Name,
                    Description = entry.Description
                };

                return project;
            }
        }
    
    }
}