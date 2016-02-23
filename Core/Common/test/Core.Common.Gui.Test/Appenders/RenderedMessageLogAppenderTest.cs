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

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            if (RenderedMessageLogAppender.Instance != null)
            {
                originalAppendMessageLineAction = RenderedMessageLogAppender.Instance.AppendMessageLineAction;
                RenderedMessageLogAppender.Instance.AppendMessageLineAction = null;
            }
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            RenderedMessageLogAppender.Instance.AppendMessageLineAction = originalAppendMessageLineAction;
        }

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

            int actionCallCount = 0;
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
            var message = Assert.Throws<InvalidOperationException>(call).Message;
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
    }
}