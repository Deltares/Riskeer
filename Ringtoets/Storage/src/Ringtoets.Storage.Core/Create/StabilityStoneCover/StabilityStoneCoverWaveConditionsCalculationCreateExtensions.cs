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
using Core.Common.Util.Extensions;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Create.StabilityStoneCover
{
    /// <summary>
    /// Extension methods for <see cref="StabilityStoneCoverWaveConditionsCalculation"/> related to creating a
    /// <see cref="StabilityStoneCoverWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class StabilityStoneCoverWaveConditionsCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StabilityStoneCoverWaveConditionsCalculationEntity"/> based on the information of the 
        /// <see cref="StabilityStoneCoverWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="calculation"/> resides within its parent.</param>
        /// <returns>A new <see cref="StabilityStoneCoverWaveConditionsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static StabilityStoneCoverWaveConditionsCalculationEntity Create(this StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                                  PersistenceRegistry registry, int order)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                Order = order,
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone()
            };

            SetInputParameters(entity, calculation.InputParameters, registry);
            SetOutputEntities(entity, calculation);

            return entity;
        }

        private static void SetInputParameters(StabilityStoneCoverWaveConditionsCalculationEntity entity,
                                               AssessmentSectionCategoryWaveConditionsInput calculationInput,
                                               PersistenceRegistry registry)
        {
            if (calculationInput.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = calculationInput.HydraulicBoundaryLocation.Create(registry, 0);
            }
            if (calculationInput.ForeshoreProfile != null)
            {
                entity.ForeshoreProfileEntity = calculationInput.ForeshoreProfile.Create(registry, 0);
            }

            entity.Orientation = calculationInput.Orientation.ToNaNAsNull();
            entity.UseBreakWater = Convert.ToByte(calculationInput.UseBreakWater);
            entity.BreakWaterType = Convert.ToByte(calculationInput.BreakWater.Type);
            entity.BreakWaterHeight = calculationInput.BreakWater.Height.ToNaNAsNull();
            entity.UseForeshore = Convert.ToByte(calculationInput.UseForeshore);
            entity.UpperBoundaryRevetment = calculationInput.UpperBoundaryRevetment.ToNaNAsNull();
            entity.LowerBoundaryRevetment = calculationInput.LowerBoundaryRevetment.ToNaNAsNull();
            entity.UpperBoundaryWaterLevels = calculationInput.UpperBoundaryWaterLevels.ToNaNAsNull();
            entity.LowerBoundaryWaterLevels = calculationInput.LowerBoundaryWaterLevels.ToNaNAsNull();
            entity.StepSize = Convert.ToByte(calculationInput.StepSize);
            entity.CategoryType = Convert.ToByte(calculationInput.CategoryType);
        }

        private static void SetOutputEntities(StabilityStoneCoverWaveConditionsCalculationEntity entity, StabilityStoneCoverWaveConditionsCalculation calculation)
        {
            if (calculation.HasOutput)
            {
                var i = 0;
                foreach (WaveConditionsOutput output in calculation.Output.BlocksOutput)
                {
                    entity.StabilityStoneCoverWaveConditionsOutputEntities.Add(output.CreateStabilityStoneCoverWaveConditionsOutputEntity(WaveConditionsOutputType.Blocks, i++));
                }
                foreach (WaveConditionsOutput output in calculation.Output.ColumnsOutput)
                {
                    entity.StabilityStoneCoverWaveConditionsOutputEntities.Add(output.CreateStabilityStoneCoverWaveConditionsOutputEntity(WaveConditionsOutputType.Columns, i++));
                }
            }
        }
    }
}