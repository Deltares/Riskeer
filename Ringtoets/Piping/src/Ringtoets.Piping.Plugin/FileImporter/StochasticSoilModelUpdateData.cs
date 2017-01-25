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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for updating the current stochastic soil models with the imported stochastic soil models.
    /// </summary>
    public class StochasticSoilModelUpdateData : IStochasticSoilModelUpdateStrategy
    {
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

            return ModifyModelCollection(readStochasticSoilModels, targetCollection, sourceFilePath);
        }

        private IEnumerable<IObservable> ModifyModelCollection(IEnumerable<StochasticSoilModel> readStochasticSoilModels, StochasticSoilModelCollection targetCollection, string sourceFilePath)
        {
            var affectedObjects = new List<IObservable>
            {
                targetCollection
            };

            var readModelList = readStochasticSoilModels.ToList();
            var addedModels = GetAddedReadModels(targetCollection, readModelList).ToList();
            var updatedModels = GetUpdatedExistingModels(targetCollection, readModelList).ToList();
            var removedModels = GetRemovedExistingModels(targetCollection, readModelList).ToList();

            affectedObjects.AddRange(UpdateModels(updatedModels, readModelList));
            affectedObjects.AddRange(RemoveModels(removedModels));

            targetCollection.Clear();
            targetCollection.AddRange(addedModels.Union(updatedModels), sourceFilePath);

            return affectedObjects;
        }

        private static IEnumerable<StochasticSoilModel> GetAddedReadModels(IEnumerable<StochasticSoilModel> existingCollection, IEnumerable<StochasticSoilModel> readStochasticSoilModels)
        {
            return readStochasticSoilModels.Except(existingCollection, new SoilModelNameEqualityComparer());
        }

        private static IEnumerable<StochasticSoilModel> GetUpdatedExistingModels(IEnumerable<StochasticSoilModel> existingCollection, IEnumerable<StochasticSoilModel> readStochasticSoilModels)
        {
            return existingCollection.Intersect(readStochasticSoilModels, new SoilModelNameEqualityComparer());
        }

        private static IEnumerable<StochasticSoilModel> GetRemovedExistingModels(IEnumerable<StochasticSoilModel> existingCollection, IEnumerable<StochasticSoilModel> readStochasticSoilModels)
        {
            return existingCollection.Except(readStochasticSoilModels, new SoilModelNameEqualityComparer());
        }

        private IEnumerable<IObservable> RemoveModels(IEnumerable<StochasticSoilModel> removedModels)
        {
            var affectedObjects = new List<IObservable>();

            foreach (var model in removedModels)
            {
                affectedObjects.AddRange(ClearStochasticSoilModelDependentData(model));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateModels(IList<StochasticSoilModel> updatedModels, IList<StochasticSoilModel> readModels)
        {
            var affectedObjects = new List<IObservable>();
            foreach (StochasticSoilModel updatedModel in updatedModels)
            {
                StochasticSoilModel readModel = readModels.Single(r => r.Name.Equals(updatedModel.Name));
                affectedObjects.AddRange(UpdateStochasticSoilModel(updatedModel, readModel));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> ClearStochasticSoilModelDependentData(StochasticSoilModel removedModel)
        {
            return PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, removedModel);
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(StochasticSoilModel existingModel, StochasticSoilModel readModel)
        {
            StochasticSoilModelProfileDifference difference = existingModel.Update(readModel);

            var affectedObjects = new List<IObservable>();
            foreach (StochasticSoilProfile removedProfile in difference.RemovedProfiles)
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, removedProfile));
            }
            foreach (StochasticSoilProfile updatedProfile in difference.UpdatedProfiles)
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.UpdateStochasticSoilProfileForInput(failureMechanism, updatedProfile));
            }
            return affectedObjects;
        }

        /// <summary>
        /// Class for comparing <see cref="StochasticSoilModel"/> by just the name.
        /// </summary>
        private class SoilModelNameEqualityComparer : IEqualityComparer<StochasticSoilModel>
        {
            public bool Equals(StochasticSoilModel x, StochasticSoilModel y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(StochasticSoilModel obj)
            {
                return obj.Name.GetHashCode();
            }
        }
    }
}