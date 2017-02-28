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
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.Primitives;
using RingtoestCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the D-Soil Model application.
    /// </summary>
    public class StochasticSoilModelImporter : FileImporterBase<StochasticSoilModelCollection>
    {
        private readonly ILog log = LogManager.GetLogger(typeof(StochasticSoilModelImporter));
        private readonly IStochasticSoilModelUpdateModelStrategy modelUpdateStrategy;

        /// <summary>
        /// Initializes a new instance of the <see cref="StochasticSoilModelImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The collection to update.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="modelUpdateStrategy">The <see cref="IStochasticSoilModelUpdateModelStrategy"/> to use
        /// when updating the <paramref name="importTarget"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public StochasticSoilModelImporter(StochasticSoilModelCollection importTarget, string filePath, IStochasticSoilModelUpdateModelStrategy modelUpdateStrategy)
            : base(filePath, importTarget)
        {
            if (modelUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(modelUpdateStrategy));
            }
            this.modelUpdateStrategy = modelUpdateStrategy;
        }

        protected override void DoPostImportUpdates()
        {
            foreach (IObservable observable in UpdatedInstances)
            {
                observable.NotifyObservers();
            }
        }

        protected override bool OnImport()
        {
            ReadResult<PipingSoilProfile> importSoilProfileResult = ReadSoilProfiles();
            if (importSoilProfileResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadResult<StochasticSoilModel> importStochasticSoilModelResult = ReadStochasticSoilModels();
            if (importStochasticSoilModelResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            AddSoilProfilesToStochasticSoilModels(importSoilProfileResult.Items, importStochasticSoilModelResult.Items);
            MergeStochasticSoilProfiles(importStochasticSoilModelResult.Items);
            CheckIfAllProfilesAreUsed(importSoilProfileResult.Items, importStochasticSoilModelResult.Items);
            if (Canceled)
            {
                return false;
            }

            UpdatedInstances = modelUpdateStrategy.UpdateModelWithImportedData(ImportTarget, GetValidStochasticSoilModels(importStochasticSoilModelResult), FilePath);

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            log.Info(Resources.StochasticSoilModelImporter_Import_Import_canceled);
        }

        private IEnumerable<IObservable> UpdatedInstances { get; set; } = Enumerable.Empty<IObservable>();

        /// <summary>
        /// Validate the definition of a <see cref="StochasticSoilModel"/>.
        /// </summary>
        /// <param name="stochasticSoilModel">The <see cref="StochasticSoilModel"/> to validate.</param>
        /// <returns><c>false</c> when the stochastic soil model does not contain any stochastic soil profiles
        /// or when a stochastic soil profile does not have a definition for a soil profile; <c>true</c>
        /// otherwise.</returns>
        private bool ValidateStochasticSoilModel(StochasticSoilModel stochasticSoilModel)
        {
            if (!stochasticSoilModel.StochasticSoilProfiles.Any())
            {
                log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_No_profiles_found_in_stochastic_soil_model_0,
                               stochasticSoilModel.Name);
                return false;
            }
            if (stochasticSoilModel.StochasticSoilProfiles.Any(ssp => ssp.SoilProfile == null))
            {
                log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_SoilModel_0_with_stochastic_soil_profile_without_profile,
                               stochasticSoilModel.Name);
                return false;
            }
            if (!IsSumOfAllProbabilitiesEqualToOne(stochasticSoilModel))
            {
                log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct,
                               stochasticSoilModel.Name);
            }
            return true;
        }

        private IEnumerable<StochasticSoilModel> GetValidStochasticSoilModels(ReadResult<StochasticSoilModel> importStochasticSoilModelResult)
        {
            var currentStep = 1;
            StochasticSoilModel[] importedModels = importStochasticSoilModelResult.Items.ToArray();
            foreach (StochasticSoilModel importedModel in importedModels)
            {
                NotifyProgress(RingtoestCommonIOResources.Importer_ProgressText_Adding_imported_data_to_data_model, currentStep, importedModels.Length);
                if (ValidateStochasticSoilModel(importedModel))
                {
                    yield return importedModel;
                }
                currentStep++;
            }
        }

        private static bool IsSumOfAllProbabilitiesEqualToOne(StochasticSoilModel stochasticSoilModel)
        {
            double sumOfAllScenarioProbabilities = stochasticSoilModel.StochasticSoilProfiles
                                                                      .Where(s => s.SoilProfile != null)
                                                                      .Sum(s => s.Probability);
            return Math.Abs(sumOfAllScenarioProbabilities - 1.0) < 1e-6;
        }

        private void AddSoilProfilesToStochasticSoilModels(ICollection<PipingSoilProfile> soilProfiles, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            foreach (StochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                foreach (StochasticSoilProfile stochasticSoilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    PipingSoilProfile soilProfile = soilProfiles.FirstOrDefault(s => s.SoilProfileType == stochasticSoilProfile.SoilProfileType && s.PipingSoilProfileId == stochasticSoilProfile.SoilProfileId);
                    if (soilProfile != null)
                    {
                        stochasticSoilProfile.SoilProfile = soilProfile;
                    }
                }
            }
        }

        private void MergeStochasticSoilProfiles(ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            foreach (StochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                StochasticSoilProfile[] profiles = stochasticSoilModel.StochasticSoilProfiles.OrderBy(sp => sp.SoilProfileId).ToArray();
                for (var i = 1; i < profiles.Length; i++)
                {
                    StochasticSoilProfile previousProfile = profiles[i - 1];
                    StochasticSoilProfile currentProfile = profiles[i];
                    if (currentProfile.SoilProfileId == previousProfile.SoilProfileId &&
                        currentProfile.SoilProfileType == previousProfile.SoilProfileType)
                    {
                        log.Warn(string.Format(Resources.StochasticSoilModelImporter_MergeStochasticSoilProfiles_Multiple_SoilProfile_0_used_in_StochasticSoilModel_1_Probabilities_added,
                                               previousProfile.SoilProfile.Name,
                                               stochasticSoilModel.Name));

                        previousProfile.AddProbability(currentProfile.Probability);
                        stochasticSoilModel.StochasticSoilProfiles.Remove(currentProfile);
                    }
                }
            }
        }

        private void CheckIfAllProfilesAreUsed(ICollection<PipingSoilProfile> soilProfiles, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            NotifyProgress(Resources.StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_Start_checking_soil_profiles, 1, 1);
            foreach (PipingSoilProfile soilProfile in soilProfiles.Where(soilProfile => !PipingSoilProfileIsUsed(soilProfile, stochasticSoilModels)))
            {
                log.WarnFormat(Resources.StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_SoilProfile_0_is_not_used_in_any_stochastic_soil_model, soilProfile.Name);
            }
        }

        private static bool PipingSoilProfileIsUsed(PipingSoilProfile soilProfile, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            return stochasticSoilModels.Any(
                stochasticSoilModel => stochasticSoilModel
                    .StochasticSoilProfiles
                    .Any(stochasticSoilProfile => ReferenceEquals(stochasticSoilProfile.SoilProfile, soilProfile)));
        }

        private void HandleException(Exception e)
        {
            string message = string.Format(Resources.StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped,
                                           e.Message);
            log.Error(message);
        }

        #region read stochastic soil models

        private ReadResult<StochasticSoilModel> ReadStochasticSoilModels()
        {
            NotifyProgress(Resources.StochasticSoilModelImporter_Reading_database, 1, 1);
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
            int totalNumberOfSteps = stochasticSoilModelReader.PipingStochasticSoilModelCount;
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
                    NotifyProgress(Resources.StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Reading_stochastic_soil_models_from_database, currentStep++, totalNumberOfSteps);
                    soilModels.Add(stochasticSoilModelReader.ReadStochasticSoilModel());
                }
                catch (StochasticSoilProfileReadException e)
                {
                    string message = string.Format(Resources.StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Error_0_stochastic_soil_model_skipped, e.Message);
                    log.Error(message);
                }
            }
            return new ReadResult<StochasticSoilModel>(false)
            {
                Items = soilModels
            };
        }

        #endregion

        #region read soil profiles

        private ReadResult<PipingSoilProfile> ReadSoilProfiles()
        {
            NotifyProgress(Resources.StochasticSoilModelImporter_Reading_database, 1, 1);
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
            int totalNumberOfSteps = soilProfileReader.Count;
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
                    NotifyProgress(Resources.StochasticSoilModelImporter_ReadingSoilProfiles, currentStep++, totalNumberOfSteps);
                    profiles.Add(soilProfileReader.ReadProfile());
                }
                catch (PipingSoilProfileReadException e)
                {
                    string message = string.Format(Resources.StochasticSoilModelImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                                   e.Message);
                    log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    string message = string.Format(Resources.StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped,
                                                   FilePath, e.Message);
                    log.Error(message);
                    return new ReadResult<PipingSoilProfile>(true);
                }
            }
            return new ReadResult<PipingSoilProfile>(false)
            {
                Items = profiles
            };
        }

        #endregion
    }
}