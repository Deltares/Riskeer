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
using System.Collections.ObjectModel;
using System.Drawing;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using log4net;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsHydraRingFormsResources = Ringtoets.HydraRing.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.HydraRing.Plugin.Properties.Resources;
using HydraRingResources = Ringtoets.HydraRing.Forms.Properties.Resources;


namespace Ringtoets.HydraRing.Plugin
{
    /// <summary>
    /// Imports Hydraulic boundary .sqlite files (SqlLite database files).
    /// </summary>
    public class HydraulicBoundaryLocationsImporter : FileImporterBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsImporter));

        /// <summary>
        /// Gets the version of the used Hydraulic Boundary Database.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the name of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public override string Name
        {
            get
            {
                return RingtoetsHydraRingFormsResources.HydraulicBoundaryLocationsCollection_DisplayName;
            }
        }

        /// <summary>
        /// Gets the category of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public override string Category
        {
            get
            {
                return RingtoetsCommonFormsResources.Ringtoets_Category;
            }
        }

        /// <summary>
        /// Gets the image of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        /// <remarks>This image can be used in selection and/or progress dialogs.</remarks>
        public override Bitmap Image
        {
            get
            {
                return RingtoetsCommonFormsResources.DatabaseIcon;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the item supported by the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public override Type SupportedItemType
        {
            get
            {
                return typeof(HydraulicBoundaryLocation);
            }
        }

        /// <summary>
        /// Gets the file filter of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public override string FileFilter
        {
            get
            {
                return string.Format("{0} (*.sqlite)|*.sqlite", HydraRingResources.SelectDatabaseFile_FilterName);
            }
        }

        /// <summary>
        /// Sets the action to perform when progress has changed.
        /// </summary>
        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        /// <summary>
        /// Validates the file at <paramref name="filePath"/> and sets the version.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        public void ValidateFile(string filePath)
        {
            try
            {
                using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(filePath))
                {
                    Version = hydraulicBoundaryDatabaseReader.Version;
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(e);
            }
        }

        /// <summary>
        /// This method imports the data to an item from a file at the given location.
        /// </summary>
        /// <param name="targetItem">The item to perform the import on.</param>
        /// <param name="filePath">The path of the file to import the data from.</param>
        /// <returns><c>True</c> if the import was successful. <c>False</c> otherwise.</returns>
        public override bool Import(object targetItem, string filePath)
        {
            var importResult = ReadHydraulicBoundaryLocations(filePath);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!ImportIsCancelled)
                {
                    AddImportedDataToModel(targetItem, importResult);
                    log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_Import_successful);
                    return true;
                }

                log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_cancelled);
                ImportIsCancelled = false;
            }

            return false;
        }

        private ReadResult<HydraulicBoundaryLocation> ReadHydraulicBoundaryLocations(string path)
        {
            NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_ReadHydraulicBoundaryLocations, 1, 1);

            try
            {
                using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(path))
                {
                    return GetHydraulicBoundaryLocationReadResult(path, hydraulicBoundaryDatabaseReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(e);
            }
            return new ReadResult<HydraulicBoundaryLocation>(true);
        }

        private void HandleException(Exception e)
        {
            var message = string.Format(ApplicationResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, e.Message);
            log.Error(message);
        }

        private ReadResult<HydraulicBoundaryLocation> GetHydraulicBoundaryLocationReadResult(string path, HydraulicBoundarySqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader)
        {
            var totalNumberOfSteps = hydraulicBoundarySqLiteDatabaseReader.Count;
            var currentStep = 1;

            var locations = new Collection<HydraulicBoundaryLocation>();
            while (hydraulicBoundarySqLiteDatabaseReader.HasNext)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<HydraulicBoundaryLocation>(false);
                }
                try
                {
                    NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_GetHydraulicBoundaryLocationReadResult, currentStep++, totalNumberOfSteps);
                    locations.Add(hydraulicBoundarySqLiteDatabaseReader.ReadLocation());
                }
                catch (CriticalFileReadException e)
                {
                    var message = string.Format(ApplicationResources.HydraulicBoundaryLocationsImporter_CriticalErrorMessage_0_File_Skipped, path);
                    log.Error(message, e);
                    return new ReadResult<HydraulicBoundaryLocation>(true);
                }
            }
            return new ReadResult<HydraulicBoundaryLocation>(false)
            {
                ImportedItems = locations
            };
        }

        private void AddImportedDataToModel(object target, ReadResult<HydraulicBoundaryLocation> imported)
        {
            var targetCollection = (ICollection<HydraulicBoundaryLocation>) target;

            int totalCount = imported.ImportedItems.Count;
            NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_Adding_imported_data_to_model, totalCount, totalCount);

            foreach (var item in imported.ImportedItems)
            {
                targetCollection.Add(item);
            }
        }
    }
}