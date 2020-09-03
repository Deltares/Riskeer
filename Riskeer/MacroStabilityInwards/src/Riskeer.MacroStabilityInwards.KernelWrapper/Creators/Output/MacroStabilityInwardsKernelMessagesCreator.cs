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
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
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
        /// based on the <see cref="Message"/> given in the <paramref name="messages"/>.
        /// </summary>
        /// <param name="messages">The messages to create the Uplift Van kernel messages for.</param>
        /// <returns>A new <see cref="IEnumerable{T}"/> of <see cref="MacroStabilityInwardsKernelMessage"/> with information
        /// taken from the <paramref name="messages"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="messages"/>
        /// is <c>null</c>.</exception>
        public static IEnumerable<MacroStabilityInwardsKernelMessage> Create(IEnumerable<Message> messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            return CreateLogMessages(messages);
        }

        private static IEnumerable<MacroStabilityInwardsKernelMessage> CreateLogMessages(IEnumerable<Message> messages)
        {
            foreach (Message message in messages)
            {
                MacroStabilityInwardsKernelMessageType type;
                switch (message.MessageType)
                {
                    case MessageType.Error:
                        type = MacroStabilityInwardsKernelMessageType.Error;
                        break;
                    case MessageType.Warning:
                        type = MacroStabilityInwardsKernelMessageType.Warning;
                        break;
                    default:
                        continue;
                }

                yield return CreateMessage(type, message.Content);
            }
        }

        private static MacroStabilityInwardsKernelMessage CreateMessage(MacroStabilityInwardsKernelMessageType type, string message)
        {
            return new MacroStabilityInwardsKernelMessage(type, message ?? Resources.UpliftVanKernelMessagesCreator_Create_Unknown);
        }
    }
}