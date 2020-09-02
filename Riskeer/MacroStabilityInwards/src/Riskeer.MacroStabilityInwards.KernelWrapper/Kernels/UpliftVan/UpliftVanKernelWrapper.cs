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
using ConstructionStage = Deltares.MacroStability.CSharpWrapper.Input.ConstructionStage;
using MacroStabilityInput = Deltares.MacroStability.CSharpWrapper.Input.MacroStabilityInput;
using Orientation = Deltares.MacroStability.CSharpWrapper.Input.Orientation;
using SearchAlgorithm = Deltares.MacroStability.CSharpWrapper.Input.SearchAlgorithm;
using WtiStabilityWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing an Uplift Van calculation.
    /// </summary>
    internal class UpliftVanKernelWrapper : IUpliftVanKernel
    {
        private readonly MacroStabilityInput macroStabilityInput;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanKernelWrapper"/>.
        /// </summary>
        public UpliftVanKernelWrapper()
        {
            macroStabilityInput = new MacroStabilityInput
            {
                StabilityModel =
                {
                    Orientation = Orientation.Inwards,
                    SearchAlgorithm = SearchAlgorithm.Grid,
                    ModelOption = StabilityModelOptionType.UpliftVan,
                    ConstructionStages =
                    {
                        AddConstructionStage(),
                        AddConstructionStage()
                    }
                },
                PreprocessingInput =
                {
                    SearchAreaConditions =
                    {
                        MaxSpacingBetweenBoundaries = 0.8,
                        OnlyAbovePleistoceen = true
                    },
                    PreConstructionStages =
                    {
                        new PreConstructionStage
                        {
                            WaternetCreationMode = WaternetCreationMode.FillInWaternetValues
                        },
                        new PreConstructionStage
                        {
                            WaternetCreationMode = WaternetCreationMode.FillInWaternetValues
                        }
                    }
                }
            };

            FactorOfStability = double.NaN;
            ForbiddenZonesXEntryMin = double.NaN;
            ForbiddenZonesXEntryMax = double.NaN;
        }

        public double FactorOfStability { get; private set; }

        public double ForbiddenZonesXEntryMin { get; private set; }

        public double ForbiddenZonesXEntryMax { get; private set; }

        public DualSlidingCircleMinimumSafetyCurve SlidingCurveResult { get; private set; }

        public UpliftVanCalculationGrid SlipPlaneResult { get; private set; }

        public IEnumerable<Message> CalculationMessages { get; private set; }

        public void SetSoilModel(IEnumerable<Soil> soilModel)
        {
            foreach (Soil soil in soilModel)
            {
                macroStabilityInput.StabilityModel.Soils.Add(soil);
            }
        }

        public void SetSoilProfile(SoilProfile soilProfile)
        {
            GetDailyConstructionStage().SoilProfile = soilProfile;
            GetExtremeConstructionStage().SoilProfile = soilProfile;
        }

        public void SetWaternetDaily(WtiStabilityWaternet waternetDaily)
        {
            GetDailyConstructionStage().Waternet = waternetDaily;
        }

        public void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme)
        {
            GetExtremeConstructionStage().Waternet = waternetExtreme;
        }

        public void SetMoveGrid(bool moveGrid)
        {
            macroStabilityInput.StabilityModel.MoveGrid = moveGrid;
        }

        public void SetMaximumSliceWidth(double maximumSliceWidth)
        {
            macroStabilityInput.StabilityModel.MaximumSliceWidth = maximumSliceWidth;
        }

        public void SetSlipPlaneUpliftVan(UpliftVanCalculationGrid slipPlaneUpliftVan)
        {
            macroStabilityInput.StabilityModel.UpliftVanCalculationGrid = slipPlaneUpliftVan;
        }

        public void SetSurfaceLine(SurfaceLine surfaceLine)
        {
            foreach (PreConstructionStage preConstructionStage in macroStabilityInput.PreprocessingInput.PreConstructionStages)
            {
                preConstructionStage.SurfaceLine = surfaceLine;
            }
        }

        public void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints)
        {
            macroStabilityInput.StabilityModel.SlipPlaneConstraints = slipPlaneConstraints;
        }

        public void SetGridAutomaticDetermined(bool gridAutomaticDetermined)
        {
            macroStabilityInput.PreprocessingInput.SearchAreaConditions.AutoSearchArea = gridAutomaticDetermined;
        }

        public void SetTangentLinesAutomaticDetermined(bool tangentLinesAutomaticDetermined)
        {
            macroStabilityInput.PreprocessingInput.SearchAreaConditions.AutoTangentLines = tangentLinesAutomaticDetermined;
        }

        public void SetFixedSoilStresses(IEnumerable<FixedSoilStress> soilStresses)
        {
            ConstructionStage stage = GetDailyConstructionStage();
            foreach (FixedSoilStress soilStress in soilStresses)
            {
                stage.FixedSoilStresses.Add(soilStress);
            }
        }

        public void SetPreConsolidationStresses(IEnumerable<PreconsolidationStress> preconsolidationStresses)
        {
            ConstructionStage stage = GetDailyConstructionStage();

            foreach (PreconsolidationStress preconsolidationStress in preconsolidationStresses)
            {
                stage.PreconsolidationStresses.Add(preconsolidationStress);
            }
        }

        public void SetAutomaticForbiddenZones(bool automaticForbiddenZones)
        {
            macroStabilityInput.PreprocessingInput.SearchAreaConditions.AutomaticForbiddenZones = automaticForbiddenZones;
        }

        public void Calculate()
        {
            try
            {
                var calculator = new Calculator(macroStabilityInput);
                MacroStabilityOutput output = calculator.Calculate();

                CalculationMessages = output.StabilityOutput.Messages ?? Enumerable.Empty<Message>();
                SetResults(output);
            }
            catch (Exception e) when (!(e is UpliftVanKernelWrapperException))
            {
                throw new UpliftVanKernelWrapperException(e.Message, e, CalculationMessages.Where(lm => lm.MessageType == MessageType.Error));
            }
        }

        public IEnumerable<Message> Validate()
        {
            try
            {
                var validator = new Validator(macroStabilityInput);
                ValidationOutput output = validator.Validate();
                return output.Messages;
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        private ConstructionStage GetDailyConstructionStage()
        {
            return macroStabilityInput.StabilityModel.ConstructionStages.First();
        }

        private ConstructionStage GetExtremeConstructionStage()
        {
            return macroStabilityInput.StabilityModel.ConstructionStages.Last();
        }

        private static ConstructionStage AddConstructionStage()
        {
            return new ConstructionStage
            {
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

        private void SetResults(MacroStabilityOutput output)
        {
            FactorOfStability = output.StabilityOutput.SafetyFactor;
            ForbiddenZonesXEntryMin = output.PreprocessingOutputBase.ForbiddenZone.XEntryMin;
            ForbiddenZonesXEntryMax = output.PreprocessingOutputBase.ForbiddenZone.XEntryMax;

            SlidingCurveResult = (DualSlidingCircleMinimumSafetyCurve) output.StabilityOutput.MinimumSafetyCurve;
            SlipPlaneResult = ((UpliftVanPreprocessingOutput) output.PreprocessingOutputBase).UpliftVanCalculationGrid;
        }
    }
}