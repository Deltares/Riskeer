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
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic boundary database files and corresponding configuration files.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : FileImporterBase<HydraulicBoundaryDatabase>
    {
        private int numberOfSteps = 3;
        private readonly IHydraulicBoundaryDatabaseUpdateHandler updateHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="updateHandler">The object responsible for updating the <see cref="HydraulicBoundaryDatabase"/>.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseImporter(HydraulicBoundaryDatabase importTarget, IHydraulicBoundaryDatabaseUpdateHandler updateHandler,
                                                 string filePath)
            : base(filePath, importTarget)
        {
            if (updateHandler == null)
            {
                throw new ArgumentNullException(nameof(updateHandler));
            }

            this.updateHandler = updateHandler;
        }

        protected override bool OnImport()
        {
            ReadResult<ReadHydraulicBoundaryDatabase> readHydraulicBoundaryDatabaseResult = ReadHydraulicBoundaryDatabase();
            if (readHydraulicBoundaryDatabaseResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = readHydraulicBoundaryDatabaseResult.Items.Single();

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult = ReadHydraulicLocationConfigurationDatabase(
                readHydraulicBoundaryDatabase.TrackId);

            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            if (!OpenSettingsFileToValidate() || Canceled)
            {
                return false;
            }

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed);
        }

        private ReadResult<ReadHydraulicBoundaryDatabase> ReadHydraulicBoundaryDatabase()
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_file, 1, numberOfSteps);
            try
            {
                using (var reader = new HydraulicBoundaryDatabaseReader(FilePath))
                {
                    return new ReadResult<ReadHydraulicBoundaryDatabase>(false)
                    {
                        Items = new[]
                        {
                            reader.Read()
                        }
                    };
                }
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                return HandleCriticalFileReadError<ReadHydraulicBoundaryDatabase>(e);
            }
        }

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase(long trackId)
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file, 2, numberOfSteps);
            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(FilePath), "hlcd.sqlite");
            try
            {
                using (var reader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath))
                {
                    return ReadHydraulicLocationConfigurationDatabase(trackId, reader);
                }
            }
            catch (CriticalFileReadException)
            {
                return HandleCriticalFileReadError<ReadHydraulicLocationConfigurationDatabase>(Resources.HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found);
            }
        }

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase(long trackId, HydraulicLocationConfigurationDatabaseReader reader)
        {
            try
            {
                return new ReadResult<ReadHydraulicLocationConfigurationDatabase>(false)
                {
                    Items = new[]
                    {
                        reader.Read(trackId)
                    }
                };
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                return HandleCriticalFileReadError<ReadHydraulicLocationConfigurationDatabase>(e);
            }
        }

        private bool OpenSettingsFileToValidate()
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_settings_file, 3, numberOfSteps);
            string settingsFilePath = HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(FilePath);
            try
            {
                using (new HydraRingSettingsDatabaseReader(settingsFilePath)) { }
                return true;
            }
            catch (CriticalFileReadException e)
            {
                HandleCriticalFileReadError(string.Format(Common.IO.Properties.Resources.HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file_0_, e.Message));
                return false;
            }
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(Exception e)
        {
            string errorMessage = string.Format(Resources.HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported,
                                                e.Message);
            Log.Error(errorMessage);
            return new ReadResult<T>(true);
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(string message)
        {
            HandleCriticalFileReadError(message);
            return new ReadResult<T>(true);
        }

        private void HandleCriticalFileReadError(string message)
        {
            string errorMessage = BuildErrorMessage(message);
            Log.Error(errorMessage);
        }

        private string BuildErrorMessage(string message)
        {
            return new FileReaderErrorMessageBuilder(FilePath).Build(
                string.Format(Resources.HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported,
                              message));
        }
    }
}