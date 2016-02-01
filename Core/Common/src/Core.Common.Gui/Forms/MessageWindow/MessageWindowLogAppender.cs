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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using Core.Common.Gui.Properties;
using log4net.Appender;
using log4net.Core;

namespace Core.Common.Gui.Forms.MessageWindow
{
    public class MessageWindowLogAppender : AppenderSkeleton
    {
        /// <summary>
        /// This list contains any messages that could not yet be delivered to the MessageWindow (typically because it doesn't exist 
        /// yet at startup). They are kept in the backlog and send to the MessageWindow upon the first message arriving while there 
        /// is a MessageWindow.
        /// </summary>
        protected static IList<LoggingEvent> messageBackLog = new List<LoggingEvent>();

        private static bool enabled;

        public static IMessageWindow MessageWindow { get; set; }

        /// <summary>
        /// Resource manager for looking up culture/language depended messages
        /// </summary>
        public static ResourceManager ResourceManager { get; set; }

        /// <summary>
        /// Resource writer makes a catalogue for not found messages at the resources
        /// </summary>
        public static ResourceWriter ResourceWriter { get; set; }

        public static bool Enabled
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
            if (MessageWindow == null || !Enabled)
            {
                messageBackLog.Add(loggingEvent);
            }
            else
            {
                FlushMessagesToMessageWindow();
                AppendToMessageWindow(loggingEvent);
            }
        }

        protected static void AppendToMessageWindow(LoggingEvent loggingEvent)
        {
            if (MessageWindow == null)
            {
                return;
            }

            string message = null;

            if (loggingEvent.MessageObject != null)
            {
                Type t = loggingEvent.MessageObject.GetType();

                if (t.FullName == "log4net.Util.SystemStringFormat")
                {
                    string format =
                        (string)
                        t.InvokeMember("m_format",
                                       BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null,
                                       loggingEvent.MessageObject, null);
                    object[] args =
                        (object[])
                        t.InvokeMember("m_args",
                                       BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance, null,
                                       loggingEvent.MessageObject, null);

                    message = GetLocalizedMessage(format, args);
                }
            }

            if (message == null)
            {
                message = GetLocalizedMessage(loggingEvent.RenderedMessage);
            }

            if (loggingEvent.ExceptionObject != null)
            {
                message += " " + Environment.NewLine + Resources.MessageWindowLogAppender_AppendToMessageWindow_Check_log_file_for_more_information_Home_Show_Log;
            }

            MessageWindow.AddMessage(loggingEvent.Level, loggingEvent.TimeStamp, message);
        }

        private static void FlushMessagesToMessageWindow()
        {
            if (MessageWindow == null)
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
                return string.Format(GetLocalizedMessage(format), args);
            }
            catch
            {
                return format;
            }
        }

        private static string GetLocalizedMessage(string message)
        {
            string localizedMessage = "";
            if (ResourceManager != null)
            {
                localizedMessage = ResourceManager.GetString(message);
            }
            if (string.IsNullOrEmpty(localizedMessage))
            {
                WriteMessageToResourceFile(message);
                return message; // return non-localized message
            }
            return localizedMessage;
        }

        private static void WriteMessageToResourceFile(string message)
        {
            if (ResourceWriter != null)
            {
                try
                {
                    //   bug: this will fail
                    // resourceWriter.AddResource(message, message);
                }
                catch (ArgumentException)
                {
                    //name (or a name that varies only by capitalization) has already been added to this ResourceWriter. 
                }
            }
        }
    }
}