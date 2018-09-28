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
using System.Collections.Generic;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for collections of <see cref="HydraulicBoundaryLocationCalculation"/> related to creating a 
    /// <see cref="HydraulicLocationCalculationCollectionEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryLocationCalculationCollectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicLocationCalculationCollectionEntity"/> based on the information
        /// of the <paramref name="calculations"/>.
        /// </summary>
        /// <param name="calculations">The collection of <see cref="HydraulicBoundaryLocationCalculation"/>
        /// to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="HydraulicLocationCalculationCollectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static HydraulicLocationCalculationCollectionEntity Create(this IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
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

            var collectionEntity = new HydraulicLocationCalculationCollectionEntity();

            foreach (HydraulicBoundaryLocationCalculation calculation in calculations)
            {
                collectionEntity.HydraulicLocationCalculationEntities.Add(calculation.Create(registry));
            }

            return collectionEntity;
        }
    }
}