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
using System.Linq;
using Core.Common.Base;
using log4net;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for updating the current stochastic soil models with the imported stochastic soil models.
    /// </summary>
    public class StochasticSoilModelUpdateData : IStochasticSoilModelUpdateStrategy
    {
        private readonly ILog log = LogManager.GetLogger(typeof(StochasticSoilModelUpdateData));
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelUpdateData"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        public StochasticSoilModelUpdateData(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.failureMechanism = failureMechanism;
        }

        /// <summary>
        /// Updates the <paramref name="targetCollection"/>. 
        /// Updates stochastic soil models in <paramref name="targetCollection"/> that are part of
        /// <paramref name="readStochasticSoilModels"/>.
        /// Adds stochastic soil models that are not in <paramref name="targetCollection"/>, but are part of 
        /// <paramref name="readStochasticSoilModels"/>.
        /// Removes stochastic soil models that are in <paramref name="targetCollection"/>, but are not part 
        /// of <paramref name="readStochasticSoilModels"/>.
        /// </summary>
        /// <param name="readStochasticSoilModels">The imported stochastic soil models.</param>
        /// <param name="sourceFilePath">The file path from which the <paramref name="readStochasticSoilModels"/>
        /// were imported.</param>
        /// <param name="targetCollection">The current collection of <see cref="StochasticSoilModel"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="targetCollection"/>
        /// contains multiple <see cref="StochasticSoilModel"/> with the same <see cref="StochasticSoilModel.Name"/>,
        /// and <paramref name="readStochasticSoilModels"/> also contains a <see cref="StochasticSoilModel"/> with
        /// the same name.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <returns>List of updated instances.</returns>
        public IEnumerable<IObservable> UpdateModelWithImportedData(
            IEnumerable<StochasticSoilModel> readStochasticSoilModels,
            string sourceFilePath,
            StochasticSoilModelCollection targetCollection)
        {
            if (readStochasticSoilModels == null)
            {
                throw new ArgumentNullException(nameof(readStochasticSoilModels));
            }
            if (targetCollection == null)
            {
                throw new ArgumentNullException(nameof(targetCollection));
            }

            var removedModels = targetCollection.ToList();
            var updatedOrAddedModels = new List<StochasticSoilModel>();
            var affectedObjects = new List<IObservable> { targetCollection };

            foreach (var readModel in readStochasticSoilModels)
            {
                var existingModel = targetCollection.SingleOrDefault(existing => existing.Name.Equals(readModel.Name));
                if (existingModel != null)
                {
                    StochasticSoilModelProfileDifference difference = existingModel.Update(readModel);
                    RemoveStochasticSoilProfilesFromInputs(difference, affectedObjects);

                    removedModels.Remove(existingModel);
                    updatedOrAddedModels.Add(existingModel);
                }
                else
                {
                    removedModels.Remove(readModel);
                    updatedOrAddedModels.Add(readModel);
                }
            }
            foreach (var model in removedModels)
            {
                RemoveStochasticSoilModel(model, affectedObjects);
            }
            targetCollection.Clear();
            targetCollection.AddRange(updatedOrAddedModels, sourceFilePath);

            return affectedObjects;
        }

        private void RemoveStochasticSoilModel(StochasticSoilModel removedModel, List<IObservable> affectedObjects)
        {
            affectedObjects.AddRange(PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, removedModel));
        }

        private void RemoveStochasticSoilProfilesFromInputs(StochasticSoilModelProfileDifference difference, List<IObservable> affectedObjects)
        {
            foreach (StochasticSoilProfile removedProfile in difference.RemovedProfiles)
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, removedProfile));
            }
        }
    }
}