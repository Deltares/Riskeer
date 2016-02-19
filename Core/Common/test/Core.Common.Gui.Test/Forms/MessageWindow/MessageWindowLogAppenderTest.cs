using System;
using System.Globalization;
using Core.Common.Gui.Forms.MessageWindow;
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
        [TestCase(LogLevel.Off)]
        [TestCase(LogLevel.Fatal)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Debug)]
        public void DoAppend_AppenderEnabled_ForwardAllMessagesToMessageWindow(LogLevel logLevel)
        {
            // Setup
            Level level = CreateLog4NetLevel(logLevel);
            var dataTime = DateTime.Now;
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

            Level level = CreateLog4NetLevel(LogLevel.Warn);
            var dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Warn,
                                            new SystemStringFormat(CultureInfo.InvariantCulture, messageText, formatArgument),
                                            null);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => (time - dataTime) <= new TimeSpan(0, 0, 0, 0, 5)),
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

            Level level = CreateLog4NetLevel(LogLevel.Warn);
            var dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Warn,
                                            new SystemStringFormat(CultureInfo.InvariantCulture, messageText, formatArgument),
                                            null);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => (time - dataTime) <= new TimeSpan(0, 0, 0, 0, 1)),
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

            Level level = CreateLog4NetLevel(LogLevel.Warn);
            var dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Warn,
                                            new SystemStringFormat(CultureInfo.InvariantCulture, messageText, null),
                                            null);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => (time - dataTime) <= new TimeSpan(0, 0, 0, 0, 1)),
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
            string expectedText = string.Format("{0} {1}{2}",
                                                messageText, Environment.NewLine, "Controleer logbestand voor meer informatie (\"Bestand\"->\"Help\"->\"Log tonen\").");
            var exception = new Exception();

            Level level = CreateLog4NetLevel(LogLevel.Error);
            var dataTime = DateTime.Now;
            var logEvent = new LoggingEvent(null, null, "<doesn't matter>", Level.Error,
                                            messageText, exception);

            var mocks = new MockRepository();
            var messageWindow = mocks.Stub<IMessageWindow>();
            messageWindow.Expect(w => w.AddMessage(Arg<Level>.Is.Equal(level),
                                                   Arg<DateTime>.Matches(time => (time - dataTime) <= new TimeSpan(0, 0, 0, 0, 1)),
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
        [TestCase(LogLevel.Off)]
        [TestCase(LogLevel.Fatal)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Debug)]
        public void DoAppend_AppenderDisabled_MessagesNotForwardedToMessageWindow(LogLevel logLevel)
        {
            // Setup
            Level level = CreateLog4NetLevel(logLevel);
            var dataTime = DateTime.Now;
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
        [TestCase(LogLevel.Off)]
        [TestCase(LogLevel.Fatal)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Debug)]
        public void Enabled_FromFalseToTrue_ForwardAllCachedMessagesToMessageWindow(LogLevel logLevel)
        {
            // Setup
            Level level = CreateLog4NetLevel(logLevel);
            var dataTime = DateTime.Today;
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
        [TestCase(LogLevel.Off)]
        [TestCase(LogLevel.Fatal)]
        [TestCase(LogLevel.Error)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Debug)]
        public void MessageWindow_FromNullToProperInstance_ForwardAllCachedMessagesToMessageWindow(LogLevel logLevel)
        {
            // Setup
            Level level = CreateLog4NetLevel(logLevel);
            var dataTime = DateTime.Today;
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

        private Level CreateLog4NetLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Off:
                    return Level.Off;
                case LogLevel.Fatal:
                    return Level.Fatal;
                case LogLevel.Error:
                    return Level.Error;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Debug:
                    return Level.Debug;
                default:
                    throw new NotImplementedException();
            }
        }

        public enum LogLevel
        {
            Off,
            Fatal,
            Error,
            Warn,
            Info,
            Debug
        }
    }
}