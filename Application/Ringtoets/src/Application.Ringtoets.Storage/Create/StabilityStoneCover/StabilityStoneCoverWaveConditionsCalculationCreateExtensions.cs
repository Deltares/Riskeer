// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils.Extensions;
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
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityStoneCoverWaveConditionsCalculationEntity Create(this StabilityStoneCoverWaveConditionsCalculation calculation,
                                                                                  PersistenceRegistry registry, int order)
        {
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

            SetInputParameters(entity, calculation, registry);
            SetOutputEntities(entity, calculation);

            return entity;
        }

        private static void SetInputParameters(StabilityStoneCoverWaveConditionsCalculationEntity entity, StabilityStoneCoverWaveConditionsCalculation calculation, PersistenceRegistry registry)
        {
            if (calculation.InputParameters.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = calculation.InputParameters.HydraulicBoundaryLocation.Create(registry, 0);
            }
            if (calculation.InputParameters.ForeshoreProfile != null)
            {
                entity.ForeshoreProfileEntity = calculation.InputParameters.ForeshoreProfile.Create(registry, 0);
            }

            entity.Orientation = calculation.InputParameters.Orientation;
            entity.UseBreakWater = Convert.ToByte(calculation.InputParameters.UseBreakWater);
            entity.BreakWaterType = (byte) calculation.InputParameters.BreakWater.Type;
            entity.BreakWaterHeight = calculation.InputParameters.BreakWater.Height;
            entity.UseForeshore = Convert.ToByte(calculation.InputParameters.UseForeshore);
            entity.UpperBoundaryRevetment = calculation.InputParameters.UpperBoundaryRevetment;
            entity.LowerBoundaryRevetment = calculation.InputParameters.LowerBoundaryRevetment;
            entity.UpperBoundaryWaterLevels = calculation.InputParameters.UpperBoundaryWaterLevels;
            entity.LowerBoundaryWaterLevels = calculation.InputParameters.LowerBoundaryWaterLevels;
            entity.StepSize = Convert.ToByte(calculation.InputParameters.StepSize);
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