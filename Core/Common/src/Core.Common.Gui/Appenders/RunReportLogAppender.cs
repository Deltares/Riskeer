using System;
using log4net.Appender;
using log4net.Core;

namespace Core.Common.Gui.Appenders
{
    public class RunReportLogAppender : AppenderSkeleton
    {
        public RunReportLogAppender()
        {
            Instance = this;
        }

        public static RunReportLogAppender Instance { get; set; }

        public Action<string> AppendMessageLineAction { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (AppendMessageLineAction != null)
            {
                AppendMessageLineAction(loggingEvent.RenderedMessage);
            }
        }
    }
}