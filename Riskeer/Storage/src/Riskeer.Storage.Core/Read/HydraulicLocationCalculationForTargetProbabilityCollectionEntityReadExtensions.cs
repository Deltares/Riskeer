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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>
    /// based on the <see cref="HydraulicLocationCalculationForTargetProbabilityCollectionEntity"/>.
    /// </summary>
    internal static class HydraulicLocationCalculationForTargetProbabilityCollectionEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HydraulicLocationCalculationForTargetProbabilityCollectionEntity"/> and uses 
        /// the information to create a <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationCalculationForTargetProbabilityCollectionEntity"/> to create the 
        /// <see cref="HydraulicBoundaryLocationCalculationsForTargetProbability"/>.</param>
        /// <param name="collector">The object keeping track of the read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static HydraulicBoundaryLocationCalculationsForTargetProbability Read(
            this HydraulicLocationCalculationForTargetProbabilityCollectionEntity entity,
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

            var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(entity.TargetProbability);
            IEnumerable<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations = 
                entity.HydraulicLocationCalculationEntities
                      .Select(hlce => CreateHydraulicBoundaryLocationCalculation(hlce, collector))
                      .ToArray();
            calculations.HydraulicBoundaryLocationCalculations.AddRange(hydraulicBoundaryLocationCalculations);
            
            return calculations;
        }

        private static HydraulicBoundaryLocationCalculation CreateHydraulicBoundaryLocationCalculation(HydraulicLocationCalculationEntity calculationEntity, 
                                                                                                       ReadConversionCollector collector)
        {
            var calculation = new HydraulicBoundaryLocationCalculation(collector.Get(calculationEntity.HydraulicLocationEntity));
            calculationEntity.Read(calculation);
            return calculation;
        }
    }
}