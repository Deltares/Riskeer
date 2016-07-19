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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Update.Piping;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Piping.Data;

using Application.Ringtoets.Storage.Update.GrassCoverErosionInwards;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="CalculationGroup"/> related to updating an <see cref="CalculationGroupEntity"/>.
    /// </summary>
    internal static class CalculationGroupUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="CalculationGroupEntity"/> in the database based on the
        /// information of the <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="calculationGroup">The calculation group to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <param name="order">The index at which <paramref name="calculationGroup"/> resides
        /// in the parent container.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="calculationGroup"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this CalculationGroup calculationGroup, PersistenceRegistry registry, IRingtoetsEntities context, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            CalculationGroupEntity entity = calculationGroup.GetCorrespondingEntity(context.CalculationGroupEntities, o => o.CalculationGroupEntityId);
            entity.Name = calculationGroup.Name;
            entity.Order = order;

            UpdateChildren(entity, calculationGroup, registry, context);

            registry.Register(entity, calculationGroup);
        }

        private static void UpdateChildren(CalculationGroupEntity entity, CalculationGroup calculationGroup, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            for (int i = 0; i < calculationGroup.Children.Count; i++)
            {
                ICalculationBase calculationBase = calculationGroup.Children[i];
                UpdateIfCalculationGroup(entity, registry, context, calculationBase, i);

                UpdateIfPipingCalculation(entity, registry, context, calculationBase, i);

                UpdateIfGrassCoverErosionInwardsCalculation(entity, registry, context, calculationBase, i);
            }
        }

        private static void UpdateIfCalculationGroup(CalculationGroupEntity entity,
                                                     PersistenceRegistry registry,
                                                     IRingtoetsEntities context,
                                                     ICalculationBase calculationBase,
                                                     int order)
        {
            var childGroup = calculationBase as CalculationGroup;
            if (childGroup != null)
            {
                if (childGroup.IsNew())
                {
                    entity.CalculationGroupEntity1.Add(childGroup.Create(registry, order));
                }
                else
                {
                    childGroup.Update(registry, context, order);
                    CalculationGroupEntity childGroupEntity = context.CalculationGroupEntities.First(cge => cge.CalculationGroupEntityId == childGroup.StorageId);
                    if (!entity.CalculationGroupEntity1.Contains(childGroupEntity))
                    {
                        childGroupEntity.CalculationGroupEntity2.CalculationGroupEntity1.Remove(childGroupEntity);
                        entity.CalculationGroupEntity1.Add(childGroupEntity);
                    }
                }
            }
        }

        private static void UpdateIfPipingCalculation(CalculationGroupEntity entity,
                                                       PersistenceRegistry registry,
                                                       IRingtoetsEntities context,
                                                       ICalculationBase calculationBase,
                                                       int order)
        {
            var childCalculation = calculationBase as PipingCalculationScenario;
            if (childCalculation != null)
            {
                if (childCalculation.IsNew())
                {
                    entity.PipingCalculationEntities.Add(childCalculation.Create(registry, order));
                }
                else
                {
                    childCalculation.Update(registry, context, order);
                    PipingCalculationEntity childCalculationEntity = context.PipingCalculationEntities.First(cge => cge.PipingCalculationEntityId == childCalculation.StorageId);
                    if (!entity.PipingCalculationEntities.Contains(childCalculationEntity))
                    {
                        childCalculationEntity.CalculationGroupEntity.PipingCalculationEntities.Remove(childCalculationEntity);
                        entity.PipingCalculationEntities.Add(childCalculationEntity);
                    }
                }
            }
        }

        private static void UpdateIfGrassCoverErosionInwardsCalculation(CalculationGroupEntity entity,
                                                                         PersistenceRegistry registry,
                                                                         IRingtoetsEntities context,
                                                                         ICalculationBase calculationBase,
                                                                         int order)
        {
            var childCalculation = calculationBase as GrassCoverErosionInwardsCalculation;
            if (childCalculation != null)
            {
                if (childCalculation.IsNew())
                {
                    entity.GrassCoverErosionInwardsCalculationEntities.Add(childCalculation.Create(registry, order));
                }
                else
                {
                    childCalculation.Update(registry, context, order);
                    GrassCoverErosionInwardsCalculationEntity childCalculationEntity = context.GrassCoverErosionInwardsCalculationEntities.First(cge => cge.GrassCoverErosionInwardsCalculationEntityId == childCalculation.StorageId);
                    if (!entity.GrassCoverErosionInwardsCalculationEntities.Contains(childCalculationEntity))
                    {
                        childCalculationEntity.CalculationGroupEntity.GrassCoverErosionInwardsCalculationEntities.Remove(childCalculationEntity);
                        entity.GrassCoverErosionInwardsCalculationEntities.Add(childCalculationEntity);
                    }
                }
            }
        }
    }
}