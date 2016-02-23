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
    /// Imports locations read from an Hydraulic boundary .sqlite file (SqlLite database file) to a 
    /// collection of <see cref="HydraulicBoundaryLocation"/>.
    /// </summary>
    public class HydraulicBoundaryLocationsImporter : FileImporterBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryLocationsImporter));

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

        public string GetHydraulicBoundaryDatabaseVersion(string filePath)
        {
            using (var hydraulicBoundaryDatabaseReader = new HydraulicBoundarySqLiteDatabaseReader(filePath))
            {
                return hydraulicBoundaryDatabaseReader.Version;
            }
        }

        public override bool Import(object targetItem, string filePath)
        {
            var importResult = ReadHydraulicBoundaryLocations(filePath);

            if (importResult.CriticalErrorOccurred)
            {
                return false;
            }
            if (ImportIsCancelled)
            {
                log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_cancelled);
                ImportIsCancelled = false;

                return false;
            }

            AddImportedDataToModel(targetItem, importResult);
            log.Info(ApplicationResources.HydraulicBoundaryLocationsImporter_Import_Import_successful);
            return true;
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
            catch (LineParseException e)
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

        private ReadResult<HydraulicBoundaryLocation> GetHydraulicBoundaryLocationReadResult(string path,
                                                                                             HydraulicBoundarySqLiteDatabaseReader hydraulicBoundarySqLiteDatabaseReader)
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
                NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_GetHydraulicBoundaryLocationReadResult, currentStep++, totalNumberOfSteps);
                try
                {
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
            NotifyProgress(ApplicationResources.HydraulicBoundaryLocationsImporter_Adding_imported_data_to_model,
                           totalCount, totalCount);

            foreach (var item in imported.ImportedItems)
            {
                targetCollection.Add(item);
            }
        }
    }
}