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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;

namespace Core.Common.Gui.TestUtil.Settings
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
            ExpectedApplicationLocalUserSettingsDirectory = TestHelper.GetScratchPadPath();
            ApplicationName = string.Empty;
            ApplicationVersion = string.Empty;
        }

        /// <summary>
        /// Sets the <see cref="ApplicationName"/>.
        /// </summary>
        public string ExpectedApplicationName
        {
            set
            {
                ApplicationName = value;
            }
        }

        /// <summary>
        /// Sets the <see cref="ApplicationVersion"/>.
        /// </summary>
        public string ExpectedApplicationVersion
        {
            set
            {
                ApplicationVersion = value;
            }
        }

        /// <summary>
        /// Gets or sets the directory to use in <see cref="GetApplicationLocalUserSettingsDirectory"/>.
        /// </summary>
        public string ExpectedApplicationLocalUserSettingsDirectory { private get; set; }

        public string ApplicationName { get; private set; }

        public string ApplicationVersion { get; private set; }

        public string GetApplicationLocalUserSettingsDirectory(params string[] subPath)
        {
            var directorypath = new List<string>
            {
                ExpectedApplicationLocalUserSettingsDirectory
            };

            if (subPath != null)
            {
                directorypath.AddRange(subPath.ToList());
            }

            string settingsDirectoryPath = Path.Combine(directorypath.ToArray());

            if (Directory.Exists(settingsDirectoryPath))
            {
                return settingsDirectoryPath;
            }

            try
            {
                Directory.CreateDirectory(settingsDirectoryPath);
            }
            catch (Exception e)
            {
                var message = $"Unable to create '{settingsDirectoryPath}'.";
                throw new IOException(message, e);
            }
            return settingsDirectoryPath;
        }
    }
}