﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan
{
    /// <summary>
    /// Uplift Van calculator stub for testing purposes.
    /// </summary>
    public class UpliftVanCalculatorStub : IUpliftVanCalculator
    {
        /// <summary>
        /// Gets or sets the Uplift Van calculator input.
        /// </summary>
        public UpliftVanCalculatorInput Input { get; set; }

        /// <summary>
        /// Gets the Uplift Van calculator output.
        /// </summary>
        public UpliftVanCalculatorResult Output { get; private set; }

        /// <summary>
        /// Indicator whether an error message must be returned when performing the calculation.
        /// </summary>
        public bool ReturnCalculationError { get; set; }

        /// <summary>
        /// Indicator whether a warning message must be returned when performing the calculation.
        /// </summary>
        public bool ReturnCalculationWarning { get; set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { get; set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the validation.
        /// </summary>
        public bool ThrowExceptionOnValidate { get; set; }

        /// <summary>
        /// Indicator whether an error message must be returned when performing the validation.
        /// </summary>
        public bool ReturnValidationError { get; set; }

        /// <summary>
        /// Indicator whether a warning message must be returned when performing the validation.
        /// </summary>
        public bool ReturnValidationWarning { get; set; }

        public UpliftVanCalculatorResult Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new UpliftVanCalculatorException($"Message 1{Environment.NewLine}Message 2");
            }

            var calculationMessages = new List<UpliftVanKernelMessage>();

            if (ReturnCalculationError)
            {
                calculationMessages.Add(new UpliftVanKernelMessage(UpliftVanKernelMessageType.Error, "Calculation Error 1"));
            }

            if (ReturnCalculationWarning)
            {
                calculationMessages.Add(new UpliftVanKernelMessage(UpliftVanKernelMessageType.Warning, "Calculation Warning 1"));
            }

            if (ReturnCalculationError)
            {
                calculationMessages.Add(new UpliftVanKernelMessage(UpliftVanKernelMessageType.Error, "Calculation Error 2"));
            }

            if (ReturnCalculationWarning)
            {
                calculationMessages.Add(new UpliftVanKernelMessage(UpliftVanKernelMessageType.Warning, "Calculation Warning 2"));
            }

            return Output ?? (Output = CreateUpliftVanCalculatorResult(calculationMessages));
        }

        public IEnumerable<UpliftVanKernelMessage> Validate()
        {
            if (ThrowExceptionOnValidate)
            {
                throw new UpliftVanCalculatorException($"Message 1{Environment.NewLine}Message 2");
            }

            if (ReturnValidationError)
            {
                yield return new UpliftVanKernelMessage(UpliftVanKernelMessageType.Error, "Validation Error");
            }

            if (ReturnValidationWarning)
            {
                yield return new UpliftVanKernelMessage(UpliftVanKernelMessageType.Warning, "Validation Warning");
            }
        }

        private static UpliftVanCalculatorResult CreateUpliftVanCalculatorResult(IEnumerable<UpliftVanKernelMessage> calculationMessages)
        {
            return new UpliftVanCalculatorResult(
                UpliftVanSlidingCurveResultTestFactory.Create(),
                new UpliftVanCalculationGridResult(
                    UpliftVanGridTestFactory.Create(),
                    UpliftVanGridTestFactory.Create(),
                    new[]
                    {
                        3,
                        2,
                        1.5
                    }),
                calculationMessages,
                new UpliftVanCalculatorResult.ConstructionProperties
                {
                    FactorOfStability = 0.1,
                    ZValue = 0.2,
                    ForbiddenZonesXEntryMin = 0.3,
                    ForbiddenZonesXEntryMax = 0.4
                });
        }
    }
}