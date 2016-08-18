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
        public void PerformValidation_ValidationFuncReturnsMessages_WritesLogMessagesAndReturnsFalse()
        {
            // Setup
            string name = "Test name";
            string[] errorMessages =
            {
                "Error message 1"
            };

            // Call
            bool valid = false;
            Action call = () => valid = CalculationServiceHelper.PerformValidation(name, () => errorMessages);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie mislukt: {0}", errorMessages[0]), msgs[1]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsFalse(valid);
        }

        [Test]
        public void PerformValidation_ValidationFuncReturnsEmtpyList_WritesStartAndEndLogMessagesAndReturnsTrue()
        {
            // Setup
            string name = "Test name";
            string[] errorMessages =
            {};

            // Call
            bool valid = false;
            Action call = () => valid = CalculationServiceHelper.PerformValidation(name, () => errorMessages);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);
            });
            Assert.IsTrue(valid);
        }

        [Test]
        public void PerformCalculation_WithCalculation_ExecutesCalculationActionAndWritesStartAndEndAndErrorLogMessages()
        {
            // Setup
            string name = "Test name";

            int called = 0;

            // Call
            Action call = () => CalculationServiceHelper.PerformCalculation(name, () => called++);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", name), msgs[1]);
            });
            Assert.AreEqual(1, called);
        }

        [Test]
        public void LogMessagesAsError_Always_LogsMessagesInGivenFormat()
        {
            // Setup
            var format = "Message: {0}";
            string[] errorMessages = new[]
            {
                "Test 1",
                "Test 2"
            };

            // Call
            Action call = () => CalculationServiceHelper.LogMessagesAsError(format, errorMessages);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
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
                var msgs = messages.ToArray();
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
                var msgs = messages.ToArray();
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
                var msgs = messages.ToArray();
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
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om:", name), msgs[0]);

                AssertLogTime(msgs[0], dateTime);
            });
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