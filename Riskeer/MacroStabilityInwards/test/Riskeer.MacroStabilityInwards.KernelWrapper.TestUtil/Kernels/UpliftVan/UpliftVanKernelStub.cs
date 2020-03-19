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
using Deltares.MacroStability.WaternetCreator;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

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
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the validation.
        /// </summary>
        public bool ThrowExceptionOnValidate { get; set; }

        /// <summary>
        /// Indicator whether a validation result must be returned when performing the validation.
        /// </summary>
        public bool ReturnValidationResults { get; set; }

        /// <summary>
        /// Indicator whether a log message must be returned when performing the calculation.
        /// </summary>
        public bool ReturnLogMessages { get; set; }

        public SoilModel SoilModel { get; private set; }

        public SoilProfile2D SoilProfile { get; private set; }

        public Location LocationExtreme { get; private set; }

        public Location LocationDaily { get; private set; }

        public bool MoveGrid { get; private set; }

        public double MaximumSliceWidth { get; private set; }

        public SurfaceLine2 SurfaceLine { get; private set; }

        public SlipPlaneUpliftVan SlipPlaneUpliftVan { get; private set; }

        public SlipPlaneConstraints SlipPlaneConstraints { get; private set; }

        public bool GridAutomaticDetermined { get; private set; }

        public double FactorOfStability { get; set; }

        public double ZValue { get; set; }

        public double ForbiddenZonesXEntryMin { get; set; }

        public double ForbiddenZonesXEntryMax { get; set; }

        public SlidingDualCircle SlidingCurveResult { get; set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; set; }

        public IEnumerable<LogMessage> CalculationMessages { get; set; }

        public void SetSoilModel(SoilModel soilModel)
        {
            SoilModel = soilModel;
        }

        public void SetSoilProfile(SoilProfile2D soilProfile)
        {
            SoilProfile = soilProfile;
        }

        public void SetLocationExtreme(Location stabilityLocation)
        {
            LocationExtreme = stabilityLocation;
        }

        public void SetLocationDaily(Location stabilityLocation)
        {
            LocationDaily = stabilityLocation;
        }

        public void SetSurfaceLine(SurfaceLine2 surfaceLine2)
        {
            SurfaceLine = surfaceLine2;
        }

        public void SetMoveGrid(bool moveGrid)
        {
            MoveGrid = moveGrid;
        }

        public void SetMaximumSliceWidth(double maximumSliceWidth)
        {
            MaximumSliceWidth = maximumSliceWidth;
        }

        public void SetSlipPlaneUpliftVan(SlipPlaneUpliftVan slipPlaneUpliftVan)
        {
            SlipPlaneUpliftVan = slipPlaneUpliftVan;
        }

        public void SetSlipPlaneConstraints(SlipPlaneConstraints slipPlaneConstraints)
        {
            SlipPlaneConstraints = slipPlaneConstraints;
        }

        public void SetGridAutomaticDetermined(bool gridAutomaticDetermined)
        {
            GridAutomaticDetermined = gridAutomaticDetermined;
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

        public IEnumerable<ValidationResult> Validate()
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