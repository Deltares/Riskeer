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
using Core.Common.Utils;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Plugin.Properties;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for updating the current stochastic soil models with the imported stochastic soil models.
    /// <list type="bullet">
    /// <item>Adds stochastic soil models that are imported and are not part of current stochastic soil model collection.</item>
    /// <item>Removes stochastic soil models that are part of the current stochastic soil model collection, but were not
    /// amongst the imported stochastic soil models.
    /// </item>
    /// <item>Updates stochastic soil models that are part of the current stochastic soil model collection and are also
    /// imported.</item>
    /// </list>
    /// </summary>
    public class StochasticSoilModelUpdateDataStrategy : UpdateDataStrategyBase<StochasticSoilModel, string, PipingFailureMechanism>,
                                                         IStochasticSoilModelUpdateModelStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public StochasticSoilModelUpdateDataStrategy(PipingFailureMechanism failureMechanism)
            : base(failureMechanism, new SoilModelNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateModelWithImportedData(StochasticSoilModelCollection targetCollection,
                                                                    IEnumerable<StochasticSoilModel> readStochasticSoilModels,
                                                                    string sourceFilePath)
        {
            try
            {
                return UpdateTargetCollectionData(targetCollection, readStochasticSoilModels, sourceFilePath);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException e)
            {
                throw new StochasticSoilModelUpdateException(e.Message, e);
            }
            catch (InvalidOperationException e)
            {
                string message = Resources.StochasticSoilModelUpdateDataStrategy_UpdateModelWithImportedData_Update_of_StochasticSoilModel_failed;
                throw new StochasticSoilModelUpdateException(message, e);
            }
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

        #region Update Data Functions

        protected override IEnumerable<IObservable> UpdateData(IEnumerable<StochasticSoilModel> objectsToUpdate,
                                                               IEnumerable<StochasticSoilModel> importedDataCollection)
        {
            var affectedObjects = new List<IObservable>();
            foreach (StochasticSoilModel updatedModel in objectsToUpdate)
            {
                affectedObjects.Add(updatedModel);
                StochasticSoilModel readModel = importedDataCollection.Single(r => r.Name.Equals(updatedModel.Name));
                affectedObjects.AddRange(UpdateStochasticSoilModel(updatedModel, readModel));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(StochasticSoilModel existingModel, StochasticSoilModel readModel)
        {
            Dictionary<StochasticSoilProfile, PipingSoilProfile> oldProfiles = existingModel
                .StochasticSoilProfiles
                .ToDictionary(ssp => ssp, ssp => ssp.SoilProfile, new ReferenceEqualityComparer<StochasticSoilProfile>());

            StochasticSoilModelProfileDifference difference = existingModel.Update(readModel);

            var affectedObjects = new List<IObservable>();
            foreach (StochasticSoilProfile removedProfile in difference.RemovedProfiles)
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(failureMechanism, removedProfile));
            }
            foreach (StochasticSoilProfile updatedProfile in difference.UpdatedProfiles)
            {
                if (!oldProfiles[updatedProfile].Equals(updatedProfile.SoilProfile))
                {
                    affectedObjects.AddRange(PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(failureMechanism, updatedProfile));
                }
            }
            return affectedObjects;
        }

        #endregion

        #region Remove Data Functions

        protected override IEnumerable<IObservable> RemoveData(IEnumerable<StochasticSoilModel> removedObjects)
        {
            var affectedObjects = new List<IObservable>();

            foreach (StochasticSoilModel model in removedObjects)
            {
                affectedObjects.AddRange(ClearStochasticSoilModelDependentData(model));
            }
            return affectedObjects;
        }

        private IEnumerable<IObservable> ClearStochasticSoilModelDependentData(StochasticSoilModel removedModel)
        {
            return PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, removedModel);
        }

        #endregion
    }
}