using System;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using DeltaShell.Gui.Forms.MessageWindow;
using DeltaShell.Gui.Properties;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace DeltaShell.Gui.Forms
{
    public class SplashScreenLogAppender : IAppender
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(SplashScreenLogAppender));

        public static SplashScreenLogAppender instance;
        private static SplashScreen splashScreen;

        private static string historyLogFilePath;

        private static MessageWindowData historyLogData;
        // use MessageWindowData DataSet to store history of startup log messages

        private static MessageWindowData currentLogData;

        private static long historyMaxTimeOffset;
        private static long timeLeft;
        private static readonly DateTime startTime = DateTime.Now;

        private static bool shuttingDown;

        private static bool appending;

        public string Name
        {
            get
            {
                return "SplashScreenLogAppender";
            }
            set {}
        }

        public static void InitializeUsingLogging(string path, SplashScreen screen)
        {
            splashScreen = screen;

            InitializeLogMessagesHistory(path);

            Logger rootLogger = GetRootLogger();

            if (instance == null)
            {
                instance = new SplashScreenLogAppender();
            }

            rootLogger.AddAppender(instance);
        }

        public static void Shutdown()
        {
            shuttingDown = true;

            while (appending)
            {
                Thread.Sleep(0);
            }

            Logger rootLogger = GetRootLogger();
            rootLogger.RemoveAppender(instance);

            bool saved = false;

            // deadlock detected, run save in the background thread
            var saveLogFileThread = new Thread(() =>
            {
                currentLogData.WriteXml(historyLogFilePath);
                saved = true;
            });
            saveLogFileThread.Start();

            while (!saved)
            {
                splashScreen.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
            }

            saveLogFileThread.Join();
        }

        public static void Dispose()
        {
            instance = null;
            splashScreen = null;
            historyLogData = null;
            currentLogData = null;
        }

        public void Close() {}

        public void DoAppend(LoggingEvent loggingEvent)
        {
            if (shuttingDown)
            {
                return;
            }

            appending = true;

            long timeOffset = (long) (loggingEvent.TimeStamp - startTime).TotalMilliseconds;

            DataRow newRow = currentLogData.Messages.AddMessagesRow(null, loggingEvent.TimeStamp, loggingEvent.LoggerName, loggingEvent.RenderedMessage, null);
            newRow["TimeOffset"] = timeOffset;

            if (historyLogData.Messages.Count != 0)
            {
                var row = GetFirstMessage(loggingEvent.RenderedMessage);

                if (row != null)
                {
                    long historyTimeOffset = (long) row["TimeOffset"];

                    double fractionTimeLeft = historyTimeOffset/(double) historyMaxTimeOffset;
                    int progressBarValue = (int) (100*fractionTimeLeft);

                    timeLeft = (long) (historyMaxTimeOffset*(1.0 - fractionTimeLeft));

                    var progressBarText = String.Format(Resources.SplashScreenLogAppender_DoAppend__1____time_left___0_N1__sec, Math.Max(0, timeLeft/1000.0), Math.Min(100, progressBarValue));

                    if (progressBarValue >= splashScreen.ProgressBarValue)
                    {
                        splashScreen.SetProgress(new SplashScreen.ProgressInfo
                        {
                            Message = loggingEvent.Level >= Level.Info ? loggingEvent.RenderedMessage : "",
                            ProgressText = progressBarText,
                            ProgressValue = progressBarValue
                        });
                    }
                }
            }

            appending = false;
        }

        private static DataRow GetFirstMessage(string message)
        {
            foreach (MessageWindowData.MessagesRow row in historyLogData.Messages.Rows)
            {
                if (row.Message.Equals(message, StringComparison.OrdinalIgnoreCase))
                {
                    return row;
                }
            }

            return null;
        }

        private static void InitializeLogMessagesHistory(string path)
        {
            historyLogFilePath = path;

            currentLogData = new MessageWindowData();
            currentLogData.Messages.Columns.Add("TimeOffset", typeof(long));

            historyLogData = new MessageWindowData();
            historyLogData.Messages.Columns.Add("TimeOffset", typeof(long));

            if (File.Exists(path)) // read previous startup lhistory
            {
                try
                {
                    historyLogData.ReadXml(path);

                    if (historyLogData.Messages.Count > 0)
                    {
                        historyMaxTimeOffset = (long) historyLogData.Messages[historyLogData.Messages.Count - 1]["TimeOffset"];
                    }
                    splashScreen.MaxProgressBarValue = 100; // %
                    return;
                }
                catch (FormatException)
                {
                    //format changed, delete the bloody file
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
            }

            splashScreen.SetProgress(new SplashScreen.ProgressInfo
            {
                Message = "", ProgressText = "", ProgressValue = 100
            });
        }

        private static Logger GetRootLogger()
        {
            ILog log = LogManager.GetLogger(typeof(SplashScreenLogAppender));
            Logger logger = (Logger) log.Logger;
            return logger.Hierarchy.Root;
        }
    }
}