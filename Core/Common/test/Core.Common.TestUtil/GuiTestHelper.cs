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
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Used by WindowsFormsTestHelper and WpfTestHelper
    /// 
    /// TODO: invert it - use GuiTestHelper as entry point in tests.
    /// </summary>
    public class GuiTestHelper
    {
        private static GuiTestHelper instance;

        private static Form synchronizationForm;

        private static bool unhandledThreadExceptionOccurred;

        private static bool appDomainExceptionOccurred;

        static GuiTestHelper()
        {
            Control.CheckForIllegalCrossThreadCalls = true;
            Application.EnableVisualStyles();

            InitializeSynchronizatonObject();
            Dispatcher.CurrentDispatcher.UnhandledException += CurrentDispatcher_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
            Application.ThreadException += Application_ThreadException;
        }

        public static GuiTestHelper Instance
        {
            get
            {
                return instance ?? (instance = new GuiTestHelper());
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
                       || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BUILD_NUMBER"));
            }
        }

        public static Exception Exception { get; private set; }

        public static void RethrowUnhandledException()
        {
            if (Exception == null)
            {
                return;
            }

            if (unhandledThreadExceptionOccurred)
            {
                throw new UnhandledException("Unhandled thread exception: " + Exception.Message, Exception, Exception.StackTrace);
            }

            if (appDomainExceptionOccurred)
            {
                throw new UnhandledException("Unhandled app domain exception: " + Exception.Message, Exception, Exception.StackTrace);
            }
        }

        public static void Initialize()
        {
            Exception = null;
            unhandledThreadExceptionOccurred = false;
            appDomainExceptionOccurred = false;
        }

        private static void CurrentDispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            unhandledThreadExceptionOccurred = true;
            Exception = e.Exception;
            RethrowUnhandledException();
        }

        private static void InitializeSynchronizatonObject()
        {
            if (synchronizationForm == null)
            {
                synchronizationForm = new Form
                {
                    ShowInTaskbar = false,
                    WindowState = FormWindowState.Minimized,
                    FormBorderStyle = FormBorderStyle.None
                };
                synchronizationForm.Load += (sender, args) => synchronizationForm.Size = new Size(0, 0);
                IntPtr handle = synchronizationForm.Handle; //force get handle
                synchronizationForm.Show();
            }
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            appDomainExceptionOccurred = true;
            Exception = e.ExceptionObject as Exception;

            RethrowUnhandledException();
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            unhandledThreadExceptionOccurred = true;
            Exception = e.Exception;
            RethrowUnhandledException();
        }

        /// <summary>
        /// Defines unhandled exception which provides stack trace of inner exception as its stack trace.
        /// </summary>
        [Serializable]
        public class UnhandledException : Exception
        {
            public UnhandledException(string message, Exception innerException, string stackTrace)
                : base(message, innerException)
            {
                StackTrace = stackTrace;
            }

            public override string StackTrace { get; }
        }
    }
}