// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Core.Common.Util;
using Core.Common.Util.Settings;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Helpers;
using Core.Gui.Settings;
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
using Riskeer.Integration.Plugin.Handlers;
using Riskeer.MacroStabilityInwards.Plugin;
using Riskeer.Migration;
using Riskeer.Piping.Plugin;
using Riskeer.StabilityPointStructures.Plugin;
using Riskeer.StabilityStoneCover.Plugin;
using Riskeer.Storage.Core;
using Riskeer.WaveImpactAsphaltCover.Plugin;
using ApplicationResources = Application.Riskeer.Properties.Resources;

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

        private static string fileToOpen = string.Empty;

        private readonly ILog log;
        private GuiCore gui;

        private delegate void ExceptionDelegate(Exception exception);

        /// <summary>
        /// Creates a new instance of <see cref="App"/>.
        /// </summary>
        public App()
        {
            LogConfigurator.Initialize();

            log = LogManager.GetLogger(typeof(App));

            SettingsHelper.Instance = new RiskeerSettingsHelper();
            SetLanguage();

            string userDisplay = UserDisplay();
            log.Info(string.Format(ApplicationResources.App_Starting_Riskeer_version_0_by_user_0,
                                   SettingsHelper.Instance.ApplicationVersion,
                                   userDisplay));
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            ParseArguments(e.Args);

            DeleteOldLogFiles();

            Resources.Add(SystemParameters.MenuPopupAnimationKey, PopupAnimation.None);

            var settings = new GuiCoreSettings
            {
                ApplicationName = "Riskeer",
                ApplicationIcon = ApplicationResources.Riskeer,
                SupportHeader = ApplicationResources.SupportHeader,
                SupportText = ApplicationResources.SupportText,
                SupportWebsiteAddressUrl = "https://iplo.nl/contact/",
                SupportPhoneNumber = "088-7970790",
                ManualFilePath = "Gebruikershandleiding Riskeer 21.1.1.pdf",
                MadeByBitmapImage = new BitmapImage(new Uri("pack://application:,,,/Resources/MadeBy.png"))
            };

            var mainWindow = new MainWindow();
            var projectMigrator = new ProjectMigrator(new DialogBasedInquiryHelper(mainWindow));
            var assessmentSectionFromFileHandler = new AssessmentSectionFromFileHandler(mainWindow);
            var projectFactory = new RiskeerProjectFactory(() => assessmentSectionFromFileHandler.GetAssessmentSectionFromFile());

            gui = new GuiCore(mainWindow, new StorageSqLite(), projectMigrator, projectFactory, settings)
            {
                Plugins =
                {
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
                    new MacroStabilityInwardsPlugin()
                }
            };

            RunRiskeer();
        }

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

        private void AppDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = e.ExceptionObject as Exception ?? new Exception(ApplicationResources.App_Unhandled_exception);

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
            log.Error(ApplicationResources.App_Unhandled_exception, exception);

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
        /// Deletes the old log files.
        /// </summary>
        private static void DeleteOldLogFiles()
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
        /// Parses the start-up parameters.
        /// </summary>
        /// <param name="arguments">The start-up parameters.</param>
        private static void ParseArguments(IEnumerable<string> arguments)
        {
            foreach (string arg in arguments)
            {
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