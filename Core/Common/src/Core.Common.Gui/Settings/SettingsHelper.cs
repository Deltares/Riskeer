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
using System.Globalization;
using System.IO;
using Core.Common.Gui.Properties;
using Core.Common.Utils.Reflection;

namespace Core.Common.Gui.Settings
{
    /// <summary>
    /// Class that defines helper methods related to user settings.
    /// </summary>
    public static class SettingsHelper
    {
        private static readonly string localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// Initializes the <see cref="SettingsHelper"/> static properties.
        /// </summary>
        static SettingsHelper()
        {
            //set defaults based on executing assembly
            var info = AssemblyUtils.GetExecutingAssemblyInfo();
            ApplicationName = info.Product;
            ApplicationVersion = info.Version;
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public static string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public static string ApplicationVersion { get; private set; }

        /// <summary>
        /// Gets the application local user settings directory.
        /// </summary>
        /// <param name="postfix">The postfix path to use after the local application data folder (if any).</param>
        /// <returns>Directory path where the user settings can be found.</returns>
        /// <exception cref="IOException">Thrown when the application local user settings directory could not be created.</exception>
        public static string GetApplicationLocalUserSettingsDirectory(string postfix)
        {
            var appSettingsDirectoryPath = string.IsNullOrWhiteSpace(postfix) ? localSettingsDirectoryPath : Path.Combine(localSettingsDirectoryPath, postfix);

            if (!Directory.Exists(appSettingsDirectoryPath))
            {
                try
                {
                    Directory.CreateDirectory(appSettingsDirectoryPath);
                }
                catch (Exception e)
                {
                    if (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
                    {
                        var message = string.Format(CultureInfo.CurrentCulture,
                                                    Resources.SettingsHelper_GetApplicationLocalUserSettingsDirectory_Folder_0_Cannot_be_created,
                                                    appSettingsDirectoryPath);
                        throw new IOException(message, e);
                    }
                    throw;
                }
            }
            return appSettingsDirectoryPath;
        }
    }
}