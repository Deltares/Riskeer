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
using System.IO;
using System.Linq;
using Core.Common.Util.Extensions;
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Interface;
using Deltares.MacroStability.Io;
using Deltares.MacroStability.Io.XmlInput;
using Deltares.MacroStability.Kernel;
using Deltares.MacroStability.Preprocessing;
using Deltares.MacroStability.Standard;
using Deltares.MacroStability.WaternetCreator;
using Deltares.SoilStress.Data;
using Deltares.WTIStability.Calculation.Wrapper;
using log4net;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan
{
    /// <summary>
    /// Class that wraps <see cref="WTIStabilityCalculation"/> for performing an Uplift Van calculation.
    /// </summary>
    internal class UpliftVanKernelWrapper : IUpliftVanKernel
    {
        private readonly KernelModel kernelModel;
        private readonly ILog log = LogManager.GetLogger(typeof(UpliftVanKernelWrapper));

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
                        AddConstructionStage(),
                        AddConstructionStage()
                    }
                }
            };

            AddPreProcessingConstructionStages();
            AddPreProcessingConstructionStages();

            FactorOfStability = double.NaN;
            ForbiddenZonesXEntryMin = double.NaN;
            ForbiddenZonesXEntryMax = double.NaN;
        }

        public double FactorOfStability { get; private set; }

        public double ForbiddenZonesXEntryMin { get; private set; }

        public double ForbiddenZonesXEntryMax { get; private set; }

        public SlidingDualCircle SlidingCurveResult { get; private set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; private set; }

        public IEnumerable<LogMessage> CalculationMessages { get; private set; }

        public void SetSoilModel(IList<Soil> soilModel)
        {
            kernelModel.StabilityModel.Soils.AddRange(soilModel);
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            kernelModel.StabilityModel.ConstructionStages.ForEachElementDo(cs => cs.SoilProfile = soilProfile);
        }

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

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            kernelModel.PreprocessingModel.LastStage.SurfaceLine = surfaceLine;
            foreach (var preProcessingConstructionStage in kernelModel.PreprocessingModel.PreProcessingConstructionStages)
            {
                preProcessingConstructionStage.SurfaceLine = surfaceLine;
            }
        }

        public void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints)
        {
            kernelModel.StabilityModel.SlipPlaneConstraints = slipPlaneConstraints;
        }

        public void SetGridAutomaticDetermined(bool gridAutomaticDetermined)
        {
            kernelModel.PreprocessingModel.SearchAreaConditions.AutoSearchArea = gridAutomaticDetermined;
        }

        public void SetTangentLinesAutomaticDetermined(bool tangentLinesAutomaticDetermined)
        {
            kernelModel.PreprocessingModel.SearchAreaConditions.AutoTangentLines = tangentLinesAutomaticDetermined;
        }

        public void SetFixedSoilStresses(IEnumerable<FixedSoilStress> soilStresses)
        {
            kernelModel.StabilityModel.ConstructionStages.ForEachElementDo(cs =>
            {
                cs.SoilStresses.AddRange(soilStresses);
            });
        }

        public void SetPreConsolidationStresses(IEnumerable<PreConsolidationStress> preConsolidationStresses)
        {
            kernelModel.StabilityModel.ConstructionStages.ForEachElementDo(cs =>
            {
                cs.PreconsolidationStresses.AddRange(preConsolidationStresses);
            });
        }

        public void Calculate()
        {
            try
            {
                var kernelCalculation = new KernelCalculation();
                kernelCalculation.Run(kernelModel);

                WriteXmlFile();

                SetResults();
                CalculationMessages = kernelCalculation.LogMessages ?? Enumerable.Empty<LogMessage>();
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
                // The following lines are necessary as a workaround for the kernel.
                // The kernel validates before generating the grid, with calling Update, this is prevented.
                PreprocessingModel preprocessingModel = kernelModel.PreprocessingModel;
                var preprocessor = new StabilityPreprocessor();
                preprocessor.Update(kernelModel.StabilityModel, preprocessingModel);

                IValidationResult[] validationResults = Validator.Validate(kernelModel);
                return validationResults;
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        private void WriteXmlFile()
        {
            FullInputModelType fullInputModel = FillXmlInputFromDomain.CreateStabilityInput(kernelModel);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"XmlFile-{DateTime.Now.Ticks}.txt");

            MacroStabilityXmlSerialization.SaveInputAsXmlFile(filePath, fullInputModel);
            log.Info($"Het Xml bestand is geschreven naar: {filePath}");
        }

        private static ConstructionStage AddConstructionStage()
        {
            return new ConstructionStage
            {
                MultiplicationFactorsCPhiForUpliftList =
                {
                    new MultiplicationFactorOnCPhiForUplift
                    {
                        MultiplicationFactor = 0.0,
                        UpliftFactor = 1.2
                    }
                }
            };
        }

        private void AddPreProcessingConstructionStages()
        {
            kernelModel.PreprocessingModel.PreProcessingConstructionStages.Add(new PreprocessingConstructionStage
            {
                StabilityModel = kernelModel.StabilityModel,
                Locations =
                {
                    // This location is necessary to prevent a location missing warning from the kernel.
                    // In new kernel versions the kernel itself does this action so later this can be removed.
                    new Location
                    {
                        WaternetCreationMode = WaternetCreationMode.FillInWaternetValues
                    }
                }
            });
        }

        private void SetResults()
        {
            StabilityModel stabilityModel = kernelModel.StabilityModel;
            FactorOfStability = stabilityModel.MinimumSafetyCurve.SafetyFactor;
            ForbiddenZonesXEntryMin = stabilityModel.SlipPlaneConstraints.XLeftMin;
            ForbiddenZonesXEntryMax = stabilityModel.SlipPlaneConstraints.XLeftMax;

            SlidingCurveResult = (SlidingDualCircle) stabilityModel.MinimumSafetyCurve;
            SlipPlaneResult = stabilityModel.SlipPlaneUpliftVan;
        }
    }
}