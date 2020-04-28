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
        private readonly StabilityModel stabilityModel;
        private KernelModel kernelModel;
        private SoilProfile2D soilProfile2D;
        private SurfaceLine2 surfaceLine2;
        private WtiStabilityWaternet dailyWaternet;
        private WtiStabilityWaternet extremeWaternet;
        private bool gridAutomaticDetermined;

        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanKernelWrapper"/>.
        /// </summary>
        public UpliftVanKernelWrapper()
        {
            stabilityModel = new StabilityModel
            {
                GridOrientation = GridOrientation.Inwards,
                SlipCircle = new SlipCircle(),
                SearchAlgorithm = SearchAlgorithm.Grid,
                ModelOption = ModelOptions.UpliftVan
            };

            FactorOfStability = double.NaN;
            ZValue = double.NaN;
            ForbiddenZonesXEntryMin = double.NaN;
            ForbiddenZonesXEntryMax = double.NaN;
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
            dailyWaternet = waternetDaily;
        }

        public void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme)
        {
            extremeWaternet = waternetExtreme;
        }

        public void SetMoveGrid(bool moveGrid)
        {
            stabilityModel.MoveGrid = moveGrid;
        }

        public void SetMaximumSliceWidth(double maximumSliceWidth)
        {
            stabilityModel.MaximumSliceWidth = maximumSliceWidth;
        }

        public void SetSlipPlaneUpliftVan(SlipPlaneUpliftVan slipPlaneUpliftVan)
        {
            stabilityModel.SlipPlaneUpliftVan = slipPlaneUpliftVan;
        }

        public void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints)
        {
            stabilityModel.SlipPlaneConstraints = slipPlaneConstraints;
        }

        public void SetGridAutomaticDetermined(bool gridAutomaticDetermined)
        {
            this.gridAutomaticDetermined = gridAutomaticDetermined;
        }

        public void SetSoilModel(IList<Soil> soilModel)
        {
            stabilityModel.Soils.AddRange(soilModel);
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            soilProfile2D = soilProfile;
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            surfaceLine2 = surfaceLine;
        }

        public void Calculate()
        {
            try
            {
                ConfigureKernelModel();

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
                ConfigureKernelModel();

                IValidationResult[] validationResults = Validator.Validate(kernelModel);
                return validationResults;
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        private void ConfigureKernelModel()
        {
            kernelModel = new KernelModel
            {
                StabilityModel = stabilityModel,
                PreprocessingModel = new PreprocessingModel
                {
                    SearchAreaConditions = 
                    {
                        AutoSearchArea = gridAutomaticDetermined
                    }
                }
            };

            AddConstructionStage(0, dailyWaternet);
            AddConstructionStage(1, extremeWaternet);
            kernelModel.PreprocessingModel.LastStage.SurfaceLine = surfaceLine2;
        }

        private void AddConstructionStage(int constructionStageIndex, WtiStabilityWaternet waternet)
        {
            stabilityModel.ConstructionStages.Add(new ConstructionStage());
            kernelModel.PreprocessingModel.PreProcessingConstructionStages.Add(new PreprocessingConstructionStage
            {
                StabilityModel = stabilityModel
            });
            ConstructionStage constructionStage = stabilityModel.ConstructionStages.ElementAt(constructionStageIndex);
            constructionStage.GeotechnicsData.CurrentWaternet = waternet;
            constructionStage.SoilProfile = soilProfile2D;
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