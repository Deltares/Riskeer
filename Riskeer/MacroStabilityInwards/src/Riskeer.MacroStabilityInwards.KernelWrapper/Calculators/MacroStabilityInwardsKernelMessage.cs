﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.ComponentModel;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators
{
    /// <summary>
    /// Class representing a message returned by the macro stability inwards kernel.
    /// </summary>
    public class MacroStabilityInwardsKernelMessage
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsKernelMessage"/>.
        /// </summary>
        /// <param name="type">The type of the message.</param>
        /// <param name="message">The text of the message.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="type"/>
        /// contains an invalid value for <see cref="MacroStabilityInwardsKernelMessageType"/>.</exception>
        public MacroStabilityInwardsKernelMessage(MacroStabilityInwardsKernelMessageType type, string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!Enum.IsDefined(typeof(MacroStabilityInwardsKernelMessageType), type))
            {
                throw new InvalidEnumArgumentException(nameof(type), (int) type,
                                                       typeof(MacroStabilityInwardsKernelMessageType));
            }

            Type = type;
            Message = message;
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public MacroStabilityInwardsKernelMessageType Type { get; }

        /// <summary>
        /// Gets the text of the message.
        /// </summary>
        public string Message { get; }
    }
}