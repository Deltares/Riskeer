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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.Primitives;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the DSoilModel application.
    /// </summary>
    public class PipingSoilProfilesImporter : FileImporterBase<ICollection<PipingSoilProfile>>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipingSoilProfilesImporter));

        public override string Name
        {
            get
            {
                return PipingFormsResources.PipingSoilProfilesCollection_DisplayName;
            }
        }

        public override string Category
        {
            get
            {
                return RingtoetsFormsResources.Ringtoets_Category;
            }
        }

        public override Bitmap Image
        {
            get
            {
                return PipingFormsResources.PipingSoilProfileIcon;
            }
        }

        public override string FileFilter
        {
            get
            {
                return String.Format("{0} {1} (*.soil)|*.soil",
                                     PipingFormsResources.PipingSoilProfilesCollection_DisplayName, RingtoetsPluginResources.Soil_file_name);
            }
        }

        public override ProgressChangedDelegate ProgressChanged { protected get; set; }

        public override bool Import(object targetItem, string filePath)
        {
            if (!IsReferenceLineAvailable(targetItem))
            {
                var message = String.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                            RingtoetsPluginResources.PipingSurfaceLinesCsvImporter_Import_Required_referenceline_missing);
                log.Error(message);
                return false;
            }

            var importSoilProfileResult = ReadSoilProfiles(filePath);
            if (importSoilProfileResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (ImportIsCancelled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddImportedDataToModel(targetItem, importSoilProfileResult);
            return true;
        }

        private static bool IsReferenceLineAvailable(object targetItem)
        {
            //return ((RingtoetsPipingSurfaceLinesContext) targetItem).AssessmentSection.ReferenceLine != null;
            return true;
        }

        private void HandleException(string path, Exception e)
        {
            var message = string.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            log.Error(message);
        }

        private void HandleUserCancellingImport()
        {
            log.Info(RingtoetsPluginResources.PipingSoilProfilesImporter_Import_Import_cancelled);

            ImportIsCancelled = false;
        }

        #region read stochastic soil models

        private ReadResult<StochasticSoilModel> ReadStochasticSoilModels(string path)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_Reading_database, 1, 1);
            try
            {
                using (var stochasticSoilModelReader = new StochasticSoilModelReader(path))
                {
                    return GetStochasticSoilModelReadResult(path, stochasticSoilModelReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(path, e);
            }
            return new ReadResult<StochasticSoilModel>(true);
        }

        private ReadResult<StochasticSoilModel> GetStochasticSoilModelReadResult(string path, StochasticSoilModelReader stochasticSoilModelReader)
        {
            var totalNumberOfSteps = stochasticSoilModelReader.PipingStochasticSoilModelCount;
            var currentStep = 1;

            var soilModels = new Collection<StochasticSoilModel>();
            while (stochasticSoilModelReader.HasNext)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<StochasticSoilModel>(false);
                }
                try
                {
                    NotifyProgress("Inlezen van de ondergrondsmodellen uit de D-Soil Model database.", currentStep++, totalNumberOfSteps);
                    soilModels.Add(stochasticSoilModelReader.ReadStochasticSoilModel());
                }
                catch (PipingSoilProfileReadException e)
                {
                    var message = string.Format("{0} " +
                                                "Dit ondergrondsmodel wordt overgeslagen.",
                                                e.Message);
                    log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    var message = string.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                path, e.Message);
                    log.Error(message);
                    return new ReadResult<StochasticSoilModel>(true);
                }
            }
            return new ReadResult<StochasticSoilModel>(false)
            {
                ImportedItems = soilModels
            };
        }

        #endregion

        #region read soil profiles

        private ReadResult<PipingSoilProfile> ReadSoilProfiles(string path)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_Reading_database, 1, 1);
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
            return new ReadResult<PipingSoilProfile>(true);
        }

        private void AddImportedDataToModel(object target, ReadResult<PipingSoilProfile> imported)
        {
            var targetCollection = (ICollection<PipingSoilProfile>) target;

            int totalProfileCount = imported.ImportedItems.Count;
            NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_Adding_imported_data_to_model, totalProfileCount, totalProfileCount);
            foreach (var item in imported.ImportedItems)
            {
                targetCollection.Add(item);
            }
        }

        private ReadResult<PipingSoilProfile> GetProfileReadResult(string path, PipingSoilProfileReader soilProfileReader)
        {
            var totalNumberOfSteps = soilProfileReader.Count;
            var currentStep = 1;

            var profiles = new Collection<PipingSoilProfile>();
            while (soilProfileReader.HasNext)
            {
                if (ImportIsCancelled)
                {
                    return new ReadResult<PipingSoilProfile>(false);
                }
                try
                {
                    NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_ReadingSoilProfiles, currentStep++, totalNumberOfSteps);
                    profiles.Add(soilProfileReader.ReadProfile());
                }
                catch (PipingSoilProfileReadException e)
                {
                    var message = string.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                                e.Message);
                    log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    var message = string.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                                path, e.Message);
                    log.Error(message);
                    return new ReadResult<PipingSoilProfile>(true);
                }
            }
            return new ReadResult<PipingSoilProfile>(false)
            {
                ImportedItems = profiles
            };
        }

        #endregion
    }
}