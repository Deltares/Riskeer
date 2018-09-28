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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Settings;

namespace Core.Common.Util.TestUtil.Settings
{
    /// <summary>
    /// A <see cref="TestSettingsHelper"/> implementation suitable for unit testing purposes.
    /// </summary>
    public class TestSettingsHelper : ISettingsHelper
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestSettingsHelper"/>.
        /// </summary>
        /// <seealso cref="UseCustomSettingsHelper"/>
        public TestSettingsHelper()
        {
            ApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath();
            CommonDocumentsDirectory = TestHelper.GetScratchPadPath();
            TempPath = TestHelper.GetScratchPadPath();
            ApplicationName = string.Empty;
            ApplicationVersion = string.Empty;
        }

        /// <summary>
        /// Gets or sets the directory to use in <see cref="GetApplicationLocalUserSettingsDirectory"/>.
        /// </summary>
        public string ApplicationLocalUserSettingsDirectory { private get; set; }

        /// <summary>
        /// Gets or sets the directory to use in <see cref="GetCommonDocumentsDirectory"/>.
        /// </summary>
        public string CommonDocumentsDirectory { private get; set; }

        /// <summary>
        /// Gets or sets the directory to use in <see cref="GetLocalUserTemporaryDirectory"/>.
        /// </summary>
        public string TempPath { private get; set; }

        public string ApplicationName { get; private set; }

        public string ApplicationVersion { get; private set; }

        /// <summary>
        /// Sets the <see cref="ApplicationName"/>.
        /// </summary>
        public void SetApplicationName(string value)
        {
            ApplicationName = value;
        }

        /// <summary>
        /// Sets the <see cref="ApplicationVersion"/>.
        /// </summary>
        public void SetApplicationVersion(string value)
        {
            ApplicationVersion = value;
        }

        public string GetApplicationLocalUserSettingsDirectory(params string[] subPath)
        {
            return GetFullPath(ApplicationLocalUserSettingsDirectory, subPath);
        }

        public string GetCommonDocumentsDirectory(params string[] subPath)
        {
            return GetFullPath(CommonDocumentsDirectory, subPath);
        }

        public string GetLocalUserTemporaryDirectory()
        {
            return TempPath;
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