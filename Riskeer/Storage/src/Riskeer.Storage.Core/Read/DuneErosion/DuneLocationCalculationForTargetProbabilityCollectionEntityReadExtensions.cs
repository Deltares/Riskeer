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
using System.Collections.Generic;
using System.Linq;
using Riskeer.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.DuneErosion
{
    /// <summary>
    /// This class defines extension methods for read operations for a collection of 
    /// <see cref="DuneLocationCalculation"/> based on the <see cref="DuneLocationCalculationForTargetProbabilityCollectionEntity"/>.
    /// </summary>
    internal static class DuneLocationCalculationForTargetProbabilityCollectionEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="DuneLocationCalculationForTargetProbabilityCollectionEntity"/> and uses the information 
        /// to update a collection of <see cref="DuneLocationCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationCalculationForTargetProbabilityCollectionEntity"/> to create the 
        /// <see cref="DuneLocationCalculationsForTargetProbability"/>.</param>
        /// <param name="collector">The object keeping track of the read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static DuneLocationCalculationsForTargetProbability Read(this DuneLocationCalculationForTargetProbabilityCollectionEntity entity,
                                                                          ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var calculations = new DuneLocationCalculationsForTargetProbability(entity.TargetProbability);
            IEnumerable<DuneLocationCalculation> duneLocationCalculations =
                entity.DuneLocationCalculationEntities
                      .Select(dlce => CreateHydraulicBoundaryLocationCalculation(dlce, collector))
                      .ToArray();
            calculations.DuneLocationCalculations.AddRange(duneLocationCalculations);

            return calculations;
        }

        private static DuneLocationCalculation CreateHydraulicBoundaryLocationCalculation(DuneLocationCalculationEntity calculationEntity,
                                                                                          ReadConversionCollector collector)
        {
            var calculation = new DuneLocationCalculation(collector.Get(calculationEntity.DuneLocationEntity));
            calculationEntity.Read(calculation);
            return calculation;
        }
    }
}