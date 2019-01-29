// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
    /// <see cref="DuneLocationCalculation"/> based on the <see cref="DuneLocationCalculationCollectionEntity"/>.
    /// </summary>
    internal static class DuneLocationCalculationCollectionEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="DuneLocationCalculationCollectionEntity"/> and uses the information 
        /// to update a collection of <see cref="DuneLocationCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationCalculationEntity"/> to update the 
        /// <see cref="DuneLocationCalculation"/>.</param>
        /// <param name="calculations">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of the read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this DuneLocationCalculationCollectionEntity entity,
                                  IEnumerable<DuneLocationCalculation> calculations,
                                  ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (calculations == null)
            {
                throw new ArgumentNullException(nameof(calculations));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            Dictionary<DuneLocation, DuneLocationCalculation> calculationsLookup =
                calculations.ToDictionary(calc => calc.DuneLocation, calc => calc);

            foreach (DuneLocationCalculationEntity calculationEntity in entity.DuneLocationCalculationEntities)
            {
                DuneLocation duneLocation = collector.Get(calculationEntity.DuneLocationEntity);
                DuneLocationCalculation calculation = calculationsLookup[duneLocation];

                calculationEntity.Read(calculation);
            }
        }
    }
}