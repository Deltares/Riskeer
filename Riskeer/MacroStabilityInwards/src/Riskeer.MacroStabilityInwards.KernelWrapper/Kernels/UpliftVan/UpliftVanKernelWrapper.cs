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
using Core.Common.Util.Extensions;
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Kernel;
using Deltares.MacroStability.Preprocessing;
using Deltares.MacroStability.Standard;
using Deltares.WTIStability.Calculation.Wrapper;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing an Uplift Van calculation.
    /// </summary>
    internal class UpliftVanKernelWrapper : IUpliftVanKernel
    {
        private readonly KernelModel kernelModel;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanKernelWrapper"/>.
        /// </summary>
        public UpliftVanKernelWrapper()
        {
            kernelModel = new KernelModel
            {
                StabilityModel =
                {
                    GridOrientation = GridOrientation.Inwards,
                    SlipCircle = new SlipCircle(),
                    SearchAlgorithm = SearchAlgorithm.Grid,
                    ModelOption = ModelOptions.UpliftVan,
                    ConstructionStages =
                    {
                        new ConstructionStage(),
                        new ConstructionStage()
                    }
                }
            };

            AddPreProcessingConstructionStages();
            AddPreProcessingConstructionStages();

            FactorOfStability = double.NaN;
            ZValue = double.NaN;
            ForbiddenZonesXEntryMin = double.NaN;
            ForbiddenZonesXEntryMax = double.NaN;
        }

        private void AddPreProcessingConstructionStages()
        {
            kernelModel.PreprocessingModel.PreProcessingConstructionStages.Add(new PreprocessingConstructionStage
            {
                StabilityModel = kernelModel.StabilityModel
            });
        }

        public double FactorOfStability { get; private set; }

        public double ZValue { get; private set; }

        public double ForbiddenZonesXEntryMin { get; private set; }

        public double ForbiddenZonesXEntryMax { get; private set; }

        public SlidingDualCircle SlidingCurveResult { get; private set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; private set; }

        public IEnumerable<LogMessage> CalculationMessages { get; private set; }

        public void SetWaternetDaily(WtiStabilityWaternet waternetDaily)
        {
            kernelModel.StabilityModel.ConstructionStages.First().GeotechnicsData.CurrentWaternet = waternetDaily;
        }

        public void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme)
        {
            kernelModel.StabilityModel.ConstructionStages.ElementAt(1).GeotechnicsData.CurrentWaternet = waternetExtreme;
        }

        public void SetMoveGrid(bool moveGrid)
        {
            kernelModel.StabilityModel.MoveGrid = moveGrid;
        }

        public void SetMaximumSliceWidth(double maximumSliceWidth)
        {
            kernelModel.StabilityModel.MaximumSliceWidth = maximumSliceWidth;
        }

        public void SetSlipPlaneUpliftVan(SlipPlaneUpliftVan slipPlaneUpliftVan)
        {
            kernelModel.StabilityModel.SlipPlaneUpliftVan = slipPlaneUpliftVan;
        }

        public void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints)
        {
            kernelModel.StabilityModel.SlipPlaneConstraints = slipPlaneConstraints;
        }

        public void SetGridAutomaticDetermined(bool gridAutomaticDetermined)
        {
            kernelModel.PreprocessingModel.SearchAreaConditions.AutoSearchArea = gridAutomaticDetermined;
        }

        public void SetSoilModel(IList<Soil> soilModel)
        {
            kernelModel.StabilityModel.Soils.AddRange(soilModel);
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            kernelModel.StabilityModel.ConstructionStages.ForEachElementDo(cs => cs.SoilProfile = soilProfile);
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            kernelModel.PreprocessingModel.LastStage.SurfaceLine = surfaceLine;
        }

        public void Calculate()
        {
            try
            {
                var kernelCalculation = new KernelCalculation();
                kernelCalculation.Run(kernelModel);

                SetResults(kernelModel);
                ReadLogMessages(kernelCalculation.LogMessages);
            }
            catch (Exception e) when (!(e is UpliftVanKernelWrapperException))
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        public IEnumerable<IValidationResult> Validate()
        {
            try
            {
                IValidationResult[] validationResults = Validator.Validate(kernelModel);
                return validationResults;
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        private void ReadLogMessages(List<LogMessage> kernelCalculationLogMessages)
        {
            CalculationMessages = kernelCalculationLogMessages ?? Enumerable.Empty<LogMessage>();
        }

        /// <summary>
        /// Reads the calculation result.
        /// </summary>
        /// <param name="returnedKernelModel">The returned kernel model.</param>
        private void SetResults(KernelModel returnedKernelModel)
        {
            StabilityModel returnedStabilityModel = returnedKernelModel.StabilityModel;
            FactorOfStability = returnedStabilityModel.MinimumSafetyCurve.SafetyFactor;
            ZValue = returnedStabilityModel.MinimumSafetyCurve.SafetyFactor - 1;
            ForbiddenZonesXEntryMin = returnedStabilityModel.SlipPlaneConstraints.XLeftMin;
            ForbiddenZonesXEntryMax = returnedStabilityModel.SlipPlaneConstraints.XLeftMax;

            SlidingCurveResult = (SlidingDualCircle) returnedStabilityModel.MinimumSafetyCurve;
            SlipPlaneResult = returnedStabilityModel.SlipPlaneUpliftVan;
        }
    }
}