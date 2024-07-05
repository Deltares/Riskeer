﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Components.Persistence.Stability.Data;
using MacroStabilityInwardsDataResources = Riskeer.MacroStabilityInwards.Data.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableStage"/>.
    /// </summary>
    internal static class PersistableStageFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="PersistableStage"/>.
        /// </summary>
        /// <param name="idFactory">The factory for creating IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableStage"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public static IEnumerable<PersistableStage> Create(IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            return new[]
            {
                Create(MacroStabilityInwardsExportStageType.Daily, MacroStabilityInwardsDataResources.Daily_DisplayName, idFactory, registry),
                Create(MacroStabilityInwardsExportStageType.Extreme, MacroStabilityInwardsDataResources.Extreme_DisplayName, idFactory, registry)
            };
        }

        private static PersistableStage Create(MacroStabilityInwardsExportStageType stageType, string label,
                                               IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            return new PersistableStage
            {
                Id = idFactory.Create(),
                Label = label,
                CalculationSettingsId = registry.Settings[stageType],
                GeometryId = registry.Geometries[stageType],
                SoilLayersId = registry.SoilLayers[stageType],
                WaternetId = registry.Waternets[stageType],
                WaternetCreatorSettingsId = registry.WaternetCreatorSettings[stageType],
                StateId = stageType == MacroStabilityInwardsExportStageType.Daily
                              ? registry.States[stageType]
                              : null
            };
        }
    }
}