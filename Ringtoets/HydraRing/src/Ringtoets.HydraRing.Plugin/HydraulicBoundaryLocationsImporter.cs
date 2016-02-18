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

namespace Ringtoets.HydraRing.Plugin
{
    public class HydraulicBoundaryLocationsImporter : IFileImporter
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsImporter));
        private bool shouldCancel;

        /// <summary>
        /// Gets the version of the used Hydraulic Boundary Database.
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the name of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return RingtoetsHydraRingFormsResources.HydraulicBoundaryLocationsCollection_DisplayName;
            }
        }

        /// <summary>
        /// Gets the category of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public string Category
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
        public Bitmap Image
        {
            get
            {
                return RingtoetsCommonFormsResources.DatabaseIcon;
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the item supported by the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary
        public Type SupportedItemType
        {
            get
            {
                return typeof(HydraulicBoundaryLocation);
            }
        }

        /// <summary>
        /// Gets the file filter of the <see cref="HydraulicBoundaryLocationsImporter"/>.
        /// </summary>
        public string FileFilter
        {
            get
            {
                return string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsCommonFormsResources.SelectDatabaseFile_FilterName);
            }
        }

        /// <summary>
        /// Sets the action to perform when progress has changed.
        /// </summary>
        public ProgressChangedDelegate ProgressChanged { get; set; }

        /// <summary>
        /// Validates the file at <paramref name="filePath"/> and sets the version.
        /// </summary>
        /// <param name="filePath">The paht to the file.</param>
        public void ValidateFile(string filePath)
        {
            try
            {
                using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(filePath))
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
        public bool Import(object targetItem, string filePath)
        {
            var importResult = ReadHydraulicBoundaryLocations(filePath);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!shouldCancel)
                {
                    AddImportedDataToModel(targetItem, importResult);
                    log.Info("Locaties uit de hydraulische randvoorwaarden ingelezen");
                    return true;
                }

                log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_cancelled);
                shouldCancel = false;
            }

            return false;
        }

        /// <summary>
        /// This method cancels an import.
        /// </summary>
        public void Cancel()
        {
            shouldCancel = true;
        }

        private ReadResult<HydraulicBoundaryLocation> ReadHydraulicBoundaryLocations(string path)
        {
            NotifyProgress("Inlezen van de de hydraulische randvoorwaarden database", 1, 1);

            try
            {
                using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(path))
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

        private ReadResult<HydraulicBoundaryLocation> GetHydraulicBoundaryLocationReadResult(string path, HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader)
        {
            var totalNumberOfSteps = hydraulicBoundaryDatabaseReader.Count;
            var currentStep = 1;

            var locations = new Collection<HydraulicBoundaryLocation>();
            while (hydraulicBoundaryDatabaseReader.HasNext)
            {
                if (shouldCancel)
                {
                    return new ReadResult<HydraulicBoundaryLocation>(false);
                }
                try
                {
                    NotifyProgress("Inlezen van de locaties uit de hydraulische randvoorwaarden database", currentStep++, totalNumberOfSteps);
                    locations.Add(hydraulicBoundaryDatabaseReader.ReadLocation());
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

            int totalProfileCount = imported.ImportedItems.Count;
            NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_Adding_imported_data_to_model, totalProfileCount, totalProfileCount);

            foreach (var item in imported.ImportedItems)
            {
                targetCollection.Add(item);
            }
        }

        private void NotifyProgress(string currentStepName, int currentStep, int totalNumberOfSteps)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(currentStepName, currentStep, totalNumberOfSteps);
            }
        }
    }
}