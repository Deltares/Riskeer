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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.HydraRing.IO;
using Ringtoets.Integration.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsHydraRingFormsResources = Ringtoets.HydraRing.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.HydraRing.Plugin.Properties.Resources;
using HydraRingResources = Ringtoets.HydraRing.Forms.Properties.Resources;

namespace Ringtoets.HydraRing.Plugin
{
    /// <summary>
    /// Imports locations read from an Hydraulic boundary .sqlite file (SqlLite database file) to a 
    /// collection of <see cref="HydraulicBoundaryLocation"/> in a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryDatabaseImporter));

        private HydraulicBoundarySqLiteDatabaseReader hydraulicBoundaryDatabaseReader;

        /// <summary>
        /// Validates the file and opens a connection.
        /// </summary>
        /// <param name="filePath">The path to the file to read.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the given file at <paramref name="filePath"/> cannot be read.</exception>
        public void ValidateAndConnectTo(string filePath)
        {
            hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(filePath);
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
        /// Based upon the data read from the hydraulic boundary database file located at 
        /// <paramref name="filePath"/>, a new instance of <see cref="HydraulicBoundaryDatabase"/>, 
        /// and saved into <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="targetItem"><see cref="HydraulicBoundaryDatabaseContext"/> to set the newly 
        /// created <see cref="HydraulicBoundaryDatabase"/>.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to open.</param>
        /// <returns><c>True</c> if the import was successful, <c>false</c> otherwise.</returns>
        public bool Import(HydraulicBoundaryDatabaseContext targetItem, string filePath)
        {
            if (hydraulicBoundaryDatabaseReader == null)
            {
                throw new InvalidOperationException(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_The_file_is_not_opened);
            }

            var importResult = GetHydraulicBoundaryDatabase(filePath);

            if (importResult == null)
            {
                return false;
            }

            AddImportedDataToModel(targetItem.Parent, importResult);
            log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_Import_successful);
            return true;
        }

        public void Dispose()
        {
            if (hydraulicBoundaryDatabaseReader != null)
            {
                hydraulicBoundaryDatabaseReader.Dispose();
                hydraulicBoundaryDatabaseReader = null;
            }
        }

        private void HandleException(Exception e)
        {
            var message = string.Format(ApplicationResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, e.Message);
            log.Error(message);
        }

        private HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase(string path)
        {
            try
            {
                var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = path,
                    Version = hydraulicBoundaryDatabaseReader.GetVersion()
                };

                hydraulicBoundaryDatabaseReader.PrepareReadLocation();
                while (hydraulicBoundaryDatabaseReader.HasNext)
                {
                    try
                    {
                        hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryDatabaseReader.ReadLocation());
                    }
                    catch (CriticalFileReadException e)
                    {
                        var message = string.Format(ApplicationResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, path);
                        log.Error(message, e);
                        return null;
                    }
                }
                return hydraulicBoundaryDatabase;
            }
            catch (LineParseException e)
            {
                HandleException(e);
            }

            return null;
        }

        private static void AddImportedDataToModel(AssessmentSectionBase assessmentSection, HydraulicBoundaryDatabase importedData)
        {
            assessmentSection.HydraulicBoundaryDatabase = importedData;
        }
    }
}