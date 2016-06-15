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
using System.Collections.Generic;
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
            List<string> errorMessages = new List<string>();
            errorMessages.Add("Error message 1");

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
            List<string> errorMessages = new List<string>();

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
        public void PerformCalculation_CalculationFuncReturnsNull_WritesStartAndEndAndErrorLogMessagesAndReturnsNull()
        {
            // Setup
            string name = "Test name";
            string errorMessage = "There was an error: {0}";
            
            object output = null;

            // Call
            Action call = () => output = CalculationServiceHelper.PerformCalculation<object>(name, () => null, errorMessage);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format(errorMessage, name), msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", name), msgs[2]);
            });
            Assert.IsNull(output);
        }

        [Test]
        public void PerformCalculation_CalculationFuncReturnsOutput_WritesStartAndEndLogMessagesAndReturnsOutput()
        {
            // Setup
            string name = "Test name";
            double outputValue = 4.0;

            double output = double.NaN;

            // Call
            Action call = () => output = CalculationServiceHelper.PerformCalculation(name, () => outputValue, string.Empty);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(2, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", name), msgs[0]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", name), msgs[1]);
            });
            Assert.AreEqual(outputValue, output);
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

            // Call
            Action call = () => CalculationServiceHelper.LogValidationBeginTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om:", name), msgs[0]);
            });
        }

        [Test]
        public void LogValidationEndTime_Always_LogsValidationEndTime()
        {
            // Setup
            var name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogValidationEndTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om:", name), msgs[0]);
            });
        }

        [Test]
        public void LogCalculationBeginTime_Always_LogsCalculationBeginTime()
        {
            // Setup
            var name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogCalculationBeginTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om:", name), msgs[0]);
            });
        }

        [Test]
        public void LogCalculationEndTime_Always_LogsCalculationEndTime()
        {
            // Setup
            var name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogCalculationEndTime(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om:", name), msgs[0]);
            });
        }
    }
}
