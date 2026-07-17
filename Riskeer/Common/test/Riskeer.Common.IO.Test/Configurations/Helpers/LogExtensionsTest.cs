// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using log4net;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.IO.Configurations.Helpers;

namespace Riskeer.Common.IO.Test.Configurations.Helpers
{
    [TestFixture]
    public class LogExtensionsTest
    {
        [Test]
        public void LogOutOfRangeException_Always_LogMessage()
        {
            // Setup
            const string message = "an error";
            const string calculationName = "calculationA";
            const string innerMessage = "Inner message";
            var log = Substitute.For<ILog>();
            log.ErrorFormat("{0} Berekening '{1}' is overgeslagen.", $"{message} {innerMessage}", calculationName);
            var exception = new ArgumentOutOfRangeException(null, innerMessage);

            // Call
            log.LogOutOfRangeException(message, calculationName, exception);

            // Assert
            log.Received().ErrorFormat("{0} Berekening '{1}' is overgeslagen.", $"{message} {innerMessage}", calculationName);
        }

        [Test]
        public void LogCalculationConversionError_Always_LogMessage()
        {
            // Setup
            const string message = "an error";
            const string calculationName = "calculationA";
            var log = Substitute.For<ILog>();
            log.ErrorFormat("{0} Berekening '{1}' is overgeslagen.", message, calculationName);
            
            // Call
            log.LogCalculationConversionError(message, calculationName);

            // Assert
            log.Received().ErrorFormat("{0} Berekening '{1}' is overgeslagen.", message, calculationName);
        }
    }
}