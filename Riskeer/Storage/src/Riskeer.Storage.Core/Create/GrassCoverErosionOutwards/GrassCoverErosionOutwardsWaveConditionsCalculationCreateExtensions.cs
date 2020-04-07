// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Revetment.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverErosionOutwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/> related to creating a
    /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsWaveConditionsCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationEntity"/> based on the information of the 
        /// <see cref="GrassCoverErosionOutwardsWaveConditionsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="calculation"/> resides within its parent.</param>
        /// <returns>A new <see cref="GrassCoverErosionOutwardsWaveConditionsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static GrassCoverErosionOutwardsWaveConditionsCalculationEntity Create(this GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
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

            var entity = new GrassCoverErosionOutwardsWaveConditionsCalculationEntity
            {
                Order = order,
                Name = calculation.Name.DeepClone(),
                Comments = calculation.Comments.Body.DeepClone()
            };

            SetInputParameters(entity, calculation.InputParameters, registry);
            SetOutputEntities(entity, calculation);

            return entity;
        }

        private static void SetInputParameters(GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity,
                                               GrassCoverErosionOutwardsWaveConditionsInput calculationInput,
                                               PersistenceRegistry registry)
        {
            HydraulicBoundaryLocation hydraulicBoundaryLocation = calculationInput.HydraulicBoundaryLocation;
            if (hydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = registry.Get(hydraulicBoundaryLocation);
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
            entity.CalculationType = Convert.ToByte(calculationInput.CalculationType);
        }

        private static void SetOutputEntities(GrassCoverErosionOutwardsWaveConditionsCalculationEntity entity,
                                              GrassCoverErosionOutwardsWaveConditionsCalculation calculation)
        {
            if (calculation.HasOutput)
            {
                var i = 0;
                if (calculation.Output.WaveRunUpOutput != null)
                {
                    foreach (WaveConditionsOutput output in calculation.Output.WaveRunUpOutput)
                    {
                        entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.Add(output.CreateGrassCoverErosionOutwardsWaveConditionsOutputEntity(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveRunUp, i++));
                    }
                }

                if (calculation.Output.WaveImpactOutput != null)
                {
                    foreach (WaveConditionsOutput output in calculation.Output.WaveImpactOutput)
                    {
                        entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.Add(output.CreateGrassCoverErosionOutwardsWaveConditionsOutputEntity(GrassCoverErosionOutwardsWaveConditionsOutputType.WaveImpact, i++));
                    }
                }

                if (calculation.Output.TailorMadeWaveImpactOutput != null)
                {
                    foreach(WaveConditionsOutput output in calculation.Output.TailorMadeWaveImpactOutput)
                    {
                        entity.GrassCoverErosionOutwardsWaveConditionsOutputEntities.Add(output.CreateGrassCoverErosionOutwardsWaveConditionsOutputEntity(GrassCoverErosionOutwardsWaveConditionsOutputType.TailorMadeWaveImpact, i++));
                    }
                }
            }
        }
    }
}