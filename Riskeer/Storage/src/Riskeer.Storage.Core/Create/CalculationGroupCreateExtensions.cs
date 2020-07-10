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
using Core.Common.Util.Extensions;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.Storage.Core.Create.GrassCoverErosionInwards;
using Riskeer.Storage.Core.Create.GrassCoverErosionOutwards;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.Create.StabilityStoneCover;
using Riskeer.Storage.Core.Create.WaveImpactAsphaltCover;
using Riskeer.Storage.Core.DbContext;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Create
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

                if (calculationBase is CalculationGroup childGroup)
                {
                    entity.CalculationGroupEntity1.Add(childGroup.Create(registry, i));
                }

                if (calculationBase is PipingCalculationScenario childPipingCalculation)
                {
                    entity.PipingCalculationEntities.Add(childPipingCalculation.Create(registry, i));
                }

                if (calculationBase is MacroStabilityInwardsCalculationScenario childMacroStabilityInwardsCalculation)
                {
                    entity.MacroStabilityInwardsCalculationEntities.Add(childMacroStabilityInwardsCalculation.Create(registry, i));
                }

                if (calculationBase is GrassCoverErosionInwardsCalculationScenario childGrassCoverErosionInwardsCalculation)
                {
                    entity.GrassCoverErosionInwardsCalculationEntities.Add(childGrassCoverErosionInwardsCalculation.Create(registry, i));
                }

                if (calculationBase is GrassCoverErosionOutwardsWaveConditionsCalculation childGrassCoverErosionOutwardsCalculation)
                {
                    entity.GrassCoverErosionOutwardsWaveConditionsCalculationEntities.Add(childGrassCoverErosionOutwardsCalculation.Create(registry, i));
                }

                if (calculationBase is StructuresCalculationScenario<HeightStructuresInput> childHeightStructuresCalculation)
                {
                    entity.HeightStructuresCalculationEntities.Add(childHeightStructuresCalculation.CreateForHeightStructures(registry, i));
                }

                if (calculationBase is StructuresCalculationScenario<ClosingStructuresInput> childClosingStructuresCalculation)
                {
                    entity.ClosingStructuresCalculationEntities.Add(childClosingStructuresCalculation.CreateForClosingStructures(registry, i));
                }

                if (calculationBase is StructuresCalculationScenario<StabilityPointStructuresInput> childStabilityPointStructuresCalculation)
                {
                    entity.StabilityPointStructuresCalculationEntities.Add(childStabilityPointStructuresCalculation.CreateForStabilityPointStructures(registry, i));
                }

                if (calculationBase is StabilityStoneCoverWaveConditionsCalculation stabilityStoneCoverWaveConditionsCalculation)
                {
                    entity.StabilityStoneCoverWaveConditionsCalculationEntities.Add(stabilityStoneCoverWaveConditionsCalculation.Create(registry, i));
                }

                if (calculationBase is WaveImpactAsphaltCoverWaveConditionsCalculation waveImpactAsphaltCoverWaveConditionsCalculation)
                {
                    entity.WaveImpactAsphaltCoverWaveConditionsCalculationEntities.Add(waveImpactAsphaltCoverWaveConditionsCalculation.Create(registry, i));
                }
            }
        }
    }
}