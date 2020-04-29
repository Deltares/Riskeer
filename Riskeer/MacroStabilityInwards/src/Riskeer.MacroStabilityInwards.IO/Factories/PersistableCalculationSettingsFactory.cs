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
using Components.Persistence.Stability.Data;
using Riskeer.MacroStabilityInwards.Data;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableCalculationSettings"/>.
    /// </summary>
    internal static class PersistableCalculationSettingsFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="PersistableCalculationSettings"/>.
        /// </summary>
        /// <param name="slidingCurve">The sliding curve to use.</param>
        /// <param name="idFactory">The factory fo IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableCalculationSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static IEnumerable<PersistableCalculationSettings> Create(MacroStabilityInwardsSlidingCurve slidingCurve, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (slidingCurve == null)
            {
                throw new ArgumentNullException(nameof(slidingCurve));
            }

            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            PersistableCalculationSettings dailySettings = Create(idFactory, registry, MacroStabilityInwardsExportStageType.Daily);
            PersistableCalculationSettings extremeSettings = Create(idFactory, registry, MacroStabilityInwardsExportStageType.Extreme);
            dailySettings.AnalysisType = PersistableAnalysisType.UpliftVan;
            extremeSettings.AnalysisType = PersistableAnalysisType.UpliftVan;
            extremeSettings.UpliftVan = new PersistableUpliftVanSettings
            {
                SlipPlane = new PersistableTwoCirclesOnTangentLine
                {
                    FirstCircleCenter = new PersistablePoint(slidingCurve.LeftCircle.Center.X,
                                                             slidingCurve.LeftCircle.Center.Y),
                    FirstCircleRadius = slidingCurve.LeftCircle.Radius,
                    SecondCircleCenter = new PersistablePoint(slidingCurve.RightCircle.Center.X,
                                                              slidingCurve.RightCircle.Center.Y)
                }
            };
            extremeSettings.CalculationType = PersistableCalculationType.Deterministic;

            return new[]
            {
                dailySettings,
                extremeSettings
            };
        }

        private static PersistableCalculationSettings Create(IdFactory idFactory, MacroStabilityInwardsExportRegistry registry,
                                                             MacroStabilityInwardsExportStageType stageType)
        {
            var settings = new PersistableCalculationSettings
            {
                Id = idFactory.Create()
            };

            registry.AddSettings(stageType, settings.Id);
            return settings;
        }
    }
}