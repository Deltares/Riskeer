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
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsCalculationScenario"/>
    /// based on the <see cref="MacroStabilityInwardsCalculationEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsCalculationEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="MacroStabilityInwardsCalculationEntity"/> and use the information to
        /// construct a <see cref="MacroStabilityInwardsCalculationScenario"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsCalculationEntity"/> to create
        /// <see cref="MacroStabilityInwardsCalculationScenario"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationScenario"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationScenario Read(this MacroStabilityInwardsCalculationEntity entity,
                                                                    ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var calculationScenario = new MacroStabilityInwardsCalculationScenario
            {
                Name = entity.Name,
                IsRelevant = Convert.ToBoolean(entity.RelevantForScenario),
                Contribution = (RoundedDouble) entity.ScenarioContribution.ToNullAsNaN(),
                Comments =
                {
                    Body = entity.Comment
                }
            };
            SetInputParameters(calculationScenario.InputParameters, entity, collector);
            SetCalculationOutputsToScenario(calculationScenario, entity);

            return calculationScenario;
        }

        private static void SetCalculationOutputsToScenario(MacroStabilityInwardsCalculationScenario calculationScenario,
                                                            MacroStabilityInwardsCalculationEntity entity)
        {
            MacroStabilityInwardsCalculationOutputEntity outputEntity = entity.MacroStabilityInwardsCalculationOutputEntities.FirstOrDefault();
            if (outputEntity != null)
            {
                calculationScenario.Output = outputEntity.Read();
            }
        }

        private static void SetInputParameters(MacroStabilityInwardsInput inputParameters,
                                               MacroStabilityInwardsCalculationEntity entity,
                                               ReadConversionCollector collector)
        {
            SetHydraulicBoundaryLocationPropertiesToInput(inputParameters, entity, collector);
            SetSurfaceLineToInput(inputParameters, entity, collector);
            SetStochasticSoilModelToInput(inputParameters, entity, collector);

            inputParameters.SlipPlaneMinimumLength = (RoundedDouble) entity.SlipPlaneMinimumLength.ToNullAsNaN();
            inputParameters.SlipPlaneMinimumDepth = (RoundedDouble) entity.SlipPlaneMinimumDepth.ToNullAsNaN();
            inputParameters.MaximumSliceWidth = (RoundedDouble) entity.MaximumSliceWidth.ToNullAsNaN();

            inputParameters.MoveGrid = Convert.ToBoolean(entity.MoveGrid);
            inputParameters.DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) entity.DikeSoilScenario;

            inputParameters.WaterLevelRiverAverage = (RoundedDouble) entity.WaterLevelRiverAverage.ToNullAsNaN();

            inputParameters.DrainageConstructionPresent = Convert.ToBoolean(entity.DrainageConstructionPresent);
            inputParameters.XCoordinateDrainageConstruction = (RoundedDouble) entity.DrainageConstructionCoordinateX.ToNullAsNaN();
            inputParameters.ZCoordinateDrainageConstruction = (RoundedDouble) entity.DrainageConstructionCoordinateZ.ToNullAsNaN();

            inputParameters.MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) entity.MinimumLevelPhreaticLineAtDikeTopRiver.ToNullAsNaN();
            inputParameters.MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) entity.MinimumLevelPhreaticLineAtDikeTopPolder.ToNullAsNaN();

            SetLocationInputExtremeToInput(inputParameters.LocationInputExtreme, entity);
            SetLocationInputDailyToInput(inputParameters.LocationInputDaily, entity);

            inputParameters.AdjustPhreaticLine3And4ForUplift = Convert.ToBoolean(entity.AdjustPhreaticLine3And4ForUplift);
            inputParameters.LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) entity.LeakageLengthOutwardsPhreaticLine4.ToNullAsNaN();
            inputParameters.LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) entity.LeakageLengthInwardsPhreaticLine4.ToNullAsNaN();
            inputParameters.LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) entity.LeakageLengthOutwardsPhreaticLine3.ToNullAsNaN();
            inputParameters.LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) entity.LeakageLengthInwardsPhreaticLine3.ToNullAsNaN();
            inputParameters.PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) entity.PiezometricHeadPhreaticLine2Outwards.ToNullAsNaN();
            inputParameters.PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) entity.PiezometricHeadPhreaticLine2Inwards.ToNullAsNaN();

            inputParameters.GridDeterminationType = (MacroStabilityInwardsGridDeterminationType) entity.GridDeterminationType;
            inputParameters.TangentLineDeterminationType = (MacroStabilityInwardsTangentLineDeterminationType) entity.TangentLineDeterminationType;

            inputParameters.TangentLineZTop = (RoundedDouble) entity.TangentLineZTop.ToNullAsNaN();
            inputParameters.TangentLineZBottom = (RoundedDouble) entity.TangentLineZBottom.ToNullAsNaN();
            inputParameters.TangentLineNumber = entity.TangentLineNumber;

            SetGridparametersToInput(inputParameters.LeftGrid, inputParameters.RightGrid, entity);

            inputParameters.CreateZones = Convert.ToBoolean(entity.CreateZones);
            inputParameters.ZoningBoundariesDeterminationType = (MacroStabilityInwardsZoningBoundariesDeterminationType) entity.ZoningBoundariesDeterminationType;
            inputParameters.ZoneBoundaryLeft = (RoundedDouble) entity.ZoneBoundaryLeft.ToNullAsNaN();
            inputParameters.ZoneBoundaryRight = (RoundedDouble) entity.ZoneBoundaryRight.ToNullAsNaN();
        }

        private static void SetSurfaceLineToInput(MacroStabilityInwardsInput inputParameters,
                                                  MacroStabilityInwardsCalculationEntity entity,
                                                  ReadConversionCollector collector)
        {
            if (entity.SurfaceLineEntity != null)
            {
                inputParameters.SurfaceLine = entity.SurfaceLineEntity.ReadAsMacroStabilityInwardsSurfaceLine(collector);
            }
        }

        private static void SetHydraulicBoundaryLocationPropertiesToInput(MacroStabilityInwardsInput inputParameters,
                                                                          MacroStabilityInwardsCalculationEntity entity,
                                                                          ReadConversionCollector collector)
        {
            inputParameters.UseAssessmentLevelManualInput = Convert.ToBoolean(entity.UseAssessmentLevelManualInput);
            inputParameters.AssessmentLevel = (RoundedDouble) entity.AssessmentLevel.ToNullAsNaN();

            if (entity.HydraulicLocationEntity != null)
            {
                inputParameters.HydraulicBoundaryLocation = entity.HydraulicLocationEntity.Read(collector);
            }
        }

        private static void SetStochasticSoilModelToInput(MacroStabilityInwardsInput inputParameters,
                                                          MacroStabilityInwardsCalculationEntity entity,
                                                          ReadConversionCollector collector)
        {
            if (entity.MacroStabilityInwardsStochasticSoilProfileEntity != null)
            {
                inputParameters.StochasticSoilModel = entity.MacroStabilityInwardsStochasticSoilProfileEntity
                                                            .StochasticSoilModelEntity
                                                            .ReadAsMacroStabilityInwardsStochasticSoilModel(collector);
                inputParameters.StochasticSoilProfile = entity.MacroStabilityInwardsStochasticSoilProfileEntity.Read(collector);
            }
        }

        private static void SetLocationInputDailyToInput(IMacroStabilityInwardsLocationInputDaily inputDaily,
                                                         MacroStabilityInwardsCalculationEntity entity)
        {
            inputDaily.WaterLevelPolder = (RoundedDouble) entity.LocationInputDailyWaterLevelPolder.ToNullAsNaN();
            inputDaily.UseDefaultOffsets = Convert.ToBoolean(entity.LocationInputDailyUseDefaultOffsets);

            inputDaily.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver.ToNullAsNaN();
            inputDaily.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) entity.LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder.ToNullAsNaN();
            inputDaily.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) entity.LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside.ToNullAsNaN();
            inputDaily.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) entity.LocationInputDailyPhreaticLineOffsetDikeToeAtPolder.ToNullAsNaN();
        }

        private static void SetLocationInputExtremeToInput(IMacroStabilityInwardsLocationInputExtreme inputExtreme,
                                                           MacroStabilityInwardsCalculationEntity entity)
        {
            inputExtreme.WaterLevelPolder = (RoundedDouble) entity.LocationInputExtremeWaterLevelPolder.ToNullAsNaN();
            inputExtreme.UseDefaultOffsets = Convert.ToBoolean(entity.LocationInputExtremeUseDefaultOffsets);

            inputExtreme.PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver.ToNullAsNaN();
            inputExtreme.PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) entity.LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder.ToNullAsNaN();
            inputExtreme.PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) entity.LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside.ToNullAsNaN();
            inputExtreme.PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) entity.LocationInputExtremePhreaticLineOffsetDikeToeAtPolder.ToNullAsNaN();
            inputExtreme.PenetrationLength = (RoundedDouble) entity.LocationInputExtremePenetrationLength.ToNullAsNaN();
        }

        private static void SetGridparametersToInput(MacroStabilityInwardsGrid leftGrid,
                                                     MacroStabilityInwardsGrid rightGrid,
                                                     MacroStabilityInwardsCalculationEntity entity)
        {
            leftGrid.XLeft = (RoundedDouble) entity.LeftGridXLeft.ToNullAsNaN();
            leftGrid.XRight = (RoundedDouble) entity.LeftGridXRight.ToNullAsNaN();
            leftGrid.NumberOfHorizontalPoints = entity.LeftGridNrOfHorizontalPoints;
            leftGrid.ZTop = (RoundedDouble) entity.LeftGridZTop.ToNullAsNaN();
            leftGrid.ZBottom = (RoundedDouble) entity.LeftGridZBottom.ToNullAsNaN();
            leftGrid.NumberOfVerticalPoints = entity.LeftGridNrOfVerticalPoints;

            rightGrid.XLeft = (RoundedDouble) entity.RightGridXLeft.ToNullAsNaN();
            rightGrid.XRight = (RoundedDouble) entity.RightGridXRight.ToNullAsNaN();
            rightGrid.NumberOfHorizontalPoints = entity.RightGridNrOfHorizontalPoints;
            rightGrid.ZTop = (RoundedDouble) entity.RightGridZTop.ToNullAsNaN();
            rightGrid.ZBottom = (RoundedDouble) entity.RightGridZBottom.ToNullAsNaN();
            rightGrid.NumberOfVerticalPoints = entity.RightGridNrOfVerticalPoints;
        }
    }
}