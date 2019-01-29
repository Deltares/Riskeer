// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.IO.Exceptions;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.Properties;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.Common.IO.SoilProfile
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
        /// Creates a new instance of <see cref="StochasticSoilModelImporter{T}"/>.
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
            string message = messageProvider.GetCancelledLogMessageText(RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor);
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
                Log.Error(e.Message, e);
                return false;
            }

            if (transformedStochasticSoilModels.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            if (!transformedStochasticSoilModels.Items.Any())
            {
                Log.Error(Resources.StochasticSoilModelImporter_No_stochastic_soil_models_found_for_failure_mechanism);
                return false;
            }

            NotifyProgress(messageProvider.GetAddDataToModelProgressText(), 1, 1);

            try
            {
                updatedInstances = updateStrategy.UpdateModelWithImportedData(transformedStochasticSoilModels.Items, FilePath);
            }
            catch (UpdateDataException e)
            {
                string message = string.Format(messageProvider.GetUpdateDataFailedLogMessageText(
                                                   RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor),
                                               e.Message);
                Log.Error(message, e);
                return false;
            }

            return true;
        }

        private void HandleException(Exception e)
        {
            Log.Error(e.Message, e);
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
            catch (Exception e) when (e is StochasticSoilModelException
                                      || e is CriticalFileReadException)
            {
                HandleException(e);
            }

            return new ReadResult<StochasticSoilModel>(true);
        }

        /// <summary>
        /// Reads all stochastic soil models from the <paramref name="stochasticSoilModelReader"/>.
        /// </summary>
        /// <param name="stochasticSoilModelReader">The <see cref="StochasticSoilModelReader"/>.</param>
        /// <returns>Returns a <see cref="ReadResult{T}"/> collection of read stochastic soil models.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the database returned incorrect 
        /// values for required properties.</exception>
        /// <exception cref="StochasticSoilModelException">Thrown when:
        /// <list type="bullet">
        /// <item>no stochastic soil profiles could be read;</item>
        /// <item>the read failure mechanism type is not supported.</item>
        /// </list>
        /// </exception>
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

                NotifyProgress(Resources.StochasticSoilModelImporter_GetStochasticSoilModelReadResult_Reading_stochastic_soil_models_from_database,
                               currentStep++,
                               totalNumberOfSteps);
                soilModels.Add(stochasticSoilModelReader.ReadStochasticSoilModel());
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
        private ReadResult<T> GetTransformedStochasticSoilModels(IEnumerable<StochasticSoilModel> stochasticSoilModels)
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

                ValidateStochasticSoilModel(stochasticSoilModel);

                transformedStochasticSoilModels.Add(transformer.Transform(stochasticSoilModel));
            }

            return new ReadResult<T>(false)
            {
                Items = transformedStochasticSoilModels
            };
        }

        private void ValidateStochasticSoilModel(StochasticSoilModel stochasticSoilModel)
        {
            if (!IsSumOfAllProbabilitiesEqualToOne(stochasticSoilModel))
            {
                Log.WarnFormat(Resources.StochasticSoilModelImporter_ValidateStochasticSoilModel_Sum_of_probabilities_of_stochastic_soil_model_0_is_not_correct,
                               stochasticSoilModel.Name);
            }
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