using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using Core.Common.Controls.Swf;
using Core.Common.Utils.Aop;
using log4net;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Common.TestUtils
{
    /// <summary>
    /// Used by WindowsFormsTestHelper and WpfTestHelper
    /// 
    /// TODO: invert it - use GuiTestHelper as entry point in tests.
    /// </summary>
    public class GuiTestHelper
    {
        /// <summary>
        /// Enable to monitor allocated/deallocated resources
        /// </summary>
        public static bool UseResourceMonitor = false;

        private static GuiTestHelper instance;

        private static readonly ILog log = LogManager.GetLogger(typeof(WindowsFormsTestHelper));

        private static Form synchronizationForm;

        private static Exception exception;

        private static bool unhandledThreadExceptionOccured;

        private static bool appDomainExceptionOccured;

        private static readonly ResourceMonitor resourceMonitor;

        static GuiTestHelper()
        {
            MessageBox.CustomMessageBox = new LoggingMessageBox();

            Control.CheckForIllegalCrossThreadCalls = true;
            Application.EnableVisualStyles();

            InitializeSynchronizatonObject();
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;

            if (UseResourceMonitor && resourceMonitor == null)
            {
                resourceMonitor = new ResourceMonitor();
                resourceMonitor.Show();
            }
        }

        public static GuiTestHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GuiTestHelper();
                }

                return instance;
            }
        }

        /// <summary>
        /// Checks build_number environment variable to determine whether we run on the build server.
        /// </summary>
        public static bool IsBuildServer
        {
            get
            {
                return File.Exists("C:\\build.server")
                       || File.Exists("D:\\build.server")
                       || File.Exists("/tmp/build-server")
                       || !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("BUILD_NUMBER"));
            }
        }

        public Exception Exception
        {
            get
            {
                return exception;
            }
        }

        public void RethrowUnhandledException()
        {
            if (unhandledThreadExceptionOccured)
            {
                throw new UnhandledException("Unhandled thread exception: " + exception.Message, exception, exception.StackTrace);
            }

            if (appDomainExceptionOccured)
            {
                throw new UnhandledException("Unhandled app domain exception: " + exception.Message, exception, exception.StackTrace);
            }
        }

        public static void Initialize()
        {
            exception = null;
            unhandledThreadExceptionOccured = false;
            appDomainExceptionOccured = false;
            MessageBox.CustomMessageBox = new LoggingMessageBox();
        }

        private static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            unhandledThreadExceptionOccured = true;
            exception = e.Exception;
            log.Error("WPF exception occured: " + e.Exception.Message, e.Exception);
        }

        private static void InitializeSynchronizatonObject()
        {
            if (synchronizationForm == null)
            {
                synchronizationForm = new Form
                {
                    ShowInTaskbar = false, WindowState = FormWindowState.Minimized, FormBorderStyle = FormBorderStyle.None
                };
                synchronizationForm.Load += (sender, args) => synchronizationForm.Size = new Size(0, 0);
                var handle = synchronizationForm.Handle; //force get handle
                synchronizationForm.Show();
            }

            if (InvokeRequiredInfo.SynchronizeObject == null)
            {
                InvokeRequiredInfo.SynchronizeObject = synchronizationForm;
                InvokeRequiredInfo.WaitMethod = Application.DoEvents;
            }
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            appDomainExceptionOccured = true;
            exception = e.ExceptionObject as Exception;

            if (exception != null)
            {
                log.Error("Exception occured: " + exception.Message, exception);
            }
            else
            {
                log.Error("Unhandled exception occured: " + e.ExceptionObject);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            unhandledThreadExceptionOccured = true;
            exception = e.Exception;
            log.Error("Windows.Forms exception occured: " + e.Exception.Message, e.Exception);
        }

        public class LoggingMessageBox : IMessageBox
        {
            public DialogResult Show(string text, string caption, MessageBoxButtons buttons)
            {
                Console.WriteLine("MessageBox: " + caption + ". " + text);

                if (buttons == MessageBoxButtons.YesNoCancel || buttons == MessageBoxButtons.YesNo)
                {
                    return DialogResult.No;
                }

                return DialogResult.None;
            }
        }

        /// <summary>
        /// Defines unhandled exception which provides stack trace of inner exception as its stack trace.
        /// </summary>
        public class UnhandledException : Exception
        {
            private readonly string stackTrace;

            public UnhandledException(string message, Exception innerException, string stackTrace)
                : base(message, innerException)
            {
                this.stackTrace = stackTrace;
            }

            public override string StackTrace
            {
                get
                {
                    return stackTrace;
                }
            }
        }
    }
}