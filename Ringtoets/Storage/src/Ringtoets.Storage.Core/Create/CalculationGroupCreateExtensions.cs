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
using Core.Common.Util.Extensions;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.Storage.Core.Create.GrassCoverErosionInwards;
using Ringtoets.Storage.Core.Create.GrassCoverErosionOutwards;
using Ringtoets.Storage.Core.Create.MacroStabilityInwards;
using Ringtoets.Storage.Core.Create.Piping;
using Ringtoets.Storage.Core.Create.StabilityStoneCover;
using Ringtoets.Storage.Core.Create.WaveImpactAsphaltCover;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Storage.Core.Create
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
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new CalculationGroupEntity
            {
                Name = group.Name.DeepClone(),
                Order = order
            };
            CreateChildElements(group, entity, registry);

            return entity;
        }

        private static void CreateChildElements(CalculationGroup parentGroup, CalculationGroupEntity entity, PersistenceRegistry registry)
        {
            for (var i = 0; i < parentGroup.Children.Count; i++)
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

                var childMacroStabilityInwardsCalculation = calculationBase as MacroStabilityInwardsCalculationScenario;
                if (childMacroStabilityInwardsCalculation != null)
                {
                    entity.MacroStabilityInwardsCalculationEntities.Add(childMacroStabilityInwardsCalculation.Create(registry, i));
                }

                var childGrassCoverErosionInwardsCalculation = calculationBase as GrassCoverErosionInwardsCalculation;
                if (childGrassCoverErosionInwardsCalculation != null)
                {
                    entity.GrassCoverErosionInwardsCalculationEntities.Add(childGrassCoverErosionInwardsCalculation.Create(registry, i));
                }

                var childGrassCoverErosionOutwardsCalculation = calculationBase as GrassCoverErosionOutwardsWaveConditionsCalculation;
                if (childGrassCoverErosionOutwardsCalculation != null)
                {
                    entity.GrassCoverErosionOutwardsWaveConditionsCalculationEntities.Add(childGrassCoverErosionOutwardsCalculation.Create(registry, i));
                }

                var childHeightStructuresCalculation = calculationBase as StructuresCalculation<HeightStructuresInput>;
                if (childHeightStructuresCalculation != null)
                {
                    entity.HeightStructuresCalculationEntities.Add(childHeightStructuresCalculation.CreateForHeightStructures(registry, i));
                }

                var childClosingStructuresCalculation = calculationBase as StructuresCalculation<ClosingStructuresInput>;
                if (childClosingStructuresCalculation != null)
                {
                    entity.ClosingStructuresCalculationEntities.Add(childClosingStructuresCalculation.CreateForClosingStructures(registry, i));
                }

                var childStabilityPointStructuresCalculation = calculationBase as StructuresCalculation<StabilityPointStructuresInput>;
                if (childStabilityPointStructuresCalculation != null)
                {
                    entity.StabilityPointStructuresCalculationEntities.Add(childStabilityPointStructuresCalculation.CreateForStabilityPointStructures(registry, i));
                }

                var stabilityStoneCoverWaveConditionsCalculation = calculationBase as StabilityStoneCoverWaveConditionsCalculation;
                if (stabilityStoneCoverWaveConditionsCalculation != null)
                {
                    entity.StabilityStoneCoverWaveConditionsCalculationEntities.Add(stabilityStoneCoverWaveConditionsCalculation.Create(registry, i));
                }

                var waveImpactAsphaltCoverWaveConditionsCalculation = calculationBase as WaveImpactAsphaltCoverWaveConditionsCalculation;
                if (waveImpactAsphaltCoverWaveConditionsCalculation != null)
                {
                    entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntities.Add(waveImpactAsphaltCoverWaveConditionsCalculation.Create(registry, i));
                }
            }
        }
    }
}