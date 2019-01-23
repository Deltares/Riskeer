// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="MacroStabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        public static FailureMechanismEntity Create(this MacroStabilityInwardsFailureMechanism mechanism,
                                                    PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.MacroStabilityInwards, registry);

            AddEntitiesForFailureMechanismMeta(mechanism, entity);
            AddEntitiesForStochasticSoilModels(mechanism, registry, entity);
            AddEntitiesForSurfaceLines(mechanism, registry, entity);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);

            entity.CalculationGroupEntity = mechanism.CalculationsGroup.Create(registry, 0);

            return entity;
        }

        private static void AddEntitiesForFailureMechanismMeta(MacroStabilityInwardsFailureMechanism mechanism,
                                                               FailureMechanismEntity entity)
        {
            var metaEntity = new MacroStabilityInwardsFailureMechanismMetaEntity
            {
                A = mechanism.MacroStabilityInwardsProbabilityAssessmentInput.A,
                StochasticSoilModelCollectionSourcePath = mechanism.StochasticSoilModels.SourcePath.DeepClone(),
                SurfaceLineCollectionSourcePath = mechanism.SurfaceLines.SourcePath.DeepClone()
            };

            entity.MacroStabilityInwardsFailureMechanismMetaEntities.Add(metaEntity);
        }

        private static void AddEntitiesForStochasticSoilModels(MacroStabilityInwardsFailureMechanism mechanism,
                                                               PersistenceRegistry registry,
                                                               FailureMechanismEntity entity)
        {
            var index = 0;
            foreach (MacroStabilityInwardsStochasticSoilModel stochasticSoilModel in mechanism.StochasticSoilModels)
            {
                entity.StochasticSoilModelEntities.Add(stochasticSoilModel.Create(registry, index++));
            }
        }

        private static void AddEntitiesForSurfaceLines(MacroStabilityInwardsFailureMechanism mechanism,
                                                       PersistenceRegistry registry,
                                                       FailureMechanismEntity entity)
        {
            var index = 0;
            foreach (MacroStabilityInwardsSurfaceLine surfaceLine in mechanism.SurfaceLines)
            {
                entity.SurfaceLineEntities.Add(surfaceLine.Create(registry, index++));
            }
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<MacroStabilityInwardsFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (MacroStabilityInwardsFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                MacroStabilityInwardsSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.MacroStabilityInwardsSectionResultEntities.Add(sectionResultEntity);
            }
        }
    }
}