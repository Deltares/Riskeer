// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Deltares.WTIStability;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.Data.Standard;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan
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

        public SoilModel SoilModel { get; set; }

        public SoilProfile2D SoilProfile { get; set; }

        public StabilityLocation LocationExtreme { get; set; }

        public StabilityLocation LocationDaily { get; set; }

        public bool MoveGrid { get; set; }

        public double MaximumSliceWidth { get; set; }

        public SurfaceLine2 SurfaceLine { get; set; }

        public SlipPlaneUpliftVan SlipPlaneUpliftVan { get; set; }

        public SlipPlaneConstraints SlipPlaneConstraints { get; set; }

        public bool GridAutomaticDetermined { get; set; }

        public double FactorOfStability { get; set; }

        public double ZValue { get; set; }

        public double ForbiddenZonesXEntryMin { get; set; }

        public double ForbiddenZonesXEntryMax { get; set; }

        public SlidingDualCircle SlidingCurveResult { get; set; }

        public SlipPlaneUpliftVan SlipPlaneResult { get; set; }

        public IEnumerable<LogMessage> CalculationMessages { get; set; }

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