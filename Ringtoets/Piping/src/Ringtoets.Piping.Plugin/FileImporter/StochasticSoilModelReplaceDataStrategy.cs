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
using Core.Common.Base;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for replacing the stochastic soil models with the imported stochastic soil models. 
    /// </summary>
    public class StochasticSoilModelReplaceDataStrategy : ReplaceDataStrategyBase<StochasticSoilModel, PipingFailureMechanism>,
                                                          IStochasticSoilModelUpdateModelStrategy
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelUpdateDataStrategy"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism in which the models are updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public StochasticSoilModelReplaceDataStrategy(PipingFailureMechanism failureMechanism) : base(failureMechanism) {}

        public IEnumerable<IObservable> UpdateModelWithImportedData(StochasticSoilModelCollection targetCollection,
                                                                    IEnumerable<StochasticSoilModel> readStochasticSoilModels,
                                                                    string sourceFilePath)
        {
            try
            {
                return ReplaceTargetCollectionWithImportedData(targetCollection, readStochasticSoilModels, sourceFilePath);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException e)
            {
                throw new StochasticSoilModelUpdateException(e.Message, e);
            }
        }

        protected override IEnumerable<IObservable> ClearData(PipingFailureMechanism failureMechanism)
        {
            return PipingDataSynchronizationService.RemoveAllStochasticSoilModels(failureMechanism);
        }
    }
}