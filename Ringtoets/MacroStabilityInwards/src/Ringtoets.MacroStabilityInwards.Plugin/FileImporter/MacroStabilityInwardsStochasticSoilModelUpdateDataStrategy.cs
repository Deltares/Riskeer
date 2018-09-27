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
using System.Linq;
using Core.Common.Base;
using Core.Common.Util;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service;

namespace Ringtoets.MacroStabilityInwards.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating stochastic soil models based on imported data.
    /// </summary>
    public class MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy : UpdateDataStrategyBase<MacroStabilityInwardsStochasticSoilModel, MacroStabilityInwardsFailureMechanism>,
                                                                              IStochasticSoilModelUpdateModelStrategy<MacroStabilityInwardsStochasticSoilModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsStochasticSoilModelUpdateDataStrategy(MacroStabilityInwardsFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.StochasticSoilModels, new SoilModelNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateModelWithImportedData(IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels, string sourceFilePath)
        {
            return UpdateTargetCollectionData(stochasticSoilModels, sourceFilePath);
        }

        #region Remove Data Functions

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(MacroStabilityInwardsStochasticSoilModel removedModel)
        {
            return MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilModel(FailureMechanism, removedModel);
        }

        #endregion

        /// <summary>
        /// Class for comparing <see cref="MacroStabilityInwardsStochasticSoilModel"/> by just the name.
        /// </summary>
        private class SoilModelNameEqualityComparer : IEqualityComparer<MacroStabilityInwardsStochasticSoilModel>
        {
            public bool Equals(MacroStabilityInwardsStochasticSoilModel x, MacroStabilityInwardsStochasticSoilModel y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(MacroStabilityInwardsStochasticSoilModel obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #region Update Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(MacroStabilityInwardsStochasticSoilModel soilModelToUpdate,
                                                                                 MacroStabilityInwardsStochasticSoilModel soilModelToUpdateFrom)
        {
            return UpdateStochasticSoilModel(soilModelToUpdate, soilModelToUpdateFrom);
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(MacroStabilityInwardsStochasticSoilModel modelToUpdate, MacroStabilityInwardsStochasticSoilModel modelToUpdateFrom)
        {
            Dictionary<MacroStabilityInwardsStochasticSoilProfile, IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>> oldProfiles =
                modelToUpdate
                    .StochasticSoilProfiles
                    .ToDictionary(ssp => ssp, ssp => ssp.SoilProfile, new ReferenceEqualityComparer<MacroStabilityInwardsStochasticSoilProfile>());

            MacroStabilityInwardsStochasticSoilModelProfileDifference difference = modelToUpdate.Update(modelToUpdateFrom);

            var affectedObjects = new List<IObservable>();
            foreach (MacroStabilityInwardsStochasticSoilProfile removedProfile in difference.RemovedProfiles)
            {
                affectedObjects.AddRange(MacroStabilityInwardsDataSynchronizationService.RemoveStochasticSoilProfileFromInput(FailureMechanism, removedProfile));
            }

            foreach (MacroStabilityInwardsStochasticSoilProfile updatedProfile in difference.UpdatedProfiles)
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