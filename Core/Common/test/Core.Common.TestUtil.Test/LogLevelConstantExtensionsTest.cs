// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.ComponentModel;
using log4net.Core;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class LogLevelConstantExtensionsTest
    {
        [Test]
        [TestCaseSource(nameof(GetMappedLogLevelConstants))]
        public void ToLog4NetLevel_SupportedLogLevelConstants_ReturnsExpectedLevel(LogLevelConstant logLevelConstant,
                                                                                   Level expectedLevel)
        {
            // Call
            Level level = logLevelConstant.ToLog4NetLevel();

            // Assert
            Assert.AreEqual(expectedLevel, level);
        }

        [Test]
        public void ToLog4NetLevel_InvalidLogLevelConstant_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;
            const LogLevelConstant invalidLogLevelConstant = (LogLevelConstant) invalidValue;

            // Call
            TestDelegate call = () => invalidLogLevelConstant.ToLog4NetLevel();

            // Assert
            string exoectedMessage = $"The value of argument 'level' ({invalidValue}) is invalid for Enum type '{nameof(LogLevelConstant)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, exoectedMessage).ParamName;
            Assert.AreEqual("level", parameterName);
        }

        private static IEnumerable<TestCaseData> GetMappedLogLevelConstants()
        {
            yield return new TestCaseData(LogLevelConstant.Off, Level.Off);
            yield return new TestCaseData(LogLevelConstant.Fatal, Level.Fatal);
            yield return new TestCaseData(LogLevelConstant.Error, Level.Error);
            yield return new TestCaseData(LogLevelConstant.Warn, Level.Warn);
            yield return new TestCaseData(LogLevelConstant.Info, Level.Info);
            yield return new TestCaseData(LogLevelConstant.Debug, Level.Debug);
        }
    }
}