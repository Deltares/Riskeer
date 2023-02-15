﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic boundary data files.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : FileImporterBase<HydraulicBoundaryData>
    {
        private const int numberOfSteps = 4;
        private readonly List<IObservable> changedObservables = new List<IObservable>();
        private readonly IHydraulicBoundaryDataUpdateHandler updateHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseImporter"/>.
        /// </summary>
        /// <param name="importTarget">The hydraulic boundary data to import to.</param>
        /// <param name="updateHandler">The object responsible for updating the <see cref="HydraulicBoundaryData"/>.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseImporter(HydraulicBoundaryData importTarget, IHydraulicBoundaryDataUpdateHandler updateHandler,
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

            InquireConfirmation(readHydraulicBoundaryDatabase);

            if (Canceled)
            {
                return false;
            }

            string hlcdFilePath = GetHlcdFilePath();

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult = ReadHydraulicLocationConfigurationDatabase(
                hlcdFilePath, readHydraulicBoundaryDatabase.TrackId);

            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = readHydraulicLocationConfigurationDatabaseResult.Items.Single();
            IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> hydraulicLocationConfigurationDatabaseSettings =
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings;
            if (hydraulicLocationConfigurationDatabaseSettings != null && hydraulicLocationConfigurationDatabaseSettings.Count() != 1)
            {
                Log.Error(BuildErrorMessage(hlcdFilePath, Resources.HydraulicBoundaryDatabaseImporter_HLCD_Invalid_number_of_ScenarioInformation_entries));
                return false;
            }

            if (readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure
                && !File.Exists(HydraulicBoundaryDataHelper.GetPreprocessorClosureFilePath(hlcdFilePath)))
            {
                Log.Error(BuildErrorMessage(hlcdFilePath, Resources.HydraulicBoundaryDatabaseImporter_PreprocessorClosure_sqlite_Not_Found));
                return false;
            }

            ReadResult<IEnumerable<long>> readExcludedLocationsResult = ReadExcludedLocations();

            if (readExcludedLocationsResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            AddHydraulicBoundaryDatabaseToDataModel(readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                    readExcludedLocationsResult.Items.Single());

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed);
        }

        protected override void DoPostImportUpdates()
        {
            updateHandler.DoPostUpdateActions();

            base.DoPostImportUpdates();

            foreach (IObservable changedObservable in changedObservables)
            {
                changedObservable.NotifyObservers();
            }
        }

        private void InquireConfirmation(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase)
        {
            if (updateHandler.IsConfirmationRequired(ImportTarget, readHydraulicBoundaryDatabase)
                && !updateHandler.InquireConfirmation())
            {
                Cancel();
            }
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

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase(string hlcdFilePath, long trackId)
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file, 2, numberOfSteps);
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

        private ReadResult<IEnumerable<long>> ReadExcludedLocations()
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_settings_file, 3, numberOfSteps);
            string hbsdFilePath = HydraulicBoundaryDataHelper.GetHydraulicBoundarySettingsDatabase(FilePath);
            try
            {
                using (var reader = new HydraRingSettingsDatabaseReader(hbsdFilePath))
                {
                    return ReadExcludedLocations(reader);
                }
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalFileReadError<IEnumerable<long>>(
                    string.Format(Resources.HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file_0_, e.Message));
            }
        }

        private ReadResult<IEnumerable<long>> ReadExcludedLocations(HydraRingSettingsDatabaseReader reader)
        {
            try
            {
                return new ReadResult<IEnumerable<long>>(false)
                {
                    Items = new[]
                    {
                        reader.ReadExcludedLocations().ToArray()
                    }
                };
            }
            catch (CriticalFileReadException e)
            {
                return HandleCriticalFileReadError<IEnumerable<long>>(e.Message);
            }
        }

        private void AddHydraulicBoundaryDatabaseToDataModel(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                                             ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                             IEnumerable<long> excludedLocationIds)
        {
            NotifyProgress(RiskeerCommonIOResources.Importer_ProgressText_Adding_imported_data_to_AssessmentSection, 4, numberOfSteps);
            changedObservables.AddRange(updateHandler.Update(ImportTarget, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                             excludedLocationIds, FilePath, GetHlcdFilePath()));
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

        private string GetHlcdFilePath()
        {
            return Path.Combine(Path.GetDirectoryName(FilePath), "hlcd.sqlite");
        }

        private string BuildErrorMessage(string message)
        {
            return BuildErrorMessage(FilePath, message);
        }

        private static string BuildErrorMessage(string filePath, string message)
        {
            return new FileReaderErrorMessageBuilder(filePath).Build(
                string.Format(Resources.HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported,
                              message));
        }
    }
}