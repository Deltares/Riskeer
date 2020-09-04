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

using System.Collections.Generic;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using KernelPreconsolidationStress = Deltares.MacroStability.CSharpWrapper.Input.PreconsolidationStress;
using SoilProfile = Deltares.MacroStability.CSharpWrapper.Input.SoilProfile;
using WaternetCreationMode = Deltares.MacroStability.CSharpWrapper.Input.WaternetCreationMode;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    internal static class MacroStabilityInputCreator
    {
        public static MacroStabilityInput CreateUpliftVan(UpliftVanCalculatorInput upliftVanInput, IEnumerable<LayerWithSoil> layersWithSoil,
                                                          IDictionary<SoilLayer, LayerWithSoil> layerLookup, SurfaceLine surfaceLine,
                                                          SoilProfile soilProfile, Waternet dailyWaternet, Waternet extremeWaternet)
        {
            return new MacroStabilityInput
            {
                StabilityModel =
                {
                    Orientation = Orientation.Inwards,
                    SearchAlgorithm = SearchAlgorithm.Grid,
                    ModelOption = StabilityModelOptionType.UpliftVan,
                    ConstructionStages =
                    {
                        AddConstructionStage(soilProfile, dailyWaternet, FixedSoilStressCreator.Create(layerLookup).ToList(),
                                             PreconsolidationStressCreator.Create(upliftVanInput.SoilProfile.PreconsolidationStresses).ToList()),
                        AddConstructionStage(soilProfile, extremeWaternet)
                    },
                    Soils = layersWithSoil.Select(lws => lws.Soil).ToList(),
                    MoveGrid = upliftVanInput.MoveGrid,
                    MaximumSliceWidth = upliftVanInput.MaximumSliceWidth,
                    UpliftVanCalculationGrid = UpliftVanCalculationGridCreator.Create(upliftVanInput.SlipPlane),
                    SlipPlaneConstraints = SlipPlaneConstraintsCreator.Create(upliftVanInput.SlipPlaneConstraints),
                },
                PreprocessingInput =
                {
                    SearchAreaConditions =
                    {
                        MaxSpacingBetweenBoundaries = 0.8,
                        OnlyAbovePleistoceen = true,
                        AutoSearchArea = upliftVanInput.SlipPlane.GridAutomaticDetermined,
                        AutoTangentLines = upliftVanInput.SlipPlane.TangentLinesAutomaticAtBoundaries,
                        AutomaticForbiddenZones = upliftVanInput.SlipPlaneConstraints.AutomaticForbiddenZones,
                        TangentLineNumber = upliftVanInput.SlipPlane.TangentLineNumber,
                        TangentLineZTop = upliftVanInput.SlipPlane.TangentZTop,
                        TangentLineZBottom = upliftVanInput.SlipPlane.TangentZBottom
                    },
                    PreConstructionStages =
                    {
                        AddPreConstructionStage(surfaceLine),
                        AddPreConstructionStage(surfaceLine)
                    }
                }
            };
        }

        public static MacroStabilityInput CreateDailyWaternetForUpliftVan(UpliftVanCalculatorInput upliftVanInput, IEnumerable<LayerWithSoil> layersWithSoil,
                                                              SurfaceLine surfaceLine, SoilProfile soilProfile)
        {
            return CreateWaternet(layersWithSoil, surfaceLine, soilProfile, UpliftVanWaternetCreatorInputCreator.CreateDaily(upliftVanInput));
        }

        public static MacroStabilityInput CreateExtremeWaternetForUpliftVan(UpliftVanCalculatorInput upliftVanInput, IEnumerable<LayerWithSoil> layersWithSoil,
                                                                SurfaceLine surfaceLine, SoilProfile soilProfile)
        {
            return CreateWaternet(layersWithSoil, surfaceLine, soilProfile, UpliftVanWaternetCreatorInputCreator.CreateExtreme(upliftVanInput));
        }

        public static MacroStabilityInput CreateWaternet(WaternetCalculatorInput input)
        {
            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> _);
            return CreateWaternet(layersWithSoil, SurfaceLineCreator.Create(input.SurfaceLine),
                                  SoilProfileCreator.Create(layersWithSoil), WaternetCreatorInputCreator.Create(input));
        }

        private static MacroStabilityInput CreateWaternet(IEnumerable<LayerWithSoil> layersWithSoil, SurfaceLine surfaceLine,
                                                          SoilProfile soilProfile, WaternetCreatorInput waternetCreatorInput)
        {
            return new MacroStabilityInput
            {
                StabilityModel =
                {
                    ConstructionStages =
                    {
                        new ConstructionStage
                        {
                            SoilProfile = soilProfile
                        }
                    },
                    Soils = layersWithSoil.Select(lws => lws.Soil).ToList()
                },
                PreprocessingInput =
                {
                    PreConstructionStages =
                    {
                        new PreConstructionStage
                        {
                            SurfaceLine = surfaceLine,
                            WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                            WaternetCreatorInput = waternetCreatorInput
                        }
                    }
                }
            };
        }

        private static ConstructionStage AddConstructionStage(
            SoilProfile soilProfile,
            Waternet waternet,
            List<FixedSoilStress> fixedSoilStresses = null,
            List<KernelPreconsolidationStress> preConsolidationStresses = null)
        {
            return new ConstructionStage
            {
                SoilProfile = soilProfile,
                Waternet = waternet,
                FixedSoilStresses = fixedSoilStresses ?? new List<FixedSoilStress>(),
                PreconsolidationStresses = preConsolidationStresses ?? new List<KernelPreconsolidationStress>(),
                MultiplicationFactorsCPhiForUplift =
                {
                    new MultiplicationFactorsCPhiForUplift
                    {
                        MultiplicationFactor = 0.0,
                        UpliftFactor = 1.2
                    }
                }
            };
        }

        private static PreConstructionStage AddPreConstructionStage(SurfaceLine surfaceLine)
        {
            return new PreConstructionStage
            {
                WaternetCreationMode = WaternetCreationMode.FillInWaternetValues,
                SurfaceLine = surfaceLine,
            };
        }
    }
}