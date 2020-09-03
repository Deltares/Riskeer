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
using Deltares.MacroStability.CSharpWrapper.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet
{
    /// <summary>
    /// Class representing a Waternet calculator.
    /// </summary>
    public abstract class WaternetCalculator : IWaternetCalculator
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaternetCalculator"/>.
        /// </summary>
        /// <param name="input">The <see cref="WaternetCalculatorInput"/> containing all the values
        /// required for performing the Waternet calculation.</param>
        /// <param name="factory">The factory responsible for creating the Waternet kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        protected WaternetCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            Input = input;
            Factory = factory;
        }

        public WaternetCalculatorResult Calculate()
        {
            IWaternetKernel waternetKernel = CalculateWaternet();

            return WaternetCalculatorResultCreator.Create(waternetKernel.Waternet);
        }

        public IEnumerable<MacroStabilityInwardsKernelMessage> Validate()
        {
            IWaternetKernel waternetKernel = GetWaternetKernel();

            try
            {
                return MacroStabilityInwardsKernelMessagesCreator.Create(waternetKernel.Validate().ToArray());
            }
            catch (WaternetKernelWrapperException e)
            {
                throw new WaternetCalculatorException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the factory responsible for creating the Waternet kernel.
        /// </summary>
        protected IMacroStabilityInwardsKernelFactory Factory { get; }

        /// <summary>
        /// Creates a Waternet kernel.
        /// </summary>
        /// <param name="location">The location to create the Waternet for.</param>
        /// <returns>The created <see cref="IWaternetKernel"/>.</returns>
        protected abstract IWaternetKernel CreateWaternetKernel(MacroStabilityInput kernelInput);

        private WaternetCalculatorInput Input { get; }

        /// <summary>
        /// Performs the Waternet calculation.
        /// </summary>
        /// <returns>The Waternet kernel with output set.</returns>
        /// <exception cref="WaternetCalculatorException">Thrown when the Waternet
        /// kernel throws a <see cref="WaternetKernelWrapperException"/>.</exception>
        private IWaternetKernel CalculateWaternet()
        {
            IWaternetKernel waternetKernel = GetWaternetKernel();

            try
            {
                waternetKernel.Calculate();
            }
            catch (WaternetKernelWrapperException e)
            {
                throw new WaternetCalculatorException(e.Message, e);
            }

            return waternetKernel;
        }

        private IWaternetKernel GetWaternetKernel()
        {
            MacroStabilityInput kernelInput = MacroStabilityInputCreator.CreateWaternet(Input);
            return CreateWaternetKernel(kernelInput);
        }
    }
}