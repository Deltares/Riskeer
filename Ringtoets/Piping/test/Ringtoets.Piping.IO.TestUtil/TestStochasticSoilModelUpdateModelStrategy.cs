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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Importers;

namespace Ringtoets.Piping.IO.TestUtil
{
    /// <summary>
    /// Implementation of a <see cref="IStochasticSoilModelUpdateModelStrategy"/> which can be used for
    /// testing.
    /// </summary>
    public class TestStochasticSoilModelUpdateModelStrategy : IStochasticSoilModelUpdateModelStrategy
    {
        /// <summary>
        /// Gets a value which indicates whether <see cref="UpdateModelWithImportedData"/> has
        /// been called.
        /// </summary>
        public bool Updated { get; private set; }

        /// <summary>
        /// Gets the models that were passed to <see cref="UpdateModelWithImportedData"/> as the read models.
        /// </summary>
        public StochasticSoilModel[] ReadModels { get; private set; }

        /// <summary>
        /// Gets the file path that was passed to <see cref="UpdateModelWithImportedData"/>.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Gets or sets the instances that will be returned by <see cref="UpdateModelWithImportedData"/>.
        /// </summary>
        public IEnumerable<IObservable> UpdatedInstances { get; set; } = Enumerable.Empty<IObservable>();

        public IEnumerable<IObservable> UpdateModelWithImportedData(StochasticSoilModelCollection targetDataCollection,
                                                                    IEnumerable<StochasticSoilModel> readStochasticSoilModels,
                                                                    string sourceFilePath)
        {
            Updated = true;
            EvaluateGetValidStochasticSoilModelsMethod(readStochasticSoilModels);
            FilePath = sourceFilePath;

            return UpdatedInstances;
        }

        private void EvaluateGetValidStochasticSoilModelsMethod(IEnumerable<StochasticSoilModel> readStochasticSoilModels)
        {
            ReadModels = readStochasticSoilModels.ToArray();
        }
    }
}