// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service;

namespace Riskeer.Piping.Plugin.FileImporter
{
    /// <summary>
    /// An <see cref="UpdateDataStrategyBase{TTargetData,TFailureMechanism}"/> for updating stochastic soil models based on imported data.
    /// </summary>
    public class PipingStochasticSoilModelUpdateDataStrategy : UpdateDataStrategyBase<PipingStochasticSoilModel, PipingFailureMechanism>,
                                                               IStochasticSoilModelUpdateModelStrategy<PipingStochasticSoilModel>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingStochasticSoilModelUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public PipingStochasticSoilModelUpdateDataStrategy(PipingFailureMechanism failureMechanism)
            : base(failureMechanism, failureMechanism?.StochasticSoilModels, new SoilModelNameEqualityComparer()) {}

        public IEnumerable<IObservable> UpdateModelWithImportedData(IEnumerable<PipingStochasticSoilModel> stochasticSoilModels, string sourceFilePath)
        {
            return UpdateTargetCollectionData(stochasticSoilModels, sourceFilePath);
        }

        #region Remove Data Functions

        protected override IEnumerable<IObservable> RemoveObjectAndDependentData(PipingStochasticSoilModel removedModel)
        {
            return PipingDataSynchronizationService.RemoveStochasticSoilModel(FailureMechanism, removedModel);
        }

        #endregion

        /// <summary>
        /// Class for comparing <see cref="PipingStochasticSoilModel"/> by just the name.
        /// </summary>
        private class SoilModelNameEqualityComparer : IEqualityComparer<PipingStochasticSoilModel>
        {
            public bool Equals(PipingStochasticSoilModel x, PipingStochasticSoilModel y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(PipingStochasticSoilModel obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        #region Update Data Functions

        protected override IEnumerable<IObservable> UpdateObjectAndDependentData(PipingStochasticSoilModel soilModelToUpdate,
                                                                                 PipingStochasticSoilModel soilModelToUpdateFrom)
        {
            return UpdateStochasticSoilModel(soilModelToUpdate, soilModelToUpdateFrom);
        }

        private IEnumerable<IObservable> UpdateStochasticSoilModel(PipingStochasticSoilModel modelToUpdate, PipingStochasticSoilModel modelToUpdateFrom)
        {
            Dictionary<PipingStochasticSoilProfile, PipingSoilProfile> oldProfiles = modelToUpdate.StochasticSoilProfiles
                                                                                                  .ToDictionary(ssp => ssp, ssp => ssp.SoilProfile,
                                                                                                                new ReferenceEqualityComparer<PipingStochasticSoilProfile>());

            PipingStochasticSoilModelProfileDifference difference = modelToUpdate.Update(modelToUpdateFrom);

            var affectedObjects = new List<IObservable>();
            foreach (PipingStochasticSoilProfile removedProfile in difference.RemovedProfiles)
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.RemoveStochasticSoilProfileFromInput(FailureMechanism, removedProfile));
            }

            foreach (PipingStochasticSoilProfile updatedProfile in difference.UpdatedProfiles)
            {
                if (!oldProfiles[updatedProfile].Equals(updatedProfile.SoilProfile))
                {
                    affectedObjects.AddRange(PipingDataSynchronizationService.ClearStochasticSoilProfileDependentData(FailureMechanism, updatedProfile));
                }

                affectedObjects.Add(updatedProfile);
            }

            return affectedObjects;
        }

        #endregion
    }
}