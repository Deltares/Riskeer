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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Service.TestUtil;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class CalculationServiceHelperTest
    {
        [Test]
        public void LogMessagesAsError_Always_LogsMessagesInGivenFormat()
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
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new []
            {
                Tuple.Create(string.Format(format, errorMessages[0]), LogLevelConstant.Error),
                Tuple.Create(string.Format(format, errorMessages[1]), LogLevelConstant.Error)
            }, 2);
        }

        [Test]
        public void LogMessagesAsWarning_Always_LogsMessagesInGivenFormat()
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
            // Setup
            const string name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogValidationBegin(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertValidationStartMessage(name, msgs[0]);
            });
        }

        [Test]
        public void LogValidationEnd_Always_LogsValidationEnd()
        {
            // Setup
            const string name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogValidationEnd(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertValidationEndMessage(name, msgs[0]);
            });
        }

        [Test]
        public void LogCalculationBegin_Always_LogsCalculationBegin()
        {
            // Setup
            const string name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogCalculationBegin(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertCalculationStartMessage(name, msgs[0]);
            });
        }

        [Test]
        public void LogCalculationEnd_Always_LogsCalculationEnd()
        {
            // Setup
            const string name = "Test name";

            // Call
            Action call = () => CalculationServiceHelper.LogCalculationEnd(name);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                string[] msgs = messages.ToArray();
                Assert.AreEqual(1, msgs.Length);
                CalculationServiceTestHelper.AssertCalculationEndMessage(name, msgs[0]);
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
    }
}