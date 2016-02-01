// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

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