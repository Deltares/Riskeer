// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.Appenders;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Settings;
using Core.Common.Util;
using Core.Common.Util.Settings;
using Core.Plugins.Chart;
using Core.Plugins.CommonTools;
using Core.Plugins.Map;
using Core.Plugins.ProjectExplorer;
using log4net;
using log4net.Appender;
using Riskeer.ClosingStructures.Plugin;
using Riskeer.DuneErosion.Plugin;
using Riskeer.GrassCoverErosionInwards.Plugin;
using Riskeer.GrassCoverErosionOutwards.Plugin;
using Riskeer.HeightStructures.Plugin;
using Riskeer.Integration.Data;
using Riskeer.Integration.Forms;
using Riskeer.Integration.Plugin;
using Riskeer.MacroStabilityInwards.Plugin;
using Riskeer.Migration;
using Riskeer.Piping.Plugin;
using Riskeer.StabilityPointStructures.Plugin;
using Riskeer.StabilityStoneCover.Plugin;
using Riskeer.Storage.Core;
using Riskeer.WaveImpactAsphaltCover.Plugin;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using MessageBox = System.Windows.MessageBox;
#if DEVELOPMENT
using Demo.Riskeer.GUIs;

#endif

namespace Application.Riskeer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        // Start application after this process will exit (used during restart)
        private const string argumentWaitForProcess = "--wait-for-process=";

        private const int numberOfDaysToKeepLogFiles = 30;

        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private GuiCore gui;
        private static int waitForProcessId = -1;
        private static string fileToOpen = string.Empty;

        private static Mutex singleInstanceMutex;

        static App()
        {
            SettingsHelper.Instance = new RiskeerSettingsHelper();
            SetLanguage();

            string userDisplay = UserDisplay();
            log.Info(string.Format(CoreCommonGuiResources.App_Starting_Riskeer_version_0_by_user_0,
                                   SettingsHelper.Instance.ApplicationVersion,
                                   userDisplay));
        }

        private delegate void ExceptionDelegate(Exception exception);

        protected override void OnExit(ExitEventArgs e)
        {
            singleInstanceMutex?.ReleaseMutex();
            base.OnExit(e);
        }

        /// <summary>
        /// Runs the main Riskeer application.
        /// </summary>
        private void RunRiskeer()
        {
            string loaderDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

            // Riskeer started, clean-up all possible memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
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
                MainWindowTitle = "Riskeer",
                ManualFilePath = "..\\Gebruikershandleiding Riskeer 19.1.1.pdf"
            };
            var mainWindow = new MainWindow();
            var projectMigrator = new ProjectMigrator(new DialogBasedInquiryHelper(mainWindow));
            gui = new GuiCore(mainWindow, new StorageSqLite(), projectMigrator, new RiskeerProjectFactory(), settings)
            {
                Plugins =
                {
                    new ProjectExplorerPlugin(),
                    new CommonToolsPlugin(),
                    new RiskeerPlugin(),
                    new ClosingStructuresPlugin(),
                    new StabilityPointStructuresPlugin(),
                    new WaveImpactAsphaltCoverPlugin(),
                    new GrassCoverErosionInwardsPlugin(),
                    new GrassCoverErosionOutwardsPlugin(),
                    new PipingPlugin(),
                    new HeightStructuresPlugin(),
                    new StabilityStoneCoverPlugin(),
                    new DuneErosionPlugin(),
                    new MacroStabilityInwardsPlugin(),
                    new ChartPlugin(),
                    new MapPlugin()
#if DEVELOPMENT
                    ,
                    new DemoProjectPlugin()
#endif
                }
            };

            RunRiskeer();

            mainWindow.Show();
        }

        private static bool ParseFileArgument(string potentialPath)
        {
            if (potentialPath.Length > 0)
            {
                try
                {
                    IOUtils.ValidateFilePath(potentialPath);
                    fileToOpen = potentialPath;
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// <code>app.config</code> has been configured to use <see cref="RiskeerUserDataFolderConverter"/>
        /// to write log files to the Riskeer user data folder. This method deletes the old log files
        /// that have been written there.
        /// </summary>
        private void DeleteOldLogFiles()
        {
            try
            {
                IOUtils.DeleteOldFiles(GetLogFileDirectory(), "*.log", numberOfDaysToKeepLogFiles);
            }
            catch (Exception e)
            {
                if (e is ArgumentException || e is IOException)
                {
                    return;
                }

                throw;
            }
        }

        private bool ShutdownIfNotFirstInstance()
        {
            var hasMutex = false;

            try
            {
                if (!AcquireSingleInstancePerUserMutex())
                {
                    MessageBox.Show(CoreCommonGuiResources.App_ShutdownIfNotFirstInstance_Cannot_start_multiple_instances_of_Riskeer_Please_close_the_other_instance_first);
                    Shutdown(1);
                    return true; //done here
                }

                hasMutex = true;
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
            // Wait until previous version of Riskeer has exited
            if (waitForProcessId == -1)
            {
                return;
            }

            try
            {
                Process process = Process.GetProcessById(waitForProcessId);
                process.WaitForExit();
            }
            catch
            {
                //Ignored, because the process may already be closed
            }
        }

        private static bool AcquireSingleInstancePerUserMutex()
        {
            bool createdNew;

            // Include the user name in the (global) mutex to ensure we limit only the number of instances per 
            // user, not per system (essential on for example Citrix systems).
            singleInstanceMutex = new Mutex(true, $"Riskeer-single-instance-mutex-{Environment.UserName}", out createdNew);

            return createdNew;
        }

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception ?? new Exception(CoreCommonGuiResources.App_Unhandled_exception);

            HandleExceptionOnMainThread(exception);
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleExceptionOnMainThread(e.Exception);
        }

        private void HandleExceptionOnMainThread(Exception exception)
        {
            var control = (Control) gui.MainWindow.PropertyGrid;

            if (control != null && control.InvokeRequired)
            {
                // Invoke executes a delegate on the thread that owns _MainForms's underlying window handle.
                control.Invoke(new ExceptionDelegate(HandleException), exception);
            }
            else
            {
                HandleException(exception);
            }
        }

        private void HandleException(Exception exception)
        {
            log.Error(CoreCommonGuiResources.App_Unhandled_exception, exception);

            if (gui?.MainWindow != null)
            {
                using (var exceptionDialog = new ExceptionDialog(gui.MainWindow, gui, exception)
                {
                    OpenLogClicked = () => gui.ApplicationCommands?.OpenLogFileExternal()
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
            foreach (string arg in arguments)
            {
                Match match = argumentWaitForProcessRegex.Match(arg);
                if (match.Success)
                {
                    int pid = int.Parse(match.Groups["processId"].Value);
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
            string language = ConfigurationManager.AppSettings["language"];
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

        private static string UserDisplay()
        {
            try
            {
                return $"{UserPrincipal.Current.DisplayName} ({UserPrincipal.Current.SamAccountName})";
            }
            catch (SystemException)
            {
                // Cannot only catch specified exceptions, as there are some hidden exception
                // that can be thrown when calling UserPrincipal.Current.
                return Environment.UserName;
            }
        }

        private static string GetLogFileDirectory()
        {
            FileAppender fileAppender = LogManager.GetAllRepositories()
                                                  .SelectMany(r => r.GetAppenders())
                                                  .OfType<FileAppender>()
                                                  .FirstOrDefault();
            return string.IsNullOrWhiteSpace(fileAppender?.File)
                       ? string.Empty
                       : Path.GetDirectoryName(fileAppender.File);
        }
    }
}