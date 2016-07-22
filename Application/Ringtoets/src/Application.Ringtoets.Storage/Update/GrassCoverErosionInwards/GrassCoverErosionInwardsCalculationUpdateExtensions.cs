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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;

using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Update.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsCalculation"/> related
    /// to updating a <see cref="GrassCoverErosionInwardsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsCalculationUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="GrassCoverErosionInwardsCalculationEntity"/> in
        /// the database based on the information of the <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The Grass Cover Erosion Inwards calculation.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <param name="order">The index in the parent collection where <paramref name="calculation"/>
        /// resides.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="calculation"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this GrassCoverErosionInwardsCalculation calculation, PersistenceRegistry registry, IRingtoetsEntities context, int order)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            GrassCoverErosionInwardsCalculationEntity entity = calculation.GetCorrespondingEntity(
                context.GrassCoverErosionInwardsCalculationEntities,
                inputEntity => inputEntity.GrassCoverErosionInwardsCalculationEntityId);
            entity.Name = calculation.Name;
            entity.Comments = calculation.Comments;
            entity.Order = order;

            UpdateInput(entity, calculation.InputParameters, registry, context);
            UpdateOutput(entity, calculation, registry, context);

            registry.Register(entity, calculation);
        }

        private static void UpdateInput(GrassCoverErosionInwardsCalculationEntity entity, GrassCoverErosionInwardsInput input, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            UpdateDikeProfile(entity, input.DikeProfile, registry, context);
            UpdateHydraulicBoundaryLocation(entity, input.HydraulicBoundaryLocation, registry, context);

            entity.Orientation = input.Orientation.Value.ToNaNAsNull();
            entity.CriticalFlowRateMean = input.CriticalFlowRate.Mean.Value.ToNaNAsNull();
            entity.CriticalFlowRateStandardDeviation = input.CriticalFlowRate.StandardDeviation.Value.ToNaNAsNull();
            entity.UseForeshore = Convert.ToByte(input.UseForeshore);
            entity.DikeHeight = input.DikeHeight.Value.ToNaNAsNull();
            entity.UseBreakWater = Convert.ToByte(input.UseBreakWater);
            entity.BreakWaterType = (short)input.BreakWater.Type;
            entity.BreakWaterHeight = input.BreakWater.Height.Value.ToNaNAsNull();
            entity.CalculateDikeHeight = Convert.ToByte(input.CalculateDikeHeight);
        }

        private static void UpdateDikeProfile(GrassCoverErosionInwardsCalculationEntity entity, DikeProfile dikeProfile, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (dikeProfile != null)
            {
                if (dikeProfile.IsNew())
                {
                    entity.DikeProfileEntity = dikeProfile.Create(registry);
                }
                else
                {
                    dikeProfile.Update(registry, context);
                }
            }
            else
            {
                entity.DikeProfileEntity = null;
            }
        }

        private static void UpdateHydraulicBoundaryLocation(GrassCoverErosionInwardsCalculationEntity entity, HydraulicBoundaryLocation hydraulicBoundaryLocation, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (hydraulicBoundaryLocation != null)
            {
                if (hydraulicBoundaryLocation.IsNew())
                {
                    entity.HydraulicLocationEntity = hydraulicBoundaryLocation.Create(registry);
                }
                else
                {
                    hydraulicBoundaryLocation.Update(registry, context);
                }
            }
            else
            {
                entity.HydraulicLocationEntity = null;
            }
        }

        private static void UpdateOutput(GrassCoverErosionInwardsCalculationEntity entity, GrassCoverErosionInwardsCalculation calculation, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (calculation.HasOutput)
            {
                if (calculation.Output.IsNew())
                {
                    entity.GrassCoverErosionInwardsOutputEntity = calculation.Output.Create(registry);
                }
                else
                {
                    calculation.Output.Update(registry, context);
                }
            }
            else
            {
                entity.GrassCoverErosionInwardsOutputEntity = null;
            }
        }
    }
}