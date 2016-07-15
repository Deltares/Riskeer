// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : IStoreProject
    {
        private string filePath;
        private string connectionString;

        public string FileFilter
        {
            get
            {
                return Resources.Ringtoets_project_file_filter;
            }
        }

        /// <summary>
        /// Converts <paramref name="project"/> to a new <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item>The database does not contain the table <c>version</c></item>
        /// <item>THe file <paramref name="databaseFilePath"/> was not created.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// </list>
        /// </exception>
        public int SaveProjectAs(string databaseFilePath, Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            SetConnectionToNewFile(databaseFilePath);

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var registry = new PersistenceRegistry();
                    dbContext.ProjectEntities.Add(project.Create(registry));
                    var changes = dbContext.SaveChanges();
                    registry.TransferIds();

                    project.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                    return changes;
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    if (exception is InvalidOperationException || exception is NotSupportedException)
                    {
                        throw CreateStorageWriterException(Resources.Error_During_Connection, exception);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Converts <paramref name="project"/> to an existing <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist.</item>
        /// <item>The database does not contain the table <c>version</c>.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// <item>The related entity was not found in the database. Therefore, no update was possible.</item>
        /// </list>
        /// </exception>
        public int SaveProject(string databaseFilePath, Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            try
            {
                SetConnectionToExistingFile(databaseFilePath);
            }
            catch
            {
                return SaveProjectAs(databaseFilePath, project);
            }

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var updateCollector = new PersistenceRegistry();
                    project.Update(updateCollector, dbContext);
                    updateCollector.RemoveUntouched(dbContext);
                    var changes = dbContext.SaveChanges();
                    updateCollector.TransferIds();

                    return changes;
                }
                catch (EntityNotFoundException e)
                {
                    throw CreateStorageWriterException(e.Message, e);
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    if (exception is InvalidOperationException || exception is NotSupportedException)
                    {
                        throw CreateStorageWriterException(Resources.Error_During_Connection, exception);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the SQLite database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the database or <c>null</c> when not found.</returns>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist.</item>
        /// <item>The database does not contain all requested tables.</item>
        /// <item>The connection to the database file failed.</item>
        /// <item>The related entity was not found in the database.</item>
        /// </list>
        /// </exception>
        public Project LoadProject(string databaseFilePath)
        {
            SetConnectionToExistingFile(databaseFilePath);
            try
            {
                using (var dbContext = new RingtoetsEntities(connectionString))
                {
                    ProjectEntity projectEntity;

                    try
                    {
                        projectEntity = dbContext.ProjectEntities.Single();
                    }
                    catch (InvalidOperationException)
                    {
                        throw CreateStorageReaderException(Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file);
                    }

                    var project = projectEntity.Read(new ReadConversionCollector());

                    project.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                    return project;
                }
            }
            catch (DataException exception)
            {
                throw CreateStorageReaderException(string.Empty, exception);
            }
            catch (SystemException exception)
            {
                throw CreateStorageReaderException(string.Empty, exception);
            }
        }

        public void CloseProject()
        {
            filePath = null;
            connectionString = null;
        }

        public bool HasChanges(Project project)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return true;
            }

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var persistenceRegistry = new PersistenceRegistry();
                    project.Update(persistenceRegistry, dbContext);
                    persistenceRegistry.RemoveUntouched(dbContext);

                    return dbContext.ChangeTracker.HasChanges();
                }
                catch (EntityNotFoundException)
                {
                    return true;
                }
                catch (SystemException)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Attempts to set the connection to an existing storage file <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist</item>
        /// <item>the database has an invalid schema.</item>
        /// </list>
        /// </exception>
        private void SetConnectionToExistingFile(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            SetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Sets the connection to a newly created (empty) Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item><paramref name="databaseFilePath"/> was not created</item>
        /// <item>executing <c>DatabaseStructure</c> script failed</item>
        /// </list>
        /// </exception>
        /// <exception cref="IOException">Thrown when the <paramref name="databaseFilePath"/> could not 
        /// be overwritten.</exception>
        private void SetConnectionToNewFile(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            CreateDatabaseStructureInFile(databaseFilePath);
            SetConnectionToFile(databaseFilePath);
        }

        private void SetConnectionToFile(string databaseFilePath)
        {
            filePath = databaseFilePath;

            if (!File.Exists(databaseFilePath))
            {
                throw CreateStorageReaderException(string.Empty, new CouldNotConnectException(UtilsResources.Error_File_does_not_exist));
            }

            connectionString = SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(databaseFilePath);

            ValidateStorage();
        }

        /// <summary>
        /// Validates if the connected storage is a valid Ringtoets database.
        /// </summary>
        /// <exception cref="StorageException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private void ValidateStorage()
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.VersionEntities.Load();
                }
                catch (Exception exception)
                {
                    throw CreateStorageReaderException(string.Empty, new StorageValidationException(string.Format(Resources.Error_Validating_Database_0, filePath), exception));
                }
            }
        }

        private static void CreateDatabaseStructureInFile(string databaseFilePath)
        {
            var temporaryFile = databaseFilePath + "~";

            if (File.Exists(temporaryFile))
            {
                TryDeleteExistingTemporaryFile(temporaryFile);
            }

            TryCreateTemporaryFile(temporaryFile);
            StorageSqliteCreator.CreateDatabaseStructure(temporaryFile);
            TryMoveTemporaryFileToDestination(databaseFilePath, temporaryFile);
        }

        private static void TryMoveTemporaryFileToDestination(string databaseFilePath, string temporaryFile)
        {
            try
            {
                File.Delete(databaseFilePath);
                File.Move(temporaryFile, databaseFilePath);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.StorageSqLite_TryMoveTemporaryFileToDestination_Could_not_overwrite_file_0, databaseFilePath);
                    throw new IOException(message, e);
                }
                throw;
            }
            finally
            {
                try
                {
                    File.Delete(temporaryFile);
                }
                catch
                {
                    // ignored
                }
            }
        }

        private static void TryCreateTemporaryFile(string temporaryFile)
        {
            try
            {
                using (File.Create(temporaryFile)) {}
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.StorageSqLite_TryCreateTemporaryFile_Could_not_create_temporary_file_at_path_0, temporaryFile);
                    throw new IOException(message, e);
                }
                throw;
            }
        }

        private static void TryDeleteExistingTemporaryFile(string temporaryFile)
        {
            try
            {
                File.Delete(temporaryFile);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException || e is SystemException)
                {
                    var message = string.Format(Resources.StorageSqLite_TryDeleteExistingTemporaryFile_Could_not_delete_existing_file_0, temporaryFile);
                    throw new IOException(message, e);
                }
                throw;
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when writing to the storage file failed.
        /// </summary>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageWriterException(string errorMessage, Exception innerException = null)
        {
            var message = new FileWriterErrorMessageBuilder(filePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when reading the storage file failed.
        /// </summary>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageReaderException(string errorMessage, Exception innerException = null)
        {
            var message = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }
    }
}