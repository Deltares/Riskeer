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
using Deltares.MacroStability.CSharpWrapper.Input;
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet
{
    /// <summary>
    /// Waternet kernel stub for testing purposes.
    /// </summary>
    public class WaternetKernelStub : IWaternetKernel
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
        /// Gets the <see cref="MacroStabilityInput"/>.
        /// </summary>
        public MacroStabilityInput KernelInput { get; private set; }

        public WtiStabilityWaternet Waternet { get; set; }

        /// <summary>
        /// Sets the <see cref="MacroStabilityInput"/>.
        /// </summary>
        /// <param name="input">The input to set.</param>
        public void SetInput(MacroStabilityInput input)
        {
            KernelInput = input;
        }

        public void Calculate()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new WaternetKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            Calculated = true;
        }

        public IEnumerable<Message> Validate()
        {
            if (ThrowExceptionOnValidate)
            {
                throw new WaternetKernelWrapperException($"Message 1{Environment.NewLine}Message 2", new Exception());
            }

            if (ReturnValidationResults)
            {
                yield return MessageHelper.CreateMessage(MessageType.Warning, "Validation Warning");
                yield return MessageHelper.CreateMessage(MessageType.Error, "Validation Error");
                yield return MessageHelper.CreateMessage(MessageType.Info, "Validation Info");
            }

            Validated = true;
        }
    }
}