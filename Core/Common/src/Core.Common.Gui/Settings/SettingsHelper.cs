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
using System.IO;
using System.Reflection;
using Core.Common.Utils.Reflection;

namespace Core.Common.Gui.Settings
{
    /// <summary>
    /// Class that defines helper methods related to user settings.
    /// </summary>
    public static class SettingsHelper
    {
        /// <summary>
        /// Initializes the <see cref="SettingsHelper"/> static properties.
        /// </summary>
        static SettingsHelper()
        {
            //set defaults based on executing assembly
            var info = AssemblyUtils.GetExecutingAssemblyInfo();
            ApplicationName = info.Product;
            ApplicationVersion = info.Version;
            ApplicationCompany = info.Company;
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
        /// Gets the company that released the application.
        /// </summary>
        public static string ApplicationCompany { get; private set; }

        /// <summary>
        /// Gets the application local user settings directory.
        /// </summary>
        /// <returns>Directory path where the user settings can be found.</returns>
        public static string GetApplicationLocalUserSettingsDirectory()
        {
            var localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyInfo = AssemblyUtils.GetAssemblyInfo(executingAssembly);
            var companySettingsDirectoryPath = Path.Combine(localSettingsDirectoryPath, assemblyInfo.Company);

            var appSettingsDirectoryPath = Path.Combine(companySettingsDirectoryPath, ApplicationName + " " + ApplicationVersion);

            if (!Directory.Exists(appSettingsDirectoryPath))
            {
                Directory.CreateDirectory(appSettingsDirectoryPath);
            }

            return appSettingsDirectoryPath;
        }
    }
}