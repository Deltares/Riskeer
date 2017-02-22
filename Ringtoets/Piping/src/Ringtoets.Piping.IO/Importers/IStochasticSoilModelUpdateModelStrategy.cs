﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.IO.Exceptions;

namespace Ringtoets.Piping.IO.Importers
{
    /// <summary>
    /// Interface describing the method of updating the data model after new stochastic soil models
    /// have been imported.
    /// </summary>
    public interface IStochasticSoilModelUpdateModelStrategy
    {
        /// <summary>
        /// Adds the imported data to the <paramref name="targetCollection"/>.
        /// </summary>
        /// <param name="targetCollection">The <see cref="ObservableUniqueItemCollectionWithSourcePath{TObject, TFeature}"/> to which the imported data
        /// is added.</param>
        /// <param name="readStochasticSoilModels">The stochastic soil models which were imported.</param>
        /// <param name="sourceFilePath">The path to the source file from which the soil models were imported.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="StochasticSoilModelUpdateException">Thrown when applying the strategy failed.
        /// <see cref="StochasticSoilModelUpdateException.InnerException"/> is set with the more detailed
        /// exception.</exception>
        /// <returns>A <see cref="IEnumerable{IObservable}"/> of updated instances.</returns>
        IEnumerable<IObservable> UpdateModelWithImportedData(
            StochasticSoilModelCollection targetCollection,
            IEnumerable<StochasticSoilModel> readStochasticSoilModels,
            string sourceFilePath);
    }
}