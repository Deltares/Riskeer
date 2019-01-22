// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.ServiceModel;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Util;
using Core.Common.Util.Builders;
using log4net;
using Ringtoets.Common.Util;
using Ringtoets.Integration.Data;
using Ringtoets.Storage.Core.Create;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Exceptions;
using Ringtoets.Storage.Core.Properties;
using Ringtoets.Storage.Core.Read;
using UtilResources = Core.Common.Util.Properties.Resources;

namespace Ringtoets.Storage.Core
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : IStoreProject
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StorageSqLite));

        private StagedProject stagedProject;

        public string FileFilter
        {
            get
            {
                return Resources.Ringtoets_project_file_filter;
            }
        }

        public bool HasStagedProject
        {
            get
            {
                return stagedProject != null;
            }
        }

        public void StageProject(IProject project)
        {
            var ringtoetsProject = project as RingtoetsProject;
            if (ringtoetsProject == null)
            {
                throw new ArgumentNullException(nameof(project));
            }

            var registry = new PersistenceRegistry();

            stagedProject = new StagedProject(ringtoetsProject, ringtoetsProject.Create(registry));
        }

        public void UnstageProject()
        {
            stagedProject = null;
        }

        public void SaveProjectAs(string databaseFilePath)
        {
            if (!HasStagedProject)
            {
                throw new InvalidOperationException("Call 'StageProject(IProject)' first before calling this method.");
            }

            try
            {
                var writer = new BackedUpFileWriter(databaseFilePath);
                writer.Perform(() => SaveProjectInDatabase(databaseFilePath));
            }
            catch (IOException e)
            {
                throw new StorageException(e.Message, e);
            }
            catch (CannotDeleteBackupFileException e)
            {
                log.Warn(e.Message, e);
            }
            finally
            {
                UnstageProject();
            }
        }

        public IProject LoadProject(string databaseFilePath)
        {
            string connectionString = GetConnectionToExistingFile(databaseFilePath);
            try
            {
                RingtoetsProject project;
                using (var dbContext = new RiskeerEntities(connectionString))
                {
                    ValidateDatabaseVersion(dbContext, databaseFilePath);

                    dbContext.LoadTablesIntoContext();

                    ProjectEntity projectEntity;
                    try
                    {
                        projectEntity = dbContext.ProjectEntities.Local.Single();
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
                    }

                    project = projectEntity.Read(new ReadConversionCollector());
                }

                project.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                return project;
            }
            catch (DataException exception)
            {
                throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
            }
            catch (SystemException exception)
            {
                throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
            }
        }

        public bool HasStagedProjectChanges(string filePath)
        {
            if (!HasStagedProject)
            {
                throw new InvalidOperationException("Call 'StageProject(IProject)' first before calling this method.");
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return true;
            }

            string connectionString = GetConnectionToExistingFile(filePath);
            try
            {
                byte[] originalHash;
                using (var dbContext = new RiskeerEntities(connectionString))
                    originalHash = dbContext.VersionEntities.Select(v => v.FingerPrint).First();

                byte[] hash = FingerprintHelper.Get(stagedProject.Entity);
                return !FingerprintHelper.AreEqual(originalHash, hash);
            }
            catch (CannotDetermineFingerprintException e)
            {
                if (e.InnerException is QuotaExceededException)
                {
                    throw new StorageException(Resources.StorageSqLite_HasStagedProjectChanges_Project_contains_too_many_objects_to_generate_fingerprint, e);
                }

                throw new StorageException(e.Message, e);
            }
        }

        private void SaveProjectInDatabase(string databaseFilePath)
        {
            string connectionString = GetConnectionToNewFile(databaseFilePath);
            using (var dbContext = new RiskeerEntities(connectionString))
            {
                try
                {
                    dbContext.VersionEntities.Add(new VersionEntity
                    {
                        Version = RingtoetsVersionHelper.GetCurrentDatabaseVersion(),
                        Timestamp = DateTime.Now,
                        FingerPrint = FingerprintHelper.Get(stagedProject.Entity)
                    });
                    dbContext.ProjectEntities.Add(stagedProject.Entity);
                    dbContext.SaveChanges();
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(databaseFilePath, Resources.Error_saving_database, exception);
                }
                catch (CannotDetermineFingerprintException exception)
                {
                    throw CreateStorageWriterException(databaseFilePath, exception.Message, exception);
                }
                catch (SystemException exception)
                {
                    if (exception is InvalidOperationException || exception is NotSupportedException)
                    {
                        throw CreateStorageWriterException(databaseFilePath, Resources.Error_during_connection, exception);
                    }

                    throw;
                }

                stagedProject.Model.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
            }
        }

        private static void ValidateDatabaseVersion(RiskeerEntities riskeerEntities, string databaseFilePath)
        {
            try
            {
                string databaseVersion = riskeerEntities.VersionEntities.Select(v => v.Version).Single();
                if (!RingtoetsVersionHelper.IsValidVersion(databaseVersion))
                {
                    string m = string.Format(Resources.StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_is_invalid,
                                             databaseVersion);
                    string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(m);
                    throw new StorageValidationException(message);
                }

                if (RingtoetsVersionHelper.IsNewerThanCurrent(databaseVersion))
                {
                    string m = string.Format(Resources.StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_higher_then_current_DatabaseVersion_1_,
                                             databaseVersion, RingtoetsVersionHelper.GetCurrentDatabaseVersion());
                    string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(m);
                    throw new StorageValidationException(message);
                }
            }
            catch (InvalidOperationException e)
            {
                string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(Resources.StorageSqLite_ValidateDatabaseVersion_Database_must_have_one_VersionEntity_row);
                throw new StorageValidationException(message, e);
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
        private static string GetConnectionToExistingFile(string databaseFilePath)
        {
            IOUtils.ValidateFilePath(databaseFilePath);
            return GetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Sets the connection to a newly created (empty) Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> is invalid</item>
        /// <item><paramref name="databaseFilePath"/> points to an existing file</item>
        /// </list></exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item>executing <c>DatabaseStructure</c> script failed</item>
        /// </list>
        /// </exception>
        private static string GetConnectionToNewFile(string databaseFilePath)
        {
            IOUtils.ValidateFilePath(databaseFilePath);
            StorageSqliteCreator.CreateDatabaseStructure(databaseFilePath);
            return GetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Establishes a connection to an existing <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to connect to.</param>
        /// <exception cref="CouldNotConnectException">No file exists at <paramref name="databaseFilePath"/>.</exception>
        private static string GetConnectionToFile(string databaseFilePath)
        {
            if (!File.Exists(databaseFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilResources.Error_File_does_not_exist);
                throw new CouldNotConnectException(message);
            }

            return GetConnectionToStorage(databaseFilePath);
        }

        /// <summary>
        /// Sets the connection to the Ringtoets database.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file, which is used for creating exceptions.</param>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private static string GetConnectionToStorage(string databaseFilePath)
        {
            string connectionString = SqLiteEntityConnectionStringBuilder.BuildSqLiteEntityConnectionString(databaseFilePath);

            using (var dbContext = new RiskeerEntities(connectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.VersionEntities.Load();
                }
                catch (Exception exception)
                {
                    string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file);
                    throw new StorageValidationException(message, exception);
                }
            }

            return connectionString;
        }

        /// <summary>
        /// Creates a configured instance of <see cref="StorageException"/> when writing to the storage file failed.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file that was attempted to connect with.</param>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private static StorageException CreateStorageWriterException(string databaseFilePath, string errorMessage, Exception innerException)
        {
            string message = new FileWriterErrorMessageBuilder(databaseFilePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        /// <summary>
        /// Creates a configured instance of <see cref="StorageException"/> when reading the storage file failed.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file that was attempted to connect with.</param>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private static StorageException CreateStorageReaderException(string databaseFilePath, string errorMessage, Exception innerException = null)
        {
            string message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        private class StagedProject
        {
            public StagedProject(RingtoetsProject projectModel, ProjectEntity projectEntity)
            {
                Model = projectModel;
                Entity = projectEntity;
            }

            public RingtoetsProject Model { get; }
            public ProjectEntity Entity { get; }
        }
    }
}