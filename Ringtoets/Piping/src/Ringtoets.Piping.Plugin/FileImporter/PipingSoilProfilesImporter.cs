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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using Core.Common.Base.IO;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using ApplicationResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the DSoilModel application.
    /// </summary>
    public class PipingSoilProfilesImporter : IFileImporter
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipingSoilProfilesImporter));
        private bool shouldCancel;

        public string Name
        {
            get
            {
                return PipingFormsResources.PipingSoilProfilesCollection_DisplayName;
            }
        }

        public string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public Bitmap Image
        {
            get
            {
                return PipingFormsResources.PipingSoilProfileIcon;
            }
        }

        public Type SupportedItemType
        {
            get
            {
                return typeof(ICollection<PipingSoilProfile>);
            }
        }

        public string FileFilter
        {
            get
            {
                return String.Format("{0} {1} (*.soil)|*.soil",
                                     PipingFormsResources.PipingSoilProfilesCollection_DisplayName, ApplicationResources.Soil_file_name);
            }
        }

        public void Cancel()
        {
            shouldCancel = true;
        }

        public ProgressChangedDelegate ProgressChanged { get; set; }

        public bool Import(object targetItem, string filePath)
        {
            var importResult = ReadSoilProfiles(filePath);

            if (!importResult.CriticalErrorOccurred)
            {
                if (!shouldCancel)
                {
                    AddImportedDataToModel(targetItem, importResult);

                    return true;
                }

                HandleUserCancellingImport();
            }

            return false;
        }

        private PipingReadResult<PipingSoilProfile> ReadSoilProfiles(string path)
        {
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_Reading_database, 1, 1);

            try
            {
                using (var soilProfileReader = new PipingSoilProfileReader(path))
                {
                    return GetProfileReadResult(path, soilProfileReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(path, e);
            }
            return new PipingReadResult<PipingSoilProfile>(true);
        }

        private void HandleException(string path, Exception e)
        {
            var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            log.Error(message);
        }

        private PipingReadResult<PipingSoilProfile> GetProfileReadResult(string path, PipingSoilProfileReader soilProfileReader)
        {
            var totalNumberOfSteps = soilProfileReader.Count;
            var currentStep = 1;

            var profiles = new Collection<PipingSoilProfile>();
            while (soilProfileReader.HasNext)
            {
                if (shouldCancel)
                {
                    return new PipingReadResult<PipingSoilProfile>(false);
                }
                try
                {
                    NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_ReadingSoilProfiles, currentStep++, totalNumberOfSteps);
                    profiles.Add(soilProfileReader.ReadProfile());
                }
                catch (PipingSoilProfileReadException e)
                {
                    var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                                e.Message);
                    log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    var message = string.Format(ApplicationResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                path, e.Message);
                    log.Error(message);
                    return new PipingReadResult<PipingSoilProfile>(true);
                }
            }
            return new PipingReadResult<PipingSoilProfile>(false)
            {
                ImportedItems = profiles
            };
        }

        private void AddImportedDataToModel(object target, PipingReadResult<PipingSoilProfile> imported)
        {
            var targetCollection = (ICollection<PipingSoilProfile>)target;

            int totalProfileCount = imported.ImportedItems.Count;
            NotifyProgress(ApplicationResources.PipingSoilProfilesImporter_Adding_imported_data_to_model, totalProfileCount, totalProfileCount);

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

        private void HandleUserCancellingImport()
        {
            log.Info(ApplicationResources.PipingSoilProfilesImporter_Import_Import_cancelled);

            shouldCancel = false;
        }
    }
}