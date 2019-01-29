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
using Ringtoets.Common.Data;

namespace Riskeer.Common.IO.SurfaceLines
{
    /// <summary>
    /// Configuration of the used components in <see cref="SurfaceLinesCsvImporter{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the mechanism specific surface line.</typeparam>
    public class SurfaceLinesCsvImporterConfiguration<T> where T : IMechanismSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="SurfaceLinesCsvImporterConfiguration{T}"/>.
        /// </summary>
        /// <param name="transformer">The transformer to use in this configuration.</param>
        /// <param name="updateStrategy">The strategy to use in this configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="transformer"/>
        /// or <paramref name="updateStrategy"/> is <c>null</c>.</exception>
        public SurfaceLinesCsvImporterConfiguration(ISurfaceLineTransformer<T> transformer, ISurfaceLineUpdateDataStrategy<T> updateStrategy)
        {
            if (updateStrategy == null)
            {
                throw new ArgumentNullException(nameof(updateStrategy));
            }

            if (transformer == null)
            {
                throw new ArgumentNullException(nameof(transformer));
            }

            UpdateStrategy = updateStrategy;
            Transformer = transformer;
        }

        /// <summary>
        /// Gets the strategy for updating the data model with the mechanism specific surface lines.
        /// </summary>
        public ISurfaceLineUpdateDataStrategy<T> UpdateStrategy { get; }

        /// <summary>
        /// Gets the transformer used for transforming generic surface lines into mechanism specific
        /// surface lines.
        /// </summary>
        public ISurfaceLineTransformer<T> Transformer { get; }
    }
}