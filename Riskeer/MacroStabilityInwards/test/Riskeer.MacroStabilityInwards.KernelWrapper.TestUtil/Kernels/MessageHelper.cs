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

using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels
{
    /// <summary>
    /// Helper class for creating and asserting instances of <see cref="Message"/>.
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="Message"/>.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <param name="message">The content of the message.</param>
        /// <returns>The created <see cref="Message"/>.</returns>
        public static Message CreateMessage(MessageType messageType, string message)
        {
            return new Message
            {
                MessageType = messageType,
                Content = message
            };
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="Message"/>.</param>
        /// <param name="actual">The actual <see cref="Message"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertMessage(Message expected, Message actual)
        {
            Assert.AreEqual(expected.MessageType, actual.MessageType);
            Assert.AreEqual(expected.Content, actual.Content);
        }
    }
}
