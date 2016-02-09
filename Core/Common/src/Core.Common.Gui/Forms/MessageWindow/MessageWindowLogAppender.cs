// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;

using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace Core.Common.Gui.Forms.MessageWindow
{
    /// <summary>
    /// A log-appender for Log4Net that is able to forward received messages to a <see cref="IMessageWindow"/>
    /// instance.
    /// </summary>
    public class MessageWindowLogAppender : AppenderSkeleton
    {
        /// <summary>
        /// This list contains any messages that could not yet be delivered to the <see cref="MessageWindow"/>
        /// (typically because it doesn't exist yet at startup). They are kept in the backlog 
        /// and send to <see cref="MessageWindow"/> upon the first message arriving while there is a MessageWindow
        /// <see cref="MessageWindow"/> has been set.
        /// </summary>
        private readonly IList<LoggingEvent> messageBackLog = new List<LoggingEvent>();

        private bool enabled;
        private IMessageWindow messageWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageWindowLogAppender"/> class and
        /// the singleton instance.
        /// </summary>
        public MessageWindowLogAppender()
        {
            Instance = this;
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static MessageWindowLogAppender Instance { get; set; }

        /// <summary>
        /// Gets or sets the message window to which log-messages should be forwarded.
        /// </summary>
        public IMessageWindow MessageWindow
        {
            get
            {
                return messageWindow;
            }
            set
            {
                messageWindow = value;
                FlushMessagesToMessageWindow();
            }
        }

        /// <summary>
        /// Indicating whether this appender should forward it's messages to <see cref="MessageWindow"/>
        /// (set to <c>true</c>) or should cache them when it's enabled at a later time (set to <c>false</c>).
        /// </summary>
        public bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;
                FlushMessagesToMessageWindow();
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (MessageWindow == null || !enabled)
            {
                messageBackLog.Add(loggingEvent);
            }
            else
            {
                FlushMessagesToMessageWindow();
                AppendToMessageWindow(loggingEvent);
            }
        }

        private void AppendToMessageWindow(LoggingEvent loggingEvent)
        {
            string message = null;

            var stringFormat = loggingEvent.MessageObject as SystemStringFormat;
            if (stringFormat != null)
            {
                string format = TypeUtils.GetField<string>(stringFormat, "m_format");
                object[] args = TypeUtils.GetField<object[]>(stringFormat, "m_args");
                message = GetLocalizedMessage(format, args);
            }

            if (message == null)
            {
                message = loggingEvent.RenderedMessage;
            }

            if (loggingEvent.ExceptionObject != null)
            {
                message += " " + Environment.NewLine + Resources.MessageWindowLogAppender_AppendToMessageWindow_Check_log_file_for_more_information_Home_Show_Log;
            }

            MessageWindow.AddMessage(loggingEvent.Level, loggingEvent.TimeStamp, message);
        }

        private void FlushMessagesToMessageWindow()
        {
            if (messageWindow == null || !enabled)
            {
                return;
            }

            if (messageBackLog.Count > 0)
            {
                foreach (LoggingEvent backLogLoggingEvent in messageBackLog.ToArray())
                {
                    AppendToMessageWindow(backLogLoggingEvent);
                }
                messageBackLog.Clear();
            }
        }

        private static string GetLocalizedMessage(string format, object[] args)
        {
            try
            {
                return string.Format(format, args);
            }
            catch (ArgumentNullException)
            {
                return format;
            }
            catch (FormatException)
            {
                return format;
            }
        }
    }
}