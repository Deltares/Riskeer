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
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Plugin.FileImporter
{
    /// <summary>
    /// Interface describing the different methods of updating the data model after new stochastic soil models
    /// have been imported.
    /// </summary>
    public interface IStochasticSoilModelUpdateStrategy
    {
        /// <summary>
        /// Adds the imported data to the <paramref name="targetCollection"/>.
        /// </summary>
        /// <param name="readStochasticSoilModels">The stochastic soil models which were imported.</param>
        /// <param name="sourceFilePath">The path to the source file from which the soil models were imported.</param>
        /// <param name="targetCollection">The <see cref="StochasticSoilModelCollection"/> to which the imported data
        /// is added.</param>
        /// <param name="notifyProgress">An action to be used to notify progress changes.</param>
        void UpdateModelWithImportedData(
            IEnumerable<StochasticSoilModel> readStochasticSoilModels,
            string sourceFilePath,
            StochasticSoilModelCollection targetCollection,
            Action<string, int, int> notifyProgress);
    }
}