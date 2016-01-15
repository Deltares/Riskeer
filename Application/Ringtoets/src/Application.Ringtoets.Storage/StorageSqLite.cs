using System;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using Application.Ringtoets.Storage.Converter;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Core.Common.Base.Data;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file.
    /// </summary>
    public class StorageSqLite
    {
        private readonly string connectionString;

        /// <summary>
        /// Creates a new instance of <see cref="StorageSqLite"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        public StorageSqLite(string databaseFilePath)
        {
            try
            {
                FileUtils.ValidateFilePath(databaseFilePath);
            }
            catch (ArgumentException e)
            {
                throw new InvalidFileException(e.Message, e);
            }
            if (!File.Exists(databaseFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilsResources.Error_File_does_not_exist);
                throw new FileNotFoundException(message);
            }

            connectionString = SqLiteStorageConnection.BuildConnectionString(databaseFilePath);
        }

        /// <summary>
        /// Tests if a connection can be made.
        /// </summary>
        /// <returns>Returns <c>true</c> if a valid connection can be made, <c>false</c> otherwise.</returns>
        public bool TestConnection()
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.Database.Connection.Open();
                    dbContext.Versions.Load();
                    return true;
                }
                catch (MetadataException) { }
                catch (InvalidOperationException){}
                catch (EntityCommandExecutionException){}
                catch (Exception) { }
                return false;
            }
        }

        /// <summary>
        /// Saves the <paramref name="project"/> at the default location.
        /// </summary>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        public int SaveProject(Project project)
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                var changes = 0;
                try
                {
                    ProjectEntityConverter.UpdateProjectEntity(dbContext.ProjectEntities, project);
                    changes = dbContext.SaveChanges();
                }
                catch (ArgumentNullException) {}
                catch (DbEntityValidationException) {}
                catch (NotSupportedException) {}
                catch (ObjectDisposedException) {}
                catch (InvalidOperationException) {}
                catch (DbUpdateConcurrencyException) {}
                catch (DbUpdateException) {}
                return changes;
            }
        }

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the SQLite database.
        /// </summary>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the database or <c>null</c> when not found.</returns>
        public Project LoadProject()
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    return ProjectEntityConverter.GetProject(dbContext.ProjectEntities);
                }
                catch (ArgumentNullException) {}
            }
            return null;
        }
    }
}