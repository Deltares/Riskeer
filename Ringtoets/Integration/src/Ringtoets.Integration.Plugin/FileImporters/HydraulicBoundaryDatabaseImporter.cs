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
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabaseContext;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabaseContext;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin.Properties;

namespace Ringtoets.Integration.Plugin.FileImporters
{
    /// <summary>
    /// Imports locations read from an Hydraulic boundary .sqlite file (SqlLite database file) to a 
    /// collection of <see cref="HydraulicBoundaryLocation"/> in a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryDatabaseImporter));
        private string hydraulicBoundaryDatabaseFilePath;

        private HydraulicBoundarySqLiteDatabaseReader hydraulicBoundaryDatabaseReader;
        private HydraulicLocationConfigurationSqLiteDatabaseReader hydraulicLocationConfigurationDatabaseReader;

        /// <summary>
        /// Validates the file and opens a connection.
        /// </summary>
        /// <param name="filePath">The path to the file to read.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: <list type="bullet">
        /// <item>The given file at <paramref name="filePath"/> cannot be read.</item>
        /// <item>The file 'HLCD.sqlite' in the same folder as <paramref name="filePath"/> cannot be read.</item></list></exception>
        public void ValidateAndConnectTo(string filePath)
        {
            hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(filePath);
            hydraulicBoundaryDatabaseFilePath = filePath;
            var fileDirectory = Path.GetDirectoryName(filePath);
            if (fileDirectory == null)
            {
                throw new ArgumentNullException("filePath");
            }
            var hlcdFilePath = Path.Combine(fileDirectory, "hlcd.sqlite");
            if (!File.Exists(hlcdFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(filePath).Build(Resources.HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found);
                throw new CriticalFileReadException(message);
            }
            hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationSqLiteDatabaseReader(hlcdFilePath);
        }

        /// <summary>
        /// Gets the version of the database.
        /// </summary>
        /// <returns>The database version.</returns>
        public string GetHydraulicBoundaryDatabaseVersion()
        {
            return hydraulicBoundaryDatabaseReader.GetVersion();
        }

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabase"/>, based upon the data read from 
        /// the hydraulic boundary database file, and saved into <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="targetItem"><see cref="HydraulicBoundaryDatabaseContext"/> to set the newly 
        /// created <see cref="HydraulicBoundaryDatabase"/>.</param>
        /// <returns><c>True</c> if the import was successful, <c>false</c> otherwise.</returns>
        public bool Import(HydraulicBoundaryDatabaseContext targetItem)
        {
            if (hydraulicBoundaryDatabaseReader == null)
            {
                throw new InvalidOperationException(Resources.HydraulicBoundaryDatabaseImporter_File_not_opened);
            }

            var importResult = GetHydraulicBoundaryDatabase(hydraulicBoundaryDatabaseFilePath);

            if (importResult == null)
            {
                return false;
            }

            AddImportedDataToModel(targetItem.Parent, importResult);
            log.Info(Resources.HydraulicBoundaryDatabaseImporter_Import_All_hydraulic_locations_read);
            return true;
        }

        public void Dispose()
        {
            if (hydraulicBoundaryDatabaseReader != null)
            {
                hydraulicBoundaryDatabaseReader.Dispose();
                hydraulicBoundaryDatabaseReader = null;
            }
            if (hydraulicLocationConfigurationDatabaseReader != null)
            {
                hydraulicLocationConfigurationDatabaseReader.Dispose();
                hydraulicLocationConfigurationDatabaseReader = null;
            }
        }

        private void HandleException(Exception e)
        {
            var message = string.Format(Resources.HydraulicBoundaryDatabaseImporter_ErrorMessage_0_file_skipped, e.Message);
            log.Error(message);
        }

        private HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase(string path)
        {
            // Get region
            var regionId = GetRegionId();
            if (regionId == 0)
            {
                return null;
            }

            try
            {
                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = path,
                    Version = hydraulicBoundaryDatabaseReader.GetVersion()
                };

                // Locations directory of HLCD location ids and HRD location ids
                var locationidsDictionary = hydraulicLocationConfigurationDatabaseReader.GetLocationsIdByRegionId(regionId);

                // Prepare query to fetch hrd locations
                hydraulicBoundaryDatabaseReader.PrepareReadLocation();
                while (hydraulicBoundaryDatabaseReader.HasNext)
                {
                    HrdLocation hrdLocation = hydraulicBoundaryDatabaseReader.ReadLocation();

                    long locationId;
                    locationidsDictionary.TryGetValue(hrdLocation.HrdLocationId, out locationId);

                    var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(
                        locationId,
                        hrdLocation.Name,
                        hrdLocation.LocationX,
                        hrdLocation.LocationY);
                    hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);
                }
                return hydraulicBoundaryDatabase;
            }
            catch (Exception e)
            {
                if (e is LineParseException || e is CriticalFileReadException)
                {
                    HandleException(e);
                    return null;
                }
                throw;
            }
        }

        private long GetRegionId()
        {
            try
            {
                return hydraulicBoundaryDatabaseReader.GetRegionId();
            }
            catch (Exception e)
            {
                if (e is LineParseException || e is CriticalFileReadException)
                {
                    HandleException(e);
                    return 0;
                }
                throw;
            }
        }

        private static void AddImportedDataToModel(AssessmentSectionBase assessmentSection, HydraulicBoundaryDatabase importedData)
        {
            assessmentSection.HydraulicBoundaryDatabase = importedData;
        }
    }
}