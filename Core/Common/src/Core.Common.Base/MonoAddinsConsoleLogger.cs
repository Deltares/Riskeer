using System;
using Core.Common.Base.Properties;
using Mono.Addins;

namespace Core.Common.Base
{
    internal class MonoAddinsConsoleLogger : IProgressStatus
    {
        public MonoAddinsConsoleLogger(int logLevel)
        {
            LogLevel = logLevel;
        }

        public bool LogAssemblyScanErrors { get; set; }

        public int LogLevel { get; private set; }

        public bool IsCanceled { get; private set; }

        public void SetMessage(string msg) {}

        public void SetProgress(double progress) {}

        public void Log(string msg)
        {
            if (LogLevel <= 3)
            {
                return;
            }
            Console.WriteLine(msg);
        }

        public void ReportWarning(string message)
        {
            if (LogLevel <= 2)
            {
                return;
            }
            Console.WriteLine(Resources.MonoAddinsConsoleLogger_ReportWarning_Warning___ + message);
        }

        public void ReportError(string message, Exception exception)
        {
            var badImageException = exception as BadImageFormatException;
            if (badImageException != null)
            {
                if (LogAssemblyScanErrors)
                {
                    Console.WriteLine(Resources.MonoAddinsConsoleLogger_ReportError_Error___ + message);
                }

                return;
            }

            if (LogLevel <= 1)
            {
                return;
            }
            Console.WriteLine(Resources.MonoAddinsConsoleLogger_ReportError_Error___ + message);
        }

        public void Cancel()
        {
            IsCanceled = true;
        }
    }
}