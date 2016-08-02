﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Application.Ringtoets.Storage;
using Core.Common.Gui;
using Core.Common.Gui.Appenders;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Settings;
using Core.Common.Utils;
using Core.Plugins.Chart;
using Core.Plugins.CommonTools;
using Core.Plugins.Map;
using Core.Plugins.ProjectExplorer;
using log4net;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using Ringtoets.HeightStructures.Plugin;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Plugin;
using Ringtoets.Piping.Plugin;
using MessageBox = System.Windows.MessageBox;
#if INCLUDE_DEMOPROJECT
using Demo.Ringtoets.GUIs;

#endif

namespace Application.Ringtoets
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        private delegate void ExceptionDelegate(Exception exception, bool isTerminating);

        // Start application after this process will exit (used during restart)
        private const string argumentWaitForProcess = "--wait-for-process=";
        private const int numberOfDaysToKeepLogFiles = 30;

        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private static GuiCore gui;
        private static int waitForProcessId = -1;
        private static string fileToOpen = String.Empty;

        private static Mutex singleInstanceMutex;

        static App()
        {
            SetLanguage();

            log.Info(Core.Common.Gui.Properties.Resources.App_Starting_Ringtoets);
        }

        /// <summary>
        /// Runs the main Ringtoets application.
        /// </summary>
        public static void RunRingtoets()
        {
            var loaderDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (loaderDirectory != null)
            {
                Environment.CurrentDirectory = loaderDirectory;
            }

            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);

            // handle exception from UI thread
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;

            // handle exception from all threads except UI
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;

            gui.Run(fileToOpen);

            // Ringtoets started, clean-up all possible memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (singleInstanceMutex != null)
            {
                singleInstanceMutex.ReleaseMutex();
            }
            base.OnExit(e);
        }

        private void App_Startup(object sender, StartupEventArgs e)
        {
            ParseArguments(e.Args);

            WaitForPreviousInstanceToExit();
            if (ShutdownIfNotFirstInstance())
            {
                return;
            }

            DeleteOldLogFiles();

            Resources.Add(SystemParameters.MenuPopupAnimationKey, PopupAnimation.None);

            var settings = new GuiCoreSettings
            {
                SupportEmailAddress = "www.helpdeskwater.nl",
                SupportPhoneNumber = "+31 (0)88-797 7102",
                MainWindowTitle = "Ringtoets",
                ManualFilePath = "Ringtoets_Manual.pdf"
            };
            var mainWindow = new MainWindow();
            gui = new GuiCore(mainWindow, new StorageSqLite(), new RingtoetsProjectFactory(), settings)
            {
                Plugins =
                {
                    new ProjectExplorerPlugin(),
                    new CommonToolsPlugin(),
                    new RingtoetsPlugin(),
                    new GrassCoverErosionInwardsPlugin(),
                    new PipingPlugin(),
                    new HeightStructuresPlugin(),
                    new ChartPlugin(),
                    new MapPlugin()
#if INCLUDE_DEMOPROJECT
                    , new DemoProjectPlugin()
#endif
                }
            };

            RunRingtoets();

            mainWindow.Show();
        }

        private static bool ParseFileArgument(string potentialPath)
        {
            if (potentialPath.Length > 0)
            {
                try
                {
                    FileUtils.ValidateFilePath(potentialPath);
                    fileToOpen = potentialPath;
                    return true;
                }
                catch (ArgumentException) {}
            }
            return false;
        }

        /// <summary>
        /// <code>app.config</code> has been configured to use <see cref="RingtoetsUserDataFolderConverter"/>
        /// to write log files to the ringtoets user data folder. This method deletes the old log files
        /// that have been written there.
        /// </summary>
        private void DeleteOldLogFiles()
        {
            string settingsDirectory = SettingsHelper.GetApplicationLocalUserSettingsDirectory();
            foreach (string logFile in Directory.GetFiles(settingsDirectory, "*.log"))
            {
                if ((DateTime.Now - File.GetCreationTime(logFile)).TotalDays > numberOfDaysToKeepLogFiles)
                {
                    File.Delete(logFile);
                }
            }
        }

        private bool ShutdownIfNotFirstInstance()
        {
            var hasMutex = false;

            try
            {
                if (!Debugger.IsAttached)
                {
                    if (!AcquireSingleInstancePerUserMutex())
                    {
                        MessageBox.Show(Core.Common.Gui.Properties.Resources.App_ShutdownIfNotFirstInstance_Cannot_start_multiple_instances_of_Ringtoets_Please_close_the_other_instance_first);
                        Shutdown(1);
                        return true; //done here
                    }
                    hasMutex = true;
                }
            }
            finally
            {
                if (!hasMutex)
                {
                    singleInstanceMutex = null;
                }
            }
            return false;
        }

        /// <summary>
        /// If variable waitForProcessId > -1, the application will hold until 
        /// the process with that ID has exited. 
        /// </summary>
        private static void WaitForPreviousInstanceToExit()
        {
            // Wait until previous version of Ringtoets has exited
            if (waitForProcessId == -1)
            {
                return;
            }

            try
            {
                var process = Process.GetProcessById(waitForProcessId);
                process.WaitForExit();
            }
            catch
            {
                //Ignored, because the process may already be closed
            }
        }

        private static bool AcquireSingleInstancePerUserMutex()
        {
            var createdNew = false;
            try
            {
                //include the user name in the (global) mutex to ensure we limit only the number of instances per 
                //user, not per system (essential on for example Citrix systems).

                //include the application name in the mutex to ensure we are allowed to start for example 'Sobek' 
                //and 'Morphan' side by side.
                var applicationName = ConfigurationManager.AppSettings.AllKeys.Contains("applicationName")
                                          ? ConfigurationManager.AppSettings["applicationName"]
                                          : string.Empty;

                var mutexName = string.Format("Ringtoets-single-instance-mutex-{0}-{1}", Environment.UserName,
                                              applicationName);
                singleInstanceMutex = new Mutex(true, mutexName, out createdNew);
            }
            catch (AbandonedMutexException) {} //might throw an abandoned mutex exception if the previous DS instance forcefully exited.
            return createdNew;
        }

        private static void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception;

            exception = e.ExceptionObject as Exception;

            if (exception == null)
            {
                exception = new Exception(Core.Common.Gui.Properties.Resources.App_AppDomain_UnhandledException_Unknown_exception_);
            }

            HandleExceptionOnMainThread(exception, e.IsTerminating);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleExceptionOnMainThread(e.Exception, false);
        }

        private static void HandleExceptionOnMainThread(Exception exception, bool isTerminating)
        {
            var control = (Control) gui.MainWindow.PropertyGrid;

            if (control != null && control.InvokeRequired)
            {
                // Invoke executes a delegate on the thread that owns _MainForms's underlying window handle.
                control.Invoke(new ExceptionDelegate(HandleException), new object[]
                {
                    exception,
                    isTerminating
                });
            }
            else
            {
                HandleException(exception, isTerminating);
            }
        }

        private static void HandleException(Exception exception, bool isTerminating)
        {
            if (gui != null && gui.MainWindow != null)
            {
                using (var exceptionDialog = new ExceptionDialog(gui.MainWindow, gui, exception)
                {
                    OpenLogClicked = () =>
                    {
                        if (gui.ApplicationCommands != null)
                        {
                            gui.ApplicationCommands.OpenLogFileExternal();
                        }
                    }
                })
                {
                    if (exceptionDialog.ShowDialog() == DialogResult.OK)
                    {
                        Restart();

                        return;
                    }
                }
            }

            Environment.Exit(1);
        }

        private static void Restart()
        {
            Process.Start(typeof(App).Assembly.Location, argumentWaitForProcess + Process.GetCurrentProcess().Id);
            Environment.Exit(1);
        }

        /// <summary>
        /// Parses the process' start-up parameters.
        /// </summary>
        /// <param name="arguments">List of start-up parameters.</param>
        private static void ParseArguments(IEnumerable<string> arguments)
        {
            var argumentWaitForProcessRegex = new Regex("^" + argumentWaitForProcess + @"(?<processId>\d+)$", RegexOptions.IgnoreCase);
            foreach (var arg in arguments)
            {
                var match = argumentWaitForProcessRegex.Match(arg);
                if (match.Success)
                {
                    var pid = int.Parse(match.Groups["processId"].Value);
                    if (pid > 0)
                    {
                        waitForProcessId = pid;
                        break;
                    }
                }

                if (ParseFileArgument(arg))
                {
                    break;
                }
            }
        }

        private static void SetLanguage()
        {
            var language = ConfigurationManager.AppSettings["language"];
            if (language != null)
            {
                var localMachineDateTimeFormat = (DateTimeFormatInfo) Thread.CurrentThread.CurrentCulture.DateTimeFormat.Clone();
                localMachineDateTimeFormat.DayNames = CultureInfo.InvariantCulture.DateTimeFormat.DayNames;
                localMachineDateTimeFormat.MonthNames = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames;
                localMachineDateTimeFormat.AbbreviatedDayNames = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedDayNames;
                localMachineDateTimeFormat.AbbreviatedMonthGenitiveNames = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthGenitiveNames;
                localMachineDateTimeFormat.AbbreviatedMonthNames = CultureInfo.InvariantCulture.DateTimeFormat.AbbreviatedMonthNames;

                var cultureInfo = new CultureInfo(language)
                {
                    NumberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat,
                    DateTimeFormat = localMachineDateTimeFormat
                };

                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
            }
        }
    }
}