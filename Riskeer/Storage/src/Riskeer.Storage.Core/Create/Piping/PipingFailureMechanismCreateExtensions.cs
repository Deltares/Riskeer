﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.Storage.Core.Create.FailureMechanismSectionResults;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class PipingFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this PipingFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.Piping, registry);

            AddEntitiesForFailureMechanismMeta(mechanism, entity);
            AddEntitiesForStochasticSoilModels(mechanism, registry, entity);
            AddEntitiesForSurfaceLines(mechanism, registry, entity);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);
            AddEntitiesForPipingScenarioConfigurationPerFailureMechanismSection(mechanism.ScenarioConfigurationsPerFailureMechanismSection,
                                                                                registry);

            entity.CalculationGroupEntity = mechanism.CalculationsGroup.Create(registry, 0);

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<AdoptableFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (AdoptableFailureMechanismSectionResult pipingFailureMechanismSectionResult in sectionResults)
            {
                AdoptableFailureMechanismSectionResultEntity sectionResultEntity = pipingFailureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(pipingFailureMechanismSectionResult.Section);
                section.AdoptableFailureMechanismSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static void AddEntitiesForPipingScenarioConfigurationPerFailureMechanismSection(
            IEnumerable<PipingScenarioConfigurationPerFailureMechanismSection> scenarioConfigurations,
            PersistenceRegistry registry)
        {
            foreach (PipingScenarioConfigurationPerFailureMechanismSection configuration in scenarioConfigurations)
            {
                PipingScenarioConfigurationPerFailureMechanismSectionEntity configurationPerFailureMechanismSectionEntity = configuration.Create();
                FailureMechanismSectionEntity section = registry.Get(configuration.Section);
                section.PipingScenarioConfigurationPerFailureMechanismSectionEntities.Add(configurationPerFailureMechanismSectionEntity);
            }
        }

        private static void AddEntitiesForFailureMechanismMeta(PipingFailureMechanism mechanism, FailureMechanismEntity entity)
        {
            var metaEntity = new PipingFailureMechanismMetaEntity
            {
                A = mechanism.ProbabilityAssessmentInput.A,
                WaterVolumetricWeight = mechanism.GeneralInput.WaterVolumetricWeight,
                StochasticSoilModelCollectionSourcePath = mechanism.StochasticSoilModels.SourcePath.DeepClone(),
                SurfaceLineCollectionSourcePath = mechanism.SurfaceLines.SourcePath.DeepClone(),
                PipingScenarioConfigurationType = Convert.ToByte(mechanism.ScenarioConfigurationType)
            };

            entity.PipingFailureMechanismMetaEntities.Add(metaEntity);
        }

        private static void AddEntitiesForStochasticSoilModels(PipingFailureMechanism mechanism, PersistenceRegistry registry, FailureMechanismEntity entity)
        {
            var index = 0;
            foreach (PipingStochasticSoilModel stochasticSoilModel in mechanism.StochasticSoilModels)
            {
                entity.StochasticSoilModelEntities.Add(stochasticSoilModel.Create(registry, index++));
            }
        }

        private static void AddEntitiesForSurfaceLines(PipingFailureMechanism mechanism, PersistenceRegistry registry, FailureMechanismEntity entity)
        {
            var index = 0;
            foreach (PipingSurfaceLine surfaceLine in mechanism.SurfaceLines)
            {
                entity.SurfaceLineEntities.Add(surfaceLine.Create(registry, index++));
            }
        }
    }
}