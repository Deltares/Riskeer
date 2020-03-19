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
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class UpliftVanKernelMessagesCreatorTest
    {
        [Test]
        public void CreateFromLogMessages_LogMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanKernelMessagesCreator.CreateFromLogMessages(null).ToList();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("logMessages", exception.ParamName);
        }

        [Test]
        public void CreateFromLogMessages_WithLogMessages_ReturnOnlyWarningAndErrorUpliftVanKernelMessages()
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
            IEnumerable<UpliftVanKernelMessage> kernelMessages = UpliftVanKernelMessagesCreator.CreateFromLogMessages(logMessages).ToList();

            // Assert
            Assert.AreEqual(3, kernelMessages.Count());
            UpliftVanKernelMessage firstMessage = kernelMessages.ElementAt(0);
            Assert.AreEqual("Calculation Warning", firstMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Warning, firstMessage.ResultType);

            UpliftVanKernelMessage secondMessage = kernelMessages.ElementAt(1);
            Assert.AreEqual("Calculation Error", secondMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, secondMessage.ResultType);

            UpliftVanKernelMessage thirdMessage = kernelMessages.ElementAt(2);
            Assert.AreEqual("Calculation Fatal Error", thirdMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, thirdMessage.ResultType);
        }

        [Test]
        public void CreateFromLogMessages_LogMessageTextNull_ReturnsUpliftVanKernelMessageWithUnknownText()
        {
            // Setup
            var logMessages = new[]
            {
                new LogMessage(LogMessageType.Error, "subject", null)
            };

            // Call
            IEnumerable<UpliftVanKernelMessage> kernelMessages = UpliftVanKernelMessagesCreator.CreateFromLogMessages(logMessages).ToList();

            // Assert
            UpliftVanKernelMessage kernelMessage = kernelMessages.Single();
            Assert.AreEqual("Onbekend", kernelMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, kernelMessage.ResultType);
        }

        [Test]
        public void CreateFromValidationResults_ValidationResultsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanKernelMessagesCreator.CreateFromValidationResults(null).ToList();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("validationResults", exception.ParamName);
        }

        [Test]
        public void CreateFromValidationResults_WithValidationResults_ReturnsOnlyWarningAndErrorUpliftVanKernelMessages()
        {
            // Setup
            var validationResults = new[]
            {
                new ValidationResult(ValidationResultType.Debug, "Validation Debug"),
                new ValidationResult(ValidationResultType.Info, "Validation Info"),
                new ValidationResult(ValidationResultType.Warning, "Validation Warning"),
                new ValidationResult(ValidationResultType.Error, "Validation Error")
            };

            // Call
            IEnumerable<UpliftVanKernelMessage> kernelMessages = UpliftVanKernelMessagesCreator.CreateFromValidationResults(validationResults).ToList();

            // Assert
            Assert.AreEqual(2, kernelMessages.Count());
            UpliftVanKernelMessage firstMessage = kernelMessages.ElementAt(0);
            Assert.AreEqual("Validation Warning", firstMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Warning, firstMessage.ResultType);

            UpliftVanKernelMessage secondMessage = kernelMessages.ElementAt(1);
            Assert.AreEqual("Validation Error", secondMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, secondMessage.ResultType);
        }

        [Test]
        public void CreateFromValidationResults_ValidationResultMessageNull_ReturnsUpliftVanKernelMessageWithUnknownText()
        {
            // Setup
            var validationResults = new[]
            {
                new ValidationResult(ValidationResultType.Error, null)
            };

            // Call
            IEnumerable<UpliftVanKernelMessage> kernelMessages = UpliftVanKernelMessagesCreator.CreateFromValidationResults(validationResults).ToList();

            // Assert
            UpliftVanKernelMessage kernelMessage = kernelMessages.Single();
            Assert.AreEqual("Onbekend", kernelMessage.Message);
            Assert.AreEqual(UpliftVanKernelMessageType.Error, kernelMessage.ResultType);
        }
    }
}