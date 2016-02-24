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
using System.Collections.Generic;
using System.Drawing;
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
    /// collection of <see cref="HydraulicBoundaryLocation"/>.
    /// </summary>
    public class HydraulicBoundaryLocationsImporter : FileImporterBase, IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsImporter));

        private HydraulicBoundarySqLiteDatabaseReader hydraulicBoundaryDatabaseReader;

        public override string Name
        {
            get
            {
                return RingtoetsHydraRingFormsResources.HydraulicBoundaryLocationsCollection_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsCommonFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return RingtoetsCommonFormsResources.DatabaseIcon;
            }
        }

        public override Type SupportedItemType
        {
            get
            {
                return typeof(ICollection<HydraulicBoundaryLocation>);
            }
        }

        public override string FileFilter
        {
            get
            {
                return string.Format("{0} (*.sqlite)|*.sqlite", HydraRingResources.SelectHydraulicBoundaryDatabaseFile_FilterName);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

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

        public override bool Import(object targetItem, string filePath)
        {
            if (hydraulicBoundaryDatabaseReader == null)
            {
                throw new InvalidOperationException(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_The_file_is_not_opened);
            }

            var importTarget = (HydraulicBoundaryDatabaseContext) targetItem;

            var importResult = ReadHydraulicBoundaryLocations(filePath);

            if (ImportIsCancelled)
            {
                log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_cancelled);
                ImportIsCancelled = false;

                return false;
            }

            if (importResult == null)
            {
                return false;
            }

            AddImportedDataToModel(importTarget.Parent, importResult);
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

        private HydraulicBoundaryDatabase ReadHydraulicBoundaryLocations(string path)
        {
            NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_ReadHydraulicBoundaryLocations, 1, 1);

            try
            {
                return GetHydraulicBoundaryDatabase(path);
            }
            catch (LineParseException e)
            {
                HandleException(e);
            }
            return null;
        }

        private void HandleException(Exception e)
        {
            var message = string.Format(ApplicationResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, e.Message);
            log.Error(message);
        }

        private HydraulicBoundaryDatabase GetHydraulicBoundaryDatabase(string path)
        {
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase()
            {
                FilePath = path,
                Version = hydraulicBoundaryDatabaseReader.GetVersion()
            };
            var totalNumberOfSteps = hydraulicBoundaryDatabaseReader.GetLocationCount();
            var currentStep = 1;

            hydraulicBoundaryDatabaseReader.PrepareReadLocation();
            while (hydraulicBoundaryDatabaseReader.HasNext)
            {
                if (ImportIsCancelled)
                {
                    return null;
                }
                NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_GetHydraulicBoundaryLocationReadResult, currentStep++, totalNumberOfSteps);
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

        private void AddImportedDataToModel(AssessmentSectionBase assessmentSection, HydraulicBoundaryDatabase importedData)
        {
            assessmentSection.HydraulicBoundaryDatabase = importedData;
        }
    }
}