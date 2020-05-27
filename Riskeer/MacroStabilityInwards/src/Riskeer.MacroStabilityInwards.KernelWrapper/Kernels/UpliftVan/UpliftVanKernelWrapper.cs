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
            kernelModel.PreprocessingModel.SearchAreaConditions.MaxSpacingBetweenBoundaries = 0.8;
            kernelModel.PreprocessingModel.SearchAreaConditions.OnlyAbovePleistoceen = true;

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
            GetDailyConstructionStage().SoilProfile = soilProfile;
            GetExtremeConstructionStage().SoilProfile = soilProfile;
        }

        public void SetWaternetDaily(WtiStabilityWaternet waternetDaily)
        {
            GetDailyConstructionStage().GeotechnicsData.CurrentWaternet = waternetDaily;
        }

        public void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme)
        {
            GetExtremeConstructionStage().GeotechnicsData.CurrentWaternet = waternetExtreme;
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
            foreach (PreprocessingConstructionStage preProcessingConstructionStage in kernelModel.PreprocessingModel.PreProcessingConstructionStages)
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
            GetDailyConstructionStage().SoilStresses.AddRange(soilStresses);
        }

        public void SetPreConsolidationStresses(IEnumerable<PreConsolidationStress> preConsolidationStresses)
        {
            GetDailyConstructionStage().PreconsolidationStresses.AddRange(preConsolidationStresses);
        }

        public void SetAutomaticForbiddenZones(bool automaticForbiddenZones)
        {
            kernelModel.PreprocessingModel.SearchAreaConditions.AutomaticForbiddenZones = automaticForbiddenZones;
        }

        public void Calculate()
        {
            var kernelCalculation = new KernelCalculation();

            try
            {
                kernelCalculation.Run(kernelModel);

                WriteXmlFile();

                CalculationMessages = kernelCalculation.LogMessages ?? Enumerable.Empty<LogMessage>();
                SetResults();
            }
            catch (Exception e) when (!(e is UpliftVanKernelWrapperException))
            {
                throw new UpliftVanKernelWrapperException(e.Message, e, CalculationMessages.Where(lm => lm.MessageType == LogMessageType.Error || lm.MessageType == LogMessageType.FatalError));
            }
        }

        public IEnumerable<IValidationResult> Validate()
        {
            try
            {
                PreprocessModel();

                return Validator.Validate(kernelModel);
            }
            catch (Exception e)
            {
                throw new UpliftVanKernelWrapperException(e.Message, e);
            }
        }

        private ConstructionStage GetDailyConstructionStage()
        {
            return kernelModel.StabilityModel.ConstructionStages.First();
        }

        private ConstructionStage GetExtremeConstructionStage()
        {
            return kernelModel.StabilityModel.ConstructionStages.ElementAt(1);
        }

        /// <summary>
        /// Pre processes the <see cref="StabilityModel"/>.
        /// </summary>
        /// <remarks>
        /// Workaround for the kernel: The kernel validates before generating the grid.
        /// This is prevented by calling update on the <see cref="StabilityPreprocessor"/>.
        /// Should be fixed in a new kernel version.
        /// </remarks>
        private void PreprocessModel()
        {
            PreprocessingModel preprocessingModel = kernelModel.PreprocessingModel;
            var preprocessor = new StabilityPreprocessor();
            preprocessor.Update(kernelModel.StabilityModel, preprocessingModel);
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
            var preprocessingConstructionStage = new PreprocessingConstructionStage
            {
                StabilityModel = kernelModel.StabilityModel
            };
            AddPreprocessingConstructionStageLocation(preprocessingConstructionStage);
            kernelModel.PreprocessingModel.PreProcessingConstructionStages.Add(preprocessingConstructionStage);
        }

        /// <summary>
        /// Add a default location to the <see cref="PreprocessingConstructionStage"/>.
        /// </summary>
        /// <param name="preprocessingConstructionStage">The <see cref="PreprocessingConstructionStage"/>
        /// to add the <see cref="Location"/> to.</param>
        /// <remarks>
        /// This is a workaround to prevent a location missing warning from the kernel.
        /// Should be fixed in a new kernel version.
        /// </remarks>
        private static void AddPreprocessingConstructionStageLocation(PreprocessingConstructionStage preprocessingConstructionStage)
        {
            preprocessingConstructionStage.Locations.Add(new Location
            {
                WaternetCreationMode = WaternetCreationMode.FillInWaternetValues
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