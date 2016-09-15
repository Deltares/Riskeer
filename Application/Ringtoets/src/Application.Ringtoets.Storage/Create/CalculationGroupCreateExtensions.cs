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
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.Create.StabilityStoneCover;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="CalculationGroup"/> related to creating a <see cref="CalculationGroupEntity"/>.
    /// </summary>
    internal static class CalculationGroupCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="CalculationGroupEntity"/> based on the information of the <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="group">The calculation group to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="group"/> resides within its parent.</param>
        /// <returns>A new <see cref="CalculationGroupEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static CalculationGroupEntity Create(this CalculationGroup group, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new CalculationGroupEntity
            {
                Name = group.Name.DeepClone(),
                IsEditable = Convert.ToByte(group.IsNameEditable),
                Order = order
            };
            CreateChildElements(group, entity, registry);

            return entity;
        }

        private static void CreateChildElements(CalculationGroup parentGroup, CalculationGroupEntity entity, PersistenceRegistry registry)
        {
            for (int i = 0; i < parentGroup.Children.Count; i++)
            {
                ICalculationBase calculationBase = parentGroup.Children[i];

                var childGroup = calculationBase as CalculationGroup;
                if (childGroup != null)
                {
                    entity.CalculationGroupEntity1.Add(childGroup.Create(registry, i));
                }
                var childPipingCalculation = calculationBase as PipingCalculationScenario;
                if (childPipingCalculation != null)
                {
                    entity.PipingCalculationEntities.Add(childPipingCalculation.Create(registry, i));
                }
                var childGrassCoverErosionInwardsCalculation = calculationBase as GrassCoverErosionInwardsCalculation;
                if (childGrassCoverErosionInwardsCalculation != null)
                {
                    entity.GrassCoverErosionInwardsCalculationEntities.Add(childGrassCoverErosionInwardsCalculation.Create(registry, i));
                }
                var stabilityStoneCoverWaveConditionsCalculation = calculationBase as StabilityStoneCoverWaveConditionsCalculation;
                if (stabilityStoneCoverWaveConditionsCalculation != null)
                {
                    entity.StabilityStoneCoverWaveConditionsCalculationEntities.Add(stabilityStoneCoverWaveConditionsCalculation.Create(registry, i));
                }
            }
        }
    }
}