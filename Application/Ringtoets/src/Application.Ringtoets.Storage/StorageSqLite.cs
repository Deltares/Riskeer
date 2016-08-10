﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.Integration.Data;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : IStoreProject
    {
        private const int currentDatabaseVersion = 1;
        private static readonly ILog log = LogManager.GetLogger(typeof(StorageSqLite));

        private string connectionString;
        private RingtoetsProject stagedProject;
        private ProjectEntity stagedProjectEntity;

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
                return stagedProjectEntity != null;
            }
        }

        public void StageProject(IProject project)
        {
            var ringtoetsProject = project as RingtoetsProject;
            if (ringtoetsProject == null)
            {
                throw new ArgumentNullException("project");
            }
            stagedProject = ringtoetsProject;

            var registry = new PersistenceRegistry();
            stagedProjectEntity = ringtoetsProject.Create(registry);
        }

        public void UnstageProject()
        {
            stagedProject = null;
            stagedProjectEntity = null;
        }

        public void SaveProjectAs(string databaseFilePath)
        {
            if (!HasStagedProject)
            {
                throw new InvalidOperationException("Call 'StageProject(IProject)' first before calling this method.");
            }

            try
            {
                BackedUpFileWriter writer = new BackedUpFileWriter(databaseFilePath);
                writer.Perform(() =>
                {
                    SetConnectionToNewFile(databaseFilePath);
                    SaveProjectInDatabase(databaseFilePath);
                });
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
            SetConnectionToExistingFile(databaseFilePath);
            try
            {
                using (var dbContext = new RingtoetsEntities(connectionString))
                {
                    ValidateDatabaseVersion(dbContext, databaseFilePath);

                    ProjectEntity projectEntity;

                    try
                    {
                        projectEntity = dbContext.ProjectEntities.Single();
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
                    }

                    var project = projectEntity.Read(new ReadConversionCollector());

                    project.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                    return project;
                }
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

        public void CloseProject()
        {
            connectionString = null;
        }

        public bool HasStagedProjectChanges()
        {
            if (!HasStagedProject)
            {
                throw new InvalidOperationException("Call 'StageProject(IProject)' first before calling this method.");
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return true;
            }

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                byte[] originalHash = dbContext.VersionEntities.Select(v => v.FingerPrint).First();
                byte[] hash = FingerprintHelper.Get(stagedProjectEntity);
                return !FingerprintHelper.AreEqual(originalHash, hash);
            }
        }

        private void ValidateDatabaseVersion(RingtoetsEntities ringtoetsEntities, string databaseFilePath)
        {
            try
            {
                long databaseVersion = ringtoetsEntities.VersionEntities.Select(v => v.Version).Single();
                if (databaseVersion <= 0)
                {
                    string m = string.Format(Resources.StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_is_invalid,
                                             databaseVersion);
                    var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(m);
                    throw new StorageValidationException(message);
                }

                if (databaseVersion > currentDatabaseVersion)
                {
                    string m = String.Format(Resources.StorageSqLite_ValidateDatabaseVersion_DatabaseVersion_0_higher_then_current_DatabaseVersion_1_,
                                             databaseVersion, currentDatabaseVersion);
                    var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(m);
                    throw new StorageValidationException(message);
                }
            }
            catch (InvalidOperationException e)
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(Resources.StorageSqLite_ValidateDatabaseVersion_Database_must_have_one_VersionEntity_row);
                throw new StorageValidationException(message, e);
            }
        }

        private void SaveProjectInDatabase(string databaseFilePath)
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var registry = new PersistenceRegistry();
                    dbContext.VersionEntities.Add(new VersionEntity
                    {
                        Version = currentDatabaseVersion,
                        Timestamp = DateTime.Now,
                        FingerPrint = FingerprintHelper.Get(stagedProjectEntity)
                    });
                    dbContext.ProjectEntities.Add(stagedProjectEntity);
                    dbContext.SaveChanges();
                    registry.TransferIds();

                    stagedProject.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(databaseFilePath, Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    if (exception is InvalidOperationException || exception is NotSupportedException)
                    {
                        throw CreateStorageWriterException(databaseFilePath, Resources.Error_During_Connection, exception);
                    }
                    throw;
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
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> is invalid</item>
        /// <item><paramref name="databaseFilePath"/> points to an existing file</item>
        /// </list></exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item>executing <c>DatabaseStructure</c> script failed</item>
        /// </list>
        /// </exception>
        private void SetConnectionToNewFile(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            StorageSqliteCreator.CreateDatabaseStructure(databaseFilePath);
            SetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Establishes a connection to an existing <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to connect to.</param>
        /// <exception cref="CouldNotConnectException">No file exists at <paramref name="databaseFilePath"/>.</exception>
        private void SetConnectionToFile(string databaseFilePath)
        {
            if (!File.Exists(databaseFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilsResources.Error_File_does_not_exist);
                throw new CouldNotConnectException(message);
            }

            SetConnectionToStorage(databaseFilePath);
        }

        /// <summary>
        /// Sets the connection to the Ringtoets database.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file, which is used for creating exceptions.</param>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private void SetConnectionToStorage(string databaseFilePath)
        {
            connectionString = SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(databaseFilePath);

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.VersionEntities.Load();
                }
                catch (Exception exception)
                {
                    var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file);
                    throw new StorageValidationException(message, exception);
                }
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when writing to the storage file failed.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file that was attempted to connect with.</param>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageWriterException(string databaseFilePath, string errorMessage, Exception innerException)
        {
            var message = new FileWriterErrorMessageBuilder(databaseFilePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when reading the storage file failed.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file that was attempted to connect with.</param>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageReaderException(string databaseFilePath, string errorMessage, Exception innerException = null)
        {
            var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }
    }
}