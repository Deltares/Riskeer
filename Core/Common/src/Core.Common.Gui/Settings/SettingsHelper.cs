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
    public class SettingsHelper
    {
        private readonly string localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static SettingsHelper instance;

        /// <summary>
        /// Creates a new instance of <see cref="SettingsHelper"/>.
        /// </summary>
        private SettingsHelper()
        {
            // set defaults based on executing assembly
            AssemblyUtils.AssemblyInfo info = AssemblyUtils.GetExecutingAssemblyInfo();
            ApplicationName = info.Product;
            ApplicationVersion = info.Version;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="SettingsHelper"/>.
        /// </summary>
        public static SettingsHelper Instance
        {
            get
            {
                return instance ?? (instance = new SettingsHelper());
            }
            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public string ApplicationName { get; private set; }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public string ApplicationVersion { get; private set; }

        /// <summary>
        /// Gets the application local user settings directory.
        /// </summary>
        /// <param name="postfix">The postfix path to use after the local application data folder (if any).</param>
        /// <returns>Directory path where the user settings can be found.</returns>
        /// <exception cref="IOException">Thrown when the application local user settings directory could not be created.</exception>
        public string GetApplicationLocalUserSettingsDirectory(string postfix)
        {
            var appSettingsDirectoryPath = string.IsNullOrWhiteSpace(postfix) ? localSettingsDirectoryPath : Path.Combine(localSettingsDirectoryPath, postfix);

            if (Directory.Exists(appSettingsDirectoryPath))
            {
                return appSettingsDirectoryPath;
            }

            try
            {
                Directory.CreateDirectory(appSettingsDirectoryPath);
            }
            catch (Exception e) when (e is ArgumentException || e is IOException || e is NotSupportedException || e is UnauthorizedAccessException)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                                            Resources.SettingsHelper_GetApplicationLocalUserSettingsDirectory_Folder_0_Cannot_be_created,
                                            appSettingsDirectoryPath);
                throw new IOException(message, e);
            }
            return appSettingsDirectoryPath;
        }
    }
}