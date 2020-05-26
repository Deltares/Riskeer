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
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableDataModel"/>.
    /// </summary>
    internal static class PersistableDataModelFactory
    {
        /// <summary>
        /// Creates a new <see cref="PersistableDataModel"/>.
        /// </summary>
        /// <param name="calculation">The calculation to get the data from.</param>
        /// <param name="getNormativeAssessmentLevelFunc"><see cref="Func{TResult}"/>
        /// for obtaining the normative assessment level.</param>
        /// <param name="filePath">The filePath that is used.</param>
        /// <returns>A created <see cref="PersistableDataModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// or <paramref name="getNormativeAssessmentLevelFunc"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="calculation"/>
        /// has no output.</exception>
        public static PersistableDataModel Create(MacroStabilityInwardsCalculation calculation,
                                                  Func<RoundedDouble> getNormativeAssessmentLevelFunc,
                                                  string filePath)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (getNormativeAssessmentLevelFunc == null)
            {
                throw new ArgumentNullException(nameof(getNormativeAssessmentLevelFunc));
            }

            if (!calculation.HasOutput)
            {
                throw new InvalidOperationException("Calculation must have output.");
            }

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            MacroStabilityInwardsInput input = calculation.InputParameters;
            IMacroStabilityInwardsSoilProfileUnderSurfaceLine soilProfile = input.SoilProfileUnderSurfaceLine;

            return new PersistableDataModel
            {
                Info = PersistableProjectInfoFactory.Create(calculation, filePath),
                CalculationSettings = PersistableCalculationSettingsFactory.Create(calculation.Output.SlidingCurve, idFactory, registry),
                Soils = PersistableSoilCollectionFactory.Create(soilProfile, idFactory, registry),
                Geometry = PersistableGeometryFactory.Create(soilProfile, idFactory, registry),
                SoilLayers = PersistableSoilLayerCollectionFactory.Create(soilProfile, idFactory, registry),
                Waternets = PersistableWaternetFactory.Create(
                    DerivedMacroStabilityInwardsInput.GetWaternetDaily(input),
                    DerivedMacroStabilityInwardsInput.GetWaternetExtreme(input, GetEffectiveAssessmentLevel(input, getNormativeAssessmentLevelFunc)),
                    idFactory, registry),
                WaternetCreatorSettings = PersistableWaternetCreatorSettingsFactory.Create(input, GetEffectiveAssessmentLevel(input, getNormativeAssessmentLevelFunc), idFactory, registry),
                States = PersistableStateFactory.Create(soilProfile, idFactory, registry),
                Stages = PersistableStageFactory.Create(idFactory, registry)
            };
        }

        private static RoundedDouble GetEffectiveAssessmentLevel(MacroStabilityInwardsInput input, Func<RoundedDouble> getNormativeAssessmentLevelFunc)
        {
            return input.UseAssessmentLevelManualInput
                       ? input.AssessmentLevel
                       : getNormativeAssessmentLevelFunc();
        }
    }
}