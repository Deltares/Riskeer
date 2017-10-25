// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Deltares.WTIStability.Data.Standard;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class UpliftVanKernelMessagesCreatorTest
    {
        [Test]
        public void Create_LogMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanKernelMessagesCreator.Create(null).ToList();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("logMessages", exception.ParamName);
        }

        [Test]
        public void Create_WithLogMessages_ReturnUpliftVanKernelMessages()
        {
            // Setup
            var logMessages = new[]
            {
                new LogMessage(LogMessageType.Trace, "subject", "Calculation Trace"),
                new LogMessage(LogMessageType.Debug, "subject", "Calculation Debug"),
                new LogMessage(LogMessageType.Info, "subject", "Calculation Info"),
                new LogMessage(LogMessageType.Warning, "subject", "Calculation Warning"),
                new LogMessage(LogMessageType.Error, "subject", "Calculation Error"),
                new LogMessage(LogMessageType.FatalError, "subject", "Calculation Fatal Error")
            };

            // Call
            IEnumerable<UpliftVanKernelMessage> kernelMessages = UpliftVanKernelMessagesCreator.Create(logMessages).ToList();

            // Assert
            Assert.AreEqual(3, kernelMessages.Count());
            Assert.AreEqual("Calculation Warning", kernelMessages.ElementAt(0).Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Warning, kernelMessages.ElementAt(0).ResultType);
            Assert.AreEqual("Calculation Error", kernelMessages.ElementAt(1).Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, kernelMessages.ElementAt(1).ResultType);
            Assert.AreEqual("Calculation Fatal Error", kernelMessages.ElementAt(2).Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, kernelMessages.ElementAt(2).ResultType);
        }
    }
}