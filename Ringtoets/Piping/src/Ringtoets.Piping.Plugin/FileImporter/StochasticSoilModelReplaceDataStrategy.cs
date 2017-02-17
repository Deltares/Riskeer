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
using Core.Common.Base.Storage;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for replacing the stochastic soil models with the imported stochastic soil models. 
    /// </summary>
    public class StochasticSoilModelReplaceDataStrategy : IStochasticSoilModelUpdateModelStrategy
    {
        private readonly PipingFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        public StochasticSoilModelReplaceDataStrategy(PipingFailureMechanism failureMechanism)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }
            this.failureMechanism = failureMechanism;
        }

        public IEnumerable<IObservable> UpdateModelWithImportedData(StochasticSoilModelCollection targetCollection,
                                                                    IEnumerable<StochasticSoilModel> readStochasticSoilModels,
                                                                    string sourceFilePath)
        {
            if (targetCollection == null)
            {
                throw new ArgumentNullException(nameof(targetCollection));
            }
            if (readStochasticSoilModels == null)
            {
                throw new ArgumentNullException(nameof(readStochasticSoilModels));
            }
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }

            var affectedObjects = new List<IObservable>
            {
                targetCollection
            };

            foreach (StochasticSoilModel model in targetCollection.ToArray())
            {
                affectedObjects.AddRange(PipingDataSynchronizationService.RemoveStochasticSoilModel(failureMechanism, model));
            }
            try
            {
                targetCollection.AddRange(readStochasticSoilModels.ToList(), sourceFilePath);
            }
            catch (ArgumentException e)
            {
                throw new StochasticSoilModelUpdateException(e.Message, e);
            }
            return affectedObjects.Distinct();
        }
    }
}