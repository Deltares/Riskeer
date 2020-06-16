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
using Deltares.MacroStability.Standard;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Properties;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output
{
    /// <summary>
    /// Creates an <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsKernelMessage"/> instances.
    /// </summary>
    internal static class MacroStabilityInwardsKernelMessagesCreator
    {
        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsKernelMessage"/> 
        /// based on the <see cref="LogMessage"/> given in the <paramref name="logMessages"/>.
        /// </summary>
        /// <param name="logMessages">The log messages to create the Uplift Van kernel messages for.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsKernelMessage"/> with information
        /// taken from the <paramref name="logMessages"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="logMessages"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MacroStabilityInwardsKernelMessage> CreateFromLogMessages(IEnumerable<LogMessage> logMessages)
        {
            if (logMessages == null)
            {
                throw new ArgumentNullException(nameof(logMessages));
            }

            return CreateLogMessages(logMessages);
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsKernelMessage"/> 
        /// based on the <see cref="IValidationResult"/> given in the <paramref name="validationResults"/>.
        /// </summary>
        /// <param name="validationResults">The validation results to create the Uplift Van kernel messages for.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsKernelMessage"/> with information
        /// taken from the <paramref name="validationResults"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validationResults"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MacroStabilityInwardsKernelMessage> CreateFromValidationResults(IEnumerable<IValidationResult> validationResults)
        {
            if (validationResults == null)
            {
                throw new ArgumentNullException(nameof(validationResults));
            }

            return CreateValidationMessages(validationResults).ToArray();
        }

        private static IEnumerable<MacroStabilityInwardsKernelMessage> CreateLogMessages(IEnumerable<LogMessage> logMessages)
        {
            foreach (LogMessage logMessage in logMessages)
            {
                MacroStabilityInwardsKernelMessageType type;
                switch (logMessage.MessageType)
                {
                    case LogMessageType.Error:
                    case LogMessageType.FatalError:
                        type = MacroStabilityInwardsKernelMessageType.Error;
                        break;
                    case LogMessageType.Warning:
                        type = MacroStabilityInwardsKernelMessageType.Warning;
                        break;
                    default:
                        continue;
                }

                yield return CreateMessage(type, logMessage.Message);
            }
        }

        private static IEnumerable<MacroStabilityInwardsKernelMessage> CreateValidationMessages(IEnumerable<IValidationResult> validationResults)
        {
            foreach (IValidationResult logMessage in validationResults)
            {
                MacroStabilityInwardsKernelMessageType type;
                switch (logMessage.MessageType)
                {
                    case ValidationResultType.Error:
                        type = MacroStabilityInwardsKernelMessageType.Error;
                        break;
                    case ValidationResultType.Warning:
                        type = MacroStabilityInwardsKernelMessageType.Warning;
                        break;
                    default:
                        continue;
                }

                yield return CreateMessage(type, logMessage.Text);
            }
        }

        private static MacroStabilityInwardsKernelMessage CreateMessage(MacroStabilityInwardsKernelMessageType type, string message)
        {
            return new MacroStabilityInwardsKernelMessage(type, message ?? Resources.UpliftVanKernelMessagesCreator_Create_Unknown);
        }
    }
}