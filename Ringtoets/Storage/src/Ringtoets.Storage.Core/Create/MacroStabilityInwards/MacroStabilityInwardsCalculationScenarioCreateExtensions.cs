// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsCalculationScenario"/> related to creating
    /// a <see cref="MacroStabilityInwardsCalculationEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsCalculationScenarioCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsCalculationEntity"/> based on the information of the <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        /// <param name="calculation">The macro stability inwards calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="calculation"/> resides within its parent.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationEntity Create(this MacroStabilityInwardsCalculationScenario calculation,
                                                                    PersistenceRegistry registry,
                                                                    int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new MacroStabilityInwardsCalculationEntity
            {
                Name = calculation.Name.DeepClone(),
                Comment = calculation.Comments.Body.DeepClone(),
                ScenarioContribution = calculation.Contribution.ToNaNAsNull(),
                RelevantForScenario = Convert.ToByte(calculation.IsRelevant),
                Order = order
            };
            SetInputParametersToEntity(entity, calculation.InputParameters, registry);
            AddEntityForMacroStabilityInwardsOutput(entity, calculation.Output);

            return entity;
        }

        private static void SetInputParametersToEntity(MacroStabilityInwardsCalculationEntity entity,
                                                       MacroStabilityInwardsInput inputParameters,
                                                       PersistenceRegistry registry)
        {
            if (inputParameters.SurfaceLine != null)
            {
                entity.SurfaceLineEntity = registry.Get(inputParameters.SurfaceLine);
            }

            if (inputParameters.StochasticSoilProfile != null)
            {
                entity.MacroStabilityInwardsStochasticSoilProfileEntity = registry.Get(inputParameters.StochasticSoilProfile);
            }

            SetHydraulicBoundaryLocationInputToEntity(entity, inputParameters, registry);

            entity.SlipPlaneMinimumDepth = inputParameters.SlipPlaneMinimumDepth.ToNaNAsNull();
            entity.SlipPlaneMinimumLength = inputParameters.SlipPlaneMinimumLength.ToNaNAsNull();
            entity.MaximumSliceWidth = inputParameters.MaximumSliceWidth.ToNaNAsNull();

            entity.MoveGrid = Convert.ToByte(inputParameters.MoveGrid);
            entity.DikeSoilScenario = Convert.ToByte(inputParameters.DikeSoilScenario);

            entity.WaterLevelRiverAverage = inputParameters.WaterLevelRiverAverage.ToNaNAsNull();

            entity.DrainageConstructionPresent = Convert.ToByte(inputParameters.DrainageConstructionPresent);
            entity.DrainageConstructionCoordinateX = inputParameters.XCoordinateDrainageConstruction.ToNaNAsNull();
            entity.DrainageConstructionCoordinateZ = inputParameters.ZCoordinateDrainageConstruction.ToNaNAsNull();

            entity.MinimumLevelPhreaticLineAtDikeTopRiver = inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver.ToNaNAsNull();
            entity.MinimumLevelPhreaticLineAtDikeTopPolder = inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder.ToNaNAsNull();

            SetLocationInputExtremeParametersToEntity(entity, inputParameters.LocationInputExtreme);
            SetLocationInputDailyParametersToEntity(entity, inputParameters.LocationInputDaily);

            entity.AdjustPhreaticLine3And4ForUplift = Convert.ToByte(inputParameters.AdjustPhreaticLine3And4ForUplift);
            entity.LeakageLengthOutwardsPhreaticLine4 = inputParameters.LeakageLengthOutwardsPhreaticLine4.ToNaNAsNull();
            entity.LeakageLengthInwardsPhreaticLine4 = inputParameters.LeakageLengthInwardsPhreaticLine4.ToNaNAsNull();
            entity.LeakageLengthOutwardsPhreaticLine3 = inputParameters.LeakageLengthOutwardsPhreaticLine3.ToNaNAsNull();
            entity.LeakageLengthInwardsPhreaticLine3 = inputParameters.LeakageLengthInwardsPhreaticLine3.ToNaNAsNull();
            entity.PiezometricHeadPhreaticLine2Outwards = inputParameters.PiezometricHeadPhreaticLine2Outwards.ToNaNAsNull();
            entity.PiezometricHeadPhreaticLine2Inwards = inputParameters.PiezometricHeadPhreaticLine2Inwards.ToNaNAsNull();

            entity.GridDeterminationType = Convert.ToByte(inputParameters.GridDeterminationType);
            entity.TangentLineDeterminationType = Convert.ToByte(inputParameters.TangentLineDeterminationType);

            entity.TangentLineZTop = inputParameters.TangentLineZTop.ToNaNAsNull();
            entity.TangentLineZBottom = inputParameters.TangentLineZBottom.ToNaNAsNull();
            entity.TangentLineNumber = inputParameters.TangentLineNumber;

            SetGridParametersToEntity(entity, inputParameters.LeftGrid, inputParameters.RightGrid);

            entity.CreateZones = Convert.ToByte(inputParameters.CreateZones);
            entity.ZoningBoundariesDeterminationType = Convert.ToByte(inputParameters.ZoningBoundariesDeterminationType);
            entity.ZoneBoundaryLeft = inputParameters.ZoneBoundaryLeft.ToNaNAsNull();
            entity.ZoneBoundaryRight = inputParameters.ZoneBoundaryRight.ToNaNAsNull();
        }

        private static void SetGridParametersToEntity(MacroStabilityInwardsCalculationEntity entity,
                                                      MacroStabilityInwardsGrid leftGrid,
                                                      MacroStabilityInwardsGrid rightGrid)
        {
            entity.LeftGridXLeft = leftGrid.XLeft.ToNaNAsNull();
            entity.LeftGridXRight = leftGrid.XRight.ToNaNAsNull();
            entity.LeftGridNrOfHorizontalPoints = leftGrid.NumberOfHorizontalPoints;
            entity.LeftGridZTop = leftGrid.ZTop.ToNaNAsNull();
            entity.LeftGridZBottom = leftGrid.ZBottom.ToNaNAsNull();
            entity.LeftGridNrOfVerticalPoints = leftGrid.NumberOfVerticalPoints;

            entity.RightGridXLeft = rightGrid.XLeft.ToNaNAsNull();
            entity.RightGridXRight = rightGrid.XRight.ToNaNAsNull();
            entity.RightGridNrOfHorizontalPoints = rightGrid.NumberOfHorizontalPoints;
            entity.RightGridZTop = rightGrid.ZTop.ToNaNAsNull();
            entity.RightGridZBottom = rightGrid.ZBottom.ToNaNAsNull();
            entity.RightGridNrOfVerticalPoints = rightGrid.NumberOfVerticalPoints;
        }

        private static void SetLocationInputDailyParametersToEntity(MacroStabilityInwardsCalculationEntity entity,
                                                                    IMacroStabilityInwardsLocationInputDaily inputParameters)
        {
            entity.LocationInputDailyWaterLevelPolder = inputParameters.WaterLevelPolder.ToNaNAsNull();
            entity.LocationInputDailyUseDefaultOffsets = Convert.ToByte(inputParameters.UseDefaultOffsets);

            entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver = inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver.ToNaNAsNull();
            entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder = inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder.ToNaNAsNull();
            entity.LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside = inputParameters.PhreaticLineOffsetBelowShoulderBaseInside.ToNaNAsNull();
            entity.LocationInputDailyPhreaticLineOffsetDikeToeAtPolder = inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder.ToNaNAsNull();
        }

        private static void SetLocationInputExtremeParametersToEntity(MacroStabilityInwardsCalculationEntity entity,
                                                                      IMacroStabilityInwardsLocationInputExtreme inputParameters)
        {
            entity.LocationInputExtremeWaterLevelPolder = inputParameters.WaterLevelPolder.ToNaNAsNull();
            entity.LocationInputExtremeUseDefaultOffsets = Convert.ToByte(inputParameters.UseDefaultOffsets);

            entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver = inputParameters.PhreaticLineOffsetBelowDikeTopAtRiver.ToNaNAsNull();
            entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder = inputParameters.PhreaticLineOffsetBelowDikeTopAtPolder.ToNaNAsNull();
            entity.LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside = inputParameters.PhreaticLineOffsetBelowShoulderBaseInside.ToNaNAsNull();
            entity.LocationInputExtremePhreaticLineOffsetDikeToeAtPolder = inputParameters.PhreaticLineOffsetBelowDikeToeAtPolder.ToNaNAsNull();
            entity.LocationInputExtremePenetrationLength = inputParameters.PenetrationLength.ToNaNAsNull();
        }

        private static void SetHydraulicBoundaryLocationInputToEntity(MacroStabilityInwardsCalculationEntity entity,
                                                                      MacroStabilityInwardsInput inputParameters,
                                                                      PersistenceRegistry registry)
        {
            entity.UseAssessmentLevelManualInput = Convert.ToByte(inputParameters.UseAssessmentLevelManualInput);
            entity.AssessmentLevel = inputParameters.AssessmentLevel.ToNaNAsNull();

            if (inputParameters.HydraulicBoundaryLocation != null)
            {
                entity.HydraulicLocationEntity = registry.Get(inputParameters.HydraulicBoundaryLocation);
            }
        }

        private static void AddEntityForMacroStabilityInwardsOutput(MacroStabilityInwardsCalculationEntity entity,
                                                                    MacroStabilityInwardsOutput output)
        {
            if (output != null)
            {
                entity.MacroStabilityInwardsCalculationOutputEntities.Add(output.Create());
            }
        }
    }
}