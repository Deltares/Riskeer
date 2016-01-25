using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Properties;
using Core.Common.Utils;
using log4net;
using log4net.Appender;

namespace Core.Common.Gui
{
    /// <summary>
    /// This class provides concrete implementations for <see cref="IApplicationFeatureCommands"/>.
    /// </summary>
    public class ApplicationFeatureCommandHandler : IApplicationFeatureCommands
    {
        private readonly IGui gui;

        public ApplicationFeatureCommandHandler(IGui gui)
        {
            this.gui = gui;
        }

        /// <summary>
        /// Makes the properties window visible and updates the <see cref="IGui.Selection"/> to the
        /// given <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object for which to show its properties.</param>
        public void ShowPropertiesFor(object obj)
        {
            ((MainWindow)gui.MainWindow).InitPropertiesWindowAndActivate();
            gui.Selection = obj;
        }

        public bool CanShowPropertiesFor(object obj)
        {
            return gui.PropertyResolver.GetObjectProperties(obj) != null;
        }

        public void OpenLogFileExternal()
        {
            bool logFileOpened = false;

            try
            {
                var fileAppender =
                    LogManager.GetAllRepositories().SelectMany(r => r.GetAppenders()).OfType
                        <FileAppender>().FirstOrDefault();
                if (fileAppender != null)
                {
                    var logFile = fileAppender.File;
                    Process.Start(logFile);
                    logFileOpened = true;
                }
            }
            catch (Exception) { }

            if (!logFileOpened)
            {
                MessageBox.Show(Resources.GuiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file_Opening_log_file_directory_instead, Resources.GuiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file);
                Process.Start(SettingsHelper.GetApplicationLocalUserSettingsDirectory());
            }
        }
    }
}