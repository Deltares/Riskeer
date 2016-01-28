using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Properties;
using log4net;
using log4net.Appender;

namespace Core.Common.Gui
{
    /// <summary>
    /// This class provides concrete implementations for <see cref="IApplicationFeatureCommands"/>.
    /// </summary>
    public class ApplicationFeatureCommandHandler : IApplicationFeatureCommands
    {
        private readonly IPropertyResolver propertyResolver;
        private readonly IMainWindow mainWindow;
        private readonly IApplicationSelection applicationSelection;

        public ApplicationFeatureCommandHandler(IPropertyResolver propertyResolver, IMainWindow mainWindow, IApplicationSelection applicationSelection)
        {
            this.propertyResolver = propertyResolver;
            this.mainWindow = mainWindow;
            this.applicationSelection = applicationSelection;
        }

        /// <summary>
        /// Makes the properties window visible and updates the <see cref="IApplicationSelection.Selection"/> to the
        /// given <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object for which to show its properties.</param>
        public void ShowPropertiesFor(object obj)
        {
            ((MainWindow)mainWindow).InitPropertiesWindowAndActivate();
            applicationSelection.Selection = obj;
        }

        public bool CanShowPropertiesFor(object obj)
        {
            return propertyResolver.GetObjectProperties(obj) != null;
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