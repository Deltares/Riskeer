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
using System.Linq;
using Ringtoets.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.DuneErosion
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="DuneLocationCalculation"/> based on the
    /// <see cref="DuneLocationCalculationEntity"/>.
    /// </summary>
    internal static class DuneLocationCalculationEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="DuneLocationCalculationEntity"/> and uses the information to update a 
        /// <see cref="DuneLocationCalculation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationCalculationEntity"/> to update the
        /// <see cref="DuneLocationCalculation"/>.</param>
        /// <param name="calculation">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this DuneLocationCalculationEntity entity, DuneLocationCalculation calculation)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            DuneLocationCalculationOutputEntity duneLocationCalculationOutputEntity = entity.DuneLocationCalculationOutputEntities.SingleOrDefault();
            if (duneLocationCalculationOutputEntity != null)
            {
                calculation.Output = duneLocationCalculationOutputEntity.Read();
            }
        }
    }
}