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
using System.Linq;
using Core.Common.TestUtil;
using log4net.Core;
using NUnit.Framework;
using Riskeer.Common.Service.TestUtil;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class CalculationServiceHelperTest
    {
        [Test]
        public void LogMessagesAsErrorWithFormat_FormatNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CalculationServiceHelper.LogMessagesAsError(null, new string[0]);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("format", exception.ParamName);
        }

        [Test]
        public void LogMessagesAsErrorWithFormat_ErrorMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CalculationServiceHelper.LogMessagesAsError("", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("errorMessages", exception.ParamName);
        }

        [Test]
        public void LogMessagesAsErrorWithFormat_WithFormatAndErrorMessages_LogsMessagesAsErrorsInGivenFormat()
        {
            // Setup
            const string format = "Message: {0}";
            var errorMessages = new[]
            {
                "Test 1",
                "Test 2"
            };

            // Call
            Action call = () => CalculationServiceHelper.LogMessagesAsError(format, errorMessages);

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new[]
            {
                Tuple.Create(string.Format(format, errorMessages[0]), LogLevelConstant.Error),
                Tuple.Create(string.Format(format, errorMessages[1]), LogLevelConstant.Error)
            }, 2);
        }

        [Test]
        public void LogMessagesAsErrorWithoutFormat_ErrorMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CalculationServiceHelper.LogMessagesAsError(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("errorMessages", exception.ParamName);
        }

        [Test]
        public void LogMessagesAsErrorWithoutFormat_WithErrorMessages_LogsMessagesAsErrors()
        {
            // Setup
            var errorMessages = new[]
            {
                "Test 1",
                "Test 2"
            };

            // Call
            Action call = () => CalculationServiceHelper.LogMessagesAsError(errorMessages);

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new[]
            {
                Tuple.Create(errorMessages[0], LogLevelConstant.Error),
                Tuple.Create(errorMessages[1], LogLevelConstant.Error)
            }, 2);
        }

        [Test]
        public void LogMessagesAsWarning_WarningMessagesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CalculationServiceHelper.LogMessagesAsWarning(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("warningMessages", exception.ParamName);
        }

        [Test]
        public void LogMessagesAsWarning_WithWarningMessages_LogsMessagesAsWarnings()
        {
            // Setup
            var warningMessages = new[]
            {
                "Test 1",
                "Test 2"
            };

            // Call
            Action call = () => CalculationServiceHelper.LogMessagesAsWarning(warningMessages);

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new[]
            {
                Tuple.Create(warningMessages[0], LogLevelConstant.Warn),
                Tuple.Create(warningMessages[1], LogLevelConstant.Warn)
            }, 2);
        }

        [Test]
        public void LogValidationBegin_Always_LogsValidationBegin()
        {
            // Call
            Action call = CalculationServiceHelper.LogValidationBegin;

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(msgs[0]);
            });
        }

        [Test]
        public void LogValidationEnd_Always_LogsValidationEnd()
        {
            // Call
            Action call = CalculationServiceHelper.LogValidationEnd;

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertValidationEndMessage(msgs[0]);
            });
        }

        [Test]
        public void LogCalculationBegin_Always_LogsCalculationBegin()
        {
            // Call
            Action call = CalculationServiceHelper.LogCalculationBegin;

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertCalculationStartMessage(msgs[0]);
            });
        }

        [Test]
        public void LogCalculationEnd_Always_LogsCalculationEnd()
        {
            // Call
            Action call = CalculationServiceHelper.LogCalculationEnd;

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertCalculationEndMessage(msgs[0]);
            });
        }

        [Test]
        public void HasErrorOccurred_CalculationNotCanceledOrExceptionThrownLastErrorSet_ReturnTrue()
        {
            // Call
            bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(false, false, "An error has occurred.");

            // Assert
            Assert.IsTrue(errorOccurred);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void HasErrorOccurred_CalculationNotCanceledOrExceptionThrownLastErrorNotSet_ReturnFalse(string errorContent)
        {
            // Call
            bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(false, false, errorContent);

            // Assert
            Assert.IsFalse(errorOccurred);
        }

        [Test]
        [TestCase(true, true, "An error has occurred.")]
        [TestCase(true, false, "An error has occurred.")]
        [TestCase(false, true, "An error has occurred.")]
        [TestCase(true, true, "")]
        [TestCase(true, false, "")]
        [TestCase(false, true, "")]
        public void HasErrorOccurred_LastErrorSetCalculationCanceledOrExceptionThrown_ReturnFalse(bool canceled, bool exceptionThrown, string errorContent)
        {
            // Call
            bool errorOccurred = CalculationServiceHelper.HasErrorOccurred(canceled, exceptionThrown, errorContent);

            // Assert
            Assert.IsFalse(errorOccurred);
        }

        [Test]
        public void LogExceptionAsError_MessageNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CalculationServiceHelper.LogExceptionAsError(null, new Exception());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("message", exception.ParamName);
        }

        [Test]
        public void LogExceptionAsError_ExceptionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CalculationServiceHelper.LogExceptionAsError("message", null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("exception", exception.ParamName);
        }

        [Test]
        public void LogExceptionAsError_WithParameters_LogMessageAndException()
        {
            // Setup
            const string message = "Message";
            var exception = new Exception();

            // Call
            Action call = () => CalculationServiceHelper.LogExceptionAsError(message, exception);

            // Assert
            TestHelper.AssertLogMessagesWithLevelAndLoggedExceptions(call, tuples =>
            {
                Tuple<string, Level, Exception> tuple = tuples.Single();
                Assert.AreEqual(message, tuple.Item1);
                Assert.AreEqual(Level.Error, tuple.Item2);
                Assert.AreSame(exception, tuple.Item3);
            });
        }
    }
}