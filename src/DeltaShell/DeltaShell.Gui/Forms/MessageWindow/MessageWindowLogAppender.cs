using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Gui.Properties;
using log4net.Appender;
using log4net.Core;

namespace DeltaShell.Gui.Forms.MessageWindow
{
    public class MessageWindowLogAppender : AppenderSkeleton
    {
        private static IMessageWindow messageWindow;
        private static ResourceManager resourceManager;
        private static ResourceWriter resourceWriter;
        private static bool enabled;

        public MessageWindowLogAppender()
        {
        }

        public static IMessageWindow MessageWindow
        {
            get { return messageWindow; }
            set { messageWindow = value; }
        }

        /// <summary>
        /// Resource manager for looking up culture/language depended messages
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get { return resourceManager; }
            set { resourceManager = value; }
        }

        /// <summary>
        /// Resource writer makes a catalogue for not found messages at the resources
        /// </summary>
        public static ResourceWriter ResourceWriter
        {
            get { return resourceWriter; }
            set { resourceWriter = value; }
        }

        public static bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                FlushMessagesToMessageWindow();
            }
        }

        /// <summary>
        /// This list contains any messages that could not yet be delivered to the MessageWindow (typically because it doesn't exist 
        /// yet at startup). They are kept in the backlog and send to the MessageWindow upon the first message arriving while there 
        /// is a MessageWindow.
        /// </summary>
        protected static IList<LoggingEvent> messageBackLog = new List<LoggingEvent>();

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (messageWindow == null || !Enabled)
            {
                messageBackLog.Add(loggingEvent);
            }
            else
            {
                FlushMessagesToMessageWindow();
                AppendToMessageWindow(loggingEvent);
            }
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
                    AppendToMessageWindow(backLogLoggingEvent);
                messageBackLog.Clear();
            }
        }

        protected static void AppendToMessageWindow(LoggingEvent loggingEvent)
        {
            if (messageWindow == null)
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

            if(loggingEvent.ExceptionObject != null)
            {
                message += loggingEvent.ExceptionObject.Message + "\n";
                message += Resources.MessageWindowLogAppender_AppendToMessageWindow__Check_log_file_for_more_information__Home__Show_Log__;
            }

            messageWindow.AddMessage(loggingEvent.Level, loggingEvent.TimeStamp,
                                     loggingEvent.LoggerName, message, loggingEvent.GetExceptionString());
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
            if (resourceManager != null)
            {
                localizedMessage = resourceManager.GetString(message);

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
            if(resourceWriter != null)
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