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
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using KernelPreconsolidationStress = Deltares.MacroStability.CSharpWrapper.Input.PreconsolidationStress;
using SoilProfile = Deltares.MacroStability.CSharpWrapper.Input.SoilProfile;
using WaternetCreationMode = Deltares.MacroStability.CSharpWrapper.Input.WaternetCreationMode;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="MacroStabilityInput"/> instances which are required
    /// by <see cref="IUpliftVanKernel"/> and <see cref="IWaternetKernel"/>.
    /// </summary>
    internal static class MacroStabilityInputCreator
    {
        /// <summary>
        /// Creates <see cref="MacroStabilityInput"/> objects based on the given input for the Uplift Van calculation.
        /// </summary>
        /// <param name="upliftVanInput">The <see cref="UpliftVanCalculatorInput"/> containing all the values required
        /// for performing the Uplift Van calculation.</param>
        /// <param name="soils">The collection of <see cref="Soil"/>.</param>
        /// <param name="layerLookup">The lookup between for <see cref="Soil"/> and <see cref="SoilLayer"/></param>
        /// <param name="surfaceLine">The <see cref="SurfaceLine"/>.</param>
        /// <param name="soilProfile">The <see cref="SoilProfile"/>.</param>
        /// <param name="dailyWaternet">The calculated <see cref="Waternet"/> for daily circumstances.</param>
        /// <param name="extremeWaternet">The calculated <see cref="Waternet"/> for extreme circumstances.</param>
        /// <returns>The created <see cref="MacroStabilityInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static MacroStabilityInput CreateUpliftVan(UpliftVanCalculatorInput upliftVanInput, ICollection<Soil> soils,
                                                          IDictionary<SoilLayer, LayerWithSoil> layerLookup, SurfaceLine surfaceLine,
                                                          SoilProfile soilProfile, Waternet dailyWaternet, Waternet extremeWaternet)
        {
            if (upliftVanInput == null)
            {
                throw new ArgumentNullException(nameof(upliftVanInput));
            }

            if (soils == null)
            {
                throw new ArgumentNullException(nameof(soils));
            }

            if (layerLookup == null)
            {
                throw new ArgumentNullException(nameof(layerLookup));
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            if (dailyWaternet == null)
            {
                throw new ArgumentNullException(nameof(dailyWaternet));
            }

            if (extremeWaternet == null)
            {
                throw new ArgumentNullException(nameof(extremeWaternet));
            }

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
                    Soils = soils,
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

        /// <summary>
        /// Creates <see cref="MacroStabilityInput"/> objects based on the given input for the daily waternet calculation.
        /// </summary>
        /// <param name="upliftVanInput">The <see cref="UpliftVanCalculatorInput"/> containing all the values required
        /// for performing the Waternet calculation.</param>
        /// <param name="soils">The collection of <see cref="Soil"/>.</param>
        /// <param name="surfaceLine">The <see cref="SurfaceLine"/>.</param>
        /// <param name="soilProfile">The <see cref="SoilProfile"/>.</param>
        /// <returns>The created <see cref="MacroStabilityInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static MacroStabilityInput CreateDailyWaternetForUpliftVan(UpliftVanCalculatorInput upliftVanInput, ICollection<Soil> soils,
                                                                          SurfaceLine surfaceLine, SoilProfile soilProfile)
        {
            if (upliftVanInput == null)
            {
                throw new ArgumentNullException(nameof(upliftVanInput));
            }

            if (soils == null)
            {
                throw new ArgumentNullException(nameof(soils));
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            return CreateWaternet(soils, surfaceLine, soilProfile, UpliftVanWaternetCreatorInputCreator.CreateDaily(upliftVanInput));
        }

        /// <summary>
        /// Creates <see cref="MacroStabilityInput"/> objects based on the given input for the extreme waternet calculation.
        /// </summary>
        /// <param name="upliftVanInput">The <see cref="UpliftVanCalculatorInput"/> containing all the values required
        /// for performing the Waternet calculation.</param>
        /// <param name="soils">The collection of <see cref="Soil"/>.</param>
        /// <param name="surfaceLine">The <see cref="SurfaceLine"/>.</param>
        /// <param name="soilProfile">The <see cref="SoilProfile"/>.</param>
        /// <returns>The created <see cref="MacroStabilityInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static MacroStabilityInput CreateExtremeWaternetForUpliftVan(UpliftVanCalculatorInput upliftVanInput, ICollection<Soil> soils,
                                                                            SurfaceLine surfaceLine, SoilProfile soilProfile)
        {
            if (upliftVanInput == null)
            {
                throw new ArgumentNullException(nameof(upliftVanInput));
            }

            if (soils == null)
            {
                throw new ArgumentNullException(nameof(soils));
            }

            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            return CreateWaternet(soils, surfaceLine, soilProfile, UpliftVanWaternetCreatorInputCreator.CreateExtreme(upliftVanInput));
        }

        /// <summary>
        /// Creates <see cref="MacroStabilityInput"/> objects based on the given input for the waternet calculation.
        /// </summary>
        /// <param name="waternetInput">The <see cref="WaternetCalculatorInput"/> containing all the values required
        /// for performing the Waternet calculation.</param>
        /// <returns>The created <see cref="MacroStabilityInput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static MacroStabilityInput CreateWaternet(WaternetCalculatorInput waternetInput)
        {
            if (waternetInput == null)
            {
                throw new ArgumentNullException(nameof(waternetInput));
            }

            LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(waternetInput.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> _);
            return CreateWaternet(
                layersWithSoil.Select(lws => lws.Soil).ToList(),
                SurfaceLineCreator.Create(waternetInput.SurfaceLine),
                SoilProfileCreator.Create(layersWithSoil),
                WaternetCreatorInputCreator.Create(waternetInput));
        }

        private static MacroStabilityInput CreateWaternet(ICollection<Soil> soils, SurfaceLine surfaceLine,
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
                    Soils = soils
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