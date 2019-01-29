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
using Riskeer.Common.Data;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// Configuration of the used components in <see cref="StochasticSoilModelImporterConfiguration{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the mechanism specific stochastic soil model.</typeparam>
    public class StochasticSoilModelImporterConfiguration<T> where T : IMechanismStochasticSoilModel
    {
        /// <summary>
        /// Creates a new instance of <see cref="StochasticSoilModelImporterConfiguration{T}"/>.
        /// </summary>
        /// <param name="transformer">The transformer to use in this configuration.</param>
        /// <param name="mechanismFilter">The failure mechanism filter.</param>
        /// <param name="updateStrategy">The strategy to use in this configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="transformer"/>
        /// or <paramref name="updateStrategy"/> is <c>null</c>.</exception>
        public StochasticSoilModelImporterConfiguration(IStochasticSoilModelTransformer<T> transformer,
                                                        IStochasticSoilModelMechanismFilter mechanismFilter,
                                                        IStochasticSoilModelUpdateModelStrategy<T> updateStrategy)
        {
            if (transformer == null)
            {
                throw new ArgumentNullException(nameof(transformer));
            }

            if (mechanismFilter == null)
            {
                throw new ArgumentNullException(nameof(mechanismFilter));
            }

            if (updateStrategy == null)
            {
                throw new ArgumentNullException(nameof(updateStrategy));
            }

            Transformer = transformer;
            MechanismFilter = mechanismFilter;
            UpdateStrategy = updateStrategy;
        }

        /// <summary>
        /// Gets the transformer used for transforming generic stochastic soil models into mechanism 
        /// specific stochastic soil models.
        /// </summary>
        public IStochasticSoilModelTransformer<T> Transformer { get; }

        /// <summary>
        /// Gets the failure mechanism filter to verify the stochastic soil model is valid 
        /// for the failure mechanism.
        /// </summary>
        public IStochasticSoilModelMechanismFilter MechanismFilter { get; }

        /// <summary>
        /// Gets the strategy for updating the data model with the mechanism specific stochastic 
        /// soil models.
        /// </summary>
        public IStochasticSoilModelUpdateModelStrategy<T> UpdateStrategy { get; }
    }
}