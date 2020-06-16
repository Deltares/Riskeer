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
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan
{
    /// <summary>
    /// Uplift Van kernel stub for testing purposes.
    /// </summary>
    public class UpliftVanKernelStub : IUpliftVanKernel
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="Calculate"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Validate"/> was called or not.
        /// </summary>
        public bool Validated { get; private set; }

        /// <summary>
        /// Gets or sets an indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        /// <summary>
        /// Gets or sets an indicator whether an exception must be thrown when performing the validation.
        /// </summary>
        public bool ThrowExceptionOnValidate { get; set; }

        /// <summary>
        /// Gets or sets an indicator whether a validation result must be returned when performing the validation.
        /// </summary>
        public bool ReturnValidationResults { get; set; }

        /// <summary>
        /// Gets or sets an indicator whether a log message must be returned when performing the calculation.
        /// </summary>
        public bool ReturnLogMessages { get; set; }

        /// <summary>
        /// Gets the soil model.
        /// </summary>
        public IList<Soil> SoilModel { get; private set; }

        /// <summary>
        /// Gets the soil profile.
        /// </summary>
        public SoilProfile2D SoilProfile { get; private set; }

        /// <summary>
        /// Gets an indicator whether a grid should be moved.
        /// </summary>
        public bool MoveGrid { get; private set; }

        /// <summary>
        /// Gets the maximum slice width.
        /// </summary>
        public double MaximumSliceWidth { get; private set; }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public SurfaceLine2 SurfaceLine { get; private set; }

        /// <summary>
        /// Gets the slip plane uplift van object.
        /// </summary>
        public SlipPlaneUpliftVan SlipPlaneUpliftVan { get; private set; }

        /// <summary>
        /// Gets the slip plane constraints object.
        /// </summary>
        public SlipPlaneConstraints SlipPlaneConstraints { get; private set; }

        /// <summary>
        /// Gets an indicator whether a grid should be automatically determined by the kernel.
        /// </summary>
        public bool GridAutomaticDetermined { get; private set; }

        /// <summary>
        /// Gets an indicator whether tangent lines should be automatically determined by the kernel.
        /// </summary>
        public bool TangentLinesAutomaticDetermined { get; private set; }

        /// <summary>
        /// Gets the Waternet daily.
        /// </summary>
        public WtiStabilityWaternet WaternetDaily { get; private set; }

        /// <summary>
        /// Gets the Waternet extreme.
        /// </summary>
        public WtiStabilityWaternet WaternetExtreme { get; private set; }

        /// <summary>
        /// Gets the fixed soil stresses.
        /// </summary>
        public IEnumerable<FixedSoilStress> SoilStresses { get; private set; }

        /// <summary>
        /// Gets the preconsolidation stresses.
        /// </summary>
        public IEnumerable<PreConsolidationStress> PreConsolidationStresses { get; private set; }

        /// <summary>
        /// Gets an indicator whether forbidden zones should be automatically determined by the kernel.
        /// </summary>
        public bool AutomaticForbiddenZones { get; private set; }

        public double FactorOfStability { get; set; }

        public double ForbiddenZonesXEntryMin { get; set; }

        public double ForbiddenZonesXEntryMax { get; set; }

        public SlidingDualCircle SlidingCurveResult { get; set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; set; }

        public IEnumerable<LogMessage> CalculationMessages { get; private set; }

        public void SetSlipPlaneUpliftVan(SlipPlaneUpliftVan slipPlaneUpliftVan)
        {
            SlipPlaneUpliftVan = slipPlaneUpliftVan;
        }

        public void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints)
        {
            SlipPlaneConstraints = slipPlaneConstraints;
        }

        public void SetSoilModel(IList<Soil> soilModel)
        {
            SoilModel = soilModel;
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            SoilProfile = soilProfile;
        }

        public void SetWaternetDaily(WtiStabilityWaternet waternetDaily)
        {
            WaternetDaily = waternetDaily;
        }

        public void SetWaternetExtreme(WtiStabilityWaternet waternetExtreme)
        {
            WaternetExtreme = waternetExtreme;
        }

        public void SetMoveGrid(bool moveGrid)
        {
            MoveGrid = moveGrid;
        }

        public void SetMaximumSliceWidth(double maximumSliceWidth)
        {
            MaximumSliceWidth = maximumSliceWidth;
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine)
        {
            SurfaceLine = surfaceLine;
        }

        public void SetGridAutomaticDetermined(bool gridAutomaticDetermined)
        {
            GridAutomaticDetermined = gridAutomaticDetermined;
        }

        public void SetTangentLinesAutomaticDetermined(bool tangentLinesAutomaticDetermined)
        {
            TangentLinesAutomaticDetermined = tangentLinesAutomaticDetermined;
        }

        public void SetFixedSoilStresses(IEnumerable<FixedSoilStress> soilStresses)
        {
            SoilStresses = soilStresses;
        }

        public void SetPreConsolidationStresses(IEnumerable<PreConsolidationStress> preConsolidationStresses)
        {
            PreConsolidationStresses = preConsolidationStresses;
        }

        public void SetAutomaticForbiddenZones(bool automaticForbiddenZones)
        {
            AutomaticForbiddenZones = automaticForbiddenZones;
        }

        public void Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new UpliftVanKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            if (ReturnLogMessages)
            {
                CalculationMessages = new[]
                {
                    new LogMessage(LogMessageType.Trace, "subject", "Calculation Trace"),
                    new LogMessage(LogMessageType.Debug, "subject", "Calculation Debug"),
                    new LogMessage(LogMessageType.Info, "subject", "Calculation Info"),
                    new LogMessage(LogMessageType.Warning, "subject", "Calculation Warning"),
                    new LogMessage(LogMessageType.Error, "subject", "Calculation Error"),
                    new LogMessage(LogMessageType.FatalError, "subject", "Calculation Fatal Error")
                };
            }
            else
            {
                CalculationMessages = new LogMessage[0];
            }

            Calculated = true;
        }

        public IEnumerable<IValidationResult> Validate()
        {
            if (ThrowExceptionOnValidate)
            {
                throw new UpliftVanKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            if (ReturnValidationResults)
            {
                yield return new ValidationResult(ValidationResultType.Warning, "Validation Warning");
                yield return new ValidationResult(ValidationResultType.Error, "Validation Error");
                yield return new ValidationResult(ValidationResultType.Info, "Validation Info");
                yield return new ValidationResult(ValidationResultType.Debug, "Validation Debug");
            }

            Validated = true;
        }
    }
}