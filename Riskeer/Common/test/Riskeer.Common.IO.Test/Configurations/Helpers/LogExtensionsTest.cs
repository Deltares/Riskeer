// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Ringtoets.Common.IO.Test.Configurations.Helpers
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

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            log.Expect(l => l.ErrorFormat("{0} Berekening '{1}' is overgeslagen.", $"{message} {innerMessage}", calculationName));
            mocks.ReplayAll();

            var exception = new ArgumentOutOfRangeException(null, innerMessage);

            // Call
            log.LogOutOfRangeException(message, calculationName, exception);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void LogCalculationConversionError_Always_LogMessage()
        {
            // Setup
            const string message = "an error";
            const string calculationName = "calculationA";

            var mocks = new MockRepository();
            var log = mocks.StrictMock<ILog>();
            log.Expect(l => l.ErrorFormat("{0} Berekening '{1}' is overgeslagen.", message, calculationName));
            mocks.ReplayAll();

            // Call
            log.LogCalculationConversionError(message, calculationName);

            // Assert
            mocks.VerifyAll();
        }
    }
}