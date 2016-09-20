﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityStoneCoverWaveConditionsCalculationEntity Create(this StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                                  PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new StabilityStoneCoverWaveConditionsCalculationEntity
            {
                Order = order,
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.DeepClone(),
                Orientation = calculation.InputParameters.Orientation,
                UseBreakWater = Convert.ToByte(calculation.InputParameters.UseBreakWater),
                BreakWaterType = (byte) calculation.InputParameters.BreakWater.Type,
                BreakWaterHeight = calculation.InputParameters.BreakWater.Height,
                UseForeshore = Convert.ToByte(calculation.InputParameters.UseForeshore),
                UpperBoundaryRevetment = calculation.InputParameters.UpperBoundaryRevetment,
                LowerBoundaryRevetment = calculation.InputParameters.LowerBoundaryRevetment,
                UpperBoundaryWaterLevels = calculation.InputParameters.UpperBoundaryWaterLevels,
                LowerBoundaryWaterLevels = calculation.InputParameters.LowerBoundaryWaterLevels,
                StepSize = Convert.ToByte(calculation.InputParameters.StepSize)
            };

            if (calculation.InputParameters.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = calculation.InputParameters.HydraulicBoundaryLocation.Create(registry, 0);
            }
            if (calculation.InputParameters.ForeshoreProfile != null)
            {
                entity.ForeshoreProfileEntity = calculation.InputParameters.ForeshoreProfile.Create(registry, 0);
            }

            if (calculation.HasOutput)
            {
                AddEntityForStabilityStoneCoverWaveConditionsOutput(calculation.Output, registry, entity);
            }

            return entity;
        }

        private static void AddEntityForStabilityStoneCoverWaveConditionsOutput(StabilityStoneCoverWaveConditionsOutput stabilityStoneCoverWaveConditionsOutputs,
                                                                                PersistenceRegistry registry,
                                                                                StabilityStoneCoverWaveConditionsCalculationEntity entity)
        {
            foreach (var output in stabilityStoneCoverWaveConditionsOutputs.BlocksOutput)
            {
                entity.StabilityStoneCoverWaveConditionsOutputEntities.Add(
                    output.CreateStabilityStoneCoverWaveConditionsOutputEntity(WaveConditionsOutputType.Blocks, registry));
            }
            foreach (var output in stabilityStoneCoverWaveConditionsOutputs.ColumnsOutput)
            {
                entity.StabilityStoneCoverWaveConditionsOutputEntities.Add(
                    output.CreateStabilityStoneCoverWaveConditionsOutputEntity(WaveConditionsOutputType.Columns, registry));
            }
        }
    }
}