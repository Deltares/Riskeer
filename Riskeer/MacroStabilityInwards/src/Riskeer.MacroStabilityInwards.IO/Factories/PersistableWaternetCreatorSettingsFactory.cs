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
using System.ComponentModel;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Factories
{
    /// <summary>
    /// Factory for creating instances of <see cref="PersistableWaternetCreatorSettings"/>.
    /// </summary>
    internal static class PersistableWaternetCreatorSettingsFactory
    {
        /// <summary>
        /// Creates a new collection of <see cref="PersistableWaternetCreatorSettings"/>.
        /// </summary>
        /// <param name="input">The input to use.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use.</param>
        /// <param name="idFactory">The factory for creating IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>A collection of <see cref="PersistableWaternetCreatorSettings"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/>, <paramref name="idFactory"/>
        /// or <paramref name="registry"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="MacroStabilityInwardsInput.DikeSoilScenario"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsDikeSoilScenario"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="MacroStabilityInwardsInput.DikeSoilScenario"/>
        /// is not supported.</exception>
        public static IEnumerable<PersistableWaternetCreatorSettings> Create(MacroStabilityInwardsInput input, RoundedDouble normativeAssessmentLevel,
                                                                             IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

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
                CreateDaily(input, idFactory, registry),
                CreateExtreme(input, normativeAssessmentLevel, idFactory, registry)
            };
        }

        /// <summary>
        /// Creates a new <see cref="PersistableWaternetCreatorSettings"/> for daily.
        /// </summary>
        /// <param name="input">The input to use.</param>
        /// <param name="idFactory">The factory for creating IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>The created <see cref="PersistableWaternetCreatorSettings"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="IMacroStabilityInwardsWaternetInput.DikeSoilScenario"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsDikeSoilScenario"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="IMacroStabilityInwardsWaternetInput.DikeSoilScenario"/>
        /// is not supported.</exception>
        private static PersistableWaternetCreatorSettings CreateDaily(IMacroStabilityInwardsWaternetInput input, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            PersistableWaternetCreatorSettings waternetCreatorSettings = Create(input, idFactory, registry, MacroStabilityInwardsExportStageType.Daily);
            waternetCreatorSettings.NormativeWaterLevel = input.WaterLevelRiverAverage;
            waternetCreatorSettings.WaterLevelHinterland = input.LocationInputDaily.WaterLevelPolder;
            SetOffsets(input.LocationInputDaily, waternetCreatorSettings);
            waternetCreatorSettings.IntrusionLength = input.LocationInputDaily.PenetrationLength;

            return waternetCreatorSettings;
        }

        /// <summary>
        /// Creates a new <see cref="PersistableWaternetCreatorSettings"/> for extreme.
        /// </summary>
        /// <param name="input">The input to use.</param>
        /// <param name="normativeAssessmentLevel">The normative assessment level to use.</param>
        /// <param name="idFactory">The factory for creating IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <returns>The created <see cref="PersistableWaternetCreatorSettings"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="IMacroStabilityInwardsWaternetInput.DikeSoilScenario"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsDikeSoilScenario"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="IMacroStabilityInwardsWaternetInput.DikeSoilScenario"/>
        /// is not supported.</exception>
        private static PersistableWaternetCreatorSettings CreateExtreme(IMacroStabilityInwardsWaternetInput input, RoundedDouble normativeAssessmentLevel,
                                                                        IdFactory idFactory, MacroStabilityInwardsExportRegistry registry)
        {
            PersistableWaternetCreatorSettings waternetCreatorSettings = Create(input, idFactory, registry, MacroStabilityInwardsExportStageType.Extreme);
            waternetCreatorSettings.NormativeWaterLevel = normativeAssessmentLevel;
            waternetCreatorSettings.WaterLevelHinterland = input.LocationInputExtreme.WaterLevelPolder;
            SetOffsets(input.LocationInputExtreme, waternetCreatorSettings);
            waternetCreatorSettings.IntrusionLength = input.LocationInputExtreme.PenetrationLength;

            return waternetCreatorSettings;
        }

        /// <summary>
        /// Creates a new <see cref="PersistableWaternetCreatorSettings"/>.
        /// </summary>
        /// <param name="input">The input to use.</param>
        /// <param name="idFactory">The factory for creating IDs.</param>
        /// <param name="registry">The persistence registry.</param>
        /// <param name="stageType">The stage type.</param>
        /// <returns>The created <see cref="PersistableWaternetCreatorSettings"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="IMacroStabilityInwardsWaternetInput.DikeSoilScenario"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsDikeSoilScenario"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="IMacroStabilityInwardsWaternetInput.DikeSoilScenario"/>
        /// is not supported.</exception>
        private static PersistableWaternetCreatorSettings Create(IMacroStabilityInwardsWaternetInput input, IdFactory idFactory, MacroStabilityInwardsExportRegistry registry,
                                                                 MacroStabilityInwardsExportStageType stageType)
        {
            bool isDitchPresent = IsDitchPresent(input.SurfaceLine);
            var waternetCreatorSettings = new PersistableWaternetCreatorSettings
            {
                Id = idFactory.Create(),
                InitialLevelEmbankmentTopWaterSide = input.MinimumLevelPhreaticLineAtDikeTopRiver,
                InitialLevelEmbankmentTopLandSide = input.MinimumLevelPhreaticLineAtDikeTopPolder,
                AdjustForUplift = input.AdjustPhreaticLine3And4ForUplift,
                PleistoceneLeakageLengthOutwards = input.LeakageLengthOutwardsPhreaticLine3,
                PleistoceneLeakageLengthInwards = input.LeakageLengthInwardsPhreaticLine3,
                AquiferLayerInsideAquitardLeakageLengthOutwards = input.LeakageLengthOutwardsPhreaticLine4,
                AquiferLayerInsideAquitardLeakageLengthInwards = input.LeakageLengthInwardsPhreaticLine4,
                AquitardHeadWaterSide = input.PiezometricHeadPhreaticLine2Outwards,
                AquitardHeadLandSide = input.PiezometricHeadPhreaticLine2Inwards,
                MeanWaterLevel = input.WaterLevelRiverAverage,
                IsDrainageConstructionPresent = input.DrainageConstructionPresent,
                DrainageConstruction = new PersistablePoint(input.XCoordinateDrainageConstruction, input.ZCoordinateDrainageConstruction),
                IsDitchPresent = isDitchPresent,
                DitchCharacteristics = CreateDitchCharacteristics(input.SurfaceLine, isDitchPresent),
                EmbankmentCharacteristics = CreateEmbankmentCharacteristics(input.SurfaceLine),
                EmbankmentSoilScenario = CreateEmbankmentSoilScenario(input.DikeSoilScenario),
                IsAquiferLayerInsideAquitard = false
            };

            IEnumerable<MacroStabilityInwardsSoilLayer2D> aquiferLayers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(input.SoilProfileUnderSurfaceLine.Layers)
                                                                                                                        .Where(l => l.Data.IsAquifer);

            if (aquiferLayers.Count() == 1)
            {
                waternetCreatorSettings.AquiferLayerId = registry.GeometryLayers[stageType][aquiferLayers.Single()];
            }

            registry.AddWaternetCreatorSettings(stageType, waternetCreatorSettings.Id);

            return waternetCreatorSettings;
        }

        private static bool IsDitchPresent(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return surfaceLine.DitchDikeSide != null
                   && surfaceLine.DitchPolderSide != null
                   && surfaceLine.BottomDitchDikeSide != null
                   && surfaceLine.BottomDitchPolderSide != null;
        }

        private static PersistableDitchCharacteristics CreateDitchCharacteristics(MacroStabilityInwardsSurfaceLine surfaceLine, bool isDitchPresent)
        {
            return new PersistableDitchCharacteristics
            {
                DitchBottomEmbankmentSide = isDitchPresent ? ToLocalXCoordinate(surfaceLine.BottomDitchDikeSide, surfaceLine) : double.NaN,
                DitchBottomLandSide = isDitchPresent ? ToLocalXCoordinate(surfaceLine.BottomDitchPolderSide, surfaceLine) : double.NaN,
                DitchEmbankmentSide = isDitchPresent ? ToLocalXCoordinate(surfaceLine.DitchDikeSide, surfaceLine) : double.NaN,
                DitchLandSide = isDitchPresent ? ToLocalXCoordinate(surfaceLine.DitchPolderSide, surfaceLine) : double.NaN
            };
        }

        private static PersistableEmbankmentCharacteristics CreateEmbankmentCharacteristics(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new PersistableEmbankmentCharacteristics
            {
                EmbankmentToeLandSide = ToLocalXCoordinate(surfaceLine.DikeToeAtPolder, surfaceLine),
                EmbankmentTopLandSide = ToLocalXCoordinate(surfaceLine.DikeTopAtPolder, surfaceLine),
                EmbankmentTopWaterSide = ToLocalXCoordinate(surfaceLine.DikeTopAtRiver, surfaceLine),
                EmbankmentToeWaterSide = ToLocalXCoordinate(surfaceLine.DikeToeAtRiver, surfaceLine),
                ShoulderBaseLandSide = surfaceLine.ShoulderBaseInside != null
                                           ? ToLocalXCoordinate(surfaceLine.ShoulderBaseInside, surfaceLine)
                                           : double.NaN
            };
        }

        private static double ToLocalXCoordinate(Point3D originalPoint, MechanismSurfaceLineBase surfaceLine)
        {
            return surfaceLine.GetLocalPointFromGeometry(originalPoint).X;
        }

        /// <summary>
        /// Creates the <see cref="PersistableEmbankmentSoilScenario"/>.
        /// </summary>
        /// <param name="dikeSoilScenario">The <see cref="MacroStabilityInwardsDikeSoilScenario"/>
        /// to create the <see cref="PersistableEmbankmentSoilScenario"/> for.</param>
        /// <returns>The created <see cref="PersistableEmbankmentSoilScenario"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="dikeSoilScenario"/>
        /// has an invalid value for <see cref="MacroStabilityInwardsDikeSoilScenario"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dikeSoilScenario"/>
        /// is not supported.</exception>
        private static PersistableEmbankmentSoilScenario CreateEmbankmentSoilScenario(MacroStabilityInwardsDikeSoilScenario dikeSoilScenario)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsDikeSoilScenario), dikeSoilScenario))
            {
                throw new InvalidEnumArgumentException(nameof(dikeSoilScenario),
                                                       (int) dikeSoilScenario,
                                                       typeof(MacroStabilityInwardsDikeSoilScenario));
            }

            switch (dikeSoilScenario)
            {
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay:
                    return PersistableEmbankmentSoilScenario.ClayEmbankmentOnClay;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay:
                    return PersistableEmbankmentSoilScenario.SandEmbankmentOnClay;
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand:
                    return PersistableEmbankmentSoilScenario.ClayEmbankmentOnSand;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand:
                    return PersistableEmbankmentSoilScenario.SandEmbankmentOnSand;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void SetOffsets(IMacroStabilityInwardsLocationInput locationInput, PersistableWaternetCreatorSettings waternetCreatorSettings)
        {
            waternetCreatorSettings.UseDefaultOffsets = locationInput.UseDefaultOffsets;
            waternetCreatorSettings.OffsetEmbankmentTopWaterSide = locationInput.PhreaticLineOffsetBelowDikeTopAtRiver;
            waternetCreatorSettings.OffsetEmbankmentTopLandSide = locationInput.PhreaticLineOffsetBelowDikeTopAtPolder;
            waternetCreatorSettings.OffsetEmbankmentToeLandSide = locationInput.PhreaticLineOffsetBelowDikeToeAtPolder;
            waternetCreatorSettings.OffsetShoulderBaseLandSide = locationInput.PhreaticLineOffsetBelowShoulderBaseInside;
        }
    }
}