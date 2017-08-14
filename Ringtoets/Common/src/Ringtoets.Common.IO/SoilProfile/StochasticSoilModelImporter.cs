﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.IO.Exceptions;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// Imports .soil files (SqlLite database files) created with the D-Soil Model application.
    /// </summary>
    /// <typeparam name="T">The type of stochastic soil models to import.</typeparam>
    public class StochasticSoilModelImporter<T>
        : FileImporterBase<ObservableUniqueItemCollectionWithSourcePath<T>> where T
        : class, IMechanismStochasticSoilModel
    {
        private readonly IImporterMessageProvider messageProvider;
        private readonly IStochasticSoilModelUpdateModelStrategy<T> updateStrategy;
        private readonly IStochasticSoilModelTransformer<T> transformer;
        private readonly IStochasticSoilModelMechanismFilter filter;

        private IEnumerable<IObservable> updatedInstances;

        /// <summary>
        /// Creates a new instance of the <see cref="StochasticSoilModelImporter{T}"/> class.
        /// </summary>
        /// <param name="importTarget">The import target.</param>
        /// <param name="filePath">The path to the file to import from.</param>
        /// <param name="messageProvider">The message provider to provide messages during importer 
        /// actions.</param>
        /// <param name="configuration">The mechanism specific configuration containing all 
        /// necessary stochastic soil model components.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        public StochasticSoilModelImporter(
            ObservableUniqueItemCollectionWithSourcePath<T> importTarget,
            string filePath,
            IImporterMessageProvider messageProvider,
            StochasticSoilModelImporterConfiguration<T> configuration)
            : base(filePath, importTarget)
        {
            if (messageProvider == null)
            {
                throw new ArgumentNullException(nameof(messageProvider));
            }
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.messageProvider = messageProvider;
            updateStrategy = configuration.UpdateStrategy;
            transformer = configuration.Transformer;
            filter = configuration.MechanismFilter;

            updatedInstances = Enumerable.Empty<IObservable>();
        }

        protected override void LogImportCanceledMessage()
        {
            string message = messageProvider.GetCancelledLogMessageText(RingtoetsCommonDataResources.StochasticSoilModelCollection_TypeDescriptor);
            Log.Info(message);
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
            ReadResult<StochasticSoilModel> importStochasticSoilModelResult = ReadStochasticSoilModels();
            if (importStochasticSoilModelResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            ReadResult<T> transformedStochasticSoilModels;

            try
            {
                transformedStochasticSoilModels = GetTransformedStochasticSoilModels(importStochasticSoilModelResult.Items);
            }
            catch (ImportedDataTransformException e)
            {
                Log.ErrorFormat(Resources.StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped,
                                e.Message);
                return false;
            }

            if (transformedStochasticSoilModels.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            if (!transformedStochasticSoilModels.Items.Any())
            {
                Log.WarnFormat(Resources.StochasticSoilModelImporter_No_stochastic_soil_models_found_for_failure_mechanism);
                return true;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 1, 1);

            try
            {
                updatedInstances = updateStrategy.UpdateModelWithImportedData(transformedStochasticSoilModels.Items, FilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(messageProvider.GetUpdateDataFailedLogMessageText(
                                                   RingtoetsCommonDataResources.StochasticSoilModelCollection_TypeDescriptor),
                                               e.Message);
                Log.Error(message, e);
                return false;
            }
            return true;
        }

        private void HandleException(Exception e)
        {
            string message = string.Format(Resources.StochasticSoilModelImporter_CriticalErrorMessage_0_File_Skipped,
                                           e.Message);
            Log.Error(message, e);
        }

        #region Read stochastic soil models

        private ReadResult<StochasticSoilModel> ReadStochasticSoilModels()
        {
            NotifyProgress(Resources.StochasticSoilModelImporter_Reading_database, 1, 1);
            try
            {
                using (var stochasticSoilModelReader = new StochasticSoilModelReader(FilePath))
                {
                    stochasticSoilModelReader.Validate();
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
            int totalNumberOfSteps = stochasticSoilModelReader.StochasticSoilModelCount;
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
                    NotifyProgress(Resources.StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Reading_stochastic_soil_models_from_database,
                                   currentStep++,
                                   totalNumberOfSteps);
                    soilModels.Add(stochasticSoilModelReader.ReadStochasticSoilModel());
                }
                catch (StochasticSoilModelException e)
                {
                    HandleException(e);
                    return new ReadResult<StochasticSoilModel>(true);
                }
            }

            return new ReadResult<StochasticSoilModel>(false)
            {
                Items = soilModels
            };
        }

        #endregion

        #region Validate stochastic soil models

        /// <summary>
        /// Transforms the stochastic soil models into mechanism specific stochastic soil models.
        /// </summary>
        /// <param name="stochasticSoilModels">The stochastic soil models to transform.</param>
        /// <returns>Returns a <see cref="ReadResult{T}"/> collection of mechanism specific stochastic 
        /// soil models, or an empty <see cref="ReadResult{T}"/> when any of the <paramref name="stochasticSoilModels"/> 
        /// is not valid to be transformed.</returns>
        /// <exception cref="ImportedDataTransformException">Thrown when transforming a stochastic 
        /// soil model failed.</exception>
        private ReadResult<T> GetTransformedStochasticSoilModels(ICollection<StochasticSoilModel> stochasticSoilModels)
        {
            var transformedStochasticSoilModels = new List<T>();
            var stochasticSoilModelNumber = 1;
            StochasticSoilModel[] importedModels = stochasticSoilModels.Where(ssm => filter.IsValidForFailureMechanism(ssm))
                                                                       .ToArray();

            foreach (StochasticSoilModel stochasticSoilModel in importedModels)
            {
                NotifyProgress(Resources.Importer_ProgressText_Validating_imported_data, stochasticSoilModelNumber++, importedModels.Length);
                if (Canceled)
                {
                    return new ReadResult<T>(false);
                }

                if (!ValidateStochasticSoilModel(stochasticSoilModel))
                {
                    return new ReadResult<T>(true);
                }

                transformedStochasticSoilModels.Add(transformer.Transform(stochasticSoilModel));
            }

            return new ReadResult<T>(false)
            {
                Items = transformedStochasticSoilModels
            };
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
                Log.ErrorFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_No_profiles_found_in_stochastic_soil_model_0,
                                stochasticSoilModel.Name);
                return false;
            }
            if (!IsSumOfAllProbabilitiesEqualToOne(stochasticSoilModel))
            {
                Log.ErrorFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct,
                                stochasticSoilModel.Name);
                return false;
            }
            return true;
        }

        private static bool IsSumOfAllProbabilitiesEqualToOne(StochasticSoilModel stochasticSoilModel)
        {
            double sumOfAllScenarioProbabilities = stochasticSoilModel.StochasticSoilProfiles
                                                                      .Sum(s => s.Probability);
            return Math.Abs(sumOfAllScenarioProbabilities - 1.0) < 1e-6;
        }

        #endregion
    }
}