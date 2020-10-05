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
using Deltares.MacroStability.CSharpWrapper;
using Deltares.MacroStability.CSharpWrapper.Output;
using Deltares.MacroStability.CSharpWrapper.Output.WaternetCreator;
using CSharpWrapperWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet
{
    /// <summary>
    /// Class that wraps the <see cref="Deltares.MacroStability.CSharpWrapper"/> for performing a Waternet calculation.
    /// </summary>
    internal class WaternetKernelWrapper : IWaternetKernel
    {
        private readonly ICalculator calculator;
        private readonly IValidator validator;
        private readonly string waternetName;

        /// <summary>
        /// Creates a new instance of <see cref="WaternetKernelWrapper"/>.
        /// </summary>
        /// <param name="calculator">The <see cref="ICalculator"/> to use.</param>
        /// <param name="validator">The <see cref="IValidator"/> to use.</param>
        /// <param name="waternetName">The name of the <see cref="Waternet"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculator"/>
        /// or <paramref name="validator"/> is <c>null</c>.</exception>
        public WaternetKernelWrapper(ICalculator calculator, IValidator validator, string waternetName)
        {
            if (calculator == null)
            {
                throw new ArgumentNullException(nameof(calculator));
            }

            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            this.calculator = calculator;
            this.validator = validator;
            this.waternetName = waternetName;
        }

        public CSharpWrapperWaternet Waternet { get; private set; }

        public void Calculate()
        {
            try
            {
                CheckIfWaternetCanBeGenerated();

                WaternetCreatorOutput output = calculator.CalculateWaternet(0);

                Waternet = output.Waternet;
                Waternet.Name = waternetName;

                ReadLogMessages(output.Messages);
            }
            catch (Exception e) when (!(e is WaternetKernelWrapperException))
            {
                throw new WaternetKernelWrapperException(e.Message, e);
            }
        }

        public IEnumerable<Message> Validate()
        {
            try
            {
                ValidationOutput output = validator.ValidateWaternetCreator();
                return output.Messages;
            }
            catch (Exception e)
            {
                throw new WaternetKernelWrapperException(e.Message, e);
            }
        }

        /// <summary>
        /// Checks if a Waternet can be generated.
        /// </summary>
        /// <exception cref="WaternetKernelWrapperException">Thrown when the Waternet can not be generated.</exception>
        private void CheckIfWaternetCanBeGenerated()
        {
            ValidationOutput output = validator.ValidateWaternetCreator();

            if (!output.IsValid)
            {
                throw new WaternetKernelWrapperException();
            }
        }

        /// <summary>
        /// Reads the log messages of the calculation.
        /// </summary>
        /// <param name="receivedLogMessages">The messages to read.</param>
        /// <exception cref="WaternetKernelWrapperException">Thrown when there
        /// are log messages of the type <see cref="MessageType.Error"/>.</exception>
        private static void ReadLogMessages(IEnumerable<Message> receivedLogMessages)
        {
            Message[] errorMessages = receivedLogMessages.Where(lm => lm.MessageType == MessageType.Error).ToArray();

            if (errorMessages.Any())
            {
                string message = errorMessages.Aggregate(string.Empty,
                                                         (current, logMessage) => current + $"{logMessage.Content}{Environment.NewLine}")
                                              .Trim();

                throw new WaternetKernelWrapperException(message);
            }
        }
    }
}