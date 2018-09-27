// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Util.Reflection;

namespace Core.Common.Util.Settings
{
    /// <summary>
    /// Class that defines helper methods related to user settings.
    /// </summary>
    public class SettingsHelper : ISettingsHelper
    {
        private static ISettingsHelper instance;
        private readonly string localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private readonly string commonDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

        /// <summary>
        /// Creates a new instance of <see cref="SettingsHelper"/>.
        /// </summary>
        protected SettingsHelper()
        {
            // set defaults based on executing assembly
            AssemblyUtils.AssemblyInfo info = AssemblyUtils.GetExecutingAssemblyInfo();
            ApplicationName = info.Product;
            ApplicationVersion = info.Version;
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="SettingsHelper"/>.
        /// </summary>
        public static ISettingsHelper Instance
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

        public string ApplicationName { get; }

        public string ApplicationVersion { get; }

        public virtual string GetApplicationLocalUserSettingsDirectory(params string[] subPath)
        {
            return GetFullPath(localSettingsDirectoryPath, subPath);
        }

        public virtual string GetCommonDocumentsDirectory(params string[] subPath)
        {
            return GetFullPath(commonDocumentsPath, subPath);
        }

        public string GetLocalUserTemporaryDirectory()
        {
            return Path.GetTempPath();
        }

        private static string GetFullPath(string rootPath, string[] subPath)
        {
            var directorypath = new List<string>
            {
                rootPath
            };

            if (subPath != null)
            {
                directorypath.AddRange(subPath.ToList());
            }

            return Path.Combine(directorypath.ToArray());
        }
    }
}