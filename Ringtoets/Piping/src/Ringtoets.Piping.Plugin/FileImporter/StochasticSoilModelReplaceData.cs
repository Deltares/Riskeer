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
using log4net;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Strategy for replacing the stochastic soil models with the imported stochastic soil models. 
    /// </summary>
    public class StochasticSoilModelReplaceData : IStochasticSoilModelUpdateStrategy
    {
        private readonly ILog log = LogManager.GetLogger(typeof(StochasticSoilModelReplaceData));

        /// <summary>
        /// Adds the <paramref name="readStochasticSoilModels"/> to the <paramref name="targetCollection"/>.
        /// </summary>
        /// <param name="readStochasticSoilModels">The imported stochastic soil models.</param>
        /// <param name="sourceFilePath">The file path from which the <paramref name="readStochasticSoilModels"/>
        /// were imported.</param>
        /// <param name="targetCollection">The current collection of <see cref="StochasticSoilModel"/>.</param>
        /// <param name="notifyProgress">An action to be used to notify progress changes.</param>
        public void UpdateModelWithImportedData(
            IEnumerable<StochasticSoilModel> readStochasticSoilModels,
            string sourceFilePath,
            StochasticSoilModelCollection targetCollection,
            Action<string, int, int> notifyProgress)
        {
            var modelsToAdd = new List<StochasticSoilModel>();
            foreach (StochasticSoilModel readStochasticSoilModel in readStochasticSoilModels)
            {
                var stochasticSoilModel = targetCollection.FirstOrDefault(ssm => ssm.Id == readStochasticSoilModel.Id);
                if (stochasticSoilModel != null)
                {
                    log.WarnFormat(Properties.Resources.PipingSoilProfilesImporter_AddImportedDataToModel_Stochastisch_soil_model_0_already_exists, stochasticSoilModel.Name);
                }
                modelsToAdd.Add(readStochasticSoilModel);
            }
            targetCollection.AddRange(modelsToAdd, sourceFilePath);
        }
    }
}