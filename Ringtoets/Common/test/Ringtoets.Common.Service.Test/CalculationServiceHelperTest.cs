// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Globalization;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class CalculationServiceHelperTest
    {
        [Test]
        public void LogMessagesAsError_Always_LogsMessagesInGivenFormat()
        {
            // Setup
            var format = "Message: {0}";
            var errorMessages = new[]
            {
                "Test 1",
                "Test 2"
            };

            // Call
            Action call = () => CalculationServiceHelper.LogMessagesAsError(format, errorMessages);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format(format, errorMessages[0]), msgs[0]);
                StringAssert.StartsWith(string.Format(format, errorMessages[1]), msgs[1]);
            });
        }

        [Test]
        public void LogValidationBeginTime_Always_LogsValidationBeginTime()
        {
            // Setup
            var name = "Test name";
            DateTime dateTime = DateTime.Now;

            // Call
            Action call = () => CalculationServiceHelper.LogValidationBeginTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om:", name), msgs[0]);

                AssertLogTime(msgs[0], dateTime);
            });
        }

        [Test]
        public void LogValidationEndTime_Always_LogsValidationEndTime()
        {
            // Setup
            var name = "Test name";
            DateTime dateTime = DateTime.Now;

            // Call
            Action call = () => CalculationServiceHelper.LogValidationEndTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om:", name), msgs[0]);

                AssertLogTime(msgs[0], dateTime);
            });
        }

        [Test]
        public void LogCalculationBeginTime_Always_LogsCalculationBeginTime()
        {
            // Setup
            var name = "Test name";
            DateTime dateTime = DateTime.Now;

            // Call
            Action call = () => CalculationServiceHelper.LogCalculationBeginTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om:", name), msgs[0]);

                AssertLogTime(msgs[0], dateTime);
            });
        }

        [Test]
        public void LogCalculationEndTime_Always_LogsCalculationEndTime()
        {
            // Setup
            var name = "Test name";
            DateTime dateTime = DateTime.Now;

            // Call
            Action call = () => CalculationServiceHelper.LogCalculationEndTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om:", name), msgs[0]);

                AssertLogTime(msgs[0], dateTime);
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

        private static void AssertLogTime(string message, DateTime dateTime)
        {
            string[] logMessageArray = message.Split(':');
            string logMessageDateTime = string.Format("{0}:{1}:{2}", logMessageArray[1], logMessageArray[2], logMessageArray[3]).Substring(1);

            DateTime dateTimeFromLog = DateTime.ParseExact(logMessageDateTime, "HH:mm:ss", CultureInfo.CurrentCulture);
            TimeSpan timeSpan = dateTimeFromLog - dateTime;
            Assert.LessOrEqual(timeSpan, TimeSpan.FromMilliseconds(500));
        }
    }
}