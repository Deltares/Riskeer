﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
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
    public class PipingSoilProfilesImporter : FileImporterBase<ObservableList<StochasticSoilModel>>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipingSoilProfilesImporter));

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingSoilProfilesImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The collection to update.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="importTarget"/> is <c>null</c>.</exception>
        public PipingSoilProfilesImporter(ObservableList<StochasticSoilModel> importTarget, string filePath) : base(filePath, importTarget) {}

        public override bool Import()
        {
            var importSoilProfileResult = ReadSoilProfiles();
            if (importSoilProfileResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            var importStochasticSoilModelResult = ReadStochasticSoilModels();
            if (importStochasticSoilModelResult.CriticalErrorOccurred)
            {
                return false;
            }

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddSoilProfilesToStochasticSoilModels(importSoilProfileResult.ImportedItems, importStochasticSoilModelResult.ImportedItems);

            CheckIfAllProfilesAreUsed(importSoilProfileResult.ImportedItems, importStochasticSoilModelResult.ImportedItems);

            if (Canceled)
            {
                HandleUserCancellingImport();
                return false;
            }

            AddImportedDataToModel(importStochasticSoilModelResult.ImportedItems);
            return true;
        }

        private void AddSoilProfilesToStochasticSoilModels(ICollection<PipingSoilProfile> soilProfiles, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            foreach (var stochasticSoilModel in stochasticSoilModels)
            {
                foreach (var stochasticSoilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    var soilProfile = soilProfiles.FirstOrDefault(s => s.SoilProfileType == stochasticSoilProfile.SoilProfileType && s.PipingSoilProfileId == stochasticSoilProfile.SoilProfileId);
                    if (soilProfile != null)
                    {
                        stochasticSoilProfile.SoilProfile = soilProfile;
                    }
                }
            }
        }

        private void CheckIfAllProfilesAreUsed(ICollection<PipingSoilProfile> soilProfiles, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_CheckIfAllProfilesAreUsed_Start_checking_soil_profiles, 1, 1);
            foreach (var soilProfile in soilProfiles.Where(soilProfile => !PipingSoilProfileIsUsed(soilProfile, stochasticSoilModels)))
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSoilProfilesImporter_CheckIfAllProfilesAreUsed_SoilProfile_0_is_not_used_in_any_stochastic_soil_model, soilProfile.Name);
            }
        }

        private static bool PipingSoilProfileIsUsed(PipingSoilProfile soilProfile, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            return stochasticSoilModels.Any(stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Any(stochasticSoilProfile => stochasticSoilProfile.SoilProfile == soilProfile));
        }

        private void AddImportedDataToModel(ICollection<StochasticSoilModel> readStochasticSoilModels)
        {
            var stochasticSoilModelCount = readStochasticSoilModels.Count;
            var currentIndex = 1;
            foreach (var readStochasticSoilModel in readStochasticSoilModels)
            {
                NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_Adding_imported_data_to_model, currentIndex++, stochasticSoilModelCount);
                if (!ValidateStochasticSoilModel(readStochasticSoilModel))
                {
                    continue;
                }
                var stochasticSoilModel = ImportTarget.FirstOrDefault(ssm => ssm.Id == readStochasticSoilModel.Id);
                if (stochasticSoilModel != null)
                {
                    log.WarnFormat(RingtoetsPluginResources.PipingSoilProfilesImporter_AddImportedDataToModel_Stochastisch_soil_model_0_already_exists, stochasticSoilModel.Name);
                }
                ImportTarget.Add(readStochasticSoilModel);
            }
        }

        private bool ValidateStochasticSoilModel(StochasticSoilModel stochasticSoilModel)
        {
            if (!stochasticSoilModel.StochasticSoilProfiles.Any())
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSoilProfilesImporter_ValidateStochasticSoilModel_No_profiles_found_in_stochastic_soil_model_0,
                               stochasticSoilModel.Name);
                return false;
            }
            if (stochasticSoilModel.StochasticSoilProfiles.Any(ssp => ssp.SoilProfile == null))
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSoilProfilesImporter_ValidateStochasticSoilModel_SoilModel_0_with_stochastic_soil_profile_without_profile,
                               stochasticSoilModel.Name);
                return false;
            }
            if (!IsSumOfAllProbabilitiesEqualToOne(stochasticSoilModel))
            {
                log.WarnFormat(RingtoetsPluginResources.PipingSoilProfilesImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct,
                               stochasticSoilModel.Name);
            }
            return true;
        }

        private static bool IsSumOfAllProbabilitiesEqualToOne(StochasticSoilModel stochasticSoilModel)
        {
            double sumOfAllScenarioProbabilities = stochasticSoilModel.StochasticSoilProfiles
                                                                      .Where(s => s.SoilProfile != null)
                                                                      .Sum(s => s.Probability);
            return Math.Abs(sumOfAllScenarioProbabilities - 1.0) < 1e-6;
        }

        private void HandleException(Exception e)
        {
            var message = string.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_CriticalErrorMessage_0_File_Skipped,
                                        e.Message);
            log.Error(message);
        }

        private void HandleUserCancellingImport()
        {
            log.Info(RingtoetsPluginResources.PipingSoilProfilesImporter_Import_Import_cancelled);

            Canceled = false;
        }

        #region read stochastic soil models

        private ReadResult<StochasticSoilModel> ReadStochasticSoilModels()
        {
            NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_Reading_database, 1, 1);
            try
            {
                using (var stochasticSoilModelReader = new StochasticSoilModelReader(FilePath))
                {
                    return GetStochasticSoilModelReadResult(stochasticSoilModelReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(e);
            }
            return new ReadResult<StochasticSoilModel>(true);
        }

        private ReadResult<StochasticSoilModel> GetStochasticSoilModelReadResult(StochasticSoilModelReader stochasticSoilModelReader)
        {
            var totalNumberOfSteps = stochasticSoilModelReader.PipingStochasticSoilModelCount;
            var currentStep = 1;

            var soilModels = new Collection<StochasticSoilModel>();
            while (stochasticSoilModelReader.HasNext)
            {
                if (Canceled)
                {
                    return new ReadResult<StochasticSoilModel>(false);
                }
                try
                {
                    NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_GetStochasticSoilModelReadResult_Reading_stochastic_soil_models_from_database, currentStep++, totalNumberOfSteps);
                    soilModels.Add(stochasticSoilModelReader.ReadStochasticSoilModel());
                }
                catch (StochasticSoilProfileReadException e)
                {
                    var message = string.Format(RingtoetsPluginResources.PipingSoilProfilesImporter_GetStochasticSoilModelReadResult_Error_0_stochastic_soil_model_skipped, e.Message);
                    log.Error(message);
                }
            }
            return new ReadResult<StochasticSoilModel>(false)
            {
                ImportedItems = soilModels
            };
        }

        #endregion

        #region read soil profiles

        private ReadResult<PipingSoilProfile> ReadSoilProfiles()
        {
            NotifyProgress(RingtoetsPluginResources.PipingSoilProfilesImporter_Reading_database, 1, 1);
            try
            {
                using (var soilProfileReader = new PipingSoilProfileReader(FilePath))
                {
                    return GetProfileReadResult(soilProfileReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(e);
            }
            return new ReadResult<PipingSoilProfile>(true);
        }

        private ReadResult<PipingSoilProfile> GetProfileReadResult(PipingSoilProfileReader soilProfileReader)
        {
            var totalNumberOfSteps = soilProfileReader.Count;
            var currentStep = 1;

            var profiles = new Collection<PipingSoilProfile>();
            while (soilProfileReader.HasNext)
            {
                if (Canceled)
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
                                                FilePath, e.Message);
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