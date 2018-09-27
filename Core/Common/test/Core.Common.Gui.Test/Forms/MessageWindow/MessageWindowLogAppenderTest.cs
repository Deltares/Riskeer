// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Globalization;
using Core.Common.Gui.Forms.MessageWindow;
using Core.Common.TestUtil;
using log4net.Core;
using log4net.Util;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.MessageWindow
{
    [TestFixture]
    public class MessageWindowLogAppenderTest
    {
        private MessageWindowLogAppender originalInstance;

        [SetUp]
        public void SetUp()
        {
            originalInstance = MessageWindowLogAppender.Instance;
        }

        [TearDown]
        public void TearDown()
        {
            MessageWindowLogAppender.Instance = originalInstance;
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var appender = new MessageWindowLogAppender();

            // Assert
            Assert.IsFalse(appender.Enabled);
            Assert.IsNull(appender.MessageWindow);
            Assert.IsInstanceOf<OnlyOnceErrorHandler>(appender.ErrorHandler);
            Assert.AreEqual(null, appender.FilterHead);
            Assert.AreEqual(null, appender.Layout);
            Assert.AreEqual(null, appender.Name);
            Assert.AreEqual(null, appender.Threshold);
            Assert.AreSame(appender, MessageWindowLogAppender.Instance);
        }

        [Test]
        [TestCase(LogLevelConstant.Off)]
        [TestCase(LogLevelConstant.Fatal)]
        [TestCase(LogLevelConstant.Error)]
        [TestCase(LogLevelConstant.Warn)]
        [TestCase(LogLevelConstant.Info)]
        [TestCase(LogLevelConstant.Debug)]
        public void DoAppend_AppenderEnabled_ForwardAllMessagesToMessageWindow(LogLevelConstant logLevel)
        {
            // Setup
            Level level = logLevel.ToLog4NetLevel();
            DateTime dataTime = DateTime.Now;
            const string message = "<some nice log-message>";

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(level, dataTime, message));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = true,
                MessageWindow = messageWindow
            };

            var logData = new LoggingEventData
            {
                Level = level,
                TimeStamp = dataTime,
                Message = message
            };
            var logEvent = new LoggingEvent(logData);

            // Call
            appender.DoAppend(logEvent);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void DoAppend_LogMessageHasFormattedText_ReturnLocalCulturedText()
        {
            // Setup
            const string messageText = "Gestart in {0:f2} seconden.";
            const double formatArgument = 1.2;

            const string expectedText = "Gestart in 1,20 seconden.";

            Level level = LogLevelConstant.Warn.ToLog4NetLevel();
            DateTime dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Warn,
                                            new SystemStringFormat(CultureInfo.InvariantCulture, messageText, formatArgument),
                                            null);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => time - dataTime <= new TimeSpan(0, 0, 0, 0, 5)),
                                                   Arg<string>.Is.Equal(expectedText)));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = true,
                MessageWindow = messageWindow
            };

            // Call
            appender.DoAppend(logEvent);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void DoAppend_LogMessageHasFormattedTextWithBug_ForwardUnformattedText()
        {
            // Setup
            const string messageText = "Gestart in {5:f2} seconden.";
            const double formatArgument = 1.2;

            Level level = LogLevelConstant.Warn.ToLog4NetLevel();
            DateTime dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Warn,
                                            new SystemStringFormat(CultureInfo.InvariantCulture, messageText, formatArgument),
                                            null);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => time - dataTime <= new TimeSpan(0, 0, 0, 0, 2)),
                                                   Arg<string>.Is.Equal(messageText)));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = true,
                MessageWindow = messageWindow
            };

            // Call
            appender.DoAppend(logEvent);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [SetCulture("nl-NL")]
        public void DoAppend_LogMessageHasFormattedTextWithoutFormatArgument_ForwardUnformattedText()
        {
            // Setup
            const string messageText = "Gestart in {5:f2} seconden.";

            Level level = LogLevelConstant.Warn.ToLog4NetLevel();
            DateTime dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Warn,
                                            new SystemStringFormat(CultureInfo.InvariantCulture, messageText, null),
                                            null);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => time - dataTime <= new TimeSpan(0, 0, 0, 0, 2)),
                                                   Arg<string>.Is.Equal(messageText)));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = true,
                MessageWindow = messageWindow
            };

            // Call
            appender.DoAppend(logEvent);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void DoAppend_LogMessageHasException_AppendTextForReferToLogfileToMessage()
        {
            // Setup
            const string messageText = "<logmessage text>";
            string expectedText = $"{messageText} {Environment.NewLine}Controleer logbestand voor meer informatie (\"Bestand\"->\"Help\"->\"Log tonen\").";
            var exception = new Exception();

            Level level = LogLevelConstant.Error.ToLog4NetLevel();
            DateTime dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Error,
                                            messageText, exception);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => time - dataTime <= new TimeSpan(0, 0, 0, 0, 2)),
                                                   Arg<string>.Is.Equal(expectedText)));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = true,
                MessageWindow = messageWindow
            };

            // Call
            appender.DoAppend(logEvent);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(LogLevelConstant.Off)]
        [TestCase(LogLevelConstant.Fatal)]
        [TestCase(LogLevelConstant.Error)]
        [TestCase(LogLevelConstant.Warn)]
        [TestCase(LogLevelConstant.Info)]
        [TestCase(LogLevelConstant.Debug)]
        public void DoAppend_AppenderDisabled_MessagesNotForwardedToMessageWindow(LogLevelConstant logLevel)
        {
            // Setup
            Level level = logLevel.ToLog4NetLevel();
            DateTime dataTime = DateTime.Now;
            const string message = "<some nice log-message>";

            var mocks = new MockRepository();
            var messageWindow = mocks.StrictMock<IMessageWindow>();
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = false,
                MessageWindow = messageWindow
            };

            var logData = new LoggingEventData
            {
                Level = level,
                TimeStamp = dataTime,
                Message = message
            };
            var logEvent = new LoggingEvent(logData);

            // Call
            appender.DoAppend(logEvent);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(LogLevelConstant.Off)]
        [TestCase(LogLevelConstant.Fatal)]
        [TestCase(LogLevelConstant.Error)]
        [TestCase(LogLevelConstant.Warn)]
        [TestCase(LogLevelConstant.Info)]
        [TestCase(LogLevelConstant.Debug)]
        public void Enabled_FromFalseToTrue_ForwardAllCachedMessagesToMessageWindow(LogLevelConstant logLevel)
        {
            // Setup
            Level level = logLevel.ToLog4NetLevel();
            DateTime dataTime = DateTime.Today;
            const string message = "<yet another nice log-message>";

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(level, dataTime, message));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = false,
                MessageWindow = messageWindow
            };

            var logData = new LoggingEventData
            {
                Level = level,
                TimeStamp = dataTime,
                Message = message
            };
            var logEvent = new LoggingEvent(logData);
            appender.DoAppend(logEvent);

            // Call
            appender.Enabled = true;

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(LogLevelConstant.Off)]
        [TestCase(LogLevelConstant.Fatal)]
        [TestCase(LogLevelConstant.Error)]
        [TestCase(LogLevelConstant.Warn)]
        [TestCase(LogLevelConstant.Info)]
        [TestCase(LogLevelConstant.Debug)]
        public void MessageWindow_FromNullToProperInstance_ForwardAllCachedMessagesToMessageWindow(LogLevelConstant logLevel)
        {
            // Setup
            Level level = logLevel.ToLog4NetLevel();
            DateTime dataTime = DateTime.Today;
            const string message = "<another nice log-message>";

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(level, dataTime, message));
            mocks.ReplayAll();

            var appender = new MessageWindowLogAppender
            {
                Enabled = true
            };

            var logData = new LoggingEventData
            {
                Level = level,
                TimeStamp = dataTime,
                Message = message
            };
            var logEvent = new LoggingEvent(logData);
            appender.DoAppend(logEvent);

            // Call
            appender.MessageWindow = messageWindow;

            // Assert
            mocks.VerifyAll();
        }
    }
}