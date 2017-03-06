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
using System.IO;
using System.Windows.Forms;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Migration.Properties;
using Core.Common.Gui;
using Core.Common.Utils;
using log4net;
using Migration.Scripts.Data.Exceptions;
using Ringtoets.Common.Utils;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using MigrationCoreStorageResources = Migration.Core.Storage.Properties.Resources;

namespace Application.Ringtoets.Migration
{
    /// <summary>
    /// A GUI implementation to migrate a Ringtoets database file to a newer version.
    /// </summary>
    public class RingtoetsProjectMigrator
    {
        private static readonly string currentProjectVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

        private readonly ILog log = LogManager.GetLogger(typeof(RingtoetsProjectMigrator));
        private readonly RingtoetsSqLiteDatabaseFileMigrator fileMigrator;
        private readonly IInquiryHelper inquiryHelper;
        private readonly FileFilterGenerator fileFilter;

        /// <summary>
        /// Instantiates a <see cref="RingtoetsProjectMigrator"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring the data.</param>
        public RingtoetsProjectMigrator(IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.inquiryHelper = inquiryHelper;
            fileMigrator = new RingtoetsSqLiteDatabaseFileMigrator();
            fileFilter = new FileFilterGenerator(Resources.RingtoetsProject_FileExtension,
                                                 Resources.RingtoetsProject_FileExtension);
        }

        /// <summary>
        /// Indicates if the project <paramref name="sourceFilePath"/> needs to be 
        /// updated to the newest version.
        /// </summary>
        /// <param name="sourceFilePath">The file path of the project which needs to be checked.</param>
        /// <returns><c>true</c> if the file needs to be migrated, <c>false</c> if:</returns>
        /// <list type="bullet">
        /// <item>The file does not need to be migrated.</item>
        /// <item>The file is not supported for the migration.</item>
        /// </list>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceFilePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="sourceFilePath"/> is an invalid file path.</exception>
        public bool ShouldMigrate(string sourceFilePath)
        {
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            ValidateProjectPath(sourceFilePath);

            var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
            string version = versionedFile.GetVersion();

            if (version.Equals(currentProjectVersion))
            {
                return false;
            }

            bool isVersionSupported = fileMigrator.IsVersionSupported(version);
            if (!isVersionSupported)
            {
                string errorMessage = string.Format(MigrationCoreStorageResources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                    version, currentProjectVersion);
                log.Error(errorMessage);
            }
            return isVersionSupported;
        }

        /// <summary>
        /// Migrates an outdated project file from <paramref name="sourceFilePath"/>
        /// to the newest project version version at a user defined target filepath.
        /// </summary>
        /// <param name="sourceFilePath">The project file which needs to be migrated.</param>
        /// <returns>A filepath to the updated project file. <c>null</c> if:
        /// <list type="bullet">
        /// <item>The user cancelled.</item>
        /// <item>The migration failed.</item>
        /// </list></returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="sourceFilePath"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="sourceFilePath"/> is an invalid file path.</exception>
        public string Migrate(string sourceFilePath)
        {
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            ValidateProjectPath(sourceFilePath);

            string query = Resources.RingtoetsProjectMigrator_Migrate_Outdated_project_file_update_to_current_version_inquire;
            if (inquiryHelper.InquireContinuation(query))
            {
                string suggestedFileName = GetSuggestedFileName(sourceFilePath);
                string targetLocation = GetTargetFileLocation(suggestedFileName);
                if (!string.IsNullOrEmpty(targetLocation))
                {
                    return MigrateToTargetLocation(sourceFilePath, targetLocation);
                }

                GenerateMigrationCancelledLogMessage(sourceFilePath);
            }

            GenerateMigrationCancelledLogMessage(sourceFilePath);

            return null;
        }

        private string MigrateToTargetLocation(string sourceFilePath, string targetLocation)
        {
            try
            {
                var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
                fileMigrator.Migrate(versionedFile, currentProjectVersion, targetLocation);

                string message = string.Format(Resources.RingtoetsProjectMigrator_MigrateToTargetLocation_Outdated_projectfile_0_succesfully_updated_to_target_filepath_1_version_2_,
                                               sourceFilePath, targetLocation, currentProjectVersion);
                log.Info(message);

                return targetLocation;
            }
            catch (CriticalMigrationException e)
            {
                string errorMessage = string.Format(Resources.RingtoetsProjectMigrator_MigrateToTargetLocation_Updating_outdated_projectfile_0_failed_with_exception_1_,
                                                    sourceFilePath, e.Message);
                log.Error(errorMessage, e);
                return null;
            }
        }

        private static string GetSuggestedFileName(string sourceFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string versionSuffix = currentProjectVersion.Replace(".", "-");
            string suggestedFileName = $"{fileName}_{versionSuffix}";

            return suggestedFileName;
        }

        private void GenerateMigrationCancelledLogMessage(string sourceFilePath)
        {
            string warningMessage = string.Format(Resources.RingtoetsProjectMigrator_GenerateMigrationCancelledLogMessage_Updating_projectfile_0_was_cancelled, sourceFilePath);
            log.Warn(warningMessage);
        }

        private string GetTargetFileLocation(string suggestedFileName)
        {
            string filePath = null;
            using (var dialog = new SaveFileDialog
            {
                Title = CoreCommonGuiResources.SaveFileDialog_Title,
                Filter = fileFilter.Filter,
                FileName = suggestedFileName
            })
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    filePath = dialog.FileName;
                }
            }
            return filePath;
        }

        private static void ValidateProjectPath(string sourceFilePath)
        {
            if (!IOUtils.IsValidFilePath(sourceFilePath))
            {
                throw new ArgumentException(Resources.RingtoetsProjectMigrator_ValidateProjectPath_Source_filepath_must_be_a_valid_path,
                                            nameof(sourceFilePath));
            }
        }
    }
}