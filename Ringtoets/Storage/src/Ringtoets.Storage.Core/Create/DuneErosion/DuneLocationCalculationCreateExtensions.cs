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

using System;
using Ringtoets.DuneErosion.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.DuneErosion
{
    /// <summary>
    /// Extension methods for <see cref="DuneLocationCalculation"/> related to creating a <see cref="DuneLocationCalculationEntity"/>.
    /// </summary>
    internal static class DuneLocationCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="DuneLocationCalculationEntity"/> based on the information of the <see cref="DuneLocationCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="DuneLocationCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static DuneLocationCalculationEntity Create(this DuneLocationCalculation calculation, PersistenceRegistry registry)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var duneLocationCalculationEntity = new DuneLocationCalculationEntity
            {
                DuneLocationEntity = registry.Get(calculation.DuneLocation)
            };
            CreateDuneLocationOutput(duneLocationCalculationEntity, calculation.Output);

            return duneLocationCalculationEntity;
        }

        private static void CreateDuneLocationOutput(DuneLocationCalculationEntity calculationEntity,
                                                     DuneLocationCalculationOutput output)
        {
            if (output != null)
            {
                calculationEntity.DuneLocationCalculationOutputEntities.Add(output.Create());
            }
        }
    }
}