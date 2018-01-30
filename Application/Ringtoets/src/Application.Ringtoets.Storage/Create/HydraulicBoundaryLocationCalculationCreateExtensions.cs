﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    ///  Extension methods for <see cref="HydraulicBoundaryLocationCalculation"/> related 
    /// to creating a <see cref="HydraulicLocationCalculationEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryLocationCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicLocationCalculationEntity"/> based on the information of 
        /// <see cref="HydraulicBoundaryLocationCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <returns>A new <see cref="HydraulicLocationCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// is <c>null</c>.</exception>
        internal static HydraulicLocationCalculationEntity Create(this HydraulicBoundaryLocationCalculation calculation)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            var hydraulicLocationCalculationEntity = new HydraulicLocationCalculationEntity
            {
                ShouldIllustrationPointsBeCalculated = Convert.ToByte(calculation.InputParameters.ShouldIllustrationPointsBeCalculated)
            };

            CreateHydraulicBoundaryLocationOutput(hydraulicLocationCalculationEntity, calculation.Output);

            return hydraulicLocationCalculationEntity;
        }

        private static void CreateHydraulicBoundaryLocationOutput(HydraulicLocationCalculationEntity hydraulicLocationCalculationEntity,
                                                                  HydraulicBoundaryLocationOutput output)
        {
            if (output != null)
            {
                hydraulicLocationCalculationEntity.HydraulicLocationOutputEntities.Add(output.CreateHydraulicLocationOutputEntity());
            }
        }
    }
}