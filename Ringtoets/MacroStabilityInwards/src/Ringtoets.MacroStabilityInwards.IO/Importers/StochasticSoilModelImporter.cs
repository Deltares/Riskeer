// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.IO.Readers;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Exceptions;
using Ringtoets.MacroStabilityInwards.IO.Properties;
using Ringtoets.MacroStabilityInwards.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsMacroStabilityInwardsDataResources = Ringtoets.MacroStabilityInwards.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.IO.Importers
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the D-Soil Model application.
    /// </summary>
    public class StochasticSoilModelImporter : FileImporterBase<StochasticSoilModelCollection>
    {
        private readonly IImporterMessageProvider messageProvider;
        private readonly IStochasticSoilModelUpdateModelStrategy modelUpdateStrategy;
        private IEnumerable<IObservable> updatedInstances;

        /// <summary>
        /// Initializes a new instance of the <see cref="StochasticSoilModelImporter"/> class.
        /// </summary>
        /// <param name="importTarget">The collection to update.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer actions.</param>
        /// <param name="modelUpdateStrategy">The <see cref="IStochasticSoilModelUpdateModelStrategy"/> to use
        /// when updating the <paramref name="importTarget"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public StochasticSoilModelImporter(StochasticSoilModelCollection importTarget, string filePath,
                                           IImporterMessageProvider messageProvider, IStochasticSoilModelUpdateModelStrategy modelUpdateStrategy)
            : base(filePath, importTarget)
        {
            if (modelUpdateStrategy == null)
            {
                throw new ArgumentNullException(nameof(modelUpdateStrategy));
            }
            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }
            this.messageProvider = messageProvider;
            this.modelUpdateStrategy = modelUpdateStrategy;
            updatedInstances = Enumerable.Empty<IObservable>();
        }

        protected override void DoPostImportUpdates()
        {
            foreach (IObservable observable in updatedInstances)
            {
                observable.NotifyObservers();
            }
        }

        protected override bool OnImport()
        {
            ReadResult<MacroStabilityInwardsSoilProfile> importSoilProfileResult = ReadSoilProfiles();
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

            StochasticSoilModel[] validStochasticSoilModels = GetValidStochasticSoilModels(importStochasticSoilModelResult).ToArray();
            if (Canceled)
            {
                return false;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 1, 1);

            try
            {
                updatedInstances = modelUpdateStrategy.UpdateModelWithImportedData(validStochasticSoilModels, FilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(messageProvider.GetUpdateDataFailedLogMessageText(
                                                   RingtoetsMacroStabilityInwardsDataResources.StochasticSoilModelCollection_TypeDescriptor),
                                               e.Message);
                Log.Error(message, e);
                return false;
            }
            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            string message = messageProvider.GetCancelledLogMessageText(RingtoetsMacroStabilityInwardsDataResources.StochasticSoilModelCollection_TypeDescriptor);
            Log.Info(message);
        }

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
                Log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_No_profiles_found_in_stochastic_soil_model_0,
                               stochasticSoilModel.Name);
                return false;
            }
            if (stochasticSoilModel.StochasticSoilProfiles.Any(ssp => ssp.SoilProfile == null))
            {
                Log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_SoilModel_0_with_stochastic_soil_profile_without_profile,
                               stochasticSoilModel.Name);
                return false;
            }
            if (!IsSumOfAllProbabilitiesEqualToOne(stochasticSoilModel))
            {
                Log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct,
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
                NotifyProgress(RingtoetsCommonIOResources.Importer_ProgressText_Validating_imported_data, currentStep, importedModels.Length);
                if (Canceled)
                {
                    yield break;
                }

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

        private void AddSoilProfilesToStochasticSoilModels(ICollection<MacroStabilityInwardsSoilProfile> soilProfiles, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            foreach (StochasticSoilModel stochasticSoilModel in stochasticSoilModels)
            {
                foreach (StochasticSoilProfile stochasticSoilProfile in stochasticSoilModel.StochasticSoilProfiles)
                {
                    MacroStabilityInwardsSoilProfile soilProfile = soilProfiles.FirstOrDefault(s => s.SoilProfileType == stochasticSoilProfile.SoilProfileType && s.MacroStabilityInwardsSoilProfileId == stochasticSoilProfile.SoilProfileId);
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
                        Log.Warn(string.Format(Resources.StochasticSoilModelImporter_MergeStochasticSoilProfiles_Multiple_SoilProfile_0_used_in_StochasticSoilModel_1_Probabilities_added,
                                               previousProfile.SoilProfile.Name,
                                               stochasticSoilModel.Name));

                        previousProfile.AddProbability(currentProfile.Probability);
                        stochasticSoilModel.StochasticSoilProfiles.Remove(currentProfile);
                    }
                }
            }
        }

        private void CheckIfAllProfilesAreUsed(ICollection<MacroStabilityInwardsSoilProfile> soilProfiles, ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            NotifyProgress(Resources.StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_Start_checking_soil_profiles, 1, 1);
            foreach (MacroStabilityInwardsSoilProfile soilProfile in soilProfiles.Where(soilProfile => !SoilProfileIsUsed(soilProfile, stochasticSoilModels)))
            {
                Log.WarnFormat(Resources.StochasticSoilModelImporter_CheckIfAllProfilesAreUsed_SoilProfile_0_is_not_used_in_any_stochastic_soil_model, soilProfile.Name);
            }
        }

        private static bool SoilProfileIsUsed(MacroStabilityInwardsSoilProfile soilProfile, ICollection<StochasticSoilModel> stochasticSoilModels)
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
            Log.Error(message);
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
            int totalNumberOfSteps = stochasticSoilModelReader.MacroStabilityInwardsStochasticSoilModelCount;
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
                    Log.Error(message);
                }
            }
            return new ReadResult<StochasticSoilModel>(false)
            {
                Items = soilModels
            };
        }

        #endregion

        #region read soil profiles

        private ReadResult<MacroStabilityInwardsSoilProfile> ReadSoilProfiles()
        {
            NotifyProgress(Resources.StochasticSoilModelImporter_Reading_database, 1, 1);
            try
            {
                using (var soilProfileReader = new MacroStabilityInwardsSoilProfileReader(FilePath))
                {
                    return GetProfileReadResult(soilProfileReader);
                }
            }
            catch (CriticalFileReadException e)
            {
                HandleException(e);
            }
            return new ReadResult<MacroStabilityInwardsSoilProfile>(true);
        }

        private ReadResult<MacroStabilityInwardsSoilProfile> GetProfileReadResult(MacroStabilityInwardsSoilProfileReader soilProfileReader)
        {
            int totalNumberOfSteps = soilProfileReader.Count;
            var currentStep = 1;

            var profiles = new Collection<MacroStabilityInwardsSoilProfile>();
            while (soilProfileReader.HasNext)
            {
                if (Canceled)
                {
                    return new ReadResult<MacroStabilityInwardsSoilProfile>(false);
                }
                try
                {
                    NotifyProgress(Resources.StochasticSoilModelImporter_ReadingSoilProfiles, currentStep++, totalNumberOfSteps);
                    profiles.Add(soilProfileReader.ReadProfile());
                }
                catch (MacroStabilityInwardsSoilProfileReadException e)
                {
                    string message = string.Format(Resources.StochasticSoilModelImporter_ReadSoilProfiles_ParseErrorMessage_0_SoilProfile_skipped,
                                                   e.Message);
                    Log.Error(message);
                }
                catch (CriticalFileReadException e)
                {
                    string message = string.Format(Resources.StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped,
                                                   FilePath, e.Message);
                    Log.Error(message);
                    return new ReadResult<MacroStabilityInwardsSoilProfile>(true);
                }
            }
            return new ReadResult<MacroStabilityInwardsSoilProfile>(false)
            {
                Items = profiles
            };
        }

        #endregion
    }
}