// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using Core.Common.Gui.Appenders;
using log4net.Appender;
using log4net.Core;
using log4net.Util;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Appenders
{
    [TestFixture]
    public class RenderedMessageLogAppenderTest
    {
        private Action<string> originalAppendMessageLineAction;

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var appender = new RenderedMessageLogAppender();

            // Assert
            Assert.IsInstanceOf<AppenderSkeleton>(appender);
            Assert.IsNull(appender.AppendMessageLineAction);
            Assert.IsInstanceOf<OnlyOnceErrorHandler>(appender.ErrorHandler,
                                                      "Appender has default Log4Net error handler.");
            Assert.IsNull(appender.FilterHead);
            Assert.IsNull(appender.Layout);
            Assert.IsNull(appender.Name);
            Assert.IsNull(appender.Threshold);

            Assert.AreSame(appender, RenderedMessageLogAppender.Instance);
        }

        [Test]
        public void DoAppend_WithAppendMessageLineActionSet_ActionCalledForText()
        {
            // Setup
            const string messagetext = "messageText";

            var logEventData = new LoggingEventData
            {
                Message = messagetext
            };
            var logEvent = new LoggingEvent(logEventData);

            var actionCallCount = 0;
            var appender = new RenderedMessageLogAppender
            {
                AppendMessageLineAction = s =>
                {
                    Assert.AreEqual(messagetext, s);
                    actionCallCount++;
                }
            };

            // Call
            appender.DoAppend(logEvent);

            // Assert
            Assert.AreEqual(1, actionCallCount);
        }

        [Test]
        public void DoAppend_WithoutAppendMessageLineActionSet_DoNotThrow()
        {
            // Setup
            const string messagetext = "messageText";

            var logEventData = new LoggingEventData
            {
                Message = messagetext
            };
            var logEvent = new LoggingEvent(logEventData);

            var appender = new RenderedMessageLogAppender();

            // Call
            TestDelegate call = () => appender.DoAppend(logEvent);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AppendMessageLineAction_SetTwoActions_ThrowInvalidOperationException()
        {
            // Setup
            var appender = new RenderedMessageLogAppender
            {
                AppendMessageLineAction = s =>
                {
                    // Do nothing
                }
            };

            // Call
            TestDelegate call = () => appender.AppendMessageLineAction = s =>
            {
                // Do nothing again.
            };

            // Assert
            string message = Assert.Throws<InvalidOperationException>(call).Message;
            Assert.AreEqual("An action is already set", message);
        }

        [Test]
        public void AppendMessageLineAction_SetToNullWhenAlreadyHasAction_SetActionToNull()
        {
            // Setup
            var appender = new RenderedMessageLogAppender
            {
                AppendMessageLineAction = s =>
                {
                    // Do nothing
                }
            };

            // Call
            appender.AppendMessageLineAction = null;

            // Assert
            Assert.IsNull(appender.AppendMessageLineAction);
        }

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            if (RenderedMessageLogAppender.Instance != null)
            {
                originalAppendMessageLineAction = RenderedMessageLogAppender.Instance.AppendMessageLineAction;
                RenderedMessageLogAppender.Instance.AppendMessageLineAction = null;
            }
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            RenderedMessageLogAppender.Instance.AppendMessageLineAction = originalAppendMessageLineAction;
        }
    }
}