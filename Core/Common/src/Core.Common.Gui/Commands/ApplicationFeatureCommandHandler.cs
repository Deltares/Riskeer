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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.PropertyGridView;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Selection;
using Core.Common.Gui.Settings;

using log4net;
using log4net.Appender;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// This class provides concrete implementations for <see cref="IApplicationFeatureCommands"/>.
    /// </summary>
    public class ApplicationFeatureCommandHandler : IApplicationFeatureCommands
    {
        private readonly IPropertyResolver propertyResolver;
        private readonly IMainWindow mainWindow;
        private readonly IApplicationSelection applicationSelection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFeatureCommandHandler"/> class.
        /// </summary>
        /// <param name="propertyResolver">The object responsible for finding the object properties
        /// for a given data object.</param>
        /// <param name="mainWindow">The main user interface of the application.</param>
        /// <param name="applicationSelection">The application's selection mechanism.</param>
        public ApplicationFeatureCommandHandler(IPropertyResolver propertyResolver, IMainWindow mainWindow, IApplicationSelection applicationSelection)
        {
            this.propertyResolver = propertyResolver;
            this.mainWindow = mainWindow;
            this.applicationSelection = applicationSelection;
        }

        public void ShowPropertiesFor(object obj)
        {
            mainWindow.InitPropertiesWindowAndActivate();
            applicationSelection.Selection = obj;
        }

        public bool CanShowPropertiesFor(object obj)
        {
            return propertyResolver.GetObjectProperties(obj) != null;
        }

        public void OpenLogFileExternal()
        {
            try
            {
                var fileAppender = LogManager.GetAllRepositories()
                                             .SelectMany(r => r.GetAppenders())
                                             .OfType<FileAppender>()
                                             .FirstOrDefault();
                if (fileAppender != null)
                {
                    var logFile = fileAppender.File;
                    Process.Start(logFile);
                }
            }
            catch (Exception e)
            {
                if (e is Win32Exception || e is ObjectDisposedException || e is FileNotFoundException)
                {
                    MessageBox.Show(Resources.ApplicationFeatureiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file_Opening_log_file_directory_instead, Resources.ApplicationFeatureCommandHandler_OpenLogFileExternal_Unable_to_open_log_file);
                    Process.Start(SettingsHelper.GetApplicationLocalUserSettingsDirectory());
                }
                else
                {
                    // Undocumented exception -> Fail Fast!
                    throw;
                }
            }
        }
    }
}