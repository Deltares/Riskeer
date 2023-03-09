// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic location configuration database files.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseImporter : FileImporterBase<HydraulicLocationConfigurationDatabase>
    {
        private const int numberOfSteps = 2;
        
        private readonly HydraulicBoundaryData hydraulicBoundaryData;
        private readonly List<IObservable> changedObservables = new List<IObservable>();
        private readonly IHydraulicLocationConfigurationDatabaseUpdateHandler updateHandler;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseImporter"/>.
        /// </summary>
        /// <param name="importTarget">The hydraulic location configuration database to import to.</param>
        /// <param name="updateHandler">The handler responsible for updating the hydraulic location configuration database.</param>
        /// <param name="hydraulicBoundaryData">The hydraulic boundary data the hydraulic location configuration database belongs to.</param>
        /// <param name="filePath">The file path of the hydraulic location configuration database to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicLocationConfigurationDatabaseImporter(HydraulicLocationConfigurationDatabase importTarget,
                                                              IHydraulicLocationConfigurationDatabaseUpdateHandler updateHandler,
                                                              HydraulicBoundaryData hydraulicBoundaryData,
                                                              string filePath)
            : base(filePath, importTarget)
        {
            if (updateHandler == null)
            {
                throw new ArgumentNullException(nameof(updateHandler));
            }

            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            this.updateHandler = updateHandler;
            this.hydraulicBoundaryData = hydraulicBoundaryData;
        }

        protected override bool OnImport()
        {
            InquireConfirmation();

            if (Canceled)
            {
                return false;
            }

            if (hydraulicBoundaryData.HydraulicBoundaryDatabases.Any()
                && Path.GetDirectoryName(hydraulicBoundaryData.HydraulicLocationConfigurationDatabase.FilePath) != Path.GetDirectoryName(FilePath))
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_Hlcd_not_in_same_folder_as_Hrd));
                return false;
            }

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult = ReadHydraulicLocationConfigurationDatabase();
            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = readHydraulicLocationConfigurationDatabaseResult.Items.Single();
            if (readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings != null
                && readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings.Count() != 1)
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_Invalid_number_of_ScenarioInformation_entries));
                return false;
            }

            bool usePreprocessorClosure = readHydraulicLocationConfigurationDatabase.ReadTracks.Any(rt => rt.UsePreprocessorClosure);
            if (usePreprocessorClosure && !File.Exists(HydraulicBoundaryDataHelper.GetPreprocessorClosureFilePath(FilePath)))
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicBoundaryDataImporter_PreprocessorClosure_sqlite_Not_Found));
                return false;
            }

            IEnumerable<long> locationIds = hydraulicBoundaryData.Locations.Select(l => l.Id);
            long[] intersect = locationIds.Intersect(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocations.Select(l => l.HlcdLocationId))
                                          .ToArray();

            if (intersect.Length != locationIds.Count())
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_Invalid_locationIds));
                return false;
            }

            SetReadHydraulicLocationConfigurationSettingsToDataModel(
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationSettings?.Single(),
                usePreprocessorClosure);

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.HydraulicLocationConfigurationDatabaseImporter_ProgressText_Import_canceled_No_data_changed);
        }

        protected override void DoPostImportUpdates()
        {
            base.DoPostImportUpdates();

            foreach (IObservable changedObservable in changedObservables)
            {
                changedObservable.NotifyObservers();
            }
        }

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase()
        {
            NotifyProgress(Resources.HydraulicBoundaryDataImporter_ProgressText_Reading_Hlcd_file, 1, numberOfSteps);
            try
            {
                using (var reader = new HydraulicLocationConfigurationDatabaseReader(FilePath))
                {
                    return new ReadResult<ReadHydraulicLocationConfigurationDatabase>(false)
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
                return HandleCriticalFileReadError<ReadHydraulicLocationConfigurationDatabase>(e);
            }
        }

        private void InquireConfirmation()
        {
            if (!updateHandler.InquireConfirmation())
            {
                Cancel();
            }
        }

        private void SetReadHydraulicLocationConfigurationSettingsToDataModel(ReadHydraulicLocationConfigurationSettings readHydraulicLocationConfigurationSettings,
                                                                              bool usePrepocessorClosure)
        {
            NotifyProgress(RiskeerCommonIOResources.Importer_ProgressText_Adding_imported_data_to_AssessmentSection, 2, numberOfSteps);
            changedObservables.AddRange(updateHandler.Update(hydraulicBoundaryData, readHydraulicLocationConfigurationSettings, usePrepocessorClosure, FilePath));
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(Exception e)
        {
            string errorMessage = string.Format(Resources.HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicLocationConfigurationDatabase_imported,
                                                e.Message);
            Log.Error(errorMessage);
            return new ReadResult<T>(true);
        }

        private static string BuildErrorMessage(string filePath, string message)
        {
            return new FileReaderErrorMessageBuilder(filePath).Build(
                string.Format(Resources.HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicLocationConfigurationDatabase_imported,
                              message));
        }
    }
}