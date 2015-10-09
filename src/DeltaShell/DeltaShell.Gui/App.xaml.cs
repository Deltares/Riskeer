using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Core.Workflow;
using DelftTools.Shell.Gui.Forms;
using DeltaShell.Core;
using DeltaShell.Gui.Properties;
using log4net;
using NDesk.Options;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DeltaShell.Gui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private delegate void ExceptionDelegate(Exception exception, bool isTerminating);

        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private static DeltaShellGui gui;

        private static int waitForProcessId = -1; // start Delta Shell after this process will exit (used during restart)

        private static string projectFilePath;

        private static Mutex singleInstanceMutex;

        private static string previousExceptionsText = "";

        private static int previousExceptionsCount;
        private static string runCommand;
        private static string runActivity;
        private static string runScriptFilePath;

        static App()
        {
            DeltaShellApplication.SetLanguageAndRegionalSettions(Settings.Default);

            log.Info(Gui.Properties.Resources.App_App_Starting_Delta_Shell____);
        }

        public static void RunDeltaShell(IMainWindow mainWindow)
        {
            log.Info(Gui.Properties.Resources.App_RunDeltaShell_Starting_Delta_Shell_Gui____);

            var loaderDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var startupDirectory = Directory.GetCurrentDirectory();
            if (loaderDirectory != null)
            {
                Environment.CurrentDirectory = loaderDirectory;
            }

            gui = new DeltaShellGui
            {
                MainWindow = mainWindow
            };

            //gui.Application.ProjectRepositoryFactory.SpeedUpSessionCreationUsingParallelThread = true;
            //gui.Application.ProjectRepositoryFactory.SpeedUpConfigurationCreationUsingCaching = true;
            //gui.Application.ProjectRepositoryFactory.ConfigurationCacheDirectory = gui.Application.GetUserSettingsDirectoryPath();
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
                    log.ErrorFormat(Gui.Properties.Resources.App_RunDeltaShell_Specified_project___0___was_not_found_, projectFilePath);
                }
            }

            gui.OnMainWindowLoaded = () =>
            {
                if (runActivity != null)
                {
                    if (gui.Application.Project == null)
                    {
                        log.ErrorFormat(Gui.Properties.Resources.App_RunDeltaShell_No_project_found__load_project_first);
                        return;
                    }

                    // search project for activities specified by the argument
                    var activity =
                        gui.Application.Project.Items
                           .OfType<IActivity>()
                           .FirstOrDefault(a => a.Name == runActivity);
                    if (activity == null)
                    {
                        log.ErrorFormat(
                            Gui.Properties.Resources.App_RunDeltaShell_Activity___0___not_found_in_project__Typo__or_did_you_forget_to_load_a_project_,
                            runActivity);
                        return;
                    }

                    log.InfoFormat(Gui.Properties.Resources.App_RunDeltaShell_Starting_activity___0__, runActivity);
                    gui.Application.RunActivity(activity);
                    log.InfoFormat(Gui.Properties.Resources.App_RunDeltaShell_Activity___0___ended_with_status__1_, runActivity, activity.Status);

                    log.InfoFormat(Gui.Properties.Resources.App_RunDeltaShell_Saving_project___0__, gui.Application.Project.Name);
                    gui.Application.SaveProject();
                    // Necessary for persisting the output of the activity run (e.g. netCDF files). 
                    log.InfoFormat(Gui.Properties.Resources.App_RunDeltaShell_Saved_project___0__, gui.Application.Project.Name);
                }
            };

            // Delta Shell started, clean-up all possible memory
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
            WaitForPreviousInstanceToExit();
            if (ShutdownIfNotFirstInstance())
            {
                return;
            }

            Resources.Add(SystemParameters.MenuPopupAnimationKey, PopupAnimation.None);
            ParseArguments(e.Args);
            StartupUri = new Uri("Forms/MainWindow/MainWindow.xaml", UriKind.Relative);
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
                        MessageBox.Show(Gui.Properties.Resources.App_ShutdownIfNotFirstInstance_Cannot_start_multiple_instances_of_DeltaShell__Please_close_the_other_instance_first_);
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

        private static void WaitForPreviousInstanceToExit()
        {
            // wait until previous version of Delta Shell has exited
            if (waitForProcessId == -1)
            {
                return;
            }

            try
            {
                var process = Process.GetProcessById(waitForProcessId);
                process.WaitForExit();
            }
            catch {}
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

                var mutexName = string.Format("DeltaShell-single-instance-mutex-{0}-{1}", Environment.UserName,
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
                exception = new Exception(Gui.Properties.Resources.App_AppDomain_UnhandledException_Unknown_exception_);
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
                    string.Format(Gui.Properties.Resources.App_HandleException_,
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
            Process.Start(typeof(App).Assembly.Location, "--wait-for-process=" + Process.GetCurrentProcess().Id);
            Environment.Exit(1);
        }

        private static void ParseArguments(IEnumerable<string> arguments)
        {
            var p = new OptionSet
            {
                {
                    "<>", delegate(string projectPath)
                    {
                        if (Path.GetExtension(projectPath) == ".dsproj")
                        {
                            projectFilePath = projectPath;
                        }
                    }
                },
                {
                    "r|run-activity=", Gui.Properties.Resources.App_ParseArguments_Run_activity_or_model_available_in_the_project_, v => runActivity = v
                },
                {
                    "f|run-file=", Gui.Properties.Resources.App_ParseArguments_Run_script_from_file, v => runScriptFilePath = Path.GetFullPath(v)
                },
                {
                    "c|run-command=", Gui.Properties.Resources.App_ParseArguments_Run_specified_command__Python__, v => runCommand = v
                },
                {
                    "p|project=", delegate(string projectPath) { projectFilePath = projectPath; }
                },
                {
                    "w|wait-for-process=", delegate(string pid) { waitForProcessId = int.Parse(pid); }
                }
            };
            p.Parse(arguments);
        }
    }
}