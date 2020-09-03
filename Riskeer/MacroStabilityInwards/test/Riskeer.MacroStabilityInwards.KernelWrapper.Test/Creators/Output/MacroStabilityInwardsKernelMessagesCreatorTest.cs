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
using Deltares.MacroStability.CSharpWrapper.Output;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class MacroStabilityInwardsKernelMessagesCreatorTest
    {
        [Test]
        public void Create_MessagesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsKernelMessagesCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("messages", exception.ParamName);
        }

        [Test]
        public void Create_WithMessages_ReturnOnlyWarningAndErrorUpliftVanKernelMessages()
        {
            // Setup
            Message[] logMessages =
            {
                CreateMessage(MessageType.Info, "Calculation Info"),
                CreateMessage(MessageType.Warning, "Calculation Warning"),
                CreateMessage(MessageType.Error, "Calculation Error")
            };

            // Call
            IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = MacroStabilityInwardsKernelMessagesCreator.Create(logMessages);

            // Assert
            Assert.AreEqual(3, kernelMessages.Count());
            MacroStabilityInwardsKernelMessage firstMessage = kernelMessages.ElementAt(0);
            Assert.AreEqual("Calculation Warning", firstMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Warning, firstMessage.Type);

            MacroStabilityInwardsKernelMessage secondMessage = kernelMessages.ElementAt(1);
            Assert.AreEqual("Calculation Error", secondMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, secondMessage.Type);

            MacroStabilityInwardsKernelMessage thirdMessage = kernelMessages.ElementAt(2);
            Assert.AreEqual("Calculation Fatal Error", thirdMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, thirdMessage.Type);
        }

        [Test]
        public void Create_MessageTextNull_ReturnsUpliftVanKernelMessageWithUnknownText()
        {
            // Setup
            Message[] logMessages =
            {
                CreateMessage(MessageType.Error, null)
            };

            // Call
            IEnumerable<MacroStabilityInwardsKernelMessage> kernelMessages = MacroStabilityInwardsKernelMessagesCreator.Create(logMessages);

            // Assert
            MacroStabilityInwardsKernelMessage kernelMessage = kernelMessages.Single();
            Assert.AreEqual("Onbekend", kernelMessage.Message);
            Assert.AreEqual(MacroStabilityInwardsKernelMessageType.Error, kernelMessage.Type);
        }

        private static Message CreateMessage(MessageType messageType, string message)
        {
            return new Message
            {
                MessageType = messageType,
                Content = message
            };
        }
    }
}