// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationFeatureCommandHandler"/> class.
        /// </summary>
        /// <param name="propertyResolver">The object responsible for finding the object properties
        /// for a given data object.</param>
        /// <param name="mainWindow">The main user interface of the application.</param>
        public ApplicationFeatureCommandHandler(IPropertyResolver propertyResolver, IMainWindow mainWindow)
        {
            this.propertyResolver = propertyResolver;
            this.mainWindow = mainWindow;
        }

        public void ShowPropertiesForSelection()
        {
            mainWindow.InitPropertiesWindowAndActivate();
        }

        public bool CanShowPropertiesFor(object obj)
        {
            return propertyResolver.GetObjectProperties(obj) != null;
        }

        public void OpenLogFileExternal()
        {
            FileAppender fileAppender = LogManager.GetAllRepositories()
                                                  .SelectMany(r => r.GetAppenders())
                                                  .OfType<FileAppender>()
                                                  .FirstOrDefault();
            if (string.IsNullOrWhiteSpace(fileAppender?.File))
            {
                return;
            }

            TryOpenLogFileExternal(fileAppender.File);
        }

        private static void TryOpenLogFileExternal(string logFile)
        {
            string logFolderPath = Path.GetDirectoryName(logFile);

            try
            {
                Process.Start(logFile);
            }
            catch (Exception e)
            {
                if (!string.IsNullOrWhiteSpace(logFolderPath) && (e is Win32Exception || e is ObjectDisposedException || e is FileNotFoundException))
                {
                    MessageBox.Show(Resources.ApplicationFeatureiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file_Opening_log_file_directory_instead,
                                    Resources.ApplicationFeatureCommandHandler_OpenLogFileExternal_Unable_to_open_log_file);
                    Process.Start(logFolderPath);
                    return;
                }

                // Undocumented exception -> Fail Fast!
                throw;
            }
        }
    }
}