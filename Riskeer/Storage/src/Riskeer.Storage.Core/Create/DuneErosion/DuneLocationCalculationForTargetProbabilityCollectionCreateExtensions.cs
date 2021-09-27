// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.DuneErosion
{
    /// <summary>
    /// Extension methods for collections of <see cref="DuneLocationCalculation"/> related to creating a 
    /// <see cref="DuneLocationCalculationForTargetProbabilityCollectionEntity"/>.
    /// </summary>
    internal static class DuneLocationCalculationForTargetProbabilityCollectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="DuneLocationCalculationForTargetProbabilityCollectionEntity"/> based on the information
        /// of the <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="DuneLocationCalculationsForTargetProbability"/>
        /// to create a database entity for.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param> 
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="DuneLocationCalculationForTargetProbabilityCollectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> or <paramref name="registry"/> is <c>null</c>.</exception>
        internal static DuneLocationCalculationForTargetProbabilityCollectionEntity Create(this DuneLocationCalculationsForTargetProbability calculations,
                                                                                           int order,
                                                                                           PersistenceRegistry registry)
        {
            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var collectionEntity = new DuneLocationCalculationForTargetProbabilityCollectionEntity
            {
                TargetProbability = calculations.TargetProbability,
                Order = order
            };

            foreach (DuneLocationCalculation calculation in calculations.DuneLocationCalculations)
            {
                collectionEntity.DuneLocationCalculationEntities.Add(calculation.Create(registry));
            }

            return collectionEntity;
        }
    }
}