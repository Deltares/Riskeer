﻿using System;
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
using Core.Common.Base.Plugin;
using Core.Common.Controls.Dialogs;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Plugins.CommonTools;
using Core.Plugins.DotSpatial;
using Core.Plugins.OxyPlot;
using Core.Plugins.ProjectExplorer;
using log4net;
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

        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private static RingtoetsGui gui;
        private static int waitForProcessId = -1;

        private static Mutex singleInstanceMutex;

        static App()
        {
            SetLanguage();

            log.Info(Core.Common.Gui.Properties.Resources.App_App_Starting_Ringtoets);
        }

        public static void RunRingtoets()
        {
            log.Info(Core.Common.Gui.Properties.Resources.App_RunRingtoets_Starting_Ringtoets_Gui);

            var loaderDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (loaderDirectory != null)
            {
                Environment.CurrentDirectory = loaderDirectory;
            }

            //gui.ApplicationCore.ProjectRepositoryFactory.SpeedUpSessionCreationUsingParallelThread = true;
            //gui.ApplicationCore.ProjectRepositoryFactory.SpeedUpConfigurationCreationUsingCaching = true;
            //gui.ApplicationCore.ProjectRepositoryFactory.ConfigurationCacheDirectory = gui.ApplicationCore.GetUserSettingsDirectoryPath();
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);

            // handle exception from UI thread
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;

            // handle exception from all threads except UI
            AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;

            gui.Run();

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

            Resources.Add(SystemParameters.MenuPopupAnimationKey, PopupAnimation.None);

            var applicationCore = new ApplicationCore();

            applicationCore.AddPlugin(new RingtoetsApplicationPlugin());
            applicationCore.AddPlugin(new PipingApplicationPlugin());

            var settings = new GuiCoreSettings
            {
                StartPageUrl = "http://www.helpdeskwater.nl",
                SupportEmailAddress = "support@deltaressystems.nl",
                SupportPhoneNumber = "+31 (0)88 335 8100",
                Copyright = "© Deltares 2016",
                LicenseDescription = "Gratis",
                MainWindowTitle = "Ringtoets",
                LicenseFilePath = "..\\Licentie.rtf",
                ManualFilePath = "Ringtoets_Manual.pdf"
            };
            var mainWindow = new MainWindow();
            gui = new RingtoetsGui(mainWindow, new StorageSqLite(), applicationCore, settings)
            {
                Plugins =
                {
                    new ProjectExplorerGuiPlugin(),
                    new CommonToolsGuiPlugin(),
                    new RingtoetsGuiPlugin(),
                    new PipingGuiPlugin(),
                    new DotSpatialGuiPlugin()
#if INCLUDE_DEMOPROJECT
                    , new DemoProjectGuiPlugin()
                    , new OxyPlotGuiPlugin()
#endif
                }
            };

            RunRingtoets();

            mainWindow.Show();
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
                                          : "";

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
                using (var exceptionDialog = new ExceptionDialog(gui.MainWindow, exception)
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