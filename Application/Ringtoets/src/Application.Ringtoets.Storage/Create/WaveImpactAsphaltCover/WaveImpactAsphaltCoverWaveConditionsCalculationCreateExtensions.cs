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
using Ringtoets.HydraRing.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Create.WaveImpactAsphaltCover
{
    /// <summary>
    /// Extension methods for <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/> related to creating a
    /// <see cref="WaveImpactAsphaltCoverWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class WaveImpactAsphaltCoverWaveConditionsCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="WaveImpactAsphaltCoverWaveConditionsCalculationEntity"/> based on the information of the 
        /// <see cref="WaveImpactAsphaltCoverWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="calculation"/> resides within its parent.</param>
        /// <returns>A new <see cref="WaveImpactAsphaltCoverWaveConditionsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static WaveImpactAsphaltCoverWaveConditionsCalculationEntity Create(this WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                                                                                     PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new WaveImpactAsphaltCoverWaveConditionsCalculationEntity
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

            HydraulicBoundaryLocation hydraulicBoundaryLocation = calculation.InputParameters.HydraulicBoundaryLocation;
            if (hydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry, 0);
            }
            if (calculation.InputParameters.ForeshoreProfile != null)
            {
                entity.ForeshoreProfileEntity = calculation.InputParameters.ForeshoreProfile.Create(registry, 0);
            }

            AddEntityForWaveImpactAsphaltCoverWaveConditionsOutput(calculation, registry, entity);

            return entity;
        }

        private static void AddEntityForWaveImpactAsphaltCoverWaveConditionsOutput(WaveImpactAsphaltCoverWaveConditionsCalculation calculation, PersistenceRegistry registry, WaveImpactAsphaltCoverWaveConditionsCalculationEntity entity)
        {
            if (calculation.HasOutput)
            {
                int i = 0;
                foreach (var waveConditionsOutput in calculation.Output.Items)
                {
                    entity.WaveImpactAsphaltCoverWaveConditionsOutputEntities.Add(waveConditionsOutput.CreateWaveImpactAsphaltCoverWaveConditionsOutputEntity(i++, registry));
                }
            }
        }
    }
}