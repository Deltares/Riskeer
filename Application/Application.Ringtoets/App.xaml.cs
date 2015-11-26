using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using Core.Common.Controls.Swf;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Globalization;
using Core.Plugins.CommonTools;
using Core.Plugins.CommonTools.Gui;
using Core.Plugins.ProjectExplorer;
using Core.Plugins.SharpMapGis;
using Core.Plugins.SharpMapGis.Gui;
using log4net;
using Ringtoets.Integration.Plugin;
using Ringtoets.Piping.Plugin;
using MessageBox = System.Windows.MessageBox;
#if INCLUDE_DEMOPROJECT
using Ringtoets.Demo;
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

        private static string projectFilePath;

        private static Mutex singleInstanceMutex;

        private static string previousExceptionsText = "";

        private static int previousExceptionsCount;

        static App()
        {
            SetLanguageAndRegionalSettings();

            log.Info(Core.Common.Gui.Properties.Resources.App_App_Starting_Ringtoets);
        }

        public static void RunRingtoets()
        {
            log.Info(Core.Common.Gui.Properties.Resources.App_RunRingtoets_Starting_Ringtoets_Gui);

            var loaderDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var startupDirectory = Directory.GetCurrentDirectory();
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

            if (projectFilePath == null)
            {
                gui.Run();
            }
            else
            {
                var path = File.Exists(projectFilePath)
                               ? projectFilePath
                               : Path.Combine(startupDirectory, Path.GetFileName(projectFilePath));

                if (File.Exists(path))
                {
                    gui.Run(path);
                }
                else
                {
                    log.ErrorFormat(Core.Common.Gui.Properties.Resources.App_RunRingtoets_Specified_project_0_was_not_found_, projectFilePath);
                }
            }

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

            var settings = new GuiCoreSettings
            {
                StartPageUrl = "http://www.helpdeskwater.nl",
                SupportEmailAddress = "support@deltaressystems.nl",
                SupportPhoneNumber = "+31 (0)88 335 8100",
                Copyright = "© Deltares 2015",
                LicenseDescription = "Free",
                MainWindowTitle = "Ringtoets",
                LicenseFilePath = "License.rtf",
                ManualFilePath = "Ringtoets_Manual.pdf"
            };

            gui = new RingtoetsGui(settings)
            {
                Plugins =
                {
                    new ProjectExplorerGuiPlugin(),
                    new CommonToolsGuiPlugin(),
                    new SharpMapGisGuiPlugin(),
                    new RingtoetsGuiPlugin(),
                    new PipingGuiPlugin()
#if INCLUDE_DEMOPROJECT
                    , new DemoProjectGuiPlugin()
#endif
                }
            };

            gui.ApplicationCore.AddPlugin(new CommonToolsApplicationPlugin());
#if INCLUDE_DEMOPROJECT
            gui.ApplicationCore.AddPlugin(new SharpMapGisApplicationPlugin());
#endif
            gui.ApplicationCore.AddPlugin(new RingtoetsApplicationPlugin());
            gui.ApplicationCore.AddPlugin(new PipingApplicationPlugin());

            var mainWindow = new MainWindow(gui);
            gui.MainWindow = mainWindow;

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
            var dialog = new ExceptionDialog(exception, previousExceptionsText);

            if (isTerminating)
            {
                dialog.ContinueButton.Visible = false;
            }

            dialog.RestartClicked += delegate
            {
                gui.SkipDialogsOnExit = true;
                Restart();
            };

            dialog.ExitClicked += delegate
            {
                gui.SkipDialogsOnExit = true;
                Environment.Exit(1);
            };

            dialog.ContinueClicked += delegate
            {
                previousExceptionsCount++;
                var s = previousExceptionsText;
                previousExceptionsText =
                    string.Format(Core.Common.Gui.Properties.Resources.App_HandleException_0_,
                                  previousExceptionsCount,
                                  dialog.ExceptionText,
                                  s);

                return;
            };

            dialog.OpenLogClicked += delegate
            {
                if (gui != null && gui.CommandHandler != null)
                {
                    gui.CommandHandler.OpenLogFileExternal();
                }
            };

            var mainWindow = gui.MainWindow as Form;
            if (mainWindow != null && mainWindow.IsHandleCreated)
            {
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(mainWindow); // show dialog at the center of main window
            }
            else
            {
                dialog.ShowDialog();
            }
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

        private static void SetLanguageAndRegionalSettings()
        {
            var language = ConfigurationManager.AppSettings["language"];
            if (language != null)
            {
                RegionalSettingsManager.Language = language;
            }

            if (Settings.Default.Properties.Count > 0)
            {
                var realNumberFormat = Settings.Default["realNumberFormat"];
                if (realNumberFormat != null)
                {
                    RegionalSettingsManager.RealNumberFormat = (string)realNumberFormat;
                }
            }
        }
    }
}