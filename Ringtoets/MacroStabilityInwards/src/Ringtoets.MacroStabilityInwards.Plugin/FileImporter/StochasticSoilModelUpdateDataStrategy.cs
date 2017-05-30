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
using System.Linq;
using Core.Common.Base;
using Core.Common.Utils;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service;

namespace Ringtoets.MacroStabilityInwards.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating stochastic soil models based on imported data.
    /// </summary>
    public class StochasticSoilModelUpdateDataStrategy : UpdateDataStrategyBase<StochasticSoilModel, MacroStabilityInwardsFailureMechanism>,
                                                         IStochasticSoilModelUpdateModelStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public StochasticSoilModelUpdateDataStrategy(MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(failureMechanism, new SoilModelNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateModelWithImportedData(StochasticSoilModelCollection targetDataCollection,
                                                                    IEnumerable<StochasticSoilModel> readStochasticSoilModels,
                                                                    string sourceFilePath)
        {
            return UpdateTargetCollectionData(targetDataCollection, readStochasticSoilModels, sourceFilePath);
        }

        #region Remove Data Functions

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(StochasticSoilModel removedModel)
        {
            return MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(FailureMechanism, removedModel);
        }

        #endregion

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

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(StochasticSoilModel soilModelToUpdate,
                                                                                 StochasticSoilModel soilModelToUpdateFrom)
        {
            return UpdateStochasticSoilModel(soilModelToUpdate, soilModelToUpdateFrom);
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(StochasticSoilModel modelToUpdate, StochasticSoilModel modelToUpdateFrom)
        {
            Dictionary<StochasticSoilProfile, MacroStabilityInwardsSoilProfile> oldProfiles = modelToUpdate
                .StochasticSoilProfiles
                .ToDictionary(ssp => ssp, ssp => ssp.SoilProfile, new ReferenceEqualityComparer<StochasticSoilProfile>());

            StochasticSoilModelProfileDifference difference = modelToUpdate.Update(modelToUpdateFrom);

            var affectedObjects = new List<IObservable>();
            foreach (StochasticSoilProfile removedProfile in difference.RemovedProfiles)
            {
                affectedObjects.AddRange(MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(FailureMechanism, removedProfile));
            }
            foreach (StochasticSoilProfile updatedProfile in difference.UpdatedProfiles)
            {
                if (!oldProfiles[updatedProfile].Equals(updatedProfile.SoilProfile))
                {
                    affectedObjects.AddRange(MacroStabilityInwardsDataSynchronizationService.ClearStochasticSoilProfileDependentData(FailureMechanism, updatedProfile));
                }
                affectedObjects.Add(updatedProfile);
            }
            return affectedObjects;
        }

        #endregion
    }
}